using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib_NMEA_GPS
{
    public class FixedDataEventArgs : EventArgs
    {
        public FixedData FixedData;
        public FixedDataEventArgs(FixedData refSourceObject)
        {   // Create a new Fixed Data Object instance to pass along to the event listeners
            // The copy constructor will copy the property values from the source to the new instance.   
            FixedData = new FixedData(refSourceObject);
        }
    }

    public partial class NMEA_GPS
    {
        /// <summary>
        /// Delegate declaration for publishing notifications of new data received from serial interface
        /// </summary>
        public delegate void FixedDataEventHandler(object sender, FixedDataEventArgs e);

        /// <summary>
        /// An event that clients can use to be notified whenever new data is received
        /// </summary>
        public event FixedDataEventHandler FixedDataReceivedEvent;

        /// <summary>
        ///  Invoke the FixedDataReceivedEvent event
        /// </summary>  
        protected virtual void FixedDataChanged(FixedDataEventArgs data)
        {
            FixedDataReceivedEvent?.Invoke(this, data);
        }
    }
}
