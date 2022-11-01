using gip.core.datamodel;
using gip.core.autocomponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.communication;
using System.Threading;

namespace kse.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'KSEConvContFill'}de{'KSEConvContFill'}", Global.ACKinds.TPABGModule, Global.ACStorableTypes.NotStorable, false, false)]
    public class KSEConvContFill : KSEConvContBase
    {
        #region ctor's

        public KSEConvContFill(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }
        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            _AlarmCode.ValueUpdatedOnReceival += AlarmCode_PropertyChanged;
            _FillingOK.ValueUpdatedOnReceival += FillingOK_PropertyChanged;
            _ButtonFinished.ValueUpdatedOnReceival += _ButtonFinished_ValueUpdatedOnReceival;
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            _AlarmCode.ValueUpdatedOnReceival -= AlarmCode_PropertyChanged;
            _FillingOK.ValueUpdatedOnReceival -= FillingOK_PropertyChanged;
            _ButtonFinished.ValueUpdatedOnReceival -= _ButtonFinished_ValueUpdatedOnReceival;
            return base.ACDeInit(deleteACClassTask);
        }
        #endregion

        #region Read-Values from PLC

        

        [ACPropertyBindingTarget(103, "AlarmCode", "en{'AlarmCode'}de{'AlarmCode'}", "", false, false)]
        public IACContainerTNet<ContFillAlarmCode> AlarmCode { get; set; }
        protected IACPropertyNetTarget _AlarmCode
        {
            get
            {
                return (IACPropertyNetTarget)this.AlarmCode;
            }
        }
        protected ContFillAlarmCode _AlarmCodeValue
        {
            get { return AlarmCode.ValueT; }
        }

        [ACPropertyBindingTarget(104, "AlarmReference", "en{'AlarmReference'}de{'AlarmReference'}", "", false, false)]
        public IACContainerTNet<UInt16> AlarmReference { get; set; }

        [ACPropertyBindingTarget(105, "ContainerNr", "en{'ContainerNr'}de{'ContainerNr'}", "", false, false)]
        public IACContainerTNet<UInt16> ContainerNr { get; set; }

        [ACPropertyBindingTarget(106, "ContainerCode", "en{'ContainerCode'}de{'ContainerCode'}", "", false, false)]
        public IACContainerTNet<UInt16> ContainerCode { get; set; }

        [ACPropertyBindingTarget(107, "GrossWeight", "en{'GrossWeight'}de{'GrossWeight'}", "", false, false)]
        public IACContainerTNet<Int32> GrossWeight { get; set; }

        [ACPropertyBindingTarget(109, "NetWeight", "en{'NetWeight'}de{'NetWeight'}", "", false, false)]
        public IACContainerTNet<Int32> NetWeight { get; set; }

        [ACPropertyBindingTarget(111, "FillingOK", "en{'FillingOK'}de{'FillingOK'}", "", false, false)]
        public IACContainerTNet<FillingOK> FillingOK { get; set; }
        protected IACPropertyNetTarget _FillingOK
        {
            get
            {
                return (IACPropertyNetTarget)this.FillingOK;
            }
        }
        protected FillingOK _FillingOKValue
        {
            get { return FillingOK.ValueT; }
        }

        [ACPropertyBindingTarget(112, "FillingOKExtern", "en{'FillingOKExtern'}de{'FillingOKExtern'}", "", false, false)]
        public IACContainerTNet<FillingOK> FillingOKExtern { get; set; }


        [ACPropertyBindingTarget(111, "ButtonFinished", "en{'ButtonFinished'}de{'ButtonFinished'}", "", false, false)]
        public IACContainerTNet<UInt16> ButtonFinished { get; set; }
        protected IACPropertyNetTarget _ButtonFinished
        {
            get
            {
                return (IACPropertyNetTarget)this.ButtonFinished;
            }
        }
        protected UInt16 _ButtonFinishedValue
        {
            get { return ButtonFinished.ValueT; }
        }


        #endregion

        #region Write-Values to PLC

        [ACPropertyBindingTarget(3, "ResetAlarm", "en{'ResetAlarm'}de{'ResetAlarm'}", "", false, false)]
        public IACContainerTNet<UInt16> ResetAlarm { get; set; }

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

        protected virtual void FillingOK_PropertyChanged(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.AfterBroadcast)
                return;
            if ((e.ValueEvent.Sender == EventRaiser.Source) && (e.ValueEvent.EventType == EventTypes.ValueChangedInSource))
            {
                if (ConversionAlarm != null)
                {
                    FillingOKExtern.ValueT = _FillingOKValue;
                }
            }
        }

        void _ButtonFinished_ValueUpdatedOnReceival(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.AfterBroadcast)
                return;
            if ((e.ValueEvent.Sender == EventRaiser.Source) && (e.ValueEvent.EventType == EventTypes.ValueChangedInSource))
            {
                if (_ButtonFinishedValue == 1 && _FillingOKValue == processapplication.FillingOK.Ja)
                {
                    PAProcessFunction function = ParentACComponent as PAProcessFunction;
                    if (function != null)
                        function.Stopp();
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

        public override PAProcessFunction.CompleteResult ReceiveACMethodResult(PAProcessFunction sender, ACMethod acMethod, out MsgWithDetails msg)
        {
            if (acMethod != null)
            {
                acMethod.ResultValueList["AlarmCode"] = _AlarmCodeValue;
                if (_FillingOKValue == processapplication.FillingOK.Ja)
                {
                    //FillingOKExtern.ValueT = processapplication.FillingOK.Nein;
                    Messages.LogError(this.GetACUrl(), "ReceiveACMethodResult()", "Füllfreigabe von KSE nicht zurückgesetzt");
                }
            }
            msg = null;
            return PAProcessFunction.CompleteResult.Succeeded;
        }

        #endregion
    }   
}
