using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using lib_NMEA_GPS;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace appAdfUltGPS.ViewModels
{
    public class ConfigurationPageViewModel : INotifyPropertyChanged, ICleanUpViewModel
    {
        private NMEA_GPS gpsDevice;

        #region BindingProperties
        private bool _connected;
        public bool Connected
        {
            get { return _connected; }
            set { Set(ref _connected, value); }
        }
        private string _serialData;
        public string SerialData
        {
            get { return _serialData; }
            set { Set(ref _serialData, value); }
        }
        private string _messageCount;
        public string MessageCount
        {
            get { return _messageCount; }
            set { Set(ref _messageCount, value); }
        }
        private string _messageLength;
        public string MessageLength
        {
            get { return _messageLength; }
            set { Set(ref _messageLength, value); }
        }
        #endregion

        public ConfigurationPageViewModel()
        {
            gpsDevice = (App.Current as App).GPSdevice;
            // Subscribe to the SerialDataReceivedEvent
            if (gpsDevice != null)
            {
                gpsDevice.SerialDataReceivedEvent += SerialDataReceivedListener;
            }
            // Initialize the connection state from the gpsDevice
            Connected = gpsDevice.Connectted;
        }

        public void CleanUpViewModel()
        {   // Un-Subscribe from the gpsDevice.SerialDataReceivedEvent and release the reference to the gpsDevice object
            // so that the garbage collector can re-claim the memory allocated for instances of this class.
            if (gpsDevice != null)
                gpsDevice.SerialDataReceivedEvent -= SerialDataReceivedListener;
            gpsDevice = null;
        }

        //TODO link this to a select box for the com port 
        public async void ConnectButtonClicked()
        {
            // TODO - Disable Connect Button
            if (!Connected)
            {
                Connected = await gpsDevice.Connect("UART0");
            }
            
        }

        public void DisconnectButtonClicked()
        {
            if(Connected)
                Connected = gpsDevice.DisConnect();
        }

        private void SerialDataReceivedListener(object sender, SerialDataEventArgs e)
        {
            SerialData = e.Data;
            MessageCount = e.MessageCount.ToString();
            MessageLength = e.MessageLength.ToString();
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
                return false;
            storage = value;
            RaisePropertyChanged(propertyName);
            return true;
        }
    }
    public class BooleanToConnectButtonColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value is bool && (bool)value) ? new SolidColorBrush(Windows.UI.Colors.LightGray) : new SolidColorBrush(Windows.UI.Colors.Green);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new Exception("Not Implemented");
        }
    }
    public class BooleanToDisconnectButtonColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value is bool && (bool)value) ? new SolidColorBrush(Windows.UI.Colors.OrangeRed) : new SolidColorBrush(Windows.UI.Colors.LightGray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new Exception("Not Implemented");
        }
    }
}