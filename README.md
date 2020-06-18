# serverless-iot-simulated-device

# Intro
This code is a very condensed view on how to connect your IoT Devices to an IoT Hub. Don't expect quality error handling. The simulator part of the code generates random telemetry which is send to your IoT Hub as soon as the connection is established.

# Setup
You can run the simulator in several modes. Each modes defines the way the simulator authenticates against the IoT Hub. To switch the mode, simply change the authType in the Program.cs file.

All of the modes require some parameters which need to be defined in environment variables. The required parameters are mentioned below under each mode.

1. The KEY mode enabled the simulator to connect via symmetric key to the IoT Hub
  - DeviceConnectionString: Taken from the IoT Hub
2. The X509 mode uses certificates to authenticate with the Hub.
  - DeviceId: The DeviceId as defined in the IoT Hub
  - IotHubName: The HubName (not the hostname)
  - CertPath: The absolute path of your PFX file
  - CertPwd: The cert password. If you use the totorial below, the password is 1234.
3. The DPSX509 mode uses certificates to creates a provisioning via the DPS and connects afterwards to the IoT Hub
  - CertPath and CertPwd as above
  - DPSScope taken from the DPS

# Important links

DPS device sample code: https://github.com/Azure-Samples/azure-iot-samples-csharp/tree/master/provisioning/Samples/device

X509 for IoT getting started: https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-security-x509-get-started

Create demo CA certificates for IoT end Iot Edge devices: https://github.com/Azure/azure-iot-sdk-c/blob/master/tools/CACertificates/CACertificateOverview.md
