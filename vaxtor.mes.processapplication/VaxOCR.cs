using gip.core.autocomponent;
using gip.core.communication;
using gip.core.datamodel;
using gip.core.processapplication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace vaxtor.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioSystem, "en{'VaxOCR'}de{'VaxOCR'}", Global.ACKinds.TPABGModule, Global.ACStorableTypes.Required, false, false)]
    public class VaxOCR : PAECameraOCR
    {
        #region c'tors

        public VaxOCR(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") :
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            return base.ACInit(startChildMode);
        }

        public override bool ACPostInit()
        {
            bool result = base.ACPostInit();

            _QueryParams = new Dictionary<string, string>
            {
                { QueryParamLimit, LimitRegistersPerPage.ToString() },
                { QueryParamPage, PageToRetrieve.ToString() },
                { QueryParamID, LastRetrievedID?.ValueT ?? "0"}
            };

            _ShutdownEvent = new ManualResetEvent(false);
            _PollThread = new ACThread(Poll);
            _PollThread.Name = "ACUrl:" + this.GetACUrl() + ";Poll();";
            _PollThread.Start();

            return result;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (_ShutdownEvent != null)
            {
                _ShutdownEvent.Set();
                if (_PollThread != null)
                {
                    _PollThread.Join(5000);
                    _PollThread = null;
                }
                _ShutdownEvent.Close();
                _ShutdownEvent = null;
            }

            if (_Client != null)
            {
                _Client.Detach();
                _Client = null;
            }

            return base.ACDeInit(deleteACClassTask);
        }

        public const string QueryParamLimit = "limit";
        public const string QueryParamPage = "page";
        public const string QueryParamID = "id";

        #endregion

        #region Properties

        private Dictionary<string, string> _QueryParams;

        [ACPropertyInfo(true, 400, "Config", "en{'Polling [ms]'}de{'Abfragezyklus [ms]'}", DefaultValue = 200)]
        public int PollingInterval { get; set; }

        [ACPropertyBindingSource(9999, "Error", "en{'VaxOCR alarm'}de{'VaxOCR Alarm'}", "", false, false)]
        public IACContainerTNet<PANotifyState> VaxOCRAlarm { get; set; }

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
                    //ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                    _Client = new ACRef<ACRestClient>(client, this);
                    return _Client.ValueT;
                }
                return null;
            }
        }

        [ACPropertyBindingSource(9999, "Error", "en{'Last retrieved container ID'}de{'Last retrieved container ID'}", "", true, true)]
        public IACContainerTNet<string> LastRetrievedID
        {
            get;
            set;
        }

        [ACPropertyInfo(true, 9999, "Config", "en{'Limit registers per page'}de{'Limit registers per page'}", DefaultValue = 5)]
        public int LimitRegistersPerPage
        {
            get;
            set;
        }

        [ACPropertyInfo(true, 9999, "Config", "en{'Page to retrieve'}de{'Page to retrieve'}", DefaultValue = 1)]
        public int PageToRetrieve
        {
            get;
            set;
        }

        private string _BaseUri;
        public string BaseUri
        {
            get
            {
                if (_BaseUri == null)
                {
                    _BaseUri = Client?.ServiceUrl?.TrimEnd('/');
                    if (!string.IsNullOrEmpty(_BaseUri) && !_BaseUri.EndsWith("/container.cgi"))
                    {
                        _BaseUri = _BaseUri + "/container.cgi";
                    }
                }
                return _BaseUri;
            }
        }

        [ACPropertyInfo(9999)]
        public string TestCode
        {
            get;
            set;
        }

        #endregion


        #region Methods

        protected ManualResetEvent _ShutdownEvent;
        ACThread _PollThread;
        private void Poll()
        {
            try
            {
                while (!_ShutdownEvent.WaitOne(PollingInterval, false))
                {
                    _PollThread.StartReportingExeTime();

                    RetrieveDBRecognitions();

                    _PollThread.StopReportingExeTime();
                }
            }
            catch (ThreadAbortException ec)
            {
                VaxOCRAlarm.ValueT = PANotifyState.AlarmOrFault;
                if (IsAlarmActive(nameof(VaxOCRAlarm), ec.Message) == null)
                {
                    Messages.LogException(GetACUrl(), $"{nameof(VaxOCRAlarm)}.{nameof(Poll)}(20)", ec);
                }
                OnNewAlarmOccurred(VaxOCRAlarm, ec.Message, true);
            }
            catch (Exception ex)
            {
                VaxOCRAlarm.ValueT = PANotifyState.AlarmOrFault;
                if (IsAlarmActive(nameof(VaxOCRAlarm), ex.Message) == null)
                {
                    Messages.LogException(GetACUrl(), $"{nameof(VaxOCRAlarm)}.{nameof(Poll)}(21)", ex);
                }
                OnNewAlarmOccurred(VaxOCRAlarm, ex.Message, true);
            }
        }

        private void RetrieveDBRecognitions()
        {
            string uri = GenerateURI();

            if (string.IsNullOrEmpty(uri))
            {
                VaxOCRAlarm.ValueT = PANotifyState.AlarmOrFault;
                string msg = "Unable to generate URI for VaxOCR request";
                if (IsAlarmActive(nameof(VaxOCRAlarm), msg) == null)
                {
                    Messages.LogError(GetACUrl(), $"{nameof(VaxOCRAlarm)}.{nameof(RetrieveDBRecognitions)}(10)", msg);
                }
                OnNewAlarmOccurred(VaxOCRAlarm, msg, true);
                return;
            }

            ACRestClient client = Client;

            if (client == null)
            {
                VaxOCRAlarm.ValueT = PANotifyState.AlarmOrFault;
                string msg = "ACRestClient not available for VaxOCR";
                if (IsAlarmActive(nameof(VaxOCRAlarm), msg) == null)
                {
                    Messages.LogError(GetACUrl(), $"{nameof(VaxOCRAlarm)}.{nameof(RetrieveDBRecognitions)}(11)", msg);
                }
                OnNewAlarmOccurred(VaxOCRAlarm, msg, true);
                return;
            }

            try
            {
                WSResponse<string> response = client.Get(uri);

                if (response == null || string.IsNullOrEmpty(response.Data))
                {
                    // No new data, this is normal - don't create alarm
                    return;
                }

                ResultSet result = Deserialize(response.Data);

                if (result != null && result.Containers != null && result.Containers.Any())
                {
                    // Update LastRetrievedID to the highest container ID
                    Container lastCont = result.Containers.OrderByDescending(c => c.ContainerID).FirstOrDefault();
                    if (lastCont != null && !string.IsNullOrEmpty(lastCont.ContainerID))
                    {
                        if (LastRetrievedID != null)
                        {
                            LastRetrievedID.ValueT = lastCont.ContainerID;
                        }
                    }

                    ProcessRecognitions(result.Containers);

                    // Clear any previous alarms when successful
                    if (VaxOCRAlarm.ValueT == PANotifyState.AlarmOrFault)
                    {
                        VaxOCRAlarm.ValueT = PANotifyState.Off;
                        AcknowledgeAlarms();
                    }
                }
            }
            catch (Exception ex)
            {
                VaxOCRAlarm.ValueT = PANotifyState.AlarmOrFault;
                if (IsAlarmActive(nameof(VaxOCRAlarm), ex.Message) == null)
                {
                    Messages.LogException(GetACUrl(), $"{nameof(VaxOCRAlarm)}.{nameof(RetrieveDBRecognitions)}(12)", ex);
                }
                OnNewAlarmOccurred(VaxOCRAlarm, ex.Message, true);
            }
        }

        private string GenerateURI()
        {
            if (string.IsNullOrEmpty(BaseUri))
                return null;

            // Update query parameters with current values
            _QueryParams[QueryParamLimit] = LimitRegistersPerPage.ToString();
            _QueryParams[QueryParamPage] = PageToRetrieve.ToString();
            _QueryParams[QueryParamID] = LastRetrievedID?.ValueT ?? "0";

            try
            {
                UriBuilder uriBuilder = new UriBuilder(BaseUri);

                // Build query string manually to ensure proper encoding
                var queryPairs = new List<string>();
                foreach (var param in _QueryParams)
                {
                    if (!string.IsNullOrEmpty(param.Value))
                    {
                        queryPairs.Add($"{Uri.EscapeDataString(param.Key)}={Uri.EscapeDataString(param.Value)}");
                    }
                }

                uriBuilder.Query = string.Join("&", queryPairs);
                return uriBuilder.Uri.ToString();
            }
            catch (Exception ex)
            {
                Messages.LogException(GetACUrl(), $"{nameof(GenerateURI)}(13)", ex);
                return null;
            }
        }

        protected virtual ResultSet Deserialize(string content)
        {
            if (string.IsNullOrEmpty(content))
                return null;

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ResultSet));
                using (StringReader reader = new StringReader(content))
                {
                    ResultSet result = (ResultSet)serializer.Deserialize(reader);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Messages.LogException(GetACUrl(), $"{nameof(Deserialize)}(14)", ex);
                return null;
            }
        }

        public virtual void ProcessRecognitions(List<Container> containers)
        {
            if (containers == null || !containers.Any())
                return;

            PAEScannerDecoder scannerDecoder = FindChildComponents<PAEScannerDecoder>(c => c is PAEScannerDecoder).FirstOrDefault() as PAEScannerDecoder;
            if (scannerDecoder != null)
            {
                // Process all new containers, but send the most recent one
                Container container = containers.OrderByDescending(c => c.ContainerID).FirstOrDefault();
                if (container != null && !string.IsNullOrEmpty(container.ContainerCode))
                {
                    scannerDecoder.OnScan(container.ContainerCode);
                }
            }
        }

        [ACMethodInteraction("", "", 9999, true)]
        public void TestProcessRecognitions()
        {
            PAEScannerDecoder scannerDecoder = FindChildComponents<PAEScannerDecoder>(c => c is PAEScannerDecoder).FirstOrDefault() as PAEScannerDecoder;
            if (scannerDecoder != null && !string.IsNullOrEmpty(TestCode))
            {
                scannerDecoder.OnScan(TestCode);
            }
        }

        #endregion
    }
}