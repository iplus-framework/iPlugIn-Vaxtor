using System;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.core.processapplication;
using System.Runtime.Serialization;
using System.Xml;

namespace stech.mes.processapplication
{
    [DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'BDS-II Command'}de{'BDS-II Kommando'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    [ACPropertyEntity(100, "Bit00", "en{'Start sampling (Bit00)'}de{'Messung Start (Bit00)'}")]
    [ACPropertyEntity(101, "Bit01", "en{'External dump (Bit01)'}de{'Probeausgabe extern (Bit01)'}")]
    [ACPropertyEntity(102, "Bit02", "en{'New batch (Bit02)'}de{'Neuer Batch (Bit02)'}")]
    [ACPropertyEntity(106, "Bit06", "en{'Reset/acknowledge error (Bit06)'}de{'Reset/Fehlerquittung (Bit06)'}")]
    [ACPropertyEntity(107, "Bit07", "en{'Start-up BDS-II (machine/analyzer) (Bit07)'}de{'Neustart BDS-II (Maschine/Analyzer) (Bit07)'}")]
    public class PABulkDensityBDS2STCmd : BitAccessForInt16
    {
        #region c'tors
        public PABulkDensityBDS2STCmd()
        {
        }

        public PABulkDensityBDS2STCmd(IACType acValueType)
            : base(acValueType)
        {
        }
        #endregion

        #region Customized Bits

        /// <summary>
        /// 1 = Start/enable sampling 
        /// 0 = BDS-II blocked for sampling
        /// </summary>
        public bool Bit00_StartSampling
        {
            get { return Bit00; }
            set { Bit00 = value; }
        }

        /// <summary>
        /// 1 = Dump/release sample at position "External sample" 
        /// 0 = In-line dump (in the product stream)
        /// </summary>
        public bool Bit01_ExternalDump
        {
            get { return Bit01; }
            set { Bit01 = value; }
        }

        /// <summary>
        /// 1 = New batch (Activate the Start-up period with continuous sampling, and reset the Sample counter) 
        /// 0 = Same batch
        /// </summary>
        public bool Bit02_NewBatch
        {
            get { return Bit02; }
            set { Bit02 = value; }
        }

        /// <summary>
        /// 0->1 = (Rising edge) Reset/acknowledge errors 
        /// 0 = No function
        /// </summary>
        public bool Bit06_ResetACK
        {
            get { return Bit06; }
            set { Bit06 = value; }
        }

        /// <summary>
        /// 0->1 = (Rising edge) Start-up the BDS-II (machine/analyzer) (To be used after a power interruption etc.) 
        /// 0 = No function
        /// </summary>
        public bool Bit07_StartUpBDS
        {
            get { return Bit07; }
            set { Bit07 = value; }
        }
        #endregion
    }


    [DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'BDS-II Status'}de{'BDS-II Status'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    [ACPropertyEntity(100, "Bit00", "en{'Watch Dog (Bit00)'}de{'Watch Dog (Bit00)'}")]
    [ACPropertyEntity(101, "Bit01", "en{'Error (Bit01)'}de{'Fehler (Bit01)'}")]
    [ACPropertyEntity(102, "Bit02", "en{'Ready (Bit02)'}de{'Bereit (Bit02)'}")]
    [ACPropertyEntity(103, "Bit03", "en{'Sampling active (Bit03)'}de{'Messung Aktiv (Bit03)'}")]
    [ACPropertyEntity(104, "Bit04", "en{'Local mode (Bit04)'}de{'Lokaler Modus (Bit04)'}")]
    [ACPropertyEntity(105, "Bit05", "en{'Manual mode (Bit05)'}de{'Handbetrieb (Bit05)'}")]
    [ACPropertyEntity(106, "Bit06", "en{'External dump (Bit06)'}de{'Probeausgabe extern (Bit06)'}")]
    public class PABulkDensityBDS2STStatus : BitAccessForInt16
    {
        #region c'tors
        public PABulkDensityBDS2STStatus()
        {
        }

        public PABulkDensityBDS2STStatus(IACType acValueType)
            : base(acValueType)
        {
        }
        #endregion

        #region Customized Bits

        /// <summary>
        /// Watch dog Toggles between true and false each second (0.5 Hz)
        /// </summary>
        public bool Bit00_WatchDog
        {
            get { return Bit00; }
            set { Bit00 = value; }
        }

        /// <summary>
        /// 1 = No function/machine errors (Ok) 
        /// 0 = Error present, and BDS-II stopped/blocked
        /// </summary>
        public bool Bit01_NoErrors
        {
            get { return Bit01; }
            set { Bit01 = value; }
        }

        /// <summary>
        /// 1 = BDS-II ready/released for sampling 
        /// 0 = BDS-II not ready (local mode, error, etc.)
        /// </summary>
        public bool Bit02_Ready
        {
            get { return Bit02; }
            set { Bit02 = value; }
        }

        /// <summary>
        /// 1 = Sample sequence pending 
        /// 0 = No activity (idle)
        /// </summary>
        public bool Bit03_SamplingActive
        {
            get { return Bit03; }
            set { Bit03 = value; }
        }

        /// <summary>
        /// 1 = Local mode (Host control blocked) 
        /// 0 = Remote/Host control enabled
        /// </summary>
        public bool Bit04_LocalMode
        {
            get { return Bit04; }
            set { Bit04 = value; }
        }

        /// <summary>
        /// 1 = Manual mode 
        /// 0 = Auto mode
        /// </summary>
        public bool Bit05_ManualMode
        {
            get { return Bit05; }
            set { Bit05 = value; }
        }

        /// <summary>
        /// 1 = Pending sample will be dumped/released at position "External sample" 
        /// 0 = Pending sample will be dumped Inline (in the product stream)
        /// </summary>
        public bool Bit06_ExternalDump
        {
            get { return Bit06; }
            set { Bit06 = value; }
        }
        #endregion
    }


    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Image/Vision result'}de{'Bildergebnis'}", Global.ACKinds.TACEnum)]
    public enum PABulkDensityBDS2STImage : short
    {
        None = 0,
        Done = 1,
        Error = 2,
    }


    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Bulk Density BDS-II System Source Technology'}de{'Schüttgewichtsmessgerät BDS-II Source Technology'}", Global.ACKinds.TPAModule, Global.ACStorableTypes.Required, false, true)]
    public class PABulkDensityBDS2ST : PAESensorAnalog
    {
        #region c'tors
        public PABulkDensityBDS2ST(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            (DensityValue as IACPropertyNetServer).ValueUpdatedOnReceival += PABulkDensityBDS2ST_ValueUpdatedOnReceival;
            (ProductFlow as IACPropertyNetServer).ValueUpdatedOnReceival += PABulkDensityBDS2ST_ValueUpdatedOnReceival;
            (BDS2Status as IACPropertyNetServer).ValueUpdatedOnReceival += PABulkDensityBDS2ST_ValueUpdatedOnReceival;
            (SampleID as IACPropertyNetServer).ValueUpdatedOnReceival += PABulkDensityBDS2ST_ValueUpdatedOnReceival;
            (DensityCorrFact as IACPropertyNetServer).ValueUpdatedOnReceival += PABulkDensityBDS2ST_ValueUpdatedOnReceival;
            return true;
        }



        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            (DensityValue as IACPropertyNetServer).ValueUpdatedOnReceival -= PABulkDensityBDS2ST_ValueUpdatedOnReceival;
            (ProductFlow as IACPropertyNetServer).ValueUpdatedOnReceival -= PABulkDensityBDS2ST_ValueUpdatedOnReceival;
            (BDS2Status as IACPropertyNetServer).ValueUpdatedOnReceival -= PABulkDensityBDS2ST_ValueUpdatedOnReceival;
            (SampleID as IACPropertyNetServer).ValueUpdatedOnReceival -= PABulkDensityBDS2ST_ValueUpdatedOnReceival;
            (DensityCorrFact as IACPropertyNetServer).ValueUpdatedOnReceival -= PABulkDensityBDS2ST_ValueUpdatedOnReceival;
            if (ApplicationManager != null)
                ApplicationManager.ProjectWorkCycleR1sec -= ApplicationManager_ProjectWorkCycleR1sec;
            return base.ACDeInit(deleteACClassTask);
        }


        public override bool ACPostInit()
        {
            bool result = base.ACPostInit();
            ExternalDumpCycle.ValueT = MaxExternalDumpCycles + 1;
            BindMyProperties();
            return result;
        }

        protected bool _PropertiesBound = false;
        protected virtual void BindMyProperties()
        {
            if (_PropertiesBound || Session == null)
                return;
            string message;
            IACPropertyNetTarget newProp;
            IACPropertyNetTarget prop2Bind = BDS2Status as IACPropertyNetTarget;
            if (prop2Bind != null && prop2Bind.Source == null)
                PAStateConverterBase.BindProperty(Session, "HoldingR", prop2Bind, "BDS2Status", out newProp, out message);
            prop2Bind = ProductCodeRes as IACPropertyNetTarget;
            if (prop2Bind != null && prop2Bind.Source == null)
                PAStateConverterBase.BindProperty(Session, "HoldingR", prop2Bind, "ProductCodeRes", out newProp, out message);
            prop2Bind = SampleID as IACPropertyNetTarget;
            if (prop2Bind != null && prop2Bind.Source == null)
                PAStateConverterBase.BindProperty(Session, "HoldingR", prop2Bind, "SampleID", out newProp, out message);
            prop2Bind = CupFillingLevel as IACPropertyNetTarget;
            if (prop2Bind != null && prop2Bind.Source == null)
                PAStateConverterBase.BindProperty(Session, "HoldingR", prop2Bind, "CupFillingLevel", out newProp, out message);
            prop2Bind = WeightValue as IACPropertyNetTarget;
            if (prop2Bind != null && prop2Bind.Source == null)
                PAStateConverterBase.BindProperty(Session, "HoldingR", prop2Bind, "WeightValue", out newProp, out message);
            prop2Bind = DensityValue as IACPropertyNetTarget;
            if (prop2Bind != null && prop2Bind.Source == null)
                PAStateConverterBase.BindProperty(Session, "HoldingR", prop2Bind, "DensityValue", out newProp, out message);
            prop2Bind = MoistureValue as IACPropertyNetTarget;
            if (prop2Bind != null && prop2Bind.Source == null)
                PAStateConverterBase.BindProperty(Session, "HoldingR", prop2Bind, "MoistureValue", out newProp, out message);
            prop2Bind = TemperatureValue as IACPropertyNetTarget;
            if (prop2Bind != null && prop2Bind.Source == null)
                PAStateConverterBase.BindProperty(Session, "HoldingR", prop2Bind, "TemperatureValue", out newProp, out message);
            prop2Bind = ImageValue as IACPropertyNetTarget;
            if (prop2Bind != null && prop2Bind.Source == null)
                PAStateConverterBase.BindProperty(Session, "HoldingR", prop2Bind, "ImageValue", out newProp, out message);
            prop2Bind = BDS2Cmd as IACPropertyNetTarget;
            if (prop2Bind != null && prop2Bind.Source == null)
                PAStateConverterBase.BindProperty(Session, "HoldingR", prop2Bind, "BDS2Cmd", out newProp, out message);
            prop2Bind = ProductCode as IACPropertyNetTarget;
            if (prop2Bind != null && prop2Bind.Source == null)
                PAStateConverterBase.BindProperty(Session, "HoldingR", prop2Bind, "ProductCode", out newProp, out message);
            prop2Bind = DensityCorrFact as IACPropertyNetTarget;
            if (prop2Bind != null && prop2Bind.Source == null)
                PAStateConverterBase.BindProperty(Session, "HoldingR", prop2Bind, "DensityCorrFact", out newProp, out message);
            prop2Bind = MoistureSpan as IACPropertyNetTarget;
            if (prop2Bind != null && prop2Bind.Source == null)
                PAStateConverterBase.BindProperty(Session, "HoldingR", prop2Bind, "MoistureSpan", out newProp, out message);
            prop2Bind = MoistureTrim as IACPropertyNetTarget;
            if (prop2Bind != null && prop2Bind.Source == null)
                PAStateConverterBase.BindProperty(Session, "HoldingR", prop2Bind, "MoistureTrim", out newProp, out message);
            _PropertiesBound = true;
        }

        #endregion


        #region Properties

        #region Configuration
        /// <summary>
        /// New batch (Bit02) should be set when product flow detected again  (ProductFlow = true)
        /// </summary>
        [ACPropertyInfo(true, 690, "Config", "en{'Send <New Batch> on product flow'}de{'<Neuer Batch> senden bei Produktfluss'}", DefaultValue = false)]
        public bool SendNewBatchWithFlow
        {
            get;
            set;
        }

        [ACPropertyInfo(true, 691, "Config", "en{'Maximum cycles of external dump'}de{'Maximum cycles of external dump'}", DefaultValue = 1)]
        public int MaxExternalDumpCycles
        {
            get;
            set;
        }

        [ACPropertyInfo(true, 692, "Config", "en{'Abort on error after n cycles'}de{'Abort on error after n cycles'}", DefaultValue = 1)]
        public int AbortOnErrorAfterNCycles
        {
            get;
            set;
        }

        [ACPropertyInfo(true, 693, "Config", "en{'Wait for SamplingActive is switched off in seconds'}de{'Wait for SamplingActive is switched off in seconds'}", DefaultValue = 5)]
        public int WaitForSwitchOffSamplingActive
        {
            get;
            set;
        }

        #endregion

        #region Read from Device
        /// <summary>
        /// Machine Status, Word, HR51.W (40050)
        /// </summary>
        [ACPropertyBindingTarget(600, "Read", "en{'Status'}de{'Status'}", "", false, false)]
        public IACContainerTNet<PABulkDensityBDS2STStatus> BDS2Status { get; set; }

        /// <summary>
        /// Reply of the Product Code / Recipe Number, received from the Plant controller/Master: >= 0; Max. 9 digits, DINT, HR55.DI (40054 + 40055)
        /// </summary>
        [ACPropertyBindingTarget(601, "Read", "en{'Product Code/No. Read'}de{'Produktnummer Ist'}", "", false, false)]
        public IACContainerTNet<Int32> ProductCodeRes { get; set; }

        /// <summary>
        /// Number of (master) Sampling Cycles, since (batch) start (Incremented after every Sample cycle): >= 0, DINT, HR57.DI (40056 + 40057)
        /// </summary>
        [ACPropertyBindingTarget(602, "Read", "en{'Sample Id/No.'}de{'Probenummer'}", "", false, false)]
        public IACContainerTNet<Int32> SampleID { get; set; }

        /// <summary>
        /// Cup filling result: 0.0 to +100.0 [%], Real, HR59.R (40058 + 40059)
        /// </summary>
        [ACPropertyBindingTarget(603, "Read", "en{'Cup filling level [%]'}de{'Füllstand [%]'}", "", false, false)]
        public IACContainerTNet<Single> CupFillingLevel { get; set; }

        /// <summary>
        /// Weight result: >= 0.0, User scaled, Real, HR61.R (40060 + 40061)
        /// </summary>
        [ACPropertyBindingTarget(604, "Read", "en{'Weight value [kg]'}de{'Gewichtswert [kg]'}", "", false, false)]
        public IACContainerTNet<Single> WeightValue { get; set; }


        /// <summary>
        /// Density value is PAESensorAnalog.ActualValue, Bulk density result: >= 0.0, User scaled, Real, HR63.R (40062 + 40063)
        /// </summary>
        [ACPropertyBindingTarget(605, "Read", "en{'Density value [g/dm³]'}de{'Dichte [g/dm³]'}", "", false, false)]
        public IACContainerTNet<Single> DensityValue { get; set; }


        /// <summary>
        /// Moisture result/contents: 0.0 to +100.0 [%], Real, HR65.R (40064 + 40065)
        /// </summary>
        [ACPropertyBindingTarget(606, "Read", "en{'Moisture value [%]'}de{'Feuchte [%]'}", "", false, false)]
        public IACContainerTNet<Single> MoistureValue { get; set; }


        /// <summary>
        /// Temperature result: >= -50.0, User scaled, Real, HR67.R (40066 + 40067)
        /// </summary>
        [ACPropertyBindingTarget(607, "Read", "en{'Temperature value [°C]'}de{'Temperatur [°C]'}", "", false, false)]
        public IACContainerTNet<Single> TemperatureValue { get; set; }

        /// <summary>
        /// Image/Vision (Camera) result/status: 0 = None;1 = Done; 2 = Error, INT, HR69.I (40068)
        /// </summary>
        [ACPropertyBindingTarget(608, "Read", "en{'Image/Vision (Camera)'}de{'Bildergebnis (Kamera)'}", "", false, false)]
        public IACContainerTNet<PABulkDensityBDS2STImage> ImageValue { get; set; }


        [ACPropertyBindingTarget(609, "Read", "en{'Last Sample Id/No.'}de{'Letzte Probenummer'}", "", false, false)]
        public IACContainerTNet<Int32> LastSampleID { get; set; }

        #endregion

        #region Write to Device

        /// <summary>
        /// Plant Controller Commands, Word, HR1.W (40000)
        /// </summary>
        [ACPropertyBindingTarget(650, "Write", "en{'Command'}de{'Kommando'}", "", false, false)]
        public IACContainerTNet<PABulkDensityBDS2STCmd> BDS2Cmd { get; set; }

        /// <summary>
        /// Product Code / Recipe Number (>=0; Max. 9 digits), DINT, HR5.I (40004 + 40005)
        /// </summary>
        [ACPropertyBindingTarget(651, "Write", "en{'Product Code/No. Write'}de{'Produktnummer Soll'}", "", false, false)]
        public IACContainerTNet<Int32> ProductCode { get; set; }

        /// <summary>
        /// Density, Correction factor: 0.000 to +2.000 (Default/No correction = 1.000), Real, HR7.R (40006 + 40007)
        /// </summary>
        [ACPropertyBindingTarget(652, "Write", "en{'Density Correction factor (0.0-2.0)'}de{'Dichte korrektur Faktor (0.0-2.0)'}", "", false, false)]
        public IACContainerTNet<Single> DensityCorrFact { get; set; }

        /// <summary>
        /// Moisture, Span parameter (Gain factor): -1.000 to +100.000 (Default/No correction = 1.000), Real, HR9.R (40008 + 40009)
        /// </summary>
        [ACPropertyBindingTarget(653, "Write", "en{'Moisture Span (-1.0-100.0)'}de{'Feuchte Spanne (-1.0-100.0)'}", "", false, false)]
        public IACContainerTNet<Single> MoistureSpan { get; set; }

        /// <summary>
        /// Moisture, Trim parameter (Offset value): -1000.000 to +1000.000 (Default/No correction = 0.000), Real, HR11.R (40010 + 40011)
        /// </summary>
        [ACPropertyBindingTarget(654, "Write", "en{'Moisture Trim (-1000.0-1000.0)'}de{'Feuchte Offset (-1000.0-1000.0)'}", "", false, false)]
        public IACContainerTNet<Single> MoistureTrim { get; set; }

        #endregion


        #region External (PLC)
        [ACPropertyBindingTarget(620, "Read PLC", "en{'Product flow'}de{'Produktfluss'}", "", false, false)]
        public IACContainerTNet<bool> ProductFlow { get; set; }

        [ACPropertyBindingSource(621, "ACConfig", "en{'Start time'}de{'Startzeit'}", "", false, true)]
        public IACContainerTNet<DateTime> LastDump { get; set; }

        [ACPropertyBindingSource(622, "ACConfig", "en{'External dump cycle'}de{'External dump cycle'}", "", false, true)]
        public IACContainerTNet<int> ExternalDumpCycle
        {
            get;
            set;
        }

        #endregion

        #region Alarms

        [ACPropertyBindingSource(670, "Error", "en{'External dump error'}de{'External dump error'}", "", false, false)]
        public IACContainerTNet<PANotifyState> ExternalDumpError { get; set; }

        #endregion

        #region Private members

        private bool _IsStatusBit03Off = false;
        private bool _SamplingActive = false;
        private DateTime? _SampleIDChanged = null;
        private int _ErrorExternalDumpCounter = 0;
        private int _SuccessExternalDumpCounter = 0;

        #endregion

        #endregion


        #region Methods

        #region Execute-Helper-Handlers
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "DoExternalDump":
                    DoExternalDump();
                    return true;
                case Const.IsEnabledPrefix + "DoExternalDump":
                    result = IsEnabledDoExternalDump();
                    return true;
                case "ResetACK":
                    ResetACK();
                    return true;
                case Const.IsEnabledPrefix + "ResetACK":
                    result = IsEnabledResetACK();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        //public static bool HandleExecuteACMethod_PABulkDensityBDS2ST(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        //{
        //    return HandleExecuteACMethod_PAProcessFunction(out result, acComponent, acMethodName, acClassMethod, acParameter);
        //}

        #endregion


        #region Events-Handler

        private void PABulkDensityBDS2ST_ValueUpdatedOnReceival(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.AfterBroadcast)
            {
                if (sender == DensityValue)
                {
                    // Refresh Sensor-Value only if Measuring active and no errors
                    if (BDS2Status != null && BDS2Status.ValueT.Bit01_NoErrors)
                    // && BDS2Status.ValueT.Bit03_SamplingActive) // Change 19.02.2021. Mr. Kuester
                    {
                        (ActualValue as IACPropertyNetTarget).ChangeValueServer((double)DensityValue.ValueT, false, e.ValueEvent.InvokerInfo);
                    }
                }
                else if (sender == ProductFlow && e.ValueEvent.InvokerInfo != null)
                {
                    if (BDS2Cmd != null)
                    {
                        BDS2Cmd.ValueT.Bit00_StartSampling = ProductFlow.ValueT;
                        if (SendNewBatchWithFlow && ProductFlow.ValueT)
                            BDS2Cmd.ValueT.Bit02_NewBatch = true;
                    }
                }
                else if (sender == BDS2Status)
                {
                    // Reset ExternalDump-Command if ExternalDump was activated by device
                    // Changed 05.11.2020
                    //if (BDS2Status.ValueT.Bit06_ExternalDump && BDS2Cmd.ValueT.Bit01_ExternalDump)
                    //{
                    //    BDS2Cmd.ValueT.Bit01_ExternalDump = false;
                    //    LastDump.ValueT = DateTime.Now;
                    //}
                    // Falls Fehler-Quittung gesetzt und keine Fehler anstehen, dann muss Startup-Kommandogesetzt werden
                    if (BDS2Status.ValueT.Bit01_NoErrors && BDS2Cmd.ValueT.Bit06_ResetACK)
                    {
                        if (this.ApplicationManager != null)
                        {
                            this.ApplicationManager.ApplicationQueue.Add(() =>
                            {
                                BDS2Cmd.ValueT.Bit07_StartUpBDS = true;
                                BDS2Cmd.ValueT.Bit06_ResetACK = false;
                            });
                        }
                    }
                    if (BDS2Status.ValueT.Bit01_NoErrors
                        && BDS2Status.ValueT.Bit02_Ready
                        && BDS2Cmd.ValueT.Bit07_StartUpBDS)
                    {
                        if (this.ApplicationManager != null)
                        {
                            this.ApplicationManager.ApplicationQueue.Add(() =>
                            {
                                BDS2Cmd.ValueT.Bit07_StartUpBDS = false;
                            });
                        }
                    }

                    //Reset command bit External dump
                    if (BDS2Status.ValueT.Bit06_ExternalDump && BDS2Cmd.ValueT.Bit01_ExternalDump)
                    {
                        BDS2Cmd.ValueT.Bit01_ExternalDump = false;
                        LastDump.ValueT = DateTime.Now;
                    }

                    if (BDS2Status.ValueT.Bit03_SamplingActive)
                    {
                        using (ACMonitor.Lock(_20015_LockValue))
                        {
                            _SamplingActive = true;
                            _IsStatusBit03Off = false;
                        }
                    }

                    if (_SamplingActive && !BDS2Status.ValueT.Bit03_SamplingActive)
                    {
                        bool runNextCycle = false;
                        using (ACMonitor.Lock(_20015_LockValue))
                            runNextCycle = ExternalDumpCycle.ValueT < MaxExternalDumpCycles;

                        if (runNextCycle)
                        {
                            if (this.ApplicationManager != null)
                            {
                                this.ApplicationManager.ApplicationQueue.Add(() =>
                                {
                                    BDS2Cmd.ValueT.Bit01_ExternalDump = true;
                                });
                            }
                            using (ACMonitor.Lock(_20015_LockValue))
                            {
                                _IsStatusBit03Off = true;
                                _SampleIDChanged = null;
                                ExternalDumpCycle.ValueT = ExternalDumpCycle.ValueT + 1;
                                _SuccessExternalDumpCounter++;
                                _SamplingActive = false;
                            }
                        }
                        else
                        {
                            using (ACMonitor.Lock(_20015_LockValue))
                            {
                                if (ExternalDumpCycle.ValueT != MaxExternalDumpCycles + 1)
                                    ExternalDumpCycle.ValueT = MaxExternalDumpCycles + 1;
                                _IsStatusBit03Off = true;
                                _SampleIDChanged = null;
                                _SamplingActive = false;
                            }
                        }
                    }
                }
                else if (sender == SampleID)
                {
                    // Reset New-Batch if SampleID was reset to zero or changed
                    if (BDS2Cmd.ValueT.Bit02_NewBatch)
                        BDS2Cmd.ValueT.Bit02_NewBatch = false;
                    if (LastSampleID.ValueT != SampleID.ValueT)
                    {
                        //if (BDS2Cmd.ValueT.Bit01_ExternalDump)
                        //{
                        //    BDS2Cmd.ValueT.Bit01_ExternalDump = false;
                        //    LastDump.ValueT = DateTime.Now;
                        //}
                        if (BDS2Status.ValueT.Bit03_SamplingActive)
                        {
                            using (ACMonitor.Lock(_20015_LockValue))
                            {
                                _SampleIDChanged = DateTime.Now;
                            }
                        }
                        else
                        {
                            using (ACMonitor.Lock(_20015_LockValue))
                            {
                                _SampleIDChanged = null;
                            }
                        }

                        LastSampleID.ValueT = SampleID.ValueT;
                    }
                }
                else if (sender == DensityCorrFact)
                {
                    // After Restarting a BDS-Unit the DensityCorrection-Factor is zero => Reset it to 1;
                    if (DensityCorrFact.ValueT < 0.001
                        && e.ValueEvent.EventType == EventTypes.ValueChangedInSource
                        && this.ApplicationManager != null)
                    {
                        this.ApplicationManager.ApplicationQueue.Add(() => { DensityCorrFact.ValueT = 1; });
                    }
                }
            }
        }

        private void ApplicationManager_ProjectWorkCycleR1sec(object sender, EventArgs e)
        {
            bool checkForError = false;
            DateTime? sampleDT = null;
            int dumpCycle = ExternalDumpCycle.ValueT;

            using (ACMonitor.Lock(_20015_LockValue))
            {
                sampleDT = _SampleIDChanged;
                checkForError = sampleDT.HasValue && !_IsStatusBit03Off && ExternalDumpCycle.ValueT > 0;
            }

            if (checkForError)
            {
                var time = DateTime.Now - sampleDT.Value;
                if (time.TotalSeconds > WaitForSwitchOffSamplingActive)
                {
                    using (ACMonitor.Lock(_20015_LockValue))
                    {
                        if (ExternalDumpCycle.ValueT > AbortOnErrorAfterNCycles && BDS2Status.ValueT.Bit03_SamplingActive)
                        {
                            ExternalDumpCycle.ValueT = MaxExternalDumpCycles;

                            Msg msg = new Msg("External dump error!", this, eMsgLevel.Error, "PABulkDensityST", "ProjectWorkCycleR1sec(10)", 636);
                            if (IsAlarmActive(ExternalDumpError, msg.Message) == null)
                                OnNewAlarmOccurred(ExternalDumpError, msg);

                            if (ApplicationManager != null)
                                ApplicationManager.ProjectWorkCycleR1sec -= ApplicationManager_ProjectWorkCycleR1sec;
                        }
                        else
                        {
                            _ErrorExternalDumpCounter++;
                            if (_ErrorExternalDumpCounter > 2 && BDS2Status.ValueT.Bit03_SamplingActive)
                            {
                                ExternalDumpCycle.ValueT = MaxExternalDumpCycles;

                                Msg msg = new Msg("External dump error!", this, eMsgLevel.Error, "PABulkDensityST", "ProjectWorkCycleR1sec(20)", 651);
                                if (IsAlarmActive(ExternalDumpError, msg.Message) == null)
                                    OnNewAlarmOccurred(ExternalDumpError, msg);

                                if (ApplicationManager != null)
                                    ApplicationManager.ProjectWorkCycleR1sec -= ApplicationManager_ProjectWorkCycleR1sec;
                            }
                            else
                            {
                                ExternalDumpCycle.ValueT = _SuccessExternalDumpCounter > 0 ? MaxExternalDumpCycles - _SuccessExternalDumpCounter : 0;
                                _SampleIDChanged = null;
                            }
                        }
                    }
                }
            }
            else if (dumpCycle > MaxExternalDumpCycles)
            {
                if (ApplicationManager != null)
                    ApplicationManager.ProjectWorkCycleR1sec -= ApplicationManager_ProjectWorkCycleR1sec;
            }
        }

        #endregion

        #region Client-Methods
        [ACMethodInteraction("", "en{'External dump'}de{'Probe (extern)'}", 601, true)]
        public virtual void DoExternalDump()
        {
            if (!IsEnabledDoExternalDump())
                return;

            using (ACMonitor.Lock(_20015_LockValue))
            {
                BDS2Cmd.ValueT.Bit01_ExternalDump = true;
                ExternalDumpCycle.ValueT = 1;
                _SuccessExternalDumpCounter = 0;
                _ErrorExternalDumpCounter = 0;
                _IsStatusBit03Off = false;
                _SampleIDChanged = null;
                _SamplingActive = false;
            }

            if (ApplicationManager != null)
            {
                ApplicationManager.ProjectWorkCycleR1sec -= ApplicationManager_ProjectWorkCycleR1sec;
                ApplicationManager.ProjectWorkCycleR1sec += ApplicationManager_ProjectWorkCycleR1sec;
            }
        }

        public virtual bool IsEnabledDoExternalDump()
        {
            return BDS2Cmd != null && BDS2Status != null && (!BDS2Cmd.ValueT.Bit01_ExternalDump || !BDS2Status.ValueT.Bit06_ExternalDump);
        }

        [ACMethodInteraction("", "en{'Alarm acknoledge'}de{'Alarmquittung'}", 602, true)]
        public virtual void ResetACK()
        {
            if (!IsEnabledResetACK())
                return;
            BDS2Cmd.ValueT.Bit06_ResetACK = true;
        }

        public virtual bool IsEnabledResetACK()
        {
            return BDS2Cmd != null && BDS2Status != null && !BDS2Cmd.ValueT.Bit06_ResetACK && !BDS2Status.ValueT.Bit01_NoErrors;
        }

        #endregion

        protected override void DumpPropertyList(XmlDocument doc, XmlElement xmlACPropertyList)
        {
            base.DumpPropertyList(doc, xmlACPropertyList);

            XmlElement xmlChild = xmlACPropertyList["IsStatusBit03Off"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("IsStatusBit03Off");
                if (xmlChild != null)
                    xmlChild.InnerText = _IsStatusBit03Off.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["SamplingActive"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("SamplingActive");
                if (xmlChild != null)
                    xmlChild.InnerText = _SamplingActive.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["SampleIDChanged"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("SampleIDChanged");
                if (xmlChild != null)
                    xmlChild.InnerText = _SampleIDChanged?.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["ErrorExternalDumpCounter"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("ErrorExternalDumpCounter");
                if (xmlChild != null)
                    xmlChild.InnerText = _ErrorExternalDumpCounter.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["SuccessExternalDumpCounter"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("SuccessExternalDumpCounter");
                if (xmlChild != null)
                    xmlChild.InnerText = _SuccessExternalDumpCounter.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }
        }

        #endregion
    }
}
