using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib_NMEA_GPS
{
    /// <summary>
    /// Class for parsing and storing 'Course and Speed Information' received from the Adafruit Ultimate GPS Device
    /// </summary>
    public class CourseAndSpeedData
    {
        public int MessageCount { get; set; }
        public string MessageID { get { return "$GPVTG"; } set { } }
        public float Course_true { get; set; }
        public char Reference_true { get; set; }
        public float Course_magnetic { get; set; }
        public char Reference_magnetic { get; set; }
        public float Speed_kn { get; set; }
        public char Speed_kn_units { get; set; }
        public float Speed_kmh { get; set; }
        public char Speed_kmh_units { get; set; }
        public char Mode { get; set; }
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Copy parameters from the 'source' class to the local parameters of this class instance
        /// </summary>
        public void Copy(CourseAndSpeedData source)
        {
            MessageCount = source.MessageCount;
            Course_true = source.Course_true;
            Reference_true = source.Reference_true;
            Course_magnetic = source.Course_magnetic;
            Reference_magnetic = source.Reference_magnetic;
            Speed_kn = source.Speed_kn;
            Speed_kn_units = source.Speed_kn_units;
            Speed_kmh = source.Speed_kmh;
            Speed_kmh_units = source.Speed_kmh_units;
            Mode = source.Mode;
            ErrorMessage = source.ErrorMessage;
        }

        /// <summary>
        /// Set all properties within the class to their respective a default value.
        /// </summary>
        public void Clear()
        {
            //MessageCount = 0;
            Course_true = 0.0f;
            Reference_true = '\0';
            Course_magnetic = 0.0f;
            Reference_magnetic = '\0';
            Speed_kn = 0.0f;
            Speed_kn_units = '\0';
            Speed_kmh = 0.0f;
            Speed_kmh_units = '\0';
            Mode = '\0';
            ErrorMessage = null;
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public CourseAndSpeedData()
        {
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        public CourseAndSpeedData(CourseAndSpeedData source)
        {
            Copy(source);
        }
    }
}
