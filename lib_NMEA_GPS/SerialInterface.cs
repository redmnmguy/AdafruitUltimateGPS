using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;

namespace lib_NMEA_GPS
{
    /// <summary>
    /// Enumeration intended for use with the 'MessageContext' parameter of the SerialInterface class' NewMessagePosted Event / NewMessageEventArgs
    /// </summary>
    /// <param name="Disconnected">Indication that the serial port has been disconnected</param>
    /// <param name="Connected">Indication that the serial port connection has succeeded</param>
    /// <param name="Data">Indication that a data received from the serial port is available within the 'Message' parameter of the Message Changed Event Args</param>
    /// <param name="Fault">Indication that a fault has occurred and additional information is available within the 'Message' parameter of the Message Changed Event Args</param>
    public enum MessageContext
    {
        Disconnected,
        Connected,
        Data,
        Information,
        Fault
    }

    /// <summary>
    /// Custom EventArgs class for passing the 'Message' received on the serial port to any active listeners of the NewMessagePosted event.
    /// <param name="MessageContext">Indication of the contents of the Message parameter, as given by the MesageContext enumeration</param>
    /// <param name="Message">Error Message, Status Info., Data as indicated by the MessageContext parameter</param>
    /// </summary>
    public class NewMessageEventArgs : EventArgs
    {
        public MessageContext MessageContext { get; set; }
        public string Message { get; set; }
        public  NewMessageEventArgs(MessageContext ctx, string msg)
        {
            MessageContext = ctx;
            Message = msg;
        }
    }

    /// <summary>
    /// Class for interfacing with a serial interface port.
    /// </summary>
    /// Note: The majority of this code was derived from the Windows Developer Center's "Serial Port Sample" article.
    /// This article can be found here: https://developer.microsoft.com/en-us/windows/iot/win10/samples/serialsample
    /// This code or derivations thereof are subject to the Windows Developer Center's Terms and Conditions.
    class SerialInterface
    {
        #region Properties
        /// <summary>
        /// List of serial interfaces available
        /// </summary>
        public List<DeviceInformation> ListOfDevices;

        /// <summary>
        /// Status of serial connection
        /// </summary>
        public bool Connected { get; private set; }
        #endregion

        #region   Fields
        private SerialDevice _serialPort = null;
        private DataWriter _dataWriteObject = null;
        private DataReader _dataReaderObject = null;
        private CancellationTokenSource ReadCancellationTokenSource;
        #endregion

        /// <summary>
        /// Default Constructor
        /// </summary>      
        public SerialInterface()
        {   // Initialize the connection status to 'Disconnected'
            Connected = false;
            // Instantiate the listOfDevices....
            ListOfDevices = new List<DeviceInformation>();
            // ... call the ListAvailabelPorts function to populate the listOfDevices collection.
            ListAvailablePorts();
        }

        /// <summary>
        /// Delegate declaration for publishing 'new message' notifications (status messages or new serial data)
        /// </summary>
        public delegate void NewMessageEventHandler(object sender, NewMessageEventArgs e);

        /// <summary>
        /// An event that clients can use to be notified whenever a new status message is posted / new data is received on the serial port.
        /// <summary>
        public event NewMessageEventHandler NewMessagePosted;

        /// <summary>
        ///  Invoke the NewMessagePosted event
        /// </summary>  
        protected virtual void PostNewMessage(NewMessageEventArgs e)
        {
            NewMessagePosted?.Invoke(this, e);
        }

        /// <summary>
        /// Populate the ListOfDevices parameter with all available serial ports.
        /// </summary>
        private async void ListAvailablePorts()
        {
            try
            {
                string aqsFilter = SerialDevice.GetDeviceSelector();
                var deviceInfo = await DeviceInformation.FindAllAsync(aqsFilter);
                var port = await SerialDevice.FromIdAsync(deviceInfo[0].Id);
                string portName = port?.PortName;
                // Clear-out any existing list items
                ListOfDevices.Clear();
                foreach (var device in deviceInfo)
                {
                    ListOfDevices.Add(device);
                }
            }
            catch (Exception ex)
            {
                PostNewMessage(new NewMessageEventArgs(MessageContext.Fault, "Could not obtain a list of serial devices. " + ex.Message));
            }
        }

