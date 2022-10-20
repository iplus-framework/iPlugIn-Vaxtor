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
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'PW Containertransport'}de{'PW Containertransport'}", Global.ACKinds.TPWNodeMethod, Global.ACStorableTypes.Optional, false, PWMethodVBBase.PWClassName, true)]
    public class PWContTranspKSE : PWNodeProcessMethod
    {
        public const string PWClassName = "PWContTranspKSE";

        #region c´tors
        static PWContTranspKSE()
        {
            RegisterExecuteHandler(typeof(PWContTranspKSE), HandleExecuteACMethod_PWContTranspKSE);
        }

        public PWContTranspKSE(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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

        #region Properties
        public PWMethodProduction ParentPWMethodProduction
        {
            get
            {
                return ParentRootWFNode as PWMethodProduction;
            }
        }
        #endregion

        #region Methods

        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PWContTranspKSE(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
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
                    AcknowledgeAlarms();
                    _WaitOnAlarmCode = null;
                }

                if (ParentPWMethodProduction != null
                    && (ParentPWMethodProduction.CurrentProdOrderBatch == null
                         || !ParentPWMethodProduction.CurrentProdOrderBatch.ProdOrderBatchPlanID.HasValue))
                {
                    // Error50067:Batchplan not found
                    Msg msg = new Msg(this, eMsgLevel.Error, PWClassName, "SMStarting(1)", 133, "Error50067");

                    if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                        Messages.LogError(this.GetACUrl(), "SMStarting(1)", msg.Message);
                    OnNewAlarmOccurred(ProcessAlarm, msg, true);
                    SubscribeToProjectWorkCycle();
                    return;
                }


                PWContDisKSE isTransportToDisStation = null;
                PWContFillKSE isTransportToFillingStation = this.FindSuccessors<PWContFillKSE>(false, c => c is PWContFillKSE, null, 1).FirstOrDefault();
                if (isTransportToFillingStation == null)
                    isTransportToDisStation = this.FindSuccessors<PWContDisKSE>(false, c => c is PWContDisKSE, null, 1).FirstOrDefault();

                if (isTransportToFillingStation == null && isTransportToDisStation == null)
                {
                    // TODO: Meldung: Programmfehler, darf nicht vorkommen
                    SubscribeToProjectWorkCycle();
                    return;
                }

                ACMethod paramMethod = refPAACClassMethod.TypeACSignature();
                PAProcessFunction responsibleFunc = CanStartProcessFunc(module, paramMethod);
                if (responsibleFunc == null)
                {
                    SubscribeToProjectWorkCycle();
                    return;
                }
                GetConfigForACMethod(paramMethod, true);
                paramMethod["ActionRequest"] = ContTranspAction.Positionieren;

                PWContGroupKSE pwContGroupKSE = ParentPWGroup as PWContGroupKSE;
                if (isTransportToFillingStation != null)
                {
                    UInt16? position = isTransportToFillingStation.GetRequiredPosition();
                    if (position == null)
                    {
                        SubscribeToProjectWorkCycle();
                        return;
                    }
                    else
                        paramMethod["RequiredPositionRequest"] = position.Value;
                }
                else //if (isTransportToDisStation)
                {
                    UInt16? position = isTransportToDisStation.GetKSEDestination();
                    if (!position.HasValue)
                    {
                        SubscribeToProjectWorkCycle();
                        return;
                    }
                    else
                        paramMethod["RequiredPositionRequest"] = position.Value;
                }

                // TODO: Generiere eindeutige ID für KSE
                //if (ParentPWMethodProduction != null)
                //    paramMethod["OccupyId"] = Convert.ToUInt16(ParentPWMethodProduction.CurrentProdOrderBatch.BatchSeqNo);
                //else
                //    paramMethod["OccupyId"] = (UInt16) 999;

                if (pwContGroupKSE != null)
                {
                    UInt16 occupyID = 0;
                    pwContGroupKSE.GetOccupyID(out occupyID);
                    paramMethod["OccupyId"] = occupyID;
                }

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
                if (!IsTaskStarted(module.TaskInvocationPoint.AddTask(paramMethod, this)))
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

        private DateTime? _WaitOnAlarmCode = null;
        public override void SMCompleted()
        {
            if (_LastCallbackResult != null)
            {
                ACValue acValue = _LastCallbackResult.GetACValue("AlarmCode");
                if (acValue != null)
                {
                    ContTranspAlarmCode alarmCode = acValue.ValueT<ContTranspAlarmCode>();
                    if (alarmCode != ContTranspAlarmCode.KeinAlarm)
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

#endregion

    }
}
