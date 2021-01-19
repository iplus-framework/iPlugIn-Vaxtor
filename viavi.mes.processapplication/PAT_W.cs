using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.processapplication;
using gip.core.autocomponent;
using gip.core.datamodel;
using System.Threading;

namespace viavi.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'PAT-W'}de{'PAT-W'}", Global.ACKinds.TPAModule, Global.ACStorableTypes.Required, false, true)]
    public class PAT_W : PAClassPhysicalBase
    {
        #region c'tors

        public PAT_W(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : 
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public const string ClassName = "PAT_W";

        #endregion

        #region Properties

        #region Properties => OPCTags

        [ACPropertyBindingTarget(9999, "", "en{'Common/Username'}de{'Common/Username'}")]
        public IACContainerTNet<string> CUserName
        {
            get;
            set;
        }

        [ACPropertyBindingTarget(9999, "", "en{'Common/Password'}de{'Common/Password'}")]
        public IACContainerTNet<string> CPassword
        {
            get;
            set;
        }

        [ACPropertyBindingTarget(9999, "", "en{'Common/SerialNumber'}de{'Common/SerialNumber'}")]
        public IACContainerTNet<string> CSerialNo
        {
            get;
            set;
        }

        [ACPropertyBindingTarget(9999, "", "en{'Common/MethodName'}de{'Common/MethodName'}")]
        public IACContainerTNet<string> CMethodName
        {
            get;
            set;
        }

        [ACPropertyBindingTarget(9999, "", "en{'Common/StartPro'}de{'Common/StartPro'}")]
        public IACContainerTNet<float> CStartPro
        {
            get;
            set;
        }

        [ACPropertyBindingTarget(9999, "", "en{'Common/StatusCode'}de{'Common/StatusCode'}")]
        public IACContainerTNet<float> CStatusCode
        {
            get;
            set;
        }

        [ACPropertyBindingTarget(9999, "", "en{'Common/Lamp'}de{'Common/Lamp'}")]
        public IACContainerTNet<float> CLamp
        {
            get;
            set;
        }

        [ACPropertyBindingTarget(9999, "", "en{'Workflow/ProjectName'}de{'Workflow/ProjectName'}")]
        public IACContainerTNet<string> WProjectName
        {
            get;
            set;
        }

        [ACPropertyBindingTarget(9999, "", "en{'Workflow/SampleName'}de{'Workflow/SampleName'}")]
        public IACContainerTNet<string> WSampleName
        {
            get;
            set;
        }

        [ACPropertyBindingTarget(9999, "", "en{'Workflow/BackgroundScan'}de{'Workflow/BackgroundScan'}")]
        public IACContainerTNet<float> WBackgroundScan
        {
            get;
            set;
        }

        [ACPropertyBindingTarget(9999, "", "en{'Workflow/InstrumentReady'}de{'Workflow/InstrumentReady'}")]
        public IACContainerTNet<float> WInstrumentReady
        {
            get;
            set;
        }

        [ACPropertyBindingTarget(9999, "", "en{'Workflow/Start'}de{'Workflow/Start'}")]
        public IACContainerTNet<float> WStart
        {
            get;
            set;
        }

        [ACPropertyBindingTarget(9999, "", "en{'Workflow/SampleInProgress'}de{'Workflow/SampleInProgress'}")]
        public IACContainerTNet<float> WSampleInProgress
        {
            get;
            set;
        }

        [ACPropertyBindingTarget(9999, "", "en{'Workflow/Stop'}de{'Workflow/Stop'}")]
        public IACContainerTNet<float> WStop
        {
            get;
            set;
        }

        [ACPropertyBindingTarget(9999, "", "en{'Workflow/ReportName'}de{'Workflow/ReportName'}")]
        public IACContainerTNet<string> WReportName
        {
            get;
            set;
        }

        [ACPropertyBindingTarget(9999, "", "en{'Common/Close'}de{'Common/Close'}")]
        public IACContainerTNet<float> CClose
        {
            get;
            set;
        }

        [ACPropertyBindingTarget(9999, "", "en{'Workflow/Method status'}de{'Workflow/Method status'}")]
        public IACContainerTNet<float> WMethodStatus
        {
            get;
            set;
        }

        #endregion

        [ACPropertyInfo(true, 301, "", DefaultValue = 1000)]
        public int ResponseTimeout
        {
            get;
            set;
        }

        [ACPropertyInfo(true, 301, "", DefaultValue = 1000)]
        public int StatusCodeResponseTimeout
        {
            get;
            set;
        }

        [ACPropertyInfo(true, 301, "", DefaultValue = 1000)]
        public int SampleInProgressResponseTimeout
        {
            get;
            set;
        }

        [ACPropertyInfo(true, 302, "")]
        public string SerialNumber
        {
            get;
            set;
        }

        [ACPropertyInfo(true, 303, "", "en{'Is device enabled'}de{'Is device enabled'}")]
        public bool IsDeviceEnabled
        {
            get;
            set;
        }

        [ACPropertyInfo(true, 304, "", "en{'Skip Device Lamp Test'}de{'Skip Device Lamp Test'}")]
        public bool SkipDeviceLampTest
        {
            get;
            set;
        }

        #endregion

        #region Methods



        #endregion
    }
}
