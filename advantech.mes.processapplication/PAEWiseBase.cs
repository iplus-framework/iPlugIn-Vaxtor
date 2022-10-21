using gip.core.autocomponent;
using gip.core.datamodel;

namespace advantech.mes.processapplication
{

    [ACClassInfo(Const.PackName_VarioAutomation, "en{'PAEWiseBase'}de{'PAEWiseBase'}", Global.ACKinds.TPAModule, Global.ACStorableTypes.Required, false, true)]
    public class PAEWiseBase : PAModule
    {

        #region ctor's

        public PAEWiseBase(gip.core.datamodel.ACClass acType, gip.core.datamodel.IACObject content, gip.core.datamodel.IACObject parentACObject, gip.core.datamodel.ACValueList parameter, string acIdentifier = "") : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            
        }

        #endregion
    }
}
