using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'ContTranspRelease'}de{'ContTranspRelease'}", Global.ACKinds.TACEnum)]
    public enum ContTranspRelease : ushort
    {
      NichtFreigegeben = 0,
      Freigegeben = 1
    }
}
