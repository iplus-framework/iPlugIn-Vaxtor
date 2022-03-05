using gip.core.autocomponent;
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.mes.processapplication;
using gip.mes.datamodel;
using gip.bso.manufacturing;
using gip.mes.facility;
using System.Xml;

namespace kse.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'PW Discharging KSE'}de{'PW Entleeren KSE'}", Global.ACKinds.TPWNodeMethod, Global.ACStorableTypes.Optional, false, PWMethodVBBase.PWClassName, true)]
    public class PWDischargingKSE : PWDischarging
    {
        public new const string PWClassName = "PWDischargingKSE";

        #region c´tors
        static PWDischargingKSE()
        {
            ACMethod method;
            method = new ACMethod(ACStateConst.SMStarting);
            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();
            method.ParameterValueList.Add(new ACValue("SkipIfNoComp", typeof(bool), false, Global.ParamOption.Required));
            paramTranslation.Add("SkipIfNoComp", "en{'Skip if no components dosed'}de{'Überspringe wenn keine Komponente dosiert'}");
            method.ParameterValueList.Add(new ACValue("CleaningCycle", typeof(short), 0, Global.ParamOption.Required));
            paramTranslation.Add("CleaningCycle", "en{'Cleaning after x cycles'}de{'Reiningung nach x Zyklen'}");
            method.ParameterValueList.Add(new ACValue("PositioningDestination", typeof(UInt16), 0, Global.ParamOption.Required));
            paramTranslation.Add("PositioningDestination", "en{'Positioning destination on wait'}de{'Entleerposition bei Warten'}");
            method.ParameterValueList.Add(new ACValue("CleaningDest", typeof(UInt16), 1003, Global.ParamOption.Required));
            paramTranslation.Add("CleaningDest", "en{'Cleanining postion'}de{'Reinigungsposition'}");
            method.ParameterValueList.Add(new ACValue("InterlockACUrl", typeof(string), null, Global.ParamOption.Optional));
            paramTranslation.Add("InterlockACUrl", "en{'Interlock with Workflow-ACUrl'}de{'Verriegelung mit Workflow-ACUrl'}");

            var wrapper = new ACMethodWrapper(method, "en{'Configuration'}de{'Konfiguration'}", typeof(PWDischargingKSE), paramTranslation, null);
            ACMethod.RegisterVirtualMethod(typeof(PWDischargingKSE), ACStateConst.SMStarting, wrapper);
            RegisterExecuteHandler(typeof(PWDischargingKSE), HandleExecuteACMethod_PWDischargingKSE);
        }


        public PWDischargingKSE(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
            //_PredecessorsChecked = false;
            _MovingToDisPosDone = false;
            _TriesWaitingOnTarget = 0;
            _SMStartingLock = false;
            _DischargingStarted = false;
            return base.ACDeInit(deleteACClassTask);
        }

        public override void Recycle(IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
        {
            //_PredecessorsChecked = false;
            _MovingToDisPosDone = false;
            _TriesWaitingOnTarget = 0;
            _DischargingStarted = false;
            _SMStartingLock = false;
            base.Recycle(content, parentACObject, parameter, acIdentifier);
        }

        #endregion

        #region Properties
        protected short CleaningCycle
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("CleaningCycle");
                    if (acValue != null)
                    {
                        return acValue.ParamAsInt16;
                    }
                }
                return 0;
            }
        }

        [ACPropertyBindingSource(601, "", "en{'In cleaning cycle'}de{'Im Reinigungszyklus'}", "", true, true, DefaultValue = false)]
        public IACContainerTNet<bool> InCleaningCycle { get; set; }


        protected UInt16 PositioningDestination
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("PositioningDestination");
                    if (acValue != null)
                    {
                        return acValue.ParamAsUInt16;
                    }
                }
                return 0;
            }
        }

        protected UInt16 CleaningDest
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("CleaningDest");
                    if (acValue != null)
                    {
                        return acValue.ParamAsUInt16;
                    }
                }
                return 1003;
            }
        }

        public string InterlockACUrl
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("InterlockACUrl");
                    if (acValue != null)
                    {
                        return acValue.ParamAsString;
                    }
                }
                return null;
            }
        }


        [ACPropertyBindingSource(602, "", "en{'Moves to discharging position'}de{'Geht zur Entleerposition'}", "", true, true, DefaultValue = false)]
        public IACContainerTNet<bool> IsMovingToDisPos { get; set; }

        private bool _MovingToDisPosDone = false;
        private int _TriesWaitingOnTarget = 0;
        private bool _DischargingStarted = false;

        #endregion

        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PWDischargingKSE(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PWDischarging(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion

        #region ACState-Methods
        public override void SMIdle()
        {
            //_PredecessorsChecked = false;
            InCleaningCycle.ValueT = false;
            IsMovingToDisPos.ValueT = false;
            _MovingToDisPosDone = false;
            _DischargingStarted = false;
            _TriesWaitingOnTarget = 0;
            base.SMIdle();
        }

        private bool _SMStartingLock = false;
        //private bool _PredecessorsChecked = false;
        [ACMethodState("en{'Executing'}de{'Ausführend'}", 20, true)]
        public override void SMStarting()
        {
            if (_SMStartingLock)
                return;
            //if (!_PredecessorsChecked)
            if (SkipIfNoComp)
            {
                //_PredecessorsChecked = true;
                bool hasRunSomeDosings = false;
                // TODO: Test if new Function works:
                List<PWDosing> previousDosings = PWDosing.FindPreviousDosingsInPWGroup<PWDosing>(this);
                if (previousDosings != null)
                    hasRunSomeDosings = previousDosings.Where(c => c.HasRunSomeDosings).Any();
                if (!hasRunSomeDosings)
                {
                    if (CurrentACState == ACStateEnum.SMStarting)
                        CurrentACState = ACStateEnum.SMCompleted;
                    return;
                }
            }

            if (!IsMovingToDisPos.ValueT)
            {
                if (!String.IsNullOrEmpty(InterlockACUrl))
                {
                    PWMethodProduction pwMethodProduction = ParentPWMethod<PWMethodProduction>();
                    if (pwMethodProduction != null)
                    {
                        PWDischargingKSE otherNode = pwMethodProduction.ACUrlCommand(InterlockACUrl) as PWDischargingKSE;
                        if (otherNode != null && otherNode.CurrentACState == ACStateEnum.SMRunning)
                        {
                            SubscribeToProjectWorkCycle();
                            return;
                        }
                    }
                }
                base.SMStarting();
                if (IsWaitingOnTarget && CurrentACState == ACStateEnum.SMStarting && !_MovingToDisPosDone && PositioningDestination > 0)
                {
                    _TriesWaitingOnTarget++;
                    if (_TriesWaitingOnTarget > 3 && StartPositioning())
                    {
                        IsMovingToDisPos.ValueT = true;
                        UnSubscribeToProjectWorkCycle();
                    }
                }
            }
        }

        protected bool StartPositioning()
        {
            Msg msg = null;

            if (ParentPWGroup == null
                || this.ContentACClassWF == null
                || !this.ContentACClassWF.RefPAACClassMethodID.HasValue)
            {
                return false;
            }
            gip.core.datamodel.ACClassMethod refPAACClassMethod = null;

            using (ACMonitor.Lock(this.ContextLockForACClassWF))
            {
                refPAACClassMethod = this.ContentACClassWF.RefPAACClassMethod;
            }
            if (refPAACClassMethod == null)
            {
                return false;
            }

            PAProcessModule module = null;
            if (ParentPWGroup.NeedsAProcessModule && (ACOperationMode == ACOperationModes.Live || ACOperationMode == ACOperationModes.Simulation))
                module = ParentPWGroup.AccessedProcessModule;
            // Testmode
            else
                module = ParentPWGroup.ProcessModuleForTestmode;

            if (module == null)
            {
                return false;
            }
            ACMethod acMethod = refPAACClassMethod.TypeACSignature();
            if (acMethod == null)
            {
                //Error50153: acMethod is null.
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartPositioning()", 191, "Error50153");

                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return false;
            }

            PAProcessFunction responsibleFunc = CanStartProcessFunc(module, acMethod);
            if (responsibleFunc == null)
                return false;

            acMethod["Route"] = new Route();
            var acValue = acMethod.ParameterValueList.GetACValue("PositioningDestination");
            if (acValue == null)
                return false;
            acValue.Value = PositioningDestination;

            
            if (!acMethod.IsValid())
            {
                msg = new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "StartPositioning(1)",
                    Message = acMethod.ValidMessage.Message
                };

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), "StartPositioning(2)", msg.Message);
                OnNewAlarmOccurred(ProcessAlarm, msg.Message, true);
                return false;
            }

            RecalcTimeInfo();
            CreateNewProgramLog(acMethod);
            _ExecutingACMethod = acMethod;

