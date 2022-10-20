using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'ContDisDischargeNotReleased'}de{'ContDisDischargeNotReleased'}", Global.ACKinds.TACEnum)]
    public enum ContDisDischargeNotReleased : ushort
    {
        Ok = 0,
        KeineFreigabeEntleerung = 1
    }
}
