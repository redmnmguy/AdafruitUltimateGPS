using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib_NMEA_GPS
{

    public class CourseAndSpeedDataEventArgs : EventArgs
    {
        public CourseAndSpeedData CourseAndSpeedData;
        public CourseAndSpeedDataEventArgs(CourseAndSpeedData refSourceObject)
        {   // Create a new Course and Speed Data Object instance to pass along to the event listeners
            // The copy constructor will copy the property values from the source to the new instance.   
            CourseAndSpeedData = new CourseAndSpeedData(refSourceObject);
        }
    }

    public partial class NMEA_GPS
    {
        /// <summary>
        /// Delegate declaration for publishing notifications of new data received from serial interface
        /// </summary>
        public delegate void CourseAndSpeedDataEventHandler(object sender, CourseAndSpeedDataEventArgs e);

        /// <summary>
        /// An event that clients can use to be notified whenever new data is received
        /// </summary>
        public event CourseAndSpeedDataEventHandler CourseAndSpeedDataReceivedEvent;

        /// <summary>
        ///  Invoke the DataReceived event
        /// </summary>  
        protected virtual void CourseAndSpeedDataChanged(CourseAndSpeedDataEventArgs data)
        {
            CourseAndSpeedDataReceivedEvent?.Invoke(this, data);
        }
    }
}