        /// <summary>
        /// Awaitable Task that will return a 'true' if the serial port connection completed successfully.  The task will:
        /// - Create a SerialDevice object for the serial device from the device at the DeviceListIndex within the ListOfDevices. 
        /// - Configure the settings for the serial port from the parameters passed
        /// - Create the ReadCancellationTokenSource token
        /// - Start listening for data on the serial port
        /// </summary>
        /// <param name="DeviceListIndex">Index # of the device within the ListOfDevices list to attempt to connect to.</param>
        /// <param name="BaudRate">Baud Rate setting.  Valid rates: 4800,9600,14400,19200,38400,57600,115200.  The default setting is 9600</param>
        /// <param name="DataBits">Set the length of data bits per byte. The valid range of values is from 5 - 8. The default setting is 8.</param>
        /// <param name="Parity">Set the data parity-checking protocol.  Valid settings are Even, Mark, None, Odd or Space.  The default setting is None.</param>
        /// <param name="StopBits">Set the number of stop bits per byte.  Valid settings are 1.0, 1.5 or 2.0.  The default setting is 1.0.</param>   
        /// <param name="Handshake">Set the data parity-checking protocol.  Valid settings are None, RequestToSend, RequestToSendXOnXOff or XOnXOff.  The default setting is None</param>
        /// <param name="Timeout"> Set the number of milliseconds before a time-out occurs when a read or write operation does not finish.  Valid setting is any value greater than zero.  The default setting is 500 milliseconds.</param>
        public async Task<bool> Connect(string PortName, uint BaudRate=9600, ushort DataBits=8, string Parity="None", double StopBits=1.0, string Handshake="None", double Timeout=500.0)
        {
            if(Connected==true)
            {
                PostNewMessage(new NewMessageEventArgs(MessageContext.Fault, "Connect Request Canceled, Already Connected."));
                return Connected;
            }
            if (ListOfDevices.Count <= 0)
            {
                PostNewMessage(new NewMessageEventArgs(MessageContext.Fault, "Connect Request Canceled, No Serial Devices Found!"));
                return Connected;
            }
            /*
            if ((DeviceListIndex < 0) || (DeviceListIndex > (ListOfDevices.Count-1)))
            {
                PostNewMessage(new NewMessageEventArgs(MessageContext.Fault, "Connect Request Canceled, Invalid Serial Device Index # Selected"));
                return Connected;
            }*/           
            if (PortName==null)
            {
                PostNewMessage(new NewMessageEventArgs(MessageContext.Fault, "Connect Request Canceled, No Port Name Specified"));
                return Connected;
            }
            try
            {   // Create a new serial device object.
                //_serialPort = await SerialDevice.FromIdAsync(ListOfDevices[DeviceListIndex].Id);
                int loopCount = 0;
                string aqsFilter = SerialDevice.GetDeviceSelector(PortName);
                var deviceInfo = await DeviceInformation.FindAllAsync(aqsFilter);
                // Let the user know that there was not device found associated with the PortName requested.
                if(deviceInfo.Count<=0)
                {
                    PostNewMessage(new NewMessageEventArgs(MessageContext.Fault, "Connect Request Canceled, Could not locate port name: " + PortName.ToString()));
                    return Connected;
                }
                // At least one device was located with the PortName requested.  Choose the first from the enumeration.
                DeviceInformation device = deviceInfo.First();

                // ############################################################################################################################################
                // TODO: The SerialDevice.FromIdAsync() function may return null for some time following the call to SerialDevice.FromIdAsync()
                // within the ListAvailablePorts() method... As a work-around, spin in a loop for up to a minute waiting for the method
                // to succeed.  Ref: http://stackoverflow.com/questions/37505107/serialdevice-fromidasync-yields-a-null-serial-port
                if (_serialPort == null)
                {
                    PostNewMessage(new NewMessageEventArgs(MessageContext.Information, "Serial Port Busy, Waiting to Connect...."));
                    do
                    {
                        loopCount++;
                        _serialPort = await SerialDevice.FromIdAsync(device.Id);
                        if (_serialPort == null)
                            await Task.Delay(100);
                    } while (_serialPort == null && loopCount < 600);
                    // Give-up if still null after waiting. ;-(
                    if (_serialPort == null)
                    {
                        PostNewMessage(new NewMessageEventArgs(MessageContext.Fault, "Connect Request Canceled, Time-Out Waiting on Serial Port!"));
                        return Connected;
                    }
                    else
                    {
                        PostNewMessage(new NewMessageEventArgs(MessageContext.Information, "Serial found after " + (((float)loopCount)/10.0).ToString("0.#")+ " Second Wait."));
                    }
                }
                // ############################################################################################################################################

                // Configure serial settings
                if (_serialPort != null)   // not needed unless/until the re-check/timeout logic above goes away.
                {
                    // Validate and apply the serial port settings
                    // BaudRate
                    if ((BaudRate == 4800) || (BaudRate == 9600) || (BaudRate == 14400) || (BaudRate == 19200) || (BaudRate == 38400) || (BaudRate == 57600) || (BaudRate == 115200))
                    {
                        _serialPort.BaudRate = BaudRate;
                    }
                    else
                    {
                        PostNewMessage(new NewMessageEventArgs(MessageContext.Fault, "Incorrect connection parameter; BaudRate = " + BaudRate.ToString() + ".  Setting BaudRate to default (9600)."));
                        _serialPort.BaudRate = 9600;
                    }
                    // DataBits
                    if( (DataBits>=5) && (DataBits<=8))
                    {
                        _serialPort.DataBits = DataBits;
                    }
                    else
                    {
                        PostNewMessage(new NewMessageEventArgs(MessageContext.Fault, "Incorrect connection parameter; DataBits = " + DataBits.ToString() + ".  Setting DataBits to default (8)."));
                        _serialPort.DataBits = 8;
                    }
                    // Parity.  Convert char to SerialParity enum.
                    if (Parity.ToLower().Equals("even"))
                    {
                        _serialPort.Parity = SerialParity.Even;
                    }
                    else if (Parity.ToLower().Equals("mark"))
                    {
                        _serialPort.Parity = SerialParity.Mark;
                    }
                    else if (Parity.ToLower().Equals("none"))
                    {
                        _serialPort.Parity = SerialParity.None;
                    }
                    else if (Parity.ToLower().Equals("odd"))
                    {
                        _serialPort.Parity = SerialParity.Odd;
                    }
                    else if (Parity.ToLower().Equals("space"))
                    {
                        _serialPort.Parity = SerialParity.Space;
                    }        
                    else
                    {
                        _serialPort.Parity = SerialParity.None;
                        PostNewMessage(new NewMessageEventArgs(MessageContext.Fault, "Incorrect connection parameter; Parity = '" + Parity + "'.  Setting Parity to default (None)."));
                    }
                    // StopBits.  Convert double to SerialStopBitCount enum.
                    if (StopBits==1.0)
                    {
                        _serialPort.StopBits = SerialStopBitCount.One;
                    }
                    else if (StopBits == 1.5)
                    {
                        _serialPort.StopBits = SerialStopBitCount.OnePointFive;
                    }
                    else if (StopBits == 2.0)
                    {
                        _serialPort.StopBits = SerialStopBitCount.Two;
                    }
                    else 
                    {
                        PostNewMessage(new NewMessageEventArgs(MessageContext.Fault, "Incorrect connection parameter; StopBits = '" + StopBits.ToString() + "'.  Setting StopBits to default (1.0)."));
                        _serialPort.StopBits = SerialStopBitCount.One;
                    }
                    // Handshake.  Convert string to SerialHandshake enum.
                    if (Handshake.ToLower().Equals("none"))
                    {
                        _serialPort.Handshake = SerialHandshake.None;
                    }
                    else if (Handshake.ToLower().Equals("requesttosend"))
                    {
                        _serialPort.Handshake = SerialHandshake.RequestToSend;
                    }
                    else if (Handshake.ToLower().Equals("requesttosendxonxoff"))
                    {
                        _serialPort.Handshake = SerialHandshake.RequestToSendXOnXOff;
                    }
                    else if (Handshake.ToLower().Equals("xonxoff"))
                    {
                        _serialPort.Handshake = SerialHandshake.XOnXOff;
                    }
                    else
                    {      _serialPort.Handshake = SerialHandshake.None;
                            PostNewMessage(new NewMessageEventArgs(MessageContext.Fault, "Incorrect connection parameter; Handshake = '" + Handshake + "'.  Setting SerialHandshake to default (None)."));
                    }
                    // Read/Write Timeout
                    if (Timeout > 0)
                    {
                        _serialPort.WriteTimeout = TimeSpan.FromMilliseconds(Timeout);
                        _serialPort.ReadTimeout = TimeSpan.FromMilliseconds(Timeout);
                    }

                    // Create cancellation token object to close I/O operations when closing the device
                    ReadCancellationTokenSource = new CancellationTokenSource();
                    Connected = true;
                    PostNewMessage(new NewMessageEventArgs(MessageContext.Connected, "Connected!"));
                    // Start listening for incoming data on the serial port.
                    Listen();
                }
                else
                {
                    PostNewMessage(new NewMessageEventArgs(MessageContext.Fault, "Serial Port Interface Memory Not Allocated!"));
                    return Connected;
                }
            }
            catch (Exception ex)
            {
                PostNewMessage(new NewMessageEventArgs(MessageContext.Fault, "Connection to serial port failed: " + ex.Message));
            }
            return Connected;
        }

