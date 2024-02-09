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
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'KSE Cont. request'}de{'KSE Cont. Suche'}", Global.ACKinds.TPAProcessFunction, Global.ACStorableTypes.Required, false, PWContReqKSE.PWClassName, true)]
    public class PAFContReqKSE : PAProcessFuncKSE
    {        
        //#region Properties
        //#region Read-Values from PLC          
        //public IACContainerTNet<UInt16> ContainerInterfaceNr { get; set; }
        //#endregion

        //#region Write-Values to PLC
        //public IACContainerTNet<UInt16> RequiredPosition { get; set; }
        //public IACContainerTNet<UInt16> ContainerType { get; set; }
        //#endregion
        //#endregion

        #region Constructors 

        static PAFContReqKSE()
        {
            ACMethod.RegisterVirtualMethod(typeof(PAFContReqKSE), "Start", CreateVirtualMethod("FindContainer", "en{'Containerrequest'}de{'Containersuche'}", typeof(PWContReqKSE)));
            RegisterExecuteHandler(typeof(PAFContReqKSE), HandleExecuteACMethod_PAFContReqKSE);
        }

        public PAFContReqKSE(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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

        public static bool HandleExecuteACMethod_PAFContReqKSE(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
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
            method.ParameterValueList.Add(new ACValue("RequiredPosition", typeof(UInt16), (UInt16)0, Global.ParamOption.Required));
            paramTranslation.Add("RequiredPosition", "en{'Position'}de{'Position'}");
            method.ParameterValueList.Add(new ACValue("ContainerType", typeof(UInt16), (UInt16)0, Global.ParamOption.Required));
            paramTranslation.Add("ContainerType", "en{'Containertype'}de{'Containertyp'}");

            Dictionary<string, string> resultTranslation = new Dictionary<string, string>();
            method.ResultValueList.Add(new ACValue("ContainerInterfaceNr", typeof(UInt16), (UInt16)0, Global.ParamOption.Optional));
            resultTranslation.Add("ContainerInterfaceNr", "en{'Containerinterfaceno.'}de{'Schnittstellennr. Container'}");
            method.ResultValueList.Add(new ACValue("AlarmCode", typeof(ContReqAlarmCode), ContReqAlarmCode.NoAlarm, Global.ParamOption.Optional));
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

        [ACMethodInfo("Function", "en{'Inherit params from config'}de{'Übernehme Parameter aus Konfiguration'}", 9999)]
        public virtual void InheritParamsFromConfig(ACMethod newACMethod, ACMethod configACMethod, bool isConfigInitialization)
        {
            if (newACMethod.ParameterValueList.GetInt16("RequiredPosition") <= 0)
                newACMethod["RequiredPosition"] = configACMethod.ParameterValueList.GetInt16("RequiredPosition");
            if (newACMethod.ParameterValueList.GetInt16("ContainerType") <= 0)
                newACMethod["ContainerType"] = configACMethod.ParameterValueList.GetInt16("ContainerType");
            if (newACMethod.ParameterValueList.GetInt16("ContainerInterfaceNr") <= 0)
                newACMethod["ContainerInterfaceNr"] = configACMethod.ParameterValueList.GetInt16("ContainerInterfaceNr");
        }
        #endregion


        public override void InitializeRouteAndConfig(Database dbIPlus)
        {
        }
    }

}
