using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using kse.mes.processapplication;

namespace gip.mes.processapplication
{
    /// <summary>
    /// Process-Knoten zur implementierung eines untergeordneten (asynchronen) ACClassMethod-Aufruf auf die Model-Welt
    /// 
    /// Methoden zur Steuerung von außen: 
    /// -Start()    Starten des Processes
    ///
    /// Mögliche ACState:
    /// SMIdle      (Definiert in ACComponent)
    /// SMStarting (Definiert in PWNode)
    /// </summary>
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'PW Containerfill'}de{'PW Containerbefüllen'}", Global.ACKinds.TPWNodeMethod, Global.ACStorableTypes.Optional, false, PWMethodVBBase.PWClassName, true)]
    public class PWContFillKSE : PWLoad
    {
        new public const string PWClassName = "PAFContFillKSE";

        #region c´tors
        static PWContFillKSE()
        {
            RegisterExecuteHandler(typeof(PWContFillKSE), HandleExecuteACMethod_PWContFillKSE);
        }

        public PWContFillKSE(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            _WaitOnAlarmCode = null;
            return base.ACDeInit(deleteACClassTask);
        }
        #endregion


        #region Methods

        #region Execute-Helper-Handlers
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "GetRequiredPosition":
                    result = GetRequiredPosition();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public static bool HandleExecuteACMethod_PWContFillKSE(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PWLoad(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }

        #endregion


        #region ACState
        public override void Reset()
        {
            _WaitOnAlarmCode = null;
            base.Reset();
        }

        public override void SMStarting()
        {
            var pwGroup = ParentPWGroup;
            if (pwGroup == null) // Is null when Service-Application is shutting down
            {
                if (this.InitState == ACInitState.Initialized)
                    Messages.LogError(this.GetACUrl(), "SMStarting()", "ParentPWGroup is null");
                return;
            }

            if (  ((ACSubStateEnum)pwGroup.CurrentACSubState == ACSubStateEnum.SMBatchCancelled)
                || ((ACSubStateEnum)RootPW.CurrentACSubState == ACSubStateEnum.SMBatchCancelled))
            {
                CurrentACState = ACStateEnum.SMCompleted;
                return;
            }

            ACClassMethod refPAACClassMethod = null;
            if (pwGroup != null && this.ContentACClassWF != null)
            {

                using (ACMonitor.Lock(this.ContextLockForACClassWF))
                {
                    refPAACClassMethod = this.ContentACClassWF.RefPAACClassMethod;
                }
            }

            if (refPAACClassMethod != null)
            {
                PAProcessModule module = null;
                if (pwGroup.NeedsAProcessModule && (ACOperationMode == ACOperationModes.Live || ACOperationMode == ACOperationModes.Simulation))
                    module = pwGroup.AccessedProcessModule;
                // Testmode
                else
                    module = pwGroup.ProcessModuleForTestmode;

                if (module == null)
                {
                    // TODO: Meldung: Programmfehler, darf nicht vorkommen
                    return;
                }

                if (_WaitOnAlarmCode.HasValue && _WaitOnAlarmCode.Value > DateTime.Now)
                {
                    SubscribeToProjectWorkCycle();
                    return;
                }
                else if (_WaitOnAlarmCode.HasValue)
                {
                    AcknowledgeAlarms();
                    _WaitOnAlarmCode = null;
                }

                ACMethod paramMethod = refPAACClassMethod.TypeACSignature();
                PAProcessFunction responsibleFunc = CanStartProcessFunc(module, paramMethod);
                if (responsibleFunc == null)
                {
                    SubscribeToProjectWorkCycle();
                    return;
                }
                GetConfigForACMethod(paramMethod, true);
                paramMethod["Source"] = (Int16)1; // TODO: Quelle eintragen

                if (!paramMethod.IsValid())
                {
                    Msg msg = new Msg
                    {
                        Source = GetACUrl(),
                        MessageLevel = eMsgLevel.Error,
                        ACIdentifier = "SMStarting(1)",
                        Message = paramMethod.ValidMessage.Message
                    };

                    if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                        Messages.LogError(this.GetACUrl(), "SMStarting(1)", msg.Message);
                    OnNewAlarmOccurred(ProcessAlarm, msg.Message, true);
                    SubscribeToProjectWorkCycle();
                    return;
                }

                RecalcTimeInfo();
                CreateNewProgramLog(paramMethod);
                _ExecutingACMethod = paramMethod;

#if DEBUG
                module.TaskInvocationPoint.ClearMyInvocations(this);
#endif
                if (!module.TaskInvocationPoint.AddTask(paramMethod, this))
                {
                    Msg msg = new Msg
                    {
                        Source = GetACUrl(),
                        MessageLevel = eMsgLevel.Error,
                        ACIdentifier = "SMStarting(2)",
                        Message = paramMethod.ValidMessage.Message
                    };

                    if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                        Messages.LogError(this.GetACUrl(), "SMStarting(2)", msg.Message);
                    OnNewAlarmOccurred(ProcessAlarm, msg.Message, true);
                    SubscribeToProjectWorkCycle();
                    return;
                }
                AcknowledgeAlarms();
                UnSubscribeToProjectWorkCycle();
            }

            // Falls module.AddTask synchron ausgeführt wurde, dann ist der Status schon weiter
            if (IsACStateMethodConsistent(ACStateEnum.SMStarting) < ACStateCompare.WrongACStateMethod)
            {
                CurrentACState = ACStateEnum.SMRunning;
                //PostExecute(Const.SMStarting);
            }
        }

        private DateTime? _WaitOnAlarmCode = null;
        public override void SMCompleted()
        {
            if (_LastCallbackResult != null)
            {
                ACValue acValue = _LastCallbackResult.GetACValue("AlarmCode");
                if (acValue != null)
                {
                    ContFillAlarmCode alarmCode = acValue.ValueT<ContFillAlarmCode>();
                    if (alarmCode != ContFillAlarmCode.KeinAlarm)
                    {
                        Msg msg = new Msg
                        {
                            Source = GetACUrl(),
                            MessageLevel = eMsgLevel.Error,
                            ACIdentifier = "SMCompleted(1)",
                            Message = alarmCode.ToString()
                        };

                        if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                            Messages.LogError(this.GetACUrl(), "SMCompleted(1)", msg.Message);
                        OnNewAlarmOccurred(ProcessAlarm, msg.Message, true);

                        _WaitOnAlarmCode = DateTime.Now.AddMinutes(1);
                        CurrentACState = ACStateEnum.SMStarting;
                        return;
                    }
                }
            }
            base.SMCompleted();
        }

        public override void AcknowledgeAlarms()
        {
            _WaitOnAlarmCode = null;
            base.AcknowledgeAlarms();
        }

#endregion


        [ACMethodInfo("", "en{'Get KSE required position'}de{'Ermittle KSE-Anforderungsziel'}", 500)]
        public virtual UInt16? GetRequiredPosition()
        {
            if (RootPW == null)
                return null;
            if (ParentPWGroup == null)
            {
                // TODO: AlarmMessage
                return null;
            }
            if (ParentPWGroup.AccessedProcessModule == null)
            {
                // TODO: AlarmMessage
                return null;
            }
            return System.Convert.ToUInt16(ParentPWGroup.AccessedProcessModule.RouteItemIDAsNum);
        }
#endregion
    }
}