#if DEBUG
            module.TaskInvocationPoint.ClearMyInvocations(this);
#endif
            if (!IsTaskStarted(module.TaskInvocationPoint.AddTask(acMethod, this)))
            {
                // Error50074: Dischargingtask not startable Order {0}, Bill of material {1}, line {2}
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartPositioning(3)", 233, "Error50074", "ProgramNo", "PartslistNo", "MaterialName1");

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), "StartPositioning(3)", msg.Message);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return false;
            }
            UpdateCurrentACMethod();

            AcknowledgeAlarms();
            return true;
        }

        public override bool AfterConfigForACMethodIsSet(ACMethod paramMethod, bool isForPAF, params Object[] acParameter)
        {
            if (!base.AfterConfigForACMethodIsSet(paramMethod, isForPAF, acParameter))
                return false;
            ACValue acValue = null;
            if (InCleaningCycle.ValueT)
            {
                acValue = paramMethod.ParameterValueList.GetACValue("DischargePosition");
                if (acValue != null && ParentPWGroup != null)
                {
                    acValue.Value = CleaningDest;
                    PAMScaleKSE kseScale = ParentPWGroup.AccessedProcessModule as PAMScaleKSE;
                    if (kseScale != null)
                        kseScale.CleaningCounter.ValueT = 0;
                }
            }
            else
            {
                acValue = paramMethod.ParameterValueList.GetACValue("PositioningDestination");
                ACValue acValueDest = paramMethod.ParameterValueList.GetACValue("Destination");
                if ((acValue == null || acValue.ParamAsUInt16 <= 0)
                    && (acValueDest != null && acValueDest.ParamAsInt16 > 0))
                {
                    _DischargingStarted = true;
                }
            }
            return true;
        }

