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
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

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
                // [Error50573] ACRestClient not available!
                LogMessage(eMsgLevel.Error, "Error50573", nameof(ACInit), 56, null);
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

        private bool? _IsResetCounterSuccessfully;

        [ACPropertyInfo(true, 205, "", "en{'Reset successfully'}de{'Zurücksetzen erfolgreich'}", "", false)]
        public bool? IsResetCounterSuccessfully
        {
            get
            {
                return _IsResetCounterSuccessfully;
            }
            set
            {
                if (_IsResetCounterSuccessfully != value)
                {
                    _IsResetCounterSuccessfully = value;
                    OnPropertyChanged();
                }
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


        [ACMethodInteraction("", "en{'Reset counter'}de{'Zähler zurücksetzen'}", 200, true)]
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

            if (!IsEnabledResetCounter())
            {
                // [Error50573] ACRestClient not available!
                LogMessage(eMsgLevel.Error, "Error50573", nameof(ACInit), 296, null);
                return false;
            }

            try
            {
                IsResetCounterSuccessfully = null;
                FilterClear filter = new FilterClear();
                string requestJson = JsonConvert.SerializeObject(filter, DefaultJsonSerializerSettings);
                using (var content = new StringContent(requestJson, Encoding.UTF8, "application/json"))
                {
                    WSResponse<string> response = this.Client.Patch<string>(content, LogClearUrl);

                    if (response.Suceeded)
                    {
                        success = true;
                    }
                    else
                    {
                        // Error50574
                        // Error by resetting counter! Error {0}.
                        LogMessage(eMsgLevel.Error, "Error50574", nameof(ACInit), 317, response.Message?.Message);
                    }
                }
                IsResetCounterSuccessfully = success;
            }
            catch (Exception ec)
            {
                LogMessage(eMsgLevel.Exception, "Error50574", nameof(ACInit), 324, ec.Message);
            }

            return success;
        }

        public bool IsEnabledResetCounter()
        {
            return CanSend();
        }

        #endregion

        #region Methods -> ACMethod -> Available

        [ACMethodInteraction("", "en{'Available'}de{'Verfügare'}", 210, true)]
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

        [ACMethodInfo("", "en{'Available'}de{'Verfügare'}", 211, true)]
        public long ReadAvailable()
        {
            long result = 0;
            ErrorText.ValueT = null;
            MeasureText.ValueT = null;

            if (!IsEnableReadAvailable())
                return 0;

            try
            {
                WSResponse<long?> amountResult = GetAmount(LogOutputUrl);
                if (amountResult.Suceeded)
                {
                    result = amountResult.Data ?? 0;
                    MeasureText.ValueT = result.ToString();
                }
                else
                {
                    // Error50575
                    // rror by reading counter! Error {0}.
                    LogMessage(eMsgLevel.Error, "Error50575", nameof(ACInit), 374, amountResult.Message?.Message);
                }
            }
            catch (Exception ec)
            {
                LogMessage(eMsgLevel.Exception, "Error50575", nameof(ACInit), 379, ec.Message);
            }

            return result;
        }

        public bool IsEnableReadAvailable()
        {
            return CanSend();
        }

        #endregion

        #region Methods -> ACMethod -> Read


        [ACMethodInteraction("", "en{'Count'}de{'Zählen'}", 220, true)]
        public void Read()
        {
            IsResetCounterSuccessfully = true;
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
            List<ChannelResult> result = null;

            if (!IsEnabledReadCounter())
            {
                // [Error50573] ACRestClient not available!
                LogMessage(eMsgLevel.Error, "Error50573", nameof(ACInit), 419, null);
                return result;
            }


            try
            {

                WSResponse<Wise4000Data> dataResult = GetData(LogOutputUrl, LogMessageUrl);
                if (dataResult.Data != null && (dataResult.Message == null || dataResult.Message.MessageLevel < eMsgLevel.Failure))
                {
                    result = CountData(dataResult.Data);
                    if (StoreRecivedData && !string.IsNullOrEmpty(ExportDir) && !string.IsNullOrEmpty(FileName) && Directory.Exists(ExportDir))
                    {
                        ExportData(ExportDir, FileName, dataResult.Data);
                    }

                    string json = JsonConvert.SerializeObject(result);
                    MeasureText.ValueT = json;
                }
                else
                {
                    // Error50575
                    // rror by reading counter! Error {0}.
                    LogMessage(eMsgLevel.Error, "Error50575", nameof(ACInit), 443, dataResult.Message?.Message);
                }

                IsResetCounterSuccessfully = null;
            }
            catch (Exception ec)
            {
                LogMessage(eMsgLevel.Exception, "Error50575", nameof(ACInit), 450, ec.Message);
            }


            return result;
        }

        public bool IsEnabledReadCounter()
        {
            return CanSend() && IsResetCounterSuccessfully != null && IsResetCounterSuccessfully.Value;
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

        public void ExportData(string exportDir, string fileName, Wise4000Data data)
        {
            try
            {
                string file = string.Format(fileName, DateTime.Now);
                string fullFileName = Path.Combine(exportDir, file);
                string json = JsonConvert.SerializeObject(data);
                File.WriteAllText(fullFileName, json);
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
                        WSResponse<Wise4000Data> dataResponse = Client.Get<Wise4000Data>(logMessageUrl);
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
                foreach (LogMsg logMsg in data.LogMsg)
                {
                    if (logMsg.Record != null)
                    {
                        foreach (int[] entry in logMsg.Record)
                        {
                            if (entry.Length == 4)
                            {
                                int channel = entry[1];
                                int measureValue = entry[3];

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

            return result;
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

        #endregion

    }
}
