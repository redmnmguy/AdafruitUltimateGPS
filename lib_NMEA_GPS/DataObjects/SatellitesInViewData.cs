using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib_NMEA_GPS
{

    /// <summary>
    /// Class for storing individual 'Satellite Data'
    /// </summary>
    public class SatelliteData
    {
        public int SatelliteID { get; set; }
        public int Elevation { get; set; }
        public int Azimuth { get; set; }
        public int SNR { get; set; }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        public SatelliteData(SatelliteData source)
        {
            SatelliteID = source.SatelliteID;
            Elevation = source.Elevation;
            Azimuth = source.Azimuth;
            SNR = source.SNR;
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public SatelliteData()
        { 
        }
    }

    /// <summary>
    /// Class for storing 'Satellites In View' data received from the Adafruit Ultimate GPS Device
    /// </summary>
    public class SatellitesInViewData
    {
        public int MessageCount { get; set; }
        public string MessageID { get { return "$GPGSV"; } set { } }
        public int SatellitesInView { get; set; }
        public List<SatelliteData> SatelliteData { get; set; }
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Copy parameters from the 'source' class to the local parameters of this class instance
        /// </summary>
        public void Copy(SatellitesInViewData source)
        {
            MessageCount = source.MessageCount;
            SatellitesInView = source.SatellitesInView;
            foreach (SatelliteData satDataObj in source.SatelliteData)
            {
                SatelliteData.Add(new SatelliteData(satDataObj));
            }
            ErrorMessage = source.ErrorMessage;
        }

        /// <summary>
        /// Set all properties within the class to their respective a default value.
        /// </summary>
        public void Clear()
        {
            // MessageCount = 0;
            SatellitesInView = 0;
            SatelliteData.Clear();
            ErrorMessage = null;
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public SatellitesInViewData()
        {
            SatelliteData = new List<SatelliteData>();
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        public SatellitesInViewData(SatellitesInViewData source)
        {
            SatelliteData = new List<SatelliteData>();
            Copy(source);
        }
    }
}
