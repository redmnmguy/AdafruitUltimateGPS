using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;

namespace lib_NMEA_GPS
{
    public partial class NMEA_GPS
    {
        // Data objects temporary storage of the corresponding parsed data
        private FixedData _fixedData = new FixedData();
        private ActiveSatellitesData _activeSatellitesData = new ActiveSatellitesData();
        private SatellitesInViewData _satellitesInViewData = new SatellitesInViewData();
        private MinimumNavigationData _minimumNavigationData = new MinimumNavigationData();
        private CourseAndSpeedData _courseAndSpeedData = new CourseAndSpeedData();
        private CommandResponse _commandResponseData = new CommandResponse();
        // The message data received from the GPS device can contain several individual data packets
        // Each data packet will be separated by carriage return and line feed (\r\n).
        private string[] _nmeaStringSeparator = new string[] { "\r\n" };
        // The SerialInterfae object provides read, write, configuration and events notification from the serial port
        private SerialInterface _serialInterface;
        private int _messageCount = 0;

        public bool Connectted { get { return _serialInterface.Connected; } }

        /// <summary>
        /// List of Serial Interface IDs available
        /// </summary>
        public List<DeviceInformation> ListOfDevices
        {   // Just pass along the serial interface's list of devices
            get { return _serialInterface.ListOfDevices; }
            set { }
        }

        public NMEA_GPS()
        {   // Instantiate the SerialInterface object 
            _serialInterface = new SerialInterface();
            // and subscribe to its NewDataReceived event
            _serialInterface.NewMessagePosted += SerialInterface_NewMessageReceived;
        }

