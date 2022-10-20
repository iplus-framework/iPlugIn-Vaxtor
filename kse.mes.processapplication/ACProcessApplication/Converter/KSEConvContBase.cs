using gip.core.autocomponent;
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.communication;
using gip.mes.processapplication;
using System.Threading;

namespace kse.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'KSEConvContBase'}de{'KSEConvContBase'}", Global.ACKinds.TACAbstractClass, Global.ACStorableTypes.NotStorable, false, false)]
    public abstract class KSEConvContBase : PAFuncStateConvBase
    {
        #region ctor's
        public KSEConvContBase(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            lock (_LifeSignalLock)
            {
                if (_LifeSignalThread == null)
                {
                    _LifeSignalThread = new ACThread(RunWorkCycle);
                    _LifeSignalThread.Name = "LifeSignalThread:" + this.ACIdentifier + ";RunWorkCycle();";
                    _LifeSignalThread.Start();
                }
            }
        }

        private static bool? _IsConverterDeactivated = null;
        public static bool IsConverterDeactivated
        {
            get
            {
                if (_IsConverterDeactivated.HasValue)
                    return _IsConverterDeactivated.Value;
                _IsConverterDeactivated = false;
                try
                {
                    ProcessConfiguration processConfig = (ProcessConfiguration)CommandLineHelper.ConfigCurrentDir.GetSection("Process/ProcessConfiguration");
                    if (processConfig != null)
                    {
                        if (processConfig.DeactivateProcessConverter)
                        {
                            _IsConverterDeactivated = processConfig.DeactivateProcessConverter;
                        }
                    }
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    if (gip.core.datamodel.Database.Root != null && gip.core.datamodel.Database.Root.Messages != null && 
                                                                          gip.core.datamodel.Database.Root.InitState == ACInitState.Initialized)
                        gip.core.datamodel.Database.Root.Messages.LogException("KSEConvContBase", "IsConverterDeactivated", msg);
                }
                return _IsConverterDeactivated.Value;
            }
        }


        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            if (!IsConverterDeactivated)
            {
                StatusAsTargetProp.ValueUpdatedOnReceival += Status_PropertyChanged;
                CommandAsTargetProp.ValueUpdatedOnReceival += Command_PropertyChanged;
                LifeSignalCycle += ApplicationManager_ProjectWorkCycleR1sec;
            }
            return true;
        }

        public override bool ACPostInit()
        {
            if (!IsConverterDeactivated)
            {
                LifeSignalAcknowledgeAsTargetProp.ValueUpdatedOnReceival += LifeSignalAcknowledge_PropertyChanged;
            }

            return base.ACPostInit();
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            lock (_LifeSignalLock)
            {
                if (_LifeSignalThread != null)
                {
                    if (_ShutdownEvent != null && _ShutdownEvent.SafeWaitHandle != null && !_ShutdownEvent.SafeWaitHandle.IsClosed)
                        _ShutdownEvent.Set();
                    if (!_LifeSignalThread.Join(10000))
                        _LifeSignalThread.Abort();

                    _LifeSignalThread = null;
                    _ShutdownEvent = null;
                }
            }

            if (!IsConverterDeactivated)
            {
                StatusAsTargetProp.ValueUpdatedOnReceival -= Status_PropertyChanged;
                CommandAsTargetProp.ValueUpdatedOnReceival -= Command_PropertyChanged;
                LifeSignalAcknowledgeAsTargetProp.ValueUpdatedOnReceival -= LifeSignalAcknowledge_PropertyChanged;
                LifeSignalCycle -= ApplicationManager_ProjectWorkCycleR1sec;
            }
            return base.ACDeInit(deleteACClassTask);
        }

        private void RunWorkCycle()
        {
            try
            {
                while (!_ShutdownEvent.WaitOne(200, false))
                {
                    _LifeSignalThread.StartReportingExeTime();
                    if (LifeSignalCycle != null)
                        LifeSignalCycle(this, new EventArgs());
                    _LifeSignalThread.StopReportingExeTime();
                }
            }
            catch (ThreadAbortException ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                if (gip.core.datamodel.Database.Root != null && gip.core.datamodel.Database.Root.Messages != null &&
                                                                      gip.core.datamodel.Database.Root.InitState == ACInitState.Initialized)
                    gip.core.datamodel.Database.Root.Messages.LogException("KSEConvContBase", "RunWorkCycle", msg);
            }
        }
        #endregion

        #region Properties

        private static object _LifeSignalLock = new object();
        private static ManualResetEvent _ShutdownEvent = new ManualResetEvent(false);
        private static ACThread _LifeSignalThread;
        public static event EventHandler LifeSignalCycle;

      
        [Flags]
        enum ConvInitState
        {
            None = 0x0,
            RequestInitialized = 0x1,
            ResponseInitialized = 0x2,
            ACStateRestored = 0x4,
        }
        ConvInitState _InitState = ConvInitState.None;
        public bool IsACStateRestored
        {
            get
            {
                return (_InitState & ConvInitState.ACStateRestored) == ConvInitState.ACStateRestored;
            }
        }

        public override bool IsReadyForSending
        {
            get
            {
                if (!this.Root.Initialized)
                    return false;
                if (IsSimulationOn)
                    return true;
                bool isReadyForSending = true;

                // TOD: Aktiviere wenn GIP-SPS verfügbar
                //if (!IsReadyForWritingExtern.ValueT)
                //    isReadyForSending = false;
                if (isReadyForSending && this.Session != null)
                {
                    ACSession acSession = this.Session as ACSession;
                    if (acSession != null && !acSession.IsReadyForWriting)
                        isReadyForSending = false;
                    else if (acSession == null && !(bool)this.Session.ACUrlCommand("IsReadyForWriting"))
                        isReadyForSending = false;
                }
                if (isReadyForSending && !IsACStateRestored)
                    isReadyForSending = false;
                return isReadyForSending;
            }
        }

        public override bool IsReadyForReading
        {
            get
            {
                return IsReadyForSending;
            }
        }

        #region Read-Values from PLC
        [ACPropertyBindingTarget(99, "IsReadyForWritingExtern", "en{'IsReadyForWritingExtern'}de{'IsReadyForWritingExtern'}", "", false, false, RemotePropID=17)]
        public IACContainerTNet<Boolean> IsReadyForWritingExtern { get; set; }
        [ACPropertyBindingSource(100, "Error", "en{'Function error'}de{'Funktionsfehler'}", "", false, false)]
        public IACContainerTNet<PANotifyState> FunctionError { get; set; }

        [ACPropertyBindingTarget(101, "LifeSignalAcknowledge", "en{'LifeSignalAcknowledge'}de{'LifeSignalAcknowledge'}", "", false, false, RemotePropID=18)]
        public IACContainerTNet<UInt16> LifeSignalAcknowledge { get; set; }
        protected IACPropertyNetTarget LifeSignalAcknowledgeAsTargetProp
        {
            get
            {
                return (IACPropertyNetTarget)this.LifeSignalAcknowledge;
            }
        }
        protected UInt16 _LifeSignalAcknowledgeValue
        {
            get { return LifeSignalAcknowledge.ValueT; }
        }

        [ACPropertyBindingTarget(102, "Status", "en{'Status'}de{'Status'}", "", false, false, RemotePropID=19)]
        public IACContainerTNet<ContainerStatus> Status { get; set; }
        protected IACPropertyNetTarget StatusAsTargetProp
        {
            get
            {
                return (IACPropertyNetTarget)this.Status;
            }
        }
        protected ContainerStatus StatusValue
        {
            get { return Status.ValueT; }
        }

        #endregion

        #region Write-Values from PLC

        [ACPropertyBindingTarget(1, "LifeSignalRequest", "en{'LifeSignalRequest'}de{'LifeSignalRequest'}", "", false, false, RemotePropID=20)]
        public IACContainerTNet<UInt16> LifeSignalRequest { get; set; }
        protected virtual IACPropertyNetTarget LifeSignalRequestAsTargetProp
        {
            get
            {
                return (IACPropertyNetTarget)LifeSignalRequest;
            }
        }
        protected virtual UInt16 LifeSignalRequestValue
        {
            get { return LifeSignalRequest.ValueT; }
            set { LifeSignalRequest.ValueT = value; }
        }

        DateTime? _LastToggleTime = null;
        void ApplicationManager_ProjectWorkCycleR1sec(object sender, EventArgs e)
        {            
            if (IsReadyForSending)
            {
                if (_LastToggleTime.HasValue)
                {
                    if ((LifeSignalRequestValue != _LifeSignalAcknowledgeValue)
                        && ((DateTime.Now - _LastToggleTime.Value).TotalSeconds > 2))
                    {
                        // TODO Alarm
                        if ((DateTime.Now - _LastToggleTime.Value).TotalSeconds > 3)
                        {
                            LifeSignalRequestValue = _LifeSignalAcknowledgeValue;
                        }
                    }
                    else if (LifeSignalRequestValue == _LifeSignalAcknowledgeValue)
                    {
                        _LastToggleTime = null;
                    }
                }
                if (!_LastToggleTime.HasValue)
                {
                    if (LifeSignalRequestValue == 0)
                        LifeSignalRequestValue = 1;
                    else
                        LifeSignalRequestValue = 0;
                    _LastToggleTime = DateTime.Now;
                }
            }
        }


        [ACPropertyBindingTarget(2, "Command", "en{'Command'}de{'Command'}", "", false, false, RemotePropID=21)]
        public IACContainerTNet<ContainerCommand> Command { get; set; }
        protected virtual IACPropertyNetTarget CommandAsTargetProp
        {
            get
            {
                return (IACPropertyNetTarget)Command;
            }
        }

        protected virtual ContainerCommand CommandValue
        {
            get { return Command.ValueT; }
            set { Command.ValueT=value; }
        }

        #endregion
     
        #endregion

        #region overridden methods

        public override ACStateEnum GetNextACState(PAProcessFunction sender, string transitionMethod = "")
        {
            ACStateEnum defaultNextState = PAFuncStateConvBase.GetDefaultNextACState(sender.CurrentACState, transitionMethod);
            if (!IsSimulationOn)
            {
                if (String.IsNullOrEmpty(transitionMethod)) 
                    return ACState.ValueT;
                if (!IsReadyForSending)
                {

                    OnNewAlarmOccurred(FunctionError, "Session Extern is not ready for writing");
                    FunctionError.ValueT = PANotifyState.AlarmOrFault;
                    return ACState.ValueT;
                }

                // Falls Reset-Befehl vom Benutzer angeklickt wird weil Containersteuerung keinen Start-Befehl angenommen hat,
                // muss der Variobatch-Zustand auf Idle zurückgeführt werden:
                if (String.IsNullOrEmpty(transitionMethod)
                    && transitionMethod == ACStateConst.TMReset
                    && ACState.ValueT != ACStateEnum.SMIdle
                    && StatusValue == ContainerStatus.Idle)
                {
                    return ACStateEnum.SMResetting;
                }
                //else if (defaultNextState == Const.SMIdle
                //    && StatusValue == ContainerStatus.Idle
                //    && ACState.ValueT == Const.SMResetting)
                //{
                //    return Const.SMIdle;
                //}
                else
                {
                    TranslateACStateToCommand(defaultNextState);
                    ACStateEnum stateInPLC = TranslateToACStateFromStatus();
                    if (stateInPLC == defaultNextState)
                        return stateInPLC;
                    return ACState.ValueT;
                }
            }
            return defaultNextState;
        }

        public override bool IsEnabledTransition(PAProcessFunction sender, string transitionMethod)
        {
            return PAFuncStateConvBase.IsEnabledTransitionDefault(sender.CurrentACState, transitionMethod, sender);
            //throw new NotImplementedException();
        }

        public override gip.core.datamodel.MsgWithDetails SendACMethod(PAProcessFunction sender, gip.core.datamodel.ACMethod acMethod)
        {
            if (Session == null)
            {
                MsgWithDetails msg = new MsgWithDetails() { Source = this.GetACUrl(), MessageLevel = eMsgLevel.Error, ACIdentifier = "SendACMethod()", Message = "Session is null" };
                return msg;
            }
            if (!IsReadyForSending)
            {
                MsgWithDetails msg = new MsgWithDetails() 
                { 
                        Source = this.GetACUrl(), 
                        MessageLevel = eMsgLevel.Error, 
                        ACIdentifier = "SendACMethod()", 
                        Message = string.Format("Session is not ready for writing. Method name: {0}", acMethod == null ? "-" : acMethod.ACIdentifier)
                };
                return msg;
            }
            bool sended = false;

            sended = SendParams(acMethod);

            if (!sended)
            {
                MsgWithDetails msg = new MsgWithDetails() { 
                    Source = this.GetACUrl(), 
                    MessageLevel = eMsgLevel.Error, 
                    ACIdentifier = "SendACMethod()", 
                    Message = string.Format("ACMethod was not sended. Method name: {0}. Please check if connection is established.", acMethod != null ? acMethod.ACIdentifier : "-")
                };
                return msg;
            }
            return null;
        }

        public override PAProcessFunction.CompleteResult ReceiveACMethodResult(PAProcessFunction sender, ACMethod acMethod, out MsgWithDetails msg)
        {
            msg = null;
            return PAProcessFunction.CompleteResult.Succeeded;
        }

        protected override void ModelProperty_ValueUpdatedOnReceival(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            return;
        }
   
        protected virtual void Status_PropertyChanged(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.AfterBroadcast)
                return;
            if ((e.ValueEvent.Sender == EventRaiser.Source) && (e.ValueEvent.EventType == EventTypes.ValueChangedInSource))
            {
                _InitState |= ConvInitState.ResponseInitialized;               

                if (!((_InitState & ConvInitState.RequestInitialized) == ConvInitState.RequestInitialized))
                    return;
                _InitState |= ConvInitState.ACStateRestored;
                SyncACStateFromPLC(1, sender, e, phase);
            }
        }

        protected virtual void LifeSignalAcknowledge_PropertyChanged(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.AfterBroadcast)
                return;
            if ((e.ValueEvent.Sender == EventRaiser.Source) && (e.ValueEvent.EventType == EventTypes.ValueChangedInSource))
            {
                //if (_LifeSignalAcknowledgeValue >= 1)
                //    _LifeSignalRequestValue = 0;
                //else _LifeSignalRequestValue = 1;                
            }
        }

        private void SyncACStateFromPLC(short callerID, object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            ACStateEnum newACState = TranslateToACStateFromStatus();
            (ACState as ACPropertyNetServerBase<ACStateEnum>).ChangeValueServer(ACState.ForceBroadcast, e.ValueEvent.InvokerInfo, newACState);
            //ACState.ValueT = newACState;           
        }

        //private bool _LockResend_ReqRunState = false;
        protected virtual void Command_PropertyChanged(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.AfterBroadcast)
                return;
            if ((e.ValueEvent.Sender == EventRaiser.Source) && (e.ValueEvent.EventType == EventTypes.ValueChangedInSource))
            {
                //_LockResend_ReqRunState = true;
                _InitState |= ConvInitState.RequestInitialized;
                if (!((_InitState & ConvInitState.ResponseInitialized) == ConvInitState.ResponseInitialized))
                    return;
                if (!((_InitState & ConvInitState.ACStateRestored) == ConvInitState.ACStateRestored))
                {
                    _InitState |= ConvInitState.ACStateRestored;
                    SyncACStateFromPLC(2, sender, e, phase);
                }

                //_LockResend_ReqRunState = false;
            }
        }

        protected ACStateEnum TranslateToACStateFromStatus()
        {
            ACStateEnum translatedState = ACStateEnum.SMIdle;
            if (StatusValue == ContainerStatus.Aborted)
            {
                translatedState = ACStateEnum.SMAborted;
            }
            else if (StatusValue == ContainerStatus.Aborting)
            {
                translatedState = ACStateEnum.SMAborting;
            }
            else if (StatusValue == ContainerStatus.Complete)
            {
                translatedState = ACStateEnum.SMCompleted;
            }
            else if (StatusValue == ContainerStatus.Held)
            {
                translatedState = ACStateEnum.SMHeld;
            }
            else if (StatusValue == ContainerStatus.Holding)
            {
                translatedState = ACStateEnum.SMHolding;
            }
            else if (StatusValue == ContainerStatus.Idle)
            {
                translatedState = ACStateEnum.SMIdle;
            }
            else if (StatusValue == ContainerStatus.Resetting)
            {
                translatedState = ACStateEnum.SMResetting;
            }
            else if (StatusValue == ContainerStatus.Restarting)
            {
                translatedState = ACStateEnum.SMRestarting;
            }
            else if (StatusValue == ContainerStatus.Running)
            {
                translatedState = ACStateEnum.SMRunning;
            }
            else if (StatusValue == ContainerStatus.Stopped)
            {
                translatedState = ACStateEnum.SMStopped;
            }
            else if (StatusValue == ContainerStatus.Stopping)
            {
                translatedState = ACStateEnum.SMStopping;
            }
            return translatedState;
        }

        protected void TranslateACStateToCommand(ACStateEnum nextACState)
        {
            bool changed = false;
            switch (nextACState)
            {
                case ACStateEnum.SMRunning:
                    if (!((CommandValue == ContainerCommand.Start) || (CommandValue == ContainerCommand.Restart)))
                    {
                        CommandValue = ContainerCommand.Start;
                        changed = true;
                    }
                    break;
                case ACStateEnum.SMPausing:
                case ACStateEnum.SMHolding:
                    if (!(CommandValue == ContainerCommand.Hold))
                    {
                        CommandValue = ContainerCommand.Hold;
                        changed = true;
                    }
                    break;
                case ACStateEnum.SMResuming:
                case ACStateEnum.SMRestarting:
                    if (!(CommandValue == ContainerCommand.Restart))
                    {
                        CommandValue = ContainerCommand.Restart;
                        changed = true;
                    }
                    break;
                case ACStateEnum.SMAborting:
                    if (!(CommandValue == ContainerCommand.Abort))
                    {
                        CommandValue = ContainerCommand.Abort;
                        changed = true;
                    }
                    break;
                case ACStateEnum.SMStopping:
                    if (!(CommandValue == ContainerCommand.Stop))
                    {
                        CommandValue = ContainerCommand.Stop;
                        changed = true;
                    }
                    break;
                case ACStateEnum.SMResetting:
                    if (!(CommandValue == ContainerCommand.Reset))
                    {
                        CommandValue = ContainerCommand.Reset;
                        changed = true;
                    }
                    break;
                case ACStateEnum.SMIdle:
                    if (!(CommandValue == ContainerCommand.Reset))
                    {
                        CommandValue = ContainerCommand.Reset;
                        changed = true;
                    }
                    break;
            }
            if (changed)
            {
                try
                {
                    Messages.LogDebug(this.GetACUrl(), "TranslateACStateToCommand()", String.Format("0x{0:x}", CommandValue));
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    if (gip.core.datamodel.Database.Root != null && gip.core.datamodel.Database.Root.Messages != null &&
                                                                          gip.core.datamodel.Database.Root.InitState == ACInitState.Initialized)
                        gip.core.datamodel.Database.Root.Messages.LogException("KSEConvContBase", "TranslateACStateToCommand", msg);
                }
            }
        }

        public override void AcknowledgeAlarms()
        {
            if (!IsEnabledAcknowledgeAlarms())
                return;
             base.AcknowledgeAlarms();
        }

        public virtual bool SendParams(gip.core.datamodel.ACMethod acMethod)
        {
            return true;
        }
        #endregion

        protected override bool OnParentServerPropertyFound(IACPropertyNetServer parentProperty)
        {
            switch (parentProperty.ACIdentifier)
            {
                case "FunctionError":
                    FunctionError = parentProperty as IACContainerTNet<PANotifyState>;
                    return true;
                default:
                    break;

            }
            bool result = base.OnParentServerPropertyFound(parentProperty);
            return result;
        }
    }
}
