using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'CB_BatchEnd'}de{'CB_BatchEnd'}", Global.ACKinds.TACEnum)]
    public enum CB_BatchEnd : ushort
    {
//        0 = No batch end
//(dosing line or discharge in between)
//1 = Batch End
//(last dosing of batch or discharge of batch)
        NoBatchEnd = 0 ,
        BatchEnd = 1
    }
}
