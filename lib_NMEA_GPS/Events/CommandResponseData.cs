using lib_NMEA_GPS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib_NMEA_GPS
{
    public class CommandResponseDataEventArgs : EventArgs
    {
        public CommandResponse CommandRespose { get; set; }
        /// <summary>
        /// CommandResponse parameter initialization constructor
        /// </summary>
        /// <param name="refNewObject">Reference to a new CommandResponse object or an object derived from CommandDesponse</param>
        public CommandResponseDataEventArgs(CommandResponse refNewObject)
        {  
            CommandRespose = refNewObject;
        }
    }

    public partial class NMEA_GPS
    {
        /// <summary>
        /// Delegate declaration for publishing notifications of new data received from serial interface
        /// </summary>
        public delegate void CommandResponseDataEventHandler(object sender, CommandResponseDataEventArgs e);

        /// <summary>
        /// An event that clients can use to be notified whenever new data is received
        /// </summary>
        public event CommandResponseDataEventHandler CommandResponseDataReceivedEvent;

        /// <summary>
        ///  Invoke the DataReceived event
        /// </summary>  
        protected virtual void CommandResponseDataChanged(CommandResponseDataEventArgs data)
        {   if(data.CommandRespose != null)  // Don't bother invoking the event if there is no CommandResponse data
                CommandResponseDataReceivedEvent?.Invoke(this, data);
        }
    }
}
