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
        /// <returns>Returns true if the parse succeeded.  The dest object's Error message will be set to null when the parse succeeds.  If the parse does not succeed, the method returns false and the dest object's ErrorMessage property will contain an error message.</returns>
        private bool ParseFixedData(string nmea, FixedData dest)
        {  
            int parseLocation = 0;
            // Set the dest.ErrorMessage to null in case the caller did not call Clear() before calling Parse()
            dest.ErrorMessage = null;
            try
            {
                // Split-out each of the individual (comma separated) elements from the nmea string
                string[] nmeaArray = nmea.Split(',');
                // The Fixed Data nmea string should have at least 8 elements
                if (nmeaArray.Length < 8)
                {
                    dest.ErrorMessage = "Not enough data within the Encapsulated Packet Header";
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

                // convert the Position Fix Indicator to an integer value
                parseLocation++;
                if (nmeaArray[parseLocation].Length < 1)
                {
                    dest.Fix = -1;
                }
                else
                {
                    dest.Fix = int.Parse(nmeaArray[parseLocation]);
                }

                // Convert the Satellites Used character(s) to an integer value
                parseLocation++;
                if (nmeaArray[parseLocation].Length < 1)
                {
                    dest.SatellitesUsed = 0;
                }
                else
                {
                    dest.SatellitesUsed = int.Parse(nmeaArray[parseLocation]);
                }

                // Convert the Horizontal Dilution of Precision character(s) to an integer value
                parseLocation++;
                if (nmeaArray[parseLocation].Length < 3)
                {
                    dest.HorizontalDOP = 0.0f;
                }
                else
                {
                    dest.HorizontalDOP = float.Parse(nmeaArray[parseLocation]);
                }

                // Convert the Antenna Altitude character(s) to an integer value
                parseLocation++;
                if (nmeaArray[parseLocation].Length < 3)
                {
                    dest.MSLAltitude = 0.0f;
                }
                else
                {
                    dest.MSLAltitude = float.Parse(nmeaArray[parseLocation]);
                }
                // Get the Antenna Altitude Units
                parseLocation++;
                if (nmeaArray[parseLocation].Length < 1)
                {
                    dest.MSLUnits = '\0';
                }
                else
                {
                    dest.MSLUnits = char.Parse(nmeaArray[parseLocation]);
                }
                // Convert the Geoidal Separation character(s) to an integer value
                parseLocation++;
                if (nmeaArray[parseLocation].Length < 3)
                {
                    dest.GeoSeparation = 0.0f;
                }
                else
                {
                    dest.GeoSeparation = float.Parse(nmeaArray[parseLocation]);
                }
                // Get the Geoidal Separation Units
                parseLocation++;
                if (nmeaArray[parseLocation].Length < 1)
                {
                    dest.GeoSepUnits = '\0';
                }
                else
                {
                    dest.GeoSepUnits = char.Parse(nmeaArray[parseLocation]);
                }
                // Convert the Age of Differential Correction character(s) to an integer value
                parseLocation++;
                if (nmeaArray[parseLocation].Length < 3)
                {
                    dest.AgeOfDiffCorr = 0.0f;
                }
                else
                {   // Look for a '*' to identify the start of checksum, remove the * and beyond before parsing the HDOP.
                    int astrexLocation = nmeaArray[parseLocation].IndexOf('*');
                    if (astrexLocation > 0)
                    {  // found the checksum, parse the preceding characters
                        string AODC = nmeaArray[parseLocation].Substring(0, astrexLocation);
                        dest.AgeOfDiffCorr = float.Parse(AODC);
                    }
                    else if (astrexLocation == 0) // the contents are ",*CK"
                        dest.AgeOfDiffCorr = 0.0f;
                    else
                        dest.AgeOfDiffCorr = float.Parse(nmeaArray[parseLocation]);
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
