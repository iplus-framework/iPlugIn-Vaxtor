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

namespace systec.mes.processapplication
{
    //TODO: Implement logic for continuos scale reading (separate thread)
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'PAMScaleITx000 TCP'}de{'PAMScaleITx000 TCP'}", Global.ACKinds.TPAModule, Global.ACStorableTypes.Required, false, true)]
    public class PAMScaleITx000 : PAEScaleCalibratableMES
    {
        #region c'tors
        public PAMScaleITx000(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _SystecRevision = new ACPropertyConfigValue<ushort>(this, "SystecRevision", 0);
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            if (Outputs1 != null)
                (Outputs1 as IACPropertyNetServer).ValueUpdatedOnReceival += BitAccess_PropertyChanged;
            if (Outputs2 != null)
                (Outputs2 as IACPropertyNetServer).ValueUpdatedOnReceival += BitAccess_PropertyChanged;
            CurrentScaleMode = PAScaleMode.Idle;
            return true;
        }

        public override bool ACPostInit()
        {
            _ = SystecRevision;
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
            if (Outputs1 != null)
                (Outputs1 as IACPropertyNetServer).ValueUpdatedOnReceival -= BitAccess_PropertyChanged;
            if (Outputs2 != null)
                (Outputs2 as IACPropertyNetServer).ValueUpdatedOnReceival -= BitAccess_PropertyChanged;
            return base.ACDeInit(deleteACClassTask);
        }
        #endregion

        #region Info

        // TELEGRAMM:
        /*
        Fehlercode Status Datum Zeit Ident-Nr. Waagen-Nr. Brutto Tara Netto Einheit Taracode Wägebereich Terminal-Nr. CRC16
        1 2 3 4 5 6 7 8 9 10 11 12 13 14

        1 = Fehlercode (00 = kein Fehler)
        2 = Waagen-Status (2-stellig):
        erstes Byte: 0 = Waage in Ruhe, 1 = Waage in Bewegung
        zweites Byte: 0 = Brutto positiv, 1 = Brutto negativ
        3 = Aktuelles Datum (8-stellig, Format je nach Konfiguration)
        4 = Aktuelle Zeit (5-stellig, Format HH:MM)
        5 = Ident-Nummer (4-stellig)
        6 = Waagen-Nr. (immer '1')
        7 = Brutto (8-stellig)
        8 = Tara (8-stellig)
        9 = Netto (8-stellig)
        10 = Einheit (2-stellig, kg, g , t oder lb)
        11 = Taracode (2-stellig): PT = Handtara (Preset Tare); _T = Taraausgleich
        (Autotara); __ = Waage nicht tariert, (_ = Leerzeichen)
        12 = Wägebereich bei Mehrteilungswaagen, sonst immer Leerzeichen
        13 = Terminal-Nr. (3-stellig) wie in der Gruppe 'General' des Service Mode
        eingegeben
        14 = Prüfziffer nach CRC16 (8-stellig)

        Beispiel: "000004.11.0311:54   41     330       0     330kg     1   56880"
                  "<000004.11.0311:54   41     330       0     330kg     1   56880>r/n/"*/

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

        #region config
        private ACPropertyConfigValue<ushort> _SystecRevision;
        [ACPropertyConfig("en{'Systec protocol revision'}de{'Systec protocol revision'}")]
        public ushort SystecRevision
        {
            get => _SystecRevision.ValueT;
            set => _SystecRevision.ValueT = value;
        }
        #endregion



        #region TCP-Communication
        [ACPropertyInfo(true, 401, "Config", "en{'Communication on'}de{'Kommunikation ein'}", DefaultValue = false)]
        public bool TCPCommEnabled { get; set; }

        protected TcpClient _tcpClient = null;
        [ACPropertyInfo(9999)]
        public TcpClient TcpClient
        {
            get { return _tcpClient; }
        }

        protected readonly ACMonitorObject _61000_LockPort = new ACMonitorObject(61000);

        [ACPropertyInfo(true, 402, "Config", "en{'IP-Address'}de{'IP-Adresse'}", DefaultValue = "195.0.133.164")]
        public string ServerIPV4Address
        {
            get;
            set;
        }

        //[ACPropertyInfo(true, 403, "Config", "en{'DNS-Name [ms]'}de{'DNS-Name [ms]'}")]
        //public string ServerDNSName { get; set; }


        [ACPropertyInfo(true, 404, "Config", "en{'Port-No.'}de{'Port-Nr.'}", DefaultValue = (int)6000)]
        public int PortNo
        {
            get;
            set;
        }

        [ACPropertyInfo(true, 405, "Config", "en{'Read-Timeout [ms]'}de{'Lese-Timeout [ms]'}", DefaultValue = 2000)]
        public int ReceiveTimeout
        {
            get;
            set;
        }

        [ACPropertyInfo(true, 406, "Config", "en{'Write-Timeout [ms]'}de{'Schreibe-Timeout [ms]'}", DefaultValue = 2000)]
        public int SendTimeout
        {
            get;
            set;
        }

        [ACPropertyInfo(true, 400, "Config", "en{'Polling [ms]'}de{'Abfragezyklus [ms]'}", DefaultValue = 1000)]
        public int PollingInterval { get; set; }

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

        protected bool _SyncDigitalOutputs = false;

        private DateTime _LastWrite = DateTime.Now;

        [ACPropertyBindingSource(408, "", "en{'Last weight'}de{'Letztes Gewicht'}")]
        public IACContainerTNet<string> LastWeight { get; set; }

        [ACPropertyBindingSource(409, "Error", "en{'Communication alarm'}de{'Communication-Alarm'}", "", false, false)]
        public IACContainerTNet<PANotifyState> CommAlarm { get; set; }

        [ACPropertyBindingSource(410, "Error", "en{'Inputs 0-31'}de{'Eingänge 0-31'}", "", false, true)]
        public IACContainerTNet<BitAccessForInt32> Inputs1 { get; set; }

        [ACPropertyBindingSource(411, "Error", "en{'Inputs 32-63'}de{'Eingänge 32-63'}", "", false, true)]
        public IACContainerTNet<BitAccessForInt32> Inputs2 { get; set; }

        [ACPropertyBindingSource(412, "Error", "en{'Outputs 0-31'}de{'Ausgänge 0-31'}", "", false, true)]
        public IACContainerTNet<BitAccessForInt32> Outputs1 { get; set; }

        [ACPropertyBindingSource(413, "Error", "en{'Outputs 32-63'}de{'Ausgänge 32-63'}", "", false, true)]
        public IACContainerTNet<BitAccessForInt32> Outputs2 { get; set; }

        [ACPropertyBindingSource(414, "Error", "en{'Last Outputs 0-31'}de{'Letzte Ausgänge 0-31'}", "", false, true)]
        public IACContainerTNet<BitAccessForInt32> LastOutputs1 { get; set; }

        [ACPropertyBindingSource(415, "Error", "en{'Last Outputs 32-63'}de{'Letzte Ausgänge 32-63'}", "", false, true)]
        public IACContainerTNet<BitAccessForInt32> LastOutputs2 { get; set; }
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

                    if (CurrentScaleMode == PAScaleMode.ReadingWeights && IsEnabledStartReadWeightData())
                        PollWeightData();
                    if (_SyncDigitalOutputs && IsEnabledSetDigitalOutputs())
                    {
                        SetDigitalOutputs();
                        _SyncDigitalOutputs = false;
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
            ReadBW(true);
        }

        #endregion

        #region Methods => Connection

        [ACMethodInteraction("Comm", "en{'Open Connection'}de{'Öffne Verbindung'}", 200, true)]
        public void OpenPort()
        {
            if (!IsEnabledOpenPort())
                return;
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
                    if (IsAlarmActive(CommAlarm, e.Message) == null)
                        Messages.LogException(GetACUrl(), "PAMIT3000.OpenPort()", e.Message);
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
                if (String.IsNullOrEmpty(this.ServerIPV4Address)/* && String.IsNullOrEmpty(this.ServerDNSName)*/)
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

        #endregion

        #region Commands
        [ACMethodInteraction("Comm", "en{'Start reading weights'}de{'Starte Gewichtsdaten lesen'}", 202, true)]
        public void StartReadWeightData()
        {
            if (!IsEnabledStartReadWeightData())
                return;
            CurrentScaleMode = PAScaleMode.ReadingWeights;
        }

        public bool IsEnabledStartReadWeightData()
        {
            return TCPCommEnabled;
        }


        [ACMethodInteraction("Comm", "en{'Stop reading weights'}de{'Stoppe Gewichtsdaten lesen'}", 203, true)]
        public void StopReadWeightData()
        {
            if (!IsEnabledStopReadWeightData())
                return;
            CurrentScaleMode = PAScaleMode.Idle;
        }

        public bool IsEnabledStopReadWeightData()
        {
            return TCPCommEnabled;
        }


        [ACMethodInteraction("", "en{'Read brutto weight'}de{'Brutto-Gewicht lesen'}", 200, true)]
        public void ReadBruttoWeightInt()
        {
            ITx000Result result = ReadBruttoWeight();
            if (result != null)
            {
                this.ActualValue.ValueT = result.BruttoWeight;
                Messages.LogDebug(this.GetACUrl(), "ReadBruttoWeightInt()", result.ToString());
            }
        }

        public bool IsEnabledReadBruttoWeightInt()
        {
            return TCPCommEnabled;
        }


        [ACMethodInfo("", "en{'Read brutto weight'}de{'Brutto-Gewicht lesen'}", 201, true)]
        public ITx000Result ReadBruttoWeight()
        {
            return ReadBW();
        }

        [ACMethodInteraction("", "en{'Read digital inputs'}de{'Digitaleingänge lesen'}", 202, true)]
        public void ReadDigitalInputs()
        {
            GetDigitalInputs();
        }

        public BitAccessForInt32 GetDigitalInputs()
        {
            try
            {
                if (!IsConnected.ValueT)
                {
                    OpenPort();
                    if (TcpClient == null || !TcpClient.Connected)
                    {
                        ReportError("Can not open TCP/IP-Port!");
                        return null;
                    }
                }

                using (ACMonitor.Lock(_61000_LockPort))
                {
                    NetworkStream ns = TcpClient.GetStream();
                    if (ns == null || !ns.CanWrite)
                    {
                        ClosePort();
                        return null;
                    }

                    string command = "<GI;>"; // Scale 1

                    Byte[] data = Encoding.ASCII.GetBytes(command);
                    ns.Write(data, 0, data.Length);
                    _LastWrite = DateTime.Now;

                    byte[] myReadBuffer = new byte[1024];
                    StringBuilder myCompleteMessage = new StringBuilder();
                    int numberOfBytesRead = 0;
                    do
                    {
                        numberOfBytesRead = ns.Read(myReadBuffer, 0, myReadBuffer.Length);
                        if (numberOfBytesRead > 0)
                            myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));
                    }
                    while (ns.DataAvailable);

                    string readResult = myCompleteMessage.ToString();

                    if (string.IsNullOrEmpty(readResult) || readResult.Length < 67)
                        return null;

                    string[] resArr = readResult.Split(';');
                    if (resArr == null || resArr.Length < 2)
                        return null;

                    int status = 0;
                    if (!int.TryParse(resArr[0], out status))
                    {
                        ReportError("Can't read waage status!");
                        return null;
                    }
                    var error = ExtractErrors(status);
                    if (error != null)
                    {
                        ReportError(error);
                        return null;
                    }

                    BitAccessForInt32 input1 = new BitAccessForInt32();
                    if (resArr[1].Length >= 32)
                        input1.SetFromString(resArr[1].Substring(0,32));
                    this.Inputs1.ValueT = input1;
                    BitAccessForInt32 input2 = new BitAccessForInt32();
                    if (resArr[1].Length >= 64)
                        input2.SetFromString(resArr[1].Substring(32, 32));
                    this.Inputs2.ValueT = input2;
                    return input1;
                }
            }
            catch (Exception e)
            {
                ReportError(e.Message);
                return null;
            }
            finally
            {
                ClosePort();
            }
        }

        public bool IsEnabledReadDigitalInputs()
        {
            return TCPCommEnabled && SystecRevision >= 19;
        }

        [ACMethodInteraction("", "en{'Set digital outputs'}de{'Digitalausgänge setzen'}", 203, true)]
        public void SetDigitalOutputs()
        {
            try
            {
                if (!IsConnected.ValueT)
                {
                    OpenPort();
                    if (TcpClient == null || !TcpClient.Connected)
                    {
                        ReportError("Can not open TCP/IP-Port!");
                        return;
                    }
                }

                using (ACMonitor.Lock(_61000_LockPort))
                {
                    NetworkStream ns = TcpClient.GetStream();
                    if (ns == null || !ns.CanWrite)
                    {
                        ClosePort();
                        ReportError("Can not write to stream!");
                        return;
                    }
                    BitAccessForInt32 outputs1 = Outputs1.ValueT;
                    BitAccessForInt32 lastout1 = LastOutputs1.ValueT;
                    for (short i = 0; i < 32; i++)
                    {
                        bool newBit = outputs1.GetBitValue(i);
                        bool lastBit = lastout1.GetBitValue(i);
                        if (newBit != lastBit)
                        {
                            string command = newBit ? String.Format("<OS{0:00}>", (i + 1)) : String.Format("<OC{0:00}>", (i + 1));
                            Byte[] data = Encoding.ASCII.GetBytes(command);
                            ns.Write(data, 0, data.Length);

                            byte[] myReadBuffer = new byte[1024];
                            int numberOfBytesRead = 0;
                            string result = null;
                            do
                            {
                                numberOfBytesRead = ns.Read(myReadBuffer, 0, myReadBuffer.Length);
                                if (numberOfBytesRead > 0)
                                    result = Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead);
                            }
                            while (ns.DataAvailable);

                            if (result != null)
                            {
                                int status = 0;
                                if (!int.TryParse(result, out status))
                                    continue;
                                var error = ExtractErrors(status);
                                if (error != null)
                                {
                                    ReportError(error);
                                    continue;
                                }
                                lastout1.SetBitValue(i, newBit);
                            }
                        }
                    }
                    LastOutputs1.ValueT = (BitAccessForInt32)outputs1.Clone();

                    BitAccessForInt32 outputs2 = Outputs2.ValueT;
                    BitAccessForInt32 lastout2 = LastOutputs2.ValueT;
                    for (short i = 0; i < 32; i++)
                    {
                        bool newBit = outputs2.GetBitValue(i);
                        bool lastBit = lastout2.GetBitValue(i);
                        if (newBit != lastBit)
                        {
                            string command = newBit ? String.Format("<OS{0:00}>", (i + 1)) : String.Format("<OC{0:00}>", (i + 1));
                            Byte[] data = Encoding.ASCII.GetBytes(command);
                            ns.Write(data, 0, data.Length);

                            byte[] myReadBuffer = new byte[1024];
                            int numberOfBytesRead = 0;
                            string result = null;
                            do
                            {
                                numberOfBytesRead = ns.Read(myReadBuffer, 0, myReadBuffer.Length);
                                if (numberOfBytesRead > 0)
                                    result = Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead);
                            }
                            while (ns.DataAvailable);

                            if (result != null)
                            {
                                int status = 0;
                                if (!int.TryParse(result, out status))
                                    continue;
                                var error = ExtractErrors(status);
                                if (error != null)
                                {
                                    ReportError(error);
                                    continue;
                                }
                                lastout2.SetBitValue(i, newBit);
                            }
                        }
                    }
                    LastOutputs2.ValueT = (BitAccessForInt32)outputs2.Clone();
                }
            }
            catch (Exception e)
            {
                ReportError(e.Message);
                return;
            }
            finally
            {
                ClosePort();
            }
        }

        public bool IsEnabledSetDigitalOutputs()
        {
            return TCPCommEnabled && SystecRevision >= 19;
        }

        #endregion

        #region overrides
        public override Msg RegisterAlibiWeight()
        {
            // Delegate to other Thread because of Socket-Blocking
            if (!IsSimulationOn && TCPCommEnabled)
            {
                ApplicationManager.ApplicationQueue.Add(() => { base.RegisterAlibiWeight(); });
                return null;
            }
            else
                return base.RegisterAlibiWeight();
        }

        public override Msg OnRegisterAlibiWeight()
        {
            if (!IsEnabledOnRegisterAlibiWeight())
                return null;

            if (IsSimulationOn)
                SimulateAlibi();
            else
                ReadBW(false);
            return null;
        }

        public override bool IsEnabledOnRegisterAlibiWeight()
        {
            return TCPCommEnabled || IsSimulationOn;
        }

        public override void SetZero()
        {
            if (!IsSimulationOn && TCPCommEnabled)
            {
                ApplicationManager.ApplicationQueue.Add(() => { SetZeroITInternal(); });
                return;
            }
            base.SetZero();
        }

        public override void Tare()
        {
            if (!IsSimulationOn && TCPCommEnabled)
            {
                ApplicationManager.ApplicationQueue.Add(() => { TareITInternal(); });
                return;
            }
            base.Tare();
        }

        //public override Msg SaveAlibiWeighing(PAOrderInfoEntry entity = null)
        //{
        //    return base.SaveAlibiWeighing(entity);
        //}
        #endregion

        #region Internal
        private ITx000Result ReadBW(bool ohneStillstand = false)
        {
            double weight = -99999;
            string wrongUniqID = "-1";

            try
            {
                if (!IsConnected.ValueT)
                {
                    OpenPort();
                    if (TcpClient == null || !TcpClient.Connected)
                        return ReportError("Can not open TCP/IP-Port!");
                }

                using (ACMonitor.Lock(_61000_LockPort))
                {
                    NetworkStream ns = TcpClient.GetStream();
                    if (ns == null || !ns.CanWrite)
                    {
                        ClosePort();
                        return ReportError("Can not write to stream!");
                    }

                    if ((DateTime.Now - _LastWrite).TotalSeconds < 1)
                        Thread.Sleep(1000);

                    if (SystecRevision == 0)
                    {
                        string command = "<RN>";
                        if (ohneStillstand)
                            command = "<RM01>";

                        Byte[] data = Encoding.ASCII.GetBytes(command);
                        ns.Write(data, 0, data.Length);
                        _LastWrite = DateTime.Now;

                        int maxTries = DetermineMaxTriesToReceiveTimeout();

                        for (int i = 0; i < maxTries; i++)
                        {
                            Thread.Sleep(100);
                            if (ns.DataAvailable)
                                break;
                        }

                        byte[] myReadBuffer = new byte[1024];
                        StringBuilder myCompleteMessage = new StringBuilder();
                        int numberOfBytesRead = 0;
                        do
                        {
                            numberOfBytesRead = ns.Read(myReadBuffer, 0, myReadBuffer.Length);
                            if (numberOfBytesRead > 0)
                                myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));
                        }
                        while (ns.DataAvailable);

                        string readResult = myCompleteMessage.ToString();

                        if (string.IsNullOrEmpty(readResult) || readResult.Length < 66)
                            return ReportError("Wrong answer from waage!");

                        int status = 0;
                        if (!int.TryParse(readResult.Substring(1, 2), out status))
                            return ReportError("Can't read waage status!");

                        var error = ExtractErrors(status);
                        if (error != null)
                            return ReportError(error);

                        int eichNr = -1;
                        string eichNrError = null;
                        if (!int.TryParse(readResult.Substring(18, 4), out eichNr))
                        {
                            eichNrError = "Can not read Alibi-Nummer";
                        }

                        string result = readResult.Substring(23, 8);
                        if (!string.IsNullOrEmpty(result) && double.TryParse(result, out weight))
                        {
                            LastWeight.ValueT = Math.Abs(weight).ToString();
                            ActualValue.ValueT = weight;
                            ITx000Result itResult = new ITx000Result(weight, eichNr.ToString(), eichNrError);
                            itResult.Date = readResult.Substring(5, 8);
                            itResult.Time = readResult.Substring(13, 5);
                            AlibiNo.ValueT = itResult.UniqueIdentifierNo;
                            AlibiWeight.ValueT = weight;
                            return itResult;
                        }
                    }
                    else if (SystecRevision >= 19)
                    {
                        string command = "<RN;1>"; // Scale 1
                        if (ohneStillstand)
                            command = "<RM;1>";

                        Byte[] data = Encoding.ASCII.GetBytes(command);
                        ns.Write(data, 0, data.Length);
                        _LastWrite = DateTime.Now;

                        byte[] myReadBuffer = new byte[1024];
                        StringBuilder myCompleteMessage = new StringBuilder();
                        int numberOfBytesRead = 0;
                        do
                        {
                            numberOfBytesRead = ns.Read(myReadBuffer, 0, myReadBuffer.Length);
                            if (numberOfBytesRead > 0)
                                myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));
                        }
                        while (ns.DataAvailable);

                        string readResult = myCompleteMessage.ToString();

                        if (string.IsNullOrEmpty(readResult) || readResult.Length < 63)
                            return ReportError("Wrong answer from waage!");

                        if (readResult[0] == '<') 
                            readResult = readResult.Substring(1);
                        int iEnd = readResult.LastIndexOf('>'); 
                        if (iEnd > 0)
                            readResult = readResult.Substring(0, iEnd);

                        if (string.IsNullOrEmpty(readResult) || readResult.Length < 63)
                            return ReportError("Wrong answer from waage!");

                        string[] resArr = readResult.Split(';');
                        if (resArr == null || resArr.Length < 10)
                            return ReportError("To less values in result!");

                        int status = 0;
                        if (!int.TryParse(resArr[0], out status))
                            return ReportError("Can't read waage status!");
                        var error = ExtractErrors(status);
                        if (error != null)
                            return ReportError(error);

                        NotStandStill.ValueT = resArr[1][0] == '1';
                        //bool empty = resArr[1][1] == '2';
 
                        int eichNr = -1;
                        string eichNrError = null;
                        string strEichNr = "";
                        if (ohneStillstand)
                        {
                            if (!int.TryParse(resArr[4], out eichNr))
                            {
                                strEichNr = string.Format("{0} {1}", resArr[2], resArr[3]);
                                eichNrError = "Can not read Alibi-Nummer";
                            }
                            else
                                strEichNr = string.Format("{0} {1} {2}", resArr[2], resArr[3], eichNr);
                        }

                        string result = resArr[6];
                        double.TryParse(result, out weight);
                        if (resArr[1][1] == '1')
                            weight *= 1;
                        if (resArr[9] == "t ")
                            weight *= 1000;
                        else if (resArr[9] == "g ")
                            weight *= 0.001;

                        ITx000Result itResult = new ITx000Result(weight, strEichNr, eichNrError);
                        LastWeight.ValueT = Math.Abs(weight).ToString();
                        ActualValue.ValueT = weight;
                        itResult.Date = resArr[2];
                        itResult.Time = resArr[3];
                        AlibiNo.ValueT = itResult.UniqueIdentifierNo;
                        AlibiWeight.ValueT = weight;

                        return itResult;
                    }
                    return new ITx000Result(weight, wrongUniqID, "Wrong answer from waage!");
                }
            }
            catch (Exception e)
            {
                return new ITx000Result(weight, wrongUniqID, e.Message);
            }
            finally
            {
                ClosePort();
            }
        }

        private string ExtractErrors(int status)
        {
            switch (status)
            {
                case 11:
                    return "General scale error (e.g. connection to load cell broken)";
                case 12: 
                    return "Scale in overload(weight exceeds maximum weighing range)";
                case 13:
                    return "Scale in motion(no rest after 6 seconds)";
                case 14:
                    return "Scale not available(e.g.only one scale configured, when using a dual ADM and scale not selected)";
                case 15:
                    return "Taring errors(e.g.tare weight formatting incorrect) or Zeroing error(outside zeroing range or scale in motion)";
                case 16:
                    return "Weight printer not ready(offline)";
                case 17:
                    return "Print pattern contains invalid command";
                case 20:
                    return "scale in underload";
                case 31:
                    return "Transmission error(e.g.data record too long or time-out)";
                case 32:
                    return "Invalid command";
                case 33:
                    return "Invalid parameter";
                case 34:
                    return "Display labeling not possible. (The display is currently being used by another online task used)";
                case 35:
                    return "Data output to printer not possible. (Other online task is currently using the data output on the printer)";
                case 36:
                    return "Printer error";
                case 37:
                    return "Traffic light control is active";
                case 44:
                    return "DUAL ADM error";
                case 66:
                    return "Service Mode is active";
                case 99:
                    return "Not possible";
                 default:
                    return null;
            }
        }

        private int DetermineMaxTriesToReceiveTimeout()
        {
            if (ReceiveTimeout > 0)
            {
                var tries = ReceiveTimeout / 100;
                if (tries > 5)
                {
                    return tries;
                }
            }
            return 10;
        }

        protected ITx000Result ReportError(string error)
        {
            CommAlarm.ValueT = PANotifyState.AlarmOrFault;
            if (IsAlarmActive(nameof(CommAlarm), error) == null)
                Messages.LogException(GetACUrl(), "ReportError()", error);
            OnNewAlarmOccurred(CommAlarm, error, true);
            return new ITx000Result(-99999, "-1", error);
        }

        protected virtual void BitAccess_PropertyChanged(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.AfterBroadcast)
                return;
            if (   (e.ValueEvent.Sender == EventRaiser.Source) && (e.ValueEvent.EventType == EventTypes.ValueChangedInSource)
                && (e.ValueEvent.InvokerInfo != null || this.Root.Initialized))
            {
                _SyncDigitalOutputs = true; 
            }
        }


        private ITx000Result SetZeroITInternal()
        {
            double weight = -99999;
            string wrongUniqID = "-1";

            try
            {
                if (!IsConnected.ValueT)
                {
                    OpenPort();
                    if (TcpClient == null || !TcpClient.Connected)
                        return ReportError("Can not open TCP/IP-Port!");
                }

                using (ACMonitor.Lock(_61000_LockPort))
                {
                    NetworkStream ns = TcpClient.GetStream();
                    if (ns == null || !ns.CanWrite)
                    {
                        ClosePort();
                        return ReportError("Can not write to stream!");
                    }

                    if ((DateTime.Now - _LastWrite).TotalSeconds < 1)
                        Thread.Sleep(1000);

                    if (SystecRevision == 0)
                    {
                        string command = "<SZ01>";

                        Byte[] data = Encoding.ASCII.GetBytes(command);
                        ns.Write(data, 0, data.Length);
                        _LastWrite = DateTime.Now;

                        int maxTries = DetermineMaxTriesToReceiveTimeout();

                        for (int i = 0; i < maxTries; i++)
                        {
                            Thread.Sleep(100);
                            if (ns.DataAvailable)
                                break;
                        }

                        byte[] myReadBuffer = new byte[1024];
                        StringBuilder myCompleteMessage = new StringBuilder();
                        int numberOfBytesRead = 0;
                        do
                        {
                            numberOfBytesRead = ns.Read(myReadBuffer, 0, myReadBuffer.Length);
                            if (numberOfBytesRead > 0)
                                myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));
                        }
                        while (ns.DataAvailable);

                        string readResult = myCompleteMessage.ToString();

                        if (string.IsNullOrEmpty(readResult) || readResult.Length < 66)
                            return ReportError("Wrong answer from waage!");

                        int status = 0;
                        if (!int.TryParse(readResult.Substring(1, 2), out status))
                            return ReportError("Can't read waage status!");

                        var error = ExtractErrors(status);
                        if (error != null)
                            return ReportError(error);
                        return new ITx000Result(-99999, "-1", null);
                    }
                    else if (SystecRevision >= 19)
                    {
                        string command = "<SZ;1>"; // Scale 1

                        Byte[] data = Encoding.ASCII.GetBytes(command);
                        ns.Write(data, 0, data.Length);
                        _LastWrite = DateTime.Now;

                        byte[] myReadBuffer = new byte[1024];
                        StringBuilder myCompleteMessage = new StringBuilder();
                        int numberOfBytesRead = 0;
                        do
                        {
                            numberOfBytesRead = ns.Read(myReadBuffer, 0, myReadBuffer.Length);
                            if (numberOfBytesRead > 0)
                                myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));
                        }
                        while (ns.DataAvailable);

                        string readResult = myCompleteMessage.ToString();

                        if (string.IsNullOrEmpty(readResult) || readResult.Length < 63)
                            return ReportError("Wrong answer from waage!");

                        if (readResult[0] == '<')
                            readResult = readResult.Substring(1);
                        int iEnd = readResult.LastIndexOf('>');
                        if (iEnd > 0)
                            readResult = readResult.Substring(0, iEnd);

                        if (string.IsNullOrEmpty(readResult) || readResult.Length < 63)
                            return ReportError("Wrong answer from waage!");

                        string[] resArr = readResult.Split(';');
                        if (resArr == null || resArr.Length < 10)
                            return ReportError("To less values in result!");

                        int status = 0;
                        if (!int.TryParse(resArr[0], out status))
                            return ReportError("Can't read waage status!");
                        var error = ExtractErrors(status);
                        if (error != null)
                            return ReportError(error);
                        return new ITx000Result(-99999, "-1", null);
                    }
                    return new ITx000Result(weight, wrongUniqID, "Wrong answer from waage!");
                }
            }
            catch (Exception e)
            {
                return new ITx000Result(weight, wrongUniqID, e.Message);
            }
            finally
            {
                ClosePort();
            }
        }

        private ITx000Result TareITInternal()
        {
            double weight = -99999;
            string wrongUniqID = "-1";

            try
            {
                if (!IsConnected.ValueT)
                {
                    OpenPort();
                    if (TcpClient == null || !TcpClient.Connected)
                        return ReportError("Can not open TCP/IP-Port!");
                }

                using (ACMonitor.Lock(_61000_LockPort))
                {
                    NetworkStream ns = TcpClient.GetStream();
                    if (ns == null || !ns.CanWrite)
                    {
                        ClosePort();
                        return ReportError("Can not write to stream!");
                    }

                    if ((DateTime.Now - _LastWrite).TotalSeconds < 1)
                        Thread.Sleep(1000);

                    if (SystecRevision == 0)
                    {
                        string command = "<TA01>";

                        Byte[] data = Encoding.ASCII.GetBytes(command);
                        ns.Write(data, 0, data.Length);
                        _LastWrite = DateTime.Now;

                        int maxTries = DetermineMaxTriesToReceiveTimeout();

                        for (int i = 0; i < maxTries; i++)
                        {
                            Thread.Sleep(100);
                            if (ns.DataAvailable)
                                break;
                        }

                        byte[] myReadBuffer = new byte[1024];
                        StringBuilder myCompleteMessage = new StringBuilder();
                        int numberOfBytesRead = 0;
                        do
                        {
                            numberOfBytesRead = ns.Read(myReadBuffer, 0, myReadBuffer.Length);
                            if (numberOfBytesRead > 0)
                                myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));
                        }
                        while (ns.DataAvailable);

                        string readResult = myCompleteMessage.ToString();

                        if (string.IsNullOrEmpty(readResult) || readResult.Length < 66)
                            return ReportError("Wrong answer from waage!");

                        int status = 0;
                        if (!int.TryParse(readResult.Substring(1, 2), out status))
                            return ReportError("Can't read waage status!");

                        var error = ExtractErrors(status);
                        if (error != null)
                            return ReportError(error);
                        return new ITx000Result(-99999, "-1", null);
                    }
                    else if (SystecRevision >= 19)
                    {
                        string command = "<TA;1>"; // Scale 1

                        Byte[] data = Encoding.ASCII.GetBytes(command);
                        ns.Write(data, 0, data.Length);
                        _LastWrite = DateTime.Now;

                        byte[] myReadBuffer = new byte[1024];
                        StringBuilder myCompleteMessage = new StringBuilder();
                        int numberOfBytesRead = 0;
                        do
                        {
                            numberOfBytesRead = ns.Read(myReadBuffer, 0, myReadBuffer.Length);
                            if (numberOfBytesRead > 0)
                                myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));
                        }
                        while (ns.DataAvailable);

                        string readResult = myCompleteMessage.ToString();

                        if (string.IsNullOrEmpty(readResult) || readResult.Length < 63)
                            return ReportError("Wrong answer from waage!");

                        if (readResult[0] == '<')
                            readResult = readResult.Substring(1);
                        int iEnd = readResult.LastIndexOf('>');
                        if (iEnd > 0)
                            readResult = readResult.Substring(0, iEnd);

                        if (string.IsNullOrEmpty(readResult) || readResult.Length < 63)
                            return ReportError("Wrong answer from waage!");

                        string[] resArr = readResult.Split(';');
                        if (resArr == null || resArr.Length < 10)
                            return ReportError("To less values in result!");

                        int status = 0;
                        if (!int.TryParse(resArr[0], out status))
                            return ReportError("Can't read waage status!");
                        var error = ExtractErrors(status);
                        if (error != null)
                            return ReportError(error);
                        return new ITx000Result(-99999, "-1", null);
                    }
                    return new ITx000Result(weight, wrongUniqID, "Wrong answer from waage!");
                }
            }
            catch (Exception e)
            {
                return new ITx000Result(weight, wrongUniqID, e.Message);
            }
            finally
            {
                ClosePort();
            }
        }

        #endregion
        #endregion

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;

            switch(acMethodName)
            {
                case nameof(ReadBruttoWeight):
                    result = ReadBruttoWeight();
                    return true;
                case nameof(IsEnabledReadBruttoWeightInt):
                    result = IsEnabledReadBruttoWeightInt();
                    return true;
                case nameof(OpenPort):
                    OpenPort();
                    return true;
                case nameof(ReadBruttoWeightInt):
                    ReadBruttoWeightInt();
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
                case nameof(ReadDigitalInputs):
                    ReadDigitalInputs();
                    return true;
                case nameof(IsEnabledReadDigitalInputs):
                    result = IsEnabledReadDigitalInputs();
                    return true;
                case nameof(SetDigitalOutputs):
                    SetDigitalOutputs();
                    return true;
                case nameof(IsEnabledSetDigitalOutputs):
                    result = IsEnabledSetDigitalOutputs();
                    return true;
            }

            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }
    }
}
