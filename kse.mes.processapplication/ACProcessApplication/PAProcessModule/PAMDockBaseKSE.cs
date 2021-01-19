using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.processapplication;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'PAMDockBaseKSE'}de{'PAMDockBaseKSE'}", Global.ACKinds.TPAProcessModule, Global.ACStorableTypes.Required, false, PWGroupVB.PWClassName, true)]
    public abstract class PAMDockBaseKSE : PAMParkingspace
    {
        #region ctor's
        static PAMDockBaseKSE()
        {
            RegisterExecuteHandler(typeof(PAMDockBaseKSE), HandleExecuteACMethod_PAMDockBaseKSE);
        }

        public PAMDockBaseKSE(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }
        #endregion

        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PAMDockBaseKSE(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PAMParkingspace(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion

    }
}
