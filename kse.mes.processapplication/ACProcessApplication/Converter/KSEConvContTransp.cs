using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.communication;
using gip.core.autocomponent;
using System.Threading;
using gip.mes.processapplication;

namespace kse.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'KSEConvContTransp'}de{'KSEConvContTransp'}", Global.ACKinds.TPABGModule, Global.ACStorableTypes.NotStorable, false, false)]
    public class KSEConvContTransp : KSEConvContBase
    {
        #region ctor's

        public KSEConvContTransp(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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

        #region Read-Values from PLC

        [ACPropertyBindingTarget(103, "AlarmCode", "en{'AlarmCode'}de{'AlarmCode'}", "", false, false)]
        public IACContainerTNet<ContTranspAlarmCode> AlarmCode { get; set; }
        protected IACPropertyNetTarget _AlarmCode
        {
            get
            {
                return (IACPropertyNetTarget)this.AlarmCode;
            }
        }

        protected ContTranspAlarmCode _AlarmCodeValue
        {
            get { return AlarmCode.ValueT; }
        }

        [ACPropertyBindingTarget(104, "AlarmReference", "en{'AlarmReference'}de{'AlarmReference'}", "", false, false)]
        public IACContainerTNet<UInt16> AlarmReference { get; set; }

        [ACPropertyBindingTarget(105, "ActionResponse", "en{'ActionResponse'}de{'ActionResponse'}", "", false, false)]
        public IACContainerTNet<ContTranspAction> ActionResponse { get; set; }

        [ACPropertyBindingTarget(106, "RequiredPositionResponse", "en{'RequiredPositionResponse'}de{'RequiredPositionResponse'}", "", false, false)]
        public IACContainerTNet<UInt16> RequiredPositionResponse { get; set; }

        [ACPropertyBindingTarget(107, "ContainerNr", "en{'ContainerNr'}de{'ContainerNr'}", "", false, false)]
        public IACContainerTNet<UInt16> ContainerNr { get; set; }

        [ACPropertyBindingTarget(108, "ContainerCode", "en{'ContainerCode'}de{'ContainerCode'}", "", false, false)]
        public IACContainerTNet<UInt16> ContainerCode { get; set; }

        [ACPropertyBindingTarget(109, "LastPosition", "en{'LastPosition'}de{'LastPosition'}", "", false, false)]
        public IACContainerTNet<UInt16> LastPosition { get; set; }

        [ACPropertyBindingTarget(110, "ActualPosition", "en{'ActualPosition'}de{'ActualPosition'}", "", false, false)]
        public IACContainerTNet<UInt16> ActualPosition { get; set; }

        [ACPropertyBindingTarget(111, "ActionPosition", "en{'ActionPosition'}de{'ActionPosition'}", "", false, false)]
        public IACContainerTNet<UInt16> ActionPosition { get; set; }

        [ACPropertyBindingTarget(112, "GrossWeight", "en{'GrossWeight'}de{'GrossWeight'}", "", false, false)]
        public IACContainerTNet<Int32> GrossWeight { get; set; }

        [ACPropertyBindingTarget(114, "WeighingPosition", "en{'WeighingPosition'}de{'WeighingPosition'}", "", false, false)]
        public IACContainerTNet<ContTranspWeighingPos> WeighingPosition { get; set; }

        [ACPropertyBindingTarget(115, "InternalOrderActive", "en{'InternalOrderActive'}de{'InternalOrderActive'}", "", false, false)]
        public IACContainerTNet<ContTranspInternalOrderActive> InternalOrderActive { get; set; }

        #endregion

        #region Write-Values to PLC

        [ACPropertyBindingTarget(3, "ResetAlarm", "en{'ResetAlarm'}de{'ResetAlarm'}", "", false, false)]
        public IACContainerTNet<UInt16> ResetAlarm { get; set; }

        [ACPropertyBindingTarget(4, "ActionRequest", "en{'Action'}de{'ActionRequest'}", "", false, false)]
        public IACContainerTNet<ContTranspAction> ActionRequest { get; set; }

        [ACPropertyBindingTarget(5, "RequiredPositionRequest", "en{'RequiredPositionRequest'}de{'RequiredPositionRequest'}", "", false, false)]
        public IACContainerTNet<UInt16> RequiredPositionRequest { get; set; }

        [ACPropertyBindingTarget(6, "OccupyId", "en{'OccupyId'}de{'OccupyId'}", "", false, false)]
        public IACContainerTNet<UInt16> OccupyId { get; set; }

        [ACPropertyBindingTarget(7, "Release", "en{'Release'}de{'Release'}", "", false, false)]
        public IACContainerTNet<ContTranspRelease> Release { get; set; }

        [ACPropertyBindingTarget(8, "FillWeight", "en{'FillWeight'}de{'FillWeight'}", "", false, false)]
        public IACContainerTNet<Int32> FillWeight { get; set; }

        [ACPropertyBindingTarget(10, "EmptyWeight", "en{'EmptyWeight'}de{'EmptyWeight'}", "", false, false)]
        public IACContainerTNet<Int32> EmptyWeight { get; set; }

        [ACPropertyBindingTarget(11, "ContainerType", "en{'ContainerType'}de{'ContainerType'}", "", false, false)]
        public IACContainerTNet<UInt16> ContainerType { get; set; }

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
            bool result = base.OnParentServerPropertyFound(parentProperty);
            return result;
        }

        public override bool SendParams(gip.core.datamodel.ACMethod acMethod)
        {
            bool result = base.SendParams(acMethod);
            ActionRequest.ForceBroadcast = true;
            ActionRequest.ValueT = (ContTranspAction)acMethod.ParameterValueList.GetUInt16("ActionRequest");
            RequiredPositionRequest.ForceBroadcast = true;
            RequiredPositionRequest.ValueT = acMethod.ParameterValueList.GetUInt16("RequiredPositionRequest");
            // Wenn nicht zuvor von PWContGroupKSE gesetzt, dann schreibe Occupy ID runter
            OccupyId.ForceBroadcast = true;
            if (OccupyId.ValueT == 0)
                OccupyId.ValueT = acMethod.ParameterValueList.GetUInt16("OccupyId");
            // Wird von PWContGroupKSE gesetzt
            //Release.ValueT = (ContTranspRelease)acMethod.ParameterValueList.GetUInt16("Release");
            FillWeight.ValueT = acMethod.ParameterValueList.GetInt32("FillWeight");
            PAMContainerKSE container = FindParentComponent<PAMContainerKSE>(c => c is PAMContainerKSE);
            if (container != null)
            {
                try
                {
                    EmptyWeight.ValueT = Convert.ToInt32(Math.Round(container.ContEmtpyWeight * 1000, 0));
                }
                catch
                {
                    EmptyWeight.ValueT = acMethod.ParameterValueList.GetInt32("EmptyWeight");
                }
            }
            else
                EmptyWeight.ValueT = acMethod.ParameterValueList.GetInt32("EmptyWeight");
            ContainerType.ForceBroadcast = true;
            ContainerType.ValueT = acMethod.ParameterValueList.GetUInt16("ContainerType");
            return result;
        }


        public override PAProcessFunction.CompleteResult ReceiveACMethodResult(PAProcessFunction sender, ACMethod acMethod, out MsgWithDetails msg)
        {
            if (acMethod != null)
            {
                acMethod.ResultValueList["AlarmCode"] = _AlarmCodeValue;
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
