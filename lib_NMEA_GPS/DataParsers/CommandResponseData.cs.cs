using lib_NMEA_GPS;
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
        /// <param name="dest">CommandResponseDataDataObj to hold the elements parsed from the NMEA data packet</param>
        private CommandResponse ParseCommandResponseData(string nmea)
        {
            int parseLocation = 0;
            int _pktType;
            try
            {
                // The command response packet should contain a minimum of 11 characters. '$PMTK' 'TYP' '*CK'
                // Where:
                //      $PMTK - Command response message ID (preamble + talker ID) (5-bytes)
                //      TYP   - Packet Type (000-999) to identify the message content and how to decode (3-bytes)
                //      *CK   - Checksum (3-bytes)
                parseLocation = nmea.IndexOf("$PMTK");
                if ((nmea.Length - parseLocation) < 11)
                {
                    return new CommandResponse("Not enough data in the DataArray for the Encapsulated Packet Header");
                }
                // Now that we have verified that we are working with a '$PMTK' packet of sufficient length, move beyond the preamble
                parseLocation += 5;
                // The PAcket Type should immediately follow the preamble... just in case check for a comma separating the two.
                if (nmea.Substring(parseLocation, 1).Equals(","))
                    parseLocation++;

                _pktType = int.Parse(nmea.Substring(parseLocation, 3));
                // Move past the Packet Type and comma
                parseLocation+=4;

                // Acknowledgement of PMTK Command
                if ((_pktType == (int)CommandPacketType.PMTK_ACK) && ((nmea.Length - parseLocation) >= 6)) // Cmd,Flg*CK  Note: CMD and FLG are not guaranteed to be 3 bytes each!
                {
                    CommandResponse_Ack response = new CommandResponse_Ack();
                    response.PktType = _pktType;
                    int nextDelimiter  = nmea.IndexOf(",", parseLocation);
                    response.Cmd = int.Parse(nmea.Substring(parseLocation, (nextDelimiter - parseLocation)));
                    parseLocation= nextDelimiter + 1;
                    nextDelimiter = nmea.IndexOf("*", parseLocation);
                    response.Flag = int.Parse(nmea.Substring(parseLocation, (nextDelimiter - parseLocation)));
                    return response;
                }
                // System message sent from GPS device
                else if ((_pktType == (int)CommandPacketType.PMTK_SYS_MSG) && ((nmea.Length - parseLocation) >= 6)) // Msg*CK
                {
                    CommandResponse_SysMsg response = new CommandResponse_SysMsg();
                    response.PktType = _pktType;
                    int nextDelimiter = nmea.IndexOf("*", parseLocation);
                    response.Msg = int.Parse(nmea.Substring(parseLocation, (nextDelimiter - parseLocation)));
                    return response;
                }
                // System text message sent from GPS device
                else if ((_pktType == (int)CommandPacketType.PMTK_TXT_MSG) && ((nmea.Length - parseLocation) >= 6)) // Msg*CK
                {
                    CommandResponse_TxtMsg response = new CommandResponse_TxtMsg();
                    response.PktType = _pktType;
                    //Copy everything following the Packet Type, up to but not including the checksum
                    int nextDelimiter = nmea.IndexOf("*", parseLocation);
                    response.Msg = nmea.Substring(parseLocation, (nextDelimiter - parseLocation));
                    return response;
                }
                //TODO Decode the others...
                else
                {
                    return new CommandResponse("Unknown Command or Packet Too Short!");
                }
            }
            catch (Exception e)
            {   // Possible Exceptions: ArgumentNullException, FormatException, OverflowException
                return new CommandResponse("Exception: " + e.Message + ", at parse location: " + parseLocation.ToString());
            }
        }
    }
}
