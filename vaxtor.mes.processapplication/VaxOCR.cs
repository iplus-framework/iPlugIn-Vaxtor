using gip.core.autocomponent;
using gip.core.communication;
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace vaxtor.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioSystem, "en{'VaxOCR'}de{'VaxOCR'}", Global.ACKinds.TPABGModule, Global.ACStorableTypes.Required, false, false)]
    public class VaxOCR : PAClassAlarmingBase
    {
        #region c'tors

        public VaxOCR(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : 
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            return base.ACInit(startChildMode);
        }

        public override bool ACPostInit()
        {
            bool result = base.ACPostInit();

            _ShutdownEvent = new ManualResetEvent(false);
            _PollThread = new ACThread(Poll);
            _PollThread.Name = "ACUrl:" + this.GetACUrl() + ";Poll();";
            //_PollThread.ApartmentState = ApartmentState.STA;
            _PollThread.Start();

            return result;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            return base.ACDeInit(deleteACClassTask);
        }

        #endregion

        #region Properties

        [ACPropertyInfo(true, 400, "Config", "en{'Polling [ms]'}de{'Abfragezyklus [ms]'}", DefaultValue = 200)]
        public int PollingInterval { get; set; }

        [ACPropertyBindingSource(9999, "Error", "en{'VaxOCR alarm'}de{'VaxOCR Alarm'}", "", false, false)]
        public IACContainerTNet<PANotifyState> VaxOCRAlarm { get; set; }

        private ACRef<ACRestClient> _Client;
        public ACRestClient Client
        {
            get
            {
                if (_Client != null)
                    return _Client.ValueT;
                var client = FindChildComponents<ACRestClient>(c => c is ACRestClient).FirstOrDefault();
                if (client != null)
                {
                    // TODO: find right place for this
                    //ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                    _Client = new ACRef<ACRestClient>(client, this);
                    return _Client.ValueT;
                }
                return null;
            }
        }

        #endregion


        #region Methods

        protected ManualResetEvent _ShutdownEvent;
        ACThread _PollThread;
        private void Poll()
        {
            try
            {
                while (!_ShutdownEvent.WaitOne(PollingInterval, false))
                {
                    _PollThread.StartReportingExeTime();


                    _PollThread.StopReportingExeTime();
                }
            }
            catch (ThreadAbortException ec)
            {
                VaxOCRAlarm.ValueT = PANotifyState.AlarmOrFault;
                if (IsAlarmActive(nameof(VaxOCRAlarm), ec.Message) == null)
                {
                    Messages.LogException(GetACUrl(), $"{nameof(VaxOCRAlarm)}.{nameof(Poll)}(20)", ec);
                }
                OnNewAlarmOccurred(VaxOCRAlarm, ec.Message, true);
            }
        }

        #endregion
    }
}
