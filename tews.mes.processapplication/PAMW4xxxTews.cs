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
    [ACPropertyEntity(100, "Bit00", "en{'Hard empty check (Bit00)'}de{'Harte Leerprüfung (Bit00)'}")]
    [ACPropertyEntity(101, "Bit01", "en{'Sample measurement (Bit01)'}de{'Probemessung (Bit01)'}")]
    [ACPropertyEntity(102, "Bit02", "en{'Device Reset (Bit02)'}de{'Gerät rücksetzen (Bit02)'}")]
    [ACPropertyEntity(103, "Bit03", "en{'Clear error register  (Bit03)'}de{'Fehlerspeicher rücksetzen (Bit03)'}")]
    [ACPropertyEntity(104, "Bit04", "en{'Soft empty check (Bit04)'}de{'Soft Leerprüfung (Bit04)'}")]
    [ACPropertyEntity(105, "Bit05", "en{'Set Register Offset (Bit05)'}de{'Aktiviere Registeroffset (Bit05)'}")]
    [ACPropertyEntity(106, "Bit06", "en{'Acknowledgement (only bypass devices) (Bit06)'}de{'Quittung (nur Bypass Geräte) (Bit06)'}")]
    [ACPropertyEntity(107, "Bit07", "en{'Start/Stop (Bit07)'}de{'Start/Stop (Bit07)'}")]
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
        public bool Bit00_HardEmptyCheck
        {
            get { return Bit00; }
            set { Bit00 = value; }
        }

        /// <summary>
        /// sample measurement
        /// </summary>
        public bool Bit01_SampleMeasurement
        {
            get { return Bit01; }
            set { Bit01 = value; }
        }

        /// <summary>
        /// Device Reset
        /// </summary>
        public bool Bit02_DeviceReset
        {
            get { return Bit02; }
            set { Bit02 = value; }
        }

        /// <summary>
        /// clear error register 
        /// </summary>
        public bool Bit03_ClearErrorRegister
        {
            get { return Bit03; }
            set { Bit03 = value; }
        }

        /// <summary>
        /// soft empty check
        /// </summary>
        public bool Bit04_SoftEmptyCheck
        {
            get { return Bit04; }
            set { Bit04 = value; }
        }

        /// <summary>
        /// set offset given in registers 40007,40008 and 40009,40010 
        /// </summary>
        public bool Bit05_SetRegisterOffset
        {
            get { return Bit05; }
            set { Bit05 = value; }
        }

        /// <summary>
        /// acknowledgement (only bypass devices) 
        /// </summary>
        public bool Bit06_AckAlarm
        {
            get { return Bit06; }
            set { Bit06 = value; }
        }

        /// <summary>
        /// start = 1 / stop = 0 measurement 
        /// </summary>
        public bool Bit07_StartStop
        {
            get { return Bit07; }
            set { Bit07 = value; }
        }
        #endregion
    }


    [DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Tews Status'}de{'Tews Status'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    [ACPropertyEntity(100, "Bit00", "en{'Measurement ready (Bit00)'}de{'Messergebnis gültig (Bit00)'}")]
    [ACPropertyEntity(101, "Bit01", "en{'Empty (Bit01)'}de{'Leer (Bit01)'}")]
    [ACPropertyEntity(102, "Bit02", "en{'Command Run (Bit02)'}de{'Befehl läuft (Bit02)'}")]
    [ACPropertyEntity(103, "Bit03", "en{'Heartbeat (Bit03)'}de{'Heartbeat (Bit03)'}")]
    [ACPropertyEntity(104, "Bit04", "en{'Measurement run  (Bit04)'}de{'Messung läuft (Bit04)'}")]
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
        public bool Bit00_MeasurementReady
        {
            get { return Bit00; }
            set { Bit00 = value; }
        }

        /// <summary>
        /// Empty bit 
        /// In case the measurement task is running, and no other error is reported the bit is set high 
        /// in case of empty measurement.
        /// </summary>
        public bool Bit01_Empty
        {
            get { return Bit01; }
            set { Bit01 = value; }
        }

        /// <summary>
        /// Command run bit 
        /// In case a command given through the holding command register is being executed, the bit is set high
        /// </summary>
        public bool Bit02_CommandRun
        {
            get { return Bit02; }
            set { Bit02 = value; }
        }

        /// <summary>
        /// Heartbeat bit 
        /// The bit is toggled every second to signalize that the system is running correctly
        /// </summary>
        public bool Bit03_Heartbeat
        {
            get { return Bit03; }
            set { Bit03 = value; }
        }

        /// <summary>
        /// Measurement run bit 
        /// The bit is set while a measurement or a calibration measurement is running
        /// </summary>
        public bool Bit04_MeasurementRun
        {
            get { return Bit04; }
            set { Bit04 = value; }
        }
        #endregion
    }


    [DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Tews Error'}de{'Tews Error'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    [ACPropertyEntity(100, "Bit00", "en{'Empty check error (Bit00)'}de{'Leerprüfungsfehler (Bit00)'}")]
    [ACPropertyEntity(102, "Bit02", "en{'Product error (Bit02)'}de{'Produktfehler (Bit02)'}")]
    [ACPropertyEntity(103, "Bit03", "en{'Device error (Bit03)'}de{'Gerätefehler (Bit03)'}")]
    [ACPropertyEntity(104, "Bit04", "en{'Database error (Bit04)'}de{'Datenbankfehler (Bit04)'}")]
    [ACPropertyEntity(105, "Bit05", "en{'Acknowledgement required  (Bit05)'}de{'Bestätigung erforderlich (Bit05)'}")]
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
        public bool Bit00_EmptyCheckError
        {
            get { return Bit00; }
            set { Bit00 = value; }
        }

        /// <summary>
        /// Product error bit 
        /// The error is set high in case of error during loading a product
        /// </summary>
        public bool Bit02_ProductError
        {
            get { return Bit02; }
            set { Bit02 = value; }
        }

        /// <summary>
        /// Device error bit 
        /// the bit is set in case of any error and/or the measurement is not running
        /// </summary>
        public bool Bit03_DeviceError 
        {
            get { return Bit03; }
            set { Bit03 = value; }
        }

        /// <summary>
        /// Database error bit 
        /// The bit is set in case of communication error with the external database(“pharma” 
        /// macroparameter enabled)
        /// </summary>
        public bool Bit04_DatabaseError
        {
            get { return Bit04; }
            set { Bit04 = value; }
        }

        /// <summary>
        /// The bit is set when the device is in blocked modus and an external acknowledgement is 
        /// required to continue (only bypass devices) 
        /// </summary>
        public bool Bit05_AcknowledgementRequired 
        {
            get { return Bit05; }
            set { Bit05 = value; }
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
            _MaxResponseWaitingTime = new ACPropertyConfigValue<ushort>(this, nameof(MaxResponseWaitingTime), 10);
            (Status as IACPropertyNetServer).ValueUpdatedOnReceival += PAMW4xxxTews_ValueUpdatedOnReceival;
            (ErrorStatus as IACPropertyNetServer).ValueUpdatedOnReceival += PAMW4xxxTews_ValueUpdatedOnReceival;
            return true;
        }



        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (this.ApplicationManager != null)
                this.ApplicationManager.ProjectWorkCycleR1sec -= ApplicationManager_ProjectWorkCycleR1sec;
            (Status as IACPropertyNetServer).ValueUpdatedOnReceival -= PAMW4xxxTews_ValueUpdatedOnReceival;
            (ErrorStatus as IACPropertyNetServer).ValueUpdatedOnReceival -= PAMW4xxxTews_ValueUpdatedOnReceival;
            return base.ACDeInit(deleteACClassTask);
        }


        public override bool ACPostInit()
        {
            bool result = base.ACPostInit();
            if (this.ApplicationManager != null)
                this.ApplicationManager.ProjectWorkCycleR1sec += ApplicationManager_ProjectWorkCycleR1sec;
            BindMyProperties();
            return result;
        }

        private void ApplicationManager_ProjectWorkCycleR1sec(object sender, EventArgs e)
        {
            //ToggleBits();
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
                PAStateConverterBase.BindProperty(Session, "InputR", prop2Bind, "State", out newProp, out message);
            prop2Bind = ErrorStatus as IACPropertyNetTarget;
            if (prop2Bind != null && prop2Bind.Source == null)
                PAStateConverterBase.BindProperty(Session, "InputR", prop2Bind, "Error", out newProp, out message);
            prop2Bind = StoredMoisture as IACPropertyNetTarget;
            if (prop2Bind != null && prop2Bind.Source == null)
                PAStateConverterBase.BindProperty(Session, "InputR", prop2Bind, "LastSMoisture", out newProp, out message);
            prop2Bind = StoredDensity as IACPropertyNetTarget;
            if (prop2Bind != null && prop2Bind.Source == null)
                PAStateConverterBase.BindProperty(Session, "InputR", prop2Bind, "LastSDensity", out newProp, out message);
            prop2Bind = StoredTemperature as IACPropertyNetTarget;
            if (prop2Bind != null && prop2Bind.Source == null)
                PAStateConverterBase.BindProperty(Session, "InputR", prop2Bind, "LastSTemp", out newProp, out message);
            prop2Bind = CurrentMoisture as IACPropertyNetTarget;
            if (prop2Bind != null && prop2Bind.Source == null)
                PAStateConverterBase.BindProperty(Session, "InputR", prop2Bind, "CurrMoisture", out newProp, out message);
            prop2Bind = CurrentDensity as IACPropertyNetTarget;
            if (prop2Bind != null && prop2Bind.Source == null)
                PAStateConverterBase.BindProperty(Session, "InputR", prop2Bind, "CurrDensity", out newProp, out message);
            prop2Bind = CurrentTemperature as IACPropertyNetTarget;
            if (prop2Bind != null && prop2Bind.Source == null)
                PAStateConverterBase.BindProperty(Session, "InputR", prop2Bind, "CurrTemp", out newProp, out message);
            prop2Bind = CurrentProductNumber as IACPropertyNetTarget;
            if (prop2Bind != null && prop2Bind.Source == null)
                PAStateConverterBase.BindProperty(Session, "InputR", prop2Bind, "CurrProductNo", out newProp, out message);
            prop2Bind = Cmd as IACPropertyNetTarget;
            if (prop2Bind != null && prop2Bind.Source == null)
                PAStateConverterBase.BindProperty(Session, "HoldingR", prop2Bind, "Cmd", out newProp, out message);
            prop2Bind = ProductCode as IACPropertyNetTarget;
            if (prop2Bind != null && prop2Bind.Source == null)
                PAStateConverterBase.BindProperty(Session, "HoldingR", prop2Bind, "ProductNo", out newProp, out message);
            prop2Bind = TargetTemperature as IACPropertyNetTarget;
            if (prop2Bind != null && prop2Bind.Source == null)
                PAStateConverterBase.BindProperty(Session, "HoldingR", prop2Bind, "TargetTemp", out newProp, out message);
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
        private ACPropertyConfigValue<ushort> _MaxResponseWaitingTime;
        [ACPropertyConfig("en{'Max. waiting time for response [sec]'}de{'Max. waiting time for response [sec]'}")]
        public ushort MaxResponseWaitingTime
        {
            get
            {
                return _MaxResponseWaitingTime.ValueT;
            }
            set
            {
                _MaxResponseWaitingTime.ValueT = value;
            }
        }
        #endregion

        #region Read from Device
        /// <summary>
        /// Input register 30013 – uint16 – status register, IR13.W
        /// </summary>
        [ACPropertyBindingTarget(600, "Read", "en{'Status'}de{'Status'}", "", false, false)]
        public IACContainerTNet<PAMW4xxxTewsStatus> Status { get; set; }

        /// <summary>
        /// Input register 30014 – uint16 – error register, IR14.W
        /// </summary>
        [ACPropertyBindingTarget(601, "Read", "en{'Error Status'}de{'Fehlertatus'}", "", false, false)]
        public IACContainerTNet<PAMW4xxxTewsError> ErrorStatus { get; set; }

        /// <summary>
        /// Input register 30001,30002 – float32 – stored moisture, IR1.R
        /// </summary>
        [ACPropertyBindingTarget(602, "Read", "en{'Stored moisture'}de{'Letzte Feuchte'}", "", false, false)]
        public IACContainerTNet<Single> StoredMoisture { get; set; }

        /// <summary>
        /// Input register 30003,30004 – float32 – stored density, IR3.R
        /// </summary>
        [ACPropertyBindingTarget(603, "Read", "en{'Stored density '}de{'Letzte Dichte'}", "", false, false)]
        public IACContainerTNet<Single> StoredDensity { get; set; }

        /// <summary>
        /// Input register 30005,30006 – float32 – stored product temperature, IR5.R
        /// </summary>
        [ACPropertyBindingTarget(604, "Read", "en{'Stored product temperature'}de{'Letzte Produkttemperatur'}", "", false, false)]
        public IACContainerTNet<Single> StoredTemperature { get; set; }

        /// <summary>
        /// Input register 30007,30008 – float32 – current moisture , IR7.R
        /// </summary>
        [ACPropertyBindingTarget(605, "Read", "en{'Current moisture'}de{'Aktuelle Feuchte'}", "", false, false)]
        public IACContainerTNet<Single> CurrentMoisture { get; set; }

        /// <summary>
        /// Input register 30009,30010 – float32 – current density, IR9.R
        /// </summary>
        [ACPropertyBindingTarget(606, "Read", "en{'Current density'}de{'Aktuelle Dichte'}", "", false, false)]
        public IACContainerTNet<Single> CurrentDensity { get; set; }

        /// <summary>
        /// Input register 30011,30012 – float32 – current product temperature, IR11.R
        /// </summary>
        [ACPropertyBindingTarget(607, "Read", "en{'Current product temperature'}de{'Aktuelle Produkttemperatur'}", "", false, false)]
        public IACContainerTNet<Single> CurrentTemperature { get; set; }

        /// <summary>
        /// Input register 30015 – uint16 – Product number, IR15.W
        /// </summary>
        [ACPropertyBindingTarget(609, "Read", "en{'Product number'}de{'Produktnummer'}", "", false, false)]
        public IACContainerTNet<UInt32> CurrentProductNumber  { get; set; }

        #endregion

        #region Write to Device

        /// <summary>
        /// Holding register 40004 – uint16 – command, HR4.W
        /// </summary>
        [ACPropertyBindingTarget(650, "Write", "en{'Command'}de{'Kommando'}", "", false, false)]
        public IACContainerTNet<PAMW4xxxTewsCmd> Cmd { get; set; }

        /// <summary>
        /// Holding register 40001 – uint16 – product number, HR1.W
        /// </summary>
        [ACPropertyBindingTarget(651, "Write", "en{'Product Code/No. Write'}de{'Produktnummer Soll'}", "", false, false)]
        public IACContainerTNet<UInt32> ProductCode { get; set; }

        /// <summary>
        /// Holding register 40002,40003 – float – product temperature, HR2.R
        /// </summary>
        [ACPropertyBindingTarget(652, "Write", "en{'Target Temperature'}de{'Temperatur Soll)'}", "", false, false)]
        public IACContainerTNet<Single> TargetTemperature { get; set; }

        /// <summary>
        /// Holding register 40007,40008 – float – moisture offset, HR7.R
        /// </summary>
        [ACPropertyBindingTarget(653, "Write", "en{'Moisture Offset'}de{'Feuchte Offset'}", "", false, false)]
        public IACContainerTNet<Single> MoistureOffset { get; set; }

        /// <summary>
        /// Holding register 40009,40010 – float – density offset, HR9.R
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

        protected DateTime _LastMeasurementReadyGoneTrue = DateTime.Now;
        protected DateTime _LastMeasurementReadyGoneFalse = DateTime.Now;
        protected DateTime _LastStop = DateTime.Now;
        protected DateTime _LastStart = DateTime.Now;

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
                        if (Status.ValueT.Bit00_MeasurementReady && !prevStatus.Bit00_MeasurementReady)
                            _LastMeasurementReadyGoneTrue = DateTime.Now;
                        else if (!Status.ValueT.Bit00_MeasurementReady && prevStatus.Bit00_MeasurementReady)
                            _LastMeasurementReadyGoneFalse = DateTime.Now;
                    }

                    PAMW4xxxTewsStatus currentStatus = new PAMW4xxxTewsStatus(Status.ValueT.ValueTypeACClass);
                    currentStatus.ValueT = Status.ValueT.ValueT;

                    OnTewsStatusChanged(prevStatus, currentStatus);

                    PreviousStatus = currentStatus;

                    ToggleBits();
                }
                else if (sender == ErrorStatus)
                {
                    if (ErrorStatus.ValueT.Bit03_DeviceError)
                        OnNewAlarmOccurred(ErrorStatus, "Device-Error", true);
                }
            }
        }

        protected virtual void OnTewsStatusChanged(PAMW4xxxTewsStatus prevStatus, PAMW4xxxTewsStatus currentStatus)
        {
        }

        #endregion

        #region Client-Methods
        [ACMethodInteraction("", "en{'Start measurement'}de{'Messung starten'}", 601, true)]
        public virtual void StartMeasurement()
        {
            if (!IsEnabledStartMeasurement())
                return;
            Cmd.ValueT.Bit07_StartStop = true;
            //if (this.ApplicationManager != null)
            //{
            //    this.ApplicationManager.ApplicationQueue.Add(() =>
            //    {
            //        ToggleBits();
            //    });
            //}
            _LastStart = DateTime.Now;
        }

        public virtual bool IsEnabledStartMeasurement()
        {
            return Cmd != null 
                && Status != null 
                && ProductCode.ValueT > 0 
                && !Status.ValueT.Bit04_MeasurementRun 
                && !Cmd.ValueT.Bit07_StartStop
                && (DateTime.Now - _LastStart).TotalSeconds > MaxResponseWaitingTime;
        }

        [ACMethodInteraction("", "en{'Stop measurement'}de{'Messung stoppen'}", 602, true)]
        public virtual void StopMeasurement()
        {
            if (!IsEnabledStopMeasurement())
                return;
            Cmd.ValueT.Bit07_StartStop = true;
            //if (this.ApplicationManager != null)
            //{
            //    this.ApplicationManager.ApplicationQueue.Add(() =>
            //    {
            //        ToggleBits();
            //    });
            //}
            _LastStop = DateTime.Now;
        }

        public virtual bool IsEnabledStopMeasurement()
        {
            return Cmd != null 
                && Status != null 
                && Status.ValueT.Bit04_MeasurementRun
                && !Cmd.ValueT.Bit07_StartStop
                && (DateTime.Now - _LastStop).TotalSeconds > MaxResponseWaitingTime;
        }

        public virtual void SendProductNo(UInt16 productNo)
        {
            ProductCode.ValueT = productNo;
        }


        public override void AcknowledgeAlarms()
        {
            Cmd.ValueT.Bit06_AckAlarm = true;
            base.AcknowledgeAlarms();
        }

        int _TogglePauseCounter = 0;
        public void ToggleBits()
        {
            if (_TogglePauseCounter < 2)
            {
                _TogglePauseCounter++;
                return;
            }
            _TogglePauseCounter = 0;
            if (Cmd.ValueT.Bit07_StartStop)
                Cmd.ValueT.Bit07_StartStop = false;
            if (Cmd.ValueT.Bit06_AckAlarm)
                Cmd.ValueT.Bit06_AckAlarm = false;
        }

        #endregion

        protected override void DumpPropertyList(XmlDocument doc, XmlElement xmlACPropertyList)
        {
            base.DumpPropertyList(doc, xmlACPropertyList);
        }

        #endregion
    }
}
