using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using gip.mes.processapplication;
using gip.mes.datamodel;
using gip.bso.masterdata;
using gip.mes.facility;
using gip.core.autocomponent;
using System.Xml;

namespace kse.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'PW Dosing KSE'}de{'PW Dosieren KSE'}", Global.ACKinds.TPWNodeMethod, Global.ACStorableTypes.Optional, false, PWMethodVBBase.PWClassName, true)]
    public class PWDosingKSE : PWDosing
    {

        public new const string PWClassName = "PWDosingKSE";

        #region c´tors
        static PWDosingKSE()
        {
            List<ACMethodWrapper> wrappers = ACMethod.OverrideFromBase(typeof(PWDosingKSE), ACStateConst.SMStarting);
            if (wrappers != null)
            {
                foreach (ACMethodWrapper wrapper in wrappers)
                {
                    wrapper.Method.ParameterValueList.Add(new ACValue("PreDosingWFACUrl", typeof(string), null, Global.ParamOption.Optional));
                    wrapper.ParameterTranslation.Add("PreDosingWFACUrl", "en{'Predosing Workflow-ACUrl'}de{'Vordosierung Workflow-ACUrl'}");
                    wrapper.Method.ParameterValueList.Add(new ACValue("PreReadyWFACUrl", typeof(string), null, Global.ParamOption.Optional));
                    wrapper.ParameterTranslation.Add("PreReadyWFACUrl", "en{'Workflow-ACUrl for occupying intermediate silo'}de{'Workflow-ACUrl für Zwischensilobelegung'}");
                }
            }
            RegisterExecuteHandler(typeof(PWDosingKSE), HandleExecuteACMethod_PWDosingKSE);
        }

        public PWDosingKSE(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
            using (ACMonitor.Lock(_20015_LockValue))
            {
                _PreDosingCompleted = false;
            }
            return base.ACDeInit(deleteACClassTask);
        }

        public override void Recycle(IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
        {
            using (ACMonitor.Lock(_20015_LockValue))
            {
                _PreDosingCompleted = false;
            }
            base.Recycle(content, parentACObject, parameter, acIdentifier);
        }

        #endregion

        #region Properties
        private bool _PreDosingCompleted = false;
        [ACPropertyInfo(true, 9999)]
        public bool PreDosingCompleted
        {
            get
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _PreDosingCompleted;
                }
            }
            set
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _PreDosingCompleted = value;
                }
                OnPropertyChanged("PreDosingCompleted");
            }
        }


        public string PreDosingWFACUrl
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("PreDosingWFACUrl");
                    if (acValue != null)
                    {
                        return acValue.ParamAsString;
                    }
                }
                return null;
            }
        }

        public string PreReadyWFACUrl
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("PreReadyWFACUrl");
                    if (acValue != null)
                    {
                        return acValue.ParamAsString;
                    }
                }
                return null;
            }
        }
        #endregion

        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PWDosingKSE(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PWDosing(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion

        #region Methods
        public override void SMIdle()
        {
            base.SMIdle();
        }

        [ACMethodState("en{'Executing'}de{'Ausführend'}", 20, true)]
        public override void SMStarting()
        {
            base.SMStarting();
        }

        public override bool HasOpenDosings(out double sumQuantity)
        {
            if (!String.IsNullOrEmpty(PreDosingWFACUrl)
                && !String.IsNullOrEmpty(PreReadyWFACUrl))
            {
                sumQuantity = 0.0;
                if (PreDosingCompleted)
                    return false;
                PWMethodProduction pwMethodProduction = ParentPWMethod<PWMethodProduction>();
                if (pwMethodProduction == null)
                    return false;
                PWDosingKSE preDosingNode = pwMethodProduction.ACUrlCommand(PreDosingWFACUrl) as PWDosingKSE;
                PWGroup readyAfterDischargingNode = pwMethodProduction.ACUrlCommand(PreReadyWFACUrl) as PWGroup;
                if (preDosingNode != null && readyAfterDischargingNode != null)
                {
                    if (!preDosingNode.HasDosedComponents(out sumQuantity))
                        return false;
                    return true;
                }
                return false;
            }
            return base.HasOpenDosings(out sumQuantity);
        }

        protected override PAProcessModule HandleNotFoundPMOnManageDosingStateProd(ManageDosingStatesMode mode, DatabaseApp dbApp)
        {
            // Workaround 08.11.2020: There is a bug after restarting:
            string acUrl = this.GetACUrl();
            if (!String.IsNullOrEmpty(acUrl) && acUrl.Contains("WAFCD(1)\\DosingKSE(0)"))
            {
                Messages.LogError(acUrl, "HandleNotFoundPMOnManageDosingStateProd", "\\W4Mix\\FB510 replaced");
                PAProcessModule pm = ACUrlCommand("\\W4Mix\\FB510") as PAProcessModule;
                return pm;
            }
            return null;
        }


        public override StartNextCompResult StartNextProdComponent(PAProcessModule module)
        {
            PWMethodProduction pwMethodProduction = ParentPWMethod<PWMethodProduction>();
            // If dosing is not for production, then do nothing
            if (pwMethodProduction == null)
            {
                return StartNextCompResult.Done;
            }

            if (   !String.IsNullOrEmpty(PreDosingWFACUrl) 
                && !String.IsNullOrEmpty(PreReadyWFACUrl))
            {
                if (PreDosingCompleted)
                    return StartNextCompResult.Done;
                PWDosingKSE preDosingNode = pwMethodProduction.ACUrlCommand(PreDosingWFACUrl) as PWDosingKSE;
                PWGroup readyAfterDischargingNode = pwMethodProduction.ACUrlCommand(PreReadyWFACUrl) as PWGroup;
                if (preDosingNode != null && readyAfterDischargingNode != null)
                {
                    Msg msg = null;
                    PAProcessModule intermediateSilo = readyAfterDischargingNode.AccessedProcessModule;
                    if (intermediateSilo == null)
                    {
                        //Error50295: No Intermediate silo accessed from {0}
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextProdComponent(5.0.1)", 1049, "Error50295", PreReadyWFACUrl);

                        Messages.LogWarning(this.GetACUrl(), "StartNextProdComponent(5.0.1)", msg.Message);
                        if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                        {
                            Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.Message);
                            OnNewAlarmOccurred(ProcessAlarm, msg, true);
                        }
                        return StartNextCompResult.CycleWait;
                    }

                    double dosingQuantity = 0.0;
                    if (!preDosingNode.HasDosedComponents(out dosingQuantity))
                        return StartNextCompResult.Done;

                    var scale = ParentPWGroup != null ? ParentPWGroup.AccessedProcessModule as PAMHopperscale : null;
                    if (scale != null)
                    {
                        double? remainingWeight = null;
                        if (scale.RemainingWeightCapacity.HasValue)
                            remainingWeight = scale.RemainingWeightCapacity.Value;
                        else if (scale.MaxWeightCapacity.ValueT > 0.00000001)
                            remainingWeight = scale.MaxWeightCapacity.ValueT;
                        if (!remainingWeight.HasValue)
                        {
                            if (!MaxWeightAlarmSet)
                            {
                                MaxWeightAlarmSet = true;
                                //Error50162: MaxWeightCapacity of scale {0} is not configured.
                                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextProdComponent(5.1)", 1050, "Error50162", scale.GetACUrl());

                                Messages.LogWarning(this.GetACUrl(), "StartNextProdComponent(5.1)", msg.Message);
                                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                {
                                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.Message);
                                    OnNewAlarmOccurred(ProcessAlarm, msg, true);
                                }
                            }
                        }
                        else if (dosingQuantity > remainingWeight.Value)
                        {
                            ParentPWGroup.CurrentACSubState = (uint)ACSubStateEnum.SMInterDischarging;
                            return StartNextCompResult.Done;
                        }
                    }

                    using (var dbIPlus = new Database())
                    using (var dbApp = new DatabaseApp(dbIPlus))
                    {

                        Route dosingRoute = null;
                        PWDischarging.DetermineDischargingRoute(dbIPlus, intermediateSilo, ParentPWGroup.AccessedProcessModule, out dosingRoute, 0, 
                                            (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule, PAProcessModule.SelRuleID_ProcessModule_Deselector, true, true, null);
                        if (dosingRoute != null)
                            dosingRoute.Detach(true);
                        else
                        {
                            // Error50296: No route found between {0} and {1}
                            msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextProdComponent(5.2)", 1070, "Error50296", intermediateSilo.GetACUrl(), ParentPWGroup.AccessedProcessModule.GetACUrl());

                            if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.Message);
                            OnNewAlarmOccurred(ProcessAlarm, msg, true);
                            return StartNextCompResult.CycleWait;
                        }

                        CurrentDosingRoute = dosingRoute;
                        NoSourceFoundForDosing.ValueT = 0;

                        // 4. Starte Dosierung von diesem Silo aus
                        gip.core.datamodel.ACClassMethod refPAACClassMethod = null;

                        using (ACMonitor.Lock(this.ContextLockForACClassWF))
                        {
                            refPAACClassMethod = this.ContentACClassWF.RefPAACClassMethod;
                        }
                        ACMethod acMethod = refPAACClassMethod.TypeACSignature();

                        if (acMethod == null)
                        {
                            //Error50154: acMethod is null.
                            msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextProdComponent(5.3)", 1120, "Error50154");
                            OnNewAlarmOccurred(ProcessAlarm, msg, true);
                            return StartNextCompResult.CycleWait;
                        }

                        PAProcessFunction responsibleFunc = CanStartProcessFunc(module, acMethod, dbApp, null, null, null, null, intermediateSilo);
                        if (responsibleFunc == null)
                            return StartNextCompResult.CycleWait;

                        if (!(bool)ExecuteMethod("GetConfigForACMethod", acMethod, true, dbApp, null, null, null, null, intermediateSilo))
                            return StartNextCompResult.CycleWait;

                        PADosingLastBatchEnum lastBatchMode = PADosingLastBatchEnum.None;
                        acMethod[PWMethodVBBase.IsLastBatchParamName] = (short)lastBatchMode;

                        if (!ValidateAndSetRouteForParam(acMethod, dosingRoute))
                            return StartNextCompResult.CycleWait;
                        acMethod["Source"] = intermediateSilo.RouteItemIDAsNum;
                        acMethod["TargetQuantity"] = Math.Abs(dosingQuantity);
                        //if (relation.SourceProdOrderPartslistPos.Material.Density > 0.00001)
                        //    acMethod["Density"] = relation.SourceProdOrderPartslistPos.Material.Density;
                        if (dosingRoute != null)
                            dosingRoute.Detach(true);

                        if (!(bool)ExecuteMethod(nameof(AfterConfigForACMethodIsSet), acMethod, true, dbApp, null, null, null, null, intermediateSilo))
                            return StartNextCompResult.CycleWait;

                        if (!acMethod.IsValid())
                        {
                            // Error50066: Dosingtask not startable Order {0}, Bill of material {1}, line {2}
                            msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextProdComponent(10)", 1130, "Error50066",
                                            "0000",
                                            "0000",
                                            "0000");

                            if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.Message);
                            OnNewAlarmOccurred(ProcessAlarm, msg, true);
                            return StartNextCompResult.CycleWait;
                        }

                        RecalcTimeInfo();
                        CurrentDosingPos.ValueT = Guid.Empty;
                        if (CreateNewProgramLog(acMethod, _NewAddedProgramLog == null) <= CreateNewProgramLogResult.ErrorNoProgramFound)
                            return StartNextCompResult.CycleWait;
                        _ExecutingACMethod = acMethod;

                        module.TaskInvocationPoint.ClearMyInvocations(this);
                        _CurrentMethodEventArgs = null;
                        if (!IsTaskStarted(module.TaskInvocationPoint.AddTask(acMethod, this)))
                        {
                            ACMethodEventArgs eM = _CurrentMethodEventArgs;
                            if (eM == null || eM.ResultState != Global.ACMethodResultState.FailedAndRepeat)
                            {
                                // Error50066: Dosingtask not startable Order {0}, Bill of material {1}, line {2}
                                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextProdComponent(10)", 1140, "Error50066",
                                            "0000",
                                            "0000",
                                            "0000");

                                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.Message);
                                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                            }
                            CurrentDosingPos.ValueT = Guid.Empty;
                            return StartNextCompResult.CycleWait;
                        }
                        UpdateCurrentACMethod();

                        CachedEmptySiloHandlingOption = null;
                        AcknowledgeAlarms();
                        ExecuteMethod(nameof(OnACMethodSended), acMethod, true, dbApp, null, null, null, null, intermediateSilo, responsibleFunc);
                        return StartNextCompResult.NextCompStarted;
                    }
                }
            }
            return base.StartNextProdComponent(module);
        }

        public override bool AfterConfigForACMethodIsSet(ACMethod paramMethod, bool isForPAF, params Object[] acParameter)
        {
            if (!base.AfterConfigForACMethodIsSet(paramMethod, isForPAF, acParameter))
                return false;
            if (isForPAF)
            {
                if (acParameter != null && acParameter.Any())
                {
                    ProdOrderPartslistPosRelation relation = acParameter.Where(c => c is ProdOrderPartslistPosRelation).FirstOrDefault() as ProdOrderPartslistPosRelation;
                    if (relation != null)
                    {
                        try
                        {
                            UInt16 dosIndication = (UInt16)relation.SourceProdOrderPartslistPos.Material["KSEDosIndication"];
                            if (dosIndication > 10)
                                dosIndication = 10;
                            var acValue = paramMethod.ParameterValueList.GetACValue("IndicationDosibility");
                            if (acValue != null)
                                acValue.Value = dosIndication;

                            acValue = paramMethod.ParameterValueList.GetACValue("WeigherNumber");
                            if (acValue == null)
                                paramMethod.ParameterValueList.Add(new ACValue("WeigherNumber", typeof(UInt16), (UInt16)CB_WeigherNumber.Automatic));

                            PAMScaleKSE accessedScale = ParentPWGroup.AccessedProcessModule as PAMScaleKSE;
                            if (accessedScale != null && ((CB_WeigherNumber)acValue.Value) == CB_WeigherNumber.Automatic)
                            {
                                double targetQuantity = paramMethod.ParameterValueList.GetDouble("TargetQuantity");
                                if (accessedScale.MaxWeightCapacity2.ValueT > 0.001)
                                {
                                    CB_WeigherNumber weigherNumber = CB_WeigherNumber.Automatic;
                                    if (targetQuantity > accessedScale.MaxWeightCapacity2.ValueT)
                                        weigherNumber = CB_WeigherNumber.LargeScale;
                                    else
                                    {
                                        weigherNumber = CB_WeigherNumber.SmallScale;
                                        double density = paramMethod.ParameterValueList.GetDouble("Density");
                                        if (accessedScale.MaxVolumeCapacity2.ValueT > 0.001 && Material.ValidateDensity(density))
                                        {
                                            double volume = (targetQuantity * 1000) / density;
                                            if (volume > accessedScale.MaxVolumeCapacity2.ValueT)
                                                weigherNumber = CB_WeigherNumber.LargeScale;
                                        }
                                    }
                                    acValue = paramMethod.ParameterValueList.GetACValue("WeigherNumber");
                                    if (acValue == null)
                                        paramMethod.ParameterValueList.Add(new ACValue("WeigherNumber", typeof(UInt16), (UInt16)weigherNumber));
                                    else
                                        acValue.Value = (UInt16)weigherNumber;
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
                                gip.core.datamodel.Database.Root.Messages.LogException("PWDosingKSE", "AfterConfigForACMethodIsSet", msg);
                        }
                    }
                }
            }
            return true;
        }

        public override Msg DoDosingBooking(IACPointNetBase sender, ACEventArgs e, IACObject wrapObject, PADosingAbortReason dosingFuncResultState, PAFDosing dosing, string dis2SpecialDest = null, bool reEvaluatePosState = true, double? actualQuantity = null, double? tolerancePlus = null, double? toleranceMinus = null, double? targetQuantity = null)
        {
            IACTask taskEntry = wrapObject as IACTask;
            if (   !String.IsNullOrEmpty(PreDosingWFACUrl) 
                && !String.IsNullOrEmpty(PreReadyWFACUrl))
            {
                ACMethodEventArgs eM = e as ACMethodEventArgs;
                if (eM.ResultState == Global.ACMethodResultState.Succeeded)
                {
                    if (!actualQuantity.HasValue)
                        actualQuantity = (double)e["ActualQuantity"];
                    if (!tolerancePlus.HasValue)
                        tolerancePlus = (double)e.ParentACMethod["TolerancePlus"];
                    if (!toleranceMinus.HasValue)
                        toleranceMinus = (double)e.ParentACMethod["ToleranceMinus"];
                    targetQuantity = (double)e.ParentACMethod["TargetQuantity"];

                    if (actualQuantity >= 0.0001 && actualQuantity > (targetQuantity * 0.2))
                        PreDosingCompleted = true;
                }
                return null;
            }
            else
                return base.DoDosingBooking(sender, e, wrapObject, dosingFuncResultState, dosing, dis2SpecialDest, reEvaluatePosState, actualQuantity, tolerancePlus, toleranceMinus, targetQuantity);
        }

        private class SiloSortElem
        {
            public ProdOrderPartslistPosRelation Relation { get; set; }
            public int DistanceX { get; set; }
            public int DistanceY { get; set; }
        }

        protected override ProdOrderPartslistPosRelation[] OnSortOpenDosings(ProdOrderPartslistPosRelation[] queryOpenDosings, Database dbIPlus, DatabaseApp dbApp)
        {
            List<SiloSortElem> sortList = new List<SiloSortElem>();
            foreach (ProdOrderPartslistPosRelation relation in queryOpenDosings)
            {
                if (!relation.SourceProdOrderPartslistPos.Material.UsageACProgram)
                {
                    sortList.Add(new SiloSortElem() { Relation = relation, DistanceX = -1, DistanceY = -1 });
                    continue;
                }
                ACPartslistManager.QrySilosResult possibleSilos;
                RouteQueryParams queryParams = new RouteQueryParams(RouteQueryPurpose.StartDosing, ACPartslistManager.SearchMode.SilosWithOutwardEnabled, null, null, ExcludedSilos, ReservationMode);
                IEnumerable<Route> routes = GetRoutes(relation, dbApp, dbIPlus, queryParams, null, out possibleSilos);
                if (routes != null && routes.Any() && possibleSilos != null && possibleSilos.FilteredResult != null)
                {
                    var silo = possibleSilos.FilteredResult.FirstOrDefault();
                    if (possibleSilos.FilteredResult.Count > 1)
                    {
                        Route firstRoute = routes.FirstOrDefault();
                        RouteItem sourceItem = firstRoute.FirstOrDefault();
                        if (sourceItem != null)
                        {
                            silo = possibleSilos.FilteredResult.Where(c => c.StorageBin.VBiFacilityACClassID.HasValue && c.StorageBin.VBiFacilityACClassID.Value == sourceItem.Source.ACClassID).FirstOrDefault();
                            if (silo == null)
                                silo = possibleSilos.FilteredResult.FirstOrDefault();
                        }
                    }
                    sortList.Add(new SiloSortElem() { Relation = relation, DistanceX = Convert.ToInt32(silo.StorageBin.DistanceFront), DistanceY = Convert.ToInt32(silo.StorageBin.DistanceBehind) });
                }
                else
                    sortList.Add(new SiloSortElem() { Relation = relation, DistanceX = 0, DistanceY = 0 });
            }
            return sortList.OrderByDescending(c => c.DistanceX)
                            .ThenByDescending(c => c.DistanceY)
                            .Select(c => c.Relation)
                            .ToArray();
        }

        protected override void DumpPropertyList(XmlDocument doc, XmlElement xmlACPropertyList, ref DumpStats dumpStats)
        {
            base.DumpPropertyList(doc, xmlACPropertyList, ref dumpStats);

            XmlElement xmlChild = xmlACPropertyList["PreDosingWFACUrl"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("PreDosingWFACUrl");
                if (xmlChild != null)
                    xmlChild.InnerText = String.IsNullOrEmpty(PreDosingWFACUrl) ? "null" : PreDosingWFACUrl;
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["PreReadyWFACUrl"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("PreReadyWFACUrl");
                if (xmlChild != null)
                    xmlChild.InnerText = String.IsNullOrEmpty(PreReadyWFACUrl) ? "null" : PreReadyWFACUrl;
                xmlACPropertyList.AppendChild(xmlChild);
            }
        }
        #endregion
    }
}
