using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'CRB_CommandResponseCode'}de{'CRB_CommandResponseCode'}", Global.ACKinds.TACEnum)]
    public enum CRB_CommandResponseCode : ushort
    {
        CommandAccepted = 0,
        CommandCodeOrNumberNotValid = 1,
        BinNumberNotValid = 2,
        DischargePositionNotValid = 3,
        WeigherNumberNotValid = 4,
        CheckTareNotValid = 5,
        Reserved = 6,
        PositioningCommandNotAvailable = 7,
        PositionDestinationNotValid = 8,
        IncorrectBatchid = 9,
        SystemIsBusy = 20,
        SystemIsInManualMode = 21
    }
}
