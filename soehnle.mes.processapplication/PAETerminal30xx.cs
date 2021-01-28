using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.processapplication;
using gip.mes.processapplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace soehnle.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Scale Terminal 30xx'}de{'Waage Terminal 30xx'}", Global.ACKinds.TPAModule, Global.ACStorableTypes.Required, false, true)]
    public class PAETerminal30xx : PAEScaleCalibratable
    {
        #region c'tors

        public PAETerminal30xx(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : 
               base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACPostInit()
        {
            OpenPort();
            //if (CurrentScaleMode == PAScaleMode.ReadingWeights)
            //{
            CurrentScaleMode = PAScaleMode.Off;
            StartReadWeightData();
            //}

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
            ClosePort();
            return base.ACDeInit(deleteACClassTask);
        }

        public new const string ClassName = "PAETerminal30xx";
        #endregion

        #region Const

        public const string SendValX1ACK = "<a>";
        public const string SendValX1 = "<A>";

        public const string SendValContinuouslyACK = "<f>";
        public const string SendValContinuously = "<F>";

        public const string SwitchOffTerminal = "<K002K>";

        public const string AlibiCommand = "<h>";

        public const string EmptyWeightUnderload = "______";

        public const int ScaleMessageLenght = 19;

        #endregion

        #region Enums
        public enum PAScaleMode : short
        {
            Off = 0,
            Idle = 5,
            ReadingWeights = 10,
            AlibiWeighing = 20,
        }
        #endregion

        #region Properties

        [ACPropertyInfo(true, 400, "Config", "en{'Polling [ms]'}de{'Abfragezyklus [ms]'}", DefaultValue = 500)]
        public int PollingInterval { get; set; }

        [ACPropertyInfo(true, 401, "Config", "en{'Communication on'}de{'Kommunikation ein'}", DefaultValue = false)]
        public bool TCPCommEnabled { get; set; }

        protected TcpClient _tcpClient = null;
        [ACPropertyInfo(9999)]
        public TcpClient TcpClient
        {
            get { return _tcpClient; }
        }

        protected readonly ACMonitorObject _61000_LockPort = new ACMonitorObject(61000);

        /// <summary>
        /// TODO: Configuration-Property
        /// </summary>
        [ACPropertyInfo(true, 402, "Config", "en{'IP-Address'}de{'IP-Adresse'}", DefaultValue = "192.168.100.1")]
        public string ServerIPV4Address { get; set; }

        [ACPropertyInfo(true, 403, "Config", "en{'DNS-Name [ms]'}de{'DNS-Name [ms]'}")]
        public string ServerDNSName { get; set; }

        /// <summary>
        /// TODO: Configuration-Property
        /// </summary>
        [ACPropertyInfo(true, 404, "Config", "en{'Port-No.'}de{'Port-Nr.'}", DefaultValue = (int)23)]
        public int PortNo { get; set; }


        /// <summary>
        /// TODO: Configuration-Property
        /// </summary>
        [ACPropertyInfo(true, 405)]
        public int ReceiveTimeout { get; set; }

        /// <summary>
        /// TODO: Configuration-Property
        /// </summary>
        [ACPropertyInfo(true, 406)]
        public int SendTimeout { get; set; }

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

        private int _CountWrites = 0;
        private int _CountReads = 0;
        private DateTime _LastWrite = DateTime.Now;

        [ACPropertyBindingSource(9999, "", "en{'Last weight'}de{'Letztes Gewicht'}")]
        public IACContainerTNet<string> LastWeight { get; set; }

        [ACPropertyBindingSource(9999, "Error", "en{'Communication alarm'}de{'Communication-Alarm'}", "", false, false)]
        public IACContainerTNet<PANotifyState> CommAlarm { get; set; }


        #endregion

        #region Methods

        #region Thread
        protected ManualResetEvent _ShutdownEvent;
        ACThread _PollThread;
        private void Poll()
        {
            try
            {
                while (!_ShutdownEvent.WaitOne(PollingInterval, false))
                {
                    _PollThread.StartReportingExeTime();
                    if (CurrentScaleMode == PAScaleMode.ReadingWeights)
                    {
                        using (ACMonitor.Lock(_61000_LockPort))
                        {
                            if (this.TcpClient == null || !this.TcpClient.Connected)
                                OpenPort();
                            if (this.TcpClient != null && IsConnected.ValueT)
                                ReadWeightData();
                        }
                    }
                    TestAndReconnect();
                    _PollThread.StopReportingExeTime();
                }
            }
            catch (ThreadAbortException ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += ec.InnerException.Message;

                Root.Messages.LogException("PAETerminal30xx", "Poll", msg);
            }
        }
        #endregion

        #region Connection
        [ACMethodInteraction("Comm", "en{'Open Connection'}de{'Öffne Verbindung'}", 200, true)]
        public void OpenPort()
        {
            if (!IsEnabledOpenPort())
            {
                IsConnected.ValueT = this.TcpClient != null && this.TcpClient.Connected;
                return;
            }
            //if (_tcpClient == null)
            _tcpClient = new System.Net.Sockets.TcpClient();
            using (ACMonitor.Lock(_61000_LockPort))
            {
                if (this.SendTimeout > 0)
                    _tcpClient.SendTimeout = this.SendTimeout;
                if (this.ReceiveTimeout > 0)
                    _tcpClient.ReceiveTimeout = this.ReceiveTimeout;

                try
                {
                    if (!this.TcpClient.Connected)
                    {
                        if (!String.IsNullOrEmpty(this.ServerIPV4Address))
                        {
                            IPAddress ipAddress = IPAddress.Parse(this.ServerIPV4Address);
                            this.TcpClient.Connect(ipAddress, PortNo);
                        }
                    }
                }
                catch (Exception e)
                {
                    CommAlarm.ValueT = PANotifyState.AlarmOrFault;
                    if (IsAlarmActive("CommAlarm", e.Message) == null)
                        this.Root.Messages.LogException(GetACUrl(), "PAETerminal30xx.OpenPort()", e.Message);
                    OnNewAlarmOccurred(CommAlarm, e.Message, true);
                }
                IsConnected.ValueT = this.TcpClient.Connected;
            }
        }

        public bool IsEnabledOpenPort()
        {
            if (!TCPCommEnabled || (ACOperationMode != ACOperationModes.Live))
                return false;
            if (this.TcpClient == null)
            {
                if (String.IsNullOrEmpty(this.ServerIPV4Address) && String.IsNullOrEmpty(this.ServerDNSName))
                    return false;
                return true;
            }
            return !this.TcpClient.Connected;
        }

        [ACMethodInteraction("Comm", "en{'Close Connection'}de{'Schliesse Verbindung'}", 201, true)]
        public void ClosePort()
        {
            if (!IsEnabledClosePort())
                return;
            if (this.TcpClient != null)
            {
                using (ACMonitor.Lock(_61000_LockPort))
                {
                    if (this.TcpClient.Connected)
                        this.TcpClient.Close();
                    _tcpClient = null;
                }
            }
            IsConnected.ValueT = false;
        }

        public bool IsEnabledClosePort()
        {
            if (this.TcpClient == null)
                return false;
            return this.TcpClient.Connected;
        }

        int _WaitForReconnect = 0;
        private int WaitForReconnect
        {
            get
            {
                if(_WaitForReconnect == 0)
                {
                    if (PollingInterval > 10000)
                        _WaitForReconnect = 1;
                    else
                        _WaitForReconnect = 10000 / PollingInterval;

                }
                return _WaitForReconnect;
            }
        }

        DateTime _LastReconnectTest = DateTime.Now;

        protected void TestAndReconnect()
        {
            if ((DateTime.Now - _LastReconnectTest) < TimeSpan.FromSeconds(30))
                return;

            _LastReconnectTest = DateTime.Now;

            if (TestConnection())
            {
                AcknowledgeAlarms();
                return;
            }

            IsConnected.ValueT = false;

            Msg msg = new Msg(eMsgLevel.Error, "Connection lost to the Terminal 3010!");
            CommAlarm.ValueT = PANotifyState.AlarmOrFault;
            OnNewAlarmOccurred(CommAlarm, msg, true);
            if (IsAlarmActive(CommAlarm, msg.Message) == null)
                Messages.LogMessageMsg(msg);
        }
        #endregion

        #region Polling WeightData
        [ACMethodInteraction("Comm", "en{'Start reading weights'}de{'Starte Gewichtsdaten lesen'}", 202, true)]
        public void StartReadWeightData()
        {
            if (!IsEnabledStartReadWeightData())
                return;
            try
            {
                using (ACMonitor.Lock(_61000_LockPort))
                {
                    NetworkStream stream = this.TcpClient.GetStream();
                    if (stream == null || !stream.CanWrite)
                        return;
                    string message = "";
                    //<F> Send value continuously
                    message = "<F>";
                    Byte[] data = Encoding.ASCII.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                    _LastWrite = DateTime.Now;

                    CurrentScaleMode = PAScaleMode.ReadingWeights;
                    _CountWrites++;
                }
            }
            catch (Exception e)
            {
                CommAlarm.ValueT = PANotifyState.AlarmOrFault;
                if (IsAlarmActive("CommAlarm", e.Message) == null)
                    this.Root.Messages.LogException(GetACUrl(), "PAETerminal.StartReadWeightData()", e.Message);
                OnNewAlarmOccurred(CommAlarm, e.Message, true);

                IsConnected.ValueT = this.TcpClient.Connected;
            }
        }

        public bool IsEnabledStartReadWeightData()
        {
            if (!IsEnabledClosePort())
                return false;
            return CurrentScaleMode != PAScaleMode.ReadingWeights;
        }

        [ACMethodInteraction("Comm", "en{'Stop reading weights'}de{'Stoppe Gewichtsdaten lesen'}", 203, true)]
        public void StopReadWeightData()
        {
            if (!IsEnabledStopReadWeightData())
                return;
            try
            {
                using (ACMonitor.Lock(_61000_LockPort))
                {
                    NetworkStream stream = this.TcpClient.GetStream();
                    if (stream == null || !stream.CanWrite)
                        return;
                    if ((DateTime.Now - _LastWrite).TotalSeconds < 1)
                        Thread.Sleep(1000);

                    // <A> Send value x1 immediately
                    string message = "<A>";
                    Byte[] data = Encoding.ASCII.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                    _LastWrite = DateTime.Now;

                    CurrentScaleMode = PAScaleMode.Idle;
                    _CountWrites++;
                }
            }
            catch (Exception e)
            {
                CommAlarm.ValueT = PANotifyState.AlarmOrFault;
                if (IsAlarmActive("CommAlarm", e.Message) == null)
                    this.Root.Messages.LogException(GetACUrl(), "PAETerminal30xx.StopReadWeightData()", e.Message);
                OnNewAlarmOccurred(CommAlarm, e.Message, true);
                IsConnected.ValueT = this.TcpClient.Connected;
            }
        }

        public bool IsEnabledStopReadWeightData()
        {
            if (!IsEnabledClosePort())
                return false;
            return CurrentScaleMode == PAScaleMode.ReadingWeights;
        }


        //EDP Standard
        //
        // | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8| 9 | 10 | 11 | 12 | 13 | 14 | 15 | 16 | 17 | 18 | 19 | 20 | 21 | 22 | 23 | 24 | 25 | 26 | 27 | 28 | 29 | 30 | 31 | .... | 42 |

        // | 0 | 0 | 0 | 1 | 0 | 1 | N |  |   |    |    | -  | 1  | 0  | 0  | .  | 0  |    | kg |    |

        //Status  | Scale | Net value with known value, prefix and dimension |
        //--------+-------+---------------------------------------------------
        //Status  | Scale |  K  |  Space  |  V  |  Weight value  | Dimension |

        // Status: 0 - inactive, 1 - active
        // 1. place: Underload
        // 2. place: Overload
        // 3. place: Scale at standstill
        // 4. place: Empty message

        //K = known value
        //V = prefix, always in front of weight value

        protected void ReadWeightData()
        {
            if (!IsEnabledStopReadWeightData())
                return;
            try
            {
                using (ACMonitor.Lock(_61000_LockPort))
                {
                    NetworkStream stream = this.TcpClient.GetStream();
                    if (stream == null)
                        return;
                    _CountReads++;
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

                        String readResult = myCompleteMessage.ToString();
                        if (!String.IsNullOrEmpty(readResult))
                        {
                            OnReadWeightDataDone(readResult);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                CommAlarm.ValueT = PANotifyState.AlarmOrFault;
                if (IsAlarmActive("CommAlarm", e.Message) == null)
                    this.Root.Messages.LogException(GetACUrl(), "PAETerminal30xx.ReadWeightData()", e.Message);
                OnNewAlarmOccurred(CommAlarm, e.Message, true);
                if (this.TcpClient == null || !this.TcpClient.Connected)
                    OpenPort();
                IsConnected.ValueT = this.TcpClient.Connected;
            }
        }

        protected virtual void OnReadWeightDataDone(String readResult)
        {
            if (readResult.Count() < ScaleMessageLenght || string.IsNullOrEmpty(readResult) || readResult.StartsWith(" "))
                return;

            if (!IsStatusOk(readResult))
                return;

            int prefixPos = 11;
            int weightPos = prefixPos + 1;

            string prefix = readResult.Substring(prefixPos, 1);
            if(prefix == " ")
            {
                string weight = readResult.Substring(weightPos, 6); //TODO: test lenght
                double resultWeight = 0;

                if(!double.TryParse(weight, out resultWeight) && weight != EmptyWeightUnderload)
                {
                    Msg msg = new Msg(this, eMsgLevel.Error, ClassName, "OnReadWeightDataDone(10)", 504, "Error50306");
                    OnNewAlarmOccurred(StateScale, msg, true);
                    if (IsAlarmActive(StateScale, msg.Message) == null)
                        Messages.LogMessageMsg(msg);
                    Messages.LogMessage(eMsgLevel.Error, this.GetACUrl(), "OnReadWeightDataDone(10a)", "String to parse:" + weight);
                }
                ActualValue.ValueT = resultWeight;
            }
        }

        private bool IsStatusOk(string readResult)
        {
            //Underload
            //string statusString = readResult.Substring(0, 1);
            //int status = 0;
            //if (!int.TryParse(statusString, out status))
            //{
            //    Msg msg = new Msg(this, eMsgLevel.Error, ClassName, "IsStatusOk", 539, "Error50307");
            //    OnNewAlarmOccurred(StateScale, msg, true);
            //    if (IsAlarmActive(StateScale, msg.Message) == null)
            //        Messages.LogMessageMsg(msg);
            //    return false;
            //}

            //if (status > 0)
            //{
            //    Msg msg = new Msg(this, eMsgLevel.Error, ClassName, "IsStatusOk", 548, "Error50309");
            //    OnNewAlarmOccurred(StateScale, msg, true);
            //    if (IsAlarmActive(StateScale, msg.Message) == null)
            //        Messages.LogMessageMsg(msg);
            //    return false;
            //}

            //Overload
            //statusString = readResult.Substring(1, 1);
            //if (!int.TryParse(statusString, out status))
            //{
            //    Msg msg = new Msg(this, eMsgLevel.Error, ClassName, "IsStatusOk", 559, "Error50307");
            //    OnNewAlarmOccurred(StateScale, msg, true);
            //    if (IsAlarmActive(StateScale, msg.Message) == null)
            //        Messages.LogMessageMsg(msg);
            //    return false;
            //}

            //if (status > 0)
            //{
            //    Msg msg = new Msg(this, eMsgLevel.Error, ClassName, "IsStatusOk", 548, "Error50308");
            //    OnNewAlarmOccurred(StateScale, msg, true);
            //    if (IsAlarmActive(StateScale, msg.Message) == null)
            //        Messages.LogMessageMsg(msg);
            //    return false;
            //}

            return true;
        }

        #endregion

        public override Msg OnRegisterAlibiWeight(IACObject parentPos)
        {
            try
            {
                if (!ApplicationManager.IsSimulationOn)
                {
                    string reply = SendCommand(AlibiCommand);
                    string alibiResult = null;
                    double alibiWeight = ActualWeight.ValueT;

                    StartReadWeightData();

                    Msg msgParse = OnParseAlibiResult(reply, out alibiResult, out alibiWeight);
                    if(string.IsNullOrEmpty(alibiResult))
                    {
                        Msg msg = new Msg(eMsgLevel.Error, "The alibi weighing is not registred or the scale response is wrong!");
                        OnNewAlarmOccurred(StateScale, msg, true);
                        if (IsAlarmActive(StateScale, msg.Message) == null)
                            Messages.LogMessageMsg(msg);
                        Messages.LogMessage(eMsgLevel.Error, this.GetACUrl(), "OnRegisterAlibiWeight(10a)", "String to parse:" + reply);
                        return msg;
                    }
                    AlibiNo.ValueT = alibiResult;
                    AlibiWeight.ValueT = alibiWeight;
                }
                else
                {
                    Random rnd = new Random();
                    AlibiNo.ValueT = "Alibi" + rnd.Next();
                    AlibiWeight.ValueT = rnd.NextDouble();
                }
                return null;
            }
            catch(Exception e)
            {
                Msg msg = new Msg("Terminal 30xx Alibi Error: " + e.Message, this, eMsgLevel.Exception, ClassName, "OnRegisterAlibiWeigh", 596);
                Messages.LogMessageMsg(msg);
                return msg;
            }
        }

        //EDP Standard
        //
        // | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | 10 | 11 | 12 | 13 | 14 | 15 | 16 | 17 | 18 | 19 | 20 | 21 | 22 | 23 | 24 | 25 | 26 | 27 | 28 | 29 | 30 | 31 | .... | 42 |

        // | A | 0 | 0 | 0 | 0 | 0 | 0 | 1 | 0 |  0 |  0 |  1 |  0 |  1 |  N |    |    |    |    |  - |  1 |  0 |  0 |  0 |  . |  0 |    |  k |  g |    |
        //                 Alibi           |      Status      |  Scale  |  Net value with marking, presign and dimension

        public Msg OnParseAlibiResult(string replyFromScale, out string alibiNo, out double alibiWeight)
        {
            alibiNo = null;
            alibiWeight = 0;

            if (string.IsNullOrEmpty(replyFromScale))
                return new Msg(eMsgLevel.Error, "The reply from scale is null or empty!");

            int startIndex = replyFromScale.IndexOf("A");
            if (startIndex < 0)
                return new Msg(eMsgLevel.Error, "The reply from scale is not correct! Can't find the AlibiNo start!");

            replyFromScale = replyFromScale.Substring(startIndex);

            alibiNo = replyFromScale.Substring(0, 8);

            string weight = replyFromScale.Substring(20, 6);
            if(!double.TryParse(weight, out alibiWeight))
                return new Msg(eMsgLevel.Error, "Can not parse the Alibi weight from scale reply");

            return null;
        }

        public string SendCommand(string command)
        {
            if (string.IsNullOrEmpty(command))
                return null;

            bool wasOpen = IsConnected.ValueT;
            if (!IsConnected.ValueT)
            {
                OpenPort();
                if (!IsConnected.ValueT)
                    throw new Exception("Can not open TCP/IP-Port");
            }
            PAScaleMode lastScaleMode = CurrentScaleMode;
            if (lastScaleMode == PAScaleMode.ReadingWeights)
                StopReadWeightData();

            using (ACMonitor.Lock(_61000_LockPort))
            {
                NetworkStream stream = this.TcpClient.GetStream();
                if (stream == null || !stream.CanWrite)
                    throw new Exception("Can't write to Stream");

                if ((DateTime.Now - _LastWrite).TotalSeconds < 1)
                    Thread.Sleep(1000);

                byte[] data = Encoding.ASCII.GetBytes(command);
                stream.Write(data, 0, data.Length);
                _LastWrite = DateTime.Now;

                Thread.Sleep(500);

                byte[] myReadBuffer = new byte[1024];
                StringBuilder myCompleteMessage = new StringBuilder();
                int numberOfBytesRead = 0;
                do
                {
                    numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length);
                    myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));
                }
                while (stream.DataAvailable);
                string readResult = myCompleteMessage.ToString();
                return readResult;
            }
        }

        public bool TestConnection()
        {
            bool wasOpen = IsConnected.ValueT;
            if (!IsConnected.ValueT)
            {
                OpenPort();
                if (!IsConnected.ValueT)
                    return false;
            }
            PAScaleMode lastScaleMode = CurrentScaleMode;
            if (lastScaleMode == PAScaleMode.ReadingWeights)
                StopReadWeightData();

            string readResult = null;

            try
            {
                using (ACMonitor.Lock(_61000_LockPort))
                {
                    NetworkStream stream = this.TcpClient.GetStream();
                    if (stream == null || !stream.CanWrite)
                        throw new Exception("Can't write to Stream");

                    byte[] data = Encoding.ASCII.GetBytes("<f>");
                    stream.Write(data, 0, data.Length);
                    _LastWrite = DateTime.Now;

                    Thread.Sleep(50);

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
                }
            }
            catch (Exception e)
            {
                Messages.LogException(this.GetACUrl(), "TestConnection", e);
                return false;
            }

            StartReadWeightData();
            return !string.IsNullOrEmpty(readResult);
        }

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

        #region Helper-Methods
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "OpenPort":
                    OpenPort();
                    return true;
                case "IsEnabledOpenPort":
                    result = IsEnabledOpenPort();
                    return true;
                case "ClosePort":
                    ClosePort();
                    return true;
                case "IsEnabledClosePort":
                    result = IsEnabledClosePort();
                    return true;
                case "StartReadWeightData":
                    StartReadWeightData();
                    return true;
                case "IsEnabledStartReadWeightData":
                    result = IsEnabledStartReadWeightData();
                    return true;
                case "StopReadWeightData":
                    StopReadWeightData();
                    return true;
                case "IsEnabledStopReadWeightData":
                    result = IsEnabledStopReadWeightData();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public static string NumberFormat(int length, short value)
        {
            string strVal = value.ToString();
            if (String.IsNullOrEmpty(strVal))
            {
                strVal = "                                                               ";
                strVal.Substring(0, length);
            }
            else if (strVal.Length < length)
            {
                strVal = strVal + "                                                               ";
                return strVal.Substring(0, length);
            }
            else if (strVal.Length > length)
            {
                return strVal.Substring(0, length);
            }
            return strVal;
        }
        #endregion

        #endregion
    }
}
