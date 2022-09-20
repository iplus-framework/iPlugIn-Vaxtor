using System;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.core.processapplication;
using System.Runtime.Serialization;
using System.Xml;

namespace tews.mes.processapplication
{
    [DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Tews Command'}de{'Tews Kommando'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    [ACPropertyEntity(101, "Bit01", "en{'Hard empty check (Bit01)'}de{'Harte Leerprüfung (Bit01)'}")]
    [ACPropertyEntity(102, "Bit02", "en{'Sample measurement (Bit02)'}de{'Probemessung (Bit02)'}")]
    [ACPropertyEntity(103, "Bit03", "en{'Device Reset (Bit03)'}de{'Gerät rücksetzen (Bit03)'}")]
    [ACPropertyEntity(104, "Bit04", "en{'Clear error register  (Bit04)'}de{'Fehlerspeicher rücksetzen (Bit04)'}")]
    [ACPropertyEntity(105, "Bit05", "en{'Soft empty check (Bit05)'}de{'Soft Leerprüfung (Bit05)'}")]
    [ACPropertyEntity(106, "Bit06", "en{'Set Register Offset (Bit06)'}de{'Aktiviere Registeroffset (Bit06)'}")]
    [ACPropertyEntity(107, "Bit07", "en{'Acknowledgement (only bypass devices)  (Bit07)'}de{'Quittung (nur Bypass Geräte) (Bit07)'}")]
    [ACPropertyEntity(107, "Bit08", "en{'Start/Stop (Bit08)'}de{'Start/Stop (Bit08)'}")]
    public class PAMW4xxxTewsCmd : BitAccessForInt16
    {
        #region c'tors
        public PAMW4xxxTewsCmd()
        {
        }

        public PAMW4xxxTewsCmd(IACType acValueType)
            : base(acValueType)
        {
        }
        #endregion

        #region Customized Bits

        /// <summary>
        /// hard empty check 
        public bool Bit01_HardEmptyCheck
        {
            get { return Bit01; }
            set { Bit01 = value; }
        }

        /// <summary>
        /// sample measurement
        /// </summary>
        public bool Bit02_SampleMeasurement
        {
            get { return Bit02; }
            set { Bit02 = value; }
        }

        /// <summary>
        /// Device Reset
        /// </summary>
        public bool Bit03_DeviceReset
        {
            get { return Bit03; }
            set { Bit03 = value; }
        }

        /// <summary>
        /// clear error register 
        /// </summary>
        public bool Bit04_ClearErrorRegister
        {
            get { return Bit04; }
            set { Bit04 = value; }
        }

        /// <summary>
        /// soft empty check
        /// </summary>
        public bool Bit05_SoftEmptyCheck
        {
            get { return Bit05; }
            set { Bit05 = value; }
        }

        /// <summary>
        /// set offset given in registers 40007,40008 and 40009,40010 
        /// </summary>
        public bool Bit06_SetRegisterOffset
        {
            get { return Bit06; }
            set { Bit06 = value; }
        }

        /// <summary>
        /// acknowledgement (only bypass devices) 
        /// </summary>
        public bool Bit07_StartUpBDS
        {
            get { return Bit07; }
            set { Bit07 = value; }
        }

        /// <summary>
        /// start = 1 / stop = 0 measurement 
        /// </summary>
        public bool Bit08_StartStop
        {
            get { return Bit08; }
            set { Bit08 = value; }
        }
        #endregion
    }


    [DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Tews Status'}de{'Tews Status'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    [ACPropertyEntity(101, "Bit01", "en{'Measurement ready (Bit01)'}de{'Messergebnis gültig (Bit01)'}")]
    [ACPropertyEntity(102, "Bit02", "en{'Empty (Bit02)'}de{'Leer (Bit02)'}")]
    [ACPropertyEntity(103, "Bit03", "en{'Command Run (Bit03)'}de{'Befehl läuft (Bit03)'}")]
    [ACPropertyEntity(104, "Bit04", "en{'Heartbeat (Bit04)'}de{'Heartbeat (Bit04)'}")]
    [ACPropertyEntity(105, "Bit05", "en{'Measurement run  (Bit05)'}de{'Messung läuft (Bit05)'}")]
    public class PAMW4xxxTewsStatus : BitAccessForInt16
    {
        #region c'tors
        public PAMW4xxxTewsStatus()
        {
        }

        public PAMW4xxxTewsStatus(IACType acValueType)
            : base(acValueType)
        {
        }
        #endregion

        #region Customized Bits

        /// <summary>
        /// Measurement ready bit 
        /// • Bypass process device
        /// The bit is set low during startup and when the measurement process is stopped.
        /// During measurement, the bit goes low right before the filling pause, and it goes high
        /// again after successfully saving the measurement.
        /// If the macroparameter “09. Delay time for OUTX Measure off[sec]” is enabled, the
        /// bit goes low again after the set amount of seconds 
        /// • Continuous process device
        /// The bit is set low during startup and when the measurement process is stopped.
        /// During measurement, the bit goes high after successfully saving a measurement and
        /// it is set back to low after n seconds if “05 Hold time of OUTX Meas.ready off[sec]” 
        /// is enabled.
        /// </summary>
        public bool Bit01_MeasurementReady
        {
            get { return Bit01; }
            set { Bit01 = value; }
        }

        /// <summary>
        /// Empty bit 
        /// In case the measurement task is running, and no other error is reported the bit is set high 
        /// in case of empty measurement.
        /// </summary>
        public bool Bit02_Empty
        {
            get { return Bit02; }
            set { Bit02 = value; }
        }

        /// <summary>
        /// Command run bit 
        /// In case a command given through the holding command register is being executed, the bit is set high
        /// </summary>
        public bool Bit03_CommandRun
        {
            get { return Bit03; }
            set { Bit03 = value; }
        }

        /// <summary>
        /// Heartbeat bit 
        /// The bit is toggled every second to signalize that the system is running correctly
        /// </summary>
        public bool Bit04_Heartbeat
        {
            get { return Bit04; }
            set { Bit04 = value; }
        }

        /// <summary>
        /// Measurement run bit 
        /// The bit is set while a measurement or a calibration measurement is running
        /// </summary>
        public bool Bit05_MeasurementRun
        {
            get { return Bit05; }
            set { Bit05 = value; }
        }
        #endregion
    }


    [DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Tews Error'}de{'Tews Error'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    [ACPropertyEntity(101, "Bit01", "en{'Empty check error (Bit01)'}de{'Leerprüfungsfehler (Bit01)'}")]
    [ACPropertyEntity(103, "Bit03", "en{'Product error (Bit03)'}de{'Produktfehler (Bit03)'}")]
    [ACPropertyEntity(104, "Bit04", "en{'Device error (Bit04)'}de{'Gerätefehler (Bit04)'}")]
    [ACPropertyEntity(105, "Bit05", "en{'Database error (Bit05)'}de{'Datenbankfehler (Bit05)'}")]
    [ACPropertyEntity(106, "Bit06", "en{'Acknowledgement required  (Bit06)'}de{'Bestätigung erforderlich (Bit06)'}")]
    public class PAMW4xxxTewsError : BitAccessForInt16
    {
        #region c'tors
        public PAMW4xxxTewsError()
        {
        }

        public PAMW4xxxTewsError(IACType acValueType)
            : base(acValueType)
        {
        }
        #endregion

        #region Customized Bits


        /// <summary>
        ///  Empty check error bit 
        ///  The bit is set high in case of error during an empty check
        /// </summary>
        public bool Bit01_EmptyCheckError
        {
            get { return Bit01; }
            set { Bit01 = value; }
        }

        /// <summary>
        /// Product error bit 
        /// The error is set high in case of error during loading a product
        /// </summary>
        public bool Bit03_ProductError
        {
            get { return Bit03; }
            set { Bit03 = value; }
        }

        /// <summary>
        /// Device error bit 
        /// the bit is set in case of any error and/or the measurement is not running
        /// </summary>
        public bool Bit04_DeviceError 
        {
            get { return Bit04; }
            set { Bit04 = value; }
        }

        /// <summary>
        /// Database error bit 
        /// The bit is set in case of communication error with the external database(“pharma” 
        /// macroparameter enabled)
        /// </summary>
        public bool Bit05_DatabaseError
        {
            get { return Bit05; }
            set { Bit05 = value; }
        }

        /// <summary>
        /// The bit is set when the device is in blocked modus and an external acknowledgement is 
        /// required to continue (only bypass devices) 
        /// </summary>
        public bool Bit06_AcknowledgementRequired 
        {
            get { return Bit06; }
            set { Bit06 = value; }
        }
        #endregion
    }


    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Tews moisture measurement device'}de{'Tews Feuchtemessgerät'}", Global.ACKinds.TPAModule, Global.ACStorableTypes.Required, false, true)]
    public class PAMW4xxxTews : PAESensorAnalog
    {
        #region c'tors
        public PAMW4xxxTews(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            (Status as IACPropertyNetServer).ValueUpdatedOnReceival += PAMW4xxxTews_ValueUpdatedOnReceival;
            return true;
        }



        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            (Status as IACPropertyNetServer).ValueUpdatedOnReceival -= PAMW4xxxTews_ValueUpdatedOnReceival;
            return base.ACDeInit(deleteACClassTask);
        }


        public override bool ACPostInit()
        {
            bool result = base.ACPostInit();
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
            IACPropertyNetTarget prop2Bind = Status as IACPropertyNetTarget;
            if (prop2Bind != null && prop2Bind.Source == null)
                PAStateConverterBase.BindProperty(Session, "InputR", prop2Bind, "Status", out newProp, out message);
            prop2Bind = ErrorStatus as IACPropertyNetTarget;
            if (prop2Bind != null && prop2Bind.Source == null)
                PAStateConverterBase.BindProperty(Session, "InputR", prop2Bind, "ErrorStatus", out newProp, out message);
            prop2Bind = StoredMoisture as IACPropertyNetTarget;
            if (prop2Bind != null && prop2Bind.Source == null)
                PAStateConverterBase.BindProperty(Session, "InputR", prop2Bind, "StoredMoisture", out newProp, out message);
            prop2Bind = StoredDensity as IACPropertyNetTarget;
            if (prop2Bind != null && prop2Bind.Source == null)
                PAStateConverterBase.BindProperty(Session, "InputR", prop2Bind, "StoredDensity", out newProp, out message);
            prop2Bind = StoredTemperature as IACPropertyNetTarget;
            if (prop2Bind != null && prop2Bind.Source == null)
                PAStateConverterBase.BindProperty(Session, "InputR", prop2Bind, "StoredTemperature", out newProp, out message);
            prop2Bind = CurrentMoisture as IACPropertyNetTarget;
            if (prop2Bind != null && prop2Bind.Source == null)
                PAStateConverterBase.BindProperty(Session, "InputR", prop2Bind, "CurrentMoisture", out newProp, out message);
            prop2Bind = CurrentDensity as IACPropertyNetTarget;
            if (prop2Bind != null && prop2Bind.Source == null)
                PAStateConverterBase.BindProperty(Session, "InputR", prop2Bind, "CurrentDensity", out newProp, out message);
            prop2Bind = CurrentTemperature as IACPropertyNetTarget;
            if (prop2Bind != null && prop2Bind.Source == null)
                PAStateConverterBase.BindProperty(Session, "InputR", prop2Bind, "CurrentTemperature", out newProp, out message);
            prop2Bind = CurrentProductNumber as IACPropertyNetTarget;
            if (prop2Bind != null && prop2Bind.Source == null)
                PAStateConverterBase.BindProperty(Session, "InputR", prop2Bind, "CurrentProductNumber", out newProp, out message);
            prop2Bind = Cmd as IACPropertyNetTarget;
            if (prop2Bind != null && prop2Bind.Source == null)
                PAStateConverterBase.BindProperty(Session, "HoldingR", prop2Bind, "Cmd", out newProp, out message);
            prop2Bind = ProductCode as IACPropertyNetTarget;
            if (prop2Bind != null && prop2Bind.Source == null)
                PAStateConverterBase.BindProperty(Session, "HoldingR", prop2Bind, "ProductCode", out newProp, out message);
            prop2Bind = TargetTemperature as IACPropertyNetTarget;
            if (prop2Bind != null && prop2Bind.Source == null)
                PAStateConverterBase.BindProperty(Session, "HoldingR", prop2Bind, "TargetTemperature", out newProp, out message);
            prop2Bind = MoistureOffset as IACPropertyNetTarget;
            if (prop2Bind != null && prop2Bind.Source == null)
                PAStateConverterBase.BindProperty(Session, "HoldingR", prop2Bind, "MoistureOffset", out newProp, out message);
            prop2Bind = DensityOffset as IACPropertyNetTarget;
            if (prop2Bind != null && prop2Bind.Source == null)
                PAStateConverterBase.BindProperty(Session, "HoldingR", prop2Bind, "DensityOffset", out newProp, out message);
            _PropertiesBound = true;
        }

        #endregion


        #region Properties

        #region Configuration
        //[ACPropertyInfo(true, 690, "Config", "en{'Send <New Batch> on product flow'}de{'<Neuer Batch> senden bei Produktfluss'}", DefaultValue = false)]
        //public bool SendNewBatchWithFlow
        //{
        //    get;
        //    set;
        //}


        #endregion

        #region Read from Device
        /// <summary>
        /// Input register 30013 – uint16 – status register 
        /// </summary>
        [ACPropertyBindingTarget(600, "Read", "en{'Status'}de{'Status'}", "", false, false)]
        public IACContainerTNet<PAMW4xxxTewsStatus> Status { get; set; }

        /// <summary>
        /// Input register 30014 – uint16 – error register 
        /// </summary>
        [ACPropertyBindingTarget(601, "Read", "en{'Error Status'}de{'Fehlertatus'}", "", false, false)]
        public IACContainerTNet<PAMW4xxxTewsError> ErrorStatus { get; set; }

        /// <summary>
        /// Input register 30001,30002 – float32 – stored moisture 
        /// </summary>
        [ACPropertyBindingTarget(602, "Read", "en{'Stored moisture'}de{'Letzte Feuchte'}", "", false, false)]
        public IACContainerTNet<Single> StoredMoisture { get; set; }

        /// <summary>
        /// Input register 30003,30004 – float32 – stored density 
        /// </summary>
        [ACPropertyBindingTarget(603, "Read", "en{'Stored density '}de{'Letzte Dichte'}", "", false, false)]
        public IACContainerTNet<Single> StoredDensity { get; set; }

        /// <summary>
        /// Input register 30005,30006 – float32 – stored product temperature
        /// </summary>
        [ACPropertyBindingTarget(604, "Read", "en{'Stored product temperature'}de{'Letzte Produkttemperatur'}", "", false, false)]
        public IACContainerTNet<Single> StoredTemperature { get; set; }

        /// <summary>
        /// Input register 30007,30008 – float32 – current moisture 
        /// </summary>
        [ACPropertyBindingTarget(605, "Read", "en{'Current moisture'}de{'Aktuelle Feuchte'}", "", false, false)]
        public IACContainerTNet<Single> CurrentMoisture { get; set; }

        /// <summary>
        /// Input register 30009,30010 – float32 – current density 
        /// </summary>
        [ACPropertyBindingTarget(606, "Read", "en{'Current density'}de{'Aktuelle Dichte'}", "", false, false)]
        public IACContainerTNet<Single> CurrentDensity { get; set; }

        /// <summary>
        /// Input register 30011,30012 – float32 – current product temperature 
        /// </summary>
        [ACPropertyBindingTarget(607, "Read", "en{'Current product temperature'}de{'Aktuelle Produkttemperatur'}", "", false, false)]
        public IACContainerTNet<Single> CurrentTemperature { get; set; }

        /// <summary>
        /// Input register 30015 – uint16 – Product number 
        /// </summary>
        [ACPropertyBindingTarget(609, "Read", "en{'Product number '}de{'Produktnummer'}", "", false, false)]
        public IACContainerTNet<UInt32> CurrentProductNumber  { get; set; }

        #endregion

        #region Write to Device

        /// <summary>
        /// Holding register 40004 – uint16 – command
        /// </summary>
        [ACPropertyBindingTarget(650, "Write", "en{'Command'}de{'Kommando'}", "", false, false)]
        public IACContainerTNet<PAMW4xxxTewsCmd> Cmd { get; set; }

        /// <summary>
        /// Holding register 40001 – uint16 – product number 
        /// </summary>
        [ACPropertyBindingTarget(651, "Write", "en{'Product Code/No. Write'}de{'Produktnummer Soll'}", "", false, false)]
        public IACContainerTNet<UInt32> ProductCode { get; set; }

        /// <summary>
        /// Holding register 40002,40003 – float – product temperature 
        /// </summary>
        [ACPropertyBindingTarget(652, "Write", "en{'Target Temperature'}de{'Temperatur Soll)'}", "", false, false)]
        public IACContainerTNet<Single> TargetTemperature { get; set; }

        /// <summary>
        /// Holding register 40007,40008 – float – moisture offset 
        /// </summary>
        [ACPropertyBindingTarget(653, "Write", "en{'Moisture Offset'}de{'Feuchte Offset'}", "", false, false)]
        public IACContainerTNet<Single> MoistureOffset { get; set; }

        /// <summary>
        /// Holding register 40009,40010 – float – density offset 
        /// </summary>
        [ACPropertyBindingTarget(654, "Write", "en{'Density offset'}de{'Dichte Offset'}", "", false, false)]
        public IACContainerTNet<Single> DensityOffset { get; set; }

        #endregion


        #region Alarms

        [ACPropertyBindingSource(670, "Error", "en{'External dump error'}de{'External dump error'}", "", false, false)]
        public IACContainerTNet<PANotifyState> ExternalDumpError { get; set; }

        #endregion

        #region Private members

        private PAMW4xxxTewsStatus _PreviousStatus;
        protected PAMW4xxxTewsStatus PreviousStatus
        {
            get
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _PreviousStatus;
                }
            }
            set
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _PreviousStatus = value;
                }
            }
        }
        DateTime _LastMeasurementReadyGoneTrue = DateTime.Now;
        DateTime _LastMeasurementReadyGoneFalse = DateTime.Now;

        #endregion

        #endregion


        #region Methods

        #region Execute-Helper-Handlers
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(StartMeasurement):
                    StartMeasurement();
                    return true;
                case Const.IsEnabledPrefix + nameof(StartMeasurement):
                    result = IsEnabledStartMeasurement();
                    return true;
                case nameof(StopMeasurement):
                    StopMeasurement();
                    return true;
                case Const.IsEnabledPrefix + nameof(StopMeasurement):
                    result = IsEnabledStopMeasurement();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        //public static bool HandleExecuteACMethod_PAMW4xxxTews(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        //{
        //    return HandleExecuteACMethod_PAProcessFunction(out result, acComponent, acMethodName, acClassMethod, acParameter);
        //}

        #endregion


        #region Events-Handler

        private void PAMW4xxxTews_ValueUpdatedOnReceival(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.AfterBroadcast)
            {
                if (sender == Status)
                {
                    PAMW4xxxTewsStatus prevStatus = PreviousStatus;
                    if (prevStatus != null)
                    {
                        if (Status.ValueT.Bit01_MeasurementReady && !prevStatus.Bit01_MeasurementReady)
                            _LastMeasurementReadyGoneTrue = DateTime.Now;
                        else if (!Status.ValueT.Bit01_MeasurementReady && prevStatus.Bit01_MeasurementReady)
                            _LastMeasurementReadyGoneFalse = DateTime.Now;
                    }

                    prevStatus = new PAMW4xxxTewsStatus(Status.ValueT.ValueTypeACClass);
                    prevStatus.ValueT = Status.ValueT.ValueT;
                    PreviousStatus = prevStatus;
                }
            }
        }

        //private void ApplicationManager_ProjectWorkCycleR1sec(object sender, EventArgs e)
        //{
        //    bool checkForError = false;
        //    DateTime? sampleDT = null;
        //    int dumpCycle = ExternalDumpCycle.ValueT;

        //    using (ACMonitor.Lock(_20015_LockValue))
        //    {
        //        sampleDT = _SampleIDChanged;
        //        checkForError = sampleDT.HasValue && !_IsStatusBit03Off && ExternalDumpCycle.ValueT > 0;
        //    }

        //    if (checkForError)
        //    {
        //        var time = DateTime.Now - sampleDT.Value;
        //        if (time.TotalSeconds > WaitForSwitchOffSamplingActive)
        //        {
        //            using (ACMonitor.Lock(_20015_LockValue))
        //            {
        //                if (ExternalDumpCycle.ValueT > AbortOnErrorAfterNCycles && BDS2Status.ValueT.Bit03_SamplingActive)
        //                {
        //                    ExternalDumpCycle.ValueT = MaxExternalDumpCycles;

        //                    Msg msg = new Msg("External dump error!", this, eMsgLevel.Error, "PABulkDensityST", "ProjectWorkCycleR1sec(10)", 636);
        //                    if (IsAlarmActive(ExternalDumpError, msg.Message) == null)
        //                        OnNewAlarmOccurred(ExternalDumpError, msg);

        //                    if (ApplicationManager != null)
        //                        ApplicationManager.ProjectWorkCycleR1sec -= ApplicationManager_ProjectWorkCycleR1sec;
        //                }
        //                else
        //                {
        //                    _ErrorExternalDumpCounter++;
        //                    if (_ErrorExternalDumpCounter > 2 && BDS2Status.ValueT.Bit03_SamplingActive)
        //                    {
        //                        ExternalDumpCycle.ValueT = MaxExternalDumpCycles;

        //                        Msg msg = new Msg("External dump error!", this, eMsgLevel.Error, "PABulkDensityST", "ProjectWorkCycleR1sec(20)", 651);
        //                        if (IsAlarmActive(ExternalDumpError, msg.Message) == null)
        //                            OnNewAlarmOccurred(ExternalDumpError, msg);

        //                        if (ApplicationManager != null)
        //                            ApplicationManager.ProjectWorkCycleR1sec -= ApplicationManager_ProjectWorkCycleR1sec;
        //                    }
        //                    else
        //                    {
        //                        ExternalDumpCycle.ValueT = _SuccessExternalDumpCounter > 0 ? MaxExternalDumpCycles - _SuccessExternalDumpCounter : 0;
        //                        _SampleIDChanged = null;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    else if (dumpCycle > MaxExternalDumpCycles)
        //    {
        //        if (ApplicationManager != null)
        //            ApplicationManager.ProjectWorkCycleR1sec -= ApplicationManager_ProjectWorkCycleR1sec;
        //    }
        //}

        #endregion

        #region Client-Methods
        [ACMethodInteraction("", "en{'Start measurement'}de{'Messung starten'}", 601, true)]
        public virtual void StartMeasurement()
        {
            if (!IsEnabledStartMeasurement())
                return;
            this.Cmd.ValueT.Bit08_StartStop = true;
        }

        public virtual bool IsEnabledStartMeasurement()
        {
            return Cmd != null && Status != null && ProductCode.ValueT > 0;
        }

        [ACMethodInteraction("", "en{'Stop measurement'}de{'Messung stoppen'}", 602, true)]
        public virtual void StopMeasurement()
        {
            if (!IsEnabledStopMeasurement())
                return;
            this.Cmd.ValueT.Bit08_StartStop = false;
        }

        public virtual bool IsEnabledStopMeasurement()
        {
            return Cmd != null && Status != null;
        }

        #endregion

        protected override void DumpPropertyList(XmlDocument doc, XmlElement xmlACPropertyList)
        {
            base.DumpPropertyList(doc, xmlACPropertyList);
        }

        #endregion
    }
}
