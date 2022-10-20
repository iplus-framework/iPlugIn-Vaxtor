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

namespace soehnle.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Scale Terminal 3720'}de{'Waage Terminal 3720'}", Global.ACKinds.TPAModule, Global.ACStorableTypes.Required, false, true)]
    public class PAETerminal3720 : PAETerminal3xxxBase
    {
        #region c'tors
        static PAETerminal3720()
        {
            RegisterExecuteHandler(typeof(PAETerminal3720), HandleExecuteACMethod_PAETerminal3720);
        }


        public PAETerminal3720(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") :
               base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACPostInit()
        {
            return base.ACPostInit();
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            return base.ACDeInit(deleteACClassTask);
        }

        public new const string ClassName = "PAETerminal3720";

        #endregion

        #region Override
        protected Comm3xxxBase _Communicator;
        protected override Comm3xxxBase Communicator
        {
            get
            {
                if (_Communicator == null)
                    _Communicator = new Comm3xxxBase();
                return _Communicator;
            }
        }
        #endregion

        #region Helper-Methods
        public static bool HandleExecuteACMethod_PAETerminal3720(out object result, IACComponent acComponent, string acMethodName, ACClassMethod acClassMethod, object[] acParameter)
        {
            return HandleExecuteACMethod_PAETerminal3xxxBase(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion
    }
}
