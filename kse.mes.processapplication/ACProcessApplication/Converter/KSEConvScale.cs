using gip.core.autocomponent;
using gip.core.communication;
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'KSEConvScale'}de{'KSEConvScale'}", Global.ACKinds.TPABGModule, Global.ACStorableTypes.NotStorable, false, false)]
    public partial class KSEConvScale : PAFuncStateConvBase
    {
        public const string ClassName = "KSEConvScale";

        #region ctor's
        public KSEConvScale(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;

            KSEConvContBase.LifeSignalCycle += ApplicationManager_ProjectWorkCycleR1sec;
            ((IACPropertyNetTarget)LifeSignalAcknowledge).ValueUpdatedOnReceival += LifeSignalAcknowledge_PropertyChanged;

            ((IACPropertyNetTarget)RBResultCode).ValueUpdatedOnReceival += ReadValues_PropertyChanged;
            ((IACPropertyNetTarget)RBResultValid).ValueUpdatedOnReceival += ReadValues_PropertyChanged;
            ((IACPropertyNetTarget)CRBCommandResponseCode).ValueUpdatedOnReceival += ReadValues_PropertyChanged;
            ((IACPropertyNetTarget)CRBCommandAcknowledge).ValueUpdatedOnReceival += ReadValues_PropertyChanged;
            ((IACPropertyNetTarget)ADMBMachineState).ValueUpdatedOnReceival += ReadValues_PropertyChanged;
            ((IACPropertyNetTarget)ADMBDosingOrDischargingProcessAlarm).ValueUpdatedOnReceival += ReadValues_PropertyChanged;
            ((IACPropertyNetTarget)ADMBPositioningEquipmentAlarm).ValueUpdatedOnReceival += ReadValues_PropertyChanged;
            ((IACPropertyNetTarget)ADMBDustExtractionEquipmentAlarm).ValueUpdatedOnReceival += ReadValues_PropertyChanged;
            _DischargeReleasedExtern.ValueUpdatedOnReceival += DischargeReleasedExtern_PropertyChanged;

#if DEBUG
            if (!LoggingEnabled)
                LoggingEnabled = true;
#endif

            return true;
        }

        public override bool ACPostInit()
        {
            bool result = base.ACPostInit();
            BindMyProperties();
            return result;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            KSEConvContBase.LifeSignalCycle -= ApplicationManager_ProjectWorkCycleR1sec;
            ((IACPropertyNetTarget)LifeSignalAcknowledge).ValueUpdatedOnReceival -= LifeSignalAcknowledge_PropertyChanged;

            ((IACPropertyNetTarget)RBResultCode).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            ((IACPropertyNetTarget)RBResultValid).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            //((IACPropertyNetTarget)RBDischargePosition).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            //((IACPropertyNetTarget)RBWeigherNumber).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            //((IACPropertyNetTarget)RBResultDosedWeight).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            //((IACPropertyNetTarget)RBDosingTime).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            //((IACPropertyNetTarget)RBPosition).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            //((IACPropertyNetTarget)RBBatchID).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            //((IACPropertyNetTarget)RBAlarmHandlingCode).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;

            //((IACPropertyNetTarget)AAMBAlarmPositioning).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            //((IACPropertyNetTarget)AAMBAlarmWeigher).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            //((IACPropertyNetTarget)AAMBAlarmDustExhaust).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            //((IACPropertyNetTarget)AAMBAlarmDosingSlides1).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            //((IACPropertyNetTarget)AAMBAlarmDosingSlides2).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            //((IACPropertyNetTarget)AAMBAlarmDosingScrew1).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            //((IACPropertyNetTarget)AAMBAlarmDosingScrew2).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            //((IACPropertyNetTarget)AAMBAlarmDosingFrame1).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            //((IACPropertyNetTarget)AAMBAlarmDosingFrame2).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            //((IACPropertyNetTarget)AAMBAlarmStirringDevices1).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            //((IACPropertyNetTarget)AAMBAlarmStirringDevices2).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            //((IACPropertyNetTarget)AAMBAlarmGeneral1).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            //((IACPropertyNetTarget)AAMBAlarmGeneral2).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            //((IACPropertyNetTarget)AAMBAlarmGeneral3).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;

            //((IACPropertyNetTarget)CRBCommandNumber).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            ((IACPropertyNetTarget)CRBCommandResponseCode).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            ((IACPropertyNetTarget)CRBCommandAcknowledge).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;

            //((IACPropertyNetTarget)ADMBMachineMode).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            ((IACPropertyNetTarget)ADMBMachineState).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            ((IACPropertyNetTarget)ADMBDosingOrDischargingProcessAlarm).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            //((IACPropertyNetTarget)ADMBDischargingEquipmentAlarm).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            ((IACPropertyNetTarget)ADMBPositioningEquipmentAlarm).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            ((IACPropertyNetTarget)ADMBDustExtractionEquipmentAlarm).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            //((IACPropertyNetTarget)ADMBProcessOperation).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            //((IACPropertyNetTarget)ADMBActualBinContNumber).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            //((IACPropertyNetTarget)ADMBActualDischargePosition).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            //((IACPropertyNetTarget)ADMBActualWeigherNumber).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            //((IACPropertyNetTarget)ADMBActualWeight).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            //((IACPropertyNetTarget)ADMBActualDosingTime).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            //((IACPropertyNetTarget)ADMBWeigher1GrossWeight).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            //((IACPropertyNetTarget)ADMBWeigher2GrossWeight).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            //((IACPropertyNetTarget)ADMBActualPosition).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            //((IACPropertyNetTarget)ADMBBatchIDAlfraScale).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            //((IACPropertyNetTarget)ADMBStatusAlfraTransport).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            //((IACPropertyNetTarget)ADMBBatchIDAlfraTransport).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            //((IACPropertyNetTarget)ADMBStatusAlfraCheckWeigher).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            //((IACPropertyNetTarget)ADMBBatchIDCheckWeigher).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            //((IACPropertyNetTarget)ACMBRequestedWeightReached).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            //((IACPropertyNetTarget)ACMBRequestDischargeTransport).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            //((IACPropertyNetTarget)ACMBAptTransportValidCheckResults).ValueUpdatedOnReceival -= ReadValues_PropertyChanged;
            _DischargeReleasedExtern.ValueUpdatedOnReceival -= DischargeReleasedExtern_PropertyChanged;

            return base.ACDeInit(deleteACClassTask);
        }


        private short _PropertiesBindPhase = 0;
        private void BindMyProperties()
        {
            if (_PropertiesBindPhase >= 2)
                return;
            _PropertiesBindPhase = 1;
            IACPropertyNetTarget newTarget = null;
            if ((AAMBAlarmDosingFrame1 as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", AAMBAlarmDosingFrame1 as IACPropertyNetTarget, "AAMBAlarmDosingFrame1", out newTarget);
            if ((AAMBAlarmDosingFrame2 as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", AAMBAlarmDosingFrame2 as IACPropertyNetTarget, "AAMBAlarmDosingFrame2", out newTarget);
            if ((AAMBAlarmDosingScrew1 as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", AAMBAlarmDosingScrew1 as IACPropertyNetTarget, "AAMBAlarmDosingScrew1", out newTarget);
            if ((AAMBAlarmDosingScrew2 as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", AAMBAlarmDosingScrew2 as IACPropertyNetTarget, "AAMBAlarmDosingScrew2", out newTarget);
            if ((AAMBAlarmDosingSlides1 as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", AAMBAlarmDosingSlides1 as IACPropertyNetTarget, "AAMBAlarmDosingSlides1", out newTarget);
            if ((AAMBAlarmDosingSlides2 as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", AAMBAlarmDosingSlides2 as IACPropertyNetTarget, "AAMBAlarmDosingSlides2", out newTarget);
            if ((AAMBAlarmDustExhaust as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", AAMBAlarmDustExhaust as IACPropertyNetTarget, "AAMBAlarmDustExhaust", out newTarget);
            if ((AAMBAlarmGeneral1 as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", AAMBAlarmGeneral1 as IACPropertyNetTarget, "AAMBAlarmGeneral1", out newTarget);
            if ((AAMBAlarmGeneral2 as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", AAMBAlarmGeneral2 as IACPropertyNetTarget, "AAMBAlarmGeneral2", out newTarget);
            if ((AAMBAlarmGeneral3 as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", AAMBAlarmGeneral3 as IACPropertyNetTarget, "AAMBAlarmGeneral3", out newTarget);
            if ((AAMBAlarmPositioning as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", AAMBAlarmPositioning as IACPropertyNetTarget, "AAMBAlarmPositioning", out newTarget);
            if ((AAMBAlarmStirringDevices1 as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", AAMBAlarmStirringDevices1 as IACPropertyNetTarget, "AAMBAlarmStirringDevices1", out newTarget);
            if ((AAMBAlarmStirringDevices2 as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", AAMBAlarmStirringDevices2 as IACPropertyNetTarget, "AAMBAlarmStirringDevices2", out newTarget);
            if ((AAMBAlarmWeigher as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", AAMBAlarmWeigher as IACPropertyNetTarget, "AAMBAlarmWeigher", out newTarget);
            if ((ACMBAptTransportValidCheckResults as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", ACMBAptTransportValidCheckResults as IACPropertyNetTarget, "ACMBAptTransportValidCheckResults", out newTarget);
            if ((ACMBFillingAllowed as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", ACMBFillingAllowed as IACPropertyNetTarget, "ACMBFillingAllowed", out newTarget);
            if ((ACMBRequestDischargeTransport as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", ACMBRequestDischargeTransport as IACPropertyNetTarget, "ACMBRequestDischargeTransport", out newTarget);
            if ((ACMBRequestedWeightReached as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", ACMBRequestedWeightReached as IACPropertyNetTarget, "ACMBRequestedWeightReached", out newTarget);
            if ((ADMBActualBinContNumber as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", ADMBActualBinContNumber as IACPropertyNetTarget, "ADMBActualBinContNumber", out newTarget);
            if ((ADMBActualDischargePosition as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", ADMBActualDischargePosition as IACPropertyNetTarget, "ADMBActualDischargePosition", out newTarget);
            if ((ADMBActualDosingTime as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", ADMBActualDosingTime as IACPropertyNetTarget, "ADMBActualDosingTime", out newTarget);
            if ((ADMBActualPosition as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", ADMBActualPosition as IACPropertyNetTarget, "ADMBActualPosition", out newTarget);
            if ((ADMBActualWeigherNumber as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", ADMBActualWeigherNumber as IACPropertyNetTarget, "ADMBActualWeigherNumber", out newTarget);
            if ((ADMBActualWeight as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", ADMBActualWeight as IACPropertyNetTarget, "ADMBActualWeight", out newTarget);
            if ((ADMBAlreadyDosedWeightWeight as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", ADMBAlreadyDosedWeightWeight as IACPropertyNetTarget, "ADMBAlreadyDosedWeightWeight", out newTarget);
            if ((ADMBBatchIDAlfraScale as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", ADMBBatchIDAlfraScale as IACPropertyNetTarget, "ADMBBatchIDAlfraScale", out newTarget);
            if ((ADMBBatchIDAlfraTransport as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", ADMBBatchIDAlfraTransport as IACPropertyNetTarget, "ADMBBatchIDAlfraTransport", out newTarget);
            if ((ADMBBatchIDCheckWeigher as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", ADMBBatchIDCheckWeigher as IACPropertyNetTarget, "ADMBBatchIDCheckWeigher", out newTarget);
            if ((ADMBDischargingEquipmentAlarm as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", ADMBDischargingEquipmentAlarm as IACPropertyNetTarget, "ADMBDischargingEquipmentAlarm", out newTarget);
            if ((ADMBDosingOrDischargingProcessAlarm as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", ADMBDosingOrDischargingProcessAlarm as IACPropertyNetTarget, "ADMBDosingOrDischargingProcessAlarm", out newTarget);
            if ((ADMBDustExtractionEquipmentAlarm as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", ADMBDustExtractionEquipmentAlarm as IACPropertyNetTarget, "ADMBDustExtractionEquipmentAlarm", out newTarget);
            if ((ADMBMachineMode as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", ADMBMachineMode as IACPropertyNetTarget, "ADMBMachineMode", out newTarget);
            if ((ADMBMachineState as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", ADMBMachineState as IACPropertyNetTarget, "ADMBMachineState", out newTarget);
            // < Version 3.0
            if ((ADMBPositioningEquipmentAlarm as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", ADMBPositioningEquipmentAlarm as IACPropertyNetTarget, "ADMBPositioningEquipmentAlarm", out newTarget);
            // >= Version 3.0
            if ((ADMBVariousEquipmentsAlarm as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", ADMBVariousEquipmentsAlarm as IACPropertyNetTarget, "ADMBVariousEquipmentsAlarm", out newTarget);
            if ((ADMBProcessOperation as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", ADMBProcessOperation as IACPropertyNetTarget, "ADMBProcessOperation", out newTarget);
            if ((ADMBStatusAlfraCheckWeigher as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", ADMBStatusAlfraCheckWeigher as IACPropertyNetTarget, "ADMBStatusAlfraCheckWeigher", out newTarget);
            if ((ADMBStatusAlfraTransport as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", ADMBStatusAlfraTransport as IACPropertyNetTarget, "ADMBStatusAlfraTransport", out newTarget);
            if ((ADMBWeigher1GrossWeight as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", ADMBWeigher1GrossWeight as IACPropertyNetTarget, "ADMBWeigher1GrossWeight", out newTarget);
            if ((ADMBWeigher2GrossWeight as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", ADMBWeigher2GrossWeight as IACPropertyNetTarget, "ADMBWeigher2GrossWeight", out newTarget);
            if ((CRBCommandAcknowledge as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", CRBCommandAcknowledge as IACPropertyNetTarget, "CRBCommandAcknowledge", out newTarget);
            if ((CRBCommandNumber as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", CRBCommandNumber as IACPropertyNetTarget, "CRBCommandNumber", out newTarget);
            if ((CRBCommandResponseCode as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", CRBCommandResponseCode as IACPropertyNetTarget, "CRBCommandResponseCode", out newTarget);
            if ((LifeSignalAcknowledge as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", LifeSignalAcknowledge as IACPropertyNetTarget, "LifeSignalAcknowledge", out newTarget);
            if ((RBAlarmHandlingCode as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", RBAlarmHandlingCode as IACPropertyNetTarget, "RBAlarmHandlingCode", out newTarget);
            if ((RBBatchID as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", RBBatchID as IACPropertyNetTarget, "RBBatchID", out newTarget);
            if ((RBBinContNumber as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", RBBinContNumber as IACPropertyNetTarget, "RBBinContNumber", out newTarget);
            if ((RBCommandNumber as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", RBCommandNumber as IACPropertyNetTarget, "RBCommandNumber", out newTarget);
            if ((RBDischargePosition as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", RBDischargePosition as IACPropertyNetTarget, "RBDischargePosition", out newTarget);
            if ((RBDosingTime as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", RBDosingTime as IACPropertyNetTarget, "RBDosingTime", out newTarget);
            if ((RBPosition as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", RBPosition as IACPropertyNetTarget, "RBPosition", out newTarget);
            if ((RBProcessResultAlarmCode as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", RBProcessResultAlarmCode as IACPropertyNetTarget, "RBProcessResultAlarmCode", out newTarget);
            if ((RBResultCode as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", RBResultCode as IACPropertyNetTarget, "RBResultCode", out newTarget);
            if ((RBResultDosedWeight as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", RBResultDosedWeight as IACPropertyNetTarget, "RBResultDosedWeight", out newTarget);
            if ((RBResultValid as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", RBResultValid as IACPropertyNetTarget, "RBResultValid", out newTarget);
            if ((RBWeigherNumber as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Read", RBWeigherNumber as IACPropertyNetTarget, "RBWeigherNumber", out newTarget);

            if ((ACBApproveAlarm as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Write", ACBApproveAlarm as IACPropertyNetTarget, "ACBApproveAlarm", out newTarget);
            if ((ACBAptTransportRestart as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Write", ACBAptTransportRestart as IACPropertyNetTarget, "ACBAptTransportRestart", out newTarget);
            if ((ACBAptTransportResultAcknowledge as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Write", ACBAptTransportResultAcknowledge as IACPropertyNetTarget, "ACBAptTransportResultAcknowledge", out newTarget);
            if ((ACBExternalDischargeRelease as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Write", ACBExternalDischargeRelease as IACPropertyNetTarget, "ACBExternalDischargeRelease", out newTarget);
            if ((ACBExternalDischargeReleaseAptTransport as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Write", ACBExternalDischargeReleaseAptTransport as IACPropertyNetTarget, "ACBExternalDischargeReleaseAptTransport", out newTarget);
            if ((ACBExternalTransportReady as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Write", ACBExternalTransportReady as IACPropertyNetTarget, "ACBExternalTransportReady", out newTarget);
            if ((ACBRejectAlarm as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Write", ACBRejectAlarm as IACPropertyNetTarget, "ACBRejectAlarm", out newTarget);
            if ((ACBResetAlarmStatus as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Write", ACBResetAlarmStatus as IACPropertyNetTarget, "ACBResetAlarmStatus", out newTarget);
            if ((ACBRestartAlarm as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Write", ACBRestartAlarm as IACPropertyNetTarget, "ACBRestartAlarm", out newTarget);
            if ((ACBStopCommand as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Write", ACBStopCommand as IACPropertyNetTarget, "ACBStopCommand", out newTarget);
            if ((ASRResetMachine as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Write", ASRResetMachine as IACPropertyNetTarget, "ASRResetMachine", out newTarget);
            if ((CBBatchEnd as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Write", CBBatchEnd as IACPropertyNetTarget, "CBBatchEnd", out newTarget);
            if ((CBBatchID as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Write", CBBatchID as IACPropertyNetTarget, "CBBatchID", out newTarget);
            if ((CBCheckTare as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Write", CBCheckTare as IACPropertyNetTarget, "CBCheckTare", out newTarget);
            if ((CBCommandNumber as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Write", CBCommandNumber as IACPropertyNetTarget, "CBCommandNumber", out newTarget);
            if ((CBCommandOperationMode as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Write", CBCommandOperationMode as IACPropertyNetTarget, "CBCommandOperationMode", out newTarget);
            if ((CBCommandValid as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Write", CBCommandValid as IACPropertyNetTarget, "CBCommandValid", out newTarget);
            if ((CBDischargingPosition as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Write", CBDischargingPosition as IACPropertyNetTarget, "CBDischargingPosition", out newTarget);
            if ((CBDosingAccuracy as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Write", CBDosingAccuracy as IACPropertyNetTarget, "CBDosingAccuracy", out newTarget);
            if ((CBDosingBinNumber as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Write", CBDosingBinNumber as IACPropertyNetTarget, "CBDosingBinNumber", out newTarget);
            if ((CBDosingMode as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Write", CBDosingMode as IACPropertyNetTarget, "CBDosingMode", out newTarget);
            if ((CBIndicationDosability as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Write", CBIndicationDosability as IACPropertyNetTarget, "CBIndicationDosability", out newTarget);
            if ((CBMassDensity as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Write", CBMassDensity as IACPropertyNetTarget, "CBMassDensity", out newTarget);
            if ((CBPosDosDestination as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Write", CBPosDosDestination as IACPropertyNetTarget, "CBPosDosDestination", out newTarget);
            if ((CBRequestedWeight as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Write", CBRequestedWeight as IACPropertyNetTarget, "CBRequestedWeight", out newTarget);
            if ((CBToleranceNegative as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Write", CBToleranceNegative as IACPropertyNetTarget, "CBToleranceNegative", out newTarget);
            if ((CBTolerancePositive as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Write", CBTolerancePositive as IACPropertyNetTarget, "CBTolerancePositive", out newTarget);
            if ((CBWeigherNumber as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Write", CBWeigherNumber as IACPropertyNetTarget, "CBWeigherNumber", out newTarget);
            if ((LifeSignalRequest as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Write", LifeSignalRequest as IACPropertyNetTarget, "LifeSignalRequest", out newTarget);
            if ((RRBResultAcknowledge as IACPropertyNetTarget).Source == null && Session != null)
                BindPropertyWithLog("Write", RRBResultAcknowledge as IACPropertyNetTarget, "RRBResultAcknowledge", out newTarget);

            _PropertiesBindPhase = 2;
        }

        protected PropBindingBindingResult BindPropertyWithLog(string subscriptionName, IACPropertyNetTarget targetProp, string acIdentifierProp, out IACPropertyNetTarget newTarget, bool bindInDBIfConverterNeeded = true)
        {
            PropBindingBindingResult bindingResult = BindProperty(subscriptionName, targetProp, acIdentifierProp, out newTarget);
            if (!(bindingResult.HasFlag(PropBindingBindingResult.Succeeded)
                || bindingResult.HasFlag(PropBindingBindingResult.TargetPropReplaced)
                || bindingResult.HasFlag(PropBindingBindingResult.TypeOfSourceWasChanged)
                || bindingResult.HasFlag(PropBindingBindingResult.AlreadyBound)))
                    Messages.LogError(this.GetACUrl(), "BindMyProperties()", acIdentifierProp + " not bounded to source");
            return bindingResult;
        }
        #endregion


        #region Properties

        public override bool IsSimulationOn
        {
            get
            {
                if (IsConnectedForSending)
                    return false;
                return base.IsSimulationOn;
            }
        }

        #region Attached Properties
        public PAProcessFunction CurrentActiveFunction
        {
            get
            {
                PAProcessFunction function = DosingFunction;
                if (function != null)
                {
                    if (function.CurrentACState != ACStateEnum.SMIdle)
                        return function;
                }
                function = DischargingFunction;
                if (function != null)
                {
                    if (function.CurrentACState != ACStateEnum.SMIdle)
                        return function;
                }
                return null;
            }
        }

        public ACStateEnum CurrentACState
        {
            get
            {
                var function = CurrentActiveFunction;
                if (function == null)
                    return ACStateEnum.SMIdle;
                return function.CurrentACState;
            }
        }

        public PAFDosingKSE DosingFunction
        {
            get
            {
                return ParentACComponent.FindChildComponents<PAFDosingKSE>(c => c is PAFDosingKSE).FirstOrDefault();
            }
        }

        public PAFDischargingKSE DischargingFunction
        {
            get
            {
                return ParentACComponent.FindChildComponents<PAFDischargingKSE>(c => c is PAFDischargingKSE).FirstOrDefault();
            }
        }
        #endregion


        #region Local Properties

        public bool IsConnectedForSending
        {
            get
            {
                bool isReadyForSending = true;

                // TODO: Aktiviere wenn GIP-SPS verfügbar
                //if (!IsReadyForWritingExtern.ValueT)
                //    isReadyForSending = false;
                if (isReadyForSending && this.Session != null)
                {
                    ACSession acSession = this.Session as ACSession;
                    if (acSession != null && !acSession.IsReadyForWriting)
                        isReadyForSending = false;
                    else if (acSession == null && !(bool)this.Session.ACUrlCommand("IsReadyForWriting"))
                        isReadyForSending = false;
                }
                return isReadyForSending;

            }
        }

        public override bool IsReadyForSending
        {
            get
            {
                if (!this.Root.Initialized)
                    return false;
                if (IsSimulationOn)
                    return true;
                return IsConnectedForSending;
            }
        }

        public override bool IsReadyForReading
        {
            get
            {
                return IsReadyForSending;
            }
        }

        [ACPropertyInfo(true, 9999, DefaultValue = 0)]
        public Int16 RBCommandNumberGenerate { get; set; }

        #region Cyclic and private
        DateTime? _LastToggleTime = null;
        #endregion

        #endregion


        #region READ-REGISTERS (101 - 164)

        #region 4.2 COMMAND RESPONSE BLOCK (129 - 131)
        /// <summary>
        /// 4.2. COMMAND RESPONSE BLOCK, Holding-Register: 129
        /// </summary>
        [ACPropertyBindingTarget(129, "CRBCommandNumber", "en{'Command-No. (129)'}de{'Kommando-Nr. (129)'}", "", false, false)]
        public IACContainerTNet<UInt16> CRBCommandNumber { get; set; }

        /// <summary>
        /// 4.2. COMMAND RESPONSE BLOCK, Holding-Register: 130
        /// </summary>
        [ACPropertyBindingTarget(130, "CRBCommandResponseCode", "en{'Start-Responsecode (Cmd. rsp. 130)'}de{'Start-Antwortcode (Cmd. rsp. 130)'}", "", false, false)]
        public IACContainerTNet<CRB_CommandResponseCode> CRBCommandResponseCode { get; set; }

        /// <summary>
        /// 4.2. COMMAND RESPONSE BLOCK, Holding-Register: 131
        /// </summary>
        [ACPropertyBindingTarget(131, "CRBCommandAcknowledge", "en{'Start accepted (Cmd. Ack. 131)'}de{'Start angenommen (Cmd. Ack. 131)'}", "", false, false)]
        public IACContainerTNet<CRB_CommandAcknowledge> CRBCommandAcknowledge { get; set; }
        #endregion


        #region 4.3 RESULT-BLOCK  (101 - 112, 132)
        /// <summary>
        /// 4.3. RESULT BLOCK, Holding-Register: 101
        /// </summary>
        [ACPropertyBindingTarget(101, "RBCommandNumber", "en{'Command-No. (101)'}de{'Kommando-Nr. (101)'}", "", false, false)]
        public IACContainerTNet<UInt16> RBCommandNumber { get; set; }

        /// <summary>
        /// 4.3. RESULT BLOCK, Holding-Register: 102
        /// </summary>
        [ACPropertyBindingTarget(102, "RBResultCode", "en{'Function (Res.Code 102)'}de{'Funktion (Res.Code 102)'}", "", false, false)]
        public IACContainerTNet<RB_ResultCode> RBResultCode { get; set; }

        /// <summary>
        /// 4.3. RESULT BLOCK, Holding-Register: 103
        /// </summary>
        [ACPropertyBindingTarget(103, "RB_ProcessResultAlarmCode", "en{'Result Alarm (Alarmc. 103)'}de{'Ergebnis Alarm (Alarmc. 103)'}", "", false, false)]
        public IACContainerTNet<RB_ProcessResultAlarmCode> RBProcessResultAlarmCode { get; set; }

        /// <summary>
        /// 4.3. RESULT BLOCK, Holding-Register: 104
        /// </summary>
        [ACPropertyBindingTarget(104, "RBBinContNumber", "en{'Dosing-source (Bin-No. 104)'}de{'Dosierquelle (104)'}", "", false, false)]
        public IACContainerTNet<UInt16> RBBinContNumber { get; set; }

        /// <summary>
        /// 4.3. RESULT BLOCK, Holding-Register: 105
        /// </summary>
        [ACPropertyBindingTarget(105, "RBDischargePosition", "en{'Discharging position (105)'}de{'Entleerposition (105)'}", "", false, false)]
        public IACContainerTNet<UInt16> RBDischargePosition { get; set; }

        /// <summary>
        /// 4.3. RESULT BLOCK, Holding-Register: 106
        /// </summary>
        [ACPropertyBindingTarget(106, "RBWeigherNumber", "en{'Weigher (106)'}de{'Waage. (106)'}", "", false, false)]
        public IACContainerTNet<CB_WeigherNumber> RBWeigherNumber { get; set; }

        /// <summary>
        /// 4.3. RESULT BLOCK, Holding-Register: 107 + 108
        /// </summary>
        [ACPropertyBindingTarget(107, "RBResultDosedWeight", "en{'Result-Quantity [g]  (107+8)'}de{'Istgewicht [g] (107+8)'}", "", false, false)]
        public IACContainerTNet<Int32> RBResultDosedWeight { get; set; }

        /// <summary>
        /// 4.3. RESULT BLOCK, Holding-Register: 109
        /// </summary>
        [ACPropertyBindingTarget(109, "RBDosingTime", "en{'Dosing time [s] (109)'}de{'Dosierzeit [s] (109)'}", "", false, false)]
        public IACContainerTNet<UInt16> RBDosingTime { get; set; }

        /// <summary>
        /// 4.3. RESULT BLOCK, Holding-Register: 110
        /// </summary>
        [ACPropertyBindingTarget(110, "RBPosition", "en{'Position (110)'}de{'Position (110)'}", "", false, false)]
        public IACContainerTNet<UInt16> RBPosition { get; set; }

        /// <summary>
        /// 4.3. RESULT BLOCK, Holding-Register: 111
        /// </summary>
        [ACPropertyBindingTarget(111, "RBBatchID", "en{'Batch-ID (11)'}de{'Batch-ID (111)'}", "", false, false)]
        public IACContainerTNet<UInt16> RBBatchID { get; set; }

        /// <summary>
        /// 4.3. RESULT BLOCK, Holding-Register: 112
        /// </summary>
        [ACPropertyBindingTarget(112, "RBAlarmHandlingCode", "en{'Alarm-Handling Code (112)'}de{'Alarm-Handling Code (112)'}", "", false, false)]
        public IACContainerTNet<RB_AlarmHandlingCode> RBAlarmHandlingCode { get; set; }

        /// <summary>
        /// 4.3. RESULT BLOCK, Holding-Register: 132, Handshake mit RRBResultAcknowledge
        /// </summary>
        [ACPropertyBindingTarget(132, "RBResultValid", "en{'Read result (Res. valid 132)'}de{'Ergebnis lesen (Res. valid 132)'}", "", false, false)]
        public IACContainerTNet<RB_ResultValid> RBResultValid { get; set; }
        #endregion


        #region 5.1. ASYNCHRONOUS DATA MONITORING BLOCK (133 - 155)
        /// <summary>
        /// 5.1. ASYNCHRONOUS DATA MONITORING BLOCK, Holding-Register 133
        /// </summary>
        [ACPropertyBindingTarget(133, "ADMBMachineMode", "en{'Machine mode (133)'}de{'Betriebsart (Mach.-Mode 133)'}", "", false, false)]
        public IACContainerTNet<ADMB_MachineMode> ADMBMachineMode { get; set; }

        /// <summary>
        /// 5.1. ASYNCHRONOUS DATA MONITORING BLOCK, Holding-Register 134
        /// </summary>
        [ACPropertyBindingTarget(134, "ADMBMachineState", "en{'Machine state (134)'}de{'Maschinenstatus (134)'}", "", false, false)]
        public IACContainerTNet<ADMB_MachineState> ADMBMachineState { get; set; }

        /// <summary>
        /// 5.1. ASYNCHRONOUS DATA MONITORING BLOCK, Holding-Register 135
        /// </summary>
        [ACPropertyBindingTarget(135, "RB_ProcessResultAlarmCode", "en{'Dosing-/Discharging process alarm (135)'}de{'Dosier-/Entleerprozessalarm (135)'}", "", false, false)]
        public IACContainerTNet<RB_ProcessResultAlarmCode> ADMBDosingOrDischargingProcessAlarm { get; set; }

        /// <summary>
        /// 5.1. ASYNCHRONOUS DATA MONITORING BLOCK, Holding-Register 136
        /// </summary>
        [ACPropertyBindingTarget(136, "RB_ProcessResultAlarmCode", "en{'Discharging Equipment Alarm (136)'}de{'Gerätealarm Entleeren (136)'}", "", false, false)]
        public IACContainerTNet<RB_ProcessResultAlarmCode> ADMBDischargingEquipmentAlarm { get; set; }

        /// <summary>
        /// 5.1. ASYNCHRONOUS DATA MONITORING BLOCK, Holding-Register 137
        /// Only to Version 2.2, Deprecated in Version 3.2
        /// </summary>
        [ACPropertyBindingTarget(137, "ADMBPositioningEquipmentAlarm", "en{'Positioning Equipment Alarm (137)'}de{'Gerätealarm Positionieren (137)'}", "", false, false)]
        public IACContainerTNet<ADMB_PositioningEquipmentAlarm> ADMBPositioningEquipmentAlarm { get; set; }

        /// <summary>
        /// 5.1. ASYNCHRONOUS DATA MONITORING BLOCK, Holding-Register 137
        /// New in Version 3.2 (Register 137)
        /// </summary>
        [ACPropertyBindingTarget(137, "ADMB_VariousEquipmentsAlarm", "en{'Various Equipments Alarm (137)'}de{'Versch. Gerätealarme (137)'}", "", false, false)]
        public IACContainerTNet<ADMB_VariousEquipmentsAlarm> ADMBVariousEquipmentsAlarm { get; set; }

        /// <summary>
        /// 5.1. ASYNCHRONOUS DATA MONITORING BLOCK, Holding-Register 138
        /// Only to Version 2.2, Deprecated in Version 3.2 (Register 138)
        /// </summary>
        [ACPropertyBindingTarget(138, "ADMBDustExtractionEquipmentAlarm", "en{'ADMBDustExtractionEquipmentAlarm'}de{'ADMBDustExtractionEquipmentAlarm'}", "", false, false)]
        public IACContainerTNet<ADMB_DustExtractionEquipmentAlarm> ADMBDustExtractionEquipmentAlarm { get; set; }

        /// <summary>
        /// 5.1. ASYNCHRONOUS DATA MONITORING BLOCK, Holding-Register 139
        /// </summary>
        [ACPropertyBindingTarget(139, "ADMBProcessOperation", "en{'Process Operation (139)'}de{'Funktion (Proc.-Op. 139)'}", "", false, false)]
        public IACContainerTNet<ADMB_ProcessOperation> ADMBProcessOperation { get; set; }

        /// <summary>
        /// 5.1. ASYNCHRONOUS DATA MONITORING BLOCK, Holding-Register 140
        /// </summary>
        [ACPropertyBindingTarget(140, "ADMBActualBinContNumber", "en{'Actual Source (140)'}de{'Aktuelle Quelle (140)'}", "", false, false)]
        public IACContainerTNet<UInt16> ADMBActualBinContNumber { get; set; }

        /// <summary>
        /// 5.1. ASYNCHRONOUS DATA MONITORING BLOCK, Holding-Register 141
        /// </summary>
        [ACPropertyBindingTarget(141, "ADMBActualDischargePosition", "en{'Actual Discharge Position (141)'}de{'Aktuelle Entleerposition (141)'}", "", false, false)]
        public IACContainerTNet<UInt16> ADMBActualDischargePosition { get; set; }

        /// <summary>
        /// 5.1. ASYNCHRONOUS DATA MONITORING BLOCK, Holding-Register 142
        /// </summary>
        [ACPropertyBindingTarget(142, "ADMBActualWeigherNumber", "en{'Actual Weigher (142)'}de{'Aktuelle Waage (142)'}", "", false, false)]
        public IACContainerTNet<CB_WeigherNumber> ADMBActualWeigherNumber { get; set; }

        /// <summary>
        /// 5.1. ASYNCHRONOUS DATA MONITORING BLOCK, Holding-Register 143+144
        /// </summary>
        [ACPropertyBindingTarget(143, "ADMBActualWeight", "en{'Actual Weight [g] (143+44)'}de{'Aktuelles Gewicht [g] (143+44)'}", "", false, false)]
        public IACContainerTNet<Int32> ADMBActualWeight { get; set; }

        /// <summary>
        /// 5.1. ASYNCHRONOUS DATA MONITORING BLOCK, Holding-Register 145
        /// </summary>
        [ACPropertyBindingTarget(145, "ADMBActualDosingTime", "en{'Dosing time [s] (145)'}de{'Dosierzeit [s] (145)'}", "", false, false)]
        public IACContainerTNet<UInt16> ADMBActualDosingTime { get; set; }

        /// <summary>
        /// 5.1. ASYNCHRONOUS DATA MONITORING BLOCK, Holding-Register 146-147
        /// </summary>
        [ACPropertyBindingTarget(146, "ADMBWeigher1GrossWeight", "en{'Weigher 1 Gross [g] (146+47)'}de{'Waage 1 Brutto [g] (146+47)'}", "", false, false)]
        public IACContainerTNet<Int32> ADMBWeigher1GrossWeight { get; set; }

        /// <summary>
        /// 5.1. ASYNCHRONOUS DATA MONITORING BLOCK, Holding-Register 148+149
        /// </summary>
        [ACPropertyBindingTarget(148, "ADMBWeigher2GrossWeight", "en{'Weigher 2 Gross [g] (148+49)'}de{'Waage 2 Brutto [g] (148+49)'}", "", false, false)]
        public IACContainerTNet<Int32> ADMBWeigher2GrossWeight { get; set; }

        /// <summary>
        /// 5.1. ASYNCHRONOUS DATA MONITORING BLOCK, Holding-Register 150
        /// </summary>
        [ACPropertyBindingTarget(150, "ADMBActualPosition", "en{'Actual Position (150)'}de{'Aktuelle Position (150)'}", "", false, false)]
        public IACContainerTNet<UInt16> ADMBActualPosition { get; set; }

        /// <summary>
        /// 5.1. ASYNCHRONOUS DATA MONITORING BLOCK, Holding-Register 151
        /// </summary>
        [ACPropertyBindingTarget(151, "ADMBBatchIDAlfraScale", "en{'ADMBBatchIDAlfraScale'}de{'ADMBBatchIDAlfraScale'}", "", false, false)]
        public IACContainerTNet<UInt16> ADMBBatchIDAlfraScale { get; set; }

        /// <summary>
        /// 5.1. ASYNCHRONOUS DATA MONITORING BLOCK, Holding-Register 152
        /// </summary>
        [ACPropertyBindingTarget(152, "ADMBStatusAlfraTransport", "en{'ADMBStatusAlfraTransport'}de{'ADMBStatusAlfraTransport'}", "", false, false)]
        public IACContainerTNet<ADMB_StatusAlfraTransport> ADMBStatusAlfraTransport { get; set; }

        /// <summary>
        /// 5.1. ASYNCHRONOUS DATA MONITORING BLOCK, Holding-Register 153
        /// </summary>
        [ACPropertyBindingTarget(153, "ADMBBatchIDAlfraTransport", "en{'ADMBBatchIDAlfraTransport'}de{'ADMBBatchIDAlfraTransport'}", "", false, false)]
        public IACContainerTNet<UInt16> ADMBBatchIDAlfraTransport { get; set; }

        /// <summary>
        /// 5.1. ASYNCHRONOUS DATA MONITORING BLOCK, Holding-Register 154
        /// </summary>
        [ACPropertyBindingTarget(154, "ADMBStatusAlfraCheckWeigher", "en{'ADMBStatusAlfraCheckWeigher'}de{'ADMBStatusAlfraCheckWeigher'}", "", false, false)]
        public IACContainerTNet<ADMB_StatusAlfraCheckWeigher> ADMBStatusAlfraCheckWeigher { get; set; }

        /// <summary>
        /// 5.1. ASYNCHRONOUS DATA MONITORING BLOCK, Holding-Register 155
        /// </summary>
        [ACPropertyBindingTarget(155, "ADMBBatchIDCheckWeigher", "en{'ADMBBatchIDCheckWeigher'}de{'ADMBBatchIDCheckWeigher'}", "", false, false)]
        public IACContainerTNet<UInt16> ADMBBatchIDCheckWeigher { get; set; }

        /// <summary>
        /// 5.1. ASYNCHRONOUS DATA MONITORING BLOCK, Holding-Register 156+157
        /// </summary>
        [ACPropertyBindingTarget(156, "ADMBAlreadyDosedWeightWeight", "en{'Actual dosing weight [g] (156+57'}de{'Aktuelles Dosiergewicht [g] (156+57)'}", "", false, false)]
        public IACContainerTNet<Int32> ADMBAlreadyDosedWeightWeight { get; set; }
        #endregion


        #region 5.2. ASYNCHRONOUS COMMAND MONITORING BLOCK (160 - 163)
        /// Only for BBT, BUV or ALFRA transport systems!
        /// <summary>
        /// 5.2. ASYNCHRONOUS COMMAND MONITORING BLOCK, Holding-Register 160
        /// </summary>
        [ACPropertyBindingTarget(160, "ACMBFillingAllowed", "en{'ACMBFillingAllowed'}de{'ACMBFillingAllowed'}", "", false, false)]
        public IACContainerTNet<ACMB_FillingAllowed> ACMBFillingAllowed { get; set; }

        /// <summary>
        /// 5.2. ASYNCHRONOUS COMMAND MONITORING BLOCK, Holding-Register 161
        /// </summary>
        [ACPropertyBindingTarget(161, "ACMBRequestedWeightReached", "en{'ACMBRequestedWeightReached'}de{'ACMBRequestedWeightReached'}", "", false, false)]
        public IACContainerTNet<ACMB_RequestedWeightReached> ACMBRequestedWeightReached { get; set; }

        /// <summary>
        /// 5.2. ASYNCHRONOUS COMMAND MONITORING BLOCK, Holding-Register 162
        /// </summary>
        [ACPropertyBindingTarget(162, "ACMBRequestDischargeTransport", "en{'ACMBRequestDischargeTransport'}de{'ACMBRequestDischargeTransport'}", "", false, false)]
        public IACContainerTNet<ACMB_RequestDischargeTransport> ACMBRequestDischargeTransport { get; set; }

        /// <summary>
        /// 5.2. ASYNCHRONOUS COMMAND MONITORING BLOCK, Holding-Register 163
        /// </summary>
        [ACPropertyBindingTarget(163, "ACMBAptTransportValidCheckResults", "en{'ACMBAptTransportValidCheckResults'}de{'ACMBAptTransportValidCheckResults'}", "", false, false)]
        public IACContainerTNet<ACMB_AptTransportValidCheckResults> ACMBAptTransportValidCheckResults { get; set; }
        #endregion


        #region 5.5 ASYNCHRONOUS ALARM MONITORING BLOCK (113 - 126)
        /// <summary>
        /// 5.5 ASYNCHRONOUS ALARM MONITORING BLOCK, Holding-Register 113
        /// </summary>
        [ACPropertyBindingTarget(113, "AAMBAlarmPositioning", "en{'AAMBAlarmPositioning'}de{'AAMBAlarmPositioning'}", "", false, false)]
        public IACContainerTNet<AlarmPositioning> AAMBAlarmPositioning { get; set; }

        /// <summary>
        /// 5.5 ASYNCHRONOUS ALARM MONITORING BLOCK, Holding-Register 114
        /// </summary>
        [ACPropertyBindingTarget(114, "AAMBAlarmWeigher", "en{'AAMBAlarmWeigher'}de{'AAMBAlarmWeigher'}", "", false, false)]
        public IACContainerTNet<AlarmWeigher> AAMBAlarmWeigher { get; set; }

        /// <summary>
        /// 5.5 ASYNCHRONOUS ALARM MONITORING BLOCK, Holding-Register 115
        /// </summary>
        [ACPropertyBindingTarget(115, "AAMBAlarmDustExhaust", "en{'AAMBAlarmDustExhaust'}de{'AAMBAlarmDustExhaust'}", "", false, false)]
        public IACContainerTNet<AlarmDustExhaust> AAMBAlarmDustExhaust { get; set; }

        /// <summary>
        /// 5.5 ASYNCHRONOUS ALARM MONITORING BLOCK, Holding-Register 116
        /// </summary>
        [ACPropertyBindingTarget(116, "AAMBAlarmDosingSlides1", "en{'AAMBAlarmDosingSlides1'}de{'AAMBAlarmDosingSlides1'}", "", false, false)]
        public IACContainerTNet<AlarmDosingSlides1> AAMBAlarmDosingSlides1 { get; set; }

        /// <summary>
        /// 5.5 ASYNCHRONOUS ALARM MONITORING BLOCK, Holding-Register 117
        /// </summary>
        [ACPropertyBindingTarget(117, "AAMBAlarmDosingSlides2", "en{'AAMBAlarmDosingSlides2'}de{'AAMBAlarmDosingSlides2'}", "", false, false)]
        public IACContainerTNet<AlarmDosingSlides2> AAMBAlarmDosingSlides2 { get; set; }

        /// <summary>
        /// 5.5 ASYNCHRONOUS ALARM MONITORING BLOCK, Holding-Register 118
        /// </summary>
        [ACPropertyBindingTarget(118, "AAMBAlarmDosingScrew1", "en{'AAMBAlarmDosingScrew1'}de{'AAMBAlarmDosingScrew1'}", "", false, false)]
        public IACContainerTNet<AlarmDosingScrew1> AAMBAlarmDosingScrew1 { get; set; }

        /// <summary>
        /// 5.5 ASYNCHRONOUS ALARM MONITORING BLOCK, Holding-Register 119
        /// </summary>
        [ACPropertyBindingTarget(119, "AAMBAlarmDosingScrew2", "en{'AAMBAlarmDosingScrew2'}de{'AAMBAlarmDosingScrew2'}", "", false, false)]
        public IACContainerTNet<AlarmDosingScrew2> AAMBAlarmDosingScrew2 { get; set; }

        /// <summary>
        /// 5.5 ASYNCHRONOUS ALARM MONITORING BLOCK, Holding-Register 120
        /// </summary>
        [ACPropertyBindingTarget(120, "AAMBAlarmDosingFrame1", "en{'AAMBAlarmDosingFrame1'}de{'AAMBAlarmDosingFrame1'}", "", false, false)]
        public IACContainerTNet<AlarmDosingFrame1> AAMBAlarmDosingFrame1 { get; set; }

        /// <summary>
        /// 5.5 ASYNCHRONOUS ALARM MONITORING BLOCK, Holding-Register 121
        /// </summary>
        [ACPropertyBindingTarget(121, "AAMBAlarmDosingFrame2", "en{'AAMBAlarmDosingFrame2'}de{'AAMBAlarmDosingFrame2'}", "", false, false)]
        public IACContainerTNet<AlarmDosingFrame2> AAMBAlarmDosingFrame2 { get; set; }

        /// <summary>
        /// 5.5 ASYNCHRONOUS ALARM MONITORING BLOCK, Holding-Register 122
        /// </summary>
        [ACPropertyBindingTarget(122, "AAMBAlarmStirringDevices1", "en{'AAMBAlarmStirringDevices1'}de{'AAMBAlarmStirringDevices1'}", "", false, false)]
        public IACContainerTNet<AlarmStirringDevices1> AAMBAlarmStirringDevices1 { get; set; }

        /// <summary>
        /// 5.5 ASYNCHRONOUS ALARM MONITORING BLOCK, Holding-Register 123
        /// </summary>
        [ACPropertyBindingTarget(123, "AAMBAlarmStirringDevices2", "en{'AAMBAlarmStirringDevices2'}de{'AAMBAlarmStirringDevices2'}", "", false, false)]
        public IACContainerTNet<AlarmStirringDevices2> AAMBAlarmStirringDevices2 { get; set; }

        /// <summary>
        /// 5.5 ASYNCHRONOUS ALARM MONITORING BLOCK, Holding-Register 124
        /// </summary>
        [ACPropertyBindingTarget(124, "AAMBAlarmGeneral1", "en{'AAMBAlarmGeneral1'}de{'AAMBAlarmGeneral1'}", "", false, false)]
        public IACContainerTNet<AlarmGeneral1> AAMBAlarmGeneral1 { get; set; }

        /// <summary>
        /// 5.5 ASYNCHRONOUS ALARM MONITORING BLOCK, Holding-Register 125
        /// </summary>
        [ACPropertyBindingTarget(125, "AAMBAlarmGeneral2", "en{'AAMBAlarmGeneral2'}de{'AAMBAlarmGeneral2'}", "", false, false)]
        public IACContainerTNet<AlarmGeneral2> AAMBAlarmGeneral2 { get; set; }

        /// <summary>
        /// 5.5 ASYNCHRONOUS ALARM MONITORING BLOCK, Holding-Register 126
        /// </summary>
        [ACPropertyBindingTarget(126, "AAMBAlarmGeneral3", "en{'AAMBAlarmGeneral3'}de{'AAMBAlarmGeneral3'}", "", false, false)]
        public IACContainerTNet<AlarmGeneral3> AAMBAlarmGeneral3 { get; set; }
        #endregion


        #region 3.5. COMMUNICATION LIFE SIGNAL (164)
        /// <summary>
        /// 3.5. COMMUNICATION LIFE SIGNAL Holding-Register 164
        /// </summary>
        [ACPropertyBindingTarget(164, "LifeSignalAcknowledge", "en{'LifeSignalAcknowledge'}de{'LifeSignalAcknowledge'}", "", false, false)]
        public IACContainerTNet<UInt16> LifeSignalAcknowledge { get; set; }

        protected UInt16 _LifeSignalAcknowledgeValue
        {
            get { return LifeSignalAcknowledge.ValueT; }
        }
        #endregion


        #region EXTERNAL PLC
        [ACPropertyBindingTarget(165, "DischargeReleasedExtern", "en{'DischargeReleasedExtern'}de{'DischargeReleasedExtern'}", "", false, false)]
        public IACContainerTNet<UInt16> DischargeReleasedExtern { get; set; }
        protected IACPropertyNetTarget _DischargeReleasedExtern
        {
            get
            {
                return (IACPropertyNetTarget)this.DischargeReleasedExtern;
            }
        }
        protected UInt16 _DischargeReleasedExternValue
        {
            get { return DischargeReleasedExtern.ValueT; }
        }
        #endregion

        #endregion


        #region WRITE-REGISTERS (1 - 64)

        #region 4.1. COMMAND BLOCK (1 - 17, 32)
        /// <summary>
        /// 4.1. COMMAND BLOCK, Holding-Register 1
        /// </summary>
        [ACPropertyBindingTarget(1, "CBCommandNumber", "en{'Command-No. (1)'}de{'Kommando-Nr. (1)'}", "", false, false)]
        public IACContainerTNet<UInt16> CBCommandNumber { get; set; }

        /// <summary>
        /// 4.1. COMMAND BLOCK, Holding-Register 2
        /// </summary>
        [ACPropertyBindingTarget(2, "CBCommandOperationMode", "en{'Function (Cmd.Op.Mode 2)'}de{'Funktion (Cmd.Op.Mode 2)'}", "", false, false)]
        public IACContainerTNet<CB_CommandOperationMode> CBCommandOperationMode { get; set; }

        /// <summary>
        /// 4.1. COMMAND BLOCK, Holding-Register 3
        /// </summary>
        [ACPropertyBindingTarget(3, "CBDosingBinNumber", "en{'Dosing-source (Bin-No. 3)'}de{'Dosierquelle (3)'}", "", false, false)]
        public IACContainerTNet<UInt16> CBDosingBinNumber { get; set; }

        /// <summary>
        /// 4.1. COMMAND BLOCK, Holding-Register 4
        /// </summary>
        [ACPropertyBindingTarget(4, "CBDischargingPosition", "en{'Discharging position (4)'}de{'Entleerposition (4)'}", "", false, false)]
        public IACContainerTNet<UInt16> CBDischargingPosition { get; set; }

        /// <summary>
        /// 4.1. COMMAND BLOCK, Holding-Register 5
        /// </summary>
        [ACPropertyBindingTarget(5, "CBWeigherNumber", "en{'Weigher (5)'}de{'Waage. (5)'}", "", false, false)]
        public IACContainerTNet<CB_WeigherNumber> CBWeigherNumber { get; set; }

        /// <summary>
        /// 4.1. COMMAND BLOCK, Holding-Register 6
        /// </summary>
        [ACPropertyBindingTarget(6, "CBCheckTare", "en{'Check tare (6)'}de{'Taraprüfung (6)'}", "", false, false)]
        public IACContainerTNet<UInt16> CBCheckTare { get; set; }

        /// <summary>
        /// 4.1. COMMAND BLOCK, Holding-Register 7+8
        /// </summary>
        [ACPropertyBindingTarget(7, "CBRequestedWeight", "en{'Target-Quantity [g]  (7+8)'}de{'Sollgewicht [g] (7+8)'}", "", false, false)]
        public IACContainerTNet<Int32> CBRequestedWeight { get; set; }

        /// <summary>
        /// 4.1. COMMAND BLOCK, Holding-Register 9
        /// </summary>
        [ACPropertyBindingTarget(9, "CBTolerancePositive", "en{'Tolerance + [g] (9)'}de{'Toleranz + [g] (9)'}", "", false, false)]
        public IACContainerTNet<UInt16> CBTolerancePositive { get; set; }

        /// <summary>
        /// 4.1. COMMAND BLOCK, Holding-Register 10
        /// </summary>
        [ACPropertyBindingTarget(10, "CBToleranceNegative", "en{'Tolerance - [g] (10)'}de{'Toleranz - [g] (10)'}", "", false, false)]
        public IACContainerTNet<UInt16> CBToleranceNegative { get; set; }

        /// <summary>
        /// 4.1. COMMAND BLOCK, Holding-Register 11
        /// </summary>
        [ACPropertyBindingTarget(11, "CBDosingAccuracy", "en{'Accuracy [g] (11)'}de{'Genauigkeit [g] (11)'}", "", false, false)]
        public IACContainerTNet<UInt16> CBDosingAccuracy { get; set; }

        /// <summary>
        /// 4.1. COMMAND BLOCK, Holding-Register 12
        /// </summary>
        [ACPropertyBindingTarget(12, "CBIndicationDosability", "en{'Indication dosability (12)'}de{'Dosierbarkeitsindex (12)'}", "", false, false)]
        public IACContainerTNet<CB_IndicationDosability> CBIndicationDosability { get; set; }

        /// <summary>
        /// 4.1. COMMAND BLOCK, Holding-Register 13
        /// </summary>
        [ACPropertyBindingTarget(13, "CBDosingMode", "en{'Dosing mode (13)'}de{'Dosiermodus (13)'}", "", false, false)]
        public IACContainerTNet<CB_DosingMode> CBDosingMode { get; set; }

        /// <summary>
        /// 4.1. COMMAND BLOCK, Holding-Register 14
        /// </summary>
        [ACPropertyBindingTarget(14, "CBPosDosDestination", "en{'Positioning destination (14)'}de{'Zielposition (14)'}", "", false, false)]
        public IACContainerTNet<UInt16> CBPosDosDestination { get; set; }

        /// <summary>
        /// 4.1. COMMAND BLOCK, Holding-Register 15
        /// </summary>
        [ACPropertyBindingTarget(15, "CBMassDensity", "en{'Density (15)'}de{'Dichte (15)'}", "", false, false)]
        public IACContainerTNet<UInt16> CBMassDensity { get; set; }

        /// <summary>
        /// 4.1. COMMAND BLOCK, Holding-Register 16
        /// </summary>
        [ACPropertyBindingTarget(16, "CBBatchID", "en{'Batch-ID (16)'}de{'Batch-ID (16)'}", "", false, false)]
        public IACContainerTNet<UInt16> CBBatchID { get; set; }

        /// <summary>
        /// 4.1. COMMAND BLOCK, Holding-Register 17
        /// </summary>
        [ACPropertyBindingTarget(17, "CBBatchEnd", "en{'Batch end (17)'}de{'Letzter Batch (17)'}", "", false, false)]
        public IACContainerTNet<CB_BatchEnd> CBBatchEnd { get; set; }

        /// <summary>
        /// 4.1. COMMAND BLOCK, Holding-Register 32
        /// </summary>
        [ACPropertyBindingTarget(32, "CBCommandValid", "en{'Start (Cmd. valid 32)'}de{'Starten (Cmd. valid 32)'}", "", false, false)]
        public IACContainerTNet<CB_CommandValid> CBCommandValid { get; set; }
        #endregion


        #region 4.4. RESULT RESPONSE BLOCK (31)
        /// <summary>
        /// 4.4. RESULT RESPONSE BLOCK, Holding-Register 31, Handshake mit RBResultValid
        /// </summary>
        [ACPropertyBindingTarget(31, "RRBResultAcknowledge", "en{'Read-Acknowledge (Res. ACK 31)'}de{'Lesebestätigung (Res. ACK 31)'}", "", false, false)]
        public IACContainerTNet<RRB_ResultAcknowledge> RRBResultAcknowledge { get; set; }
        #endregion


        #region 5.3. ASYNCHRONOUS COMMAND BLOCK (34 - 43)
        /// <summary>
        /// 5.3. ASYNCHRONOUS COMMAND BLOCK, Holding-Register 34
        /// </summary>
        [ACPropertyBindingTarget(34, "ACBResetAlarmStatus", "en{'Reset Alarm-Status (34)'}de{'Reset Alarm-Status (34)'}", "", false, false)]
        public IACContainerTNet<UInt16> ACBResetAlarmStatus { get; set; }

        /// <summary>
        /// 5.3. ASYNCHRONOUS COMMAND BLOCK, Holding-Register 35
        /// </summary>
        [ACPropertyBindingTarget(35, "ACBStopCommand", "en{'Stop command (35)'}de{'Stoppen (Stop.-Cmd.35)'}", "", false, false)]
        public IACContainerTNet<UInt16> ACBStopCommand { get; set; }

        /// <summary>
        /// 5.3. ASYNCHRONOUS COMMAND BLOCK, Holding-Register 36
        /// </summary>
        [ACPropertyBindingTarget(36, "ACBExternalDischargeRelease", "en{'External Discharge Release (36)'}de{'Externe-Entleerfreigabe (36)'}", "", false, false)]
        public IACContainerTNet<UInt16> ACBExternalDischargeRelease { get; set; }

        /// <summary>
        /// 5.3. ASYNCHRONOUS COMMAND BLOCK, Holding-Register 37
        /// </summary>
        [ACPropertyBindingTarget(37, "ACBExternalTransportReady", "en{'ACBExternalTransportReady'}de{'ACBExternalTransportReady'}", "", false, false)]
        public IACContainerTNet<UInt16> ACBExternalTransportReady { get; set; }

        /// <summary>
        /// 5.3. ASYNCHRONOUS COMMAND BLOCK, Holding-Register 38
        /// </summary>
        [ACPropertyBindingTarget(38, "ACBExternalDischargeReleaseAptTransport", "en{'ACBExternalDischargeReleaseAptTransport'}de{'ACBExternalDischargeReleaseAptTransport'}", "", false, false)]
        public IACContainerTNet<UInt16> ACBExternalDischargeReleaseAptTransport { get; set; }

        /// <summary>
        /// 5.3. ASYNCHRONOUS COMMAND BLOCK, Holding-Register 39
        /// </summary>
        [ACPropertyBindingTarget(39, "ACBAptTransportResultAcknowledge", "en{'ACBAptTransportResultAcknowledge'}de{'ACBAptTransportResultAcknowledge'}", "", false, false)]
        public IACContainerTNet<UInt16> ACBAptTransportResultAcknowledge { get; set; }

        /// <summary>
        /// 5.3. ASYNCHRONOUS COMMAND BLOCK, Holding-Register 40
        /// </summary>
        [ACPropertyBindingTarget(40, "ACBAptTransportRestart", "en{'ACBAptTransportRestart'}de{'ACBAptTransportRestart'}", "", false, false)]
        public IACContainerTNet<UInt16> ACBAptTransportRestart { get; set; }

        /// <summary>
        /// 5.3. ASYNCHRONOUS COMMAND BLOCK, Holding-Register 41, DEPRECATED!
        /// </summary>
        [ACPropertyBindingTarget(41, "ACBRestartAlarm", "en{'ACBRestartAlarm'}de{'ACBRestartAlarm'}", "", false, false)]
        public IACContainerTNet<UInt16> ACBRestartAlarm { get; set; }

        /// <summary>
        /// 5.3. ASYNCHRONOUS COMMAND BLOCK, Holding-Register 42, DEPRECATED!
        /// </summary>
        [ACPropertyBindingTarget(42, "ACBApproveAlarm", "en{'ACBApproveAlarm'}de{'ACBApproveAlarm'}", "", false, false)]
        public IACContainerTNet<UInt16> ACBApproveAlarm { get; set; }

        /// <summary>
        /// 5.3. ASYNCHRONOUS COMMAND BLOCK, Holding-Register 43, DEPRECATED!
        /// </summary>
        [ACPropertyBindingTarget(43, "ACBRejectAlarm", "en{'ACBRejectAlarm'}de{'ACBRejectAlarm'}", "", false, false)]
        public IACContainerTNet<UInt16> ACBRejectAlarm { get; set; }
        #endregion


        #region 5.4. ASYNCHRONOUS SYSTEM RESET (33)
        /// <summary>
        /// 5.4. ASYNCHRONOUS SYSTEM RESET, Holding-Register 33
        /// </summary>
        [ACPropertyBindingTarget(33, "ASRResetMachine", "en{'System-Reset (33)'}de{'System-Reset (33)'}", "", false, false)]
        public IACContainerTNet<UInt16> ASRResetMachine { get; set; }
        #endregion


        #region 3.5. COMMUNICATION LIFE SIGNAL (64)
        [ACPropertyBindingTarget(64, "LifeSignalRequest", "en{'LifeSignalRequest'}de{'LifeSignalRequest'}", "", false, false)]
        public IACContainerTNet<UInt16> LifeSignalRequest { get; set; }

        protected virtual UInt16 _LifeSignalRequestValue
        {
            get { return LifeSignalRequest.ValueT; }
            set { LifeSignalRequest.ValueT = value; }
        }
        #endregion


        #region EXTERNAL PLC
        [ACPropertyBindingTarget(10, "Destination", "en{'Destination GIP'}de{'Ziel GIP'}", "", false, false)]
        public IACContainerTNet<UInt16> Destination { get; set; }
        #endregion

        #endregion

        #endregion


        #region Methods

        #region Execute-Helper-Handlers
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "ResetMachineKSE":
                    ResetMachineKSE();
                    return true;
                case "ResetAlarmStateKSE":
                    ResetAlarmStateKSE();
                    return true;
                case Const.IsEnabledPrefix + "ResetMachineKSE":
                    result = IsEnabledResetMachineKSE();
                    return true;
                case Const.IsEnabledPrefix + "ResetAlarmStateKSE":
                    result = IsEnabledResetAlarmStateKSE();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }
        #endregion


        #region Cyclic
        void ApplicationManager_ProjectWorkCycleR1sec(object sender, EventArgs e)
        {
            if (IsReadyForSending)
            {
                if (_LastToggleTime.HasValue)
                {
                    if ((_LifeSignalRequestValue != _LifeSignalAcknowledgeValue)
                        && ((DateTime.Now - _LastToggleTime.Value).TotalSeconds > 2))
                    {
                        // TODO Alarm
                        if ((DateTime.Now - _LastToggleTime.Value).TotalSeconds > 3)
                        {
                            _LifeSignalRequestValue = _LifeSignalAcknowledgeValue;
                        }
                    }
                    else if (_LifeSignalRequestValue == _LifeSignalAcknowledgeValue)
                    {
                        _LastToggleTime = null;
                    }
                }
                if (!_LastToggleTime.HasValue)
                {
                    if (_LifeSignalRequestValue == 0)
                        _LifeSignalRequestValue = 1;
                    else
                        _LifeSignalRequestValue = 0;
                    _LastToggleTime = DateTime.Now;
                }
            }

            if (ASRResetMachine.ValueT > 0)
            {
                ACStateEnum newACState = DetermineACStateFromPLC();
                RepairHandshakeValuesIfOutOfSync(newACState);
            }

            if (ACBResetAlarmStatus.ValueT > 0)
            {
                ACStateEnum newACState = DetermineACStateFromPLC();
                if (ADMBMachineState.ValueT == ADMB_MachineState.Busy)
                    RepairHandshakeValuesIfOutOfSync(newACState);
            }

        }
        #endregion


        #region Property-Change Event-Handler
        protected override void ModelProperty_ValueUpdatedOnReceival(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
        }

        protected virtual void LifeSignalAcknowledge_PropertyChanged(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.AfterBroadcast)
                return;
        }

        protected virtual void DischargeReleasedExtern_PropertyChanged(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.AfterBroadcast)
                return;
            if ((e.ValueEvent.Sender == EventRaiser.Source) && (e.ValueEvent.EventType == EventTypes.ValueChangedInSource))
            {
                ACBExternalDischargeRelease.ValueT = DischargeReleasedExtern.ValueT;
            }
        }

        protected virtual void ReadValues_PropertyChanged(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.AfterBroadcast || _PropertiesBindPhase <= 0)
                return;
            if ((e.ValueEvent.Sender == EventRaiser.Source) && (e.ValueEvent.EventType == EventTypes.ValueChangedInSource))
            {
                // ADMBMachineState, RBResultValid, CRBCommandAcknowledge are ACState-Relevant Properties
                if (   sender == ADMBMachineState 
                    || sender == RBResultValid 
                    || sender == CRBCommandAcknowledge
                    || sender == CRBCommandResponseCode)
                {
                    LogStateMachine(1010);
                    SyncACStateFromPLC(1, sender, e, phase);
                }
                /// Only to Version 2.2:
                else if (sender == ADMBPositioningEquipmentAlarm && (ADMBPositioningEquipmentAlarm as IACPropertyNetTarget).Source != null)
                {
                    ADMB_PositioningEquipmentAlarm nAlarmValuePosition;
                    nAlarmValuePosition = ADMBPositioningEquipmentAlarm.ValueT;
                    if (nAlarmValuePosition == ADMB_PositioningEquipmentAlarm.EquipmentHardwareAlarm)
                    {
                        if ((ACBResetAlarmStatus.ValueT == 0))      //AutoReset PositioningAlarm 29.09.2016
                        {
                            ACBResetAlarmStatus.ValueT = 1;
                            LogStateMachine(1020);
                        }
                    }
                }
            }
        }
        #endregion


        #region State-Converter

        #region ACState
        private void SyncACStateFromPLC(short callerID, object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            ACStateEnum newACState = DetermineACStateFromPLC();
            ACStateEnum defaultNextState = PAFuncStateConvBase.GetDefaultNextACState(CurrentACState);
            // TODO: Prüfe, ob Zustandsübergang erlaubt
            if (defaultNextState < newACState)
            {
            }

            PAProcessFunction currentActiveFunction = CurrentActiveFunction;

            /// STATE-HANDLING (Idle -> Busy -> Alarm -> Ready)
            if (sender == ADMBMachineState)
            {
                if (ADMBMachineState.ValueT == ADMB_MachineState.Alarm)
                {
                    RB_ProcessResultAlarmCode nAlarmValueDosing;
                    nAlarmValueDosing = ADMBDosingOrDischargingProcessAlarm.ValueT;
                    //////////////////////////////
                    // Deprecated code:
                    // Since Version 3.0 this will not happen any more, that nAlarmValueDosing has a value
                    // The RB_ProcessResultAlarmCode can be read in the Result-Block when the Machine-State switches to Ready.
                    if (CurrentActiveFunction is PAFDosingKSE)
                    {
                        this.DosingFunction.Malfunction.ValueT = PANotifyState.Off;
                        if ((nAlarmValueDosing == RB_ProcessResultAlarmCode.DosedWeightBelowTolerance)
                         || (nAlarmValueDosing == RB_ProcessResultAlarmCode.DosedWeightAboveTolerance))       // Tolerance
                        {
                            this.DosingFunction.StateTolerance.ValueT = PANotifyState.AlarmOrFault;
                            this.DosingFunction.Malfunction.ValueT = PANotifyState.AlarmOrFault;
                        }
                        else
                            this.DosingFunction.StateTolerance.ValueT = PANotifyState.Off;

                        if ((nAlarmValueDosing == RB_ProcessResultAlarmCode.NoFlowDetectedDosedWeightBelowRejectionLimit)
                         || (nAlarmValueDosing == RB_ProcessResultAlarmCode.NoFlowDetectedDosedWeightAboveRejectionLimit)
                         || (nAlarmValueDosing == RB_ProcessResultAlarmCode.NoFlowDetectedDosedWeightAboveTolerance)
                         || (nAlarmValueDosing == RB_ProcessResultAlarmCode.NoFlowDetectedDosedWeightInGeneral))    // Matmangel
                            this.DosingFunction.StateLackOfMaterial.ValueT = PANotifyState.AlarmOrFault;
                        else
                            this.DosingFunction.StateLackOfMaterial.ValueT = PANotifyState.Off;
                    }
                    else if (CurrentActiveFunction is PAFDischargingKSE)
                    {
                        this.DischargingFunction.Malfunction.ValueT = PANotifyState.Off;
                        if ((nAlarmValueDosing == RB_ProcessResultAlarmCode.WeigherNo1OverflowExpected)
                         || (nAlarmValueDosing == RB_ProcessResultAlarmCode.WeigherNo1OverloadExpected))       // Tolerance
                        {
                            this.DischargingFunction.StateDestinationFull.ValueT = PANotifyState.AlarmOrFault;
                            this.DischargingFunction.Malfunction.ValueT = PANotifyState.AlarmOrFault;
                        }
                        else
                            this.DischargingFunction.StateDestinationFull.ValueT = PANotifyState.Off;
                    }
                    /////////////////////////////
                }
            }
            // Handshake, dass der Resultblock gelesen wurde:
            else if (sender == RBResultValid)
            {
                if ((RBResultValid.ValueT == RB_ResultValid.NotValid) && (RRBResultAcknowledge.ValueT == RRB_ResultAcknowledge.Acknowledge))
                {
                    RRBResultAcknowledge.ForceBroadcast = true;
                    RRBResultAcknowledge.ValueT = RRB_ResultAcknowledge.ResultNotAcknowledge;
                    LogStateMachine(2010);
                }
            }
            // Handshake, dass das Kommando verstanden wurde
            else if (sender == CRBCommandAcknowledge || sender == CRBCommandResponseCode)
            {
                if (CRBCommandAcknowledge.ValueT == CRB_CommandAcknowledge.Acknowledge)
                {
                    if (ADMBMachineState.ValueT == ADMB_MachineState.Idle
                        && CBCommandValid.ValueT == CB_CommandValid.Valid
                        && CRBCommandResponseCode.ValueT != CRB_CommandResponseCode.CommandAccepted)
                    {
                        ConversionAlarm.ValueT = PANotifyState.AlarmOrFault;
                        Msg msg = new Msg(String.Format("Command was not accepted from KSE. Reason: {0}", CRBCommandResponseCode.ValueT.ToString()), this, eMsgLevel.Error, "KSEConvScale", "SyncACStateFromPLC()", 1000);
                        if (IsAlarmActive(ConversionAlarm, msg.Message) == null)
                            Messages.LogError(this.GetACUrl(), "SyncACStateFromPLC()", msg.Message);
                        OnNewAlarmOccurred(ConversionAlarm, msg, true);
                    }

                    CBCommandValid.ForceBroadcast = true;
                    CBCommandValid.ValueT = CB_CommandValid.NotValid;
                    LogStateMachine(2020);
                }
            }

            if (currentActiveFunction != null && currentActiveFunction.CurrentACState != newACState)
            {
                (currentActiveFunction.ACState as ACPropertyNetServerBase<ACStateEnum>).ChangeValueServer(currentActiveFunction.ACState.ForceBroadcast, e.ValueEvent.InvokerInfo, newACState);
            }
        }


        public override ACStateEnum GetNextACState(PAProcessFunction sender, string transitionMethod = "")
        {
            ACStateEnum defaultNextState = PAFuncStateConvBase.GetDefaultNextACState(sender.CurrentACState, transitionMethod);
            if (!IsSimulationOn)
            {
                ACStateEnum stateInPLC = DetermineACStateFromPLC();
                RepairHandshakeValuesIfOutOfSync(stateInPLC);

                // transitionMethod is Empty in following cases:
                // 1. Invoked from cyclic method in PAProcessFunction which is not a transition (e.G. SMAborting, SMStopping....) 
                // 2. Invoked from SyncACStateFromPLC() when Response in PLC changed
                if (String.IsNullOrEmpty(transitionMethod))
                {
                }
                // Else state-change is in iPlus (Transition)
                else
                {
                    if (transitionMethod == ACStateConst.TMRun)
                    {
                        if (stateInPLC == ACStateEnum.SMRunning)
                            defaultNextState = ACStateEnum.SMRunning;
                        else // if (stateInPLC == ACStateEnum.SMStarting) evtl. könnte kurzzeitig auch Idle kommen wegen Zustand [24]:
                            defaultNextState = ACStateEnum.SMStarting;
                    }
                    // Wenn Stopp-Kommando ausgelöst wird, dann muss unterschieen werden ob sich die MAschine im Alarm-Zustand befindet
                    else if (transitionMethod == ACStateConst.TMStopp)
                    {
                        if (ADMBMachineState.ValueT == ADMB_MachineState.Alarm)
                        {
                            // Mit der Alarm-Quittierung beendet KSE von alleine und wechsel in dern Ready-State!
                            if (ACBResetAlarmStatus.ValueT == 0)
                            {
                                ACBResetAlarmStatus.ValueT = 1;
                                LogStateMachine(3010);
                            }
                        }
                        else
                        {
                            if (ACBStopCommand.ValueT == 0)
                            {
                                ACBStopCommand.ValueT = 1;
                                LogStateMachine(3020);
                            }
                        }
                        // SMStopp-State gibt es nicht in KSE
                        defaultNextState = sender.CurrentACState;
                    }
                }
            }
            else
            {
                if (transitionMethod == ACStateConst.TMRun
                    && RRBResultAcknowledge.ValueT != RRB_ResultAcknowledge.ResultNotAcknowledge)
                {
                    RRBResultAcknowledge.ValueT = RRB_ResultAcknowledge.ResultNotAcknowledge;
                    LogStateMachine(3030);
                }
            }
            return defaultNextState;
        }


        public override bool IsEnabledTransition(PAProcessFunction sender, string transitionMethod)
        {
            ACStateEnum acState = sender.CurrentACState;

            switch (transitionMethod)
            {
                case ACStateConst.TMStart:
                    return acState == ACStateEnum.SMIdle;
                case ACStateConst.TMRun:
                    {
                        bool isEnabled = (acState == ACStateEnum.SMStarting
                            && (ADMBMachineState.ValueT == ADMB_MachineState.Idle)
                            && (CBCommandValid.ValueT == CB_CommandValid.NotValid || IsSimulationOn));
                        if (!isEnabled)
                        {
                            ACStateEnum newACState = DetermineACStateFromPLC();
                            RepairHandshakeValuesIfOutOfSync(newACState);
                        }
                        return isEnabled;
                    }
                case ACStateConst.TMReset:
                    return acState == ACStateEnum.SMIdle || acState == ACStateEnum.SMStarting || acState == ACStateEnum.SMRunning || acState == ACStateEnum.SMCompleted || acState == ACStateEnum.SMAborted || acState == ACStateEnum.SMStopped || acState == ACStateEnum.SMResetting;
                case ACStateConst.TMStopp:
                    return acState == ACStateEnum.SMRunning || acState == ACStateEnum.SMHeld || acState == ACStateEnum.SMPaused;
                // KSE doesn't support following commands:
                case ACStateConst.TMAbort:
                case ACStateConst.TMPause:
                case ACStateConst.TMResume:
                case ACStateConst.TMHold:
                case ACStateConst.TMRestart:
                    return false;
            }
            return false;
        }


        /// <summary>
        /// Calls TranslateToACStateFromPLCBits() and then repairs some states 
        /// because some states in the KSE-State-machine are not uniqe
        /// State [7] and [11], [1] and [24] can't be distinguished clearly
        /// </summary>
        /// <returns></returns>
        protected ACStateEnum DetermineACStateFromPLC()
        {
            ACStateEnum acState = TranslateToACStateFromPLCBits();
            var currentActiveFunction = CurrentActiveFunction;
            if (currentActiveFunction != null)
            {
                // Notwendig Weil Zustand [7] und Zustand [11] nicht unterschieden werden können
                if (acState == ACStateEnum.SMResetting && currentActiveFunction.CurrentACState < ACStateEnum.SMCompleted)
                    acState = ACStateEnum.SMRunning;
                // Notwendig Weil Zustand [1] und  Zustand [5] und Zustand [24] nicht eindeutig unterschieden werden können
                else if (acState == ACStateEnum.SMIdle && currentActiveFunction.CurrentACState == ACStateEnum.SMStarting)
                    acState = ACStateEnum.SMStarting;
            }
            return acState;
        }

        /// <summary>
        /// Determine current ACState according the current values in the PLC
        /// ADMBMachineState, CBCommandValid, CRBCommandAcknowledge, RBResultValid, RRBResultAcknowledge
        /// </summary>
        /// <returns></returns>
        protected ACStateEnum TranslateToACStateFromPLCBits()
        {
            /// Signalfolge:
            /// [1 ] VB: ADMBMachineState = Idle (0)
            ///      [2 ] VB: Parameter senden + CBCommandValid = Valid (1) setzen (Benachrichtigung das Parameter geschrieben wurden)
            ///      [3 ] KSE: CRBCommandAcknowledge = Acknowledge (1) (Bestätigung dass Parameter gelesen wurden)
            ///          [3 ] KSE: Falls Parameter OK, dann  CRBCommandResponseCode = CommandAccepted (0)
            ///              [4 ] KSE: ADMBMachineState = Busy (1)
            ///              [5 ] VB: CBCommandValid = NotValid (0)
            ///              [6 ] KSE: CRBCommandAcknowledge = CommandNotAcknowledged(0) 
            ///              -> Folgezustand [7 ] oder [30]
            ///          [20]  KSE: Falls Parameter NICHT OK, dann  CRBCommandResponseCode >= CommandCodeOrNumberNotValid (1-21)
            ///              [21] KSE: ADMBMachineState = Alarm (0)
            ///              [22] VB: CBCommandValid = NotValid (0) + ACBResetAlarmStatus (1)
            ///              [23] KSE: CRBCommandAcknowledge = CommandNotAcknowledged(0) 
            ///              [24] VB: ACBResetAlarmStatus (0)
            ///              -> Folgezustand [1 ]
            ///
            /// [30] KSE: ADMBMachineState = Alarm (2)
            ///      [31] VB: ACBResetAlarmStatus = 1 (Alarm Quitierung)
            ///      [32] ADMBMachineState = Ready (3) ENDE
            ///      [33] Rückgabewerte schreiben: RBResultValid = Valid(1) (Signal das Rückgabewerte da sind)
            ///      -> Folgezustand [9 ]
            ///
            /// [7] ADMBMachineState = Ready (3) ENDE
            ///         [8] KSE: Rückgabewerte schreiben: RBResultValid = Valid(1) (Signal das Rückgabewerte da sind)
            ///         [9] VB: RRBResultAcknowledge = Acknowledge (1) setzen (Rückgebaewerte lesen und dann bestätigen dass Datengelesen wurden) + ACBResetAlarmStatus = 0 + ACBStopCommand = 0
            ///         [10] KSE: RBResultValid = NotValid(0)
            ///         [11] VB: RRBResultAcknowledge = ResultNotAcknowledge (0) setzen
            ///         -> Folgezustand [1 ]
            ///
            ///
            ///-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            ///      VB->KSE                                                                   |   KSE->VB
            ///-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            ///      CommandValid  | ACBResetAl.St | ACBStopCommand|   RRBResultAcknowledge    |   CRBCommandAcknowledge       | CRBCommandResponseCode    | RBResultValid | ADMBMachineState
            ///-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            ///
            /// *** Normal ***
            /// [1 ]  0(NotValid)  |       0       |       0       |   0(ResultNotAcknowledge) |   0(CommandNotAcknowledged)   |   0(CommandAccepted)      |   0(NotValid) |   *0(Idle)   Grundzustand                      ->SMIdle
            /// [2 ] *1(Valid)     |       0       |       0       |   0(ResultNotAcknowledge) |   0(CommandNotAcknowledged)   |   0(CommandAccepted)      |   0(NotValid) |   0(Idle)    Parameter geschrieben             ->SMStarting
            /// [3 ]  1(Valid)     |       0       |       0       |   0(ResultNotAcknowledge) |  *1(Acknowledge)              |   0(CommandAccepted)      |   0(NotValid) |   0(Idle)    Parameter gelesen                 ->SMStarting
            /// [4 ] *0(NotValid)  |       0       |       0       |   0(ResultNotAcknowledge) |   1(Acknowledge)              |   0(CommandAccepted)      |   0(NotValid) |   0(Idle)    Handshake                         ->SMStarting
            /// [5 ]  0(NotValid)  |       0       |       0       |   0(ResultNotAcknowledge) |  *0(CommandNotAcknowledged)   |   0(CommandAccepted)      |   0(NotValid) |   0(Idle)    Handshake                         ->SMStarting
            /// [6 ]  0(NotValid)  |       0       |       0       |   0(ResultNotAcknowledge) |   0(CommandNotAcknowledged)   |   0(CommandAccepted)      |   0(NotValid) |  *1(Busy)    Dosierung / Entleerung aktiv      ->SMRunning
            /// [7 ]  0(NotValid)  |       0       |       0       |   0(ResultNotAcknowledge) |   0(CommandNotAcknowledged)   |   0(CommandAccepted)      |   0(NotValid) |  *3(Ready)   Dosierung / Entleerung fertig     ->SMRunning
            /// [8 ]  0(NotValid)  |       0       |       0       |   0(ResultNotAcknowledge) |   0(CommandNotAcknowledged)   |   0(CommandAccepted)      |  *1(Valid)    |   3(Ready)   Rückgabewerte geschrieben         ->SMCompleted
            /// [9 ]  0(NotValid)  |       0       |       0       |  *1(Acknowledge)          |   0(CommandNotAcknowledged)   |   0(CommandAccepted)      |   1(Valid)    |   3(Ready)   Rückgabewerte gelesen             ->SMResetting
            /// [10]  0(NotValid)  |       0       |       0       |   1(Acknowledge)          |   0(CommandNotAcknowledged)   |   0(CommandAccepted)      |  *0(NotValid) |   3(Ready)   Handshake                         ->SMResetting
            /// [11]  0(NotValid)  |       0       |       0       |  *0(ResultNotAcknowledge) |   0(CommandNotAcknowledged)   |   0(CommandAccepted)      |   0(NotValid) |   3(Ready)   Handshake                         ->SMResetting
            ///
            /// *** Wrong PARAMETERS ***
            /// [1 ]  0(NotValid)  |       0       |       0       |   0(ResultNotAcknowledge) |   0(CommandNotAcknowledged)   |   0(CommandAccepted)      |   0(NotValid) |  *0(Idle)    Grundzustand                      ->SMIdle
            /// [2 ] *1(Valid)     |       0       |       0       |   0(ResultNotAcknowledge) |   0(CommandNotAcknowledged)   |   0(CommandAccepted)      |   0(NotValid) |   0(Idle)    Parameter geschrieben             ->SMStarting
            /// [20]  1(Valid)     |       0       |       0       |   0(ResultNotAcknowledge) |  *1(Acknowledge)              |  *> 0(NotValid)           |   0(NotValid) |   0(Idle)    Parameter gelesen und fehlerhaft  ->SMStarting
            /// [21]  1(Valid)     |       0       |       0       |   0(ResultNotAcknowledge) |   1(Acknowledge)              |   > 0(NotValid)           |   0(NotValid) |  *2(Alarm)   Fehlerzustand                     ->SMStarting
            /// [22] *0(NotValid)  |      *1       |       0       |   0(ResultNotAcknowledge) |   1(Acknowledge)              |   > 0(NotValid)           |   0(NotValid) |   2(Alarm)   Handshake                         ->SMStarting
            /// [23]  0(NotValid)  |       1       |       0       |   0(ResultNotAcknowledge) |  *0(CommandNotAcknowledged)   |   > 0(NotValid)           |   0(NotValid) |  *0(Idle)    Handshake                         ->SMStarting
            /// [24]  0(NotValid)  |      *0       |       0       |   0(ResultNotAcknowledge) |   0(CommandNotAcknowledged)   |   0(CommandAccepted)      |   0(NotValid) |   0(Idle)    Handshake                         ->SMStarting
            ///
            /// *** Alarm ***
            /// [1 ]  0(NotValid)  |       0       |       0       |   0(ResultNotAcknowledge) |   0(CommandNotAcknowledged)   |   0(CommandAccepted)      |   0(NotValid) |   *0(Idle)   Grundzustand                      ->SMIdle
            /// [2 ] *1(Valid)     |       0       |       0       |   0(ResultNotAcknowledge) |   0(CommandNotAcknowledged)   |   0(CommandAccepted)      |   0(NotValid) |   0(Idle)    Parameter geschrieben             ->SMStarting
            /// [3 ]  1(Valid)     |       0       |       0       |   0(ResultNotAcknowledge) |  *1(Acknowledge)              |   0(CommandAccepted)      |   0(NotValid) |   0(Idle)    Parameter gelesen                 ->SMStarting
            /// [4 ] *0(NotValid)  |       0       |       0       |   0(ResultNotAcknowledge) |   1(Acknowledge)              |   0(CommandAccepted)      |   0(NotValid) |   0(Idle)    Handshake                         ->SMStarting
            /// [5 ]  0(NotValid)  |       0       |       0       |   0(ResultNotAcknowledge) |  *0(CommandNotAcknowledged)   |   0(CommandAccepted)      |   0(NotValid) |   0(Idle)    Handshake                         ->SMStarting
            /// [6 ]  0(NotValid)  |       0       |       0       |   0(ResultNotAcknowledge) |   0(CommandNotAcknowledged)   |   0(CommandAccepted)      |   0(NotValid) |  *1(Busy)    Dosierung / Entleerung aktiv      ->SMRunning
            /// [30]  0(NotValid)  |       0       |       0       |   0(ResultNotAcknowledge) |   0(CommandNotAcknowledged)   |   0(CommandAccepted)      |   0(NotValid) |  *2(Alarm)   Alarmzustand, Warte auf Bediener  ->SMPaused
            /// [31]  0(NotValid)  |      *1       |       0       |   0(ResultNotAcknowledge) |   0(CommandNotAcknowledged)   |   0(CommandAccepted)      |   0(NotValid) |   2(Alarm)   Alarmquittung                     ->SMPaused
            /// [32]  0(NotValid)  |       1       |       0 / 1   |   0(ResultNotAcknowledge) |   0(CommandNotAcknowledged)   |   0(CommandAccepted)      |   0(NotValid) |  *3(Ready)   Dosierung / Entleerung abgebrochen->SMRunning
            /// [33]  0(NotValid)  |       1       |       0 / 1   |   0(ResultNotAcknowledge) |   0(CommandNotAcknowledged)   |   0(CommandAccepted)      |  *1(Valid)    |   3(Ready)   Rückgabewerte geschrieben         ->SMCompleted
            /// [9 ]  0(NotValid)  |      *0       |       0       |  *1(Acknowledge)          |   0(CommandNotAcknowledged)   |   0(CommandAccepted)      |   1(Valid)    |   3(Ready)   Rückgabewerte gelesen             ->SMResetting
            /// [10]  0(NotValid)  |       0       |       0       |   1(Acknowledge)          |   0(CommandNotAcknowledged)   |   0(CommandAccepted)      |  *0(NotValid) |   3(Ready)   Handshake                         ->SMResetting
            /// [11]  0(NotValid)  |       0       |       0       |  *0(ResultNotAcknowledge) |   0(CommandNotAcknowledged)   |   0(CommandAccepted)      |   0(NotValid) |   3(Ready)   Handshake                         ->SMResetting


            ACStateEnum translatedState = ACStateEnum.SMIdle;
            if (ADMBMachineState.ValueT == ADMB_MachineState.Idle)
            {
                if (CBCommandValid.ValueT == CB_CommandValid.Valid
                    || CRBCommandAcknowledge.ValueT == CRB_CommandAcknowledge.Acknowledge
                    || CRBCommandResponseCode.ValueT != CRB_CommandResponseCode.CommandAccepted)
                    translatedState = ACStateEnum.SMStarting;
                else
                    translatedState = ACStateEnum.SMIdle;
            }
            else if (ADMBMachineState.ValueT == ADMB_MachineState.Busy)
            {
                translatedState = ACStateEnum.SMRunning;
            }
            else if (ADMBMachineState.ValueT == ADMB_MachineState.Alarm)
            {
                if (CBCommandValid.ValueT == CB_CommandValid.Valid
                    || CRBCommandAcknowledge.ValueT == CRB_CommandAcknowledge.Acknowledge
                    || CRBCommandResponseCode.ValueT != CRB_CommandResponseCode.CommandAccepted)
                {
                    translatedState = ACStateEnum.SMStarting;
                }
                else
                    translatedState = ACStateEnum.SMPaused;
            }
            else //if (ADMBMachineState.ValueT == ADMB_MachineState.Ready)
            {
                if (RBResultValid.ValueT == RB_ResultValid.Valid && RRBResultAcknowledge.ValueT == RRB_ResultAcknowledge.ResultNotAcknowledge)
                    translatedState = ACStateEnum.SMCompleted;
                else
                    translatedState = ACStateEnum.SMResetting;
            }
            return translatedState;
        }


        /// <summary>
        /// Resets the Commands which are set from iPlus
        /// </summary>
        /// <param name="stateInPLC"></param>
        private void RepairHandshakeValuesIfOutOfSync(ACStateEnum stateInPLC)
        {
            ACStateEnum currentState = stateInPLC;
            var currentActiveFunction = CurrentActiveFunction;
            if (currentActiveFunction != null)
            {
                currentState = currentActiveFunction.CurrentACState;
                if (currentState <= ACStateEnum.SMStarting)
                {
                    PAFDosingKSE dosing = currentActiveFunction as PAFDosingKSE;
                    PAFDischargingKSE discharging = currentActiveFunction as PAFDischargingKSE;
                    if (dosing != null)
                    {
                        dosing.Malfunction.ValueT = PANotifyState.Off;
                        dosing.StateTolerance.ValueT = PANotifyState.Off;
                        dosing.StateLackOfMaterial.ValueT = PANotifyState.Off;
                        dosing.StateDosingTime.ValueT = PANotifyState.Off;
                    }
                    if (discharging != null)
                    {
                        discharging.Malfunction.ValueT = PANotifyState.Off;
                        discharging.StateDestinationFull.ValueT = PANotifyState.Off;
                    }
                    if (stateInPLC == ACStateEnum.SMCompleted
                        && CBCommandValid.ValueT == CB_CommandValid.NotValid
                        && RRBResultAcknowledge.ValueT == RRB_ResultAcknowledge.ResultNotAcknowledge)
                    {
                        RRBResultAcknowledge.ValueT = RRB_ResultAcknowledge.Acknowledge;
                        LogStateMachine(4010);
                    }
                }
            }

            // RESET CBCommandValid
            if (   (CBCommandValid.ValueT != CB_CommandValid.NotValid)
                && (stateInPLC < ACStateEnum.SMStarting
                    || stateInPLC > ACStateEnum.SMRunning))
            {
                CBCommandValid.ValueT = CB_CommandValid.NotValid;
                LogStateMachine(4020);
            }
            // RESET RRBResultAcknowledge
            if (   (RRBResultAcknowledge.ValueT == RRB_ResultAcknowledge.Acknowledge)
                && (stateInPLC < ACStateEnum.SMRunning))
            {
                RRBResultAcknowledge.ValueT = RRB_ResultAcknowledge.ResultNotAcknowledge;
                LogStateMachine(4030);
            }
            // RESET ACBResetAlarmStatus
            if (   (ACBResetAlarmStatus.ValueT > 0)
                && (   stateInPLC < ACStateEnum.SMStarting
                        || stateInPLC >= ACStateEnum.SMResetting
                        || ADMBMachineState.ValueT == ADMB_MachineState.Busy))
            {
                ACBResetAlarmStatus.ValueT = 0;
                LogStateMachine(4040);
            }
            // RESET ACBStopCommand
            if (   (ACBStopCommand.ValueT > 0)
                && (stateInPLC < ACStateEnum.SMRunning
                        || stateInPLC >= ACStateEnum.SMCompleted
                        || ADMBMachineState.ValueT == ADMB_MachineState.Busy))
            {
                ACBStopCommand.ValueT = 0;
                LogStateMachine(4050);
            }
            // RESET ASRResetMachine
            if (   (ASRResetMachine.ValueT > 0)
                && (stateInPLC == ACStateEnum.SMIdle || ADMBMachineState.ValueT == ADMB_MachineState.Idle))
            {
                ASRResetMachine.ValueT = 0;
            }
        }

        #endregion


        #region Sending
        public override MsgWithDetails SendACMethod(PAProcessFunction sender, ACMethod acMethod, gip.core.datamodel.ACMethod previousParams = null)
        {
            if (!IsReadyForSending)
                return new MsgWithDetails("Keine TCP-Verbindung zu KSE", this, eMsgLevel.Error, ClassName, "SendACMethod", 1000);

            if (ADMBMachineMode.ValueT != ADMB_MachineMode.AutoMode)
                return new MsgWithDetails("Waage ist nicht im Automatikmodus", this, eMsgLevel.Error, ClassName, "SendACMethod", 1010);

            if (   (CBCommandValid.ValueT == CB_CommandValid.Valid || CRBCommandAcknowledge.ValueT == CRB_CommandAcknowledge.Acknowledge)
                && (sender.CurrentACState == ACStateEnum.SMStarting || sender.CurrentACState == ACStateEnum.SMRunning))
            {
                return null;
            }

            if (sender is PAFDosingKSE)
            {
                WriteParamsDosingAndStart(acMethod);
                return null;
            }
            else if (sender is PAFDischargingKSE)
            {
                WriteParamsDischargingOrPositioningAndStart(acMethod);
                return null;
            }
            return new MsgWithDetails("Wrong Parent-ACComponent", this, eMsgLevel.Error, ClassName, "SendACMethod", 1020);
        }

        private void WriteParamsDosingAndStart(ACMethod acMethod)
        {
            CBCommandValid.ValueT = CB_CommandValid.NotValid;

            RBCommandNumberGenerateIncrement();
            CBCommandNumber.ValueT = (UInt16)RBCommandNumberGenerate;

            bool odrRest = acMethod.ParameterValueList["ODRReset"] != null ? (bool)acMethod.ParameterValueList["ODRReset"] : false;
            if (odrRest)
                CBCommandOperationMode.ValueT = CB_CommandOperationMode.Reset;
            else
                CBCommandOperationMode.ValueT = CB_CommandOperationMode.Dose;

            if ((acMethod.ParameterValueList["DosingMode"] != null) && ((CB_DosingMode)acMethod.ParameterValueList["DosingMode"] <= CB_DosingMode.DoseToEmpty))
                CBDosingMode.ValueT = (CB_DosingMode)acMethod.ParameterValueList["DosingMode"];
            else
                CBDosingMode.ValueT = CB_DosingMode.ODRDosing;

            if (acMethod.ParameterValueList["Source"] != null)
                CBDosingBinNumber.ValueT = Convert.ToUInt16((Int16)acMethod.ParameterValueList["Source"]);
            else
                CBDosingBinNumber.ValueT = 0;

            CBDischargingPosition.ValueT = 0;    // (UInt16)acMethod.ParameterValueList["DischargePosition"];
            CBWeigherNumber.ValueT = (CB_WeigherNumber)acMethod.ParameterValueList["WeigherNumber"];

            CBCheckTare.ValueT = 1;
            if (acMethod.ParameterValueList["CheckTare"] != null)
            {
                if ((bool)acMethod.ParameterValueList["CheckTare"])
                {
                    CBCheckTare.ValueT = 2;
                }
            }

            double nSollMenge = 0;
            if (acMethod.ParameterValueList["TargetQuantity"] != null)
                nSollMenge = (double)acMethod.ParameterValueList["TargetQuantity"];
            CBRequestedWeight.ValueT = Convert.ToInt32(nSollMenge * 1000); // kg -> g

            double nWert = 0;
            if (acMethod.ParameterValueList["TolerancePlus"] != null)
                nWert = (double)acMethod.ParameterValueList["TolerancePlus"];
            CBTolerancePositive.ValueT = Convert.ToUInt16(nWert * 1000); // kg -> g

            nWert = 0;
            if (acMethod.ParameterValueList["ToleranceMinus"] != null)
                nWert = (double)acMethod.ParameterValueList["ToleranceMinus"];
            CBToleranceNegative.ValueT = Convert.ToUInt16(nWert * 1000); // kg -> g

            nWert = 0;
            if (acMethod.ParameterValueList["DosingAccuracy"] != null)
                nWert = (double)acMethod.ParameterValueList["DosingAccuracy"];
            CBDosingAccuracy.ValueT = Convert.ToUInt16(nWert * 1000); // kg -> g

            try
            {
                CBIndicationDosability.ValueT = (CB_IndicationDosability)acMethod.ParameterValueList["IndicationDosibility"];  //CB_IndicationDosability.UseLocalValue;          //(UInt16)acMethod.ParameterValueList["DosingAccuracy"];
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                if (gip.core.datamodel.Database.Root != null && gip.core.datamodel.Database.Root.Messages != null &&
                                                                      gip.core.datamodel.Database.Root.InitState == ACInitState.Initialized)
                    gip.core.datamodel.Database.Root.Messages.LogException("KSEConvContScale", "WriteParamsDosing", msg);
            }

            if (acMethod.ParameterValueList["PositioningDestination"] != null)
                CBPosDosDestination.ValueT = (UInt16)acMethod.ParameterValueList["PositioningDestination"];
            else
                CBPosDosDestination.ValueT = 0;

            CBMassDensity.ValueT = 0;        // Automatic mode

            if (acMethod.ParameterValueList["BatchID"] != null)
                CBBatchID.ValueT = (UInt16)acMethod.ParameterValueList["BatchID"];
            else
                CBBatchID.ValueT = 0;

            if (acMethod.ParameterValueList["BatchEnd"] != null)
                CBBatchEnd.ValueT = (CB_BatchEnd)acMethod.ParameterValueList["BatchEnd"];
            else
                CBBatchEnd.ValueT = 0;

            // START
            CBCommandValid.ValueT = CB_CommandValid.Valid;
            LogStateMachine(5010);
        }

        private void WriteParamsDischargingOrPositioningAndStart(ACMethod acMethod)
        {
            CBCommandValid.ValueT = CB_CommandValid.NotValid;

            // Falls NAchbehälter belegt, dann wird zuerst posiitoniert und danach eine Entleerung gestartet, 
            // damit die Verfahrzeit kurz gehalten wird
            if (acMethod.ParameterValueList["PositioningDestination"] != null)
                CBPosDosDestination.ValueT = (UInt16)acMethod.ParameterValueList["PositioningDestination"];
            else
                CBPosDosDestination.ValueT = 0;

            RBCommandNumberGenerateIncrement();
            CBCommandNumber.ValueT = (UInt16)RBCommandNumberGenerate;

            CBCommandOperationMode.ValueT = CBPosDosDestination.ValueT > 0 ? CB_CommandOperationMode.Position : CB_CommandOperationMode.Discharge;
            CBDosingBinNumber.ValueT = 0;

            // Ziel an KSE-Steuerung schreiben wenn es sich um eine Entelerung handelt und nicht um eine Positionierung der Waage über dem Entleerloch
            if (CBPosDosDestination.ValueT == 0 && acMethod.ParameterValueList["DischargePosition"] != null)
                CBDischargingPosition.ValueT = (UInt16)acMethod.ParameterValueList["DischargePosition"];
            else
                CBDischargingPosition.ValueT = 0;

            CBWeigherNumber.ValueT = 0;
            CBCheckTare.ValueT = 1;
            CBRequestedWeight.ValueT = 0;
            CBTolerancePositive.ValueT = 0;
            CBToleranceNegative.ValueT = 0;
            CBDosingAccuracy.ValueT = 0;
            CBIndicationDosability.ValueT = 0;


            CBMassDensity.ValueT = 0;        // Automatic mode
            CBBatchID.ValueT = 0;
            CBBatchEnd.ValueT = 0;
            // START
            CBCommandValid.ValueT = CB_CommandValid.Valid;
            LogStateMachine(6010);

            // Ziel an GIP-SPS schreiben
            if (CBPosDosDestination.ValueT == 0)
                Destination.ValueT = Convert.ToUInt16((Int16)acMethod.ParameterValueList["Destination"]);
            else
                Destination.ValueT = 0;
        }

        private void RBCommandNumberGenerateIncrement()
        {
            if (RBCommandNumberGenerate >= 1000)
                RBCommandNumberGenerate = 1;
            else
                RBCommandNumberGenerate++;
        }
        #endregion


        #region Receiving
        public override PAProcessFunction.CompleteResult ReceiveACMethodResult(PAProcessFunction sender, ACMethod acMethod, out MsgWithDetails msg)
        {
            RB_ProcessResultAlarmCode alarmCode = RBProcessResultAlarmCode.ValueT; // ADMBDosingOrDischargingProcessAlarm.ValueT;
            PAProcessFunction.CompleteResult completeResult = PAProcessFunction.CompleteResult.Succeeded;
            if (RRBResultAcknowledge.ValueT == RRB_ResultAcknowledge.ResultNotAcknowledge)
            {
                PAProcessFunction currentActiveFunction = CurrentActiveFunction;
                if (currentActiveFunction == null)
                {
                    msg = new MsgWithDetails("No function active!", this, eMsgLevel.Error, ClassName, "ReceiveACMethodResult", 1899);
                    return PAProcessFunction.CompleteResult.FailedAndWait;
                }
                if (RBResultCode.ValueT == RB_ResultCode.NotDefined && !IsSimulationOn)
                {
                    msg = new MsgWithDetails("Waiting for KSE", this, eMsgLevel.Error, ClassName, "ReceiveACMethodResult", 1900);
                    return PAProcessFunction.CompleteResult.FailedAndWait;
                }

                PAFDosingKSE activeDosingFunc = currentActiveFunction as PAFDosingKSE;
                PAFDischargingKSE activeDischargingFunc = currentActiveFunction as PAFDischargingKSE;

                bool isOutOfTolerance = false;
                double nWeight = 0.0;
                if (   RBResultCode.ValueT == RB_ResultCode.Dose 
                    || (activeDosingFunc != null && RBResultCode.ValueT == RB_ResultCode.NotDefined))
                {
                    nWeight = Convert.ToDouble(RBResultDosedWeight.ValueT);
                    if (nWeight < 0.0001 && alarmCode > RB_ProcessResultAlarmCode.None)
                        nWeight = 0.0000001; // Damit kein Alarm generiert wird in AnalyzeACMethodResult, weil Alarm sowieso weiter unten generiert wird
                    else
                        nWeight = nWeight / 1000; // to kg

                    double nTolerancePlus = (double)acMethod.ParameterValueList["TolerancePlus"];
                    double nToleranceMinus = (double)acMethod.ParameterValueList["ToleranceMinus"];
                    double nRequestedWeight = (double)acMethod.ParameterValueList["TargetQuantity"];
                    acMethod.ResultValueList["ActualQuantity"] = nWeight;
                    UInt16 nDosingTime = RBDosingTime.ValueT;
                    acMethod.ResultValueList["DosingTime"] = nDosingTime;

                    double nWeightPlusTol = nRequestedWeight + nTolerancePlus;
                    double nWeightMinusTol = nRequestedWeight - nToleranceMinus;
                    if (nWeight > nWeightPlusTol)
                        isOutOfTolerance = true;
                    if (nWeight < nWeightMinusTol)
                        isOutOfTolerance = true;
                }
                else if    (RBResultCode.ValueT == RB_ResultCode.Discharge 
                        || (activeDischargingFunc != null && RBResultCode.ValueT == RB_ResultCode.NotDefined))
                {
                    nWeight = Convert.ToDouble(RBResultDosedWeight.ValueT) / 1000; // to kg
                    acMethod.ResultValueList["ActualQuantity"] = nWeight;
                }
                else if (RBResultCode.ValueT == RB_ResultCode.Position 
                    ||  (activeDischargingFunc != null && RBResultCode.ValueT == RB_ResultCode.NotDefined))
                {
                    acMethod.ResultValueList["ActualQuantity"] = 0;
                }

                if (alarmCode > RB_ProcessResultAlarmCode.None)
                {
                    string alarmCodeAsText = alarmCode.ToString();
                    ACValueItem alarmCodeItem = RB_ProcessResultAlarmCodeList.Where(c => (RB_ProcessResultAlarmCode)c.Value == alarmCode).FirstOrDefault();
                    if (alarmCodeItem != null)
                        alarmCodeAsText = alarmCodeItem.ACCaption;
                    /// Wenn ein Fehler auftritt, soll das PDF-Dokument "Process-Alarms" dem Bediener angezeigt werden
                    switch (alarmCode)
                    {
                        case RB_ProcessResultAlarmCode.DosingForcedStopped:
                            /// Falls in Toleranz, dann mit nächster Komponente weiter machen (automatisch quittieren)
                            /// Bei Überdosierung, soll Bediener entscheiden ob eine Sonderentleerung gemacht wird, sonst weiter mit nächster Komponente
                            /// Unterdosierung, soll Bediener entscheiden ob er die Komponente mit der Restmenge dosieren will
                            if (activeDosingFunc != null)
                            {
                                if (!isOutOfTolerance
                                    || (activeDosingFunc.StateTolerance.ValueT == PANotifyState.AlarmOrFault && activeDosingFunc.FaultAckTolerance.ValueT))
                                {
                                    activeDosingFunc.StateLackOfMaterial.ValueT = PANotifyState.Off;
                                }
                                else
                                {
                                    activeDosingFunc.StateTolerance.ValueT = PANotifyState.AlarmOrFault;
                                    msg = new MsgWithDetails(alarmCodeAsText + " / Out of tolerance due to manual stop / Auserhalb der Toleranz wegen manuellem Stop", this, eMsgLevel.Error, ClassName, "ReceiveACMethodResult", 1002);
                                    return PAProcessFunction.CompleteResult.FailedAndWait;
                                }
                            }
                            break;
                        case RB_ProcessResultAlarmCode.DosingAborted:
                        case RB_ProcessResultAlarmCode.DosingModeConfigurationError:
                        case RB_ProcessResultAlarmCode.SoftwareConfigurationError:
                            /// Alarm dem Bediener anzeigen. 
                            /// Problem kann nur durh KSE gelöst werden.
                            /// Der Schritt bleibt solange stehen, bis der Anlagenfahrer den Alarm quittiert hat und danach soll wieder Dosierung/Entleerung erneut gestartet werden
                            if (currentActiveFunction != null)
                            {
                                if (currentActiveFunction.Malfunction.ValueT == PANotifyState.AlarmOrFault && currentActiveFunction.AckMalfunction.ValueT)
                                {
                                    currentActiveFunction.Malfunction.ValueT = PANotifyState.Off;
                                    if (activeDischargingFunc != null)
                                        completeResult = PAProcessFunction.CompleteResult.FailedAndRepeat;
                                }
                                else
                                {
                                    currentActiveFunction.Malfunction.ValueT = PANotifyState.AlarmOrFault;
                                    msg = new MsgWithDetails(alarmCodeAsText + " / Internal Problem of KSE / Internes Problem bei KSE. Bitte KSE anrufen, Problem beheben und dann quittieren.", this, eMsgLevel.Error, ClassName, "ReceiveACMethodResult", 1004);
                                    return PAProcessFunction.CompleteResult.FailedAndWait;
                                }
                            }
                            break;
                        case RB_ProcessResultAlarmCode.InputParametersError:
                        case RB_ProcessResultAlarmCode.IncorrectContainerID:
                            /// Der Schritt bleibt stehen. 
                            /// - InputParametersError: Der Bediener muss die richtigen Parameter eingeben. 
                            /// Nach der Quttierung soll der Schritt erneut gestartet werden.
                            if (currentActiveFunction != null)
                            {
                                if (currentActiveFunction.Malfunction.ValueT == PANotifyState.AlarmOrFault && currentActiveFunction.AckMalfunction.ValueT)
                                {
                                    currentActiveFunction.Malfunction.ValueT = PANotifyState.Off;
                                    if (activeDischargingFunc != null)
                                        completeResult = PAProcessFunction.CompleteResult.FailedAndRepeat;
                                }
                                else
                                {
                                    currentActiveFunction.Malfunction.ValueT = PANotifyState.AlarmOrFault;
                                    msg = new MsgWithDetails(alarmCodeAsText + " / Wrong parameters / Falsche Parameterübergabe. Bitte Korrigieren Sie die Parameter bevor die Funktion erneut gestartet wird.", this, eMsgLevel.Error, ClassName, "ReceiveACMethodResult", 1005);
                                    return PAProcessFunction.CompleteResult.FailedAndWait;
                                }
                            }
                            break;
                        case RB_ProcessResultAlarmCode.DoseWeigherNo2NotEmptyBeforeDosing:
                        case RB_ProcessResultAlarmCode.DoseWeigherNo1NotEmptyBeforeDosing:
                        case RB_ProcessResultAlarmCode.WeigherNo2TooFarNegative:
                        case RB_ProcessResultAlarmCode.WeigherNo1TooFarNegative:
                            /// Der Schritt bleibt stehen.
                            /// - NotEmptyBeforeDosing: Der Bediener reinigt die Waage oder erhöht den Leerbereich.
                            /// - TooFarNegative: Der Bediener prüft warum die Waage so einen negativen Wert anzeigt
                            /// Nach Quittierung soll der Schritt erneut gestartet werden.
                            if (currentActiveFunction != null)
                            {
                                if (currentActiveFunction.Malfunction.ValueT == PANotifyState.AlarmOrFault && activeDosingFunc.AckMalfunction.ValueT)
                                {
                                    currentActiveFunction.Malfunction.ValueT = PANotifyState.Off;
                                    if (activeDischargingFunc != null)
                                        completeResult = PAProcessFunction.CompleteResult.FailedAndRepeat;
                                }
                                else
                                {
                                    currentActiveFunction.Malfunction.ValueT = PANotifyState.AlarmOrFault;
                                    msg = new MsgWithDetails(alarmCodeAsText + " / Weigher not empty or to far negative / Die Waage ist nicht leer oder zu weit im negativen Bereich. Reinigen Sie die Waage. Prüfen Sie den Leerbereich.", this, eMsgLevel.Error, ClassName, "ReceiveACMethodResult", 1006);
                                    return PAProcessFunction.CompleteResult.FailedAndWait;
                                }
                            }
                            break;
                        case RB_ProcessResultAlarmCode.WeigherNo2OverloadExpected:
                        case RB_ProcessResultAlarmCode.WeigherNo1OverloadExpected:
                        case RB_ProcessResultAlarmCode.WeigherNo2OverflowExpected:
                        case RB_ProcessResultAlarmCode.WeigherNo1OverflowExpected:
                            /// Der Schritt bleibt stehen. 
                            /// - WeigherNo2OverloadExpected: Die Sollmenge muss reduziert werden
                            ///      -> Nach der Quttierung soll der Schritt erneut gestartet werden.
                            /// - WeigherNo1OverloadExpected: Große Waage muss zuerst entleert werden. 
                            ///      -> (Dosierschritt auf Pause setzen. Entleerfunktion im Monitor starten. Dosierschritt fortsetzen)
                            /// - OverflowExpected: Die Sollmenge muss reduziert werden, weil Max-Volumen überschritten wurde
                            ///     Nach der Quttierung soll der Schritt erneut gestartet werden.
                            if (activeDosingFunc != null)
                            {
                                if (activeDosingFunc.Malfunction.ValueT == PANotifyState.AlarmOrFault && activeDosingFunc.AckMalfunction.ValueT)
                                {
                                    activeDosingFunc.Malfunction.ValueT = PANotifyState.Off;
                                }
                                else
                                {
                                    activeDosingFunc.Malfunction.ValueT = PANotifyState.AlarmOrFault;
                                    msg = new MsgWithDetails(alarmCodeAsText + " / Overload or Overflow expected / Überladung oder Überfüllung der Waage erwartet. Reduzieren Sie das Sollgewicht oder entleeren Sie die Waage", this, eMsgLevel.Error, ClassName, "ReceiveACMethodResult", 1010);
                                    return PAProcessFunction.CompleteResult.FailedAndWait;
                                }
                            }
                            break;
                        case RB_ProcessResultAlarmCode.NoStandstillWeightIndicatorBeforeDosing:
                        case RB_ProcessResultAlarmCode.SoftwareReleaseOfDosingDisturbedBeforeDosing:
                        case RB_ProcessResultAlarmCode.WeightIndicatorAlarmAtStartDosing:
                        case RB_ProcessResultAlarmCode.HardwareAlarmBeforeDosing:
                        case RB_ProcessResultAlarmCode.WrongSlideOpen:
                        case RB_ProcessResultAlarmCode.WeightIncreaseInWrongWeigher:
                        case RB_ProcessResultAlarmCode.WeightIndicatorAlarm:
                        case RB_ProcessResultAlarmCode.CalculationDosedWeightError:
                        case RB_ProcessResultAlarmCode.NoDischargedWeightAfterDischarge:
                        case RB_ProcessResultAlarmCode.NoStandstillWeightIndicator:
                        case RB_ProcessResultAlarmCode.WeigherNo2NotEmptyAfterDischarge:
                        case RB_ProcessResultAlarmCode.WeigherNo1NotEmptyAfterDischarge:
                        case RB_ProcessResultAlarmCode.WeightSignalNo2TooFarNegativeAfterDischarge:
                        case RB_ProcessResultAlarmCode.WeightSignalNo1TooFarNegativeAfterDischarge:
                            /// Der Schritt bleibt stehen. 
                            /// Technisches Problem muss gelöst werden (evtl. KSE anrufen)
                            /// Nach der Quttierung soll der Schritt erneut gestartet werden.
                            if (currentActiveFunction != null)
                            {
                                if (currentActiveFunction.Malfunction.ValueT == PANotifyState.AlarmOrFault && currentActiveFunction.AckMalfunction.ValueT)
                                {
                                    currentActiveFunction.Malfunction.ValueT = PANotifyState.Off;
                                    if (activeDischargingFunc != null)
                                        completeResult = PAProcessFunction.CompleteResult.FailedAndRepeat;
                                }
                                else
                                {
                                    currentActiveFunction.Malfunction.ValueT = PANotifyState.AlarmOrFault;
                                    msg = new MsgWithDetails(alarmCodeAsText + " / Technical Problem / Technisches Problem. Bitte beheben Sie das Problem oder rufen evtl. KSE an. Anschliessend quitteren!", this, eMsgLevel.Error, ClassName, "ReceiveACMethodResult", 1014);
                                    return PAProcessFunction.CompleteResult.FailedAndWait;
                                }
                            }
                            break;
                        case RB_ProcessResultAlarmCode.DosedWeightAboveTolerance:
                        case RB_ProcessResultAlarmCode.WeightIncreaseInWrongWeigherAndAboveTolerance:
                        case RB_ProcessResultAlarmCode.WeightIncreaseInWrongWeigherAndAboveRejectionLimit:
                            /// Sonderentleerung als Option (TODO: PRÜFEN. Ziel sollte automatisch eingetragen werden)
                            /// Sonst kann er es nur akzeptieren und weitermachen (evtl. Material vorher manuell aus der Waage entnehmen)
                            if (activeDosingFunc != null)
                            {
                                if (activeDosingFunc.StateTolerance.ValueT == PANotifyState.AlarmOrFault && activeDosingFunc.FaultAckTolerance.ValueT)
                                {
                                    activeDosingFunc.StateTolerance.ValueT = PANotifyState.Off;
                                }
                                else
                                {
                                    activeDosingFunc.StateTolerance.ValueT = PANotifyState.AlarmOrFault;
                                    msg = new MsgWithDetails(alarmCodeAsText + " / Upper tolerance / Über Toleranz", this, eMsgLevel.Error, ClassName, "ReceiveACMethodResult", 1027);
                                    return PAProcessFunction.CompleteResult.FailedAndWait;
                                }
                            }
                            break;
                        case RB_ProcessResultAlarmCode.DosedWeightBelowTolerance:
                            /// Anwender muss Silo sperren weil technisches Problem
                            /// TODO: SetAbortReasonMalfunction() soll möglich sein, weil technisches Problem
                            if (activeDosingFunc != null)
                            {
                                if (activeDosingFunc.StateTolerance.ValueT == PANotifyState.AlarmOrFault && activeDosingFunc.FaultAckTolerance.ValueT)
                                {
                                    activeDosingFunc.StateTolerance.ValueT = PANotifyState.Off;
                                }
                                else
                                {
                                    activeDosingFunc.StateTolerance.ValueT = PANotifyState.AlarmOrFault;
                                    msg = new MsgWithDetails(alarmCodeAsText + " / Under tolerance / Unter Toleranz", this, eMsgLevel.Error, ClassName, "ReceiveACMethodResult", 1028);
                                    return PAProcessFunction.CompleteResult.FailedAndWait;
                                }
                            }
                            break;
                        case RB_ProcessResultAlarmCode.DosingControlModuleAlarm:
                            /// Falls in Toleranz, dann mit nächster Komponente weiter machen (automatisch quittieren)
                            /// SONST:
                            /// Der Schritt bleibt stehen. 
                            /// Technisches Problem muss gelöst werden (evtl. KSE anrufen)
                            /// Nach der Quttierung soll der Schritt erneut gestartet werden.
                            if (currentActiveFunction != null)
                            {
                                if (   (!isOutOfTolerance && activeDosingFunc != null)
                                    || (currentActiveFunction.Malfunction.ValueT == PANotifyState.AlarmOrFault && currentActiveFunction.AckMalfunction.ValueT))
                                {
                                    currentActiveFunction.Malfunction.ValueT = PANotifyState.Off;
                                }
                                else
                                {
                                    currentActiveFunction.Malfunction.ValueT = PANotifyState.AlarmOrFault;
                                    msg = new MsgWithDetails(alarmCodeAsText + " / Module Alarm / Modul-Alarm", this, eMsgLevel.Error, ClassName, "ReceiveACMethodResult", 1033);
                                    return PAProcessFunction.CompleteResult.FailedAndWait;
                                }
                            }
                            break;
                        case RB_ProcessResultAlarmCode.NoDosingResultAfterDosing:
                        case RB_ProcessResultAlarmCode.DosedWeightAboveRejectionLimit:
                        case RB_ProcessResultAlarmCode.DosedWeightBelowRejectionLimit:
                        case RB_ProcessResultAlarmCode.NoFlowDetectedDosedWeightInGeneral:
                        case RB_ProcessResultAlarmCode.MaxDosingTimeReached:
                        case RB_ProcessResultAlarmCode.NoFlowDetectedDosedWeightAboveRejectionLimit:  // AlarmCode 51
                        case RB_ProcessResultAlarmCode.NoFlowDetectedDosedWeightBelowRejectionLimit:  // AlarmCode 52
                        case RB_ProcessResultAlarmCode.NoFlowDetectedDosedWeightAboveTolerance:  // AlarmCode 53
                        case RB_ProcessResultAlarmCode.NoFlowDetectedDosedWeightBelowTolerance:  // AlarmCode 54
                            /// Handungsoptionen wenn ausserhalb Toleranz: Silowechsel, Sonderentleerung, Wiederholen
                            if (activeDosingFunc != null)
                            {
                                if (!isOutOfTolerance
                                    || (activeDosingFunc.StateLackOfMaterial.ValueT == PANotifyState.AlarmOrFault && activeDosingFunc.FaultAckLackOfMaterial.ValueT)
                                    || activeDosingFunc.DosingAbortReason.ValueT != gip.mes.processapplication.PADosingAbortReason.NotSet)
                                {
                                    this.DosingFunction.StateLackOfMaterial.ValueT = PANotifyState.Off;
                                }
                                else
                                {
                                    this.DosingFunction.StateLackOfMaterial.ValueT = PANotifyState.AlarmOrFault;
                                    msg = new MsgWithDetails(alarmCodeAsText + " / No Flow / Kein Material", this, eMsgLevel.Error, ClassName, "ReceiveACMethodResult", 1055);
                                    return PAProcessFunction.CompleteResult.FailedAndWait;
                                }
                            }
                            break;
                    }
                }
                RRBResultAcknowledge.ForceBroadcast = true;
                RRBResultAcknowledge.ValueT = RRB_ResultAcknowledge.Acknowledge;
                LogStateMachine(7010);
            }
            else
            {
                msg = new MsgWithDetails("Waiting for KSE for ResultNotAcknowledge", this, eMsgLevel.Error, ClassName, "ReceiveACMethodResult", 1910);
                return PAProcessFunction.CompleteResult.FailedAndWait;
            }

            // Ziel in GIP-SPS zurücksetzen
            Destination.ValueT = 0;

            msg = null;
            return completeResult;
        }
        #endregion

        #region Logging
        private void LogStateMachine(int calledFrom)
        {
            if (!LoggingEnabled)
                return;
            ACStateEnum currentACState = ACStateEnum.SMIdle;
            var currentActiveFunction = CurrentActiveFunction;
            if (currentActiveFunction != null)
                currentACState = currentActiveFunction.CurrentACState;

            int stateNoInExcel = 0;

            if (CBCommandValid.ValueT == CB_CommandValid.NotValid
                && ACBResetAlarmStatus.ValueT == 0
                && ACBStopCommand.ValueT == 0
                && RRBResultAcknowledge.ValueT == RRB_ResultAcknowledge.ResultNotAcknowledge
                && CRBCommandAcknowledge.ValueT == CRB_CommandAcknowledge.CommandNotAcknowledged
                && CRBCommandResponseCode.ValueT == CRB_CommandResponseCode.CommandAccepted
                && RBResultValid.ValueT == RB_ResultValid.NotValid
                && ADMBMachineState.ValueT == ADMB_MachineState.Idle
                && currentACState != ACStateEnum.SMStarting)
                stateNoInExcel = 1;
            else if (CBCommandValid.ValueT == CB_CommandValid.Valid
                && ACBResetAlarmStatus.ValueT == 0
                && ACBStopCommand.ValueT == 0
                && RRBResultAcknowledge.ValueT == RRB_ResultAcknowledge.ResultNotAcknowledge
                && CRBCommandAcknowledge.ValueT == CRB_CommandAcknowledge.CommandNotAcknowledged
                && CRBCommandResponseCode.ValueT == CRB_CommandResponseCode.CommandAccepted
                && RBResultValid.ValueT == RB_ResultValid.NotValid
                && ADMBMachineState.ValueT == ADMB_MachineState.Idle)
                stateNoInExcel = 2;
            else if (CBCommandValid.ValueT == CB_CommandValid.Valid
                && ACBResetAlarmStatus.ValueT == 0
                && ACBStopCommand.ValueT == 0
                && RRBResultAcknowledge.ValueT == RRB_ResultAcknowledge.ResultNotAcknowledge
                && CRBCommandAcknowledge.ValueT == CRB_CommandAcknowledge.Acknowledge
                && CRBCommandResponseCode.ValueT == CRB_CommandResponseCode.CommandAccepted
                && RBResultValid.ValueT == RB_ResultValid.NotValid
                && ADMBMachineState.ValueT == ADMB_MachineState.Idle)
                stateNoInExcel = 3;
            else if (CBCommandValid.ValueT == CB_CommandValid.NotValid
                && ACBResetAlarmStatus.ValueT == 0
                && ACBStopCommand.ValueT == 0
                && RRBResultAcknowledge.ValueT == RRB_ResultAcknowledge.ResultNotAcknowledge
                && CRBCommandAcknowledge.ValueT == CRB_CommandAcknowledge.Acknowledge
                && CRBCommandResponseCode.ValueT == CRB_CommandResponseCode.CommandAccepted
                && RBResultValid.ValueT == RB_ResultValid.NotValid
                && ADMBMachineState.ValueT == ADMB_MachineState.Idle)
                stateNoInExcel = 4;
            else if (CBCommandValid.ValueT == CB_CommandValid.NotValid
                && ACBResetAlarmStatus.ValueT == 0
                && ACBStopCommand.ValueT == 0
                && RRBResultAcknowledge.ValueT == RRB_ResultAcknowledge.ResultNotAcknowledge
                && CRBCommandAcknowledge.ValueT == CRB_CommandAcknowledge.CommandNotAcknowledged
                && CRBCommandResponseCode.ValueT == CRB_CommandResponseCode.CommandAccepted
                && RBResultValid.ValueT == RB_ResultValid.NotValid
                && ADMBMachineState.ValueT == ADMB_MachineState.Idle
                && currentACState == ACStateEnum.SMStarting)
                stateNoInExcel = 5;
            else if (CBCommandValid.ValueT == CB_CommandValid.NotValid
                && ACBResetAlarmStatus.ValueT == 0
                && ACBStopCommand.ValueT == 0
                && RRBResultAcknowledge.ValueT == RRB_ResultAcknowledge.ResultNotAcknowledge
                && CRBCommandAcknowledge.ValueT == CRB_CommandAcknowledge.CommandNotAcknowledged
                && CRBCommandResponseCode.ValueT == CRB_CommandResponseCode.CommandAccepted
                && RBResultValid.ValueT == RB_ResultValid.NotValid
                && ADMBMachineState.ValueT == ADMB_MachineState.Busy)
                stateNoInExcel = 6;
            else if (CBCommandValid.ValueT == CB_CommandValid.NotValid
                && ACBResetAlarmStatus.ValueT == 0
                && ACBStopCommand.ValueT == 0
                && RRBResultAcknowledge.ValueT == RRB_ResultAcknowledge.ResultNotAcknowledge
                && CRBCommandAcknowledge.ValueT == CRB_CommandAcknowledge.CommandNotAcknowledged
                && CRBCommandResponseCode.ValueT == CRB_CommandResponseCode.CommandAccepted
                && (RBResultValid.ValueT == RB_ResultValid.Valid || RBResultValid.ValueT == RB_ResultValid.NotValid)
                && ADMBMachineState.ValueT == ADMB_MachineState.Busy
                && currentACState == ACStateEnum.SMRunning)
                stateNoInExcel = 7;
            else if (CBCommandValid.ValueT == CB_CommandValid.NotValid
                && ACBResetAlarmStatus.ValueT == 0
                && ACBStopCommand.ValueT == 0
                && RRBResultAcknowledge.ValueT == RRB_ResultAcknowledge.ResultNotAcknowledge
                && CRBCommandAcknowledge.ValueT == CRB_CommandAcknowledge.CommandNotAcknowledged
                && CRBCommandResponseCode.ValueT == CRB_CommandResponseCode.CommandAccepted
                && RBResultValid.ValueT == RB_ResultValid.Valid
                && ADMBMachineState.ValueT == ADMB_MachineState.Ready)
                stateNoInExcel = 8;
            else if (CBCommandValid.ValueT == CB_CommandValid.NotValid
                && ACBResetAlarmStatus.ValueT == 0
                && ACBStopCommand.ValueT == 0
                && RRBResultAcknowledge.ValueT == RRB_ResultAcknowledge.Acknowledge
                && CRBCommandAcknowledge.ValueT == CRB_CommandAcknowledge.CommandNotAcknowledged
                && CRBCommandResponseCode.ValueT == CRB_CommandResponseCode.CommandAccepted
                && RBResultValid.ValueT == RB_ResultValid.Valid
                && ADMBMachineState.ValueT == ADMB_MachineState.Ready)
                stateNoInExcel = 9;
            else if (CBCommandValid.ValueT == CB_CommandValid.NotValid
                && ACBResetAlarmStatus.ValueT == 0
                && ACBStopCommand.ValueT == 0
                && RRBResultAcknowledge.ValueT == RRB_ResultAcknowledge.Acknowledge
                && CRBCommandAcknowledge.ValueT == CRB_CommandAcknowledge.CommandNotAcknowledged
                && CRBCommandResponseCode.ValueT == CRB_CommandResponseCode.CommandAccepted
                && RBResultValid.ValueT == RB_ResultValid.NotValid
                && ADMBMachineState.ValueT == ADMB_MachineState.Ready)
                stateNoInExcel = 10;
            else if (CBCommandValid.ValueT == CB_CommandValid.NotValid
                && ACBResetAlarmStatus.ValueT == 0
                && ACBStopCommand.ValueT == 0
                && RRBResultAcknowledge.ValueT == RRB_ResultAcknowledge.ResultNotAcknowledge
                && CRBCommandAcknowledge.ValueT == CRB_CommandAcknowledge.CommandNotAcknowledged
                && CRBCommandResponseCode.ValueT == CRB_CommandResponseCode.CommandAccepted
                && RBResultValid.ValueT == RB_ResultValid.NotValid
                && ADMBMachineState.ValueT == ADMB_MachineState.Ready
                && currentACState >= ACStateEnum.SMCompleted)
                stateNoInExcel = 11;
            else if (CBCommandValid.ValueT == CB_CommandValid.Valid
                && ACBResetAlarmStatus.ValueT == 0
                && ACBStopCommand.ValueT == 0
                && RRBResultAcknowledge.ValueT == RRB_ResultAcknowledge.ResultNotAcknowledge
                && CRBCommandAcknowledge.ValueT == CRB_CommandAcknowledge.Acknowledge
                && CRBCommandResponseCode.ValueT != CRB_CommandResponseCode.CommandAccepted
                && RBResultValid.ValueT == RB_ResultValid.NotValid
                && ADMBMachineState.ValueT == ADMB_MachineState.Idle)
                stateNoInExcel = 20;
            else if (CBCommandValid.ValueT == CB_CommandValid.Valid
                && ACBResetAlarmStatus.ValueT == 0
                && ACBStopCommand.ValueT == 0
                && RRBResultAcknowledge.ValueT == RRB_ResultAcknowledge.ResultNotAcknowledge
                && CRBCommandAcknowledge.ValueT == CRB_CommandAcknowledge.Acknowledge
                && CRBCommandResponseCode.ValueT != CRB_CommandResponseCode.CommandAccepted
                && RBResultValid.ValueT == RB_ResultValid.NotValid
                && ADMBMachineState.ValueT == ADMB_MachineState.Alarm)
                stateNoInExcel = 21;
            else if (CBCommandValid.ValueT == CB_CommandValid.NotValid
                && ACBResetAlarmStatus.ValueT == 1
                && ACBStopCommand.ValueT == 0
                && RRBResultAcknowledge.ValueT == RRB_ResultAcknowledge.ResultNotAcknowledge
                && CRBCommandAcknowledge.ValueT == CRB_CommandAcknowledge.Acknowledge
                && CRBCommandResponseCode.ValueT != CRB_CommandResponseCode.CommandAccepted
                && RBResultValid.ValueT == RB_ResultValid.NotValid
                && ADMBMachineState.ValueT == ADMB_MachineState.Alarm)
                stateNoInExcel = 22;
            else if (CBCommandValid.ValueT == CB_CommandValid.NotValid
                && ACBResetAlarmStatus.ValueT == 1
                && ACBStopCommand.ValueT == 0
                && RRBResultAcknowledge.ValueT == RRB_ResultAcknowledge.ResultNotAcknowledge
                && CRBCommandAcknowledge.ValueT == CRB_CommandAcknowledge.CommandNotAcknowledged
                && CRBCommandResponseCode.ValueT != CRB_CommandResponseCode.CommandAccepted
                && RBResultValid.ValueT == RB_ResultValid.NotValid
                && ADMBMachineState.ValueT == ADMB_MachineState.Idle)
                stateNoInExcel = 23;
            else if (CBCommandValid.ValueT == CB_CommandValid.NotValid
                && ACBResetAlarmStatus.ValueT == 0
                && ACBStopCommand.ValueT == 0
                && RRBResultAcknowledge.ValueT == RRB_ResultAcknowledge.ResultNotAcknowledge
                && CRBCommandAcknowledge.ValueT == CRB_CommandAcknowledge.CommandNotAcknowledged
                && CRBCommandResponseCode.ValueT == CRB_CommandResponseCode.CommandAccepted
                && RBResultValid.ValueT == RB_ResultValid.NotValid
                && ADMBMachineState.ValueT == ADMB_MachineState.Idle)
                stateNoInExcel = 24;
            else if (CBCommandValid.ValueT == CB_CommandValid.NotValid
                && ACBResetAlarmStatus.ValueT == 0
                && ACBStopCommand.ValueT == 0
                && RRBResultAcknowledge.ValueT == RRB_ResultAcknowledge.ResultNotAcknowledge
                && CRBCommandAcknowledge.ValueT == CRB_CommandAcknowledge.CommandNotAcknowledged
                && CRBCommandResponseCode.ValueT == CRB_CommandResponseCode.CommandAccepted
                && RBResultValid.ValueT == RB_ResultValid.NotValid
                && ADMBMachineState.ValueT == ADMB_MachineState.Alarm)
                stateNoInExcel = 30;
            else if (CBCommandValid.ValueT == CB_CommandValid.NotValid
                && ACBResetAlarmStatus.ValueT == 1
                && ACBStopCommand.ValueT == 0
                && RRBResultAcknowledge.ValueT == RRB_ResultAcknowledge.ResultNotAcknowledge
                && CRBCommandAcknowledge.ValueT == CRB_CommandAcknowledge.CommandNotAcknowledged
                && CRBCommandResponseCode.ValueT == CRB_CommandResponseCode.CommandAccepted
                && RBResultValid.ValueT == RB_ResultValid.NotValid
                && ADMBMachineState.ValueT == ADMB_MachineState.Alarm)
                stateNoInExcel = 31;
            else if (CBCommandValid.ValueT == CB_CommandValid.NotValid
                && ACBResetAlarmStatus.ValueT == 1
                && ACBStopCommand.ValueT == 0
                && RRBResultAcknowledge.ValueT == RRB_ResultAcknowledge.ResultNotAcknowledge
                && CRBCommandAcknowledge.ValueT == CRB_CommandAcknowledge.CommandNotAcknowledged
                && CRBCommandResponseCode.ValueT == CRB_CommandResponseCode.CommandAccepted
                && RBResultValid.ValueT == RB_ResultValid.NotValid
                && ADMBMachineState.ValueT == ADMB_MachineState.Ready)
                stateNoInExcel = 32;
            else if (CBCommandValid.ValueT == CB_CommandValid.NotValid
                && ACBResetAlarmStatus.ValueT == 1
                && ACBStopCommand.ValueT == 0
                && RRBResultAcknowledge.ValueT == RRB_ResultAcknowledge.ResultNotAcknowledge
                && CRBCommandAcknowledge.ValueT == CRB_CommandAcknowledge.CommandNotAcknowledged
                && CRBCommandResponseCode.ValueT == CRB_CommandResponseCode.CommandAccepted
                && RBResultValid.ValueT == RB_ResultValid.Valid
                && ADMBMachineState.ValueT == ADMB_MachineState.Ready)
                stateNoInExcel = 33;

            string dumpStr = String.Format("<Excel: {11}><CBCommandValid: {0}> <ACBResetAlarmStatus: {1}> <ACBStopCommand: {2}> <RRBResultAcknowledge: {3}> <CRBCommandAcknowledge: {4}> <CRBCommandResponseCode: {5}> <RBResultValid: {6}> <ADMBMachineState: {7}> <PLCState: {8}> <ACState: {9}> <Called from: {10}>", 
                CBCommandValid.ValueT,
                ACBResetAlarmStatus.ValueT,
                ACBStopCommand.ValueT,
                RRBResultAcknowledge.ValueT,
                CRBCommandAcknowledge.ValueT,
                CRBCommandResponseCode.ValueT,
                RBResultValid.ValueT,
                ADMBMachineState.ValueT,
                DetermineACStateFromPLC(),
                currentACState,
                calledFrom,
                stateNoInExcel);
            Messages.LogDebug(GetACUrl(), "LogStateMachine", dumpStr);
        }
        #endregion

        #endregion


        #region Interaction-Methods
        [ACMethodInteraction("", "en{'Reset Machine KSE'}de{'Reset Machine KSE'}", 803, true)]
        public void ResetMachineKSE()
        {
            if (!IsEnabledResetMachineKSE())
                return;
            ASRResetMachine.ValueT = 1;
        }

        public bool IsEnabledResetMachineKSE()
        {
            return ASRResetMachine.ValueT == 0;
        }


        [ACMethodInteraction("", "en{'Reset Alarm-Status KSE'}de{'Reset Alarm-Status KSE'}", 804, true)]
        public virtual void ResetAlarmStateKSE()
        {
            if (!IsEnabledResetAlarmStateKSE())
                return;
            ACBResetAlarmStatus.ValueT = 1;
            LogStateMachine(9010);
        }

        public virtual bool IsEnabledResetAlarmStateKSE()
        {
            /// Only to Version 2.2:
            if ((ADMBPositioningEquipmentAlarm as IACPropertyNetTarget).Source != null
                && ADMBPositioningEquipmentAlarm.ValueT == ADMB_PositioningEquipmentAlarm.EquipmentHardwareAlarm
                && ACBResetAlarmStatus.ValueT == 0)
            {
                return true;
            }
            // From Version 3.0 upwards
            else if (ADMBVariousEquipmentsAlarm.ValueT != ADMB_VariousEquipmentsAlarm.None
                && ADMBMachineState.ValueT == ADMB_MachineState.Alarm
                && ACBResetAlarmStatus.ValueT == 0)
            {
                return true;
            }
            return false;
        }
        #endregion

        #endregion

    }
}

