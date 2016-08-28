using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib_NMEA_GPS
{

    public enum CommandPacketType
    {
        PMTK_ACK =              1,
        PMTK_SYS_MSG =          10,
        PMTK_TXT_MSG =          11,
        PMTK_CMD_HOT_START =    101,
        PMTK_CMD_WARM_START =   102
        // TODO Add others.. 
    }
    /// <summary>
    /// Base Class for storing command packet response received from the NMEA GPS Device
    /// </summary>
    public class CommandResponse 
    {
        public string MessageID { get { return "$PMTK"; } set { } }
        public string ErrorMessage { get; set; }
        public int PktType { get; set; }
        public CommandResponse() { }
        public CommandResponse(string error) { ErrorMessage = error; }
    }
    
    public class CommandResponse_Ack : CommandResponse
    {
        public int Cmd { get; set; }
        public int Flag { get; set; }
    }
    public class CommandResponse_SysMsg : CommandResponse
    {
        public int Msg { get; set; }
    }
    public class CommandResponse_TxtMsg : CommandResponse
    {
        public string Msg { get; set; }
    }
}
