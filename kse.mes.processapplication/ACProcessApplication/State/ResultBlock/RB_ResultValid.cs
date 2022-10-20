using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{

    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'RB_ResultValid'}de{'RB_ResultValid'}", Global.ACKinds.TACEnum)]
    public enum RB_ResultValid: ushort
    {
        NotValid = 0,
        Valid = 1
    }
}
