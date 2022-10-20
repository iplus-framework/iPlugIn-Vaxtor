using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'ADMB_ProcessOperation'}de{'ADMB_ProcessOperation'}", Global.ACKinds.TACEnum)]
    public enum ADMB_ProcessOperation : ushort
    {
        Idle = 0,
        Dosing = 1,
        Discharging = 2,
        Positioning = 3,
        ResettingOdr = 4
    }
}
