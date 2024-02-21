using gip.core.autocomponent;
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.mes.processapplication;
using gip.mes.datamodel;
using gip.core.processapplication;

namespace kse.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'KSE Scale Dosing'}de{'KSE Waage Dosieren'}", Global.ACKinds.TPAProcessFunction, Global.ACStorableTypes.Required, false, PWDosingKSE.PWClassName, true)]
    public class PAFDosingKSE : PAFDosing
    {
        #region ctor's

        static PAFDosingKSE()
        {
            ACMethod.RegisterVirtualMethod(typeof(PAFDosingKSE), "Start", CreateVirtualMethod("DosingKSE", "en{'Dosing KSE'}de{'Dosieren KSE'}", typeof(PWDosingKSE)));
            RegisterExecuteHandler(typeof(PAFDosingKSE), HandleExecuteACMethod_PAFDosingKSE);
        }

        public PAFDosingKSE(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {

        }
        #endregion

        #region Execute-Helper-Handlers
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "ResetAlarmStateKSE":
                    ResetAlarmStateKSE();
                    return true;
                case Const.IsEnabledPrefix + "ResetAlarmStateKSE":
                    result = IsEnabledResetAlarmStateKSE();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public static bool HandleExecuteACMethod_PAFDosingKSE(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case Const.AskUserPrefix + "EndDosDisNextCompOnTol":
                    result = AskUserEndDosDisNextCompOnTol(acComponent);
                    return true;
                case Const.AskUserPrefix + "EndDosDisNextComp":
                    result = AskUserEndDosDisNextComp(acComponent);
                    return true;
            }
            return HandleExecuteACMethod_PAFDosing(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }

        #endregion


        #region Properties
        public KSEConvScale KSEConverter
        {
            get
            {
                return this.ACStateConverter as KSEConvScale;
            }
        }

        public override bool IsSimulationOn
        {
            get
            {
                return KSEConverter.IsSimulationOn;
            }
        }

        public string ExtraDisTargetDestKSE
        {
            get
            {
                PAFDischargingKSE disKSE = ParentACComponent.FindChildComponents<PAFDischargingKSE>(c => c is PAFDischargingKSE).FirstOrDefault();
                if (disKSE != null)
                    return disKSE.ExtraDisTargetDestKSE;
                return null;
            }
        }

        #endregion

        #region Methods
        [ACMethodAsync("Process", "en{'Start'}de{'Start'}", (short)MISort.Start, false)]
        public override ACMethodEventArgs Start(ACMethod acMethod)
        {
            return base.Start(acMethod);
        }

        [ACMethodInteraction("", "en{'Reset Alarm-Status KSE'}de{'Reset Alarm-Status KSE'}", 804, true)]
        public virtual void ResetAlarmStateKSE()
        {
            if (!IsEnabledResetAlarmStateKSE())
                return;
            if (KSEConverter != null)
                KSEConverter.ResetAlarmStateKSE();
        }

        public virtual bool IsEnabledResetAlarmStateKSE()
        {
            if (KSEConverter != null)
                return KSEConverter.IsEnabledResetAlarmStateKSE();
            return false;
        }

        public override bool IsEnabledSetAbortReasonEmpty()
        {
            if ((this.StateLackOfMaterial.ValueT == PANotifyState.Off && this.Malfunction.ValueT == PANotifyState.Off)
                || (CurrentACState != ACStateEnum.SMRunning && CurrentACState != ACStateEnum.SMPaused && CurrentACState != ACStateEnum.SMHeld && CurrentACState != ACStateEnum.SMCompleted))
                return false;
            return true;
        }

        public override bool IsEnabledSetAbortReasonMalfunction()
        {
            if ((this.StateLackOfMaterial.ValueT == PANotifyState.Off && this.StateTolerance.ValueT == PANotifyState.Off)
                || (CurrentACState != ACStateEnum.SMRunning && CurrentACState != ACStateEnum.SMPaused && CurrentACState != ACStateEnum.SMHeld && CurrentACState != ACStateEnum.SMCompleted))
                return false;
            return true; // Erlaubt nocheinmal zu setzen falls Komando nicht an SPS angekommen ist
            //if (DosingAbortReason.ValueT == PADosingAbortReason.NotSet)
            //    return true;
            //return false;
        }

        public override bool IsEnabledEndDosDisEnd()
        {
            return false;
            //if ((this.StateLackOfMaterial.ValueT == PANotifyState.Off && this.Malfunction.ValueT == PANotifyState.Off)
            //    || (CurrentACState != ACStateEnum.SMRunning && CurrentACState != ACStateEnum.SMPaused && CurrentACState != ACStateEnum.SMHeld && CurrentACState != ACStateEnum.SMCompleted))
            //    return false;
            //return true; // Erlaubt nocheinmal zu setzen falls Komando nicht an SPS angekommen ist
            ////return base.IsEnabledEndDosDisEnd();
        }

        public override bool IsEnabledEndDosDisEndOnTol()
        {
            return false;
            //if (this.StateTolerance.ValueT == PANotifyState.Off
            //    || (CurrentACState != ACStateEnum.SMRunning && CurrentACState != ACStateEnum.SMPaused && CurrentACState != ACStateEnum.SMHeld && CurrentACState != ACStateEnum.SMCompleted))
            //    return false;
            //return true; // Erlaubt nocheinmal zu setzen falls Komando nicht an SPS angekommen ist
            //return base.IsEnabledEndDosDisEndOnTol();
        }

        public override bool IsEnabledEndDosEndOrder()
        {
            return false;
            //if ((this.StateLackOfMaterial.ValueT == PANotifyState.Off && this.Malfunction.ValueT == PANotifyState.Off)
            //    || (CurrentACState != ACStateEnum.SMRunning && CurrentACState != ACStateEnum.SMPaused && CurrentACState != ACStateEnum.SMHeld && CurrentACState != ACStateEnum.SMCompleted))
            //    return false;
            //return true;
            //return base.IsEnabledEndDosEndOrder();
        }

        public override void EndDosDisNextComp()
        {
            if (!IsEnabledEndDosDisNextComp())
                return;
            string extraDisDest = ExtraDisTargetDestKSE;
            if (String.IsNullOrEmpty(extraDisDest))
                return;
            ExtraDisTargetDest = extraDisDest;
            EndDosDisNextCompForced();
        }


        public override bool IsEnabledEndDosDisNextComp()
        {
            if ((this.StateLackOfMaterial.ValueT == PANotifyState.Off && this.Malfunction.ValueT == PANotifyState.Off)
                || (CurrentACState != ACStateEnum.SMRunning && CurrentACState != ACStateEnum.SMPaused && CurrentACState != ACStateEnum.SMHeld && CurrentACState != ACStateEnum.SMCompleted))
                return false;
            return true; // Erlaubt nocheinmal zu setzen falls Komando nicht an SPS angekommen ist
            //return base.IsEnabledEndDosDisNextComp();
        }

        public static new bool AskUserEndDosDisNextComp(IACComponent acComponent)
        {
            if (acComponent == null)
                return false;
            // "Question50022": Do you want do cancel the current Dosing, then discharge to a alternate target and then dose the next component?
            Global.MsgResult questionRes = acComponent.Messages.Question(acComponent, "Question50022", Global.MsgResult.Yes);
            if (questionRes == Global.MsgResult.Yes)
            {
                // "Question50033" Please enter the Facility-No. or the address (ACUrl) of the alternative Target if you want to reject the batch. If you wan't to transport it to the originally planned target leave it blank.?
                //PWMethodVBBase.EnterExtraDisTargetDest(acComponent, "Question50034");
                return true;
            }
            return false;
        }


        public override void EndDosDisNextCompOnTol()
        {
            if (!IsEnabledEndDosDisNextCompOnTol())
                return;
            string extraDisDest = ExtraDisTargetDestKSE;
            if (String.IsNullOrEmpty(extraDisDest))
                return;
            ExtraDisTargetDest = extraDisDest;
            EndDosDisNextCompOnTolForced();
        }

        public override bool IsEnabledEndDosDisNextCompOnTol()
        {
            if (this.StateTolerance.ValueT == PANotifyState.Off
                || (CurrentACState != ACStateEnum.SMRunning && CurrentACState != ACStateEnum.SMPaused && CurrentACState != ACStateEnum.SMHeld && CurrentACState != ACStateEnum.SMCompleted))
                return false;
            return true; // Erlaubt nocheinmal zu setzen falls Komando nicht an SPS angekommen ist
            //return base.IsEnabledEndDosDisNextCompOnTol();
        }

        public static new bool AskUserEndDosDisNextCompOnTol(IACComponent acComponent)
        {
            if (acComponent == null)
                return false;
            // "Question50022": Do you want do cancel the current Dosing, then discharge to a alternate target and then dose the next component?
            Global.MsgResult questionRes = acComponent.Messages.Question(acComponent, "Question50022", Global.MsgResult.Yes);
            if (questionRes == Global.MsgResult.Yes)
            {
                // "Question50033" Please enter the Facility-No. or the address (ACUrl) of the alternative Target if you want to reject the batch. If you wan't to transport it to the originally planned target leave it blank.?
                //PWMethodVBBase.EnterExtraDisTargetDest(acComponent, "Question50034");
                return true;
            }
            return false;
        }

        public override bool IsEnabledAbort()
        {
            return base.IsEnabledAbort();
        }

        public override bool IsEnabledEndDosAdjustRestWait()
        {
            return false;
            //if ((this.StateLackOfMaterial.ValueT == PANotifyState.Off && this.Malfunction.ValueT == PANotifyState.Off)
            //    || (CurrentACState != ACStateEnum.SMRunning && CurrentACState != ACStateEnum.SMPaused && CurrentACState != ACStateEnum.SMHeld && CurrentACState != ACStateEnum.SMCompleted))
            //    return false;
            //return true; // Erlaubt nocheinmal zu setzen falls Komando nicht an SPS angekommen ist
        }

        public override void InitializeRouteAndConfig(gip.core.datamodel.Database dbIPlus)
        {
            //base.InitializeRouteAndConfig(dbIPlus);
            gip.core.datamodel.ACClass thisACClass = this.ACType as gip.core.datamodel.ACClass;
            gip.core.datamodel.ACClass parentACClass = ParentACComponent.ACType as gip.core.datamodel.ACClass;
            try
            {
                ACRoutingParameters routingParameters = new ACRoutingParameters()
                {
                    Database = dbIPlus,
                    DBSelector = (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule && c.ACClassID != parentACClass.ACClassID,
                    Direction = RouteDirections.Backwards,
                    DBIncludeInternalConnections = true
                };

                var routes = ACRoutingService.DbSelectRoutesFromPoint(thisACClass, this.PAPointMatIn1.PropertyInfo, routingParameters);
                if (routes != null && routes.Any())
                {
                    foreach (Route route in routes)
                    {
                        ACMethod acMethod = ACUrlACTypeSignature("!DosingKSE");
                        GetACMethodFromConfig(dbIPlus, route, acMethod, true);
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
                    gip.core.datamodel.Database.Root.Messages.LogException("PAFDosingKSE", "InitializeRouteAndConfig", msg);
            }
        }

        public override void InheritParamsFromConfig(ACMethod newACMethod, ACMethod configACMethod, bool isConfigInitialization)
        {
            if (isConfigInitialization)
            {
                object valueSource = null;
                ACValue acValue = newACMethod.ParameterValueList.GetACValue("Source");
                if (acValue != null)
                    valueSource = acValue.Value;

                newACMethod.ParameterValueList.CopyValues(configACMethod.ParameterValueList);

                try
                {
                    if (acValue != null)
                        newACMethod.ParameterValueList["Source"] = valueSource;
                    newACMethod.ParameterValueList[Material.ClassName] = "";
                    newACMethod.ParameterValueList["PLPosRelation"] = Guid.Empty;
                    newACMethod.ParameterValueList["Route"] = null;
                    newACMethod.ParameterValueList["TargetQuantity"] = (double) 0.0;
                    newACMethod.ParameterValueList["ODRReset"] = false;
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    if (gip.core.datamodel.Database.Root != null && gip.core.datamodel.Database.Root.Messages != null &&
                                                                          gip.core.datamodel.Database.Root.InitState == ACInitState.Initialized)
                        gip.core.datamodel.Database.Root.Messages.LogException("PAFDosingKSE", "InheritParamsFromConfig", msg);
                }
            }
            else
            {
                double targetQ = newACMethod.ParameterValueList.GetDouble("TargetQuantity");
                double tolPlus = newACMethod.ParameterValueList.GetDouble("TolerancePlus");
                if (Math.Abs(tolPlus) <= Double.Epsilon)
                    tolPlus = configACMethod.ParameterValueList.GetDouble("TolerancePlus");
                // Falls Toleranz negativ, dann ist die Toleranz in % angegeben
                if (tolPlus < -0.0000001)
                {
                    if (Math.Abs(targetQ) > Double.Epsilon)
                        tolPlus = targetQ * tolPlus * -0.01;
                    else
                        tolPlus = 0.001;
                }
                else if (Math.Abs(tolPlus) <= Double.Epsilon)
                {
                    if (Math.Abs(targetQ) > Double.Epsilon)
                        tolPlus = targetQ * 0.05;
                    else
                        tolPlus = 0.001;
                }
                if (tolPlus < 0.001)
                    tolPlus = 0.001;
                newACMethod["TolerancePlus"] = tolPlus;

                double tolMinus = newACMethod.ParameterValueList.GetDouble("ToleranceMinus");
                if (Math.Abs(tolMinus) <= Double.Epsilon)
                    tolMinus = configACMethod.ParameterValueList.GetDouble("ToleranceMinus");
                // Falls Toleranz negativ, dann ist die Toleranz in % angegeben
                if (tolMinus < -0.0000001)
                {
                    if (Math.Abs(targetQ) > Double.Epsilon)
                        tolMinus = targetQ * tolMinus * -0.01;
                    else
                        tolMinus = 0.001;
                }
                else if (Math.Abs(tolMinus) <= Double.Epsilon)
                {
                    if (Math.Abs(targetQ) > Double.Epsilon)
                        tolMinus = targetQ * 0.05;
                    else
                        tolMinus = 0.001;
                }
                if (tolMinus < 0.001)
                    tolMinus = 0.001;
                newACMethod["ToleranceMinus"] = tolMinus;

                double dosAcc = newACMethod.ParameterValueList.GetDouble("DosingAccuracy");
                if (Math.Abs(dosAcc) <= Double.Epsilon)
                    dosAcc = configACMethod.ParameterValueList.GetDouble("DosingAccuracy");
                // Falls DosingAccuracy negativ, dann ist die DosingAccuracy in % angegeben
                if (dosAcc < -0.0000001)
                {
                    if (Math.Abs(targetQ) > Double.Epsilon)
                        dosAcc = targetQ * dosAcc * -0.01;
                    else
                        dosAcc = 0.0;
                }
                newACMethod["DosingAccuracy"] = dosAcc;

                //if (newACMethod.ParameterValueList.GetUInt16("DosingAccuracy") == 0)
                //    newACMethod["DosingAccuracy"] = configACMethod.ParameterValueList.GetUInt16("DosingAccuracy");

                if (newACMethod.ParameterValueList.GetBoolean("CheckTare") == false)
                    newACMethod["CheckTare"] = configACMethod.ParameterValueList.GetBoolean("CheckTare");

                if (newACMethod.ParameterValueList.GetDouble("Density") <= 0.000001)
                    newACMethod["Density"] = configACMethod.ParameterValueList.GetDouble("Density");

                var acValueWeigherNumber = newACMethod.ParameterValueList.GetACValue("WeigherNumber");
                if (acValueWeigherNumber != null)
                {
                    if ((CB_WeigherNumber)acValueWeigherNumber.Value == CB_WeigherNumber.SmallScale)
                    {
                        var acValMaxVol = configACMethod.ParameterValueList.GetACValue("MaxVolumeSmallScale");
                        if (acValMaxVol != null && acValMaxVol.ParamAsDouble > 0.001)
                        {
                            double targetQuantity = newACMethod.ParameterValueList.GetDouble("TargetQuantity");
                            double density = newACMethod.ParameterValueList.GetDouble("Density");
                            if (Material.ValidateDensity(density))
                            {
                                double volume = (targetQuantity * 1000) / density;
                                if (volume > acValMaxVol.ParamAsDouble)
                                    acValueWeigherNumber.Value = (UInt16)CB_WeigherNumber.LargeScale;
                            }
                        }
                    }
                    CB_WeigherNumber weigherNumber = (CB_WeigherNumber)acValueWeigherNumber.Value;
                    // Reduziere Toleranz abhängig von der technischen Genauigkeit
                    if (weigherNumber != CB_WeigherNumber.Automatic)
                    {
                        var scale = ParentACComponent as PAMScaleKSE;
                        if (scale != null)
                        {
                            PAEScaleGravimetric dosingScale = null;
                            if (weigherNumber == CB_WeigherNumber.LargeScale)
                                dosingScale = scale.Scale;
                            else
                                dosingScale = scale.ScaleSmall;
                            if (dosingScale != null && dosingScale.DigitWeight.ValueT >= 0.000001)
                            {
                                double digitWeight = dosingScale.DigitWeight.ValueT * 0.001;
                                if (tolPlus < digitWeight)
                                {
                                    tolPlus = digitWeight;
                                    newACMethod["TolerancePlus"] = tolPlus;
                                }
                                if (tolMinus < digitWeight)
                                {
                                    tolMinus = digitWeight;
                                    newACMethod["ToleranceMinus"] = tolMinus;
                                }
                                if (dosAcc < digitWeight)
                                {
                                    dosAcc = digitWeight;
                                    newACMethod["DosingAccuracy"] = dosAcc;
                                }
                            }
                        }
                    }
                }
            }
        }


        protected override void OnSetRouteItemData(ACMethod acMethod, PAMSilo pamSilo, RouteItem sourceRouteItem, bool isConfigInitialization)
        {
            base.OnSetRouteItemData(acMethod, pamSilo, sourceRouteItem, isConfigInitialization);
            if (pamSilo != null && pamSilo.LearnDosing != null && pamSilo.LearnDosing.ValueT)
            {
                ACValue odrReset = acMethod.ParameterValueList.GetACValue("ODRReset");
                if (odrReset != null)
                    odrReset.Value = pamSilo.LearnDosing.ValueT;
                pamSilo.LearnDosing.ValueT = false;
            }
        }

        private new static ACMethodWrapper CreateVirtualMethod(string acIdentifier, string captionTranslation, Type pwClass)
        {
            ACMethod method = new ACMethod(acIdentifier);

            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();
            method.ParameterValueList.Add(new ACValue(Material.ClassName, typeof(string), null, Global.ParamOption.Optional));
            paramTranslation.Add(Material.ClassName, "en{'Material'}de{'Material'}");
            method.ParameterValueList.Add(new ACValue("PLPosRelation", typeof(Guid), null, Global.ParamOption.Optional));
            paramTranslation.Add("PLPosRelation", "en{'Order position'}de{'Auftragsposition'}");
            method.ParameterValueList.Add(new ACValue("Route", typeof(Route), null, Global.ParamOption.Required));
            paramTranslation.Add("Route", "en{'Route'}de{'Route'}");
            // Dosing bin number
            method.ParameterValueList.Add(new ACValue("Source", typeof(Int16), 0, Global.ParamOption.Required));
            paramTranslation.Add("Source", "en{'Source'}de{'Quelle'}");
            // Discharging position
            method.ParameterValueList.Add(new ACValue("DischargePosition", typeof(UInt16), 0, Global.ParamOption.Required));
            paramTranslation.Add("DischargePosition", "en{'DischargePosition'}de{'Entleerposition'}");
            // weigher number
            method.ParameterValueList.Add(new ACValue("WeigherNumber", typeof(UInt16), 0, Global.ParamOption.Required));
            paramTranslation.Add("WeigherNumber", "en{'WeigherNumber'}de{'Waagennummer'}");
            // check tare
            method.ParameterValueList.Add(new ACValue("CheckTare", typeof(bool), (bool)false, Global.ParamOption.Optional));
            paramTranslation.Add("CheckTare", "en{'Check Tare'}de{'Tara Überprüfung'}");
            // ToDo split into LSW/MSW
            method.ParameterValueList.Add(new ACValue("TargetQuantity", typeof(Double), (Double)0.0, Global.ParamOption.Required));
            paramTranslation.Add("TargetQuantity", "en{'TargetQuantity'}de{'Sollmenge'}");
            method.ParameterValueList.Add(new ACValue("TolerancePlus", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            paramTranslation.Add("TolerancePlus", "en{'Tolerance + [+=kg/-=%]'}de{'Toleranz + [+=kg/-=%]'}");
            method.ParameterValueList.Add(new ACValue("ToleranceMinus", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            paramTranslation.Add("ToleranceMinus", "en{'Tolerance - [+=kg/-=%]'}de{'Toleranz - [+=kg/-=%]'}");
            // Dosing accuracy
            method.ParameterValueList.Add(new ACValue("DosingAccuracy", typeof(Double), (Double)0.0, Global.ParamOption.Required));
            paramTranslation.Add("DosingAccuracy", "en{'DosingAccuracy [+=kg/-=%]'}de{'DosierGenauigkeit [+=kg/-=%]'}");
            // Indication dosability
            method.ParameterValueList.Add(new ACValue("IndicationDosibility", typeof(UInt16), 0, Global.ParamOption.Required));
            paramTranslation.Add("IndicationDosibility", "en{'IndicationDosibility'}de{'Dosierbarkeitsindex'}");
            // Dosing Mode
            method.ParameterValueList.Add(new ACValue("ODRReset", typeof(bool), (bool)false, Global.ParamOption.Optional));
            paramTranslation.Add("ODRReset", "en{'Reset ODR'}de{'ODR zurücksetzen'}");

            method.ParameterValueList.Add(new ACValue("DosingMode", typeof(UInt16), 1, Global.ParamOption.Optional));
            paramTranslation.Add("DosingMode", "en{'Dosing Mode'}de{'DosierMode'}");

            // Positioning destination
            method.ParameterValueList.Add(new ACValue("PositioningDestination", typeof(UInt16), 0, Global.ParamOption.Required));
            paramTranslation.Add("PositioningDestination", "en{'PositioningDestination'}de{'ZielPositionierung'}");
            // MassDensity
            method.ParameterValueList.Add(new ACValue("MassDensity", typeof(UInt16), 0, Global.ParamOption.Required));
            paramTranslation.Add("MassDensity", "en{'MassDensity [g]'}de{'MassDensity [g]'}");
            method.ParameterValueList.Add(new ACValue("Density", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            paramTranslation.Add("Density", "en{'Density [g/dm³]'}de{'Dichte [g/dm³]'}");
            method.ParameterValueList.Add(new ACValue("MaxVolumeSmallScale", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            paramTranslation.Add("MaxVolumeSmallScale", "en{'Max volume small scale [dm³]'}de{'Max. Volumen kleine Waage [dm³]'}");
            // BatchID
            method.ParameterValueList.Add(new ACValue("BatchID", typeof(UInt16), 0, Global.ParamOption.Required));
            paramTranslation.Add("BatchID", "en{'BatchID'}de{'BatchNummer'}");
            // BatchEnd
            method.ParameterValueList.Add(new ACValue("BatchEnd", typeof(UInt16), 0, Global.ParamOption.Required));
            paramTranslation.Add("BatchEnd", "en{'BatchEnd'}de{'Chargenende'}");

            Dictionary<string, string> resultTranslation = new Dictionary<string, string>();
            method.ResultValueList.Add(new ACValue("ActualQuantity", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            resultTranslation.Add("ActualQuantity", "en{'Actual quantity'}de{'Istgewicht'}");
            method.ResultValueList.Add(new ACValue("DosingTime", typeof(UInt16), (UInt16)0, Global.ParamOption.Optional));
            resultTranslation.Add("DosingTime", "en{'DosingTime'}de{'Dosierzeit'}");

            return new ACMethodWrapper(method, captionTranslation, pwClass, paramTranslation, resultTranslation);
        }
        #endregion

    }
}

