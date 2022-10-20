using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.datamodel;
using kse.mes.processapplication;

namespace gip.mes.processapplication

{
    /// <summary>
    /// Gruppenknoten für Steuerrezeptstufen
    /// 
    /// Methoden zur Steuerung von außen: 
    /// -Start()    Starten des Processes
    ///
    /// Mögliche ACState:
    /// SMIdle      (Definiert in ACComponent)
    /// SMStarting (Definiert in PWGroup)
    /// SMRunning   (Definiert in PWGroup)
    /// </summary>
    [ACClassInfo(Const.PackName_VarioSystem, "en{'Container WF group'}de{'Container WF Gruppe'}", Global.ACKinds.TPWGroup, Global.ACStorableTypes.Optional, false, PWProcessFunction.PWClassName, true)]
    public class PWContGroupKSE : PWGroupVB
    {
        new public const string PWClassName = "PWContGroupKSE";

        #region c´tors
        static PWContGroupKSE()
        {
            RegisterExecuteHandler(typeof(PWContGroupKSE), HandleExecuteACMethod_PWContGroupKSE);
        }

        public PWContGroupKSE(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier) 
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;

            return true;
        }
        #endregion

        #region Properties

        public override List<PAProcessModule> ProcessModuleList
        {
            get
            {
                if (ApplicationManager == null || RootPW == null)
                    return null;

                PWContReqKSE reqNode = RootPW.FindChildComponents<PWContReqKSE>(c => c is PWContReqKSE).FirstOrDefault();
                if (reqNode == null)
                {
                    // TODO: AlarmMessage
                    return null;
                }

                if (reqNode.IterationCount.ValueT <= 0)
                {
                    return null;
                }

                ACMethod lastResult = reqNode.PreviousACMethod;
                if (lastResult == null)
                {
                    // TODO: Alarm message
                    return null;
                }

                UInt16 contInterfaceNo = lastResult.ResultValueList.GetUInt16("ContainerInterfaceNr");
                if (contInterfaceNo <= 0 && !this.ApplicationManager.IsSimulationOn)
                {
                    // TODO: Alarm message
                    return null;
                }
                else if (contInterfaceNo <= 0 && this.ApplicationManager.IsSimulationOn)
                {
                    return base.ProcessModuleList;
                }
                else
                {
                    core.datamodel.ACClass refPAACClass = null;

                    using (ACMonitor.Lock(this.ContextLockForACClassWF))
                    {
                        refPAACClass = ContentACClassWF.RefPAACClass;
                    }

                    var possibleContainer = ApplicationManager.FindChildComponents(refPAACClass, 0)
                        .Select(c => c as PAProcessModule)
                        .Where(c => c.OperatingMode.ValueT == Global.OperatingMode.Automatic)
                        .ToList();
                    if (!possibleContainer.Any())
                        return null;


                    return possibleContainer.Where(c => c.RouteItemIDAsNum == contInterfaceNo).ToList();
                }
           }
        }
#endregion

#region Methods

#region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PWContGroupKSE(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PWGroupVB(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
#endregion


        public override void Reset()
        {
            ResetOccupyID();
            base.Reset();
        }

        public override void SMStarting()
        {
            base.SMStarting();
        }

        public override void SMRunning()
        {
            UInt16 occupyID = 0;
            GetOccupyID(out occupyID);
            KSEConvContTransp contTransp = AccessedProcessModule.FindChildComponents<KSEConvContTransp>(c => c is KSEConvContTransp).FirstOrDefault();
            if (contTransp != null)
            {
                contTransp.OccupyId.ValueT = occupyID;
                contTransp.Release.ValueT = ContTranspRelease.Freigegeben;
            }
        }

        public void GetOccupyID(out UInt16 occupyID)
        {
            occupyID = 0;
            if (IsProduction && ParentPWMethod<PWMethodProduction>().CurrentProdOrderBatch != null && AccessedProcessModule != null)
            {
                occupyID = Convert.ToUInt16(ParentPWMethod<PWMethodProduction>().CurrentProdOrderBatch.BatchSeqNo);
            }
            else if (IsIntake && AccessedProcessModule != null)
            {
                var pwMethod = ParentPWMethod<PWMethodIntake>();
                ACMethod acMethod = pwMethod.CurrentACMethod.ValueT;
                if (acMethod != null)
                {
                    using (var db = new Database())
                    using (var dbApp = new DatabaseApp(db))
                    {
                        Picking picking = PWDischarging.GetTransportEntityFromACMethod(dbApp, acMethod) as Picking;
                        if (picking != null)
                        {
                            try
                            {
                                occupyID = Convert.ToUInt16(picking.InsertDate.TimeOfDay.TotalMinutes);
                            }
                            catch (Exception ec)
                            {
                                string msg = ec.Message;
                                if (ec.InnerException != null && ec.InnerException.Message != null)
                                    msg += " Inner:" + ec.InnerException.Message;

                                if (gip.core.datamodel.Database.Root != null && gip.core.datamodel.Database.Root.Messages != null &&
                                                                                      gip.core.datamodel.Database.Root.InitState == ACInitState.Initialized)
                                    gip.core.datamodel.Database.Root.Messages.LogException("PWContGroupKSE", "GetOccupyID", msg);
                            }
                        }
                    }
                }
            }
        }

        public UInt16? GetPickingDestination()
        {
            UInt16? destID = null;
            if (IsIntake && AccessedProcessModule != null)
            {
                var pwMethod = ParentPWMethod<PWMethodIntake>();
                ACMethod acMethod = pwMethod.CurrentACMethod.ValueT;
                if (acMethod != null)
                {
                    using (var db = new Database())
                    using (var dbApp = new DatabaseApp(db))
                    {
                        Picking picking = PWDischarging.GetTransportEntityFromACMethod(dbApp, acMethod) as Picking;
                        if (picking != null)
                        {
                            PickingPos pos = picking.PickingPos_Picking.FirstOrDefault();
                            if (pos != null && pos.ToFacility != null)
                            {
                                if (pos.ToFacility.FacilityNo.StartsWith("4FA") || pos.ToFacility.FacilityNo.StartsWith("4KA"))
                                {
                                    string siloNr = pos.ToFacility.FacilityNo.Substring(3);
                                    if (!String.IsNullOrEmpty(siloNr))
                                    {
                                        try
                                        {
                                            destID = Convert.ToUInt16(siloNr);
                                        }
                                        catch (Exception ec)
                                        {
                                            string msg = ec.Message;
                                            if (ec.InnerException != null && ec.InnerException.Message != null)
                                                msg += " Inner:" + ec.InnerException.Message;

                                            if (gip.core.datamodel.Database.Root != null && gip.core.datamodel.Database.Root.Messages != null &&
                                                                                                  gip.core.datamodel.Database.Root.InitState == ACInitState.Initialized)
                                                gip.core.datamodel.Database.Root.Messages.LogException("PWContGroupKSE", "GetPickingDestination", msg);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return destID;
        }

        public override void SMCompleted()
        {
            ResetOccupyID();
            base.SMCompleted();
        }

        protected void ResetOccupyID()
        {
            if (AccessedProcessModule != null)
            {
                KSEConvContTransp contTransp = AccessedProcessModule.FindChildComponents<KSEConvContTransp>(c => c is KSEConvContTransp).FirstOrDefault();
                if (contTransp != null)
                {
                    contTransp.OccupyId.ValueT = (UInt16)0;
                    contTransp.Release.ValueT = ContTranspRelease.Freigegeben;
                }
            }
        }
#endregion
    }
}
