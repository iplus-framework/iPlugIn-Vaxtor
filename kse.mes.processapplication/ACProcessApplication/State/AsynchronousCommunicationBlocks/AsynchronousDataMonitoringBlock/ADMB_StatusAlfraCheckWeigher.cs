using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'ADMB_StatusAlfraCheckWeigher'}de{'ADMB_StatusAlfraCheckWeigher'}", Global.ACKinds.TACEnum)]
    public enum ADMB_StatusAlfraCheckWeigher
    {
        Idle = 0,
        Busy = 1,
        Alarm = 2
    }
}
