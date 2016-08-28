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
using Windows.UI.Xaml.Media.Imaging;

namespace appAdfUltGPS.ViewModels
{
    /// <summary>
    /// Class that represents an item within the Status Messages List View
    /// </summary>
    public class StatusMessageLstVwItem
    {
        public MessageContext MsgContext;
        public string Message;

        public StatusMessageLstVwItem(MessageContext msgContext, string message)
        {
            MsgContext = msgContext;
            Message = message;
        }
    }

    public class StatusMessagesPageViewModel : INotifyPropertyChanged, ICleanUpViewModel
    {
        private NMEA_GPS gpsDevice;

        #region BindingProperties

        public ObservableCollection<StatusMessageLstVwItem> StatusMessages = new ObservableCollection<StatusMessageLstVwItem>();

        #endregion

        public StatusMessagesPageViewModel()
        {
            gpsDevice = (App.Current as App).GPSdevice;
            // Subscribe to the StatusInformationChangedEvent
            if (gpsDevice != null)
                gpsDevice.StatusInformationChangedEvent += StatusInformationDataListener;
        }

        public void CleanUpViewModel()
        {   // Un-Subscribe from the StatusInformationChangedEvent and release the reference to the gpsDevice object
            // so that the garbage collector can re-claim the memory allocated for instances of this class.
            if (gpsDevice != null)
                gpsDevice.StatusInformationChangedEvent -= StatusInformationDataListener;
            gpsDevice = null;
        }

        private void StatusInformationDataListener(object sender, StatusInformationEventArgs e)
        {   // Update our property values from the StatusInformationEventArgs.  The view will be notified of the updates.
            // Add the new message at the TOP of the list
            StatusMessages.Insert(0, new StatusMessageLstVwItem(e.MessageContext, e.StatusMessage));
            // Remove messages from the BOTTOM of the list when the list count is greater than 10.
            for (int i = StatusMessages.Count; StatusMessages.Count > 10; i--)
            {
                StatusMessages.RemoveAt(i-1);
            }            
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
    public class IntToStatusImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is MessageContext && ((MessageContext)value == MessageContext.Disconnected))
               return new BitmapImage(new Uri("ms-appx:///Assets/Disconnect.png"));
            else if (value is MessageContext && ((MessageContext)value == MessageContext.Connected))
                return new BitmapImage(new Uri("ms-appx:///Assets/Connect.png"));
            else if(value is MessageContext && ((MessageContext)value == MessageContext.Data))
                return new BitmapImage(new Uri("ms-appx:///Assets/Data.png"));
            else if (value is MessageContext && ((MessageContext)value == MessageContext.Information))
                return new BitmapImage(new Uri("ms-appx:///Assets/Information.png"));
            else if (value is MessageContext && ((MessageContext)value == MessageContext.Fault))
                return new BitmapImage(new Uri("ms-appx:///Assets/Fault.png"));
            else
                return new BitmapImage(new Uri("ms-appx:///Assets/Question.png")); ;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
