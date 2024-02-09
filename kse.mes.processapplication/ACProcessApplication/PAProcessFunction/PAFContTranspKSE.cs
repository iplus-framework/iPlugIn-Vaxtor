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
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'KSE Containerinterface'}de{'KSE Containerschnittstelle'}", Global.ACKinds.TPAProcessFunction, Global.ACStorableTypes.Required, false, PWContTranspKSE.PWClassName, true)]
    public class PAFContTranspKSE : PAProcessFuncKSE
    {        
        #region Properties
        #endregion

        #region Constructors 

        static PAFContTranspKSE()
        {
            ACMethod.RegisterVirtualMethod(typeof(PAFContTranspKSE), "Start", CreateVirtualMethod("MoveContainer", "en{'Containertransport'}de{'Containertransport'}", typeof(PWContTranspKSE)));
            RegisterExecuteHandler(typeof(PAFContTranspKSE), HandleExecuteACMethod_PAFContTranspKSE);
        }

        public PAFContTranspKSE(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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

        public static bool HandleExecuteACMethod_PAFContTranspKSE(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
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
            method.ParameterValueList.Add(new ACValue("ActionRequest", typeof(ContTranspAction), (ContTranspAction)0, Global.ParamOption.Required));
            paramTranslation.Add("ActionRequest", "en{'ActionRequest'}de{'ActionRequest'}");
            method.ParameterValueList.Add(new ACValue("RequiredPositionRequest", typeof(UInt16), (UInt16)0, Global.ParamOption.Required));
            paramTranslation.Add("RequiredPositionRequest", "en{'RequiredPositionRequest'}de{'RequiredPositionRequest'}");
            method.ParameterValueList.Add(new ACValue("OccupyId", typeof(UInt16), (UInt16)0, Global.ParamOption.Optional));
            paramTranslation.Add("OccupyId", "en{'OccupyId'}de{'OccupyId'}");
            method.ParameterValueList.Add(new ACValue("Release", typeof(ContTranspRelease), (ContTranspRelease)0, Global.ParamOption.Optional));
            paramTranslation.Add("Release", "en{'Release'}de{'Release'}");
            method.ParameterValueList.Add(new ACValue("FillWeight", typeof(Int32), (Int32)0, Global.ParamOption.Optional));
            paramTranslation.Add("FillWeight", "en{'FillWeight'}de{'FillWeight'}");
            method.ParameterValueList.Add(new ACValue("EmptyWeight", typeof(Int32), (Int32)0, Global.ParamOption.Optional));
            paramTranslation.Add("EmptyWeight", "en{'EmptyWeight'}de{'EmptyWeight'}");
            method.ParameterValueList.Add(new ACValue("ContainerType", typeof(UInt16), (UInt16)0, Global.ParamOption.Optional));
            paramTranslation.Add("ContainerType", "en{'ContainerType'}de{'ContainerType'}");

            Dictionary<string, string> resultTranslation = new Dictionary<string, string>();
            method.ResultValueList.Add(new ACValue("ActionResponse", typeof(ContTranspAction), (ContTranspAction)0, Global.ParamOption.Optional));
            resultTranslation.Add("ActionResponse", "en{'ActionResponse'}de{'ActionResponse'}");
            method.ResultValueList.Add(new ACValue("RequiredPositionResponse", typeof(UInt16), (UInt16)0, Global.ParamOption.Optional));
            resultTranslation.Add("RequiredPositionResponse", "en{'RequiredPositionResponse'}de{'RequiredPositionResponse'}");
            method.ResultValueList.Add(new ACValue("ContainerNr", typeof(UInt16), (UInt16)0, Global.ParamOption.Optional));
            resultTranslation.Add("ContainerNr", "en{'ContainerNr'}de{'ContainerNr'}");
            method.ResultValueList.Add(new ACValue("ContainerCode", typeof(UInt16), (UInt16)0, Global.ParamOption.Optional));
            resultTranslation.Add("ContainerCode", "en{'ContainerCode'}de{'ContainerCode'}");
            method.ResultValueList.Add(new ACValue("LastPosition", typeof(UInt16), (UInt16)0, Global.ParamOption.Optional));
            resultTranslation.Add("LastPosition", "en{'LastPosition'}de{'LastPosition'}");
            method.ResultValueList.Add(new ACValue("ActualPosition", typeof(UInt16), (UInt16)0, Global.ParamOption.Optional));
            resultTranslation.Add("ActualPosition", "en{'ActualPosition'}de{'ActualPosition'}");
            method.ResultValueList.Add(new ACValue("ActionPosition", typeof(UInt16), (UInt16)0, Global.ParamOption.Optional));
            resultTranslation.Add("ActionPosition", "en{'ActionPosition'}de{'ActionPosition'}");
            method.ResultValueList.Add(new ACValue("GrossWeight", typeof(Int32), (Int32)0, Global.ParamOption.Optional));
            resultTranslation.Add("GrossWeight", "en{'GrossWeight'}de{'GrossWeight'}");
            method.ResultValueList.Add(new ACValue("WeighingPosition", typeof(ContTranspWeighingPos), (ContTranspWeighingPos)0, Global.ParamOption.Optional));
            resultTranslation.Add("WeighingPosition", "en{'WeighingPosition'}de{'WeighingPosition'}");
            method.ResultValueList.Add(new ACValue("InternalOrderActive", typeof(ContTranspInternalOrderActive), (ContTranspInternalOrderActive)0, Global.ParamOption.Optional));
            resultTranslation.Add("InternalOrderActive", "en{'InternalOrderActive'}de{'InternalOrderActive'}");
            method.ResultValueList.Add(new ACValue("AlarmCode", typeof(ContTranspAlarmCode), ContTranspAlarmCode.KeinAlarm, Global.ParamOption.Optional));
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
        }
        #endregion


        public override void InitializeRouteAndConfig(Database dbIPlus)
        {
        }
    }

}
