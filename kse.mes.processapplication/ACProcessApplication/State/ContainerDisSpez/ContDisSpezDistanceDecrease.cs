using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'ContDisSpezDistanceDecrease'}de{'ContDisSpezDistanceDecrease'}", Global.ACKinds.TACEnum)]
    public enum ContDisSpezDistanceDecrease : ushort
    {
        NotActive = 0,
        DecreaseDistance = 1
    }
}
