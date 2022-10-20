using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'ContainerCommand'}de{'ContainerCommand'}", Global.ACKinds.TACEnum)]
    public enum ContainerCommand : ushort
    {
        NotDefined = 0,
        Start = 1 ,
        Stop = 2,
        Hold = 3,
        Restart = 4, 
        Abort = 5,
        Reset = 6
    }
}
