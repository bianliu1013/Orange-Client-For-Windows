using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartUpdate
{
    public class CheckBeforeClosing
    {
        private ISmartUpdatable applicationInfo;
        private BackgroundWorker bgWorker;
        public bool IsAvailableUpdate { get; set; }
        public SmartUpdateXml updateXml { get; set; }
        public CheckBeforeClosing(ISmartUpdatable applicationInfo)
        {
            this.applicationInfo = applicationInfo;
            IsAvailableUpdate = false;
            this.bgWorker = new BackgroundWorker();
            this.bgWorker.DoWork += new DoWorkEventHandler(bgWorker_DoWork);
            this.bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorker_RunWorkerCompleted);
        }

    
        #region Thread bgWork

        public void CheckUpdate()
        {
            if (!this.bgWorker.IsBusy)
                this.bgWorker.RunWorkerAsync(this.applicationInfo);
        }


        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ISmartUpdatable application = (ISmartUpdatable)e.Argument;

            if (!SmartUpdateXml.ExistsOnServer(application.UpdateXmlLocation))
                e.Cancel = true;
            else
                e.Result = SmartUpdateXml.Parse(application.UpdateXmlLocation, application.ApplicationID);
        }

        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                SmartUpdateXml updateXml = (SmartUpdateXml)e.Result;


                //업데이트 확인
                if (updateXml != null && updateXml.IsNewerThan(this.applicationInfo.appId))
                {
                    
                    //업데이트 작업
                    IsAvailableUpdate = true;
                    this.updateXml = updateXml;
                }
                else if (updateXml != null && updateXml.IsEqualsVer(this.applicationInfo.appId))
                {
                    IsAvailableUpdate = false;                    
                }
            }
        }

        #endregion
    }
}
