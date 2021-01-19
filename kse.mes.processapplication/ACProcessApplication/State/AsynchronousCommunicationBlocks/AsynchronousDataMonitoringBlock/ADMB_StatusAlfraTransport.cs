using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'ADMB_StatusAlfraTransport'}de{'ADMB_StatusAlfraTransport'}", Global.ACKinds.TACEnum)]
    public enum ADMB_StatusAlfraTransport : ushort
    {
        Idle = 0,
        Busy = 1,
        Alarm = 2
    }
}
