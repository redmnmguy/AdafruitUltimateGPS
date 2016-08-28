using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib_NMEA_GPS
{

    /// <summary>
    /// Custom EventArgs class for passing status information
    /// along with the status information changed event.
    /// </summary>
    public class StatusInformationEventArgs : EventArgs
    {
        public MessageContext MessageContext { get; set; }
        public string StatusMessage { get; set; }

        // Default Constructor
        public StatusInformationEventArgs()
        {
        }
        // 'Status Message' Constructor
        public StatusInformationEventArgs(MessageContext msgContext, string stsMsg)
        {
            MessageContext = msgContext;
            StatusMessage = stsMsg;
        }
    }

    public partial class NMEA_GPS
    {
        /// <summary>
        /// Delegate declaration for publishing notifications that status information has been updated
        /// </summary>
        public delegate void StatusInformationEventHandler(object sender, StatusInformationEventArgs e);

        /// <summary>
        /// An event that clients can use to be notified whenever status information has been updated
        /// </summary>
        public event StatusInformationEventHandler StatusInformationChangedEvent;

        /// <summary>
        ///  Invoke the Status Information Changed event(s)
        /// </summary>  
        protected virtual void StatusInformationChanged(MessageContext msgContext, string stsMsg)
        {
            StatusInformationChangedEvent?.Invoke(this, new StatusInformationEventArgs(msgContext, stsMsg));
        }
    }
}
