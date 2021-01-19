using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'ContReqAlarmCode'}de{'ContReqAlarmCode'}", Global.ACKinds.TACEnum)]
    public enum ContReqAlarmCode : ushort
    {
      NoAlarm = 0,
      KeinContainerVerfügbar = 1,
      RequiredPositionIstUnbekannt = 2,
      ActStatusOutOfSync = 10
    }
}
