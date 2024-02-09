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
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'KSE Container service interface'}de{'KSE Container Service Schnittstelle'}", Global.ACKinds.TPAProcessFunction, Global.ACStorableTypes.Required, false, PWProcessFunction.PWClassName, true)]
    public class PAFContServiceKSE : PAProcessFunction
    {
        #region Properties
        #endregion

        #region Constructors

        static PAFContServiceKSE()
        {
            ACMethod.RegisterVirtualMethod(typeof(PAFContServiceKSE), "Start", CreateVirtualMethod("CleanContainer", "en{'Clean Container'}de{'Contaier Reinigen'}", typeof(PWProcessFunction)));
            RegisterExecuteHandler(typeof(PAFContServiceKSE), HandleExecuteACMethod_PAFContServiceKSE);
        }

        public PAFContServiceKSE(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
        protected override MsgWithDetails CompleteACMethodOnSMStarting(ACMethod acMethod, ACMethod previousParams)
        {
           
            return null;
        }

        protected override PAProcessFunction.CompleteResult AnalyzeACMethodResult(ACMethod acMethod, out MsgWithDetails msg, CompleteResult completeResult)
        {
            msg = null;
            return completeResult;
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

        public static bool HandleExecuteACMethod_PAFContServiceKSE(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PAProcessFunction(out result, acComponent, acMethodName, acClassMethod, acParameter);
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
            method.ParameterValueList.Add(new ACValue("ActionRequest", typeof(ContServiceAction), (ContServiceAction)0, Global.ParamOption.Required));
            paramTranslation.Add("ActionRequest", "en{'Action Request'}de{'Gewünschte Aktion'}");
           

            Dictionary<string, string> resultTranslation = new Dictionary<string, string>();
            method.ResultValueList.Add(new ACValue("ActionResponse", typeof(ContServiceAction), (ContServiceAction)0, Global.ParamOption.Optional));
            resultTranslation.Add("ActionResponse", "en{'Action Response.'}de{'Aktuelle Aktion'}");

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

        [ACMethodInfo("Function", "en{'Inherit params from config'}de{'Übernehme Parameter aus Konfiguration'}", 9999)]
        public virtual void InheritParamsFromConfig(ACMethod newACMethod, ACMethod configACMethod, bool isConfigInitialization)
        {
        }
        #endregion


        public override void InitializeRouteAndConfig(Database dbIPlus)
        {
        }
    }

}
