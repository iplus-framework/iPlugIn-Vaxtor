using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'ContTranspAlarmCode'}de{'ContTranspAlarmCode'}", Global.ACKinds.TACEnum)]
    public enum ContTranspAlarmCode : ushort
    {
        KeinAlarm = 0,
        WerkzeugAlarm = 1,
        GewünschtePositionUnbekannt = 2,
        GewünschteAktionUnbekanntx = 3,
        KommunikationsfehlerMitActDatenbank = 4,
        RouteNichtMöglich = 5,
        ContainerNichtImActSystem = 6,
        ContainerAufUnbekannterPosition = 7,
        NullstellenDerWaageNichtGelungen = 8,
        GewünschteAktionUnbekannt = 9,
        ActStatusOutOfSync = 10
    }
}
