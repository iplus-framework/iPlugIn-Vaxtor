using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.core.autocomponent;
using System.ComponentModel;
using kse.mes.processapplication;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'KSE Func Base'}de{'KSE Cont. Func base'}", Global.ACKinds.TACAbstractClass, Global.ACStorableTypes.Required, false, "", true)]
    public abstract class PAProcessFuncKSE : PAProcessFunction
    {
        static PAProcessFuncKSE()
        {
            RegisterExecuteHandler(typeof(PAProcessFuncKSE), HandleExecuteACMethod_PAProcessFuncKSE);
        }

        public PAProcessFuncKSE(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public static bool HandleExecuteACMethod_PAProcessFuncKSE(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PAProcessFunction(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }

        protected DateTime? _WaitForKSEData = null;
        public override void SMCompleted()
        {
            if (!_WaitForKSEData.HasValue)
            {
                _WaitForKSEData = DateTime.Now.AddSeconds(1);
                SubscribeToProjectWorkCycle();
                return;
            }
            else if (_WaitForKSEData.Value > DateTime.Now)
                return;
            _WaitForKSEData = null;
            UnSubscribeToProjectWorkCycle();
            base.SMCompleted();
        }

        public override void SMAborted()
        {
            if (!_WaitForKSEData.HasValue)
            {
                _WaitForKSEData = DateTime.Now.AddSeconds(1);
                SubscribeToProjectWorkCycle();
                return;
            }
            else if (_WaitForKSEData.Value > DateTime.Now)
                return;
            _WaitForKSEData = null;
            UnSubscribeToProjectWorkCycle();
            base.SMAborted();
        }

        public override void SMStopped()
        {
            if (!_WaitForKSEData.HasValue)
            {
                _WaitForKSEData = DateTime.Now.AddSeconds(1);
                SubscribeToProjectWorkCycle();
                return;
            }
            else if (_WaitForKSEData.Value > DateTime.Now)
                return;
            _WaitForKSEData = null;
            UnSubscribeToProjectWorkCycle();
            base.SMStopped();
        }
    }

}
