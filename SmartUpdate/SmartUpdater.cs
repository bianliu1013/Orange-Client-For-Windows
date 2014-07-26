using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;

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

        public SmartUpdater(ISmartUpdatable applicationInfo, SmartUpdaterForOrange uc_updater)
        {
            type = 0;

            this.applicationInfo = applicationInfo;
            this.uc_updater = uc_updater;
            this.bgWorker = new BackgroundWorker();
            this.bgWorker.DoWork += new DoWorkEventHandler(bgWorker_DoWork);
            this.bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorker_RunWorkerCompleted);
        }

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


                //업데이트 확인
                if (updateXml != null && updateXml.IsNewerThan(this.applicationInfo.ApplicationAssembly.GetName().Version))
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
                else if (updateXml != null && updateXml.IsEqualsVer(this.applicationInfo.ApplicationAssembly.GetName().Version))
                {
                    //같을때
                    if(type==0)
                        uc_updater.SetVersionState(false, updateXml);
                    
                }
            }
        }

        #endregion

        private void UpdateApplication(string tempFilePath, string currentPath, string newPath, string launchArgs)
        {
            string argument = "/C Choice /C Y /N /D Y /T 4 & Del /F /Q \"{0}\" & Choice /C Y /N /D Y /T 2 & Move /Y \"{1}\" \"{2}\" & Start \"\" /D \"{3}\" \"{4}\" {5}";

            ProcessStartInfo info = new ProcessStartInfo();
            info.Arguments = string.Format(argument, currentPath, tempFilePath, newPath, Path.GetDirectoryName(newPath), Path.GetFileName(newPath), launchArgs);
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.CreateNoWindow = true;
            info.FileName = "cmd.exe";
            Process.Start(info);
        }

    }
}
