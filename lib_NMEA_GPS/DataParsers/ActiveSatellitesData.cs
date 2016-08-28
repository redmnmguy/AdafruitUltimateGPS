using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib_NMEA_GPS
{
    public partial class NMEA_GPS
    {
        /// <summary>
        /// Expands the input variable 'nmea' into the properties of the 'dest'.
        /// </summary>
        /// <param name="nmea">NMEA data packet string which holds comma separated elements</param>
        /// <param name="dest">FixedDataObj to hold the elements parsed from the NMEA data packet</param>
        private bool ParseActiveSatellitesData(string nmea, ActiveSatellitesData dest)
        {
            int parseLocation = 0;
            // Set the dest.ErrorMessage to null in case the caller did not call Clear() before calling Parse()
            dest.ErrorMessage = null;
            try
            {
                // Split-out each of the individual (comma separated) elements from the nmea string
                string[] nmeaArray = nmea.Split(',');
                // The Fixed Data nmea string should have at least 18 elements
                if (nmeaArray.Length < 18)
                {
                    dest.ErrorMessage = "Not enough data in the DataArray for the Encapsulated Packet Header";
                    return false;
                }
                // Make sure that the Message ID within the nmea packet matches the Message ID that we are expecting to parse             
                if (!nmeaArray[parseLocation].Equals(dest.MessageID))
                {
                    dest.ErrorMessage = "Incorrect MessageID: " + nmeaArray[0];
                    return false;
                }
                // Parse Mode1
                parseLocation++;
                if (nmeaArray[parseLocation].Length < 1)
                {
                    dest.Mode1 = '\0';
                }
                else
                {
                    dest.Mode1 = char.Parse(nmeaArray[parseLocation]);
                }
                // Parse Mode2
                parseLocation++;
                if (nmeaArray[parseLocation].Length < 1)
                {
                    dest.Mode2 = '\0';
                }
                else
                {
                    dest.Mode2 = char.Parse(nmeaArray[parseLocation]);
                }
                //Parse the Satellite View On Channel #
                for (int index = 0; index < dest.SatViewOnChannel.Length; index++)
                {
                    parseLocation++;
                    if (nmeaArray[parseLocation].Length < 1)
                    {
                        dest.SatViewOnChannel[index] = 0;
                    }
                    else
                    {
                        dest.SatViewOnChannel[index] = int.Parse(nmeaArray[parseLocation]);
                    }
                }
                // Parse Position Dilution of Precision
                parseLocation = 15;
                if (nmeaArray[parseLocation].Length < 4)
                {
                    dest.PositionDOP = 0.0f;
                }
                else
                {
                    dest.PositionDOP = float.Parse(nmeaArray[parseLocation]);
                }
                // Parse Horizontal Dilution of Precision
                parseLocation++;
                if (nmeaArray[parseLocation].Length < 4)
                {
                    dest.HorizontalDOP = 0.0f;
                }
                else
                {
                    dest.HorizontalDOP = float.Parse(nmeaArray[parseLocation]);
                }
                // Parse Vertical Dilution of Precision
                parseLocation++;
                if (nmeaArray[parseLocation].Length < 4)
                {
                    dest.VerticalDOP = 0.0f;
                }
                else
                {
                    // For some reason there is no comma between the VDOP and the Checksum.
                    // Look for a '*' to identify the start of checksum, remove the * and beyond before parsing the HDOP.
                    int astrexLocation = nmeaArray[parseLocation].IndexOf('*');
                    if (astrexLocation > 0)
                    {  // found the checksum, parse the preceding characters
                        string HDOP = nmeaArray[parseLocation].Substring(0,astrexLocation);
                        dest.VerticalDOP = float.Parse(HDOP);
                    }
                    else if (astrexLocation == 0) // the contents are ",*CK"
                        dest.VerticalDOP = 0.0f;
                    else
                        dest.VerticalDOP = float.Parse(nmeaArray[parseLocation]);
                }
            }
            catch (Exception e)
            {   // Possible Exceptions: ArgumentNullException, FormatException, OverflowException
                dest.ErrorMessage = "Exception: " + e.Message + ", at parse location: " + parseLocation.ToString();
                return false;
            }
            // If we made it to this point, the nmea data packet was successfully parsed.
            return true;
        }

    }
}
