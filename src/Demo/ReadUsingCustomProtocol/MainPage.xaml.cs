using Microsoft.Maker.Serial;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
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

namespace ReadUsingCustomProtocol
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Task captureReadingsTask;

        public MainPage()
        {
            this.InitializeComponent();
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
            this.captureReadingsTask = Task.Run(this.CaptureReadings);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
