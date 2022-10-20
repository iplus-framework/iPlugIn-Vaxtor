using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.processapplication;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Containerrequest Processmodule'}de{'Containeranforderung Prozessmodul'}", Global.ACKinds.TPAProcessModule, Global.ACStorableTypes.Required, false, PWGroupVB.PWClassName, true)]
    public class PAMContReqKSE : PAProcessModuleVB
    {
        #region ctor's
        static PAMContReqKSE()
        {
            RegisterExecuteHandler(typeof(PAMContReqKSE), HandleExecuteACMethod_PAMContReqKSE);
        }

        public PAMContReqKSE(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }
        #endregion

        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PAMContReqKSE(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PAProcessModuleVB(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion

    }
}