        /// <summary>
        /// Receive notification from the SerialInterface, via its NewDateReceived event, that new data has been received on the serial port.
        /// TODO Skip parsing of any individual message packet where there are no subscribers??  If a tree falls in the woods....
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SerialInterface_NewMessageReceived(object sender, NewMessageEventArgs e)
        {
            #region MessageContext.Data
            // The SerialInterface object is letting us know that new data has been received on the serial port
            // and a copy of that data is contained within the 'Message' property of the event args.
            if (e.MessageContext == MessageContext.Data)
            { 
                // Increment our internal message counter.
                _messageCount++;
                // Let any subscribers to our SerialDataReceivedEvent that new data has been received,
                // include the data and the value of our message counter.
                SerialDataChanged(e.Message, _messageCount);
                try
                {
                    // Split the data received into individual message packets.  Each individual message packet will be
                    // separated by carriage return and line feed (\r\n), which is stored in the 'nmeaStringSeparator' variable.
                    string[] _dataArray = e.Message.Split(_nmeaStringSeparator, StringSplitOptions.None);
                    int _count = _dataArray.Length;
                    // Look for the appropriate parser for each of the individual message packets
                    for (int _index = 0; _index < _count; _index++)
                    {
                        // Check if this message packet starts with "$GPGGA" (Fixed Data) 
                        if (_dataArray[_index].IndexOf(_fixedData.MessageID) == 0)
                        {   // Clear parameter data from last 'Parse' before calling 'Parse' again
                            _fixedData.Clear();
                            if (ParseFixedData(_dataArray[_index], _fixedData))
                            {   // The parse of the dataArray was successful, increment the Fixed Data MessageCount
                                _fixedData.MessageCount++;
                                // Let subscribers to the FixedDataReceivedEvent know that a new Fixed Data sentence was
                                // received passing along a copy of the Fixed Data values parsed from the sentence.
                                FixedDataChanged(new FixedDataEventArgs(_fixedData));
                            }
                            else if (_fixedData.ErrorMessage != null)
                            {
                                // Report to any subscribers of the StatusInformationChangedEvent that there was a problem parsing the Fixed Data sentence.
                                StatusInformationChanged(MessageContext.Fault, "Fixed Data Sentence Parse Error: "+_fixedData.ErrorMessage);
                            }
                        }
                        // Check if this message packet starts with "$GPGSA" (Active Satellites) 
                        else if (_dataArray[_index].IndexOf(_activeSatellitesData.MessageID) == 0)
                        {   // Clear parameter data from last 'Parse' before calling 'Parse' again
                            _activeSatellitesData.Clear();
                            if (ParseActiveSatellitesData(_dataArray[_index], _activeSatellitesData))
                            {   // The parse of the dataArray was successful, increment the Active Satellites MessageCount
                                _activeSatellitesData.MessageCount++;
                                // Let subscribers to the ActiveSatellitesDataReceivedEvent know that a new Active Satellites sentence was
                                // received passing along a copy of the Active Satellites Data values parsed from the sentence.
                                ActiveSatellitesDataChanged(new ActiveSatellitesDataEventArgs(_activeSatellitesData));
                            }
                            else if (_activeSatellitesData.ErrorMessage!= null)
                            {
                                // Report to any subscribers of the StatusInformationChangedEvent that there was a problem parsing the Active Satellites packet.
                                StatusInformationChanged(MessageContext.Fault, "Active Satellites Sentence Parse Error: " + _activeSatellitesData.ErrorMessage);
                            }
                        }
                        // Check if the dataArray contains "$GPGSV"  (Satellites In View) 
                        else if (_dataArray[_index].IndexOf(_satellitesInViewData.MessageID) == 0)
                        {   // We may get multiple packets in the same serial message, so we cannot call clear() here!
                            // SatellitesInViewData.Clear();
                            if (ParseSatellitesInViewData(_dataArray[_index], _satellitesInViewData))
                            {   // The parse of the dataArray was successful, increment the Satellites In View Message Counter
                                _satellitesInViewData.MessageCount++;
                                // Create a new ActiveSatellitesDataObject in memory and let the constructor copy the Property values
                                // contained within our local ActiveSatellitesData object which parsed the serial data received.
                                SatellitesInViewDataChanged(new SatellitesInViewDataEventArgs(_satellitesInViewData));  
                                // clear-out the local storage here??
                            }
                            else if(_satellitesInViewData.ErrorMessage != null)
                            {
                                // Report to any subscribers of the SatellitesInViewDataReceivedEvent that there was a problem parsing the Satellites In View Data sentence.
                                StatusInformationChanged(MessageContext.Fault, "Satellites In View Data Sentence Parse Error: " + _satellitesInViewData.ErrorMessage);
                            }
                        }
                        // Check if the dataArray starts with  "$GPRMC"  (Recommended Minimum Navigation Information) 
                        else if (_dataArray[_index].IndexOf(_minimumNavigationData.MessageID)==0)
                        {   // Clear parameter data from last 'Parse' before calling 'Parse' again
                            _minimumNavigationData.Clear();
                            if (ParseMinimumNavigationData(_dataArray[_index], _minimumNavigationData))
                            {   // The parse of the dataArray was successful, increment the Minimum Navigation MessageCount
                                _minimumNavigationData.MessageCount++;
                                // Let subscribers to the MinimumNavigationDataReceivedEvent know that a new Minimum Navigation Data sentence was
                                // received passing along a copy of the Minimum Navigation Data values parsed from the sentence.
                                MinimumNavigationDataChanged(new MinimumNavigationDataEventArgs(_minimumNavigationData));
                             }
                            else if (_minimumNavigationData.ErrorMessage != null)
                            {
                                // Report to any subscribers of the StatusInformationChangedEvent that there was a problem parsing the Minimum Navigation Data sentence.
                                StatusInformationChanged(MessageContext.Fault, "Minimum Navigation Data Sentence Parse Error: " + _minimumNavigationData.ErrorMessage);
                            }
                        }
                        // Check if the dataArray starts with "$GPVTG"  (Course and Speed Information) 
                        else if (_dataArray[_index].IndexOf(_courseAndSpeedData.MessageID) == 0)
                        {   // Clear parameter data from last 'Parse' before calling 'Parse' again
                            _courseAndSpeedData.Clear();
                            if (ParseCourseAndSpeedData(_dataArray[_index], _courseAndSpeedData))
                            {   // The parse of the dataArray was successful, increment the Course and Speed Data MessageCount
                                _courseAndSpeedData.MessageCount++;
                                // Let subscribers to the CourseAndSpeedDataChangedEvent know that a new Course and Speed sentence was
                                // received passing along a copy of the Course and Speed Data values parsed from the sentence.
                                CourseAndSpeedDataChanged(new CourseAndSpeedDataEventArgs(_courseAndSpeedData));
                            }
                            else if (_courseAndSpeedData.ErrorMessage != null)
                            {
                                // Report to any subscribers of the StatusInformationChangedEvent that there was a problem parsing the Course and Speed Data sentence.
                                StatusInformationChanged(MessageContext.Fault, "Course and Speed Data Sentence Parse Error: " + _courseAndSpeedData.ErrorMessage);
                            }
                        }
                        // Check if the dataArray starts with "$PMTK"  (Command Response Information) 
                        else if (_dataArray[_index].IndexOf(_commandResponseData.MessageID) == 0)
                        {   // Parse the command response and let subscribers of the CommandResponseDataChangedEven know that a new response
                            // was received passing along response within an appropriate CommandResponse or CommandResponse derived class.
                            CommandResponseDataChanged(new CommandResponseDataEventArgs(ParseCommandResponseData(_dataArray[_index])));
                        }
                        //TODO: On Power-up, $PGACK messages are observed.  However, there is no definition of the $PGACK  
                        // packet type within the data sheet for the GPS chipset used to develop this class library. 
                        else if (_dataArray[_index].Length > 0)
                        {  // Report back any packet (except an empty string) that we didn't recognize and know how to parse
                            StatusInformationChanged(MessageContext.Fault, "Unknown Data Packet: " + _dataArray[_index]);
                        }   
                    }
                }
                catch (Exception ex)
                {
                    StatusInformationChanged(MessageContext.Fault, "Error Trying to Parse Serial Data: " + ex.Message);
                }
            }
            #endregion
            else
            {  // The MessageContext indicates that the 'Message' property does not contain 'Data'.
               // The 'Message' therefore contains a Connect, Disconnect or Error message.  Pass
               // the 'Message' along as Status Information to listeners of the Status Information Changed Event.
                StatusInformationChanged(e.MessageContext, e.Message);
            }
        }

