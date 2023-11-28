using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.processapplication;
using gip.mes.processapplication;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace soehnle.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Scale Terminal 30xx'}de{'Waage Terminal 30xx'}", Global.ACKinds.TPAModule, Global.ACStorableTypes.Required, false, true)]
    public abstract class PAETerminal3xxxBase : PAEScaleCalibratableMES, IACPAESerialPort
    {
        #region c'tors
        static PAETerminal3xxxBase()
        {
            RegisterExecuteHandler(typeof(PAETerminal3xxxBase), HandleExecuteACMethod_PAETerminal3xxxBase);
        }


        public PAETerminal3xxxBase(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") :
               base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACPostInit()
        {
            CurrentScaleMode = PAScaleMode.Init;
            if (PollingInterval <= 0)
                PollingInterval = 2000;
            if (ACOperationMode == ACOperationModes.Live)
            {
                _ShutdownEvent = new ManualResetEvent(false);
                _PollThread = new ACThread(Poll);
                _PollThread.Name = "ACUrl:" + this.GetACUrl() + ";Poll();";
                _PollThread.Start();
            }

            return base.ACPostInit();
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (_PollThread != null)
            {
                if (_ShutdownEvent != null && _ShutdownEvent.SafeWaitHandle != null && !_ShutdownEvent.SafeWaitHandle.IsClosed)
                    _ShutdownEvent.Set();
                if (!_PollThread.Join(5000))
                    _PollThread.Abort();
                _PollThread = null;
            }
            StopReadWeightData();
            ClosePort();
            return base.ACDeInit(deleteACClassTask);
        }

        #endregion


        #region Const
        //public const string SwitchOffTerminal = "<K002K>";

        #endregion


        #region Enums
        public enum PAScaleMode : short
        {
            Disconnect = 0,
            Init = 1,
            Idle = 2,
            ReadingWeightsRequested = 3,
            ReadingWeights = 4,
        }
        #endregion


        #region Properties

        #region TCP-Communication
        [ACPropertyInfo(true, 401, "Config", "en{'TCP-Communication on'}de{'TCP- Kommunikation ein'}", DefaultValue = false)]
        public bool TCPCommEnabled { get; set; }

        protected TcpClient _tcpClient = null;
        [ACPropertyInfo(9999)]
        public TcpClient TcpClient
        {
            get
            {
                using (ACMonitor.Lock(_61000_LockPort))
                {
                    return _tcpClient;
                }
            }
        }

        /// <summary>
        /// ServerIPV4Address
        /// </summary>
        [ACPropertyInfo(true, 402, "Config", "en{'IP-Address'}de{'IP-Adresse'}", DefaultValue = "192.168.100.1")]
        public string ServerIPV4Address { get; set; }

        [ACPropertyInfo(true, 403, "Config", "en{'DNS-Name [ms]'}de{'DNS-Name [ms]'}")]
        public string ServerDNSName { get; set; }

        /// <summary>
        /// PortNo
        /// </summary>
        [ACPropertyInfo(true, 404, "Config", "en{'Port-No.'}de{'Port-Nr.'}", DefaultValue = (int)23)]
        public int PortNo { get; set; }

        [ACPropertyInfo(false, 405, "Config", "en{'Trace read values'}de{'Ausgabe gelesene Werte'}", DefaultValue = false)]
        public bool TraceValues { get; set; }

        [ACPropertyInfo(false, 406, "Config", "en{'IgnoreInvalidTeleLength}de{'IgnoreInvalidTeleLength'}", DefaultValue = false)]
        public bool IgnoreInvalidTeleLength { get; set; }
        #endregion


        #region Serial Communication

        [ACPropertyInfo(true, 411, "Config", "en{'Serial-Communication on'}de{'Serial-Kommunikation ein'}", DefaultValue = false)]
        public bool SerialCommEnabled { get; set; }


        protected SerialPort _serialPort = null;
        [ACPropertyInfo(9999)]
        public SerialPort SerialPort
        {
            get
            {
                using (ACMonitor.Lock(_61000_LockPort))
                {
                    return _serialPort;
                }
            }
        }

        /// <summary>
        /// PortName
        /// </summary>
        [ACPropertyInfo(true, 412, DefaultValue = "COM1")]
        public string PortName { get; set; }

        /// <summary>
        /// BaudRate
        /// </summary>
        [ACPropertyInfo(true, 413, DefaultValue = 9600)]
        public int BaudRate { get; set; }

        /// <summary>
        /// Parity
        /// </summary>
        [ACPropertyInfo(true, 414)]
        public Parity Parity { get; set; }

        /// <summary>
        /// DataBits
        /// </summary>
        [ACPropertyInfo(true, 415, DefaultValue = 8)]
        public int DataBits { get; set; }

        /// <summary>
        /// StopBits
        /// </summary>
        [ACPropertyInfo(true, 416)]
        public StopBits StopBits { get; set; }


        /// <summary>
        /// RtsEnable
        /// </summary>
        [ACPropertyInfo(true, 417, DefaultValue = false)]
        public bool RtsEnable { get; set; }

        /// <summary>
        /// Handshake
        /// </summary>
        [ACPropertyInfo(true, 418)]
        public Handshake Handshake { get; set; }

        #endregion


        #region Common configuration
        [ACPropertyInfo(true, 400, "Config", "en{'Polling [ms]'}de{'Abfragezyklus [ms]'}", DefaultValue = 200)]
        public int PollingInterval { get; set; }

        /// <summary>
        /// ReadTimeout
        /// </summary>
        [ACPropertyInfo(true, 405, "Config", "en{'Read-Timeout [ms]'}de{'Lese-Timeout [ms]'}", DefaultValue = 2000)]
        public int ReadTimeout { get; set; }

        /// <summary>
        /// WriteTimeout
        /// </summary>
        [ACPropertyInfo(true, 406, "Config", "en{'Write-Timeout [ms]'}de{'Schreibe-Timeout [ms]'}", DefaultValue = 2000)]
        public int WriteTimeout { get; set; }

        #endregion


        #region Broadcast-Properties
        public IACContainerTNet<bool> _IsConnected = null;
        [ACPropertyBindingSource]
        public IACContainerTNet<bool> IsConnected
        {
            get
            {
                return _IsConnected;
            }
            set
            {
                _IsConnected = value;
            }
        }

        [ACPropertyBindingSource(407, "", "en{'Scale mode'}de{'Modus Waage'}", "", false, true)]
        public IACContainerTNet<short> ScaleMode { get; set; }

        public PAScaleMode CurrentScaleMode
        {
            get
            {
                return (PAScaleMode)ScaleMode.ValueT;
            }
            set
            {
                ScaleMode.ValueT = (short)value;
            }
        }

        [ACPropertyBindingSource(9999, "Error", "en{'Communication alarm'}de{'Communication-Alarm'}", "", false, false)]
        public IACContainerTNet<PANotifyState> CommAlarm { get; set; }
        #endregion


        #region Misc
        protected readonly ACMonitorObject _61000_LockPort = new ACMonitorObject(61000);

        protected abstract Comm3xxxBase Communicator { get; }

        private int _CountEmptyReads = 0;
        private int _CountInvalidWeights = 0;
        private DateTime _LastWrite = DateTime.Now;
        protected DateTime LastWrite
        {
            get
            {
                using (ACMonitor.Lock(_61000_LockPort))
                {
                    return _LastWrite;
                }
            }
            set
            {
                using (ACMonitor.Lock(_61000_LockPort))
                {
                    _LastWrite = value;
                }
            }
        }

        #endregion

        #endregion


        #region Methods

        #region Thread
        protected ManualResetEvent _ShutdownEvent;
        ACThread _PollThread;
        private void Poll()
        {
            try
            {
                int pollInterval = PollingInterval;
                while (!_ShutdownEvent.WaitOne(pollInterval, false))
                {
                    pollInterval = PollingInterval;
                    _PollThread.StartReportingExeTime();

                    if (CurrentScaleMode == PAScaleMode.Init)
                    {
                        OpenPort();
                        if (IsConnected.ValueT)
                            SendStartReadingWeights();
                    }

                    if (CurrentScaleMode == PAScaleMode.ReadingWeightsRequested
                        || CurrentScaleMode == PAScaleMode.ReadingWeights)
                    {
                        OpenPort();
                        if (IsConnected.ValueT)
                        {
                            PollWeightData();
                            if (_CountInvalidWeights > 0)
                                pollInterval = PollingInterval + (100 * _CountInvalidWeights); // Scale was to slow send data, enlarge polling time
                        }
                    }

                    _PollThread.StopReportingExeTime();
                }
            }
            catch (ThreadAbortException ec)
            {
                Messages.LogException("PAETerminal3xxxBase", "Poll", ec);
            }
        }

        protected void PollWeightData()
        {
            string readResult;
            bool succ = ReadWeightData(out readResult, Tele3xxxEDV.C_TelegramLength);
            if (succ)
            {
                var msg = OnParseReadWeightResult(readResult);
                _CountEmptyReads = 0;
                if (   msg == null 
                    && _CountInvalidWeights == 0 
                    && CurrentScaleMode == PAScaleMode.ReadingWeightsRequested)
                    CurrentScaleMode = PAScaleMode.ReadingWeights;
            }
            else if (  CurrentScaleMode == PAScaleMode.ReadingWeightsRequested 
                    || CurrentScaleMode == PAScaleMode.ReadingWeights)
            {
                _CountEmptyReads++;
                if (_CountEmptyReads > 5)
                {
                    if (CurrentScaleMode == PAScaleMode.ReadingWeights)
                        CurrentScaleMode = PAScaleMode.ReadingWeightsRequested;
                    _CountEmptyReads = 0;
                    SendStartReadingWeights();
                }
            }
        }

        #endregion


        #region Connection
        [ACMethodInteraction("Comm", "en{'Open Connection'}de{'Öffne Verbindung'}", 200, true)]
        public void OpenPort()
        {
            if (!IsEnabledOpenPort())
            {
                UpdateIsConnectedState();
                return;
            }
            if (CurrentScaleMode == PAScaleMode.Disconnect)
                CurrentScaleMode = PAScaleMode.Init;
            if (TCPCommEnabled)
            {
                try
                {
                    using (ACMonitor.Lock(_61000_LockPort))
                    {
                        if (_tcpClient == null)
                            _tcpClient = new TcpClient();
                        if (this.WriteTimeout > 0)
                            _tcpClient.SendTimeout = this.WriteTimeout;
                        if (this.ReadTimeout > 0)
                            _tcpClient.ReceiveTimeout = this.ReadTimeout;
                        if (!_tcpClient.Connected)
                        {
                            if (!String.IsNullOrEmpty(this.ServerIPV4Address))
                            {
                                IPAddress ipAddress = IPAddress.Parse(this.ServerIPV4Address);
                                _tcpClient.Connect(ipAddress, PortNo);
                            }
                            else
                            {
                                _tcpClient.Connect(ServerDNSName, PortNo);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    CommAlarm.ValueT = PANotifyState.AlarmOrFault;
                    if (IsAlarmActive("CommAlarm", e.Message) == null)
                        Messages.LogException(GetACUrl(), "PAETerminal3xxxBase.OpenPort(10)", e);
                    OnNewAlarmOccurred(CommAlarm, e.Message, true);
                    ClosePort();
                    CurrentScaleMode = PAScaleMode.Init;
                }
                UpdateIsConnectedState();
            }
            else if (SerialCommEnabled)
            {
                try
                {
                    using (ACMonitor.Lock(_61000_LockPort))
                    {
                        //_serialPort = new SerialPort(PortName, BaudRate, Parity, DataBits, StopBits);
                        _serialPort = new SerialPort(PortName, BaudRate);
                        if (ReadTimeout > 0)
                            _serialPort.ReadTimeout = ReadTimeout;
                        else
                            _serialPort.ReadTimeout = 5000;
                        if (WriteTimeout > 0)
                            _serialPort.WriteTimeout = WriteTimeout;
                        if (RtsEnable == true)
                            _serialPort.RtsEnable = true;
                        if (Handshake != System.IO.Ports.Handshake.None)
                            _serialPort.Handshake = Handshake;
                        if (!_serialPort.IsOpen)
                            _serialPort.Open();
                    }
                }
                catch (Exception e)
                {
                    CommAlarm.ValueT = PANotifyState.AlarmOrFault;
                    if (IsAlarmActive("CommAlarm", e.Message) == null)
                        Messages.LogException(GetACUrl(), "PAETerminal3xxxBase.OpenPort(20)", e);
                    OnNewAlarmOccurred(CommAlarm, e.Message, true);
                }
                UpdateIsConnectedState();
            }
        }

        public bool IsEnabledOpenPort()
        {
            if (   (!TCPCommEnabled && !SerialCommEnabled)
                || (ACOperationMode != ACOperationModes.Live))
                return false;
            if (TCPCommEnabled)
            {
                var client = TcpClient;
                if (client == null)
                    return !String.IsNullOrEmpty(this.ServerIPV4Address) || !String.IsNullOrEmpty(this.ServerDNSName);
                return !client.Connected;
            }
            else if (SerialCommEnabled)
            {
                var port = SerialPort;
                if (port == null)
                    return !String.IsNullOrEmpty(this.PortName);
                return !port.IsOpen;
            }
            return false;
        }

        [ACMethodInteraction("Comm", "en{'Close Connection'}de{'Schliesse Verbindung'}", 201, true)]
        public void ClosePort()
        {
            if (!IsEnabledClosePort())
                return;
            using (ACMonitor.Lock(_61000_LockPort))
            {
                if (_tcpClient != null)
                {
                    if (_tcpClient.Connected)
                        _tcpClient.Close();
                    _tcpClient.Dispose();
                    _tcpClient = null;
                }
                if (_serialPort != null)
                {
                    if (_serialPort.IsOpen)
                        _serialPort.Close();
                    _serialPort.Dispose();
                    _serialPort = null;
                }
            }
            CurrentScaleMode = PAScaleMode.Disconnect;
            IsConnected.ValueT = false;
        }

        public bool IsEnabledClosePort()
        {
            var client = TcpClient;
            if (client != null)
                return true;
            var port = SerialPort;
            if (port != null)
                return port.IsOpen;
            return false;
        }

        protected void UpdateIsConnectedState()
        {
            var client = TcpClient;
            if (client != null)
            {
                IsConnected.ValueT = client.Connected;
                return;
            }
            else
            {
                var port = SerialPort;
                if (port != null)
                {
                    IsConnected.ValueT = port.IsOpen;
                    return;
                }
            }
            IsConnected.ValueT = false;
        }
        #endregion


        #region Commands

        [ACMethodInteraction("Comm", "en{'Start reading weights'}de{'Starte Gewichtsdaten lesen'}", 202, true)]
        public void StartReadWeightData()
        {
            if (!IsEnabledStartReadWeightData())
                return;
            SendStartReadingWeights();
        }

        public bool IsEnabledStartReadWeightData()
        {
            return IsEnabledClosePort();
            //if (!IsEnabledClosePort())
            //    return false;
            //return CurrentScaleMode != PAScaleMode.ReadingWeights;
        }


        [ACMethodInteraction("Comm", "en{'Stop reading weights'}de{'Stoppe Gewichtsdaten lesen'}", 203, true)]
        public void StopReadWeightData()
        {
            if (!IsEnabledStopReadWeightData())
                return;
            SendStopReadWeightData();
        }

        public bool IsEnabledStopReadWeightData()
        {
            return IsEnabledClosePort();
            //if (!IsEnabledClosePort())
            //    return false;
            //return CurrentScaleMode == PAScaleMode.ReadingWeights;
        }


        public override Msg OnRegisterAlibiWeight()
        {
            PAScaleMode lastScaleMode = CurrentScaleMode;
            try
            {
                if (!ApplicationManager.IsSimulationOn)
                {
                    OpenPort();
                    if (!IsEnabledClosePort())
                        return new Msg(eMsgLevel.Error, "Connection to scale is disabled");

                    if (lastScaleMode == PAScaleMode.ReadingWeights
                        || lastScaleMode == PAScaleMode.ReadingWeightsRequested)
                    {
                        if (!SendStopReadWeightData())
                            return new Msg(eMsgLevel.Error, "Stop-Cmd not sended");
                    }

                    if (!SendReadAlibiCmd())
                        return new Msg(eMsgLevel.Error, "Alibi-Request not sended");

                    Thread.Sleep(1000);

                    string readResult = null;
                    if (!ReadWeightData(out readResult, Tele3xxxAlibi.C_TelegramLengthAlibi))
                        return new Msg(eMsgLevel.Error, "Alibi-Request not read");
                    if (TraceValues)
                        Messages.LogMessage(eMsgLevel.Error, this.GetACUrl(), "OnRegisterAlibiWeight(10)", "String to parse:" + readResult);

                    string alibiResult = null;
                    double alibiWeight = ActualWeight.ValueT;

                    Msg msg = OnParseAlibiResult(readResult, out alibiResult, out alibiWeight);
                    if (msg != null)
                        return msg;
                    AlibiNo.ValueT = alibiResult;
                    AlibiWeight.ValueT = alibiWeight;
                }
                else
                {
                    Random rnd = new Random();
                    AlibiNo.ValueT = "Alibi" + rnd.Next();
                    if (Math.Abs(AlibiWeight.ValueT) <= Double.Epsilon)
                        AlibiWeight.ValueT = rnd.NextDouble();
                }
                return null;
            }
            catch (Exception e)
            {
                Msg msg = new Msg("Terminal 30xx Alibi Error: " + e.Message, this, eMsgLevel.Exception, nameof(PAETerminal3xxxBase), "OnRegisterAlibiWeigh", 596);
                Messages.LogMessageMsg(msg);
                return msg;
            }
            finally
            {
                if (lastScaleMode == PAScaleMode.ReadingWeights
                    || lastScaleMode == PAScaleMode.ReadingWeightsRequested)
                {
                    StartReadWeightData();
                }
            }
        }

        #endregion


        #region Communication
        public bool SendStartReadingWeights()
        {
            if (!IsEnabledClosePort())
                return false;
            try
            {
                WaitWriteRecently();

                bool succ = false;
                using (ACMonitor.Lock(_61000_LockPort))
                {
                    var client = TcpClient;
                    if (client != null)
                        succ =  Communicator.StartReadingWeights(client);
                    else
                    {
                        var port = SerialPort;
                        succ = Communicator.StartReadingWeights(port);
                    }
                    _LastWrite = DateTime.Now;
                    CurrentScaleMode = PAScaleMode.ReadingWeightsRequested;
                }
                return succ;
            }
            catch (Exception e)
            {
                CommAlarm.ValueT = PANotifyState.AlarmOrFault;
                if (IsAlarmActive("CommAlarm", e.Message) == null)
                    Messages.LogException(GetACUrl(), "PAETerminal.StartReadWeightData()", e.Message);
                OnNewAlarmOccurred(CommAlarm, e.Message, true);
                UpdateIsConnectedState();
            }
            return false;
        }

        public bool SendStopReadWeightData()
        {
            if (!IsEnabledClosePort())
                return false;
            try
            {
                WaitWriteRecently();

                bool succ = false;
                using (ACMonitor.Lock(_61000_LockPort))
                {
                    var client = TcpClient;
                    if (client != null)
                        succ = Communicator.StopReadingWeights(client);
                    else
                    {
                        var port = SerialPort;
                        succ = Communicator.StopReadingWeights(port);
                    }

                    _LastWrite = DateTime.Now;
                    CurrentScaleMode = PAScaleMode.Idle;
                }
                return succ;
            }
            catch (Exception e)
            {
                CommAlarm.ValueT = PANotifyState.AlarmOrFault;
                if (IsAlarmActive("CommAlarm", e.Message) == null)
                    Messages.LogException(GetACUrl(), "PAETerminal3xxxBase.StopReadWeightData()", e.Message);
                OnNewAlarmOccurred(CommAlarm, e.Message, true);
                UpdateIsConnectedState();
            }
            return false;
        }

        protected bool ReadWeightData(out string readResult, int teleLength = 0)
        {
            readResult = "";
            if (!IsEnabledClosePort())
                return false;
            try
            {
                using (ACMonitor.Lock(_61000_LockPort))
                {
                    bool succ = false;
                    var client = TcpClient;
                    if (client != null)
                        succ = Communicator.ReadWeightData(client, out readResult);
                    else
                    {
                        var port = SerialPort;
                        succ = Communicator.ReadWeightData(port, teleLength, out readResult);
                    }
                    return succ;
                }
            }
            catch (Exception e)
            {
                CommAlarm.ValueT = PANotifyState.AlarmOrFault;
                if (IsAlarmActive("CommAlarm", e.Message) == null)
                    Messages.LogException(GetACUrl(), "PAETerminal3xxxBase.ReadWeightData()", e);
                OnNewAlarmOccurred(CommAlarm, e.Message, true);
                UpdateIsConnectedState();
            }
            return false;
        }

        protected virtual bool SendReadAlibiCmd()
        {
            if (!IsEnabledClosePort())
                return false;
            try
            {
                WaitWriteRecently();

                bool succ = false;
                using (ACMonitor.Lock(_61000_LockPort))
                {
                    var client = TcpClient;
                    if (client != null)
                        succ = Communicator.SendReadAlibiCmd(client);
                    else
                    {
                        var port = SerialPort;
                        succ = Communicator.SendReadAlibiCmd(port);
                    }
                    _LastWrite = DateTime.Now;
                }
                return succ;
            }
            catch (Exception e)
            {
                CommAlarm.ValueT = PANotifyState.AlarmOrFault;
                if (IsAlarmActive("CommAlarm", e.Message) == null)
                    Messages.LogException(GetACUrl(), "PAETerminal.SendReadAlibiCmd()", e.Message);
                OnNewAlarmOccurred(CommAlarm, e.Message, true);
                UpdateIsConnectedState();
            }
            return false;
        }

        /// <summary>
        /// Wait if the write has taken place recently
        /// </summary>
        private void WaitWriteRecently()
        {
            DateTime lastWrite = LastWrite;
            if ((DateTime.Now - lastWrite).TotalSeconds < 1)
                Thread.Sleep(1000);
        }
        #endregion


        #region Parse Result

        protected virtual Msg OnParseReadWeightResult(string readResult)
        {
            if (string.IsNullOrEmpty(readResult))
                return new Msg(eMsgLevel.Error, "The reply from scale is null or empty!");

            try
            {
                bool isSerialComm = SerialPort != null || SerialCommEnabled;
                Tele3xxxEDV tele3XxxEDV = new Tele3xxxEDV(readResult);

                if (!tele3XxxEDV.InvalidTelegram)
                {
                    if (tele3XxxEDV.IsOverLoad)
                        StateUL2.ValueT = PANotifyState.AlarmOrFault;
                    else
                        StateUL2.ValueT = PANotifyState.Off;

                    if (tele3XxxEDV.IsUnderLoad)
                        StateLL2.ValueT = PANotifyState.AlarmOrFault;
                    else
                        StateLL2.ValueT = PANotifyState.Off;
                    NotStandStill.ValueT = !tele3XxxEDV.IsStandStill;
                    IsDosing.ValueT = !tele3XxxEDV.IsStandStill;
                    StateScale.ValueT = PANotifyState.Off;
                }

                if (tele3XxxEDV.InvalidWeight)
                {
                    _CountInvalidWeights++;
                    if (  !isSerialComm
                        || _CountInvalidWeights > 5)
                    {
                        _CountInvalidWeights = 0;
                        if (tele3XxxEDV.InvalidTelegram)
                        {
                            Msg msg = new Msg(this, eMsgLevel.Error, nameof(PAETerminal3xxxBase), "OnParseReadWeightResult(10)", 504, "Error50306");
                            if (IsAlarmActive(StateScale, msg.Message) == null || IgnoreInvalidTeleLength)
                            {
                                Messages.LogMessageMsg(msg);
                                Messages.LogMessage(eMsgLevel.Error, this.GetACUrl(), "OnParseReadWeightResult(10a)", "String to parse:" + readResult);
                            }
                            if (!IgnoreInvalidTeleLength)
                            {
                                StateScale.ValueT = PANotifyState.AlarmOrFault;
                                OnNewAlarmOccurred(StateScale, msg, true);
                            }
                            return msg;
                        }
                    }
                }
                else
                {
                    _CountInvalidWeights = 0;
                    ActualValue.ValueT = tele3XxxEDV.WeightKg;
                }
            }
            catch (Exception ex)
            {
                return new Msg(eMsgLevel.Exception, ex.Message);
            }
            return null;
        }

        protected virtual Msg OnParseAlibiResult(string readResult, out string alibiNo, out double alibiWeight)
        {
            alibiNo = null;
            alibiWeight = 0;

            if (string.IsNullOrEmpty(readResult))
                return new Msg(eMsgLevel.Error, "The reply from scale is null or empty!");

            try
            {
                Tele3xxxAlibi tele3XxxEDV = new Tele3xxxAlibi(readResult);
                if (!tele3XxxEDV.InvalidTelegram)
                {
                    if (tele3XxxEDV.IsOverLoad)
                        StateUL2.ValueT = PANotifyState.AlarmOrFault;
                    else
                        StateUL2.ValueT = PANotifyState.Off;

                    if (tele3XxxEDV.IsUnderLoad)
                        StateLL2.ValueT = PANotifyState.AlarmOrFault;
                    else
                        StateLL2.ValueT = PANotifyState.Off;
                    NotStandStill.ValueT = !tele3XxxEDV.IsStandStill;
                    IsDosing.ValueT = !tele3XxxEDV.IsStandStill;
                    StateScale.ValueT = PANotifyState.Off;
                }

                if (tele3XxxEDV.InvalidWeight)
                {
                    StateScale.ValueT = PANotifyState.AlarmOrFault;
                    Msg msg = new Msg(this, eMsgLevel.Error, nameof(PAETerminal3xxxBase), "OnParseAlibiResult(10)", 504, "Error50306");
                    if (IsAlarmActive(StateScale, msg.Message) == null)
                    {
                        Messages.LogMessageMsg(msg);
                        Messages.LogMessage(eMsgLevel.Error, this.GetACUrl(), "OnParseAlibiResult(10a)", "String to parse:" + readResult);
                    }
                    OnNewAlarmOccurred(StateScale, msg, true);
                    return msg;
                }
                else
                {
                    alibiNo = tele3XxxEDV.AlibiNumber;
                    alibiWeight = tele3XxxEDV.WeightKg;
                    ActualValue.ValueT = tele3XxxEDV.WeightKg;
                }
            }
            catch (Exception ex)
            {
                return new Msg(eMsgLevel.Exception, ex.Message);
            }

            return null;
        }

        #endregion


        #region Alarms
        public override void AcknowledgeAlarms()
        {
            if (!IsEnabledAcknowledgeAlarms())
                return;
            if (CommAlarm.ValueT == PANotifyState.AlarmOrFault)
            {
                CommAlarm.ValueT = PANotifyState.Off;
                OnAlarmDisappeared(CommAlarm);
            }
            base.AcknowledgeAlarms();
        }
        #endregion


        #region Helper-Methods
        public static bool HandleExecuteACMethod_PAETerminal3xxxBase(out object result, IACComponent acComponent, string acMethodName, ACClassMethod acClassMethod, object[] acParameter)
        {
            return HandleExecuteACMethod_PAEScaleCalibratable(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(OpenPort):
                    OpenPort();
                    return true;
                case nameof(IsEnabledOpenPort):
                    result = IsEnabledOpenPort();
                    return true;
                case nameof(ClosePort):
                    ClosePort();
                    return true;
                case nameof(IsEnabledClosePort):
                    result = IsEnabledClosePort();
                    return true;
                case nameof(StartReadWeightData):
                    StartReadWeightData();
                    return true;
                case nameof(IsEnabledStartReadWeightData):
                    result = IsEnabledStartReadWeightData();
                    return true;
                case nameof(StopReadWeightData):
                    StopReadWeightData();
                    return true;
                case nameof(IsEnabledStopReadWeightData):
                    result = IsEnabledStopReadWeightData();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }
        #endregion

        #endregion
    }
}
