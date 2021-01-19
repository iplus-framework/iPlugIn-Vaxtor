using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'ContDisSpezDistanceIncrease'}de{'ContDisSpezDistanceIncrease'}", Global.ACKinds.TACEnum)]
    public enum ContDisSpezDistanceIncrease : ushort
    {
        NotActive = 0,
        IncreaseDistance = 1
    }
}
