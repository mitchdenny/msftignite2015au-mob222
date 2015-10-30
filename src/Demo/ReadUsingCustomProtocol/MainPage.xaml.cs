using Microsoft.Maker.Serial;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
        private BluetoothSerial bluetooth = new BluetoothSerial("RNBT-9E86");

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void ReadButton_Click(object sender, RoutedEventArgs e)
        {
            var devices = await DeviceInformation.FindAllAsync(
                RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort)
                );

            var device = devices.First();
            var service = await RfcommDeviceService.FromIdAsync(device.Id);
            var socket = new StreamSocket();
            await socket.ConnectAsync(service.ConnectionHostName, service.ConnectionServiceName, SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication);
            var reader = new DataReader(socket.InputStream);
            reader.InputStreamOptions = InputStreamOptions.ReadAhead;


            while (true)
            {
                var length = await reader.LoadAsync(3);
                var distance = reader.ReadString(3);
            }

        }
    }
}
