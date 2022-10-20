using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'ACMB_RequestDischargeTransport'}de{'ACMB_RequestDischargeTransport'}", Global.ACKinds.TACEnum)]
    public enum ACMB_RequestDischargeTransport : ushort
    {
        NoRequest = 0,
        DischargeRequest = 1
    }
}