        /// <summary>
        /// Awaitable Task that will return a 'true' if the DataToWrite was written to the serial port successfully
        /// </summary>
        /// <param name="DataToWrite"></param>
        public async Task<bool> WriteAsync(string DataToWrite)
        {
            bool _writeOK = false;
            try
            {
                if (_serialPort != null)
                {
                    // Create the DataWriter object and attach to OutputStream
                    _dataWriteObject = new DataWriter(_serialPort.OutputStream);

                    //Launch the WriteAsync task to perform the write
                    Task<UInt32> storeAsyncTask;

                    if (DataToWrite.Length != 0)
                    {   // Load the text from the sendText input text box to the dataWriter object
                        _dataWriteObject.WriteString(DataToWrite);              
                        // Launch an async task to complete the write operation
                        storeAsyncTask = _dataWriteObject.StoreAsync().AsTask();

                        UInt32 bytesWritten = await storeAsyncTask;
                        if (bytesWritten > 0)
                        {
                            _writeOK = true;
                            PostNewMessage(new NewMessageEventArgs(MessageContext.Fault, "Serial Write: " + bytesWritten.ToString() + " bytes written successfully."));
                        }
                        else
                            PostNewMessage(new NewMessageEventArgs(MessageContext.Fault, "Serial Write Error: Zero bytes written."));
                    }
                    else
                    {
                        PostNewMessage(new NewMessageEventArgs(MessageContext.Fault, "Serial Write Error: Data length must be greater than zero."));
                    }

                }
                else
                {
                    PostNewMessage(new NewMessageEventArgs(MessageContext.Fault, "Serial Write Error: Not Connected."));
                }
            }
            catch (Exception ex)
            {
                PostNewMessage(new NewMessageEventArgs(MessageContext.Fault, "Serial Write Error: " + ex.Message));
            }
            finally
            {
                // Cleanup once complete
                if (_dataWriteObject != null)
                {
                    _dataWriteObject.DetachStream();
                    _dataWriteObject = null;
                }
            }
            return _writeOK;
        }

