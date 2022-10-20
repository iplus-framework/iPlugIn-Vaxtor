using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.processapplication;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Containerinterf. Processmodule'}de{'Containerschnitt. Prozessmodul'}", Global.ACKinds.TPAProcessModule, Global.ACStorableTypes.Required, false, PWContGroupKSE.PWClassName, true)]
    public class PAMContainerKSE : PAProcessModuleVB
    {
        #region ctor's
        static PAMContainerKSE()
        {
            RegisterExecuteHandler(typeof(PAMContainerKSE), HandleExecuteACMethod_PAMContainerKSE);
        }

        public PAMContainerKSE(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _ContEmtpyWeight = new ACPropertyConfigValue<double>(this, "ContEmtpyWeight", 0);
        }
        #endregion

        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PAMContainerKSE(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PAProcessModuleVB(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion

        #region Config
        protected ACPropertyConfigValue<double> _ContEmtpyWeight = null;
        [ACPropertyConfig("en{'Empty weight'}de{'Leergewicht'}")]
        public virtual double ContEmtpyWeight
        {
            get
            {
                return _ContEmtpyWeight.ValueT;
            }
            set
            {
                _ContEmtpyWeight.ValueT = value;
            }
        }


        [ACPropertyInfo(false, 201, "Configuration", "en{'Empty weight'}de{'Leergewicht'}", "", true)]
        public double ContEmtpyWeightView
        {
            get
            {
                return _ContEmtpyWeight.ValueT;
            }

            set
            {
                _ContEmtpyWeight.ValueT = value;
            }
        }

        #endregion

    }
}
