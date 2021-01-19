using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'RB_ResultCode'}de{'RB_ResultCode'}", Global.ACKinds.TACEnum)]
    public enum RB_ResultCode : ushort
    {
        NotDefined = 0,
        Dose = 1,
        Discharge = 2,
        Position = 3,
        ResetOdr = 4
    }
}
