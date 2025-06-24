using gip.core.autocomponent;
using gip.core.communication;
using gip.core.datamodel;
using gip.core.processapplication;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
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
            _PollingMode = new ACPropertyConfigValue<ushort>(this, nameof(PollingMode), 0);
            _PollingInterval = new ACPropertyConfigValue<int>(this, nameof(PollingInterval), 10000);
            _LimitRegistersPerPage = new ACPropertyConfigValue<int>(this, nameof(LimitRegistersPerPage), 5);
            _PageToRetrieve = new ACPropertyConfigValue<int>(this, nameof(PageToRetrieve), 0);
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            return base.ACInit(startChildMode);
        }

        public override bool ACPostInit()
        {
            _ = PollingMode;
            _ = PollingInterval;
            _ = LimitRegistersPerPage;
            _ = PageToRetrieve;

            bool result = base.ACPostInit();

            if (CurrentWorkingMode != null)
                CurrentWorkingMode.ValueT = -1; // Initialize to -1 (unknown)
            _QueryParams = new Dictionary<string, string>
            {
                { QueryParamLimit, LimitRegistersPerPage.ToString() },
                { QueryParamPage, PageToRetrieve.ToString() }
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
            }

            if (_PollThread != null)
            {
                _PollThread.Join(5000);
                _PollThread = null;
            }

            if (_ShutdownEvent != null)
            {
                _ShutdownEvent.Close();
                _ShutdownEvent = null;
            }

            return base.ACDeInit(deleteACClassTask);
        }

        public const string QueryParamLimit = "limit";
        public const string QueryParamPage = "page";
        public const string QueryParamID = "id";
        public const string ContainerCgiPath = "/containers.cgi";
        public const string TriggerReadCgiPath = "/trigger.cgi";
        public const string DownloadConfigCgiPath = "/alpr.cgi";
        public const string UploadConfigCgiPath = "/alpr.cgi";

        #endregion

        #region Properties

        private Dictionary<string, string> _QueryParams;

        protected ACPropertyConfigValue<int> _PollingInterval;
        [ACPropertyConfig("en{'Polling [ms]'}de{'Abfragezyklus [ms]'}")]
        public int PollingInterval
        {
            get
            {
                return _PollingInterval.ValueT;
            }
            set
            {
                _PollingInterval.ValueT = value;
            }
        }

        protected const ushort C_PollingMode_Off = 0;
        protected const ushort C_PollingMode_On_WithoutPresenceCheck = 1;
        protected const ushort C_PollingMode_On_WithPresenceCheck = 2;

        protected ACPropertyConfigValue<ushort> _PollingMode;
        [ACPropertyConfig("en{'Cyclic polling: 0=Off, 2=With presence check, 1=Without'}de{'Zyklische Abfrage: 0=Aus, 2=Mit Anwesenheitsprüfung, 1=Ohne'}")]
        public ushort PollingMode
        {
            get
            {
                return _PollingMode.ValueT;
            }
            set
            {
                _PollingMode.ValueT = value;
            }
        }

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

        [ACPropertyBindingSource(9999, "Error", "en{'Last retrieved ID'}de{'Last retrieved ID'}", "", true, true)]
        public IACContainerTNet<string> LastRetrievedID
        {
            get;
            set;
        }

        protected const string C_NoContainerPresent = "NONE";
        [ACPropertyBindingSource(9999, "Error", "en{'Last retrieved container code'}de{'Last retrieved container code'}", "", true, true)]
        public IACContainerTNet<string> LastContainerCode
        {
            get;
            set;
        }

        private double _MinConfidence;
        [ACPropertyInfo(true, 500, "en{'Polling [ms]'}de{'Abfragezyklus [ms]'}", DefaultValue = (double)99.0)]
        public double MinConfidence
        {
            get
            {
                if (_MinConfidence <= double.Epsilon)
                    _MinConfidence = 0.0;
                if (_MinConfidence > 100.0)
                    _MinConfidence = 100.0;
                return _MinConfidence;
            }
            set
            {
                _MinConfidence = value;
                if (_MinConfidence <= double.Epsilon)
                    _MinConfidence = 0.0;
                if (_MinConfidence > 100.0)
                    _MinConfidence = 100.0;
                OnPropertyChanged();
            }
        }

        protected ACPropertyConfigValue<int> _LimitRegistersPerPage;
        [ACPropertyConfig("en{'Limit registers per page'}de{'Limit registers per page'}", DefaultValue = 5)]
        public int LimitRegistersPerPage
        {
            get
            {
                return _LimitRegistersPerPage.ValueT;
            }
            set
            {
                _LimitRegistersPerPage.ValueT = value;
            }
        }

        protected ACPropertyConfigValue<int> _PageToRetrieve;
        [ACPropertyConfig("en{'Page to retrieve'}de{'Page to retrieve'}", DefaultValue = 0)]
        public int PageToRetrieve
        {
            get
            {
                return _PageToRetrieve.ValueT;
            }
            set
            {
                _PageToRetrieve.ValueT = value;
            }
        }

        private string _BaseUri;
        public string BaseUri
        {
            get
            {
                if (_BaseUri == null)
                {
                    _BaseUri = Client?.ServiceUrl;
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

        protected const int C_WorkingMode_Unkown = -1;
        protected const int C_WorkingMode_FreeFlow = 0;
        protected const int C_WorkingMode_Signaled = 1;

        [ACPropertyBindingSource(9999, "Config", "en{'Current Working Mode'}de{'Aktueller Arbeitsmodus'}", "",true, false)]
        public IACContainerTNet<int> CurrentWorkingMode
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
                if (PollingMode == C_PollingMode_Off)
                {
                    // Polling is off, just return
                    return;
                }
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
                    Messages.LogException(GetACUrl(), $"{nameof(VaxOCRAlarm)}.{nameof(Poll)}(30)", ex);
                }
                OnNewAlarmOccurred(VaxOCRAlarm, ex.Message, true);
            }
        }

        private void RetrieveDBRecognitions()
        {
            try
            {
                string containerCodeBefore = LastContainerCode.ValueT;
                if (PollingMode == C_PollingMode_On_WithPresenceCheck)
                {
                    int currentMode = this.CurrentWorkingMode.ValueT;
                    if (currentMode == C_WorkingMode_Unkown)
                        currentMode = GetWorkingModeInternal();
                    if (currentMode == C_WorkingMode_Unkown)
                        return;
                    // In this Mode we force the camera to read the current container code until NONE is returned which means, that there is no container present.
                    // This is necessary because in Free Flow mode we do not get any updates when the container is removed.
                    if (   !string.IsNullOrEmpty(containerCodeBefore)
                        && !string.Equals(containerCodeBefore, C_NoContainerPresent, StringComparison.OrdinalIgnoreCase))
                    {
                        if (currentMode != C_WorkingMode_Signaled)
                        {
                            if (this.SetWorkingModeInternal(C_WorkingMode_Signaled))
                                currentMode = this.CurrentWorkingMode.ValueT;
                        }
                        TriggerOCRReadInternal();
                    }
                    // Switch back to Free Flow mode to avoid unnecessary Reads
                    else if (   !string.IsNullOrEmpty(containerCodeBefore)
                             && string.Equals(containerCodeBefore, C_NoContainerPresent, StringComparison.OrdinalIgnoreCase)
                             && currentMode == C_WorkingMode_Signaled)
                    {
                        if (this.SetWorkingModeInternal(C_WorkingMode_FreeFlow))
                            currentMode = this.CurrentWorkingMode.ValueT;
                    }
                    Thread.Sleep(5000); // Wait the camera to process the read
                }

                string uri = GenerateRetrieveURI();

                if (string.IsNullOrEmpty(uri))
                {
                    string msg = "Failed to generate URI for VaxOCR request";
                    VaxOCRAlarm.ValueT = PANotifyState.AlarmOrFault;
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
                    string msg = "ACRestClient not available";
                    VaxOCRAlarm.ValueT = PANotifyState.AlarmOrFault;
                    if (IsAlarmActive(nameof(VaxOCRAlarm), msg) == null)
                    {
                        Messages.LogError(GetACUrl(), $"{nameof(VaxOCRAlarm)}.{nameof(RetrieveDBRecognitions)}(20)", msg);
                    }
                    OnNewAlarmOccurred(VaxOCRAlarm, msg, true);
                    return;
                }

                WSResponse<string> response = client.Get(new Uri(uri, UriKind.Absolute));

                if (response == null || string.IsNullOrEmpty(response.Data))
                {
                    // No data returned, this is normal - no new recognitions
                    return;
                }

                VaxOCRResultSet result = Deserialize(response.Data);

                if (result != null && result.Containers != null && result.Containers.Any())
                {
                    // Update LastRetrievedID to the highest ID from the current batch
                    VaxOCRContainer lastCont = result.Containers
                        .Where(c => !string.IsNullOrEmpty(c.ID))
                        .OrderByDescending(c => c.ID)
                        .FirstOrDefault();

                    bool changed = false;
                    if (lastCont != null)
                    {
                        if (LastRetrievedID != null)
                        {
                            changed = LastRetrievedID.ValueT != lastCont.ID;
                            LastRetrievedID.ValueT = lastCont.ID;
                        }
                        if (LastContainerCode != null)
                        {
                            if (lastCont.ConfidenceValue < MinConfidence)
                                LastContainerCode.ValueT = "*" + lastCont.ContainerCode; // Mark as low confidence
                            else
                                LastContainerCode.ValueT = lastCont.ContainerCode;
                        }
                    }

                    if (changed)
                    {
                        ProcessRecognitions(lastCont, result.Containers);

                        string containerCodeAfter = LastContainerCode.ValueT;
                        // Switch back to Free Flow mode to avoid unnecessary Reads
                        if (   PollingMode == C_PollingMode_On_WithPresenceCheck
                            && string.Equals(containerCodeAfter, C_NoContainerPresent, StringComparison.OrdinalIgnoreCase)
                            && CurrentWorkingMode.ValueT == C_WorkingMode_Signaled)
                        {
                            this.SetWorkingModeInternal(C_WorkingMode_FreeFlow);
                        }
                    }

                    // Clear any previous alarms if we successfully processed data
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
                    Messages.LogException(GetACUrl(), $"{nameof(VaxOCRAlarm)}.{nameof(RetrieveDBRecognitions)}(30)", ex);
                }
                OnNewAlarmOccurred(VaxOCRAlarm, ex.Message, true);
            }
        }

        /// <summary>
        /// Triggers the camera to perform an immediate OCR reading.
        /// This method can be used to check if a container is still present.
        /// If the container has been removed, subsequent calls to RetrieveDBRecognitions 
        /// will return container_code="NONE" and serial_code="NONE".
        /// Note: This only works when the camera is in working_mode="1" (manual mode).
        /// </summary>
        /// <returns>True if the trigger was successful, false otherwise</returns>
        [ACMethodInteraction("", "en{'Trigger OCR Read'}de{'OCR-Lesung auslösen'}", 500, true)]
        public void TriggerOCRRead()
        {
            this.ApplicationManager.ApplicationQueue.Add(() =>
            {
                try
                {
                    if (!TriggerOCRReadInternal())
                    {
                        string msg = "Failed to trigger OCR read";
                        VaxOCRAlarm.ValueT = PANotifyState.AlarmOrFault;
                        if (IsAlarmActive(nameof(VaxOCRAlarm), msg) == null)
                        {
                            Messages.LogError(GetACUrl(), $"{nameof(VaxOCRAlarm)}.{nameof(TriggerOCRRead)}(10)", msg);
                        }
                        OnNewAlarmOccurred(VaxOCRAlarm, msg, true);
                    }
                }
                catch (Exception ex)
                {
                    VaxOCRAlarm.ValueT = PANotifyState.AlarmOrFault;
                    if (IsAlarmActive(nameof(VaxOCRAlarm), ex.Message) == null)
                    {
                        Messages.LogException(GetACUrl(), $"{nameof(VaxOCRAlarm)}.{nameof(TriggerOCRRead)}(20)", ex);
                    }
                    OnNewAlarmOccurred(VaxOCRAlarm, ex.Message, true);
                }
            });
        }

        public bool IsEnabledTriggerOCRRead()
        {
            return CurrentWorkingMode.ValueT == C_WorkingMode_Signaled && Client != null && !string.IsNullOrEmpty(Client.ServiceUrl);
        }

        protected bool TriggerOCRReadInternal()
        {
            try
            {
                // First check if camera is in manual mode (working_mode = 1)
                int currentMode = CurrentWorkingMode.ValueT;
                if (currentMode == C_WorkingMode_Unkown)
                {
                    // If unknown, try to retrieve the current working mode
                    currentMode = GetWorkingModeInternal();
                    if (currentMode == C_WorkingMode_Unkown)
                    {
                        string msg = "Failed to retrieve current working mode from camera";
                        Messages.LogError(GetACUrl(), $"{nameof(TriggerOCRReadInternal)}(01)", msg);
                        return false;
                    }
                }
                if (currentMode != C_WorkingMode_Signaled)
                {
                    string msg = $"Camera must be in sihnaled mode (working_mode=1) to trigger reads. Current mode: {currentMode}";
                    Messages.LogWarning(GetACUrl(), $"{nameof(TriggerOCRReadInternal)}(05)", msg);
                    return false;
                }

                string uri = GenerateTriggerReadURI();

                if (string.IsNullOrEmpty(uri))
                {
                    string msg = "Failed to generate URI for VaxOCR Trigger Read request";
                    Messages.LogError(GetACUrl(), $"{nameof(TriggerOCRReadInternal)}(10)", msg);
                    return false;
                }

                ACRestClient client = Client;

                if (client == null)
                {
                    string msg = "ACRestClient not available for Trigger Read";
                    Messages.LogError(GetACUrl(), $"{nameof(TriggerOCRReadInternal)}(20)", msg);
                    return false;
                }

                WSResponse<string> response = client.Get(new Uri(uri, UriKind.Absolute));

                if (response == null)
                {
                    string msg = "No response received from Trigger Read request";
                    Messages.LogError(GetACUrl(), $"{nameof(TriggerOCRReadInternal)}(30)", msg);
                    return false;
                }

                // Log successful trigger
                Messages.LogInfo(GetACUrl(), $"{nameof(TriggerOCRReadInternal)}(40)", "OCR Read triggered successfully");
                return true;
            }
            catch (Exception ex)
            {
                Messages.LogException(GetACUrl(), $"{nameof(TriggerOCRReadInternal)}(50)", ex);
                return false;
            }
        }

        /// <summary>
        /// Triggers OCR read and then immediately retrieves the latest recognition results.
        /// This is useful for checking container presence status.
        /// </summary>
        /// <returns>The latest recognition result, or null if no data available</returns>
        [ACMethodInteraction("", "en{'Check Container Presence'}de{'Container-Anwesenheit prüfen'}", 600, true)]
        public void CheckContainerPresence()
        {
            this.ApplicationManager.ApplicationQueue.Add(() =>
            {
                try
                {
                    Messages.LogInfo(GetACUrl(), $"{nameof(CheckContainerPresence)}(10)", "Starting container presence check test");

                    VaxOCRResultSet result = CheckContainerPresenceInternal();

                    if (result != null && result.Containers != null && result.Containers.Any())
                    {
                        var containers = result.Containers;
                        Messages.LogInfo(GetACUrl(), $"{nameof(CheckContainerPresence)}(20)",
                            $"Found {containers.Count} container(s) in recognition result");

                        foreach (var container in containers)
                        {
                            string status = (string.Equals(container.ContainerCode, C_NoContainerPresent, StringComparison.OrdinalIgnoreCase) ||
                                           string.Equals(container.SerialCode, C_NoContainerPresent, StringComparison.OrdinalIgnoreCase))
                                           ? "REMOVED" : "PRESENT";

                            Messages.LogInfo(GetACUrl(), $"{nameof(CheckContainerPresence)}(30)",
                                $"Container: {container.ContainerCode}, Serial: {container.SerialCode}, Status: {status}");
                        }
                    }
                    else
                    {
                        Messages.LogInfo(GetACUrl(), $"{nameof(CheckContainerPresence)}(40)", "No container data returned");
                    }
                }
                catch (Exception ex)
                {
                    Messages.LogException(GetACUrl(), $"{nameof(CheckContainerPresence)}(50)", ex);
                }
            });
        }

        protected VaxOCRResultSet CheckContainerPresenceInternal()
        {
            try
            {
                int workingModeBefore = CurrentWorkingMode.ValueT;

                // First trigger the OCR read
                if (!TriggerOCRReadInternal())
                {
                    Messages.LogError(GetACUrl(), $"{nameof(CheckContainerPresenceInternal)}(10)", "Failed to trigger OCR read");
                    return null;
                }

                int workingModeAfter = CurrentWorkingMode.ValueT;

                // Wait a short moment for the camera to process
                Thread.Sleep(500);

                // Now retrieve the latest results
                string uri = GenerateRetrieveURI();

                if (string.IsNullOrEmpty(uri))
                {
                    Messages.LogError(GetACUrl(), $"{nameof(CheckContainerPresenceInternal)}(20)", "Failed to generate retrieve URI");
                    return null;
                }

                ACRestClient client = Client;
                if (client == null)
                {
                    Messages.LogError(GetACUrl(), $"{nameof(CheckContainerPresenceInternal)}(30)", "ACRestClient not available");
                    return null;
                }

                WSResponse<string> response = client.Get(new Uri(uri, UriKind.Absolute));

                if (response == null || string.IsNullOrEmpty(response.Data))
                {
                    Messages.LogInfo(GetACUrl(), $"{nameof(CheckContainerPresenceInternal)}(40)", "No recognition data returned");
                    return null;
                }

                VaxOCRResultSet result = Deserialize(response.Data);

                if (result != null && result.Containers != null && result.Containers.Any())
                {
                    // Check if any container shows NONE values (indicating container removal)
                    //var noneContainers = result.Containers.Where(c =>
                    //    string.Equals(c.ContainerCode, C_NoContainerPresent, StringComparison.OrdinalIgnoreCase) ||
                    //    string.Equals(c.SerialCode, C_NoContainerPresent, StringComparison.OrdinalIgnoreCase)).ToList();

                    //if (noneContainers.Any())
                    //{
                    //    Messages.LogInfo(GetACUrl(), $"{nameof(CheckContainerPresence)}(50)",
                    //        $"Container removal detected: {noneContainers.Count} entries with NONE values");
                    //}

                    VaxOCRContainer lastCont = result.Containers
                                                .Where(c => !string.IsNullOrEmpty(c.ID))
                                                .OrderByDescending(c => c.ID)
                                                .FirstOrDefault();

                    //// Update LastRetrievedID if we have valid containers
                    //VaxOCRContainer lastCont = result.Containers
                    //    .Where(c => !string.IsNullOrEmpty(c.ID))
                    //    .OrderByDescending(c => c.ID)
                    //    .FirstOrDefault();

                    bool changed = false;
                    if (lastCont != null)
                    {
                        if (LastRetrievedID != null)
                        {
                            changed = LastRetrievedID.ValueT != lastCont.ID;
                            LastRetrievedID.ValueT = lastCont.ID;
                        }
                        if (lastCont.ConfidenceValue < MinConfidence)
                            LastContainerCode.ValueT = "*" + lastCont.ContainerCode; // Mark as low confidence
                        else
                            LastContainerCode.ValueT = lastCont.ContainerCode;
                    }

                    if (changed)
                        ProcessRecognitions(lastCont, result.Containers);
                }

                if (workingModeBefore != workingModeAfter && workingModeAfter != C_WorkingMode_Unkown)
                {
                    SetWorkingModeInternal(workingModeBefore);
                }

                return result;
            }
            catch (Exception ex)
            {
                Messages.LogException(GetACUrl(), $"{nameof(CheckContainerPresenceInternal)}(60)", ex);
                return null;
            }
        }

        /// <summary>
        /// Downloads the current configuration from the camera as raw XML string
        /// </summary>
        /// <returns>Configuration XML string or null if failed</returns>
        public string DownloadConfigurationXml()
        {
            try
            {
                string uri = GenerateDownloadConfigURI();

                if (string.IsNullOrEmpty(uri))
                {
                    Messages.LogError(GetACUrl(), $"{nameof(DownloadConfigurationXml)}(10)", "Failed to generate download config URI");
                    return null;
                }

                ACRestClient client = Client;
                if (client == null)
                {
                    Messages.LogError(GetACUrl(), $"{nameof(DownloadConfigurationXml)}(20)", "ACRestClient not available");
                    return null;
                }

                WSResponse<string> response = client.Get(new Uri(uri, UriKind.Absolute));

                if (response == null || string.IsNullOrEmpty(response.Data))
                {
                    Messages.LogError(GetACUrl(), $"{nameof(DownloadConfigurationXml)}(30)", "No configuration data returned");
                    return null;
                }

                // Update current working mode by parsing the XML
                int workingMode = ExtractWorkingModeFromXml(response.Data);
                UpdateWorkingModeProperty(workingMode);

                Messages.LogInfo(GetACUrl(), $"{nameof(DownloadConfigurationXml)}(40)",
                    $"Configuration downloaded successfully. Working mode: {workingMode}");

                return response.Data;
            }
            catch (Exception ex)
            {
                Messages.LogException(GetACUrl(), $"{nameof(DownloadConfigurationXml)}(50)", ex);
                return null;
            }
        }


        /// <summary>
        /// Uploads a configuration XML string to the camera
        /// </summary>
        /// <param name="configXml">The configuration XML to upload</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool UploadConfigurationXml(string configXml)
        {
            try
            {
                if (string.IsNullOrEmpty(configXml))
                {
                    Messages.LogError(GetACUrl(), $"{nameof(UploadConfigurationXml)}(10)", "Configuration XML is null or empty");
                    return false;
                }

                string uri = GenerateUploadConfigURI();

                if (string.IsNullOrEmpty(uri))
                {
                    Messages.LogError(GetACUrl(), $"{nameof(UploadConfigurationXml)}(20)", "Failed to generate upload config URI");
                    return false;
                }

                ACRestClient client = Client;
                if (client == null)
                {
                    Messages.LogError(GetACUrl(), $"{nameof(UploadConfigurationXml)}(30)", "ACRestClient not available");
                    return false;
                }

                StringContent content = new StringContent(configXml, Encoding.UTF8, "application/xml");
                WSResponse<string> response = client.Post(content, new Uri(uri, UriKind.Absolute));

                if (response == null)
                {
                    Messages.LogError(GetACUrl(), $"{nameof(UploadConfigurationXml)}(40)", "No response received from upload config request");
                    return false;
                }

                // Update current working mode by parsing the uploaded XML
                int workingMode = ExtractWorkingModeFromXml(configXml);
                UpdateWorkingModeProperty(workingMode);

                Messages.LogInfo(GetACUrl(), $"{nameof(UploadConfigurationXml)}(50)",
                    $"Configuration uploaded successfully. Working mode set to: {workingMode}");
                return true;
            }
            catch (Exception ex)
            {
                Messages.LogException(GetACUrl(), $"{nameof(UploadConfigurationXml)}(60)", ex);
                return false;
            }
        }

        /// <summary>
        /// Extracts the working_mode value from the configuration XML
        /// </summary>
        /// <param name="configXml">The configuration XML string</param>
        /// <returns>Working mode value (0 or 1) or -1 if not found</returns>
        private int ExtractWorkingModeFromXml(string configXml)
        {
            if (string.IsNullOrEmpty(configXml))
                return C_WorkingMode_Unkown;

            try
            {
                // Use regex to find working_mode attribute in the <mode> element
                // Pattern matches: working_mode="0" or working_mode="1" etc.
                Match match = Regex.Match(configXml, @"working_mode\s*=\s*[""'](\d+)[""']", RegexOptions.IgnoreCase);

                if (match.Success && int.TryParse(match.Groups[1].Value, out int workingMode))
                {
                    return workingMode;
                }

                return C_WorkingMode_Unkown;
            }
            catch (Exception ex)
            {
                Messages.LogException(GetACUrl(), $"{nameof(ExtractWorkingModeFromXml)}(10)", ex);
                return C_WorkingMode_Unkown;
            }
        }

        /// <summary>
        /// Modifies the working_mode value in the configuration XML
        /// </summary>
        /// <param name="configXml">The original configuration XML</param>
        /// <param name="newWorkingMode">The new working mode value (0 or 1)</param>
        /// <returns>Modified XML string or null if failed</returns>
        private string ModifyWorkingModeInXml(string configXml, int newWorkingMode)
        {
            if (string.IsNullOrEmpty(configXml))
                return null;

            try
            {
                // Use regex to replace working_mode attribute value
                // Pattern matches: working_mode="0" or working_mode="1" etc. and replaces with new value
                string modifiedXml = Regex.Replace(configXml, @"(working_mode\s*=\s*[""'])(\d+)([""'])", match => match.Groups[1].Value + newWorkingMode + match.Groups[3].Value, RegexOptions.IgnoreCase);

                // Verify that the replacement actually happened
                if (modifiedXml == configXml)
                {
                    Messages.LogWarning(GetACUrl(), $"{nameof(ModifyWorkingModeInXml)}(10)",
                        "No working_mode attribute found in configuration XML to modify");
                    return null;
                }

                Messages.LogInfo(GetACUrl(), $"{nameof(ModifyWorkingModeInXml)}(20)",
                    $"Working mode modified in XML to: {newWorkingMode}");

                return modifiedXml;
            }
            catch (Exception ex)
            {
                Messages.LogException(GetACUrl(), $"{nameof(ModifyWorkingModeInXml)}(30)", ex);
                return null;
            }
        }

        /// <summary>
        /// Sets the camera working mode
        /// </summary>
        /// <param name="mode">0 = Free flow, 1 = signaled mode</param>
        /// <returns>True if successful, false otherwise</returns>
        [ACMethodInfo("", "en{'Set Working Mode'}de{'Arbeitsmodus setzen'}", 601, true)]
        public void SetWorkingMode(int mode)
        {
            this.ApplicationManager.ApplicationQueue.Add(() =>
            {
                SetWorkingModeInternal(C_WorkingMode_FreeFlow);
            });
        }

        protected bool SetWorkingModeInternal(int mode)
        {
            try
            {
                if (mode != C_WorkingMode_FreeFlow && mode != C_WorkingMode_Signaled)
                {
                    Messages.LogError(GetACUrl(), $"{nameof(SetWorkingModeInternal)}(10)", $"Invalid working mode: {mode}. Must be 0 (Free flow) or 1 (signaled)");
                    return false;
                }

                // Download current configuration
                string configXml = DownloadConfigurationXml();
                if (string.IsNullOrEmpty(configXml))
                {
                    Messages.LogError(GetACUrl(), $"{nameof(SetWorkingModeInternal)}(20)", "Failed to download current configuration");
                    return false;
                }

                // Check if mode is already set
                int currentMode = ExtractWorkingModeFromXml(configXml);
                if (currentMode == mode)
                {
                    Messages.LogInfo(GetACUrl(), $"{nameof(SetWorkingModeInternal)}(30)", $"Working mode is already set to {mode}");
                    return true;
                }

                // Modify working mode in XML
                string modifiedXml = ModifyWorkingModeInXml(configXml, mode);
                if (string.IsNullOrEmpty(modifiedXml))
                {
                    Messages.LogError(GetACUrl(), $"{nameof(SetWorkingModeInternal)}(40)", "Failed to modify working mode in configuration XML");
                    return false;
                }

                // Upload modified configuration
                bool success = UploadConfigurationXml(modifiedXml);

                if (success)
                {
                    string modeText = mode == C_WorkingMode_FreeFlow ? "Free flow" : "Signaled";
                    Messages.LogInfo(GetACUrl(), $"{nameof(SetWorkingModeInternal)}(50)", $"Working mode successfully set to {mode} ({modeText})");
                }

                return success;
            }
            catch (Exception ex)
            {
                Messages.LogException(GetACUrl(), $"{nameof(SetWorkingModeInternal)}(60)", ex);
                return false;
            }
        }

        /// <summary>
        /// Enables Free flow mode (working_mode = 0)
        /// In this mode, the camera automatically detects containers and triggers OCR
        /// </summary>
        [ACMethodInteraction("", "en{'Enable Free flow Mode'}de{'Free flow Modus aktivieren'}", 501, true)]
        public void EnableFreeFlowMode()
        {
            this.ApplicationManager.ApplicationQueue.Add(() =>
            {
                SetWorkingModeInternal(C_WorkingMode_FreeFlow);
            });
        }

        public bool IsEnabledEnableFreeFlowMode()
        {
            return CurrentWorkingMode.ValueT != C_WorkingMode_FreeFlow && Client != null && !string.IsNullOrEmpty(Client.ServiceUrl);
        }

        /// <summary>
        /// Enables signaled mode (working_mode = 1)
        /// In this mode, OCR must be triggered manually via TriggerOCRRead()
        /// </summary>
        [ACMethodInteraction("", "en{'Enable Signaled Mode'}de{'Signaled Modus aktivieren'}", 502, true)]
        public void EnableSignaledMode()
        {
            this.ApplicationManager.ApplicationQueue.Add(() =>
            {
                SetWorkingModeInternal(C_WorkingMode_Signaled);
            });
        }

        public bool IsEnabledEnableSignaledMode()
        {
            return CurrentWorkingMode.ValueT != C_WorkingMode_Signaled && Client != null && !string.IsNullOrEmpty(Client.ServiceUrl);
        }

        /// <summary>
        /// Gets the current working mode from the camera
        /// </summary>
        /// <returns>0 = Free flow, 1 = signaled mode, -1 = Error</returns>
        [ACMethodInfo("", "en{'Get Working Mode'}de{'Arbeitsmodus abrufen'}", 503, true)]
        public void GetWorkingMode()
        {
            this.ApplicationManager.ApplicationQueue.Add(() =>
            {
                int workingMode = GetWorkingModeInternal();
            });
        }

        protected int GetWorkingModeInternal()
        { 
            try
            {
                string configXml = DownloadConfigurationXml();
                int workingMode = ExtractWorkingModeFromXml(configXml);
                UpdateWorkingModeProperty(workingMode);
                return workingMode;
            }
            catch (Exception ex)
            {
                Messages.LogException(GetACUrl(), $"{nameof(GetWorkingModeInternal)}(10)", ex);
                return C_WorkingMode_Unkown;
            }
        }

        protected void UpdateWorkingModeProperty(int mode)
        {
            if (CurrentWorkingMode != null && mode >= 0 && CurrentWorkingMode.ValueT != mode)
            {
                CurrentWorkingMode.ValueT = mode;
            }
        }

        public bool IsEnabledGetWorkingMode()
        {
            return  Client != null && !string.IsNullOrEmpty(Client.ServiceUrl);
        }

        private string GenerateRetrieveURI()
        {
            if (string.IsNullOrEmpty(BaseUri))
                return null;

            try
            {
                // Update query parameters
                _QueryParams[QueryParamLimit] = LimitRegistersPerPage.ToString();
                _QueryParams[QueryParamPage] = PageToRetrieve.ToString();

                // Set ID parameter - use 0 if LastRetrievedID is null or empty
                string lastId = "0";
                if (LastRetrievedID != null && !string.IsNullOrEmpty(LastRetrievedID.ValueT))
                {
                    lastId = LastRetrievedID.ValueT;
                }
                _QueryParams[QueryParamID] = lastId;

                // Build the complete URI
                UriBuilder uriBuilder = new UriBuilder(BaseUri.TrimEnd('/') + ContainerCgiPath);

                // Both variants are possible:
                //http://192.168.1.116/local/Vaxcontainer/containers.cgi
                //http://192.168.1.116/local/Vaxcontainer/containers.cgi?page=0&limit=5

                // Build query string manually to ensure proper encoding
                var queryParts = _QueryParams.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value ?? "")}");
                uriBuilder.Query = string.Join("&", queryParts);

                return uriBuilder.Uri.ToString();
            }
            catch (Exception ex)
            {
                Messages.LogException(GetACUrl(), $"{nameof(GenerateRetrieveURI)}(10)", ex);
                return null;
            }
        }

        private string GenerateTriggerReadURI()
        {
            if (string.IsNullOrEmpty(BaseUri))
                return null;

            try
            {
                // Build the complete URI for Trigger Read
                UriBuilder uriBuilder = new UriBuilder(BaseUri.TrimEnd('/') + TriggerReadCgiPath);
                
                //https://192.168.1.116/local/Vaxcontainer/trigger.cgi
                //https://192.168.1.116/local/Vaxcontainer/trigger.cgi?id=iPlus0000003
                
                return uriBuilder.Uri.ToString();
            }
            catch (Exception ex)
            {
                Messages.LogException(GetACUrl(), $"{nameof(GenerateTriggerReadURI)}(10)", ex);
                return null;
            }
        }

        private string GenerateDownloadConfigURI()
        {
            if (string.IsNullOrEmpty(BaseUri))
                return null;

            try
            {
                //https://192.168.1.116/local/Vaxcontainer/alpr.cgi
                UriBuilder uriBuilder = new UriBuilder(BaseUri.TrimEnd('/') + DownloadConfigCgiPath);
                return uriBuilder.Uri.ToString();
            }
            catch (Exception ex)
            {
                Messages.LogException(GetACUrl(), $"{nameof(GenerateDownloadConfigURI)}(10)", ex);
                return null;
            }
        }

        private string GenerateUploadConfigURI()
        {
            if (string.IsNullOrEmpty(BaseUri))
                return null;

            try
            {
                //https://192.168.1.116/local/Vaxcontainer/alpr.cgi
                UriBuilder uriBuilder = new UriBuilder(BaseUri.TrimEnd('/') + UploadConfigCgiPath);
                return uriBuilder.Uri.ToString();
            }
            catch (Exception ex)
            {
                Messages.LogException(GetACUrl(), $"{nameof(GenerateUploadConfigURI)}(10)", ex);
                return null;
            }
        }

        protected virtual VaxOCRResultSet Deserialize(string content)
        {
            if (string.IsNullOrEmpty(content))
                return null;

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(VaxOCRResultSet));
                using (StringReader reader = new StringReader(content))
                {
                    return (VaxOCRResultSet)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                Messages.LogException(GetACUrl(), $"{nameof(Deserialize)}(10)", ex);
                return null;
            }
        }

        public virtual void ProcessRecognitions(VaxOCRContainer lastContainer, List<VaxOCRContainer> containers)
        {
            if (lastContainer == null || lastContainer.ConfidenceValue < MinConfidence)
            {
                return;
            }
            PAEScannerDecoder scannerDecoder = FindChildComponents<PAEScannerDecoder>(c => c is PAEScannerDecoder).FirstOrDefault() as PAEScannerDecoder;
            if (scannerDecoder != null)
            {
                if (!string.IsNullOrEmpty(lastContainer.ContainerCode))
                {
                    string codeForDecoder = lastContainer.ContainerCode;
                    // Check if this is a "NONE" result indicating container removal
                    if (string.Equals(codeForDecoder, C_NoContainerPresent, StringComparison.OrdinalIgnoreCase))
                    {
                        Messages.LogInfo(GetACUrl(), $"{nameof(ProcessRecognitions)}(10)",
                            "Container removal detected - ContainerCode is NONE");
                        // You might want to trigger a specific event or action here
                        // For now, we'll still pass it to the scanner decoder
                        //codeForDecoder = string.Empty; // Clear the code to indicate removal
                    }

                    scannerDecoder.OnScan(codeForDecoder);
                }
            }
        }

        [ACMethodCommand("", "", 602, true)]
        public void TestProcessRecognitions()
        {
            PAEScannerDecoder scannerDecoder = FindChildComponents<PAEScannerDecoder>(c => c is PAEScannerDecoder).FirstOrDefault() as PAEScannerDecoder;
            if (scannerDecoder != null && !string.IsNullOrEmpty(TestCode))
            {
                scannerDecoder.OnScan(TestCode);
            }
        }

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(TriggerOCRRead):
                    TriggerOCRRead();
                    return true;
                case nameof(IsEnabledTriggerOCRRead):
                    result = IsEnabledTriggerOCRRead();
                    return true;
                case nameof(CheckContainerPresence):
                    CheckContainerPresence();
                    return true;
                case nameof(DownloadConfigurationXml):
                    result = DownloadConfigurationXml();
                    return true;
                case nameof(SetWorkingMode):
                    SetWorkingMode((int)acParameter[0]);
                    return true;
                case nameof(EnableFreeFlowMode):
                    EnableFreeFlowMode();
                    return true;
                case nameof(IsEnabledEnableFreeFlowMode):
                    result = IsEnabledEnableFreeFlowMode();
                    return true;
                case nameof(EnableSignaledMode):
                    EnableSignaledMode();
                    return true;
                case nameof(IsEnabledEnableSignaledMode):
                    result = IsEnabledEnableSignaledMode();
                    return true;
                case nameof(GetWorkingMode):
                    GetWorkingMode();
                    return true;
                case nameof(IsEnabledGetWorkingMode):
                    result = IsEnabledGetWorkingMode();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion
    }
}