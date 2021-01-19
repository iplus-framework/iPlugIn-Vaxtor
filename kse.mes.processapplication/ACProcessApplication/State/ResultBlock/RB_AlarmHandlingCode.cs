using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'RB_AlarmHandlingCode'}de{'RB_AlarmHandlingCode'}", Global.ACKinds.TACEnum)]
    public enum RB_AlarmHandlingCode : ushort
    {
        NotDefined = 0,
        AlarmRejected = 1,
        AlarmApproved = 2,
        AlarmRestarted = 3,
        RequestNewCellNotEmpty = 4,
        RequestNewCellEmpty = 5
    }
}
