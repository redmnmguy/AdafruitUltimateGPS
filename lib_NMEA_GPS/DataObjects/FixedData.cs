using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib_NMEA_GPS
{
    /// <summary>
    /// Class for parsing and storing 'Fixed Data' received from the Adafruit Ultimate GPS Device
    /// </summary>
    public class FixedData
    {
        public int MessageCount { get; set; }
        public string MessageID { get { return "$GPGGA"; } set { } }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Second { get; set; }
        public int Milisecond { get; set; }
        public int LatDegree { get; set; }
        public int LatMinutes { get; set; }
        public float LatSeconds { get; set; }
        public int Latitude { get; set; }
        public int LonDegree { get; set; }
        public int LonMinutes { get; set; }
        public float LonSeconds { get; set; }
        public int Longitude { get; set; }
        public char NorthSouth { get; set; }
        public char EastWest { get; set; }
        public int Fix { get; set; }
        public int SatellitesUsed { get; set; }
        public float HorizontalDOP { get; set; }
        public float MSLAltitude { get; set; }
        public char MSLUnits { get; set; }
        public float GeoSeparation { get; set; }
        public char GeoSepUnits { get; set; }
        public float AgeOfDiffCorr { get; set; }
        public string ErrorMessage { get; set; }
  
        /// <summary>
        /// Copy parameters from the 'source' class to the local parameters of this class instance
        /// </summary>
        public void Copy(FixedData source)
        {
            MessageCount = source.MessageCount;
            Hour = source.Hour;
            Minute = source.Minute;
            Second = source.Second;
            Milisecond = source.Milisecond;
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
            Fix = source.Fix;
            SatellitesUsed = source.SatellitesUsed;
            HorizontalDOP = source.HorizontalDOP;
            MSLAltitude = source.MSLAltitude;
            MSLUnits = source.MSLUnits;
            GeoSeparation = source.GeoSeparation;
            GeoSepUnits = source.GeoSepUnits;
            AgeOfDiffCorr = source.AgeOfDiffCorr;
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
            Fix = 0;
            SatellitesUsed = 0;
            HorizontalDOP = 0.0f;
            MSLAltitude = 0.0f;
            MSLUnits = '\0';
            GeoSeparation = 0.0f;
            GeoSepUnits = '\0';
            AgeOfDiffCorr = 0.0f;
            ErrorMessage = null;
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public FixedData()
        {
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        public FixedData(FixedData source)
        {
            Copy(source);
        }
    }
}
