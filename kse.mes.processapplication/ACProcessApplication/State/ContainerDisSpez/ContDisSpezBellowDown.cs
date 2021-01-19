using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'ContDisSpezBellowDown'}de{'ContDisSpezBellowDown'}", Global.ACKinds.TACEnum)]
    public enum ContDisSpezBellowDown : ushort
    {
        StopNotActive = 0,
        ControlBellowDown = 1
    }
}
