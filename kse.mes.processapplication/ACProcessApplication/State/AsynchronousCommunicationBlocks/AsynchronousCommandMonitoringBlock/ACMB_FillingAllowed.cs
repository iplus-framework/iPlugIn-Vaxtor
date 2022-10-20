using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'ACMB_FillingAllowed'}de{'ACMB_FillingAllowed'}", Global.ACKinds.TACEnum)]
    public enum ACMB_FillingAllowed : ushort
    {
        FillingNotAllowed = 0,
        FillingAllowed = 1
    }
}
