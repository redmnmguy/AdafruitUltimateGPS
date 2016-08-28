using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using lib_NMEA_GPS;


namespace appAdfUltGPS.ViewModels
{
    // Implement ICleanUpViewModel so that events can be un-subscribed from when the page is unloaded.
    public class FixedDataPageViewModel : INotifyPropertyChanged, ICleanUpViewModel
    {
        private NMEA_GPS gpsDevice;

        #region BindingProperties
        private string _gpsTime;
        public string GPSTime
        {
            get { return _gpsTime; }
            set { Set(ref _gpsTime, value); }
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
        private string _satellitesUsed;
        public string SatellitesUsed
        {
            get { return _satellitesUsed; }
            set { Set(ref _satellitesUsed, value); }
        }
        private string _fix;
        public string Fix
        {
            get { return _fix; }
            set { Set(ref _fix, value); }
        }
        private string _horizontalDOP;
        public string HorizontalDOP
        {
            get { return _horizontalDOP; }
            set { Set(ref _horizontalDOP, value); }
        }
        private string _antennaAltitude;
        public string AntennaAltitude
        {
            get { return _antennaAltitude; }
            set { Set(ref _antennaAltitude, value); }
        }
        private string _geoSeparation;
        public string GeoSeparation
        {
            get { return _geoSeparation; }
            set { Set(ref _geoSeparation, value); }
        }
        private string _ageOfDiffCorr;
        public string AgeOfDiffCorr
        {
            get { return _ageOfDiffCorr; }
            set { Set(ref _ageOfDiffCorr, value); }
        }
        private string _messageCount;
        public string MessageCount
        {
            get { return _messageCount; }
            set { Set(ref _messageCount, value); }
        }
        #endregion

        public FixedDataPageViewModel()
        {
            gpsDevice = (App.Current as App).GPSdevice;
            // Subscribe to the FixedDataReceivedEvent
            if (gpsDevice != null)
                gpsDevice.FixedDataReceivedEvent  += FixedDataListener;
        }

        public void CleanUpViewModel()
        {   // Un-Subscribe from the FixedDataReceivedEvent and release the reference to the gpsDevice object
            // so that the garbage collector can re-claim the memory allocated for instances of this class.
            if (gpsDevice != null)
                gpsDevice.FixedDataReceivedEvent -= FixedDataListener;
            gpsDevice = null;
        }

        private void FixedDataListener(object sender, FixedDataEventArgs e)
        {   // Update our property values from the ActiveSatellitesDataObj.  The view will be notified of the updates.
            GPSTime = e.FixedData.Hour.ToString("00") + ":" + e.FixedData.Minute.ToString("00") + ":" + e.FixedData.Second.ToString("00") + "." + e.FixedData.Milisecond.ToString("000");
            LatDegree = e.FixedData.LatDegree.ToString() + "° " + e.FixedData.LatMinutes.ToString() + "' " + e.FixedData.LatSeconds.ToString("0.0000") + "\"" + e.FixedData.NorthSouth.ToString();
            LonDegree = e.FixedData.LonDegree.ToString() + "° " + e.FixedData.LonMinutes.ToString() + "' " + e.FixedData.LonSeconds.ToString("0.0000") + "\"" + e.FixedData.EastWest.ToString();
            Latitude = e.FixedData.Latitude.ToString("000,000,000");
            Longitude = e.FixedData.Longitude.ToString("000,000,000");
            SatellitesUsed = e.FixedData.SatellitesUsed.ToString();
            switch (e.FixedData.Fix)
            {
                case 0:
                    Fix = "Not Available";
                    break;
                case 1:
                    Fix = "GPS";
                    break;
                case 2:
                    Fix = "Differential GPS";
                    break;
                default:
                    Fix = "Unknown Indicator";
                    break;
            }
            HorizontalDOP = e.FixedData.HorizontalDOP.ToString();
            AntennaAltitude = e.FixedData.MSLAltitude.ToString() + " " + e.FixedData.MSLUnits.ToString();
            GeoSeparation = e.FixedData.GeoSeparation.ToString() + " " + e.FixedData.GeoSepUnits.ToString();
            AgeOfDiffCorr = e.FixedData.AgeOfDiffCorr.ToString();
            MessageCount = e.FixedData.MessageCount.ToString();
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
