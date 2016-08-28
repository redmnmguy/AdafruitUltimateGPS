using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib_NMEA_GPS
{
    
    /// <summary>
    /// Class for parsing and storing 'Minimum Recommended Navigation Information' received from the Adafruit Ultimate GPS Device
    /// </summary>
    public class MinimumNavigationData
    {
        public int MessageCount { get; set; }
        public string MessageID { get { return "$GPRMC"; } set { } }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Second{ get; set; }
        public int Milisecond{ get; set; }
        public char Status{ get; set; }
        public int LatDegree{ get; set; }
        public int LatMinutes{ get; set; }
        public float LatSeconds{ get; set; }
        public int Latitude{ get; set; }
        public int LonDegree{ get; set; }
        public int LonMinutes{ get; set; }
        public float LonSeconds{ get; set; }
        public int Longitude{ get; set; }
        public char NorthSouth{ get; set; }
        public char EastWest{ get; set; }
        public float SpeedOverGround{ get; set; }
        public float CourseOverGround{ get; set; }
        public int Day{ get; set; }
        public int Month{ get; set; }
        public int Year{ get; set; }
        public float MagVar_Degrees{ get; set; }
        public char MagVar_EastWest{ get; set; }
        public char Mode{ get; set; }
        public string ErrorMessage{ get; set; }

        /// <summary>
        /// Copy parameters from the 'source' class to the local parameters of this class instance
        /// </summary>
        public void Copy(MinimumNavigationData source)
        {
            MessageCount = source.MessageCount;
            Hour = source.Hour;
            Minute = source.Minute;
            Second = source.Second;
            Milisecond = source.Milisecond;
            Status = source.Status;
            LatDegree = source.LatDegree;
            LatMinutes = source.LatMinutes;
            LatSeconds = source.LatSeconds;
            Latitude = source.Latitude;
            LonDegree = source.LonDegree;
            LonMinutes = source.LonMinutes;
            LonSeconds = source.LonSeconds;
            Longitude = source.Longitude;
            NorthSouth = source.NorthSouth;
            EastWest = source.EastWest;
            SpeedOverGround = source.SpeedOverGround;
            CourseOverGround = source.CourseOverGround;
            Day = source.Day;
            Month = source.Month;
            Year = source.Year;
            MagVar_Degrees = source.MagVar_Degrees;
            MagVar_EastWest = source.MagVar_EastWest;
            Mode = source.Mode;
            ErrorMessage = source.ErrorMessage;
        }

        /// <summary>
        /// Set all properties within the class to their respective a default value.
        /// </summary>
        public void Clear()
        {
            //MessageCount = 0;
            Hour = 0;
            Minute = 0;
            Second = 0;
            Milisecond = 0;
            Status = '\0';
            LatDegree = 0;
            LatMinutes = 0;
            LatSeconds = 0.0f;
            Latitude = 0;
            LonDegree = 0;
            LonMinutes = 0;
            LonSeconds = 0.0f;
            Longitude = 0;
            NorthSouth = '\0';
            EastWest = '\0';
            SpeedOverGround = 0.0f;
            CourseOverGround = 0.0f;
            Day = 0;
            Month = 0;
            Year = 0;
            MagVar_Degrees = 0.0f;
            MagVar_EastWest = '\0';
            Mode = '\0';
            ErrorMessage = null;
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public MinimumNavigationData()
        {
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        public MinimumNavigationData(MinimumNavigationData source)
        {
            Copy(source);
        }
    }
}
