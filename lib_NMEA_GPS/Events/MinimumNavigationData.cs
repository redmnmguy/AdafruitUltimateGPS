using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib_NMEA_GPS
{
    public class MinimumNavigationDataEventArgs : EventArgs
    {
        public MinimumNavigationData MinimumNavigationData;
        public MinimumNavigationDataEventArgs(MinimumNavigationData refSourceObject)
        {   // Create a new  Minimum Navigation Data Object instance to pass along to the event listeners
            // The copy constructor will copy the property values from the source to the new instance.   
            MinimumNavigationData = new MinimumNavigationData(refSourceObject);
        }
    }

    public partial class NMEA_GPS
    {
        /// <summary>
        /// Delegate declaration for publishing notifications of new data received from serial interface
        /// </summary>
        public delegate void MinimumNavigationDataEventHandler(object sender, MinimumNavigationDataEventArgs e);

        /// <summary>
        /// An event that clients can use to be notified whenever new data is received
        /// </summary>
        public event MinimumNavigationDataEventHandler MinimumNavigationDataReceivedEvent;

        /// <summary>
        ///  Invoke the DataReceived event
        /// </summary>  
        protected virtual void MinimumNavigationDataChanged(MinimumNavigationDataEventArgs data)
        {
            MinimumNavigationDataReceivedEvent?.Invoke(this, data);
        }
    }
}
