using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'ContFillAlarmCode'}de{'ContFillAlarmCode'}", Global.ACKinds.TACEnum)]
    public enum ContFillAlarmCode : ushort
    {
        KeinAlarm = 0,
        Werkzeugalarm = 1,
        KeinContainerAufFüllpositionDetektiert = 2,
        MehrereContainerAufFüllpositionDetektiert = 3,
        NullstellenDerWaageNichtGelungen = 8,
        ActStatusOutOfSunc = 10
    }
}
