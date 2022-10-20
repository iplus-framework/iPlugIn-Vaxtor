using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'ACMB_RequestedWeightReached'}de{'ACMB_RequestedWeightReached'}", Global.ACKinds.TACEnum)]
    public enum ACMB_RequestedWeightReached : ushort
    {
        RequestedWeightNotReached = 0,
        RequestedWeightReached = 1
    }
}