        /// <summary>
        /// Awaitable Task that will return a 'true' if the serial port connection completed successfully, using the serial settings supplied.
        /// </summary>
        /// <param name="DeviceListIndex">Index # of the device within the ListOfDevices list to attempt to connect to</param>
        /// <param name="BaudRate">Baud Rate setting.  Valid rates: 4800,9600,14400,19200,38400,57600,115200.  The default setting is 9600</param>
        /// <param name="DataBits">Set the length of data bits per byte. The valid range of values is from 5 - 8. The default setting is 8.</param>
        /// <param name="Parity">Set the data parity-checking protocol.  Valid settings are Even, Mark, None, Odd or Space.  The default setting is None.</param>
        /// <param name="StopBits">Set the number of stop bits per byte.  Valid settings are 1.0, 1.5 or 2.0.  The default setting is 1.0.</param>   
        /// <param name="Handshake">Set the data parity-checking protocol.  Valid settings are None, RequestToSend, RequestToSendXOnXOff or XOnXOff.  The default setting is None</param>
        /// <param name="Timeout"> Set the number of milliseconds before a time-out occurs when a read or write operation does not finish.  Valid setting is any value greater than zero.  The default setting is 500 milliseconds.</param>
        public async Task<bool> Connect(string PortName, uint BaudRate = 9600, ushort DataBits = 8, string Parity = "None", double StopBits = 1.0, string Handshake = "None", double Timeout = 500.0)
        {
            return await _serialInterface.Connect(PortName, BaudRate, DataBits, Parity, StopBits, Handshake, Timeout);
        }

