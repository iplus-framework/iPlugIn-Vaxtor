using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.processapplication;
using System.Net.Sockets;
using System;
using System.Text;
using System.Threading;
using System.Net;


namespace bosche.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Bosche Scale MCI'}de{'Bosche Waage MCI'}", Global.ACKinds.TPAModule, Global.ACStorableTypes.Required)]
    public class PAEScaleBoscheMCI : PAEScaleCalibratable
    {
        public PAEScaleBoscheMCI(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        #region c'tors

        public override bool ACPostInit()
        {
            OpenPort();
            //if (CurrentScaleMode == PAScaleMode.ReadingWeights)
            //{
            CurrentScaleMode = PAScaleMode.Off;
            //StartReadWeightData();
            ReadWeightData();
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

        [ACPropertyInfo(true, 405)]
        public int ReceiveTimeout { get; set; }

        [ACPropertyInfo(true, 406)]
        public int SendTimeout { get; set; }

        protected readonly ACMonitorObject _61000_LockPort = new ACMonitorObject(61000);

        [ACPropertyInfo(true, 400, "Config", "en{'Polling [ms]'}de{'Abfragezyklus [ms]'}", DefaultValue = 500)]
        public int PollingInterval { get; set; }

        [ACPropertyInfo(true, 401, "Config", "en{'Communication on'}de{'Kommunikation ein'}", DefaultValue = false)]
        public bool TCPCommEnabled { get; set; }

        [ACPropertyInfo(true, 402, "Config", "en{'IP-Address'}de{'IP-Adresse'}", DefaultValue = "192.168.100.1")]
        public string ServerIPV4Address { get; set; }

        [ACPropertyInfo(true, 403, "Config", "en{'DNS-Name [ms]'}de{'DNS-Name [ms]'}")]
        public string ServerDNSName { get; set; }

        [ACPropertyInfo(true, 404, "Config", "en{'Port-No.'}de{'Port-Nr.'}", DefaultValue = (int)23)]
        public int PortNo { get; set; }

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

        protected TcpClient _tcpClient = null;
        [ACPropertyInfo(9999)]
        public TcpClient TcpClient
        {
            get { return _tcpClient; }
        }

        [ACPropertyBindingSource(9999, "Error", "en{'Communication alarm'}de{'Communication-Alarm'}", "", false, false)]
        public IACContainerTNet<PANotifyState> CommAlarm { get; set; }

        private int _CountWrites = 0;
        private int _CountReads = 0;
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

        #region Methods

        [ACMethodInfo("Read weight", "en{'Read weight'}de{'Gewicht ablesen'}", 701)]
        public void ReadWeights(string source)
        {
            string[] sourceSplit = source.Split(';');
            string targetWord = "GRO:";

            // gross weight from string
            foreach (var word in sourceSplit)
            {
                if (word.Contains(targetWord))
                {
                    string result = word.Substring(5);
                    try
                    {
                        double weight = Double.Parse(result);
                        ActualValue.ValueT = weight;
                        ActualWeight.ValueT = weight;
                        Console.WriteLine(weight);
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine($"Unable to parse '{result}'");
                    }
                }
            }
        }

        #endregion

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
                    _tcpClient.Dispose();
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
                if (_WaitForReconnect == 0)
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
                    //<F> Send value continuously
                    //stream.Write(Cmd3xxx.GetValueAllTime, 0, Cmd3xxx.C_CmdLength);
                    _LastWrite = DateTime.Now;

                    CurrentScaleMode = PAScaleMode.ReadingWeights;
                    _CountWrites++;
                }
            }
            catch (Exception e)
            {
                CommAlarm.ValueT = PANotifyState.AlarmOrFault;
                if (IsAlarmActive("CommAlarm", e.Message) == null)
                    this.Root.Messages.LogException(GetACUrl(), "PAEScaleBoscheMCI.StartReadWeightData()", e.Message);
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

                    //// <A> Send value x1 immediately
                    //stream.Write(Cmd3xxx.GetValueOneTime, 0, Cmd3xxx.C_CmdLength);
                    _LastWrite = DateTime.Now;

                    CurrentScaleMode = PAScaleMode.Idle;
                    _CountWrites++;
                }
            }
            catch (Exception e)
            {
                CommAlarm.ValueT = PANotifyState.AlarmOrFault;
                if (IsAlarmActive("CommAlarm", e.Message) == null)
                    this.Root.Messages.LogException(GetACUrl(), "PAEScaleBoscheMCI.StopReadWeightData()", e.Message);
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
                            ReadWeights(readResult);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                CommAlarm.ValueT = PANotifyState.AlarmOrFault;
                if (IsAlarmActive("CommAlarm", e.Message) == null)
                    this.Root.Messages.LogException(GetACUrl(), "PAEScaleBoscheMCI.ReadWeightData()", e.Message);
                OnNewAlarmOccurred(CommAlarm, e.Message, true);
                if (this.TcpClient == null || !this.TcpClient.Connected)
                    OpenPort();
                IsConnected.ValueT = this.TcpClient.Connected;
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

                    //stream.Write(Cmd3xxx.GetValueAllTime, 0, Cmd3xxx.C_CmdLength);
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

            //StartReadWeightData(); ja sam dodao ReadWeightData();
            ReadWeightData();
            return !string.IsNullOrEmpty(readResult);
        }

        #endregion


    }


}


