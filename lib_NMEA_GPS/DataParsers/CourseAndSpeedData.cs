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
        /// <param name="dest">CourseAndSpeedDataObj to hold the elements parsed from the NMEA data packet</param>
        private bool ParseCourseAndSpeedData(string nmea, CourseAndSpeedData dest)
        {
            int parseLocation = 0;
            // Set the dest.ErrorMessage to null in case the caller did not call Clear() before calling Parse()
            dest.ErrorMessage = null;
            try
            {
                // Split-out each of the individual (comma separated) elements from the nmea string
                string[] nmeaArray = nmea.Split(',');
                // The Fixed Data nmea string should have at least 10 elements
                if (nmeaArray.Length < 10)
                {
                    dest.ErrorMessage = "Not enough data in the DataArray for the Encapsulated Packet Header";
                    return false;
                }
                // Make sure that the Message ID within the nmea packet matches the Message ID that we are expecting to parse             
                if (!nmeaArray[parseLocation].Equals(dest.MessageID))
                {
                    dest.ErrorMessage = "Incorrect MessageID: " + nmeaArray[parseLocation];
                    return false;
                }
                // Parse the Course
                parseLocation++;
                if (nmeaArray[parseLocation].Length < 6)
                {
                    dest.Course_true = 0.0f;
                }
                else
                {
                    dest.Course_true = float.Parse(nmeaArray[parseLocation]);
                }
                // Parse the Course Reference
                parseLocation++;
                if (nmeaArray[parseLocation].Length < 1)
                {
                    dest.Reference_true = '\0';
                }
                else
                {
                    dest.Course_true = char.Parse(nmeaArray[parseLocation]);
                }
                // Parse the Course
                parseLocation++;
                if (nmeaArray[parseLocation].Length < 6)
                {
                    dest.Course_magnetic = 0.0f;
                }
                else
                {
                    dest.Course_magnetic = float.Parse(nmeaArray[parseLocation]);
                }
                // Parse the Course Reference
                parseLocation++;
                if (nmeaArray[parseLocation].Length < 1)
                {
                    dest.Reference_magnetic = '\0';
                }
                else
                {
                    dest.Course_magnetic = char.Parse(nmeaArray[parseLocation]);
                }
                // Parse the Speed
                parseLocation++;
                if (nmeaArray[parseLocation].Length < 4)
                {
                    dest.Speed_kn = 0.0f;
                }
                else
                {
                    dest.Speed_kn = float.Parse(nmeaArray[parseLocation]);
                }
                // Parse the Speed Units
                parseLocation++;
                if (nmeaArray[parseLocation].Length < 1)
                {
                    dest.Speed_kn_units = '\0';
                }
                else
                {
                    dest.Speed_kn_units = char.Parse(nmeaArray[parseLocation]);
                }
                // Parse the Speed
                parseLocation++;
                if (nmeaArray[parseLocation].Length < 4)
                {
                    dest.Speed_kmh = 0.0f;
                }
                else
                {
                    dest.Speed_kmh = float.Parse(nmeaArray[parseLocation]);
                }
                // Parse the Speed Units
                parseLocation++;
                if (nmeaArray[parseLocation].Length < 1)
                {
                    dest.Speed_kmh_units = '\0';
                }
                else
                {
                    dest.Speed_kmh_units = char.Parse(nmeaArray[parseLocation]);
                }
                // Parse the Mode
                parseLocation++;
                if (nmeaArray[parseLocation].Length < 1)
                {
                    dest.Mode = '\0';
                }
                else
                {
                    // Look for a '*' to identify the start of checksum, remove the * and beyond before parsing the Mode.
                    int astrexLocation = nmeaArray[parseLocation].IndexOf('*');
                    if (astrexLocation > 0)
                    {  // found the checksum, parse the preceding characters
                        string mode = nmeaArray[parseLocation].Substring(0,astrexLocation);
                        dest.Mode = char.Parse(mode);
                    }
                    else if (astrexLocation == 0) // the contents are ",*CK"
                        dest.Mode = '\0';
                    else
                    {
                        dest.Mode = char.Parse(nmeaArray[parseLocation]);
                    }
                }
            }
            catch (Exception e)
            {   // Possible Exceptions: ArgumentNullException, FormatException, OverflowException
                dest.ErrorMessage = "Exception: " + e.Message + ", at parse location: " + parseLocation.ToString();
                return true;
            }
            // If we made it to this point, the nmea data packet was successfully parsed.
            return true;
        }
    }
}