        /// <summary>
        /// Disconnect from the serial device.
        /// </summary>
        ///<returns>Serial port connection status; false if disconnect is successful</returns>
        public bool DisConnect()
        {
            _serialInterface.Disconnect();
            return _serialInterface.Connected;
        }

        /// <summary>
        /// Awaitable task that sends the data contained within the CommandPacket parameter to the serial device.  The data can be any string of UTF-16 characters.
        /// </summary>
        /// <param name="CommandPacket"></param>
        ///<returns>Returns true if the data within the CommandPacket parameter is written successfully, false otherwise.  The return does not indicate that the serial device received nor accepted the data, only that the data was set. </returns>
        public async Task<bool> SendAsync(string CommandPacket)
        {
            return await _serialInterface.WriteAsync(CommandPacket);
        }

        /// <summary>
        /// Awaitable task that sends a pre-defined command to the serial device.  The pre-built command to send is specified by passing one of the items from the gpsDeviceCommand enumeration.
        /// </summary>
        /// <param name="cmd">Item from the gpsDeviceCommand enumeration which indicates the command sentence to send.</param>
        /// <returns>Returns true if the command is written successfully, false otherwise.  The return does not indicate that the serial device received nor accepted the command, only that the command was set. </returns>
        /// Note: Many of the pre-built commands originate from the Adafruit_GPS library: https://github.com/adafruit/Adafruit_GPS
        public async Task<bool> SendAsync(gpsDeviceCommand cmd)
        {
            string _command;
            switch (cmd)
            {
                case gpsDeviceCommand.PMTK_TEST:
                    _command = "$PMTK000*32\r\n";
                    break;
                case gpsDeviceCommand.PMTK_CMD_HOT_START:
                    _command = "$PMTK101*32\r\n";
                    break;
                case gpsDeviceCommand.PMTK_CMD_WARM_START:
                    _command = "$PMTK102*31\r\n";
                    break;
                case gpsDeviceCommand.PMTK_CMD_COLD_START:
                    _command = "$PMTK103*30\r\n";
                    break;
                case gpsDeviceCommand.PMTK_CMD_FULL_COLD_START:
                    _command = "$PMTK104*37\r\n";
                    break;
                // Extend the PMTK_SET_NEMA_UPDATERATE for individual rates
                case gpsDeviceCommand.PMTK_SET_NMEA_UPDATERATE_100_MILLIHERTZ:
                    _command = "$PMTK220,10000*2F\r\n";
                    break;
                case gpsDeviceCommand.PMTK_SET_NMEA_UPDATERATE_200_MILLIHERTZ:
                    _command = "$PMTK220,5000*1B\r\n";
                    break;
                case gpsDeviceCommand.PMTK_SET_NMEA_UPDATERATE_1HZ:
                    _command = "$PMTK220,1000*1F\r\n";
                    break;
                case gpsDeviceCommand.PMTK_SET_NMEA_UPDATERATE_5HZ:
                    _command = "$PMTK220,200*2C\r\n";
                    break;
                case gpsDeviceCommand.PMTK_SET_NMEA_UPDATERATE_10HZ:
                    _command = "$PMTK220,100*2F\r\n";
                    break;
                case gpsDeviceCommand.PMTK_SET_NMEA_BAUDRATE_4800:
                    _command = "$PMTK251,4800*30\r\n";
                    break;
                case gpsDeviceCommand.PMTK_SET_NMEA_BAUDRATE_9600:
                    _command = "$PMTK251,9600*17\r\n";
                    break;
                case gpsDeviceCommand.PMTK_SET_NMEA_BAUDRATE_14400:
                    _command = "$PMTK251,14400*0D\r\n";
                    break;
                case gpsDeviceCommand.PMTK_SET_NMEA_BAUDRATE_19200:
                    _command = "$PMTK251,19200*06\r\n"; 
                    break;
                case gpsDeviceCommand.PMTK_SET_NMEA_BAUDRATE_38400:
                    _command = "$PMTK251,38400*27\r\n";     
                    break;
                case gpsDeviceCommand.PMTK_SET_NMEA_BAUDRATE_57600:
                    _command = "$PMTK251,57600*2C\r\n";
                    break;
                case gpsDeviceCommand.PMTK_SET_NMEA_BAUDRATE_115200:
                    _command = "$PMTK251,115200*3B\r\n"; 
                    break;
                case gpsDeviceCommand.PMTK_SET_NMEA_FIX_CTL_100_MILLIHERTZ:
                    _command = "$PMTK300,10000,0,0,0,0*2C\r\n";
                    break;
                case gpsDeviceCommand.PMTK_SET_NMEA_FIX_CTL_200_MILLIHERTZ:
                    _command = "$PMTK300,5000,0,0,0,0*18\r\n";
                    break;
                case gpsDeviceCommand.PMTK_SET_NMEA_FIX_CTL_1HZ:
                    _command = "$PMTK300,1000,0,0,0,0*1C\r\n";
                    break;
                case gpsDeviceCommand.PMTK_SET_NMEA_FIX_CTL_5HZ:
                    _command = "$PMTK300,200,0,0,0,0*2F\r\n";
                    break;
                case gpsDeviceCommand.PMTK_SET_NMEA_FIX_CTL_10HZ:
                    _command = "$PMTK300,100,0,0,0,0*08\r\n"; 
                    break;
                // Extend the PMTK_API_SET_NMEA_OUTPUT command for enabling/disabling (not frequency change) of 'some' sentence combinations  
                case gpsDeviceCommand.PMTK_API_SET_NMEA_OUTPUT_RMC_ONLY:
                    _command = "$PMTK314,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0*29\r\n";
                    break;
                case gpsDeviceCommand.PMTK_API_SET_NMEA_OUTPUT_RMC_GGA:
                    _command = "$PMTK314,0,1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0*28\r\n";
                    break;
                case gpsDeviceCommand.PMTK_API_SET_NMEA_OUTPUT_ALLDATA:
                    _command = "$PMTK314,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0*28\r\n";
                    break;
                case gpsDeviceCommand.PMTK_API_SET_NMEA_OUTPUT_OFF:
                    _command = "$PMTK314,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0*28\r\n";
                    break;
                case gpsDeviceCommand.LOCUS_STARTLOG:
                    _command = "$PMTK185,0*22\r\n";
                    break;
                case gpsDeviceCommand.LOCUS_STOPLOG:
                    _command = "$PMTK185,1*23\r\n";
                    break;
                case gpsDeviceCommand.LOCUS_STARTSTOPACK:
                    _command = "$PMTK001,185,3*3C\r\n";
                    break;
                case gpsDeviceCommand.LOCUS_QUERY_STATUS:
                    _command = "$PMTK183*38\r\n";
                    break;
                case gpsDeviceCommand.LOCUS_ERASE_FLASH:
                    _command = "$PMTK184,1*22\r\n";
                    break;
                case gpsDeviceCommand.ENABLE_SBAS:
                    _command = "$PMTK313,1*2E\r\n";
                    break;
                case gpsDeviceCommand.ENABLE_WAAS:
                    _command = "$PMTK301,2*2E\r\n";
                    break;
                case gpsDeviceCommand.STANDBY:
                    _command = "$PMTK161,0*28\r\n";
                    break;
                case gpsDeviceCommand.STANDBY_SUCCESS:
                    _command = "$PMTK001,161,3*36\r\n";
                    break;
                case gpsDeviceCommand.AWAKE:
                    _command = "$PMTK010,002*2D\r\n";
                    break;
                case gpsDeviceCommand.QUERY_RELEASE:
                    _command = "$PMTK605*31\r\n";
                    break;
                case gpsDeviceCommand.PGCMD_ANTENNA:
                    _command = "$PGCMD,33,1*6C\r\n";
                    break;
                case gpsDeviceCommand.PGCMD_NOANTENNA:
                    _command = "$PGCMD,33,0*6D\r\n";
                    break;
                default:
                    _command = null;
                    break;
            }
            // Send the command string
            if (_command != null)
                return await _serialInterface.WriteAsync(_command);
            else
                return false;
        }

    }
    /// <summary>
    /// Enumeration for use with the SendAsync(gpsDeviceCommand cmd) method.  Each item in the enumeration specifies a pre-built command within the SendAsync(gpsDeviceCommand cmd) method.
    /// </summary>
    /// Note: Many of the pre-built commands and names used within the enumeration originate from the Adafruit_GPS library: https://github.com/adafruit/Adafruit_GPS
    public enum gpsDeviceCommand
    {
        PMTK_TEST,                      // Test Command (no action, just acknowledgement return)
        PMTK_CMD_HOT_START,
        PMTK_CMD_WARM_START,
        PMTK_CMD_COLD_START,
        PMTK_CMD_FULL_COLD_START,
        // Commands to set the update rate from once a second (1 Hz) to 10 times a second (10Hz)
        // Note that these only control the rate at which the position is echoed, to actually speed up the
        // position fix you must also send one of the position fix rate commands below too.
        PMTK_SET_NMEA_UPDATERATE_100_MILLIHERTZ,     // Once every 10 seconds, 100 millihertz.
        PMTK_SET_NMEA_UPDATERATE_200_MILLIHERTZ,     // Once every 5 seconds, 200 millihertz.
        PMTK_SET_NMEA_UPDATERATE_1HZ,
        PMTK_SET_NMEA_UPDATERATE_5HZ,
        PMTK_SET_NMEA_UPDATERATE_10HZ,
        // Commands for setting the baud rate
        PMTK_SET_NMEA_BAUDRATE_4800,
        PMTK_SET_NMEA_BAUDRATE_9600,
        PMTK_SET_NMEA_BAUDRATE_14400,
        PMTK_SET_NMEA_BAUDRATE_19200,
        PMTK_SET_NMEA_BAUDRATE_38400,
        PMTK_SET_NMEA_BAUDRATE_57600,
        PMTK_SET_NMEA_BAUDRATE_115200,
        // Position fix update rate commands.
        PMTK_SET_NMEA_FIX_CTL_100_MILLIHERTZ,     // Once every 10 seconds, 100 millihertz.
        PMTK_SET_NMEA_FIX_CTL_200_MILLIHERTZ,     // Once every 20 seconds, 200 millihertz.
        PMTK_SET_NMEA_FIX_CTL_1HZ,
        PMTK_SET_NMEA_FIX_CTL_5HZ,                // Can't fix position faster than 5 times a second!
        PMTK_SET_NMEA_FIX_CTL_10HZ,
        // Commands for extending the PMTK_API_SET_NMEA_OUTPUT packet to enable/disable specific NMEA sentences.
        // Note: There are many other possibilities including options for setting the output frequency of the individual sentences.
        PMTK_API_SET_NMEA_OUTPUT_RMC_ONLY,       // turn on only the second sentence (GPRMC)
        PMTK_API_SET_NMEA_OUTPUT_RMC_GGA,        // turn on GPRMC and GGA                                           
        PMTK_API_SET_NMEA_OUTPUT_ALLDATA,        // turn on ALL THE DATA
        PMTK_API_SET_NMEA_OUTPUT_OFF,            // turn off output

        LOCUS_STARTLOG,
        LOCUS_STOPLOG,
        LOCUS_STARTSTOPACK,
        LOCUS_QUERY_STATUS,
        LOCUS_ERASE_FLASH,
        // standby command & boot successful message
        ENABLE_SBAS,
        ENABLE_WAAS,
        STANDBY,
        STANDBY_SUCCESS,
        AWAKE,
        QUERY_RELEASE,              // ask for the release and version
        // request for updates on antenna status
        PGCMD_ANTENNA,
        PGCMD_NOANTENNA
    }

}
