using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Reflection;

namespace SmartUpdate
{
    public class SmartUpdater
    {
/*        private SmartUpdateXml updateXml;*/
        private ISmartUpdatable applicationInfo;
        private BackgroundWorker bgWorker;
        private SmartUpdaterForOrange uc_updater;
        private UpdateWindow win;
        private int type;

        //프로그램 정보 창
        public SmartUpdater(ISmartUpdatable applicationInfo, SmartUpdaterForOrange uc_updater)
        {
            type = 0;

            this.applicationInfo = applicationInfo;
            this.uc_updater = uc_updater;
            this.bgWorker = new BackgroundWorker();
            this.bgWorker.DoWork += new DoWorkEventHandler(bgWorker_DoWork);
            this.bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorker_RunWorkerCompleted);
        }

        //프로그램 종료 전
        public SmartUpdater(ISmartUpdatable applicationInfo, UpdateWindow win)
        {
            type = 1;

            this.applicationInfo = applicationInfo;
            this.win = win;
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


                //string version = fvi.FileVersion;
                //업데이트 확인
                if (updateXml != null && updateXml.IsNewerThan(applicationInfo.appId))
                {
                    if(type==0)
                        uc_updater.SetVersionState(true, updateXml);
                    else if(type==1)
                    {
                        win.SetVersionState(updateXml);
                    }
                   
                    //업데이트 작업

                    //this.DownloadUpdate(updateXml);

                }
                else if (updateXml != null && updateXml.IsEqualsVer(applicationInfo.appId))
                {
                    //같을때
                    if(type==0)
                        uc_updater.SetVersionState(false, updateXml);
                    
                }
            }
        }

        #endregion


    }
}
