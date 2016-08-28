using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using lib_NMEA_GPS;
using appAdfUltGPS.ViewModels;

namespace appAdfUltGPS
{
    // Implement ICleanUpViewModel so that events can be un-subscribed from when the page is unloaded.
    public class CourseAndSpeedPageViewModel : INotifyPropertyChanged, ICleanUpViewModel
    {
        private NMEA_GPS gpsDevice;

        #region BindingProperties
        private string _course_true;
        public string Course_true
        {
            get { return _course_true; }
            set { Set(ref _course_true, value); }
        }
        private string _course_magnetic;
        public string Course_magnetic
        {
            get { return _course_magnetic; }
            set { Set(ref _course_magnetic, value); }
        }
        private string _speed_kn;
        public string Speed_kn
        {
            get { return _speed_kn; }
            set { Set(ref _speed_kn, value); }
        }
        private string _speed_kmh;
        public string Speed_kmh
        {
            get { return _speed_kmh; }
            set { Set(ref _speed_kmh, value); }
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

        public CourseAndSpeedPageViewModel()
        {
            gpsDevice = (App.Current as App).GPSdevice;
            // Subscribe to the CourseAndSpeedDataReceivedEvent
            if (gpsDevice != null)
                gpsDevice.CourseAndSpeedDataReceivedEvent += CourseAndSpeedDataListener;
        }

        public void CleanUpViewModel()
        {   // Un-Subscribe from the CourseAndSpeedDataReceivedEvent and release the reference to the gpsDevice object
            // so that the garbage collector can re-claim the memory allocated for instances of this class.
            if (gpsDevice != null)
                gpsDevice.CourseAndSpeedDataReceivedEvent -= CourseAndSpeedDataListener;
            gpsDevice = null;
        }

        private void CourseAndSpeedDataListener(object sender, CourseAndSpeedDataEventArgs e)
        {   // Update our property values from the CourseAndSpeedDataEventArgs.  The view will be notified of the updates.
            Course_true = e.CourseAndSpeedData.Course_true.ToString("0.00");
            Course_magnetic = e.CourseAndSpeedData.Course_magnetic.ToString("0.00");
            Speed_kn = e.CourseAndSpeedData.Speed_kn.ToString("0.00");
            Speed_kmh = e.CourseAndSpeedData.Speed_kmh.ToString("0.00");
            Mode = e.CourseAndSpeedData.Mode.ToString();
            MessageCount = e.CourseAndSpeedData.MessageCount.ToString();
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
