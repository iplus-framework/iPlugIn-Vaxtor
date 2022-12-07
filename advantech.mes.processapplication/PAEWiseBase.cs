using gip.core.autocomponent;
using gip.core.communication;
using gip.core.datamodel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace advantech.mes.processapplication
{

    [ACClassInfo(Const.PackName_VarioAutomation, "en{'PAEWiseBase'}de{'PAEWiseBase'}", Global.ACKinds.TPAModule, Global.ACStorableTypes.Required, false, true)]
    public abstract class PAEWiseBase : PAModule
    {

        #region ctor's

        public PAEWiseBase(gip.core.datamodel.ACClass acType, gip.core.datamodel.IACObject content, gip.core.datamodel.IACObject parentACObject, gip.core.datamodel.ACValueList parameter, string acIdentifier = "") : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _StoreRecivedData = new ACPropertyConfigValue<bool>(this, "StoreRecivedData", false);
            _ExportDir = new ACPropertyConfigValue<string>(this, "ExportDir", "");
            _FileName = new ACPropertyConfigValue<string>(this, "FileName", "advantec_{0:yyyyMMddHHmmssfff}.json");
            _SensorMinCountValue = new ACPropertyConfigValue<int>(this, "SensorMinCountValue", 30000);
            _LogOutputUrl = new ACPropertyConfigValue<string>(this, "LogOutputUrl", "log_output");
            _LogMessageUrl = new ACPropertyConfigValue<string>(this, "LogMessageUrl", "log_message");
            _LogClearUrl = new ACPropertyConfigValue<string>(this, "LogClearUrl", "control");
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            bool baseResult = base.ACInit(startChildMode);

            using (ACMonitor.Lock(_20015_LockValue))
            {
                _DelegateQueue = new ACDelegateQueue(this.GetACUrl());
            }
            _DelegateQueue.StartWorkerThread();

            _ = StoreRecivedData;
            _ = ExportDir;
            _ = FileName;
            _ = SensorMinCountValue;
            _ = LogOutputUrl;
            _ = LogMessageUrl;
            _ = LogClearUrl;

            if (!CanSend())
            {

                // Error50585
                // ACRestClient not available!
                // ACRestClient nicht verfügbar!
                LogMessage(eMsgLevel.Error, "Error50585", nameof(ACInit), 56, null);
            }

            return baseResult;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            bool baseDeinit = base.ACDeInit(deleteACClassTask);

            _DelegateQueue.StopWorkerThread();
            using (ACMonitor.Lock(_20015_LockValue))
            {
                _DelegateQueue = null;
            }

            return baseDeinit;
        }

        public override bool ACPostInit()
        {
            if ((IsSessionConnected as IACPropertyNetTarget).Source == null)
                IsSessionConnected.ValueT = CanSend();
            return base.ACPostInit();
        }

        #endregion

        #region Configuration

        private ACPropertyConfigValue<bool> _StoreRecivedData;
        [ACPropertyConfig("StoreRecivedData")]
        public bool StoreRecivedData
        {
            get
            {
                return _StoreRecivedData.ValueT;
            }
            set
            {
                _StoreRecivedData.ValueT = value;
            }
        }

        private ACPropertyConfigValue<string> _ExportDir;
        [ACPropertyConfig("ExportDir")]
        public string ExportDir
        {
            get
            {
                return _ExportDir.ValueT;
            }
            set
            {
                _ExportDir.ValueT = value;
            }
        }

        private ACPropertyConfigValue<string> _FileName;
        [ACPropertyConfig("FileName")]
        public string FileName
        {
            get
            {
                return _FileName.ValueT;
            }
            set
            {
                _FileName.ValueT = value;
            }
        }

        private ACPropertyConfigValue<int> _SensorMinCountValue;
        [ACPropertyConfig("SensorMinCountValue")]
        public int SensorMinCountValue
        {
            get
            {
                return _SensorMinCountValue.ValueT;
            }
            set
            {
                _SensorMinCountValue.ValueT = value;
            }
        }

        private ACPropertyConfigValue<string> _LogOutputUrl;
        [ACPropertyConfig("LogOutputUrl")]
        public string LogOutputUrl
        {
            get
            {
                return _LogOutputUrl.ValueT;
            }
            set
            {
                _LogOutputUrl.ValueT = value;
            }
        }

        private ACPropertyConfigValue<string> _LogMessageUrl;
        [ACPropertyConfig("LogMessageUrl")]
        public string LogMessageUrl
        {
            get
            {
                return _LogMessageUrl.ValueT;
            }
            set
            {
                _LogMessageUrl.ValueT = value;
            }
        }

        private ACPropertyConfigValue<string> _LogClearUrl;
        [ACPropertyConfig("LogClearUrl")]
        public string LogClearUrl
        {
            get
            {
                return _LogClearUrl.ValueT;
            }
            set
            {
                _LogClearUrl.ValueT = value;
            }
        }

        #endregion

        #region Binding properties

        [ACPropertyBindingSource(210, "Error", "en{'Reading Counter Alarm'}de{'Reading Counter Alarm'}", "", false, true)]
        public IACContainerTNet<PANotifyState> IsReadingCounterAlarm { get; set; }

        [ACPropertyBindingSource(211, "Error", "en{'Error-text'}de{'Fehlertext'}", "", false, true)]
        public IACContainerTNet<string> ErrorText { get; set; }

        [ACPropertyBindingSource(212, "MeasureText", "en{'Measure'}de{'Messen'}", "", false, true)]
        public IACContainerTNet<string> MeasureText { get; set; }

        [ACPropertyBindingSource(213, "MeasureTime", "en{'Measure Time'}de{'Messen Time'}", "", false, true)]
        public IACContainerTNet<DateTime> MeasureTime { get; set; }

        #endregion

        #region Properties

        private JsonSerializerSettings _DefaultJsonSerializerSettings;
        public JsonSerializerSettings DefaultJsonSerializerSettings
        {
            get
            {
                if (_DefaultJsonSerializerSettings == null)
                    _DefaultJsonSerializerSettings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        //TypeNameHandling = TypeNameHandling.None,
                        //DefaultValueHandling = DefaultValueHandling.Ignore,
                        Formatting = Newtonsoft.Json.Formatting.None
                    };

                return _DefaultJsonSerializerSettings;
            }
        }


        private ACRef<ACRestClient> _Client;
        public ACRestClient Client
        {
            get
            {
                if (_Client != null)
                    return _Client.ValueT;
                var client = FindChildComponents<ACRestClient>(c => c is ACRestClient).FirstOrDefault();
                if (client != null)
                {
                    // TODO: find right place for this
                    ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                    _Client = new ACRef<ACRestClient>(client, this);
                    return _Client.ValueT;
                }
                return null;
            }
        }

        private ACDelegateQueue _DelegateQueue = null;
        public ACDelegateQueue DelegateQueue
        {
            get
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _DelegateQueue;
                }
            }
        }

        #endregion

        #region Methods

        #region Methods -> ACMethod

        #region Methods -> ACMethod -> Reset


        [ACMethodInteraction("", "en{'Reset counter'}de{'Zähler zurücksetzen'}", 220, true)]
        public void Reset()
        {
            ResetCounter();
        }

        public bool IsEnabledReset()
        {
            return CanSend();
        }

        [ACMethodInfo("", "en{'Reset counter'}de{'Zähler zurücksetzen'}", 201, true)]
        public bool ResetCounter()
        {
            bool success = false;
            ErrorText.ValueT = null;
            MeasureText.ValueT = null;
            MeasureTime.ValueT = DateTime.MinValue;

            if (!IsEnabledResetCounter())
            {
                // Error50586.
                // Unable to reset! Counter is not ready.
                // Zurücksetzen nicht möglich! Zähler ist nicht bereit.
                LogMessage(eMsgLevel.Error, "Error50586", nameof(ResetCounter), 296, null);
                return false;
            }

            try
            {
                FilterClear filter = new FilterClear();
                string requestJson = JsonConvert.SerializeObject(filter, DefaultJsonSerializerSettings);
                using (var content = new StringContent(requestJson, Encoding.UTF8, "application/json"))
                {
                    WSResponse<string> response = this.Client.Patch(content, LogClearUrl);

                    if (response.Suceeded)
                    {
                        success = true;
                    }
                    else
                    {
                        // Error50587.
                        // Error by resetting counter! Error {0}.
                        // Fehler beim Zurücksetzen des Zählers! Fehler {0}.
                        LogMessage(eMsgLevel.Error, "Error50587", nameof(ReadCounter), 317, response.Message?.Message);
                    }
                }
            }
            catch (Exception ec)
            {
                // Error50587.
                // Error by resetting counter! Error {0}.
                // Fehler beim Zurücksetzen des Zählers! Fehler {0}.
                LogMessage(eMsgLevel.Exception, "Error50587", nameof(ReadCounter), 324, ec.Message);
                Messages.LogException(this.GetACUrl(), "ResetCounter(100)", ec);
            }

            return success;
        }

        public bool IsEnabledResetCounter()
        {
            return CanSend();
        }

        #endregion

        #region Methods -> ACMethod -> Test Connection

        [ACMethodInteraction("", "en{'Test Connection'}de{'Verbindung testen'}", 200, true)]
        public void Available()
        {
            if (!IsEnabledAvailable())
                return;
            ReadAvailable();
        }

        public bool IsEnabledAvailable()
        {
            return CanSend();
        }

        [ACMethodInfo("", "en{'Test Connection'}de{'Verbindung testen'}", 201, true)]
        public long ReadAvailable()
        {
            long result = 0;
            ErrorText.ValueT = null;
            MeasureText.ValueT = null;
            MeasureTime.ValueT = DateTime.MinValue;

            if (!IsEnableReadAvailable())
                return 0;

            try
            {
                WSResponse<long?> amountResult = GetAmount(LogOutputUrl);
                if (amountResult.Suceeded)
                {
                    result = amountResult.Data ?? 0;
                    MeasureText.ValueT = result.ToString();
                    MeasureTime.ValueT = DateTime.Now;
                }
                else
                {
                    // Error50588
                    // Error by reading counter! Error {0}.
                    // Fehler beim Lesen des Zählers! Fehler {0}.
                    LogMessage(eMsgLevel.Error, "Error50588", nameof(ReadAvailable), 374, amountResult.Message?.Message);
                }
            }
            catch (Exception ec)
            {
                // Error50588
                // Error by reading counter! Error {0}.
                // Fehler beim Lesen des Zählers! Fehler {0}.
                LogMessage(eMsgLevel.Exception, "Error50588", nameof(ReadAvailable), 379, ec.Message);
                Messages.LogException(this.GetACUrl(), "ReadAvailable(100)", ec);
            }

            return result;
        }

        public bool IsEnableReadAvailable()
        {
            return CanSend();
        }

        #endregion

        #region Methods -> ACMethod -> Read


        [ACMethodInteraction("", "en{'Count'}de{'Zählen'}", 210, true)]
        public void Read()
        {
            if (!IsEnabledRead())
                return;
            ReadCounter();
        }

        public bool IsEnabledRead()
        {
            return CanSend();
        }

        [ACMethodInfo("", "en{'Count'}de{'Zählen'}", 221, true)]
        public List<ChannelResult> ReadCounter()
        {
            ErrorText.ValueT = null;
            MeasureText.ValueT = null;
            MeasureTime.ValueT = DateTime.MinValue;

            List<ChannelResult> result = null;

            if (!IsEnabledReadCounter())
            {
                // Error50586.
                // Unable to reset! Counter is not ready.
                // Zurücksetzen nicht möglich! Zähler ist nicht bereit.
                LogMessage(eMsgLevel.Error, "Error50586", nameof(ReadCounter), 421, null);

                PABase parentPAObj = FindParentComponent<PABase>();
                if (parentPAObj != null && parentPAObj.IsSimulationOn)
                    result = SimulateCountData();
                else
                    return result;
            }
            else
            {
                try
                {
                    WSResponse<Wise4000Data> dataResult = GetData(LogOutputUrl, LogMessageUrl);
                    if (dataResult.Data != null && (dataResult.Message == null || dataResult.Message.MessageLevel < eMsgLevel.Failure))
                    {
                        result = CountData(dataResult.Data);
                        string json = JsonConvert.SerializeObject(result);
                        MeasureText.ValueT = json;
                        MeasureTime.ValueT = DateTime.Now;
                    }
                    else
                    {
                        // Error50588
                        // Error by reading counter! Error {0}.
                        // Fehler beim Lesen des Zählers! Fehler {0}.
                        LogMessage(eMsgLevel.Error, "Error50588", nameof(ReadCounter), 443, dataResult.Message?.Message);
                        PABase parentPAObj = FindParentComponent<PABase>();
                        if (parentPAObj != null && parentPAObj.IsSimulationOn)
                            result = SimulateCountData();
                    }
                }
                catch (Exception ec)
                {
                    // Error50588
                    // Error by reading counter! Error {0}.
                    // Fehler beim Lesen des Zählers! Fehler {0}.
                    LogMessage(eMsgLevel.Exception, "Error50588", nameof(ReadCounter), 450, ec.Message);
                    Messages.LogException(this.GetACUrl(), "ReadCounter(100)", ec);
                }
            }

            return result;
        }

        public bool IsEnabledReadCounter()
        {
            return CanSend();
        }

        #endregion

        #endregion

        public virtual bool IsEnabledGetValues()
        {
            if (!CanSend())
                return false;
            return true;
        }

        #region Methods -> Others

        public void ExportData(string exportDir, string fileName, string jsonContent)
        {
            try
            {
                string file = string.Format(fileName, DateTime.Now);
                string fullFileName = Path.Combine(exportDir, file);
                File.WriteAllText(fullFileName, jsonContent);
            }
            catch (Exception ec)
            {
                Messages.LogException(GetACUrl(), "ExportData(10)", ec);
            }
        }

        public virtual WSResponse<long?> GetAmount(string logOutputUrl)
        {
            WSResponse<long?> result = new WSResponse<long?>();
            Filter filter = new Filter();
            string requestJson = JsonConvert.SerializeObject(filter, DefaultJsonSerializerSettings);
            using (var content = new StringContent(requestJson, Encoding.UTF8, "application/json"))
            {
                WSResponse<string> setFilterResponse = Client.Patch(content, logOutputUrl);
                if (setFilterResponse.Suceeded)
                {
                    IsSessionConnected.ValueT = true;
                    WSResponse<Filter> responseFilter = Client.Get<Filter>(logOutputUrl);
                    if (responseFilter.Suceeded)
                    {
                        result.Data = responseFilter.Data.Amt;
                    }
                    else
                    {
                        result.Message = responseFilter.Message;
                    }
                }
                else
                {
                    IsSessionConnected.ValueT = false;
                    result.Message = setFilterResponse.Message;
                }
            }
            return result;
        }

        public virtual WSResponse<Wise4000Data> GetData(string logOutputUrl, string logMessageUrl)
        {
            WSResponse<Wise4000Data> result = new WSResponse<Wise4000Data>();
            WSResponse<long?> amountResult = GetAmount(logOutputUrl);
            if (amountResult.Data != null)
            {
                Filter filter = new Filter();
                filter.FltrEnum = FltrEnum.AmountFilter;
                filter.Amt = amountResult.Data.Value;
                string requestJson = JsonConvert.SerializeObject(filter, DefaultJsonSerializerSettings);
                using (var content = new StringContent(requestJson, Encoding.UTF8, "application/json"))
                {
                    WSResponse<string> setFilterResponse = Client.Patch(content, logOutputUrl);

                    if (setFilterResponse.Suceeded)
                    {
                        IsSessionConnected.ValueT = true;

                        WSResponse<Wise4000Data> dataResponse = new WSResponse<Wise4000Data>();
                        Task<WSResponse<Wise4000Data>> dataResponseRequest = GetAsyncWise<Wise4000Data>(Client, logMessageUrl);
                        dataResponseRequest.Wait();
                        dataResponse = dataResponseRequest.Result;

                        if (dataResponse.Suceeded)
                        {
                            result.Data = dataResponse.Data;
                        }
                        else
                        {
                            result.Message = dataResponse.Message;
                        }
                    }
                    else
                    {
                        IsSessionConnected.ValueT = false;
                        result.Message = setFilterResponse.Message;
                    }
                }
            }
            else if (amountResult.Data != null)
            {
                result.Data = new Wise4000Data() { LogMsg = new LogMsg[] { } };
            }
            else
            {
                result.Message = amountResult.Message;
            }
            return result;
        }

        public List<ChannelResult> CountData(Wise4000Data data)
        {
            List<ChannelResult> result = new List<ChannelResult>();
            if (data.LogMsg != null)
            {
                try
                {
                    foreach (LogMsg logMsg in data.LogMsg)
                    {
                        if (logMsg.Record != null)
                        {
                            foreach (double[] entry in logMsg.Record)
                            {
                                if (entry.Length == 4)
                                {
                                    int channel = Convert.ToInt32(entry[1]);
                                    int ioType = Convert.ToInt32(entry[2]);
                                    if (ioType != 7) // AI-Value
                                        continue;
                                    double measureValue = entry[3];

                                    if (measureValue > SensorMinCountValue)
                                    {
                                        ChannelResult channelResult = result.Where(c => c.Channel == channel).FirstOrDefault();
                                        if (channelResult == null)
                                        {
                                            channelResult = new ChannelResult();
                                            channelResult.Channel = channel;
                                            channelResult.Count = 1;
                                            result.Add(channelResult);
                                        }
                                        else
                                        {
                                            channelResult.Count += 1;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Messages.LogException(this.GetACUrl(), "CountData", ex);
                }
            }

            return result;
        }


        public static List<ChannelResult> SimulateCountData()
        {
            return new List<ChannelResult>()
            {
                new ChannelResult(){ Channel = 0, Count = 1 },
                new ChannelResult(){ Channel = 1, Count = 1 },
                new ChannelResult(){ Channel = 2, Count = 1 },
                new ChannelResult(){ Channel = 3, Count = 1 }
            };
        }

        #endregion

        #endregion

        #region Overrides

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(Reset):
                    Reset();
                    return true;
                case nameof(IsEnabledReset):
                    result = IsEnabledReset();
                    return true;
                case nameof(ResetCounter):
                    result = ResetCounter();
                    return true;
                case nameof(IsEnabledResetCounter):
                    result = IsEnabledResetCounter();
                    return true;
                case nameof(Available):
                    Available();
                    return true;
                case nameof(IsEnabledAvailable):
                    result = IsEnabledAvailable();
                    return true;
                case nameof(ReadAvailable):
                    result = ReadAvailable();
                    return true;
                case nameof(Read):
                    Read();
                    return true;
                case nameof(IsEnabledRead):
                    result = IsEnabledRead();
                    return true;
                case nameof(ReadCounter):
                    result = ReadCounter();
                    return true;
                case nameof(IsEnabledReadCounter):
                    result = IsEnabledReadCounter();
                    return true;
                case nameof(IsEnabledGetValues):
                    result = IsEnabledGetValues();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

        #region Private

        private bool CanSend()
        {
            return Client != null
                    && !string.IsNullOrEmpty(Client.ServiceUrl)
                    && !Client.ConnectionDisabled;
        }

        public Msg LogMessage(eMsgLevel level, string translationID, string methodName, int linie, params object[] parameter)
        {
            Msg msg = new Msg(this, level, this.ACType.ACIdentifier, methodName, linie, translationID, parameter);
            IsReadingCounterAlarm.ValueT = PANotifyState.AlarmOrFault;
            ErrorText.ValueT = msg.Message;
            Messages.LogMessageMsg(msg);
            OnNewAlarmOccurred(IsReadingCounterAlarm, msg, true);
            return msg;
        }

        private async Task<WSResponse<TResult>> GetAsyncWise<TResult>(ACRestClient aCRestClient, string url)
        {
            HttpClient client = aCRestClient.Client;
            if (client == null)
            {
                return await Task.FromResult(new WSResponse<TResult>(new Msg(eMsgLevel.Error, "Disconnected")));
            }

            HttpResponseMessage response = await client.GetAsync(new Uri(url, UriKind.Relative));
            if (response.IsSuccessStatusCode)
            {
                aCRestClient.IsConnected.ValueT = true;
                string json = await response.Content.ReadAsStringAsync();
                if (StoreRecivedData && !string.IsNullOrEmpty(ExportDir) && !string.IsNullOrEmpty(FileName) && Directory.Exists(ExportDir))
                {
                    ExportData(ExportDir, FileName, json);
                }
                return new WSResponse<TResult>(await Task.Run(() => JsonConvert.DeserializeObject<TResult>(json)), response.StatusCode);
            }

            return await Task.FromResult(new WSResponse<TResult>(new Msg(eMsgLevel.Failure, $"{response.ReasonPhrase},{response.StatusCode}")));
        }

        #endregion

    }
}
