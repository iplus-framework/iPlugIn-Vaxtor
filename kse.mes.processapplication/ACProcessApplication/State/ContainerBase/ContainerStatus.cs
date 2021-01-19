using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'ContainerStatus'}de{'ContainerStatus'}", Global.ACKinds.TACEnum)]
    public enum ContainerStatus : ushort
    {
       NotDefined = 0,
       Idle = 1,
       Complete = 2,
       Holding = 3,
       Held = 4,
       Restarting = 5,
       Stopping = 6,
       Stopped = 7, 
       Aborting = 8,
       Aborted = 9,
       Running = 10,
       Resetting = 11
    }
}
