using lib_NMEA_GPS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace appAdfUltGPS.ViewModels
{ 
    // Implement ICleanUpViewModel so that events can be un-subscribed from when the page is unloaded.
    public class MinimumNavigationPageViewModel : INotifyPropertyChanged, ICleanUpViewModel
    {
        private NMEA_GPS gpsDevice;

        #region BindingProperties
        private string _gpsTime;
        public string GPSTime
        {
            get { return _gpsTime; }
            set { Set(ref _gpsTime, value); }
        }
        private string _gpsDate;
        public string GPSDate
        {
            get { return _gpsDate; }
            set { Set(ref _gpsDate, value); }
        }
        private string _status;
        public string Status
        {
            get { return _status; }
            set { Set(ref _status, value); }
        }
        private string _latDegree;
        public string LatDegree
        {
            get { return _latDegree; }
            set { Set(ref _latDegree, value); }
        }
        private string _lonDegree;
        public string LonDegree
        {
            get { return _lonDegree; }
            set { Set(ref _lonDegree, value); }
        }
        private string _latitude;
        public string Latitude
        {
            get { return _latitude; }
            set { Set(ref _latitude, value); }
        }
        private string _longitude;
        public string Longitude
        {
            get { return _longitude; }
            set { Set(ref _longitude, value); }
        }
        private string _speedOverGround;
        public string SpeedOverGround
        {
            get { return _speedOverGround; }
            set { Set(ref _speedOverGround, value); }
        }
        private string _courseOverGround;
        public string CourseOverGround
        {
            get { return _courseOverGround; }
            set { Set(ref _courseOverGround, value); }
        }
        private string _magneticVariation;
        public string MagneticVariation
        {
            get { return _magneticVariation; }
            set { Set(ref _magneticVariation, value); }
        }
        private string _mode;
        public string Mode
        {
            get { return _mode; }
            set { Set(ref _mode, value); }
        }
        private string _messageCount;
        public string MessageCount
        {
            get { return _messageCount; }
            set { Set(ref _messageCount, value); }
        }
        #endregion

        public MinimumNavigationPageViewModel()
        {
            gpsDevice = (App.Current as App).GPSdevice;
            // Subscribe to the .MinimumNavigationDataReceivedEvent
            if (gpsDevice != null)
                gpsDevice.MinimumNavigationDataReceivedEvent += MinimumNavigationDataListener;
        }

        public void CleanUpViewModel()
        {   // Un-Subscribe from the .MinimumNavigationDataReceivedEvent and release the reference to the gpsDevice object
            // so that the garbage collector can re-claim the memory allocated for instances of this class.
            if (gpsDevice != null)
                gpsDevice.MinimumNavigationDataReceivedEvent -= MinimumNavigationDataListener;
            gpsDevice = null;
        }

        private void MinimumNavigationDataListener(object sender, MinimumNavigationDataEventArgs e)
        {   // Update our property values from the ActiveSatellitesDataEventArgs.  The view will be notified of the updates.
            GPSTime = e.MinimumNavigationData.Hour.ToString("00") + ":" + e.MinimumNavigationData.Minute.ToString("00") + ":" + e.MinimumNavigationData.Second.ToString("00") + "." + e.MinimumNavigationData.Milisecond.ToString("000");
            GPSDate = e.MinimumNavigationData.Month.ToString("00") + "/" + e.MinimumNavigationData.Day.ToString("00") + "/" + e.MinimumNavigationData.Year.ToString("00");
            LatDegree = e.MinimumNavigationData.LatDegree.ToString() + "° " + e.MinimumNavigationData.LatMinutes.ToString() + "' " + e.MinimumNavigationData.LatSeconds.ToString("0.0000") + "\"" + e.MinimumNavigationData.NorthSouth.ToString();
            LonDegree = e.MinimumNavigationData.LonDegree.ToString() + "° " + e.MinimumNavigationData.LonMinutes.ToString() + "' " + e.MinimumNavigationData.LonSeconds.ToString("0.0000") + "\"" + e.MinimumNavigationData.EastWest.ToString();
            Latitude = e.MinimumNavigationData.Latitude.ToString("000,000,000");
            Longitude = e.MinimumNavigationData.Longitude.ToString("000,000,000");
            switch (e.MinimumNavigationData.Status)
            {
                case 'A':
                case 'a':
                    Status = "Data Valid";
                    break;
                case 'V':
                case 'v':
                    Status = "Data Not Valid";
                    break;
                default:
                    Status = "Unknown Status: "+ e.MinimumNavigationData.Status.ToString();
                    break;
            }

            SpeedOverGround = e.MinimumNavigationData.SpeedOverGround.ToString() + " kn";
            CourseOverGround = e.MinimumNavigationData.CourseOverGround.ToString() + "°";
            MagneticVariation = e.MinimumNavigationData.MagVar_Degrees.ToString() + " " + e.MinimumNavigationData.MagVar_EastWest.ToString(); 
            switch (e.MinimumNavigationData.Mode)
            {
                case 'A':
                case 'a':
                    Mode = "Autonomous";
                    break;
                case 'D':
                case 'd':
                    Mode = "Differential";
                    break;
                case 'E':
                case 'e':
                    Mode = "Estimated";
                    break;
                default:
                    Mode = "Unknown Mode: " + e.MinimumNavigationData.Mode.ToString();
                    break;
            }
            MessageCount = e.MinimumNavigationData.MessageCount.ToString();
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
}
