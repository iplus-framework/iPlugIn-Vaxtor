using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.processapplication;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'KSE Fillingstation'}de{'KSE Befüllstation'}", Global.ACKinds.TPAProcessModule, Global.ACStorableTypes.Required, false, PWGroupVB.PWClassName, true)]
    public class PAMDockFillKSE : PAMDockBaseKSE
    {
        #region ctor's
        static PAMDockFillKSE()
        {
            RegisterExecuteHandler(typeof(PAMDockFillKSE), HandleExecuteACMethod_PAMDockFillKSE);
        }

        public PAMDockFillKSE(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }
        #endregion

        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PAMDockFillKSE(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PAMDockBaseKSE(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion

    }
}
