using gip.core.autocomponent;
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.communication;

namespace kse.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'KSEConvEquipment'}de{'KSEConvEquipment'}", Global.ACKinds.TPABGModule, Global.ACStorableTypes.NotStorable, false, false)]
    public class KSEConvEquipment : PAStateConverterBase
    {
        #region ctor's

        public KSEConvEquipment(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            _Response.ValueUpdatedOnReceival += Response_PropertyChanged;
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            _Response.ValueUpdatedOnReceival -= Response_PropertyChanged;
            return base.ACDeInit(deleteACClassTask);
        }

        #endregion

        #region Properties

        #region Read-Values from PLC
        [ACPropertyBindingTarget(101, "Response", "en{'Response'}de{'Response'}", "", false, false, RemotePropID=14)]
        public IACContainerTNet<UInt16> Response { get; set; }
        protected IACPropertyNetTarget _Response
        {
            get
            {
                return (IACPropertyNetTarget)this.Response;
            }
        }
        protected UInt16 _ResponseValue
        {
            get { return Response.ValueT; }
        }

        public IACContainerTNet<PANotifyState> FaultState { get; set; }
        public IACContainerTNet<Boolean> OnSiteTurnedOn { get; set; }
        public IACContainerTNet<Boolean> TurnOnInterlock { get; set; }
        public IACContainerTNet<Global.OperatingMode> OperatingMode { get; set; }
        public IACContainerTNet<Boolean> IsTriggered { get; set; }
        public IACContainerTNet<Boolean> RunOpenPos1 { get; set; }
        public IACContainerTNet<Boolean> StopClosedPos2 { get; set; }
        #endregion   
        #endregion

        #region overridden methods              
        protected virtual void Response_PropertyChanged(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.AfterBroadcast)
                return;
            if ((e.ValueEvent.Sender == EventRaiser.Source) && (e.ValueEvent.EventType == EventTypes.ValueChangedInSource))
            {
               RunOpenPos1.ValueT = false;
               StopClosedPos2.ValueT = false;
               FaultState.ValueT = PANotifyState.Off;

               OperatingMode.ValueT = Global.OperatingMode.Automatic;
               switch(Response.ValueT)
               {
                   case 10:
                       RunOpenPos1.ValueT = true;
                       return;
                   case 1:
                       StopClosedPos2.ValueT = true;
                       return;
                   case 12:
                       FaultState.ValueT = PANotifyState.AlarmOrFault;
                       return;
               }
               return;
            }
        }
        protected override bool OnParentServerPropertyFound(IACPropertyNetServer parentProperty)
        {
            switch (parentProperty.ACIdentifier)
            {
                case "FaultState":
                    FaultState = parentProperty as IACContainerTNet<PANotifyState>;
                    return true;
                case "OnSiteTurnedOn":
                    OnSiteTurnedOn = parentProperty as IACContainerTNet<Boolean>;
                    return true;
                case "TurnOnInterlock":
                    TurnOnInterlock = parentProperty as IACContainerTNet<Boolean>;
                    return true;
                case "IsTriggered":
                    IsTriggered = parentProperty as IACContainerTNet<Boolean>;
                    return true;
                case "OperatingMode":
                    OperatingMode = parentProperty as IACContainerTNet<Global.OperatingMode>;
                    return true;
                case "Pos1":
                case "Pos1Open":
                case "RunState":
                    RunOpenPos1 = parentProperty as IACContainerTNet<Boolean>;
                    return true;                             
                case "Pos2":
                    StopClosedPos2 = parentProperty as IACContainerTNet<Boolean>;
                    return true;
            }
            return false;
        }

        protected override void ModelProperty_ValueUpdatedOnReceival(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
                return; 
        }

        #endregion

        
    }
}
