using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'ContDisRequestPusher'}de{'ContDisRequestPusher'}", Global.ACKinds.TACEnum)]
    public enum ContDisRequestPusher : ushort
    {
        Nein = 0,
        Ja = 1
    }
}
