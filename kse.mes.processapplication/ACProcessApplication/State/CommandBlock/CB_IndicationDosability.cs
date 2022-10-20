using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'CB_IndicationDosability'}de{'CB_IndicationDosability'}", Global.ACKinds.TACEnum)]
    public enum CB_IndicationDosability : ushort
    {
        UseLocalValue = 0,
        Easy = 1,
        Easy2 = 2,
        Easy3 = 3,
        Easy4 = 4,
        Normal = 5,
        Normal1 = 6,
        Normal2 = 7,
        Normal3 = 8,
        Normal4 = 9,
        Difficult = 10,
    }
}
