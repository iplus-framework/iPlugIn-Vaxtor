using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using kse.mes.processapplication;
using gip.mes.datamodel;

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
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'PW Containerdischarging'}de{'PW Containerentleerung'}", Global.ACKinds.TPWNodeMethod, Global.ACStorableTypes.Optional, false, PWMethodVBBase.PWClassName, true)]
    public class PWContDisKSE : PWDischarging
    {
        new public const string PWClassName = "PWContDisKSE";

        #region c´tors
        static PWContDisKSE()
        {
            RegisterExecuteHandler(typeof(PWContDisKSE), HandleExecuteACMethod_PWContDisKSE);
            ACMethod.InheritFromBase(typeof(PWContDisKSE), ACStateConst.SMStarting);
        }


        public PWContDisKSE(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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

        #region Methods

        #region Execute-Helper-Handlers
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "GetKSEDestination":
                    result = GetKSEDestination();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public static bool HandleExecuteACMethod_PWContDisKSE(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PWDischarging(out result, acComponent, acMethodName, acClassMethod, acParameter);
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

            base.SMStarting();
        }

        private DateTime? _WaitOnAlarmCode = null;
        public override void SMCompleted()
        {
            if (_LastCallbackResult != null)
            {
                ACValue acValue = _LastCallbackResult.GetACValue("AlarmCode");
                if (acValue != null)
                {
                    ContDisAlarmCode alarmCode = acValue.ValueT<ContDisAlarmCode>();
                    if (alarmCode != ContDisAlarmCode.KeinAlarm)
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

        /// <summary>
        /// Fills Parameterlist in ACMethod with values from Config-Store-Hierarchy
        /// Derivations of PWClasses can manipulate the Paramterlist as well according to their individual logic
        /// </summary>
        /// <param name="paramMethod">ACMethod to fill</param>
        /// <param name="isForPAF">If its a acMethod which will be passed to the Start-Method of a PAPocessFunction else it's the local configuration for this PWNode</param>
        public override bool GetConfigForACMethod(ACMethod paramMethod, bool isForPAF, params Object[] customParams)
        {
            if (!base.GetConfigForACMethod(paramMethod, isForPAF, customParams))
                return false;
            if (isForPAF)
            {
                paramMethod.ParameterValueList["Destination"] = (UInt16)1;
                paramMethod.ParameterValueList["ActionRequest"] = ContDisAction.AutomatischerStopEntleerung;
                paramMethod.ParameterValueList["ActivateVibrator"] = ContDisActivateVibrator.Nein;
                paramMethod.ParameterValueList["RequestPusher"] = ContDisRequestPusher.Nein;
                paramMethod.ParameterValueList["VibratorTime"] = (UInt16)10;
            }
            return true;
        }

        #endregion



        [ACMethodInfo("", "en{'Get KSE required position'}de{'Ermittle KSE-Anforderungsziel'}", 500)]
        public virtual UInt16? GetKSEDestination()
        {
            if (RootPW == null)
                return null;
            if (ParentPWGroup == null)
            {
                // TODO: AlarmMessage
                return null;
            }
            if (ParentPWGroup.AccessedProcessModule == null)
            {
                // TODO: AlarmMessage
                return null;
            }
            PAMDockDisKSE dockingStation = ParentPWGroup.AccessedProcessModule as PAMDockDisKSE;
            if (dockingStation != null && !dockingStation.IsFinalDest)
            {
                return GetPickingDestination();
            }

            return System.Convert.ToUInt16(ParentPWGroup.AccessedProcessModule.RouteItemIDAsNum);
        }

        public UInt16? GetPickingDestination()
        {
            UInt16? destID = null;
            if (IsIntake)
            {
                var pwMethod = ParentPWMethod<PWMethodIntake>();
                ACMethod acMethod = pwMethod.CurrentACMethod.ValueT;
                if (acMethod != null)
                {
                    using (var db = new Database())
                    using (var dbApp = new DatabaseApp(db))
                    {
                        Picking picking = PWDischarging.GetTransportEntityFromACMethod(dbApp, acMethod) as Picking;
                        if (picking != null)
                        {
                            PickingPos pos = picking.PickingPos_Picking.FirstOrDefault();
                            if (pos != null && pos.ToFacility != null)
                            {
                                var acClass = pos.ToFacility.FacilityACClass;
                                if (acClass != null)
                                {
                                    PAMSilo pamSilo = ACUrlCommand(acClass.GetACUrlComponent()) as PAMSilo;
                                    if (pamSilo != null)
                                    {
                                        int siloNr = pamSilo.RouteItemIDAsNum;
                                        if (siloNr >= 10000)
                                            siloNr -= 10000;
                                        if (siloNr <= 0)
                                            return null;
                                        return Convert.ToUInt16(siloNr);
                                    }
                                }
                                //if (pos.ToFacility.FacilityNo.StartsWith("4FA") || pos.ToFacility.FacilityNo.StartsWith("4KA"))
                                //{
                                //    string siloNr = pos.ToFacility.FacilityNo.Substring(3);
                                //    if (!String.IsNullOrEmpty(siloNr))
                                //    {
                                //        try
                                //        {
                                //            destID = Convert.ToUInt16(siloNr);
                                //        }
                                //        catch
                                //        {
                                //        }
                                //    }
                                //}
                            }
                        }
                    }
                }
            }
            return destID;
        }


        #endregion
    }
}
