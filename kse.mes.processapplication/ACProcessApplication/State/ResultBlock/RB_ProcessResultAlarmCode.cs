using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'RB_ProcessResultAlarmCode'}de{'RB_ProcessResultAlarmCode'}", Global.ACKinds.TACEnum)]
    public enum RB_ProcessResultAlarmCode : ushort
    {
        None = 0,

        /// <summary>
        /// (internal Alarm from KSE when started via KSE-SCADA in Development; should not appear)
        /// </summary>
        DosingAborted = 1,

        /// <summary>
        /// DosingForcedStopped is a result whne operater stops manually a dosing. 
        /// This Alarm doesn't appear at Discharging
        /// </summary>
        DosingForcedStopped = 2,

        /// <summary>
        /// (internal Alarm from KSE when started via KSE-SCADA; should not appear)
        /// </summary>
        DosingModeConfigurationError = 3,

        /// <summary>
        /// (internal Alarm from KSE when started via KSE-SCADA; should not appear)
        /// </summary>
        SoftwareConfigurationError = 4,

        /// <summary>
        /// InputParametersError appears when wrong dosing-parameters are sended to the scale an are out of range.
        /// </summary>
        InputParametersError = 5,

        /// <summary>
        /// Dose Weigher No.2 Not Empty Before Dosing
        /// </summary>
        DoseWeigherNo2NotEmptyBeforeDosing = 6,

        /// <summary>
        /// Dose Weigher No.1 Not Empty Before Dosing
        /// </summary>
        DoseWeigherNo1NotEmptyBeforeDosing = 7,

        /// <summary>
        /// Weigher No 2 Too Far Negative (Waage zeigt negativen Wert an)
        /// </summary>
        WeigherNo2TooFarNegative = 8,

        /// <summary>
        /// Weigher No 1 Too Far Negative (Waage zeigt negativen Wert an)
        /// </summary>
        WeigherNo1TooFarNegative = 9,

        /// <summary>
        /// Weigher No 2 Overload Expected.
        /// (Wenn die zu dosierend Sollmenge größer als die aktuelle Restmenge in der Waage ist. Dosierung wird auch nicht ausgeführt)
        /// </summary>
        WeigherNo2OverloadExpected = 10,

        /// <summary>
        /// Weigher No 1 Overload Expected
        /// Wenn die zu dosierend Sollmenge größer als die aktuelle Restmenge in der Waage ist. 
        /// Passiert dann, wenn alle Dosierungen Übertoleranz hatten und die aktuelle Restmenge nicht ausreicht um die letzte Komponente zu dosieren.
        /// </summary>
        WeigherNo1OverloadExpected = 11,

        /// <summary>
        /// Weigher No 2 Overflow Expected, Max.-Volumen würde überschritten werden
        /// </summary>
        WeigherNo2OverflowExpected = 12,

        /// <summary>
        /// Weigher No 1 Overflow Expected, Max.-Volumen würde überschritten werden
        /// </summary>
        WeigherNo1OverflowExpected = 13,

        /// <summary>
        /// No standstill in Weight-Indicator before dosing
        /// </summary>
        NoStandstillWeightIndicatorBeforeDosing = 14,

        /// <summary>
        /// Software release of dosing disturbed before dosing
        /// </summary>
        SoftwareReleaseOfDosingDisturbedBeforeDosing = 15,

        /// <summary>
        /// Weight indicator alarm at start dosing
        /// </summary>
        WeightIndicatorAlarmAtStartDosing = 16,

        /// <summary>
        /// Hardware alarm before dosing
        /// </summary>
        HardwareAlarmBeforeDosing = 17,

        /// <summary>
        /// Wrong slide open (Only Dosing)
        /// </summary>
        WrongSlideOpen = 18,

        /// <summary>
        /// Weight increase in wrong weigher (Only Dosing)
        /// </summary>
        WeightIncreaseInWrongWeigher = 19,

        /// <summary>
        /// Weight indicator alarm (Only Dosing)
        /// </summary>
        WeightIndicatorAlarm = 20,

        /// <summary>
        /// Calculation dosed weight error (Only Dosing)
        /// </summary>
        CalculationDosedWeightError = 21,

        /// <summary>
        /// No dosing result after dosing (Only Dosing)
        /// </summary>
        NoDosingResultAfterDosing = 22,

        /// <summary>
        /// No discharged weight after discharge (Only Discharging)
        /// </summary>
        NoDischargedWeightAfterDischarge = 23,

        /// <summary>
        /// No Standstill Weight Indicator (internal Alarm from KSE; should not appear)
        /// </summary>
        NoStandstillWeightIndicator = 24,

        /// <summary>
        /// Dosed weight above rejection limit (internal Alarm from KSE; should not appear)
        /// </summary>
        DosedWeightAboveRejectionLimit = 25,

        /// <summary>
        /// Dosed weight above rejection limit (internal Alarm from KSE; should not appear)
        /// </summary>
        DosedWeightBelowRejectionLimit = 26,

        /// <summary>
        /// Dosed weight above tolerance (only Dosing)
        /// </summary>
        DosedWeightAboveTolerance = 27,

        /// <summary>
        /// Dosed weight below tolerance (only Dosing). Difference to AlarmCode 55: Technical Problem.
        /// </summary>
        DosedWeightBelowTolerance = 28,

        /// <summary>
        /// Weigher No. 2 not empty after discharge (only Discharging)
        /// </summary>
        WeigherNo2NotEmptyAfterDischarge = 29,

        /// <summary>
        /// Weigher No. 1 not empty after discharge (only Discharging)
        /// </summary>
        WeigherNo1NotEmptyAfterDischarge = 30,

        /// <summary>
        /// Weight signal No. 2 too far negative after discharge
        /// </summary>
        WeightSignalNo2TooFarNegativeAfterDischarge = 31,

        /// <summary>
        /// Weight signal No. 1 too far negative after discharge
        /// </summary>
        WeightSignalNo1TooFarNegativeAfterDischarge = 32,

        /// <summary>
        /// Dosing control-module alarm (only Dosing). Difference to AlarmCode 55: Technical Problem.
        /// </summary>
        DosingControlModuleAlarm = 33,

        /// <summary>
        /// Dosed weight above tolerance (internal Alarm from KSE; should not appear)
        /// </summary>
        WeightIncreaseInWrongWeigherAndAboveTolerance = 34,

        /// <summary>
        /// Dosed weight above tolerance (internal Alarm from KSE; should not appear)
        /// </summary>
        WeightIncreaseInWrongWeigherAndAboveRejectionLimit = 35,

        /// <summary>
        /// Incorrect Container-ID (internal Alarm from KSE; should not appear)
        /// </summary>
        IncorrectContainerID = 36,

        /// <summary>
        ///  (internal Alarm from KSE; should not appear)
        /// </summary>
        NoFlowDetectedDosedWeightAboveRejectionLimit = 51,

        /// <summary>
        /// (internal Alarm from KSE; should not appear)
        /// </summary>
        NoFlowDetectedDosedWeightBelowRejectionLimit = 52,

        /// <summary>
        /// (internal Alarm from KSE; should not appear)
        /// </summary>
        NoFlowDetectedDosedWeightAboveTolerance = 53,

        /// <summary>
        /// (internal Alarm from KSE; should not appear)
        /// </summary>
        NoFlowDetectedDosedWeightBelowTolerance = 54,

        /// <summary>
        /// Lack of Material
        /// </summary>
        NoFlowDetectedDosedWeightInGeneral = 55,

        /// <summary>
        /// Max dosing time reached
        /// </summary>
        MaxDosingTimeReached = 56
    }
}
