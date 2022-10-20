using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'ContDisAction'}de{'ContDisAction'}", Global.ACKinds.TACEnum)]
    public enum ContDisAction : ushort
    {
        NotDefined = 0,
        ExternerStopEntleerung = 1,
        AutomatischerStopEntleerung = 2
    }
}
