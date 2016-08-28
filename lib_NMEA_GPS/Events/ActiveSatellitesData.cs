using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib_NMEA_GPS
{
    public class ActiveSatellitesDataEventArgs : EventArgs
    {
        public ActiveSatellitesData ActiveSatellitesData;
        public ActiveSatellitesDataEventArgs(ActiveSatellitesData refSourceObject)
        {   // Create a new Active Satellites Data Object instance to pass along to the event listeners
            // The copy constructor will copy the property values from the source to the new instance.   
            ActiveSatellitesData = new ActiveSatellitesData(refSourceObject);
        }
    }

    public partial class NMEA_GPS
    {
        /// <summary>
        /// Delegate declaration for publishing notifications of new data received from serial interface
        /// </summary>
        public delegate void ActiveSatellitesDataEventHandler(object sender, ActiveSatellitesDataEventArgs e);

        /// <summary>
        /// An event that clients can use to be notified whenever new data is received
        /// </summary>
        public event ActiveSatellitesDataEventHandler ActiveSatellitesDataReceivedEvent;

        /// <summary>
        ///  Invoke the DataReceived event(s)
        /// </summary>  
        protected virtual void ActiveSatellitesDataChanged(ActiveSatellitesDataEventArgs data)
        {
            ActiveSatellitesDataReceivedEvent?.Invoke(this, data);
        }
    }
}
