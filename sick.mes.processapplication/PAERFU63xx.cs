using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.processapplication;
using gip.mes.processapplication;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace sick.mes.processapplication
{
    //[ACSerializeableInfo(new Type[] { typeof(ACPropertyValueEvent<string[]>) })]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Sick RFID 63xx}de{'Sick RFID 63xx'}", Global.ACKinds.TPAModule, Global.ACStorableTypes.Required, false, true)]
    public class PAERFU63xx : PAERFU6xxxBase
    {
        #region c'tors
        static PAERFU63xx()
        {
            RegisterExecuteHandler(typeof(PAERFU63xx), HandleExecuteACMethod_PAERFU63xx);
        }


        public PAERFU63xx(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") :
               base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        #endregion

        #region Helper-Methods
        public static bool HandleExecuteACMethod_PAERFU63xx(out object result, IACComponent acComponent, string acMethodName, ACClassMethod acClassMethod, object[] acParameter)
        {
            return HandleExecuteACMethod_PAERFU6xxxBase(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }
        #endregion
    }
}
