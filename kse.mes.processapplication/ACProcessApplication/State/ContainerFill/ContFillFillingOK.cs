using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'FillingOK'}de{'FillingOK'}", Global.ACKinds.TACEnum)]
    public enum FillingOK : ushort
    {
        Nein = 0,
        Ja = 1
    }
}
