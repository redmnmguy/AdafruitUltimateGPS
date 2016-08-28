using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib_NMEA_GPS
{
    /// <summary>
    /// Custom EventArgs class for passing counter data received within
    /// the EventArgs data of the CounterChangedEventHandler.
    /// </summary>
    public class SerialDataEventArgs : EventArgs
    {
        public int MessageCount { get; set; }
        public int MessageLength { get; set; }
        public string Data { get; set; }

        public SerialDataEventArgs(string data, int msgCount)
        {
            MessageCount = msgCount;
            Data = data;
            MessageLength = Data.Length;
        }
    }

    public partial class NMEA_GPS
    {
        #region SerialDataReceivedEvent
        /// <summary>
        /// Delegate declaration for publishing notifications of new data received from serial interface
        /// </summary>
        public delegate void SerialDataEventHandler(object sender, SerialDataEventArgs e);

        /// <summary>
        /// An event that clients can use to be notified whenever new data is received
        /// </summary>
        public event SerialDataEventHandler SerialDataReceivedEvent;

        /// <summary>
        ///  Invoke the DataReceived event(s)
        /// </summary>  
        protected virtual void SerialDataChanged(string data, int msgCount)
        {
            SerialDataReceivedEvent?.Invoke(this, new SerialDataEventArgs(data, msgCount));
        }
        #endregion
    }
}
