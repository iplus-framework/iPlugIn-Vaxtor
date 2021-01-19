using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'CB_DosingMode'}de{'CB_DosingMode'}", Global.ACKinds.TACEnum)]
    public enum CB_DosingMode: ushort
    {
        NotDefined = 0,
        ODRDosing = 1,
        DoseToEmpty = 2
    }
}
