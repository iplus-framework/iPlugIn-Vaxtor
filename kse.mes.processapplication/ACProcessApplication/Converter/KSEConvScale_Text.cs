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
    public partial class KSEConvScale
    {
        static ACValueItemList _RB_ProcessResultAlarmCodeList = null;
        [ACPropertyList(1000, "RB_ProcessResultAlarmCode")]
        static public ACValueItemList RB_ProcessResultAlarmCodeList
        {
            get
            {
                if (_RB_ProcessResultAlarmCodeList == null)
                {
                    _RB_ProcessResultAlarmCodeList = new ACValueItemList("RB_ProcessResultAlarmCodeIndex");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.None, "en{'No Alarm'}de{'Kein Alarm'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.DosingAborted, "en{'Dosing aborted'}de{'Dosierung abgebrochen'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.DosingForcedStopped, "en{'Dosing forced stopped'}de{'Dosierung gestoppt'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.DosingModeConfigurationError, "en{'Dosing mode configuration error'}de{'Konfigurationsfehler im Dosiermodus'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.SoftwareConfigurationError, "en{'Software configuration error'}de{'Softwarekonfigurationsfehler'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.InputParametersError, "en{'Input parameters error'}de{'Eingabeparameterfehler'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.DoseWeigherNo2NotEmptyBeforeDosing, "en{'Dose weigher no. 2 not empty before dosing'}de{'Dosierwaage Nr. 2 vor der Dosierung nicht leer'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.DoseWeigherNo1NotEmptyBeforeDosing, "en{'Dose weigher no. 1 not empty before dosing'}de{'Dosierwaage Nr. 1 vor der Dosierung nicht leer'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.WeigherNo2TooFarNegative, "en{'Weigher no. 2 too far negative'}de{'Waage Nr. 2 zu stark negativ'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.WeigherNo1TooFarNegative, "en{'Weigher no. 1 too far negative'}de{'Waage Nr. 1 zu stark negativ'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.WeigherNo2OverloadExpected, "en{'Weigher no. 2 overload expected'}de{'Waage Nr. 2 Überladung erwartet'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.WeigherNo1OverloadExpected, "en{'Weigher no. 1 overload expected'}de{'Waage Nr. 1 Überladung erwartet'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.WeigherNo2OverflowExpected, "en{'Weigher no. 2 overflow expected'}de{'Waage Nr. 2 Überfüllung erwartet'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.WeigherNo1OverflowExpected, "en{'Weigher no. 1 overflow expected'}de{'Waage Nr. 1 Überfüllung erwartet'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.NoStandstillWeightIndicatorBeforeDosing, "en{'No standstill weight indicator before dosing'}de{'Kein Waagenstillstand zum Beginnen der Dosierung'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.SoftwareReleaseOfDosingDisturbedBeforeDosing, "en{'Software release of dosing disturbed before dosing'}de{'Software-Freigabe der Vor-Dosierung gestört'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.WeightIndicatorAlarmAtStartDosing, "en{'Weight indicator alarm at start dosing'}de{'Gewichtsanzeige Alarm bei der Start-Dosierung'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.HardwareAlarmBeforeDosing, "en{'Hardware alarm before dosing'}de{'Hardware-Alarm vor Dosierstart'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.WrongSlideOpen, "en{'Wrong slide open'}de{'Falscher Schieber geöffnet'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.WeightIncreaseInWrongWeigher, "en{'Weight increase in wrong weigher'}de{'Gewichtszunahme bei falscher Waage'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.WeightIndicatorAlarm, "en{'Weight indicator alarm'}de{'Gewichtsanzeige-Alarm'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.CalculationDosedWeightError, "en{'Calculation dosed weight error'}de{'Berechnung dosierter Gewichtsfehler'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.NoDosingResultAfterDosing, "en{'No dosing result after dosing'}de{'Kein Dosierergebnis nach der Dosierung'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.NoDischargedWeightAfterDischarge, "en{'No discharged weight after discharge'}de{'Kein Gewicht erfasst nach der Entleerung'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.NoStandstillWeightIndicator, "en{'No standstill weight indicator'}de{'Keine Stillstandsgewichtsanzeige'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.DosedWeightAboveRejectionLimit, "en{'Dosed weight above rejection limit'}de{'Dosiergewicht über Ablehnungsgrenze'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.DosedWeightBelowRejectionLimit, "en{'Dosed weight below rejection limit'}de{'Dosiergewicht unterhalb der Ablehnungsgrenze'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.DosedWeightAboveTolerance, "en{'Dosed weight above tolerance'}de{'Dosiertes Gewicht über Toleranz'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.DosedWeightBelowTolerance, "en{'Dosed weight below tolerance'}de{'Dosiertes Gewicht unter Toleranz'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.WeigherNo2NotEmptyAfterDischarge, "en{'Weigher no. 2 not empty after discharge'}de{'Waage Nr. 2 nach Entleerung nicht leer'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.WeigherNo1NotEmptyAfterDischarge, "en{'Weigher no. 1 not empty after discharge'}de{'Waage Nr. 1 nach Entleerung nicht leer'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.WeightSignalNo2TooFarNegativeAfterDischarge, "en{'Weight signal no. 2 too far negative after discharge'}de{'Bruttogewicht Waage 2 stark negativ nach Entleerung'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.WeightSignalNo1TooFarNegativeAfterDischarge, "en{'Weight signal no. 1 too far negative after discharge'}de{'Bruttogewicht Waage 1 stark negativ nach Entleerung'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.DosingControlModuleAlarm, "en{'Dosing control module alarm'}de{'Dosier-Steuermodul-Alarm'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.WeightIncreaseInWrongWeigherAndAboveTolerance, "en{'Weight increase in wrong weigher and above tolerance'}de{'Gewichtszunahme bei falscher Waage und Toleranz'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.WeightIncreaseInWrongWeigherAndAboveRejectionLimit, "en{'Weight increase in wrong weigher and above rejection limit'}de{'Gewichtszunahme bei falscher Waage und über Ablehnungsgrenze'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.IncorrectContainerID, "en{'Incorrect container ID'}de{'Falsche Container-ID'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.NoFlowDetectedDosedWeightAboveRejectionLimit, "en{'No flow detected - dosed weight above rejection limit'}de{'Kein Durchfluss erkannt - dosiertes Gewicht über Ablehnungsgrenze'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.NoFlowDetectedDosedWeightBelowRejectionLimit, "en{'No flow detected - dosed weight below rejection limit'}de{'Kein Durchfluss erkannt - dosiertes Gewicht unterhalb der Ablehnungsgrenze'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.NoFlowDetectedDosedWeightAboveTolerance, "en{'No flow detected - dosed weight above tolerance'}de{'Kein Durchfluss erkannt - dosiertes Gewicht über Toleranz'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.NoFlowDetectedDosedWeightBelowTolerance, "en{'No flow detected - dosed weight below tolerance'}de{'Kein Durchfluss erkannt - dosiertes Gewicht unter Toleranz'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.NoFlowDetectedDosedWeightInGeneral, "en{'No flow detected during dosing'}de{'Kein Fluss während der Dosierung erkannt'}");
                    _RB_ProcessResultAlarmCodeList.AddEntry(RB_ProcessResultAlarmCode.MaxDosingTimeReached, "en{'Max. dosing time reached'}de{'Max. Dosierzeit erreicht'}");
                }
                return _RB_ProcessResultAlarmCodeList;
            }
        }

        static ACValueItemList _ADMB_VariousEquipmentsAlarmList = null;
        [ACPropertyList(1001, "ADMB_VariousEquipmentsAlarm")]
        static public ACValueItemList ADMB_VariousEquipmentsAlarmList
        {
            get
            {
                if (_ADMB_VariousEquipmentsAlarmList == null)
                {
                    _ADMB_VariousEquipmentsAlarmList=new ACValueItemList("ADMB_VariousEquipmentsAlarmIndex");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.ConfigAlarm, "en{'ConfigAlarm'}de{'ConfigAlarm'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.None, "en{'None'}de{'None'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0001, "en{'Reserved_0001'}de{'Reserved_0001'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0002, "en{'Reserved_0002'}de{'Reserved_0002'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0003, "en{'Reserved_0003'}de{'Reserved_0003'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0004, "en{'Reserved_0004'}de{'Reserved_0004'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.InterruptedForFencingAccess, "en{'Interrupted For Fencing Access'}de{'Interrupted For Fencing Access'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0006, "en{'Reserved_0006'}de{'Reserved_0006'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0007, "en{'Reserved_0007'}de{'Reserved_0007'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0008, "en{'Reserved_0008'}de{'Reserved_0008'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0009, "en{'Reserved_0009'}de{'Reserved_0009'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0010, "en{'Reserved_0010'}de{'Reserved_0010'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.DustExhaustModuleAlarm, "en{'Dust Exhaust Module Alarm'}de{'Dust Exhaust Module Alarm'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0102, "en{'Reserved_0102'}de{'Reserved_0102'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.AirPressureControlModuleAlarm, "en{'Air Pressure Control Module Alarm'}de{'Air Pressure Control Module Alarm'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0202, "en{'Reserved_0202'}de{'Reserved_0202'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0203, "en{'Reserved_0203'}de{'Reserved_0203'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0204, "en{'Reserved_0204'}de{'Reserved_0204'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0205, "en{'Reserved_0205'}de{'Reserved_0205'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.PositioningModuleAlarm, "en{'Positioning Module Alarm'}de{'Positioning Module Alarm'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.NoReleasePositioningWeigher, "en{'No Release Positioning Weigher'}de{'No Release Positioning Weigher'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.WeigherPositionWasLost, "en{'Weigher Position Was Lost'}de{'Weigher Position Was Lost'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0401, "en{'Reserved_0401'}de{'Reserved_0401'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0402, "en{'Reserved_0402'}de{'Reserved_0402'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0403, "en{'Reserved_0403'}de{'Reserved_0403'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0404, "en{'Reserved_0404'}de{'Reserved_0404'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0405, "en{'Reserved_0405'}de{'Reserved_0405'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0406, "en{'Reserved_0406'}de{'Reserved_0406'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0407, "en{'Reserved_0407'}de{'Reserved_0407'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0408, "en{'Reserved_0408'}de{'Reserved_0408'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0409, "en{'Reserved_0409'}de{'Reserved_0409'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0410, "en{'Reserved_0410'}de{'Reserved_0410'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.VibratorModuleAlarm, "en{'Vibrator Module Alarm'}de{'Vibrator Module Alarm'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0502, "en{'Reserved_0502'}de{'Reserved_0502'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0503, "en{'Reserved_0503'}de{'Reserved_0503'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0504, "en{'Reserved_0504'}de{'Reserved_0504'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.PusherModuleAlarm, "en{'Pusher Module Alarm'}de{'Pusher Module Alarm'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0602, "en{'Reserved_0602'}de{'Reserved_0602'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0603, "en{'Reserved_0603'}de{'Reserved_0603'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0604, "en{'Reserved_0604'}de{'Reserved_0604'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.VibratorOnDischargeSealModuleAlarm, "en{'Vibrator On Discharge Seal Module Alarm'}de{'Vibrator On Discharge Seal Module Alarm'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0702, "en{'Reserved_0702'}de{'Reserved_0702'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0703, "en{'Reserved_0703'}de{'Reserved_0703'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0704, "en{'Reserved_0704'}de{'Reserved_0704'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0801, "en{'Reserved_0801'}de{'Reserved_0801'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0802, "en{'Reserved_0802'}de{'Reserved_0802'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0803, "en{'Reserved_0803'}de{'Reserved_0803'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0804, "en{'Reserved_0804'}de{'Reserved_0804'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.WeightSignalW1NotValid, "en{'Weight Signal W1 Not Valid'}de{'Weight Signal W1 Not Valid'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.WeightSignalW2NotValid, "en{'Weight Signal W2 Not Valid'}de{'Weight Signal W2 Not Valid'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.PressureSignalNotValid, "en{'Pressure Signal Not Valid'}de{'Pressure Signal Not Valid'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0808, "en{'Reserved_0808'}de{'Reserved_0808'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0809, "en{'Reserved_0809'}de{'Reserved_0809'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0810, "en{'Reserved_0810'}de{'Reserved_0810'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0901, "en{'Reserved_0901'}de{'Reserved_0901'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0902, "en{'Reserved_0902'}de{'Reserved_0902'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0903, "en{'Reserved_0903'}de{'Reserved_0903'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0904, "en{'Reserved_0904'}de{'Reserved_0904'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0905, "en{'Reserved_0905'}de{'Reserved_0905'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0906, "en{'Reserved_0906'}de{'Reserved_0906'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0907, "en{'Reserved_0907'}de{'Reserved_0907'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0908, "en{'Reserved_0908'}de{'Reserved_0908'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0909, "en{'Reserved_0909'}de{'Reserved_0909'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0910, "en{'Reserved_0910'}de{'Reserved_0910'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0911, "en{'Reserved_0911'}de{'Reserved_0911'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0912, "en{'Reserved_0912'}de{'Reserved_0912'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0913, "en{'Reserved_0913'}de{'Reserved_0913'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0914, "en{'Reserved_0914'}de{'Reserved_0914'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0915, "en{'Reserved_0915'}de{'Reserved_0915'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0916, "en{'Reserved_0916'}de{'Reserved_0916'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0917, "en{'Reserved_0917'}de{'Reserved_0917'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0918, "en{'Reserved_0918'}de{'Reserved_0918'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0919, "en{'Reserved_0919'}de{'Reserved_0919'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0920, "en{'Reserved_0920'}de{'Reserved_0920'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0921, "en{'Reserved_0921'}de{'Reserved_0921'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0922, "en{'Reserved_0922'}de{'Reserved_0922'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0923, "en{'Reserved_0923'}de{'Reserved_0923'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0924, "en{'Reserved_0924'}de{'Reserved_0924'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0925, "en{'Reserved_0925'}de{'Reserved_0925'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0926, "en{'Reserved_0926'}de{'Reserved_0926'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0927, "en{'Reserved_0927'}de{'Reserved_0927'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0928, "en{'Reserved_0928'}de{'Reserved_0928'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0929, "en{'Reserved_0929'}de{'Reserved_0929'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0930, "en{'Reserved_0930'}de{'Reserved_0930'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0931, "en{'Reserved_0931'}de{'Reserved_0931'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0932, "en{'Reserved_0932'}de{'Reserved_0932'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0933, "en{'Reserved_0933'}de{'Reserved_0933'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0934, "en{'Reserved_0934'}de{'Reserved_0934'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0935, "en{'Reserved_0935'}de{'Reserved_0935'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0936, "en{'Reserved_0936'}de{'Reserved_0936'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0937, "en{'Reserved_0937'}de{'Reserved_0937'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0938, "en{'Reserved_0938'}de{'Reserved_0938'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0939, "en{'Reserved_0939'}de{'Reserved_0939'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0940, "en{'Reserved_0940'}de{'Reserved_0940'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0941, "en{'Reserved_0941'}de{'Reserved_0941'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0942, "en{'Reserved_0942'}de{'Reserved_0942'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0943, "en{'Reserved_0943'}de{'Reserved_0943'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0944, "en{'Reserved_0944'}de{'Reserved_0944'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0945, "en{'Reserved_0945'}de{'Reserved_0945'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0946, "en{'Reserved_0946'}de{'Reserved_0946'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0947, "en{'Reserved_0947'}de{'Reserved_0947'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0948, "en{'Reserved_0948'}de{'Reserved_0948'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0949, "en{'Reserved_0949'}de{'Reserved_0949'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0950, "en{'Reserved_0950'}de{'Reserved_0950'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0951, "en{'Reserved_0951'}de{'Reserved_0951'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0952, "en{'Reserved_0952'}de{'Reserved_0952'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0953, "en{'Reserved_0953'}de{'Reserved_0953'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0954, "en{'Reserved_0954'}de{'Reserved_0954'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_0955, "en{'Reserved_0955'}de{'Reserved_0955'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_1001, "en{'Reserved_1001'}de{'Reserved_1001'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_1002, "en{'Reserved_1002'}de{'Reserved_1002'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_1003, "en{'Reserved_1003'}de{'Reserved_1003'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_1004, "en{'Reserved_1004'}de{'Reserved_1004'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_1005, "en{'Reserved_1005'}de{'Reserved_1005'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_1006, "en{'Reserved_1006'}de{'Reserved_1006'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_1007, "en{'Reserved_1007'}de{'Reserved_1007'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_1008, "en{'Reserved_1008'}de{'Reserved_1008'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_1009, "en{'Reserved_1009'}de{'Reserved_1009'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_1010, "en{'Reserved_1010'}de{'Reserved_1010'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_1101, "en{'Reserved_1101'}de{'Reserved_1101'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_1102, "en{'Reserved_1102'}de{'Reserved_1102'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_1103, "en{'Reserved_1103'}de{'Reserved_1103'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_1104, "en{'Reserved_1104'}de{'Reserved_1104'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_1105, "en{'Reserved_1105'}de{'Reserved_1105'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_1106, "en{'Reserved_1106'}de{'Reserved_1106'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_1107, "en{'Reserved_1107'}de{'Reserved_1107'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_1108, "en{'Reserved_1108'}de{'Reserved_1108'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_1109, "en{'Reserved_1109'}de{'Reserved_1109'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_1110, "en{'Reserved_1110'}de{'Reserved_1110'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_1201, "en{'Reserved_1201'}de{'Reserved_1201'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_1202, "en{'Reserved_1202'}de{'Reserved_1202'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_1203, "en{'Reserved_1203'}de{'Reserved_1203'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_1204, "en{'Reserved_1204'}de{'Reserved_1204'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_1205, "en{'Reserved_1205'}de{'Reserved_1205'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_1206, "en{'Reserved_1206'}de{'Reserved_1206'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_1207, "en{'Reserved_1207'}de{'Reserved_1207'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_1208, "en{'Reserved_1208'}de{'Reserved_1208'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_1209, "en{'Reserved_1209'}de{'Reserved_1209'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_1210, "en{'Reserved_1210'}de{'Reserved_1210'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.DischargeW2ModuleAlarm, "en{'Discharge W2 Module Alarm'}de{'Discharge W2 Module Alarm'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_1302, "en{'Reserved_1302'}de{'Reserved_1302'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.DischargeW2ModuleAlarm2, "en{'Discharge W2 Module Alarm 2'}de{'Discharge W2 Module Alarm 2'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_1402, "en{'Reserved_1402'}de{'Reserved_1402'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_1403, "en{'Reserved_1403'}de{'Reserved_1403'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.W2NotEmpty, "en{'W2 Not Empty'}de{'W2 Not Empty'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.W2NotEmptyAfterCheck, "en{'W2 Not Empty After Check'}de{'W2 Not Empty After Check'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.KnockerW2ModuleAlarm, "en{'Knocker W2 Module Alarm'}de{'Knocker W2 Module Alarm'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_1502, "en{'Reserved_1502'}de{'Reserved_1502'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_1503, "en{'Reserved_1503'}de{'Reserved_1503'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_1504, "en{'Reserved_1504'}de{'Reserved_1504'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.TopDockingModuleAlarm, "en{'Top Docking Module Alarm'}de{'Top Docking Module Alarm'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_1602, "en{'Reserved_1602'}de{'Reserved_1602'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_1603, "en{'Reserved_1603'}de{'Reserved_1603'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.Reserved_1604, "en{'Reserved_1604'}de{'Reserved_1604'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.PositioningModuleAlarm_B, "en{'Positioning Module Alarm'}de{'Positioning Module Alarm'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.UnexpectedStopPositioningModule, "en{'Unexpected Stop Positioning Module'}de{'Unexpected Stop Positioning Module'}");
                    _ADMB_VariousEquipmentsAlarmList.AddEntry(ADMB_VariousEquipmentsAlarm.WeigherPositionWasLost_B, "en{'Weigher Position Was Lost'}de{'Weigher Position Was Lost'}");
                }
                return _ADMB_VariousEquipmentsAlarmList;
            }
        }

    }
}

