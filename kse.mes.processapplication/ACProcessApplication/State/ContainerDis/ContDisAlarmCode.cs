using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'ContDisAlarmCode'}de{'ContDisAlarmCode'}", Global.ACKinds.TACEnum)]
    public enum ContDisAlarmCode : ushort
    {
        KeinAlarm = 0,
        Werkzeugalarm = 1,
        KeinContainerAufEntleerpositionDetektiert = 2,
        MehrereContainerAufEntleerpositionDetektiert = 3,
        KeineZunahmeAbnahme = 4,
        KeinStillstand = 5,
        ConatinerNichtLeerNachSchliessenEntleerklappe = 6,
        DsmAlarm = 7,
        NullstellenDerWaageNichtGelungen = 8,
        GewünschteAktionUnbekannt = 9,
        ActStatusOutOfSunc = 10
    }
}
