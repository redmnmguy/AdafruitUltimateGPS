using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib_NMEA_GPS
{
    public class SatellitesInViewDataEventArgs : EventArgs
    {
        public SatellitesInViewData SatellitesInViewData;
        public SatellitesInViewDataEventArgs(SatellitesInViewData refSourceObject)
        {   // Create a new Satellites In View Data Object instance to pass along to the event listeners
            // The copy constructor will copy the property values from the source to the new instance.   
            SatellitesInViewData = new SatellitesInViewData(refSourceObject);
        }
    }

    public partial class NMEA_GPS
    {
        /// <summary>
        /// Delegate declaration for publishing notifications of new data received from serial interface
        /// </summary>
        public delegate void SatellitesInViewDataEventHandler(object sender, SatellitesInViewDataEventArgs e);

        /// <summary>
        /// An event that clients can use to be notified whenever new data is received
        /// </summary>
        public event SatellitesInViewDataEventHandler SatellitesInViewDataReceivedEvent;

        /// <summary>
        ///  Invoke the DataReceived event
        /// </summary>  
        protected virtual void SatellitesInViewDataChanged(SatellitesInViewDataEventArgs data)
        {
            SatellitesInViewDataReceivedEvent?.Invoke(this, data);
        }
    }
}
