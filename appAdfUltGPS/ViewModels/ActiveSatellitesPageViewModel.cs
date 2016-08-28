using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using lib_NMEA_GPS;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace appAdfUltGPS.ViewModels
{
    /// <summary>
    /// Class that represents an item within the Active Satellites List View
    /// </summary>
    public class SatelliteLstVwItem
    {
        public bool Visible;
        public string ChannelName;
        public int SatelliteID;

        public SatelliteLstVwItem(bool visible, string name, int id)
        {
            Visible = visible;
            ChannelName = name;
            SatelliteID = id;
        }
    }

    public class ActiveSatellitesPageViewModel : INotifyPropertyChanged, ICleanUpViewModel
    {
        private NMEA_GPS gpsDevice;
        public ObservableCollection<SatelliteLstVwItem> SatViewOnChannel;

        #region BindingProperties
        private string _mode1;
        public string Mode1
        {
            get { return _mode1; }
            set { Set(ref _mode1, value); }
        }
        private string _mode2;
        public string Mode2
        {
            get { return _mode2; }
            set { Set(ref _mode2, value); }
        }
        private string _positionDOP;
        public string PositionDOP
        {
            get { return _positionDOP; }
            set { Set(ref _positionDOP, value); }
        }
        private string _horizontalDOP;
        public string HorizontalDOP
        {
            get { return _horizontalDOP; }
            set { Set(ref _horizontalDOP, value); }
        }
        private string _verticalDOP;
        public string VerticalDOP
        {
            get { return _verticalDOP; }
            set { Set(ref _verticalDOP, value); }
        }
        private string _messageCount;
        public string MessageCount
        {
            get { return _messageCount; }
            set { Set(ref _messageCount, value); }
        }
        #endregion

        public ActiveSatellitesPageViewModel()
        {
            gpsDevice = (App.Current as App).GPSdevice;
            // Create 12 'Channels' within the list box.
            if (SatViewOnChannel == null)
            {
                SatViewOnChannel = new ObservableCollection<SatelliteLstVwItem>();
                for (int i = 0; i < 12; i++)
                {
                    SatViewOnChannel.Add(new SatelliteLstVwItem(false, "Chanel " + (i + 1).ToString() + ":", 0));
                }
            }
            
            // Subscribe to the ActiveSatellitesDataReceivedEvent
            if (gpsDevice != null)
                gpsDevice.ActiveSatellitesDataReceivedEvent += ActiveSatellitesDataListener;
        }

        public void CleanUpViewModel()
        {   // Un-Subscribe from the ActiveSatellitesDataReceivedEvent and release the reference to the gpsDevice object
            // so that the garbage collector can re-claim the memory allocated for instances of this class.
            if (gpsDevice != null)
                gpsDevice.ActiveSatellitesDataReceivedEvent -= ActiveSatellitesDataListener;
            gpsDevice = null;
        }

        private void ActiveSatellitesDataListener(object sender, ActiveSatellitesDataEventArgs e)
        {   // Update our property values from the ActiveSatellitesDataEventArgs.  The view will be notified of the updates.
            // Mode1 = e.refData.Mode1.ToString();
            switch (e.ActiveSatellitesData.Mode1)
            {
                case 'M':
                    Mode1 = "Manual";
                    break;
                case 'A':
                    Mode1 = "2D Auto.";
                    break;
                default:
                     Mode1 = "Unknown";
                    break;
            }
            
            // Mode2 = e.refData.Mode2.ToString();
            switch (e.ActiveSatellitesData.Mode2)
            {
                case '1':
                    Mode2 = "Fix Not Avail.";
                    break;
                case '2':
                    Mode2 = "2D";
                    break;
                case '3':
                    Mode2 = "3D";
                    break;
                default:
                    Mode2 = "Unknown";
                    break;
            }

            // The number of channels received and the length of our observable collection should both be 12.
            // Just in case, double check, and use the smaller of the two counts.
            int end = SatViewOnChannel.Count;
            if (end > e.ActiveSatellitesData.SatViewOnChannel.Length)
                end = e.ActiveSatellitesData.SatViewOnChannel.Length;
            // loop through each of the twelve channels within the data received (e.refData), and compare the value of the ID
            // against our SatViewOnChannel observable collection. If the ID for a channel is different, update it or disable (visibility = false) if ID=0.
            for (int i=0;i<end;i++)
            {
                if (SatViewOnChannel[i].SatelliteID!=e.ActiveSatellitesData.SatViewOnChannel[i])
                {
                    if (e.ActiveSatellitesData.SatViewOnChannel[i] == 0)
                        SatViewOnChannel[i] = new SatelliteLstVwItem(false, "Chanel " + (i+1).ToString() + ":", 0);
                    else
                        SatViewOnChannel[i] = new SatelliteLstVwItem(true, "Chanel " + (i+1).ToString() + ":", e.ActiveSatellitesData.SatViewOnChannel[i]);
                }
            }
            PositionDOP = e.ActiveSatellitesData.PositionDOP.ToString();
            HorizontalDOP = e.ActiveSatellitesData.HorizontalDOP.ToString();
            VerticalDOP = e.ActiveSatellitesData.VerticalDOP.ToString();
            MessageCount = e.ActiveSatellitesData.MessageCount.ToString();
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

    public class BooleanToSatelliteTextColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value is bool && (bool)value) ? new SolidColorBrush(Windows.UI.Colors.Black) : new SolidColorBrush(Windows.UI.Colors.LightGray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new Exception("Not Implemented");
        }
    }

    public class BooleanToSatelliteImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value is bool && (bool)value) ? new BitmapImage(new Uri("ms-appx:///Assets/SatelliteAvailable.png")) : new BitmapImage(new Uri("ms-appx:///Assets/NoSatellite.png")); ;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class IntToSateliteIDStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value is int && (int)value != 0) ? ((int)value).ToString("00") : "" ;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
