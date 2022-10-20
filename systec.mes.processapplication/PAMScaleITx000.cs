using gip.core.autocomponent;
using gip.core.datamodel;
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
    public class PAMScaleITx000 : PAModule
    {
        #region c'tors
        public PAMScaleITx000(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            return true;
        }

        public override bool ACPostInit()
        {
            return base.ACPostInit();
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
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

        #region Properties

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

        [ACPropertyInfo(true, 9999)]
        public int ReceiveTimeout
        {
            get;
            set;
        }

        [ACPropertyInfo(true, 9999)]
        public int SendTimeout
        {
            get;
            set;
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

        private DateTime _LastWrite = DateTime.Now;

        [ACPropertyBindingSource(9999, "", "en{'Last weight'}de{'Letztes Gewicht'}")]
        public IACContainerTNet<string> LastWeight { get; set; }

        [ACPropertyBindingSource(9999, "Error", "en{'Communication alarm'}de{'Communication-Alarm'}", "", false, false)]
        public IACContainerTNet<PANotifyState> CommAlarm { get; set; }

        #endregion

        #region Methods

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

        private ITx000Result ReadBW(bool ohneStillstand = false)
        {
            double weight = -99999;
            string wrongUniqID = "-1";

            if (!IsConnected.ValueT)
            {
                OpenPort();
                if (!TcpClient.Connected)
                    return new ITx000Result(weight, wrongUniqID, "Can not open TCP/IP-Port!");
            }

            using (ACMonitor.Lock(_61000_LockPort))
            {
                NetworkStream ns = TcpClient.GetStream();
                if (ns == null || !ns.CanWrite)
                {
                    ClosePort();
                    return new ITx000Result(weight, wrongUniqID, "Can not write to stream!");
                }

                if ((DateTime.Now - _LastWrite).TotalSeconds < 1)
                    Thread.Sleep(1000);

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
                    myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));
                }
                while (ns.DataAvailable);

                string readResult = myCompleteMessage.ToString();
                
                if (string.IsNullOrEmpty(readResult) || readResult.Length != 66)
                {
                    ClosePort();
                    return new ITx000Result(weight, wrongUniqID, "Wrong answer from waage!");
                }

                int status = 00;
                if (!int.TryParse(readResult.Substring(1, 2), out status))
                {
                    ClosePort();
                    return new ITx000Result(weight, wrongUniqID, "Can't read waage status!");
                }

                var error = ExtractErrors(status);
                if (error != null)
                {
                    ClosePort();
                    return new ITx000Result(weight, wrongUniqID, error);
                }

                int eichNr = -1;
                string eichNrError = null;
                if (!int.TryParse(readResult.Substring(18, 4), out eichNr))
                {
                    eichNrError = "Can not read Alibi-Nummer";
                }

                string result = readResult.Substring(23, 8);
                if (!string.IsNullOrEmpty(result) && double.TryParse(result, out weight))
                {
                    ClosePort();
                    LastWeight.ValueT = Math.Abs(weight).ToString();
                    ITx000Result itResult = new ITx000Result(weight, eichNr.ToString(), eichNrError);
                    itResult.Date = readResult.Substring(3, 8);
                    itResult.Time = readResult.Substring(11, 5);
                    return itResult;
                }
                else
                {
                    ClosePort();
                    return new ITx000Result(weight, wrongUniqID, "Wrong answer from waage!");
                }
            }
        }

        private string ExtractErrors(int status)
        {
            if (status == 00)
                return null;

            if (status == 13)
                return "Kein Stillstand";

            return "Wrong waage status: " + status;
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

        [ACMethodInfo("", "en{'Read brutto weight'}de{'Brutto-Gewicht lesen'}", 999, true)]
        public ITx000Result ReadBruttoWeight()
        {
            try
            {
                return ReadBW();
            }
            catch (Exception e)
            {
                ClosePort();
                return new ITx000Result(-99999, "-1", e.Message);
            }
        }

        #endregion

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;

            switch(acMethodName)
            {
                case "ReadBruttoWeight":
                    result = ReadBruttoWeight();
                    return true;
            }

            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }
    }
}
