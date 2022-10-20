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
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'KSE Cont. Filling'}de{'KSE Cont. Befüllen'}", Global.ACKinds.TPAProcessFunction, Global.ACStorableTypes.Required, false, PWContFillKSE.PWClassName, true)]
    public class PAFContFillKSE : PAProcessFuncKSE
    {        
        #region Properties
        #endregion

        #region Constructors 

        static PAFContFillKSE()
        {
            ACMethod.RegisterVirtualMethod(typeof(PAFContFillKSE), "Start", CreateVirtualMethod("Filling", "en{'Filling'}de{'Befüllen'}", typeof(PWContFillKSE)));
            RegisterExecuteHandler(typeof(PAFContFillKSE), HandleExecuteACMethod_PAFContFillKSE);
        }

        public PAFContFillKSE(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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

        #endregion
                
        #region Public

        [ACMethodAsync("Process", "en{'Start'}de{'Start'}", (short)MISort.Start, false)]
        public override ACMethodEventArgs Start(ACMethod acMethod)
        {
            return base.Start(acMethod);
        }

        #endregion

        #region Private 
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

        public static bool HandleExecuteACMethod_PAFContFillKSE(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PAProcessFuncKSE(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }

        #endregion

        protected static ACMethodWrapper CreateVirtualMethod(string acIdentifier, string captionTranslation, Type pwClass)
        {
            ACMethod method = new ACMethod(acIdentifier);

            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();
            method.ParameterValueList.Add(new ACValue("Source", typeof(Int16), (Int16)0, Global.ParamOption.Optional));
            paramTranslation.Add("Source", "en{'Source'}de{'Quelle'}");
            //method.ParameterValueList.Add(new ACValue("Route", typeof(Route), null, Global.ParamOption.Required));
            //paramTranslation.Add("Route", "en{'Route'}de{'Route'}");
            //method.ParameterValueList.Add(new ACValue("Destination", typeof(Int16), (Int16)0, Global.ParamOption.Required));
            //paramTranslation.Add("Destination", "en{'Destination'}de{'Ziel'}");
            //method.ParameterValueList.Add(new ACValue("EmptyWeight", typeof(Double?), null, Global.ParamOption.Optional));
            //paramTranslation.Add("EmptyWeight", "en{'Empty weight'}de{'Leergewicht'}");
            //method.ParameterValueList.Add(new ACValue("DischargingTime", typeof(TimeSpan), TimeSpan.Zero, Global.ParamOption.Optional));
            //paramTranslation.Add("DischargingTime", "en{'Discharging time'}de{'Entleerzeit'}");
            //method.ParameterValueList.Add(new ACValue("PulsationTime", typeof(TimeSpan), TimeSpan.Zero, Global.ParamOption.Optional));
            //paramTranslation.Add("PulsationTime", "en{'Pulsation time'}de{'Pulszeit'}");
            //method.ParameterValueList.Add(new ACValue("PulsationPauseTime", typeof(TimeSpan), TimeSpan.Zero, Global.ParamOption.Optional));
            //paramTranslation.Add("PulsationPauseTime", "en{'Pulsation pause time'}de{'Puls-Pause Zeit'}");
            //method.ParameterValueList.Add(new ACValue("IdleCurrent", typeof(Double), (Double)0.0, Global.ParamOption.Optional)); // Leerlaufstrom
            //paramTranslation.Add("IdleCurrent", "en{'Idle current'}de{'Leerlaufstrom'}");
            //method.ParameterValueList.Add(new ACValue("InterDischarging", typeof(Int16), (Int16)0, Global.ParamOption.Optional));
            //paramTranslation.Add("InterDischarging", "en{'Inter discharging'}de{'Zwischenentleerung'}");
            //method.ParameterValueList.Add(new ACValue("Vibrator", typeof(Int16), (Int16)0, Global.ParamOption.Optional));
            //paramTranslation.Add("Vibrator", "en{'Vibrator'}de{'Rüttler'}");
            //method.ParameterValueList.Add(new ACValue("Power", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            //paramTranslation.Add("Power", "en{'Power'}de{'Leistung'}");
            //method.ParameterValueList.Add(new ACValue("Tolerance", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            //paramTranslation.Add("Tolerance", "en{'Tolerance'}de{'Toleranz'}");

            Dictionary<string, string> resultTranslation = new Dictionary<string, string>();
            //method.ResultValueList.Add(new ACValue("ActualQuantity", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            //resultTranslation.Add("ActualQuantity", "en{'Actual quantity'}de{'Istmenge'}");
            //method.ResultValueList.Add(new ACValue("ScaleTotalWeight", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            //resultTranslation.Add("ScaleTotalWeight", "en{'Total quantity'}de{'Gesamtgewicht'}");
            method.ResultValueList.Add(new ACValue("AlarmReference", typeof(UInt16), (UInt16)0, Global.ParamOption.Optional));
            resultTranslation.Add("AlarmReference", "en{'AlarmReference'}de{'AlarmReference'}");
            method.ResultValueList.Add(new ACValue("ContainerNr", typeof(UInt16), (UInt16)0, Global.ParamOption.Optional));
            resultTranslation.Add("ContainerNr", "en{'ContainerNr time'}de{'ContainerNr'}");
            method.ResultValueList.Add(new ACValue("ContainerCode", typeof(UInt16), (UInt16)0, Global.ParamOption.Optional));
            resultTranslation.Add("ContainerCode", "en{'ContainerCode'}de{'ContainerCode'}");
            method.ResultValueList.Add(new ACValue("GrossWeight", typeof(Int32), (Int32)0, Global.ParamOption.Optional));
            resultTranslation.Add("GrossWeight", "en{'GrossWeight'}de{'GrossWeight'}");
            method.ResultValueList.Add(new ACValue("NetWeight", typeof(Int32), (Int32)0, Global.ParamOption.Optional));
            resultTranslation.Add("NetWeight", "en{'NetWeight'}de{'NetWeight'}");
            method.ResultValueList.Add(new ACValue("AlarmCode", typeof(ContFillAlarmCode), ContFillAlarmCode.KeinAlarm, Global.ParamOption.Optional));
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