        /// <summary>
        /// An async task to read from the SerialDevice InputStream
        /// </summary>
        private async void Listen()
        {
            try
            {
                if (_serialPort != null)
                {
                    _dataReaderObject = new DataReader(_serialPort.InputStream);

                    // keep reading the serial input
                    while (Connected)
                    {
                        await ReadAsync(ReadCancellationTokenSource.Token);
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.GetType().Name == "TaskCanceledException")
                {
                    PostNewMessage(new NewMessageEventArgs(MessageContext.Fault, "Reading task was canceled, closing device and cleaning up..."));
                    CloseDevice();
                }
                else
                {
                    PostNewMessage(new NewMessageEventArgs(MessageContext.Fault, "Error Reading Data from serial port: " + ex.Message));
                }
            }
            finally
            {
                // Cleanup once complete
                if (_dataReaderObject != null)
                {
                    _dataReaderObject.DetachStream();
                    _dataReaderObject = null;
                }
            }
        }

        /// <summary>
        /// Task that waits on data and reads asynchronously from the serial device InputStream
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task ReadAsync(CancellationToken cancellationToken)
        {
            Task<UInt32> loadAsyncTask;

            uint ReadBufferLength = 1024;
            try
            {
                // If task cancellation was requested, comply
                cancellationToken.ThrowIfCancellationRequested();

                // Set InputStreamOptions to complete the asynchronous read operation when one or more bytes is available
                _dataReaderObject.InputStreamOptions = InputStreamOptions.Partial;
                // Create a task object to wait for data on the serialPort.InputStream
                loadAsyncTask = _dataReaderObject.LoadAsync(ReadBufferLength).AsTask(cancellationToken);

                // Launch the task and wait
                UInt32 bytesRead = await loadAsyncTask;
                if (bytesRead > 0)
                {
                    //dataReceived = dataReaderObject.ReadString(bytesRead);
                    PostNewMessage(new NewMessageEventArgs(MessageContext.Data, _dataReaderObject.ReadString(bytesRead)));
                }
            }
            catch(Exception ex)
            {
                PostNewMessage(new NewMessageEventArgs(MessageContext.Fault, "Serial Interface Data Read Error: " + ex.Message.ToString()));
            }
        }

        /// <summary>
        /// Uses the ReadCancellationTokenSource to cancel read operations
        /// </summary>
        private void CancelReadTask()
        {
            if (ReadCancellationTokenSource != null)
            {
                if (!ReadCancellationTokenSource.IsCancellationRequested)
                {
                    ReadCancellationTokenSource.Cancel();
                }
            }
        }

        /// <summary>
        /// Dispose of the SerialDevice object and clear the ListofDevices
        /// </summary>
        private void CloseDevice()
        {
            if (_serialPort != null)
            {
                _serialPort.Dispose();
            }
            _serialPort = null;
            Connected = false;
            PostNewMessage(new NewMessageEventArgs(MessageContext.Disconnected, "Serial Port Disconnected"));
            ListOfDevices.Clear();
        }

        /// <summary>
        /// Cancel all read operation, close and dispose of the SerialDevice object
        /// </summary>
        public void Disconnect()
        {
            try
            {
                CancelReadTask();
                CloseDevice();
                ListAvailablePorts();
            }
            catch (Exception ex)
            {
                PostNewMessage(new NewMessageEventArgs(MessageContext.Fault, ex.Message));
            }
        }

    }
}
