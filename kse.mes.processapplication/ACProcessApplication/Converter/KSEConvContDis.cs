using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.autocomponent;
using gip.core.communication;
using System.Threading;


namespace kse.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'KSEConvContDis'}de{'KSEConvContDis'}", Global.ACKinds.TPABGModule, Global.ACStorableTypes.NotStorable, false, false)]
    public class KSEConvContDis : KSEConvContBase
    {
        #region ctor's

        public KSEConvContDis(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            _AlarmCode.ValueUpdatedOnReceival += AlarmCode_PropertyChanged;
            _DischargeReleasedExtern.ValueUpdatedOnReceival += DischargeReleasedExtern_PropertyChanged;
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            _AlarmCode.ValueUpdatedOnReceival -= AlarmCode_PropertyChanged;
            _DischargeReleasedExtern.ValueUpdatedOnReceival -= DischargeReleasedExtern_PropertyChanged;
            return base.ACDeInit(deleteACClassTask);
        }
        #endregion

        #region Properties
        #region Read-Values from PLC


        [ACPropertyBindingTarget(103, "AlarmCode", "en{'AlarmCode'}de{'AlarmCode'}", "", false, false, RemotePropID=25)]
        public IACContainerTNet<ContDisAlarmCode> AlarmCode { get; set; }
        protected IACPropertyNetTarget _AlarmCode
        {
            get
            {
                return (IACPropertyNetTarget)this.AlarmCode;
            }
        }
        protected ContDisAlarmCode _AlarmCodeValue
        {
            get { return AlarmCode.ValueT; }
        }


        [ACPropertyBindingTarget(104, "AlarmReference", "en{'AlarmReference'}de{'AlarmReference'}", "", false, false, RemotePropID=26)]
        public IACContainerTNet<UInt16> AlarmReference { get; set; }

        [ACPropertyBindingTarget(105, "ActionResponse", "en{'ActionResponse'}de{'ActionResponse'}", "", false, false, RemotePropID=27)]
        public IACContainerTNet<ContDisAction> ActionResponse { get; set; }

        [ACPropertyBindingTarget(106, "ContainerNr", "en{'ContainerNr'}de{'ContainerNr'}", "", false, false, RemotePropID=28)]
        public IACContainerTNet<UInt16> ContainerNr { get; set; }

        [ACPropertyBindingTarget(107, "ContainerCode", "en{'ContainerCode'}de{'ContainerCode'}", "", false, false, RemotePropID=29)]
        public IACContainerTNet<UInt16> ContainerCode { get; set; }

        [ACPropertyBindingTarget(108, "GrossWeight", "en{'GrossWeight'}de{'GrossWeight'}", "", false, false, RemotePropID=30)]
        public IACContainerTNet<Int32> GrossWeight { get; set; }

        [ACPropertyBindingTarget(110, "NetWeight", "en{'NetWeight'}de{'NetWeight'}", "", false, false, RemotePropID=31)]
        public IACContainerTNet<Int32> NetWeight { get; set; }

        [ACPropertyBindingTarget(112, "DischargeNotReleased", "en{'DischargeNotReleased'}de{'DischargeNotReleased'}", "", false, false, RemotePropID=32)]
        public IACContainerTNet<ContDisDischargeNotReleased> DischargeNotReleased { get; set; }

        [ACPropertyBindingTarget(113, "DustDischarging", "en{'DustDischarging'}de{'DustDischarging'}", "", false, false, RemotePropID=33)]
        public IACContainerTNet<ContDisDustDischarging> DustDischarging { get; set; }

        [ACPropertyBindingTarget(114, "DischargeReleasedExtern", "en{'DischargeReleasedExtern'}de{'DischargeReleasedExtern'}", "", false, false, RemotePropID=34)]
        public IACContainerTNet<ContDisDischargeReleased> DischargeReleasedExtern { get; set; }
        protected IACPropertyNetTarget _DischargeReleasedExtern
        {
            get
            {
                return (IACPropertyNetTarget)this.DischargeReleasedExtern;
            }
        }
        protected ContDisDischargeReleased _DischargeReleasedExternValue
        {
            get { return DischargeReleasedExtern.ValueT; }
        }

        #endregion

        #region Write-Values to PLC

        [ACPropertyBindingTarget(3, "ResetAlarm", "en{'ResetAlarm'}de{'ResetAlarm'}", "", false, false, RemotePropID=35)]
        public IACContainerTNet<UInt16> ResetAlarm { get; set; }

        [ACPropertyBindingTarget(4, "ActionRequest", "en{'ActionRequest'}de{'ActionRequest'}", "", false, false, RemotePropID=36)]
        public IACContainerTNet<ContDisAction> ActionRequest { get; set; }

        [ACPropertyBindingTarget(6, "ActivateVibrator", "en{'ActivateVibrator'}de{'ActivateVibrator'}", "", false, false, RemotePropID=37)]
        public IACContainerTNet<ContDisActivateVibrator> ActivateVibrator { get; set; }

        [ACPropertyBindingTarget(7, "RequestPusher", "en{'RequestPusher'}de{'RequestPusher'}", "", false, false, RemotePropID=38)]
        public IACContainerTNet<ContDisRequestPusher> RequestPusher { get; set; }

        [ACPropertyBindingTarget(8, "VibratorTime", "en{'VibratorTime'}de{'VibratorTime'}", "", false, false, RemotePropID=39)]
        public IACContainerTNet<UInt16> VibratorTime { get; set; }

        [ACPropertyBindingTarget(9, "DischargeReleased", "en{'DischargeReleased'}de{'DischargeReleased'}", "", false, false, RemotePropID=40)]
        public IACContainerTNet<ContDisDischargeReleased> DischargeReleased { get; set; }

        [ACPropertyBindingTarget(10, "Destination", "en{'Destination GIP'}de{'Ziel GIP'}", "", false, false, RemotePropID=41)]
        public IACContainerTNet<UInt16> Destination { get; set; }

        #endregion
        #endregion

        #region overridden methods

        protected virtual void DischargeReleasedExtern_PropertyChanged(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.AfterBroadcast)
                return;
            if ((e.ValueEvent.Sender == EventRaiser.Source) && (e.ValueEvent.EventType == EventTypes.ValueChangedInSource))
            {
                DischargeReleased.ValueT = DischargeReleasedExtern.ValueT;
            }
        }

        protected virtual void AlarmCode_PropertyChanged(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.AfterBroadcast)
                return;
            if ((e.ValueEvent.Sender == EventRaiser.Source) && (e.ValueEvent.EventType == EventTypes.ValueChangedInSource))
            {
                if (ConversionAlarm != null)
                {
                    if (_AlarmCodeValue > 0)
                    {
                        ConversionAlarm.ValueT = PANotifyState.AlarmOrFault;
                        OnAlarmDisappeared(ConversionAlarm);
                        OnNewAlarmOccurred(ConversionAlarm, this.AlarmCode.ValueT.ToString());
                    }   
                    else
                    {
                        ConversionAlarm.ValueT = PANotifyState.Off;
                        OnAlarmDisappeared(ConversionAlarm);
                    }
                }
            }
        }

        protected override bool OnParentServerPropertyFound(IACPropertyNetServer parentProperty)
        {
            //switch (parentProperty.ACIdentifier)
            //{
            //    //case "FunctionErrorL":
            //    //    ActionResponse = parentProperty as IACContainerTNet<ContDisAction>;
            //    //    return true;
            //    //case "ContainerNr":
            //    //    ContainerNr = parentProperty as IACContainerTNet<UInt16>;
            //    //    return true;
            //    //case "ContainerCode":
            //    //    ContainerCode = parentProperty as IACContainerTNet<UInt16>;
            //    //    return true;
            //    //case "GrossWeight":
            //    //    GrossWeight = parentProperty as IACContainerTNet<Int32>;
            //    //    return true;
            //    //case "NetWeight":
            //    //    NetWeight = parentProperty as IACContainerTNet<Int32>;
            //    //    return true;
            //    //case "DischargeNotReleased":
            //    //    DischargeNotReleased = parentProperty as IACContainerTNet<ContDisDischargeNotReleased>;
            //    //    return true;
            //    //case "DustDischarging":
            //    //    DustDischarging = parentProperty as IACContainerTNet<ContDisDustDischarging>;
            //    //    return true;

            //    //case "ActionRequest":
            //    //    ActionRequest = parentProperty as IACContainerTNet<ContDisAction>;
            //    //    return true;
            //    //case "ActivateVibrator":
            //    //    ActivateVibrator = parentProperty as IACContainerTNet<ContDisActivateVibrator>;
            //    //    return true;
            //    //case "RequestPusher":
            //    //    RequestPusher = parentProperty as IACContainerTNet<ContDisRequestPusher>;
            //    //    return true;
            //    //case "VibratorTime":
            //    //    VibratorTime = parentProperty as IACContainerTNet<UInt16>;
            //    //    return true;
            //    //case "DischargeReleased":
            //    //    DischargeReleased = parentProperty as IACContainerTNet<ContDisDischargeReleased>;
            //    //    return true;
            //    //default:
            //    //    break;
            //}

            return base.OnParentServerPropertyFound(parentProperty);
        }

        public override bool SendParams(gip.core.datamodel.ACMethod acMethod)
        {
            bool result = base.SendParams(acMethod);
            ActionRequest.ForceBroadcast = true;
            ActionRequest.ValueT = (ContDisAction)(acMethod.ParameterValueList.GetUInt16("ActionRequest"));
            ActivateVibrator.ForceBroadcast = true;
            ActivateVibrator.ValueT = (ContDisActivateVibrator)(acMethod.ParameterValueList.GetUInt16("ActivateVibrator"));
            RequestPusher.ForceBroadcast = true;
            RequestPusher.ValueT = (ContDisRequestPusher)(acMethod.ParameterValueList.GetUInt16("RequestPusher"));
            VibratorTime.ForceBroadcast = true;
            VibratorTime.ValueT = acMethod.ParameterValueList.GetUInt16("VibratorTime");
            //kein Param kommt von Extern DischargeReleased.ValueT = (ContDisDischargeReleased)(acMethod.ParameterValueList.GetUInt16("ContDisDischargeReleased")); 
            Destination.ForceBroadcast = true;
            Destination.ValueT = acMethod.ParameterValueList.GetUInt16("Destination");

            // Entleerfreigabe immer setzen wenn es nicht von Extern kommt
            IACPropertyNetTarget disProp = DischargeReleasedExtern as IACPropertyNetTarget;
            if (disProp != null && disProp.Source == null)
                DischargeReleased.ValueT = ContDisDischargeReleased.Ja;

            return result;
        }


        public override PAProcessFunction.CompleteResult ReceiveACMethodResult(PAProcessFunction sender, ACMethod acMethod, out MsgWithDetails msg)
        {
            if (acMethod != null)
            {
                acMethod.ResultValueList["AlarmCode"] = _AlarmCodeValue;
                Destination.ValueT = 0;
            }
            msg = null;
            return PAProcessFunction.CompleteResult.Succeeded;
        }


        public override void AcknowledgeAlarms()
        {
            if (!IsEnabledAcknowledgeAlarms())
                return;
            if (this.ConversionAlarm.ValueT == PANotifyState.AlarmOrFault)
            {
                ResetAlarm.ValueT = 1;
                ThreadPool.QueueUserWorkItem((object state) => { Thread.Sleep(1000); ResetAlarm.ValueT = 0; });
            }
            base.AcknowledgeAlarms();
        }

        #endregion
    }
}
