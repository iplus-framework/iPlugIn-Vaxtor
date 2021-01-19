using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'ContDisSpezBellowUp'}de{'ContDisSpezBellowUp'}", Global.ACKinds.TACEnum)]
    public enum ContDisSpezBellowUp : ushort
    {
        StopNotActive = 0,
        ControlBellowUp = 1
    }
}
