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
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'PW Containerrequest'}de{'PW Containersuche'}", Global.ACKinds.TPWNodeMethod, Global.ACStorableTypes.Optional, false, PWMethodVBBase.PWClassName, true)]
    public class PWContReqKSE : PWNodeProcessMethod
    {
        public const string PWClassName = "PWContReqKSE";

        #region c´tors
        static PWContReqKSE()
        {
            RegisterExecuteHandler(typeof(PWContReqKSE), HandleExecuteACMethod_PWContReqKSE);
        }

        public PWContReqKSE(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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

        public static bool HandleExecuteACMethod_PWContReqKSE(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PWNodeProcessMethod(out result, acComponent, acMethodName, acClassMethod, acParameter);
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

            if (   ((ACSubStateEnum)pwGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMBatchCancelled)
                || ((ACSubStateEnum)RootPW.CurrentACSubState).HasFlag(ACSubStateEnum.SMBatchCancelled))
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
                    //AcknowledgeAlarms();
                    _WaitOnAlarmCode = null;
                }

                ACMethod paramMethod = null;

                using (ACMonitor.Lock(this.ContextLockForACClassWF))
                {
                    paramMethod = refPAACClassMethod.TypeACSignature();
                }
                PAProcessFunction responsibleFunc = CanStartProcessFunc(module, paramMethod);
                if (responsibleFunc == null)
                {
                    SubscribeToProjectWorkCycle();
                    return;
                }
                GetConfigForACMethod(paramMethod, true);
                UInt16 position = (UInt16) paramMethod.ParameterValueList["RequiredPosition"];
                if (position <= 0)
                {
                    UInt16? position2 = (UInt16?)ACUrlCommand("!GetRequiredPosition");
                    if (position2 == null)
                    {
                        SubscribeToProjectWorkCycle();
                        return;
                    }
                    paramMethod.ParameterValueList["RequiredPosition"] = position2.Value;
                }
                UInt16 containerType = (UInt16)paramMethod.ParameterValueList["ContainerType"];
                if (containerType == 0 || containerType > 2)
                    paramMethod.ParameterValueList["ContainerType"] = (UInt16)2; // 1 Für mischcontainer, 2=Raw mterial

                //paramMethod.ParameterValueList["ContainerType"] = (UInt16)1; // 1 Für mischcontainer, 2=Raw mterial
                OnFillKSEMethod(paramMethod);

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
                UpdateCurrentACMethod();
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

        protected virtual void OnFillKSEMethod(ACMethod acMethod)
        {
        }

        private DateTime? _WaitOnAlarmCode = null;
        public override void SMCompleted()
        {
            if (_LastCallbackResult != null)
            {
                ACValue acValue = _LastCallbackResult.GetACValue("AlarmCode");
                if (acValue != null)
                {
                    ContReqAlarmCode alarmCode = acValue.ValueT<ContReqAlarmCode>();
                    if (alarmCode != ContReqAlarmCode.NoAlarm)
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
                    else
                        AcknowledgeAlarms();
                }
            }
            base.SMCompleted();
        }

        public override void AcknowledgeAlarms()
        {
            _WaitOnAlarmCode = null;
            base.AcknowledgeAlarms();
        }

        [ACMethodInfo("", "en{'Get KSE required position'}de{'Ermittle KSE-Anforderungsziel'}", 500)]
        public virtual UInt16? GetRequiredPosition()
        {
            if (RootPW == null)
                return null;
            PWContFillKSE fillingNode = RootPW.FindChildComponents<PWContFillKSE>(c => c is PWContFillKSE).FirstOrDefault();
            if (fillingNode == null)
            {
                // TODO: AlarmMessage
                return null;
            }
            return fillingNode.GetRequiredPosition();
        }
#endregion

#endregion
    }
}
