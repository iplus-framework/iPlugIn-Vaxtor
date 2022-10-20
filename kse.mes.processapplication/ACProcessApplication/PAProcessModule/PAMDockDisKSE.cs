using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.processapplication;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'KSE Dischargingstation'}de{'KSE Entleerstation'}", Global.ACKinds.TPAProcessModule, Global.ACStorableTypes.Required, false, PWGroupVB.PWClassName, true)]
    public class PAMDockDisKSE : PAMDockBaseKSE
    {
        #region ctor's
        static PAMDockDisKSE()
        {
            RegisterExecuteHandler(typeof(PAMDockDisKSE), HandleExecuteACMethod_PAMDockDisKSE);
        }

        public PAMDockDisKSE(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _IsFinalDest = new ACPropertyConfigValue<bool>(this, "IsFinalDest", false);
        }
        #endregion

        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PAMDockDisKSE(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PAMDockBaseKSE(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion

        #region Config
        private ACPropertyConfigValue<bool> _IsFinalDest;
        [ACPropertyConfig("en{'Is final Destination'}de{'Ist engültiges Ziel'}")]
        public virtual bool IsFinalDest
        {
            get
            {
                return _IsFinalDest.ValueT;
            }
        }
        #endregion

    }
}
