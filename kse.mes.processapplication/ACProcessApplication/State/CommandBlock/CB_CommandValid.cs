using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{

    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'CB_CommandValid'}de{'CB_CommandValid'}", Global.ACKinds.TACEnum)]
    public enum CB_CommandValid: ushort
    {
        NotValid = 0,
        Valid = 1
    }
}