#region Callback
        public override void TaskCallback(IACPointNetBase sender, ACEventArgs e, IACObject wrapObject)
        {
            _InCallback = true;
            try
            {
                if (e != null)
                {
                    IACTask taskEntry = wrapObject as IACTask;
                    ACMethodEventArgs eM = e as ACMethodEventArgs;
                    if (taskEntry.State == PointProcessingState.Deleted && CurrentACState != ACStateEnum.SMIdle)
                    {
                        CheckIfAutomaticTargetChangePossible = null;
                        ACMethod acMethod = e.ParentACMethod;
                        if (acMethod == null)
                            acMethod = taskEntry.ACMethod;
                        if (ParentPWGroup == null)
                        {
                            Messages.LogException(this.GetACUrl(), "TaskCallback()", "ParentPWGroup is null");
                            return;
                        }
                        PAProcessFunction discharging = ParentPWGroup.GetExecutingFunction<PAProcessFunction>(taskEntry.RequestID);
                        CheckIfAutomaticTargetChangePossible = null;
                        if (discharging != null && !IsMovingToDisPos.ValueT && !InCleaningCycle.ValueT)
                        {
                            double actualWeight = 0;
                            var acValue = e.GetACValue("ActualQuantity");
                            if (acValue != null)
                                actualWeight = acValue.ParamAsDouble;
                            //short simulationWeight = (short)acMethod["Source"];

                            using (var db = new Database())
                            {
                                using (var dbApp = new DatabaseApp(db))
                                {
                                    ProdOrderPartslistPos currentBatchPos = null;
                                    if (IsProduction)
                                    {
                                        currentBatchPos = ParentPWMethod<PWMethodProduction>().CurrentProdOrderPartslistPos.FromAppContext<ProdOrderPartslistPos>(dbApp);
                                        // Wenn kein Istwert von der Funktion zurückgekommen, dann berechne Zugangsmenge über die Summe der dosierten Mengen
                                        // Minus der bereits zugebuchten Menge (falls zyklische Zugagnsbuchungen im Hintergrund erfolgten)
                                        if (actualWeight <= 0.000001)
                                        {
                                            ACProdOrderManager prodOrderManager = ACProdOrderManager.GetServiceInstance(this);
                                            if (prodOrderManager != null)
                                            {
                                                double calculatedBatchWeight = 0;
                                                if (prodOrderManager.CalcProducedBatchWeight(dbApp, currentBatchPos, out calculatedBatchWeight) == null)
                                                {
                                                    double diff = calculatedBatchWeight - currentBatchPos.ActualQuantityUOM;
                                                    if (diff > 0.00001)
                                                        actualWeight = diff;
                                                }
                                            }
                                        }

                                        if ((this.IsSimulationOn/* || simulationWeight == 1*/)
                                            && actualWeight <= 0.000001
                                            && currentBatchPos != null)
                                        {
                                            actualWeight = currentBatchPos.TargetQuantityUOM;
                                        }
                                        // Entleerschritt liefert keine Menge
                                        else if (actualWeight <= 0.000001)
                                        {
                                            actualWeight = -0.001;
                                        }

                                        var routeItem = CurrentDischargingDest(db);
                                        PAProcessModule targetModule = TargetPAModule(db); // If Discharging is to Processmodule, then targetSilo ist null
                                        if (routeItem != null && targetModule != null)
                                        {
                                            try
                                            {
                                                DoInwardBooking(actualWeight, dbApp, routeItem, null, currentBatchPos, e, true);
                                            }
                                            finally
                                            {
                                                routeItem.Detach();
                                            }
                                        }

                                        if (ParentPWGroup != null)
                                        {
                                            List<PWDosing> previousDosings = PWDosing.FindPreviousDosingsInPWGroup<PWDosing>(this);
                                            if (previousDosings != null)
                                            {
                                                foreach (var pwDosing in previousDosings)
                                                {
                                                    if (((ACSubStateEnum)ParentPWGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMDisThenNextComp))
                                                        pwDosing.ResetDosingsAfterInterDischarging(dbApp);
                                                    else
                                                        pwDosing.SetDosingsCompletedAfterDischarging(dbApp);
                                                }
                                            }
                                        }
                                        if (dbApp.IsChanged)
                                        {
                                            dbApp.ACSaveChanges();
                                        }
                                    }
                                    else if (IsIntake)
                                    {
                                        // TODO: DeliveryNote-Entry DoInwardBooking()
                                    }
                                }
                            }
                        }

                        if (IsMovingToDisPos.ValueT)
                        {
                            _MovingToDisPosDone = true;
                            _TriesWaitingOnTarget = 0;
                            IsMovingToDisPos.ValueT = false;
                            _LastCallbackResult = e;
                            SubscribeToProjectWorkCycle();
                            return;
                        }
                        else
                        {
                            bool scaleIsEmpty = true;
                            PAMScaleKSE kseScale = ParentPWGroup.AccessedProcessModule as PAMScaleKSE;
                            if (kseScale != null)
                                scaleIsEmpty = kseScale.IsScaleEmpty;
                            // Falls Entleerung noch nicht gestartet oder Waage nicht vollständig leer
                            if (!_DischargingStarted || !scaleIsEmpty)
                            {
                                if (!scaleIsEmpty)
                                {
                                    Messages.LogDebug(this.GetACUrl(), "TaskCallback(10)", "Scale is not empty");
                                    _DischargingStarted = false;
                                }
                                _LastCallbackResult = e;
                                _SMStartingLock = true;
                                CurrentACState = ACStateEnum.SMStarting;
                                _SMStartingLock = false;
                                SubscribeToProjectWorkCycle();
                                return;
                            }
                            else if (_DischargingStarted && !InCleaningCycle.ValueT && CleaningCycle > 0)
                            {
                                kseScale = ParentPWGroup.AccessedProcessModule as PAMScaleKSE;
                                if (kseScale != null)
                                    kseScale.CleaningCounter.ValueT++;
                                if (kseScale.CleaningCounter.ValueT >= CleaningCycle)
                                {
                                    InCleaningCycle.ValueT = true;
                                    _SMStartingLock = true;
                                    CurrentACState = ACStateEnum.SMStarting;
                                    _SMStartingLock = false;
                                    SubscribeToProjectWorkCycle();
                                    return;
                                }
                            }
                            else if (InCleaningCycle.ValueT)
                                InCleaningCycle.ValueT = false;
                        }

                        _LastCallbackResult = e;
                        UnSubscribeToProjectWorkCycle();
                        CurrentACState = ACStateEnum.SMCompleted;
                    }
                    else if (PWPointRunning != null && eM != null && eM.ResultState == Global.ACMethodResultState.InProcess && taskEntry.State == PointProcessingState.Accepted)
                    {
                        PAProcessModule module = sender.ParentACComponent as PAProcessModule;
                        if (module != null)
                        {
                            PAProcessFunction function = module.GetExecutingFunction<PAProcessFunction>(eM.ACRequestID);
                            if (function != null)
                            {
                                if (function.CurrentACState == ACStateEnum.SMRunning)
                                {
                                    ACEventArgs eventArgs = ACEventArgs.GetVirtualEventArgs("PWPointRunning", VirtualEventArgs);
                                    eventArgs.GetACValue("TimeInfo").Value = RecalcTimeInfo();
                                    PWPointRunning.Raise(eventArgs);
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                _InCallback = false;
            }
        }
        #endregion

        protected override void DumpPropertyList(XmlDocument doc, XmlElement xmlACPropertyList)
        {
            base.DumpPropertyList(doc, xmlACPropertyList);

            XmlElement xmlChild = xmlACPropertyList["_MovingToDisPosDone"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("_MovingToDisPosDone");
                if (xmlChild != null)
                    xmlChild.InnerText = _MovingToDisPosDone.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["_TriesWaitingOnTarget"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("_TriesWaitingOnTarget");
                if (xmlChild != null)
                    xmlChild.InnerText = _TriesWaitingOnTarget.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["_DischargingStarted"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("_DischargingStarted");
                if (xmlChild != null)
                    xmlChild.InnerText = _DischargingStarted.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["InterlockACUrl"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("InterlockACUrl");
                if (xmlChild != null)
                    xmlChild.InnerText = String.IsNullOrEmpty(InterlockACUrl) ? "null" : InterlockACUrl;
                xmlACPropertyList.AppendChild(xmlChild);
            }

        }
        #endregion

    }
}
