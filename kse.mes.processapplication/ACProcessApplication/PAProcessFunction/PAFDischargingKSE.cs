using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.core.autocomponent;
using System.ComponentModel;
using gip.mes.processapplication;

namespace kse.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'KSE Scale Discharging'}de{'KSE Waage Entleeren'}", Global.ACKinds.TPAProcessFunction, Global.ACStorableTypes.Required, false, PWDischargingKSE.PWClassName, true)]
    public class PAFDischargingKSE : PAFDischarging
    {

        #region ctor's

        static PAFDischargingKSE()
        {
            ACMethod.RegisterVirtualMethod(typeof(PAFDischargingKSE), "Start", CreateVirtualMethod("DischargingKSE", "en{'Discharging KSE'}de{'Entleeren KSE'}", typeof(PWDischargingKSE)));
            RegisterExecuteHandler(typeof(PAFDischargingKSE), HandleExecuteACMethod_PAFDischargingKSE);
        }

        public PAFDischargingKSE(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _ExtraDisTargetDestKSE = new ACPropertyConfigValue<string>(this, "ExtraDisTargetDestKSE", "\\W4Mix\\LPAusKSE");
        }
        #endregion

        #region Config
        protected ACPropertyConfigValue<string> _ExtraDisTargetDestKSE;
        [ACPropertyConfig("en{'ACUrl extra discharging dest.'}de{'ACUrl Sonderentleerziel'}")]
        public string ExtraDisTargetDestKSE
        {
            get
            {
                return _ExtraDisTargetDestKSE.ValueT;
            }
        }
        #endregion

        #region PAFDischarging implementation

        protected override MsgWithDetails CompleteACMethodOnSMStarting(ACMethod acMethod)
        {
            var acValue = acMethod.ParameterValueList.GetACValue("PositioningDestination");
            if (acValue != null && acValue.ParamAsUInt16 > 0)
                return null;
            return base.CompleteACMethodOnSMStarting(acMethod);
        }

        protected override void OnSetRouteItemData(ACMethod acMethod, RouteItem targetItem, RouteItem sourceItem, Route route)
        {
            base.OnSetRouteItemData(acMethod, targetItem, sourceItem, route);
            if (targetItem != null)
            {
                PAProcessModule acCompTarget = targetItem.TargetACComponent as PAProcessModule;
                var acValue = acMethod.ParameterValueList.GetACValue("DischargePosition");
                if (acCompTarget != null && acValue != null && acValue.ParamAsUInt16 == 0)
                {
                    int targetNo = acCompTarget.RouteItemIDAsNum;
                    // Sonderentleerziel (Loch neben Entstaubungsloch)
                    if (targetNo <= 1)
                    {
                        UInt16 idExtraTarget = 0;
                        if (!String.IsNullOrEmpty(ExtraDisTargetDestKSE))
                        {
                            PAProcessModule disModule = ACUrlCommand(ExtraDisTargetDestKSE) as PAProcessModule;
                            if (disModule != null)
                            {
                                try
                                {
                                    idExtraTarget = Convert.ToUInt16(disModule.RouteItemIDAsNum);
                                }
                                catch (Exception e)
                                {
                                    Messages.LogException(this.GetACUrl(), "OnSetRouteItemData(10)", e.Message);
                                }
                            }
                        }
                        acValue.Value = idExtraTarget > 0 ? idExtraTarget : (UInt16)1002;
                    }
                    // Normale Entleerung über Standardentleerloch
                    else
                    {
                        if (targetNo >= 1000 && targetNo <= 1002)
                            acValue.Value = Convert.ToUInt16(targetNo);
                        else
                            acValue.Value = (UInt16)1001;
                    }
                }
            }
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

        #endregion

        #region Methods

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

        public static bool HandleExecuteACMethod_PAFDischargingKSE(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PAFDischarging(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }

        #endregion


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


        private new static ACMethodWrapper CreateVirtualMethod(string acIdentifier, string captionTranslation, Type pwClass)
        {
            ACMethod method = new ACMethod(acIdentifier);

            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();
            method.ParameterValueList.Add(new ACValue("Route", typeof(Route), null, Global.ParamOption.Required));
            paramTranslation.Add("Route", "en{'Route'}de{'Route'}");
            // Dosing bin number
            method.ParameterValueList.Add(new ACValue("Destination", typeof(Int16), (Int16)0, Global.ParamOption.Required));
            paramTranslation.Add("Destination", "en{'Destination'}de{'Ziel'}");
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
            paramTranslation.Add("TolerancePlus", "en{'Tolerance +'}de{'Toleranz +'}");
            method.ParameterValueList.Add(new ACValue("ToleranceMinus", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            paramTranslation.Add("ToleranceMinus", "en{'Tolerance -'}de{'Toleranz -'}");
            // Dosing accuracy
            method.ParameterValueList.Add(new ACValue("DosingAccuracy", typeof(UInt16), 0, Global.ParamOption.Required));
            paramTranslation.Add("DosingAccuracy", "en{'DosingAccuracy'}de{'DosierGenauigkeit'}");
            // Indication dosability
            method.ParameterValueList.Add(new ACValue("IndicationDosibility", typeof(UInt16), 0, Global.ParamOption.Required));
            paramTranslation.Add("IndicationDosibility", "en{'IndicationDosibility'}de{'DosierIndikation'}");
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
            paramTranslation.Add("MassDensity", "en{'MassDensity'}de{'MassDensity'}");
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

        public override void InheritParamsFromConfig(ACMethod newACMethod, ACMethod configACMethod, bool isConfigInitialization)
        {
            if (isConfigInitialization)
            {
                newACMethod.ParameterValueList.CopyValues(configACMethod.ParameterValueList);
            }
            else
            {
                var acValue = newACMethod.ParameterValueList.GetACValue("TolerancePlus");
                if (acValue != null && acValue.ParamAsDouble <= 0.000001)
                {
                    ACValue acValueConfig = configACMethod.ParameterValueList.GetACValue("TolerancePlus");
                    if (acValueConfig != null)
                        acValue.Value = acValueConfig.ParamAsDouble;
                }
                acValue = newACMethod.ParameterValueList.GetACValue("ToleranceMinus");
                if (acValue != null && acValue.ParamAsDouble <= 0.000001)
                {
                    ACValue acValueConfig = configACMethod.ParameterValueList.GetACValue("ToleranceMinus");
                    if (acValueConfig != null)
                        acValue.Value = acValueConfig.ParamAsDouble;
                }
            }
        }
        #endregion
    }
}
