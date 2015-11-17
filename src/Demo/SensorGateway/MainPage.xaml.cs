using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SensorGateway
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        
        }

        private Random generator = new Random();

        private double distance = 24;
        private double temperature = 19;
        private double humidity = 50;

        private async Task SendTelemetry()
        {
            var deviceId = "ef7ee566-d7c9-4662-9d16-795c43c929b7";

            var client = DeviceClient.Create(
                "msauignite.azure-devices.net",
                new DeviceAuthenticationWithRegistrySymmetricKey(
                    deviceId,
                    "HX5oWKUl2AfGqzDez0ArFw=="
                    ),
                TransportType.Http1
                );

            var deviceMetadata = new
            {
                ObjectType = "DeviceInfo",
                IsSimulatedDevice = 0,
                Version = "1.0",
                DeviceProperties = new
                {
                    DeviceID = deviceId,
                    HubEnabledState = 1,
                    CreatedTime = DateTime.UtcNow,
                    DeviceState = "normal",
                    UpdatedTime = DateTime.UtcNow,
                    Manufacturer = "Microsoft",
                    ModelNumber = "RDFY-X9000-001",
                    SerialNumber = "X9000-001-ABC123",
                    FirmwareVersion = "1.10",
                    Platform = "Windows IoT Core",
                    Processor = "ARM",
                    InstalledRAM = "64 MB",
                    Latitude = -28.0284672,
                    Longitude = 153.427156,
                },
                Commands = new []
                {
                    new {
                        Name = "LockPump",
                        Parameters = new []
                        {
                            new { Name = "Minutes", Type = "double"}
                        }
                    },
                    new {
                        Name = "UnlockPump",
                        Parameters = new []
                        {
                            new { Name = "Minutes", Type = "double"}
                        }
                    }
                }
            };

            var deviceMetadataString = JsonConvert.SerializeObject(deviceMetadata);
            var deviceMetadataBytes = Encoding.ASCII.GetBytes(deviceMetadataString);
            var deviceMetadataMessage = new Message(deviceMetadataBytes);
            await client.SendEventAsync(deviceMetadataMessage);

            while (true)
            {
                this.distance = this.distance + this.generator.Next(-5, 5);
                this.temperature = this.temperature + this.generator.Next(-1, 1);
                this.humidity = this.temperature + this.generator.Next(-1, 1);

                var reading = new
                {
                    DeviceId = deviceId,
                    Distance = distance,
                    Humidity = humidity,
                    Temperature = temperature,
                    ExternalTemperature = temperature
                };

                var readingString = JsonConvert.SerializeObject(reading);
                var readingBytes = Encoding.ASCII.GetBytes(readingString);

                var readingMessage = new Message(readingBytes);
                await client.SendEventAsync(readingMessage);
                await Task.Delay(10000);
            }
        }

        private async Task CaptureReadings()
        {
            var devices = await DeviceInformation.FindAllAsync(
                RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort)
                );

            var device = devices.Single((x) => x.Name == "RNBT-9A57");

            RfcommDeviceService service = null;

            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                service = RfcommDeviceService.FromIdAsync(device.Id).AsTask().Result;
            });

            using (var socket = new StreamSocket())
            {
                await socket.ConnectAsync(service.ConnectionHostName, service.ConnectionServiceName, SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication);
                var reader = new DataReader(socket.InputStream);

                while (true)
                {
                    var distance = await this.ReadDistanceAsync(reader);

                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        this.ReadingTextBlock.Text = distance.ToString();
                    });
                }
            }
        }

        private async Task<int> ReadDistanceAsync(DataReader reader)
        {
            var builder = new StringBuilder();

            while (true)
            {
                await reader.LoadAsync(1);
                var input = reader.ReadString(1);

                if (input == "|")
                {
                    int distance = 0;
                    int.TryParse(builder.ToString(), out distance);
                    return distance;
                }
                else
                {
                    builder.Append(input);
                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Run(this.CaptureReadings);
            Task.Run(this.SendTelemetry);
        }
    }
}
