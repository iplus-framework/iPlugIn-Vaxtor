using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'CB_CommandOperationMode'}de{'CB_CommandOperationMode'}", Global.ACKinds.TACEnum)]
    public enum CB_CommandOperationMode : ushort
    {
        NotDefined = 0,
        Dose = 1,
        Discharge = 2,
        Position = 3,
        Reset =     4
    }
}
