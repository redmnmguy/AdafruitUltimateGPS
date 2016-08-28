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
        /// <param name="dest">MinimumNavigationDataObj to hold the elements parsed from the NMEA data packet</param>
        private bool ParseMinimumNavigationData(string nmea, MinimumNavigationData dest)
        {
            int parseLocation = 0;
            // Set the dest.ErrorMessage to null in case the caller did not call Clear() before calling Parse()
            dest.ErrorMessage = null;
            try
            {
                // Split-out each of the individual (comma separated) elements from the nmea string
                string[] nmeaArray = nmea.Split(',');
                // The Fixed Data nmea string should have at least 12 elements
                if (nmeaArray.Length < 12)
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
                // Parse the Time
                parseLocation++;
                if (nmeaArray[parseLocation].Length < 10)
                {
                    dest.Hour = 0;
                    dest.Minute = 0;
                    dest.Second = 0;
                    dest.Milisecond = 0;
                }
                else
                {
                    dest.Hour = int.Parse((nmeaArray[parseLocation].Substring(0, 2)));
                    dest.Minute = int.Parse((nmeaArray[parseLocation].Substring(2, 2)));
                    dest.Second = int.Parse((nmeaArray[parseLocation].Substring(4, 2)));
                    // Skip the decimal point
                    dest.Milisecond = int.Parse((nmeaArray[parseLocation].Substring(7, 3)));
                }
               // Parse the Status
                parseLocation++;
                if (nmeaArray[parseLocation].Length < 1)
                {
                    dest.Status = '\0';
                }
                else
                {
                    dest.Status = char.Parse(nmeaArray[parseLocation]);
                }
              // Parse the Latitude
                parseLocation++;
                if (nmeaArray[parseLocation].Length < 9)
                {
                    dest.LatDegree = 0;
                    dest.LatMinutes = 0;
                    dest.LatSeconds = 0.0f;
                    dest.Latitude = 0;
                }
                else
                {
                    dest.LatDegree = int.Parse((nmeaArray[parseLocation].Substring(0, 2)));
                    dest.LatMinutes = int.Parse((nmeaArray[parseLocation].Substring(2, 2)));
                    // Skip the decimal point and store the remaining value as the
                    // fractional portion of the minutes * 10000
                    // TODO: could we get fewer than 4 characters back on the right of the decimal point??
                    int fractMinutes = int.Parse((nmeaArray[parseLocation].Substring(5, 4)));
                    // Convert the fractional minutes into Seconds ( 0 - 10000 Min*10000 = 0 - 60 Sec.)
                    dest.LatSeconds = (Convert.ToSingle(fractMinutes) * 6.0f) / 1000.0f;
                    // Create an integer value which represents the Deg, Min & Seconds.
                    // This value will range -900,000,000 to 900,000,000
                    // First, place the Degree value in the upper portion of the integer:
                    dest.Latitude = dest.LatDegree * 10000000;
                    // Combine the minutes and fractional minutes, store the result in the 'fractMinutes' temporary register:
                    fractMinutes = fractMinutes + (dest.LatMinutes * 10000);
                    // convert the minutes to deg.
                    fractMinutes = (fractMinutes * 50) / 3;   // Note: 1000/60 = 50/3
                                                              // add the minutes value (converted above into units of deg) to the property value
                    dest.Latitude = dest.Latitude + fractMinutes;
                }

                // Get the N/S Indicator
                parseLocation++;
                if (nmeaArray[parseLocation].Length < 1)
                {
                    dest.NorthSouth = '\0';
                }
                else
                {
                    dest.NorthSouth = char.Parse(nmeaArray[parseLocation]);
                }
                // Latitude is negative if N/S indicator is 'S'
                if (dest.NorthSouth == 'S' || dest.NorthSouth == 's')
                {
                    dest.LatDegree *= -1;
                    dest.Latitude *= -1;
                }

                // Parse the Longitude
                parseLocation++;
                if (nmeaArray[parseLocation].Length < 10)
                {
                    dest.LonDegree = 0;
                    dest.LonMinutes = 0;
                    dest.LonSeconds = 0.0f;
                    dest.Longitude = 0;
                }
                else
                {
                    dest.LonDegree = int.Parse((nmeaArray[parseLocation].Substring(0, 3)));
                    dest.LonMinutes = int.Parse((nmeaArray[parseLocation].Substring(3, 2)));
                    // Skip the decimal point and store the remaining value as the
                    // fractional portion of the minutes * 10000
                    // TODO: could we get fewer than 4 characters back on the right of the decimal point??
                    int fractMinutes = int.Parse((nmeaArray[parseLocation].Substring(6, 4)));
                    // Convert the fractional minutes into Seconds ( 0 - 10000 Min*10000 = 0 - 60 Sec.)
                    dest.LonSeconds = (Convert.ToSingle(fractMinutes) * 6.0f) / 1000.0f;
                    // Create an integer value which represents the Deg, Min & Seconds.
                    // This value will range -1800,000,000 to 1800,000,000
                    // First, place the Degree value in the upper portion of the integer:
                    dest.Longitude = dest.LonDegree * 10000000;
                    // Combine the minutes and fractional minutes, store the result in the 'fractMinutes' temporary register:
                    fractMinutes = fractMinutes + (dest.LonMinutes * 10000);
                    // convert the minutes to deg.
                    fractMinutes = (fractMinutes * 50) / 3;   // Note: 1000/60 = 50/3
                                                              // add the minutes value (converted above into units of deg) to the property value
                    dest.Longitude = dest.Longitude + fractMinutes;
                }
                // Get the E/W Indicator
                parseLocation++;
                if (nmeaArray[parseLocation].Length < 1)
                {
                    dest.EastWest = '\0';
                }
                else
                {
                    dest.EastWest = char.Parse(nmeaArray[parseLocation]);
                }

                // Longitude is negative if E/W indicator is 'W'
                if (dest.EastWest == 'W' || dest.EastWest == 'w')
                {
                    dest.LonDegree *= -1;
                    dest.Longitude *= -1;
                }

                // Parse the Speed Over Ground value
                parseLocation++;
                if (nmeaArray[parseLocation].Length < 4)
                {
                    dest.SpeedOverGround = 0.0f;
                }
                else
                {
                    dest.SpeedOverGround = float.Parse(nmeaArray[parseLocation]);
                }

                // Parse the Course Over Ground value
                parseLocation++;
                if (nmeaArray[parseLocation].Length < 4)
                {
                    dest.CourseOverGround = 0;
                }
                else
                {
                    dest.CourseOverGround = float.Parse(nmeaArray[parseLocation]);
                }
                // Parse the Date
                parseLocation++;
                if (nmeaArray[parseLocation].Length < 6)
                {
                    dest.Day = 0;
                    dest.Month = 0;
                    dest.Year = 0;
                }
                else
                {
                    dest.Day = int.Parse((nmeaArray[parseLocation].Substring(0, 2)));
                    dest.Month = int.Parse((nmeaArray[parseLocation].Substring(2, 2)));
                    dest.Year = int.Parse((nmeaArray[parseLocation].Substring(4, 2)));
                }
                // Parse the Magnetic Variation Value
                parseLocation++;
                if (nmeaArray[parseLocation].Length < 3)
                {
                    dest.MagVar_Degrees = 0.0f;
                }
                else
                {
                    dest.MagVar_Degrees = float.Parse(nmeaArray[parseLocation]);
                }

                // Parse the Magnetic Variation East/West character
                parseLocation++;
                if (nmeaArray[parseLocation].Length < 1)
                {
                    dest.MagVar_EastWest = '\0';
                }
                else
                {
                    dest.MagVar_EastWest = char.Parse(nmeaArray[parseLocation]);
                }
                // Parse the Mode character
                parseLocation++;
                if (nmeaArray[parseLocation].Length < 1)
                {
                    dest.Mode = '\0';
                }
                else
                {   // For some reason there is no comma between the Mode and the Checksum.
                    // Look for a '*' to identify the start of checksum, remove the * and beyond before parsing the MOde.
                    int astrexLocation = nmeaArray[parseLocation].IndexOf('*');
                    if (astrexLocation > 0)
                    {  // found the checksum, parse the preceding characters
                        string mode = nmeaArray[parseLocation].Substring(0, (astrexLocation));
                        dest.Mode  = char.Parse(mode);
                    }
                    else if (astrexLocation == 0) // the contents are ",*CK"
                        dest.Mode = '\0';
                    else
                        dest.Mode = char.Parse(nmeaArray[parseLocation]);                           
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
