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

namespace sick.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Sick RFID 6xxx}de{'Sick RFID 6xxx'}", Global.ACKinds.TPAModule, Global.ACStorableTypes.Required, false, true)]
    public abstract class PAERFU6xxxBase : PAEControlModuleBase
    {
        #region c'tors
        static PAERFU6xxxBase()
        {
            RegisterExecuteHandler(typeof(PAERFU6xxxBase), HandleExecuteACMethod_PAERFU6xxxBase);
        }


        public PAERFU6xxxBase(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") :
               base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACPostInit()
        {
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
            StopReadRFIDData();
            ClosePort();
            return base.ACDeInit(deleteACClassTask);
        }

        public new const string ClassName = "PAERFU6xxxBase";
        #endregion


        #region Const
        public const string C_MethodCmd = "sMN";
        public const string C_MethodRes = "sAN";
        
        public const string C_ReadingGateOn = "mTCgateon";
        public const string C_ReadingGateOnCmd = C_MethodCmd + " " + C_ReadingGateOn;
        
        public const string C_ReadingGateOff = "mTCgateoff";
        public const string C_ReadingGateOffCmd = C_MethodCmd + " " + C_ReadingGateOff;

        public const string C_ReadTagData = "TAreadTagData";
        public const string C_ReadTagDataCmd = C_MethodCmd + " " + C_ReadTagData + " 0 1 2 6 32"; // Parameters 

        public const char C_STX = '\x02';
        public const char C_ETX = '\x03';

        public const char C_DefaultRfidSeparator = '#';
        protected virtual char RfidSeparator
        {
            get
            {
                return C_DefaultRfidSeparator;
            }
        }
        #endregion


        #region Enums
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
        [ACPropertyInfo(true, 404, "Config", "en{'Port-No.'}de{'Port-Nr.'}", DefaultValue = (int)2112)]
        public int PortNo { get; set; }

        [ACPropertyInfo(false, 405, "Config", "en{'Trace read values'}de{'Ausgabe gelesene Werte'}", DefaultValue = false)]
        public bool TraceValues { get; set; }

        [ACPropertyInfo(false, 406, "Config", "en{'IgnoreInvalidTeleLength}de{'IgnoreInvalidTeleLength'}", DefaultValue = false)]
        public bool IgnoreInvalidTeleLength { get; set; }
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

        public enum RfidModeEnum : short
        {
            Disconnect = -1,
            Init = 0,
            Idle = 1,
            ReadingRfid = 2
        }


        [ACPropertyBindingSource(407, "", "en{'RFID mode'}de{'Modus RFID'}", "", false, true)]
        public IACContainerTNet<short> RfidMode { get; set; }

        public RfidModeEnum CurrentRfidMode
        {
            get
            {
                return (RfidModeEnum)RfidMode.ValueT;
            }
            set
            {
                RfidMode.ValueT = (short)value;
            }
        }

        [ACPropertyBindingSource(9999, "Error", "en{'Communication alarm'}de{'Communication-Alarm'}", "", false, false)]
        public IACContainerTNet<PANotifyState> CommAlarm { get; set; }

        [ACPropertyBindingSource(407, "", "en{'Command'}de{'Command'}", "", false, false)]
        public IACContainerTNet<string> Cmd { get; set; }

        [ACPropertyBindingSource(408, "", "en{'Result'}de{'Ausgabe'}", "", false, false)]
        public IACContainerTNet<string> ReadResult { get; set; }

        [ACPropertyBindingSource(409, "", "en{'Result History'}de{'Ausgabe Historie'}", "", false, false)]
        public IACContainerTNet<string> ReadLog { get; set; }

        protected CircularBuffer<string> _ReadLog = new CircularBuffer<string>(50);

        [ACPropertyBindingSource(410, "", "en{'Read Rfids'}de{'Gelesene Rfids'}", "", false, false)]
        public IACContainerTNet<string[]> ReadRfids { get; set; }

        #endregion


        #region Misc
        protected readonly ACMonitorObject _61000_LockPort = new ACMonitorObject(61000);

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

                    if (CurrentRfidMode == RfidModeEnum.Init)
                        OpenPort();

                    if (CurrentRfidMode >= RfidModeEnum.Idle)
                    {
                        OpenPort();
                        if (IsConnected.ValueT)
                            PollRfidData();
                    }

                    _PollThread.StopReportingExeTime();
                }
            }
            catch (ThreadAbortException ec)
            {
                Messages.LogException("PAERFU6xxxBase", "Poll", ec);
            }
        }

        protected void PollRfidData()
        {
            string readResult;
            bool succ = ReadRfidData(out readResult);
            if (succ)
            {
                if (!String.IsNullOrEmpty(readResult))
                {
                    string[] rawRfids = new string[] { };
                    int posSTX = readResult.LastIndexOf(C_STX);
                    int posETX = readResult.LastIndexOf(C_ETX);
                    if (posSTX >= 0 && (posETX - 1) > posSTX)
                    {
                        string data = readResult.Substring(posSTX + 1, posETX - posSTX - 1);
                        rawRfids = data.Split(RfidSeparator);
                    }
                    ReadRfids.ValueT = OnDecodeRfids(rawRfids);
                }
            }
        }

        protected virtual string[] OnDecodeRfids(string[] rawRfids)
        {
            return rawRfids;
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
            if (CurrentRfidMode == RfidModeEnum.Disconnect)
                CurrentRfidMode = RfidModeEnum.Init;
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
                        if (_tcpClient.Connected)
                            CurrentRfidMode = RfidModeEnum.Idle;
                    }
                }
                catch (Exception e)
                {
                    CommAlarm.ValueT = PANotifyState.AlarmOrFault;
                    if (IsAlarmActive("CommAlarm", e.Message) == null)
                        Messages.LogException(GetACUrl(), "PAERFU6xxxBase.OpenPort(10)", e);
                    OnNewAlarmOccurred(CommAlarm, e.Message, true);
                    ClosePort();
                    CurrentRfidMode = RfidModeEnum.Init;
                }
                UpdateIsConnectedState();
            }
        }

        public bool IsEnabledOpenPort()
        {
            if (   !TCPCommEnabled
                || (ACOperationMode != ACOperationModes.Live))
                return false;
            if (TCPCommEnabled)
            {
                var client = TcpClient;
                if (client == null)
                    return !String.IsNullOrEmpty(this.ServerIPV4Address) || !String.IsNullOrEmpty(this.ServerDNSName);
                return !client.Connected;
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
            }
            CurrentRfidMode = RfidModeEnum.Disconnect;
            IsConnected.ValueT = false;
        }

        public bool IsEnabledClosePort()
        {
            var client = TcpClient;
            if (client != null)
                return true;
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
            IsConnected.ValueT = false;
        }
        #endregion


        #region Commands

        [ACMethodInteraction("Comm", "en{'Start reading RFID'}de{'Starte RFID lesen'}", 202, true)]
        public void StartReadRfidData()
        {
            if (!IsEnabledStartReadWeightData())
                return;
            if (SendCommand(C_ReadingGateOn))
                CurrentRfidMode = RfidModeEnum.ReadingRfid;
        }

        public bool IsEnabledStartReadWeightData()
        {
            return IsEnabledClosePort();
        }


        [ACMethodInteraction("Comm", "en{'Stop reading RFID'}de{'Stoppe RFID lesen'}", 203, true)]
        public void StopReadRFIDData()
        {
            if (!IsEnabledStopReadRFIDData())
                return;
            if (SendCommand(C_ReadingGateOffCmd))
                CurrentRfidMode = RfidModeEnum.Idle;
        }

        public bool IsEnabledStopReadRFIDData()
        {
            return IsEnabledClosePort();
            //if (!IsEnabledClosePort())
            //    return false;
            //return CurrentScaleMode == PAScaleMode.ReadingWeights;
        }

        [ACMethodInteraction("Comm", "en{'Send'}de{'Senden'}", 201, true)]
        public void Send()
        {
            if (!IsEnabledSend())
                return;
            SendCommand(Cmd.ValueT);
        }

        public bool IsEnabledSend()
        {
            return IsConnected.ValueT;
        }

        #endregion


        #region Communication
        [ACMethodInfo("", "en{'Send Command'}de{'Send Command'}", 100)]
        public bool SendCommand(string cmd)
        {
            try
            {
                if (string.IsNullOrEmpty(cmd))
                    return false;
                string telegram = String.Format("\x02{0}\x03", cmd);
                byte[] ascArr = System.Text.Encoding.ASCII.GetBytes(telegram);

                using (ACMonitor.Lock(_61000_LockPort))
                {
                    var client = TcpClient;
                    if (client != null)
                    {
                        if (!client.Connected)
                            return false;
                        NetworkStream stream = client.GetStream();
                        if (stream == null || !stream.CanWrite)
                            return false;
                        stream.Write(ascArr, 0, ascArr.Length);
                        return true;
                    }
                }
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

        protected bool ReadRfidData(out string readResult)
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
                    {
                        if (!client.Connected)
                            return false;
                        NetworkStream stream = client.GetStream();
                        if (stream == null)
                            return false;
                        if (stream.CanRead && stream.DataAvailable)
                        {
                            byte[] myReadBuffer = new byte[1024];
                            StringBuilder myCompleteMessage = new StringBuilder();
                            int numberOfBytesRead = 0;
                            do
                            {
                                numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length);
                                myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));
                            }
                            while (stream.DataAvailable);

                            readResult = myCompleteMessage.ToString();
                            ReadResult.ValueT = readResult;
                            
                            _ReadLog.Put(readResult);
                            StringBuilder readLog = new StringBuilder();
                            foreach (string log in _ReadLog)
                                readLog.AppendLine(log);
                            ReadLog.ValueT = readLog.ToString();
                            return true;
                        }
                    }
                    return succ;
                }
            }
            catch (Exception e)
            {
                CommAlarm.ValueT = PANotifyState.AlarmOrFault;
                if (IsAlarmActive("CommAlarm", e.Message) == null)
                    Messages.LogException(GetACUrl(), "PAERFU6xxxBase.ReadWeightData()", e);
                OnNewAlarmOccurred(CommAlarm, e.Message, true);
                UpdateIsConnectedState();
            }
            return false;
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
        public static bool HandleExecuteACMethod_PAERFU6xxxBase(out object result, IACComponent acComponent, string acMethodName, ACClassMethod acClassMethod, object[] acParameter)
        {
            return HandleExecuteACMethod_PAEControlModuleBase(out result, acComponent, acMethodName, acClassMethod, acParameter);
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
                case nameof(StartReadRfidData):
                    StartReadRfidData();
                    return true;
                case nameof(IsEnabledStartReadWeightData):
                    result = IsEnabledStartReadWeightData();
                    return true;
                case nameof(StopReadRFIDData):
                    StopReadRFIDData();
                    return true;
                case nameof(IsEnabledStopReadRFIDData):
                    result = IsEnabledStopReadRFIDData();
                    return true;
                case nameof(Send):
                    Send();
                    return true;
                case nameof(IsEnabledSend):
                    result = IsEnabledSend();
                    return true;
                case nameof(SendCommand):
                    SendCommand((string) acParameter[0]);
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }
        #endregion

        #endregion
    }
}
