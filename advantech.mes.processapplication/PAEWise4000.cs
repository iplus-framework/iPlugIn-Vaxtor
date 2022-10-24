using gip.core.autocomponent;
using gip.core.communication;
using gip.core.datamodel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Xml;

namespace advantech.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'PAEWise4000'}de{'PAEWise4000'}", Global.ACKinds.TPAModule, Global.ACStorableTypes.Required, false, true)]
    public class PAEWise4000 : PAEWiseBase
    {

        #region ctor's

        public PAEWise4000(gip.core.datamodel.ACClass acType, gip.core.datamodel.IACObject content,
            gip.core.datamodel.IACObject parentACObject, gip.core.datamodel.ACValueList parameter, string acIdentifier = "") : base(acType, content, parentACObject, parameter, acIdentifier)
        {
           

        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
           
            return base.ACInit(startChildMode);
        }

        #endregion



    }
}
