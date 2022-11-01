using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.autocomponent;
using gip.core.communication;

namespace kse.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'KSEConvContReq'}de{'KSEConvContReq'}", Global.ACKinds.TPABGModule, Global.ACStorableTypes.NotStorable, false, false)]
    public class KSEConvContReq : KSEConvContBase
    {
        #region ctor's

        public KSEConvContReq(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }
        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            _AlarmCode.ValueUpdatedOnReceival += AlarmCode_PropertyChanged;
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            _AlarmCode.ValueUpdatedOnReceival -= AlarmCode_PropertyChanged;
            return base.ACDeInit(deleteACClassTask);
        }
        #endregion

        #region Properties
        #region Read-Values from PLC

        [ACPropertyBindingTarget(103, "AlarmCode", "en{'AlarmCode'}de{'AlarmCode'}", "", false, false)]
        public IACContainerTNet<ContReqAlarmCode> AlarmCode { get; set; }
        protected IACPropertyNetTarget _AlarmCode
        {
            get
            {
                return (IACPropertyNetTarget)this.AlarmCode;
            }
        }
        protected ContReqAlarmCode _AlarmCodeValue
        {
            get { return AlarmCode.ValueT; }
        }

        [ACPropertyBindingTarget(104, "ContainerInterfaceNr", "en{'ContainerInterfaceNr'}de{'ContainerInterfaceNr'}", "", false, false)]
        public IACContainerTNet<UInt16> ContainerInterfaceNr { get; set; }

        #endregion

        #region Write-Values to PLC

        [ACPropertyBindingTarget(3, "RequiredPosition", "en{'RequiredPosition'}de{'RequiredPosition'}", "", false, false)]
        public IACContainerTNet<UInt16> RequiredPosition { get; set; }

        [ACPropertyBindingTarget(4, "ContainerType", "en{'ContainerType'}de{'ContainerType'}", "", false, false)]
        public IACContainerTNet<UInt16> ContainerType { get; set; }

        #endregion
        #endregion

        #region overridden methods

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
            //    case "RequiredPosition":
            //        RequiredPosition = parentProperty as IACContainerTNet<UInt16>;
            //        return true;
            //    case "ContainerType":
            //        ContainerType = parentProperty as IACContainerTNet<UInt16>;
            //        return true;
            //    case "ContainerInterfaceNr":
            //        ContainerInterfaceNr = parentProperty as IACContainerTNet<UInt16>;
            //        return true;
              
            //    default:
            //        break;
            //}

            return base.OnParentServerPropertyFound(parentProperty);
        }


        public override bool SendParams(gip.core.datamodel.ACMethod acMethod)
        {
            bool result = base.SendParams(acMethod);
            RequiredPosition.ForceBroadcast = true;
            RequiredPosition.ValueT = acMethod.ParameterValueList.GetUInt16("RequiredPosition");
            ContainerType.ForceBroadcast = true;
            ContainerType.ValueT = acMethod.ParameterValueList.GetUInt16("ContainerType");
            return result;
        }


        public override PAProcessFunction.CompleteResult ReceiveACMethodResult(PAProcessFunction sender, ACMethod acMethod, out MsgWithDetails msg)
        {
            if (acMethod != null)
            {
                acMethod.ResultValueList["ContainerInterfaceNr"] = ContainerInterfaceNr.ValueT;
                acMethod.ResultValueList["AlarmCode"] = _AlarmCodeValue;
                Messages.LogDebug(this.GetACUrl(), "ReceiveACMethodResult()", String.Format("ContainerInterfaceNr: {0}", ContainerInterfaceNr.ValueT));
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
               
            }
            base.AcknowledgeAlarms();
        }

        #endregion

    }
}
