using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'CB_WeigherNumber'}de{'CB_WeigherNumber'}", Global.ACKinds.TACEnum)]
    public enum CB_WeigherNumber : ushort
    {
        // (Choice made on mass density (if provided) or requested weight)
        Automatic = 0,
        LargeScale = 1, 
        SmallScale=  2
    }
}
