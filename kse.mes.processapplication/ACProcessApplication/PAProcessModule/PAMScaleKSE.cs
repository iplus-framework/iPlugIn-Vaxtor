using gip.core.datamodel;
using gip.core.processapplication;
using gip.mes.processapplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kse.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'KSE Scale'}de{'KSE Waage'}", Global.ACKinds.TPAProcessModule, Global.ACStorableTypes.Required, false, PWGroupVB.PWClassName, true)]
    public class PAMScaleKSE : PAMHopperscale
    {

        #region ctor's
        static PAMScaleKSE()
        {
            RegisterExecuteHandler(typeof(PAMScaleKSE), HandleExecuteACMethod_PAMScaleKSE);
        }


        public PAMScaleKSE(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {

        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            bool result = base.ACInit(startChildMode);
            if (result && KSEConverter != null)
            {
                PAFDosingKSE dosingKSE = FindChildComponents<PAFDosingKSE>(c => c is PAFDosingKSE).FirstOrDefault();
                if (dosingKSE != null)
                    dosingKSE.ACStateConverter = KSEConverter;
                PAFDischargingKSE dischargingKSE = FindChildComponents<PAFDischargingKSE>(c => c is PAFDischargingKSE).FirstOrDefault();
                if (dischargingKSE != null)
                    dischargingKSE.ACStateConverter = KSEConverter;
            }
            return result;
        }

        #endregion

        #region Properties
        [ACPropertyBindingSource(501, "Config", "en{'Cleaning counter'}de{'Reinigungszähler'}", "", true, true, DefaultValue = 0)]
        public IACContainerTNet<Int32> CleaningCounter { get; set; }

        [ACPropertyBindingSource(502, "Configuration", "en{'Max. weight small scale [kg]'}de{'Max. Gewicht kleine Waage [kg]'}", "", true, true)]
        public IACContainerTNet<Double> MaxWeightCapacity2 { get; set; }

        [ACPropertyBindingSource(503, "Configuration", "en{'Max volume small scale [dm³]'}de{'Max. Volumen kleine Waage [dm³]'}", "", true, true)]
        public IACContainerTNet<Double> MaxVolumeCapacity2 { get; set; }


        public override PAEScaleGravimetric Scale
        {
            get
            {
                return FindChildComponents<PAEScaleGravimetric>(c => c is PAEScaleGravimetric && (c as PAEScaleGravimetric).ACIdentifier.EndsWith("Large"),
                                                                null, 1)
                                                                .FirstOrDefault();
            }
        }

        public PAEScaleGravimetric ScaleSmall
        {
            get
            {
                return FindChildComponents<PAEScaleGravimetric>(c => c is PAEScaleGravimetric && (c as PAEScaleGravimetric).ACIdentifier.EndsWith("Small"),
                                                                null, 1)
                                                                .FirstOrDefault();
            }
        }

        public KSEConvScale KSEConverter
        {
            get 
            {
                return FindChildComponents<KSEConvScale>(c => c is KSEConvScale).FirstOrDefault();
            }
        }
        #endregion

        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PAMScaleKSE(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PAMHopperscale(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion

    }
}
