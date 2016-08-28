using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using lib_NMEA_GPS;
using appAdfUltGPS.ViewModels;
using System.Collections.ObjectModel;

namespace appAdfUltGPS.ViewModels
{
    public class SendCommandsPageViewModel : INotifyPropertyChanged, ICleanUpViewModel
    {
        private NMEA_GPS gpsDevice;

        #region BindingProperties
        public ObservableCollection<string> CommandResponses = new ObservableCollection<string>();
        private string _messageCount;
        public string MessageCount
        {
            get { return _messageCount; }
            set { Set(ref _messageCount, value); }
        }
        #endregion

        public SendCommandsPageViewModel()
        {
            gpsDevice = (App.Current as App).GPSdevice;
            // Subscribe to the CourseAndSpeedDataReceivedEvent
            if (gpsDevice != null)
                gpsDevice.CommandResponseDataReceivedEvent += CommandResponseDataListener;
        }

        public void CleanUpViewModel()
        {   // Un-Subscribe from the CourseAndSpeedDataReceivedEvent and release the reference to the gpsDevice object
            // so that the garbage collector can re-claim the memory allocated for instances of this class.
            if (gpsDevice != null)
                gpsDevice.CommandResponseDataReceivedEvent -= CommandResponseDataListener;
            gpsDevice = null;
        }


        // Send one of the pre-defined command messages based on the button pressed:
        public async void buttonTestPacket_Clicked()
        {
            await gpsDevice.SendAsync(gpsDeviceCommand.PMTK_TEST);
        }
        public async void buttonAntStsON_Clicked()
        {
            await gpsDevice.SendAsync(gpsDeviceCommand.PGCMD_ANTENNA);
        }
        public async void buttonAntStsOFF_Clicked()
        {
            await gpsDevice.SendAsync(gpsDeviceCommand.PGCMD_NOANTENNA);
        }
        public async void buttonFirmwareRelease_Clicked()
        {
            await gpsDevice.SendAsync(gpsDeviceCommand.QUERY_RELEASE);
        }
        public async void buttonLocusStatus_Clicked()
        {
            await gpsDevice.SendAsync(gpsDeviceCommand.LOCUS_QUERY_STATUS);
        }

        private void CommandResponseDataListener(object sender, CommandResponseDataEventArgs e)
        {   // Update our property values from the CourseAndSpeedDataEventArgs.  The view will be notified of the updates.
            string responseMessage;
            // If we CommandRespose object received within the event args is of the base class type, then 
            // no suitable response type was found for the packet received or there was an error parsing the packet.
            if (e.CommandRespose.GetType() == typeof(CommandResponse))
            {
                responseMessage = e.CommandRespose.ErrorMessage;
            }
            else if (e.CommandRespose.GetType() == typeof(CommandResponse_Ack))
            {
                responseMessage = "Ack to command: " + ((CommandResponse_Ack)(e.CommandRespose)).Cmd.ToString("000") + ", Flag = " + ((CommandResponse_Ack)(e.CommandRespose)).Flag.ToString();
            }
            else if (e.CommandRespose.GetType() == typeof(CommandResponse_SysMsg))
            {
                responseMessage = "System Message Received, Message Code: " + ((CommandResponse_SysMsg)(e.CommandRespose)).Msg.ToString();
            }
            else if (e.CommandRespose.GetType() == typeof(CommandResponse_TxtMsg))
            {
                responseMessage = "System Text Message Received: " + ((CommandResponse_TxtMsg)(e.CommandRespose)).Msg;
            }
            // TODO add other messages here
            else
            { 
                responseMessage = "Unknown Message Received";
            }
            CommandResponses.Insert(0, responseMessage);
            for (int i = CommandResponses.Count; CommandResponses.Count > 5; i--)
            {
                CommandResponses.RemoveAt(i-1);
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
}
