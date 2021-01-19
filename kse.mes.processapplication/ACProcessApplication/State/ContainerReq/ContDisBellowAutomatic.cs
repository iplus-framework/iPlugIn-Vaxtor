using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'ContDisBellowAutomatic'}de{'ContDisBellowAutomatic'}", Global.ACKinds.TACEnum)]
    public enum ContDisBellowAutomatic : ushort
    {
        BellowControlledManually = 0,
        BellowControlledAutomatic = 1
    }
}
