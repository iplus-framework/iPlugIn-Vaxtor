using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'ContServiceAlarmCode'}de{'ContServiceAlarmCode'}", Global.ACKinds.TACEnum)]
    public enum ContServiceAlarmCode : ushort
    {
      KeinAlarm = 0,
      WerkzeugAlarm = 1,
      KeinConatinerAufServicePositionDetektiert = 2,
      MehrereConatinerAufServicePositionDetektiert = 3,
      GewünschteAktionUnbekanntx = 4,
      ReinigenAufDieserPositionNichtMöglich = 5,
      ConatinerNichtReinNachReinigungsZyklus = 6,
      GewünschteAktionUnbekannt = 9,
      ActStatusOutOfSync = 10
    }
}
