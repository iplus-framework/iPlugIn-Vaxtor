using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'ContTranspAction'}de{'ContTranspAction'}", Global.ACKinds.TACEnum)]
    public enum ContTranspAction : ushort
    {
        NotDefined = 0,
        Positionieren = 1,
        ÄndernAktuellerPosition = 2
    }
}
