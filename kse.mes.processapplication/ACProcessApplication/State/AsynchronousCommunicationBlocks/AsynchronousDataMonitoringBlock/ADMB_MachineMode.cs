using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'ADMB_MachineMode'}de{'ADMB_MachineMode'}", Global.ACKinds.TACEnum)]
    public enum ADMB_MachineMode: ushort
    {
        NotDefined = 0,
        ManualMode = 1,
        AutoMode = 2
    }
}
