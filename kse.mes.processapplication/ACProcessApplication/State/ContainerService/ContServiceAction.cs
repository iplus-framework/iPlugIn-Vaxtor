using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'ContServiceAction'}de{'ContServiceAction'}", Global.ACKinds.TACEnum)]
    public enum ContServiceAction : ushort
    {
        NotDefined = 0,
        Clean = 1
    }
}
