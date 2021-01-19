using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'ADMB_DustExtractionEquipmentAlarm'}de{'ADMB_DustExtractionEquipmentAlarm'}", Global.ACKinds.TACEnum)]
    public enum ADMB_DustExtractionEquipmentAlarm: ushort
    {
        // In case of equipment alarm more information can be found in the asynchronous alarm monitoring block
        NoAlarm = 0,
        EquipmentHardwareAlarm = 1,
        ConfigurationAlarm = 2,
        Reserve = 3,
        AlarmFilter = 4
    }
}
