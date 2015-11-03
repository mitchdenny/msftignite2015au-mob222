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
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            var devices = await DeviceInformation.FindAllAsync(
                RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort)
                );

            var device = devices.First();
            var service = await RfcommDeviceService.FromIdAsync(device.Id);
            using (var socket = new StreamSocket())
            {
                await socket.ConnectAsync(service.ConnectionHostName, service.ConnectionServiceName, SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication);
                var reader = new DataReader(socket.InputStream);
                reader.InputStreamOptions = InputStreamOptions.ReadAhead;

                var distance = await this.ReadDistanceAsync(reader);
                this.ReadingTextBlock.Text = distance.ToString();
            }
        }

        private async Task<int> ReadDistanceAsync(DataReader reader)
        {
            var builder = new StringBuilder();
            bool preceedingBreakEncountered = false;

            while (true)
            {
                await reader.LoadAsync(1);
                var character = reader.ReadString(1);
                
                if (preceedingBreakEncountered && character == "\r")
                {
                    break;
                }
                else if (preceedingBreakEncountered && char.IsDigit(character, 0))
                {
                    builder.Append(character);
                }
                else if (!preceedingBreakEncountered && character == "\n")
                {
                    preceedingBreakEncountered = true;
                }
            }

            return int.Parse(builder.ToString());
        }
    }
}
