using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'ContServiceInternalOrderActive'}de{'ContServiceInternalOrderActive'}", Global.ACKinds.TACEnum)]
    public enum ContServiceInternalOrderActive : ushort
    {
        Nein = 0,
        Ja = 1
    }
}
