using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'CRB_CommandAcknowledge'}de{'CRB_CommandAcknowledge'}", Global.ACKinds.TACEnum)]
    public enum CRB_CommandAcknowledge : ushort
    {
        CommandNotAcknowledged = 0,
        Acknowledge = 1
    }
}
