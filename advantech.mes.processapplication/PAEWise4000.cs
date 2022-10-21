using gip.core.autocomponent;
using gip.core.communication;
using gip.core.datamodel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Xml;

namespace advantech.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'PAEWise4000'}de{'PAEWise4000'}", Global.ACKinds.TPAModule, Global.ACStorableTypes.Required, false, true)]
    public class PAEWise4000 : PAEWiseBase
    {

        #region ctor's

        public PAEWise4000(gip.core.datamodel.ACClass acType, gip.core.datamodel.IACObject content,
            gip.core.datamodel.IACObject parentACObject, gip.core.datamodel.ACValueList parameter, string acIdentifier = "") : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _StoreRecivedData = new ACPropertyConfigValue<bool>(this, "StoreRecivedData", false);
            _ExportDir = new ACPropertyConfigValue<string>(this, "ExportDir", null);
            _FileName = new ACPropertyConfigValue<string>(this, "FileName", "advantec_{0:yyyyMMddHHmmssfff}");
            _SensorMinCountValue = new ACPropertyConfigValue<int>(this, "SensorMinCountValue", 30000);

        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            _ = StoreRecivedData;
            _ = _ExportDir;
            _ = _FileName;
            return base.ACInit(startChildMode);
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

        #endregion

        #region Properties

        #region HTTP-Const

        private const string C_log_output = "log_output";
        private const string C_log_message = "log_message";

        #endregion

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

        #endregion

        #region Methods



        [ACMethodInteraction("", "en{'Reset counter'}de{'Zähler zurücksetzen'}", 202, true)]
        public WSResponse<bool> ResetCounter()
        {
            if (!IsEnabledResetCounter())
                return null;
            bool success = false;



            IsResetCounterSuccessfully = success;
            //WSResponse<string> response = this.Client.Get(C_StopOrderString);
            //if (!response.Suceeded)
            //    return false;
            //IsRecording.ValueT = false;
            //PWSampleNode = null;
            return new WSResponse<bool> { Data = true};
        }


        public bool IsEnabledResetCounter()
        {
            return CanSend();
        }

        [ACMethodInteraction("", "en{'Count'}de{'Zählen'}", 203, true)]
        public WSResponse<int> ReadCounter()
        {
            WSResponse<int> result = new WSResponse<int>();
            if (!IsEnabledReadCounter())
                return result;

            WSResponse<Wise4000Data> dataResult = GetData();
            if(dataResult.Data != null && (dataResult.Message == null || dataResult.Message.MessageLevel < eMsgLevel.Failure))
            {
                result.Data = CountData(dataResult.Data);
            }
            else
            {
                result.Message = dataResult.Message;
            }
            
            IsResetCounterSuccessfully = null;
            return result;
        }


        public bool IsEnabledReadCounter()
        {

            return CanSend() && IsResetCounterSuccessfully != null && IsResetCounterSuccessfully.Value;
        }



        #endregion

        #region Common methods

        public WSResponse<long?> GetAmount()
        {
            WSResponse<long?> result = new WSResponse<long?>();
            Filter filter = new Filter();
            string requestJson = JsonConvert.SerializeObject(filter, DefaultJsonSerializerSettings);
            using (var content = new StringContent(requestJson, Encoding.UTF8, "application/json"))
            {
                WSResponse<string> setFilterResponse = Client.Patch(content, C_log_output);

                if (setFilterResponse.Suceeded)
                {
                    WSResponse<Filter> responseFilter = Client.Get<Filter>(C_log_output);
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

        public WSResponse<Wise4000Data> GetData()
        {
            WSResponse<Wise4000Data> result = new WSResponse<Wise4000Data>();
            WSResponse<long?> amountResult = GetAmount();
            if (amountResult.Data != null && amountResult.Data > 0)
            {
                Filter filter = new Filter();
                filter.FltrEnum = FltrEnum.AmountFilter;
                filter.Amt = amountResult.Data.Value;
                string requestJson = JsonConvert.SerializeObject(filter, DefaultJsonSerializerSettings);
                using (var content = new StringContent(requestJson, Encoding.UTF8, "application/json"))
                {
                    WSResponse<string> setFilterResponse = Client.Patch(content, C_log_output);
                    if (setFilterResponse.Suceeded)
                    {
                        WSResponse<Wise4000Data> dataResponse = Client.Get<Wise4000Data>(C_log_message);
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
            else
            {
                result.Message = amountResult.Message;
            }
            return result;
        }

        public int CountData(Wise4000Data data)
        {
            int count = 0;
            if (data.LogMsg != null)
            {
                foreach (LogMsg logMsg in data.LogMsg)
                {
                    if (logMsg.Record != null)
                    {
                        foreach (int[] entry in logMsg.Record)
                        {
                            foreach (int subEntry in entry)
                            {
                                if (subEntry > SensorMinCountValue)
                                {
                                    count++;
                                }
                            }
                        }
                    }
                }
            }

            return count;
        }

        public bool IsEnabledGetValues()
        {
            if (!CanSend())
                return false;
            return true;
        }

        #endregion


        #region Private methods

        private bool CanSend()
        {
            return Client != null
                    && !string.IsNullOrEmpty(Client.ServiceUrl)
                    && !Client.ConnectionDisabled;
        }


        
        #endregion


    }
}
