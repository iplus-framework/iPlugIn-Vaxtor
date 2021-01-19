using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'ContTranspWeighingPos'}de{'ContTranspWeighingPos'}", Global.ACKinds.TACEnum)]
    public enum ContTranspWeighingPos : ushort
    {
        Nein = 0,
        Ja = 1
    }
}
