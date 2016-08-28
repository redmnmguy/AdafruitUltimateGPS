using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib_NMEA_GPS
{
    /// <summary>
    /// Class for parsing and storing 'Active Satellites' data received from the Adafruit Ultimate GPS Device
    /// </summary>
    public class ActiveSatellitesData
    {
        public int MessageCount { get; set; }
        public string MessageID { get { return "$GPGSA"; } set { } }
        public char Mode1 { get; set; }
        public char Mode2 { get; set; }
        public int[] SatViewOnChannel { get; set; }
        public float PositionDOP { get; set; }
        public float HorizontalDOP { get; set; }
        public float VerticalDOP { get; set; }
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Copy parameters from the 'source' class to the local parameters of this class instance
        /// </summary>
        public void Copy(ActiveSatellitesData source)
        {
            MessageCount = source.MessageCount;
            Mode1 = source.Mode1;
            Mode2 = source.Mode2;
            SatViewOnChannel[0] = source.SatViewOnChannel[0];
            SatViewOnChannel[1] = source.SatViewOnChannel[1];
            SatViewOnChannel[2] = source.SatViewOnChannel[2];
            SatViewOnChannel[3] = source.SatViewOnChannel[3];
            SatViewOnChannel[4] = source.SatViewOnChannel[4];
            SatViewOnChannel[5] = source.SatViewOnChannel[5];
            SatViewOnChannel[6] = source.SatViewOnChannel[6];
            SatViewOnChannel[7] = source.SatViewOnChannel[7];
            SatViewOnChannel[8] = source.SatViewOnChannel[8];
            SatViewOnChannel[9] = source.SatViewOnChannel[9];
            SatViewOnChannel[10] = source.SatViewOnChannel[10];
            SatViewOnChannel[11] = source.SatViewOnChannel[11];
            PositionDOP = source.PositionDOP;
            HorizontalDOP = source.VerticalDOP;
            VerticalDOP = source.VerticalDOP;
            ErrorMessage = source.ErrorMessage;
        }

        /// <summary>
        /// Set all properties within the class to their respective a default value.
        /// </summary>
        public void Clear()
        {
            // MessageCount = 0;
            Mode1 = '\0';
            Mode2 = '\0';
            SatViewOnChannel[0] = 0;
            SatViewOnChannel[1] = 0;
            SatViewOnChannel[2] = 0;
            SatViewOnChannel[3] = 0;
            SatViewOnChannel[4] = 0;
            SatViewOnChannel[5] = 0;
            SatViewOnChannel[6] = 0;
            SatViewOnChannel[7] = 0;
            SatViewOnChannel[8] = 0;
            SatViewOnChannel[9] = 0;
            SatViewOnChannel[10] = 0;
            SatViewOnChannel[11] = 0;
            PositionDOP = 0.0f;
            HorizontalDOP = 0.0f;
            VerticalDOP = 0.0f;
            ErrorMessage = null;
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ActiveSatellitesData()
        {
            SatViewOnChannel = new int[12];
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        public ActiveSatellitesData(ActiveSatellitesData source)
        {
            SatViewOnChannel = new int[12];
            Copy(source);
        }

    }
}
