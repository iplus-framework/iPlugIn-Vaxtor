using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'ContDisDustDischarging'}de{'ContDisDustDischarging'}", Global.ACKinds.TACEnum)]
    public enum ContDisDustDischarging : ushort
    {
        NotActive = 0,
        DustDischargingActive = 1
    }
}
