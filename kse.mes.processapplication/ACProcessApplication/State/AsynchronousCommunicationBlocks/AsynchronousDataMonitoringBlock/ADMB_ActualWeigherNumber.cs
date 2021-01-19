using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'ADMB_ActualWeigherNumber'}de{'ADMB_ActualWeigherNumber'}", Global.ACKinds.TACEnum)]
    public enum ADMB_ActualWeigherNumber
    {
        NotDefined = 0,
        LargeScale = 1,
        SmallScale = 2
    }
}
    