using Microsoft.Maker.Firmata;
using Microsoft.Maker.RemoteWiring;
using Microsoft.Maker.Serial;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ReadUsingFirmataProtocol
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

        private BluetoothSerial bluetooth = null;
        private RemoteDevice device = null;
        private UwpFirmata firmata = null;

        public async Task CaptureReadings()
        {
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.bluetooth = new BluetoothSerial("RNBT-9E86");
                this.firmata = new UwpFirmata();
                this.firmata.FirmataConnectionReady += Firmata_FirmataConnectionReady;
                this.firmata.StringMessageReceived += Firmata_StringMessageReceived;
                this.device = new RemoteDevice(firmata);
                this.firmata.begin(bluetooth);
                bluetooth.begin(9600, SerialConfig.SERIAL_8N1);
            });
        }

        private async void Firmata_StringMessageReceived(UwpFirmata caller, StringCallbackEventArgs argv)
        {
            var content = argv.getString();
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.ReadingTextBlock.Text = content;
            });
        }

        private async void Firmata_FirmataConnectionReady()
        {
            byte ULTRASONIC_DISTANCE_QUERY = 0x42;
            this.firmata.sendSysex(ULTRASONIC_DISTANCE_QUERY, new byte[] { 99 }.AsBuffer());
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Run(this.CaptureReadings);
        }
    }
}
