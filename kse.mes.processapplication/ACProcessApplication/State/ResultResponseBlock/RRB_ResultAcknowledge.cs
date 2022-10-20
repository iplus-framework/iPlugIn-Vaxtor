using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'RRB_ResultAcknowledge'}de{'RRB_ResultAcknowledge'}", Global.ACKinds.TACEnum)]
    public enum RRB_ResultAcknowledge: ushort
    {
        ResultNotAcknowledge = 0,
        Acknowledge = 1
    }
}
