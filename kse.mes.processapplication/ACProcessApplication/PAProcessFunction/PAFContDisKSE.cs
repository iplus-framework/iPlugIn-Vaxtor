using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.core.autocomponent;
using System.ComponentModel;
using kse.mes.processapplication;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'KSE Cont. Discharging'}de{'KSE Cont. Entleeren'}", Global.ACKinds.TPAProcessFunction, Global.ACStorableTypes.Required, false, PWContDisKSE.PWClassName, true)]
    public class PAFContDisKSE : PAProcessFuncKSE
    {        
        #region Properties
        #endregion

        #region Constructors 

        static PAFContDisKSE()
        {
            ACMethod.RegisterVirtualMethod(typeof(PAFContDisKSE), "Start", CreateVirtualMethod("Discharging", "en{'Discharge'}de{'Entleeren'}", typeof(PWContDisKSE)));
            RegisterExecuteHandler(typeof(PAFContDisKSE), HandleExecuteACMethod_PAFContDisKSE);
        }

        public PAFContDisKSE(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            return true;
        }

        void PAFContDisKSE_ValueUpdatedOnReceival(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            return base.ACDeInit(deleteACClassTask);
        }

        #endregion

        #region override abstract methods
        protected override MsgWithDetails CompleteACMethodOnSMStarting(ACMethod acMethod)
        {
            return null;
        }

        protected override PAProcessFunction.CompleteResult AnalyzeACMethodResult(ACMethod acMethod, out MsgWithDetails msg, CompleteResult completeResult)
        {
            msg = null;
            return completeResult;
        }

        public override void InitializeRouteAndConfig(Database dbIPlus)
        {
        }

        protected MsgWithDetails GetACMethodFromConfig(Database db, Route route, ACMethod acMethod, bool isConfigInitialization = false)
        {
            return null;
        }

        #region Execute-Helper-Handlers
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "InheritParamsFromConfig":
                    InheritParamsFromConfig(acParameter[0] as ACMethod, acParameter[1] as ACMethod, (bool)acParameter[2]);
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public static bool HandleExecuteACMethod_PAFContDisKSE(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PAProcessFuncKSE(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }

        #endregion


        #endregion

        #region Public

        [ACMethodAsync("Process", "en{'Start'}de{'Start'}", (short)MISort.Start, false)]
        public override ACMethodEventArgs Start(ACMethod acMethod)
        {
            return base.Start(acMethod);
        }

        #endregion

        #region Private 

        protected static ACMethodWrapper CreateVirtualMethod(string acIdentifier, string captionTranslation, Type pwClass)
        {
            ACMethod method = new ACMethod(acIdentifier);

            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();

            method.ParameterValueList.Add(new ACValue("Route", typeof(Route), null, Global.ParamOption.Required));
            paramTranslation.Add("Route", "en{'Route'}de{'Route'}");
            method.ParameterValueList.Add(new ACValue("Destination", typeof(UInt16), (UInt16)0, Global.ParamOption.Required));
            paramTranslation.Add("Destination", "en{'Destination'}de{'Ziel'}");
            method.ParameterValueList.Add(new ACValue("ActionRequest", typeof(ContDisAction), (ContDisAction)0, Global.ParamOption.Required));
            paramTranslation.Add("ActionRequest", "en{'ActionRequest'}de{'ActionRequest'}");
            method.ParameterValueList.Add(new ACValue("ActivateVibrator", typeof(ContDisActivateVibrator), (ContDisActivateVibrator)0, Global.ParamOption.Optional));
            paramTranslation.Add("ActivateVibrator", "en{'ActivateVibrator'}de{'Anwahl Vibrator'}");
            method.ParameterValueList.Add(new ACValue("RequestPusher", typeof(ContDisRequestPusher), (ContDisRequestPusher)0, Global.ParamOption.Optional));
            paramTranslation.Add("RequestPusher", "en{'RequestPusher'}de{'Anwahl Klopfer'}");
            method.ParameterValueList.Add(new ACValue("VibratorTime", typeof(UInt16), (UInt16)0, Global.ParamOption.Optional));
            paramTranslation.Add("VibratorTime", "en{'VibratorTime'}de{'Vibrator Zeit'}");

            Dictionary<string, string> resultTranslation = new Dictionary<string, string>();
            
            method.ResultValueList.Add(new ACValue("ActionResponse", typeof(ContDisAction), (ContDisAction)0, Global.ParamOption.Optional));
            resultTranslation.Add("ActionResponse", "en{'ActionResponse'}de{'ActionResponse'}");
            method.ResultValueList.Add(new ACValue("ContainerNr", typeof(UInt16), (UInt16)0, Global.ParamOption.Optional));
            resultTranslation.Add("ContainerNr", "en{'ContainerNr'}de{'ContainerNr'}");
            method.ResultValueList.Add(new ACValue("ContainerCode", typeof(UInt16), (UInt16)0, Global.ParamOption.Optional));
            resultTranslation.Add("ContainerCode", "en{'ContainerCode'}de{'ContainerCode'}");
            method.ResultValueList.Add(new ACValue("GrossWeight", typeof(Int32), (Int32)0, Global.ParamOption.Optional));
            resultTranslation.Add("GrossWeight", "en{'GrossWeight'}de{'GrossWeight'}");
            method.ResultValueList.Add(new ACValue("NetWeight", typeof(Int32), (Int32)0, Global.ParamOption.Optional));
            resultTranslation.Add("NetWeight", "en{'NetWeight'}de{'NetWeight'}");
            method.ResultValueList.Add(new ACValue("DischargeNotReleased", typeof(ContDisDischargeNotReleased), (ContDisDischargeNotReleased)0, Global.ParamOption.Optional));
            resultTranslation.Add("DischargeNotReleased", "en{'DischargeNotReleased'}de{'DischargeNotReleased'}");
            method.ResultValueList.Add(new ACValue("DustDischarging", typeof(ContDisDustDischarging), (ContDisDustDischarging)0, Global.ParamOption.Optional));
            resultTranslation.Add("DustDischarging", "en{'DustDischarging'}de{'DustDischarging'}");
            method.ResultValueList.Add(new ACValue("AlarmCode", typeof(ContDisAlarmCode), ContDisAlarmCode.KeinAlarm, Global.ParamOption.Optional));
            resultTranslation.Add("AlarmCode", "en{'AlarmCode'}de{'AlarmCode'}");

            return new ACMethodWrapper(method, captionTranslation, pwClass, paramTranslation, resultTranslation);
        }

        public override void AcknowledgeAlarms()
        {
            if (!IsEnabledAcknowledgeAlarms())
                return;
            if (Malfunction.ValueT == PANotifyState.AlarmOrFault)
                AckMalfunction.ValueT = true;
            if (FunctionError.ValueT == PANotifyState.AlarmOrFault)
                FunctionError.ValueT = PANotifyState.Off;
            base.AcknowledgeAlarms();
        }

        public override bool IsEnabledAcknowledgeAlarms()
        {
            if ((Malfunction.ValueT == PANotifyState.AlarmOrFault && !AckMalfunction.ValueT)
                || FunctionError.ValueT == PANotifyState.AlarmOrFault)
                return true;
            return base.IsEnabledAcknowledgeAlarms();
        }

        [ACMethodInfo("Function", "en{'Inherit params from config'}de{'Übernehme Dosierparameter aus Konfiguration'}", 9999)]
        public virtual void InheritParamsFromConfig(ACMethod newACMethod, ACMethod configACMethod, bool isConfigInitialization)
        {
        }
        #endregion

    }

}
