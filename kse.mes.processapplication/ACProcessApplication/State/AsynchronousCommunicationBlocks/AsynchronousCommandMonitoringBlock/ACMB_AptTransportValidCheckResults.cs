using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'ACMB_AptTransportValidCheckResults'}de{'ACMB_AptTransportValidCheckResults'}", Global.ACKinds.TACEnum)]
    public enum ACMB_AptTransportValidCheckResults : ushort
    {
        NotChecked = 0,
        CheckedOk = 1
    }
}
