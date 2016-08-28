using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib_NMEA_GPS
{
    public partial class NMEA_GPS
    {
        private bool msg1rcvd, msg2rcvd, msg3rcvd;
        /// <summary>
        /// Expands the input variable 'nmea' into the properties of the 'dest'.
        /// </summary>
        /// <param name="nmea">NMEA data packet string which holds comma separated elements</param>
        /// <param name="dest">FixedDataObj to hold the elements parsed from the NMEA data packet</param>
        private bool ParseSatellitesInViewData(string nmea, SatellitesInViewData dest)
        {
            int NumberOfMessages;
            int MessageNumber;
            int parseLocation = 0;
           
            // Set the dest.ErrorMessage to null in case the caller did not call Clear() before calling Parse()
            dest.ErrorMessage = null;
            try
            {
                // Split-out each of the individual (comma separated) elements from the nmea string
                string[] nmeaArray = nmea.Split(',');
                // The Fixed Data nmea string should have at least 8 elements
                // However, a 4 element packet indicating 0 satellites in view is valid: $GPGSV,1,1,00*79
                if (nmeaArray.Length < 3)
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
                // Parse the Number of Messages
                parseLocation++;
                if (nmeaArray[parseLocation].Length < 1)
                {
                    NumberOfMessages = 0;
                }
                else
                {
                    NumberOfMessages = int.Parse(nmeaArray[parseLocation]);
                }
                // Parse the Message Number
                parseLocation++;
                if (nmeaArray[parseLocation].Length < 1)
                {
                    MessageNumber = 0;
                }
                else
                {
                    MessageNumber = int.Parse(nmeaArray[parseLocation]);
                }

                // If we receive a message sentence containing a message number
                // that we have already received, then assume that we are being
                // sent a new set of message sentences.  Clear-out the data
                // storage and clear the message received flags.
                if((MessageNumber==1 && msg1rcvd)
                    || (MessageNumber == 2 && msg2rcvd)
                    || (MessageNumber == 3 && msg3rcvd))
                    {
                        dest.Clear();
                        msg1rcvd= msg2rcvd= msg3rcvd = false;
                    }

                // If the device is sending only one message, set message received flags
                // two and three so that the message received check at the end of the
                // method succeeds when just the first message is received.
                if (NumberOfMessages == 1)
                    msg2rcvd = msg3rcvd = true;
                // likewise set message received flag three when we expect two messages....
                else if (NumberOfMessages == 2)
                    msg3rcvd = true;

                // Set the message received flag corresponding
                // to the Message Number we received.
                if (MessageNumber == 1)
                    msg1rcvd = true;
                else if (MessageNumber == 2)
                    msg2rcvd = true;
                else if (MessageNumber == 3)
                    msg3rcvd = true;

                // Parse the Number of Satellites in View
                parseLocation++;
                if (nmeaArray[parseLocation].Length < 1)
                {
                    dest.SatellitesInView = 0;
                }
                else if (nmeaArray[parseLocation].Length > 2)
                {  // The SatellitesInView number is likely 00 followed by the checksum: $GPGSV,1,1,00*79
                    dest.SatellitesInView = int.Parse(nmeaArray[parseLocation].Substring(0,2));
                    if(dest.SatellitesInView==0)
                    {   // If the Satellites in View value is 0, then there are no
                        // satellites in view, clear the destination buffer and indicate
                        // that the parse has succeeded by returning a true.
                        dest.Clear();
                        return true;
                    }
                }
                else
                {
                    dest.SatellitesInView = int.Parse(nmeaArray[parseLocation]);
                }

                // Parse the individual 'Satellite Data' groups
                int loopCount = 0;    // in case we get a huge buffer full of data, limit
                                        // the number of times we iterate within the while  loop 
                while ((parseLocation <= (nmeaArray.Length - 5)) && (loopCount < 4))
                {
                    SatelliteData sateliteData = new SatelliteData();
                    // Parse the Satellite ID
                    parseLocation++;
                    if (nmeaArray[parseLocation].Length < 2)
                        sateliteData.SatelliteID = 0;
                    else
                        sateliteData.SatelliteID = int.Parse(nmeaArray[parseLocation]);

                    // Parse the Elevation
                    parseLocation++;
                    if (nmeaArray[parseLocation].Length < 2)
                        sateliteData.Elevation = 0;
                    else
                        sateliteData.Elevation = int.Parse(nmeaArray[parseLocation]);

                    // Parse the Azimuth
                    parseLocation++;
                    if (nmeaArray[parseLocation].Length < 2)
                        sateliteData.Azimuth = 0;
                    else
                        sateliteData.Azimuth = int.Parse(nmeaArray[parseLocation]);

                    // Parse the Signal to Noise Ratio
                    parseLocation++;
                    if (nmeaArray[parseLocation].Length < 2)
                        sateliteData.SNR = 0;
                    else
                    {
                        // For some reason there is no comma between the last SNR value and the Checksum.
                        // Look for a '*' to identify the start of checksum, remove the * and beyond before parsing the SNR.
                        int astrexLocation = nmeaArray[parseLocation].IndexOf('*');
                        if (astrexLocation > 0)
                        {
                            // found the checksum, parse the preceding characters
                            string substring = nmeaArray[parseLocation].Substring(0,astrexLocation);
                            sateliteData.SNR = int.Parse(substring);
                        }
                        else if (astrexLocation == 0) // the contents are ",*CK" 
                            sateliteData.SNR = 0;
                        else
                            sateliteData.SNR = int.Parse(nmeaArray[parseLocation]);
                    }
                    // add to the SatelliteData list
                    dest.SatelliteData.Add(sateliteData);
                    loopCount++;
                }
            }
            catch (Exception e)
            {   // Possible Exceptions: IndexOutOfRangeException, ArgumentNullException, FormatException, OverflowException
                dest.ErrorMessage = "Exception: " + e.Message + ", at parse location: " + parseLocation.ToString();
                return true;
            }
            // If we made it to this point, the nmea data packet was successfully parsed.
            if (msg1rcvd && msg2rcvd && msg3rcvd)
                return true;
            else
            {   // The sentence was successfully parsed. However, there are more sentences (messages) to follow...
                dest.ErrorMessage = null;
                return false;
            }
        }
    }
}
