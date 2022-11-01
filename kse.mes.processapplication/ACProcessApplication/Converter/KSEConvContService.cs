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
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'KSEConvContService'}de{'KSEConvContService'}", Global.ACKinds.TPABGModule, Global.ACStorableTypes.NotStorable, false, false)]
    public class KSEConvContService : KSEConvContBase
    {
        #region ctor's

        public KSEConvContService(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
        public IACContainerTNet<ContServiceAlarmCode> AlarmCode { get; set; }
        protected IACPropertyNetTarget _AlarmCode
        {
            get
            {
                return (IACPropertyNetTarget)this.AlarmCode;
            }
        }
        protected ContServiceAlarmCode _AlarmCodeValue
        {
            get { return AlarmCode.ValueT; }
        }


        [ACPropertyBindingTarget(104, "ActionResponse", "en{'ActionResponse'}de{'ActionResponse'}", "", false, false)]
        public IACContainerTNet<ContServiceAction> ActionResponse { get; set; }

        [ACPropertyBindingTarget(105, "ContainerNr", "en{'ContainerNr'}de{'ContainerNr'}", "", false, false)]
        public IACContainerTNet<UInt16> ContainerNr { get; set; }

        [ACPropertyBindingTarget(106, "ContainerCode", "en{'ContainerCode'}de{'ContainerCode'}", "", false, false)]
        public IACContainerTNet<UInt16> ContainerCode { get; set; }

        [ACPropertyBindingTarget(107, "InternalOrderActive", "en{'InternalOrderActive'}de{'InternalOrderActive'}", "", false, false)]
        public IACContainerTNet<ContServiceInternalOrderActive> InternalOrderActive { get; set; }

        #endregion

        #region Write-Values to PLC

        [ACPropertyBindingTarget(3, "ResetAlarm", "en{'ResetAlarm'}de{'ResetAlarm'}", "", false, false)]
        public IACContainerTNet<UInt16> ResetAlarm { get; set; }

        [ACPropertyBindingTarget(4, "ActionRequest", "en{'ActionRequest'}de{'ActionRequest'}", "", false, false)]
        public IACContainerTNet<ContServiceAction> ActionRequest { get; set; }

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
               
            //}

            return base.OnParentServerPropertyFound(parentProperty);
        }

        public override bool SendParams(gip.core.datamodel.ACMethod acMethod)
        {
            bool result = base.SendParams(acMethod);
            ActionRequest.ValueT = (ContServiceAction)acMethod.ParameterValueList.GetUInt16("ActionRequest");
            return result;
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
