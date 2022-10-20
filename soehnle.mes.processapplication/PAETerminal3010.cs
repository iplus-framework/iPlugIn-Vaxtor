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
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Scale Terminal 3010'}de{'Waage Terminal 3010'}", Global.ACKinds.TPAModule, Global.ACStorableTypes.Required, false, true)]
    public class PAETerminal3010 : PAETerminal3xxxBase
    {
        #region c'tors
        static PAETerminal3010()
        {
            RegisterExecuteHandler(typeof(PAETerminal3010), HandleExecuteACMethod_PAETerminal3010);
        }


        public PAETerminal3010(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") :
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

        public new const string ClassName = "PAETerminal3010";

        #endregion

        #region Override
        protected Comm3010 _Communicator;
        protected override Comm3xxxBase Communicator
        {
            get
            {
                if (_Communicator == null)
                    _Communicator = new Comm3010();
                return _Communicator;
            }
        }
        #endregion

        #region Helper-Methods
        public static bool HandleExecuteACMethod_PAETerminal3010(out object result, IACComponent acComponent, string acMethodName, ACClassMethod acClassMethod, object[] acParameter)
        {
            return HandleExecuteACMethod_PAETerminal3xxxBase(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion
    }
}
