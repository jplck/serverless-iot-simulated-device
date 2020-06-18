using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Provisioning.Client;
using Microsoft.Azure.Devices.Provisioning.Client.Transport;

namespace SimulatedDevice
{
    class Program
    {
        enum AuthType
        {
            KEY,
            X509,
            DPSX509,
            DPSKey
        }

        private static TransportType transportType = TransportType.Amqp;
        private static AuthType authType = AuthType.DPSX509;
        private static string dpsEndpoint = "global.azure-devices-provisioning.net";


        static async Task<int> Main(string[] args)
        {
            using (var deviceClient = await SelectAuthType())
            {
                var methodHandler = new MethodHandler(deviceClient);
                await methodHandler.RunMethodHandlerAsync().ConfigureAwait(false);

                var simulator = new Simulator(deviceClient);
                await simulator.RunSimulation().ConfigureAwait(false);
            }

            return 0;
        }

        static async Task<DeviceClient> SelectAuthType()
        {
            switch (authType)
            {
                case AuthType.X509:
                    return CreateFromX509();
                case AuthType.DPSX509:
                    return await CreateForDPS(AuthType.DPSX509);
                default:
                    return CreateFromConnectionString();
            }
        }

        static DeviceClient CreateFromConnectionString()
        {
            var deviceConnectionString = Environment.GetEnvironmentVariable("DeviceConnectionString");
            var _ = deviceConnectionString ?? throw new ArgumentNullException("DeviceConnectionString", "Device connection string cannot be empty");
            return DeviceClient.CreateFromConnectionString(deviceConnectionString, transportType);
        }

        static DeviceClient CreateFromX509()
        {
            var deviceId = Environment.GetEnvironmentVariable("DeviceId");
            var hubName = Environment.GetEnvironmentVariable("IotHubName");
            var auth = new DeviceAuthenticationWithX509Certificate(deviceId, LoadCertificate());
            return DeviceClient.Create($"{hubName}.azure-devices.net", auth, transportType);
        }

        static async Task<DeviceClient> CreateForDPS(AuthType authType)
        {
            var dpsScope = Environment.GetEnvironmentVariable("DPSScope");
            var security = new SecurityProviderX509Certificate(LoadCertificate());
            var transport = new ProvisioningTransportHandlerAmqp(TransportFallbackType.TcpOnly);
            var provClient = ProvisioningDeviceClient.Create(dpsEndpoint, dpsScope, security, transport);

            DeviceRegistrationResult result = await provClient.RegisterAsync().ConfigureAwait(false);

            if (result.Status != ProvisioningRegistrationStatusType.Assigned) throw new Exception("Device could not be provisioned.");
            var auth = new DeviceAuthenticationWithX509Certificate(result.DeviceId, security.GetAuthenticationCertificate());
            return DeviceClient.Create(result.AssignedHub, auth, transportType);
        }

        static X509Certificate2 LoadCertificate()
        {
            var certPath = Environment.GetEnvironmentVariable("CertPath");
            var certPwd = Environment.GetEnvironmentVariable("CertPwd");
            return new X509Certificate2($@"{certPath}", certPwd);
        }
    }
}
