using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;
namespace SmartUpdate
{
	/// <summary>
	/// SmartUpdaterForOrange.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class SmartUpdaterForOrange : UserControl
	{
        private bool IsDownloadState;
        private SmartUpdater updater;

        private ISmartUpdatable applicationInfo;
        private SmartUpdateXml updateInfo;

        private WebClient webClient;
        private BackgroundWorker bgWorker;
        private string tempFile;
        private string cur_ver;

		public SmartUpdaterForOrange()
		{
			this.InitializeComponent();
           // current_ver.Text = applicationInfo.ApplicationAssembly.GetName().Version.ToString();

            

            tempFile = System.IO.Path.GetTempFileName();  



		}
		
        public void initSmartUpdate(ISmartUpdatable applicationInfo, string cur_ver)
        {
            this.cur_ver = cur_ver;
            this.applicationInfo = applicationInfo;

            updater = new SmartUpdater(applicationInfo, this);
            current_ver.Text = cur_ver;


            if (updater != null)
                updater.CheckUpdate();
            else
            { MessageBox.Show("init problem"); }

        }




		private void Image_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
            e.Handled = true;

            if(e.ClickCount > 3)
            {
                name.Visibility = Visibility.Visible;
            }
            else
            {

            }
        }

        public void SetVersionState(bool IsNewVer, SmartUpdateXml updateXml)
        {
            if(IsNewVer)
            {
                this.updateInfo = updateXml;
                NewVersionTb.Text = String.Format("New Ver : {0}", updateInfo.Version.ToString());
                IsDownloadState = true;
                existNewGrid.Visibility = Visibility.Visible;
            }
            else
            {
                updateText.Visibility = Visibility.Visible;
            }
        }


        private void updatebtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try { 
                //webClient.DownloadFileAsync(updateInfo.Uri, this.tempFile); 

                UpdateWindow updatewin = new UpdateWindow();
                updatewin.initSmartUpdate(applicationInfo);
                bool result = (bool)updatewin.ShowDialog();

                if (result == true)
                {

                }
                else if (result == false)
                {
                    //MessageBoxOrange.ShowDialog("the update download was cancelled.");
                }
            
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }




        #region UpdateDownload

        private void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.progressBar.Value = e.ProgressPercentage;
            updateText.Text = String.Format("Downloaded {0} of {1}", FormatBytes(e.BytesReceived, 1, true), FormatBytes(e.TotalBytesToReceive, 1, true));
        }

        private string FormatBytes(long bytes, int decimalPlaces, bool showByteType)
        {
            double newBytes = bytes;
            string formatString = "{0";
            string byteType = "B";

            if (newBytes > 1024 && newBytes < 1048576)
            {
                newBytes /= 1024;
                byteType = "KB";
            }
            else if (newBytes > 1048576 && newBytes < 1073741824)
            {
                newBytes /= 1048576;
                byteType = "MB";
            }
            else
            {
                newBytes /= 1073741824;
                byteType = "GB";
            }

            if (decimalPlaces > 0)
                formatString += ":0.";

            for (int i = 0; i < decimalPlaces; i++)
                formatString += "0";

            formatString += "}";

            if (showByteType)
                formatString += byteType;

            return string.Format(formatString, newBytes);
        }

        private void webClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.ToString());
                existNewGrid.Visibility = Visibility.Visible;
                updateText.Visibility = Visibility.Hidden;
            }
            else if (e.Cancelled)
            {
                MessageBox.Show("the update download was cancelled.");
                existNewGrid.Visibility = Visibility.Visible;
                updateText.Visibility = Visibility.Hidden;
            }
            else
            {
                updateText.Text = "Verifying Download...";
                //progressBar.Style = ProgressBarStyle.Marquee;
                bgWorker.RunWorkerAsync(new string[] { this.tempFile, updateInfo.MD5 });
            }


        }

        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string file = ((string[])e.Argument)[0];
            string updateMd5 = ((string[])e.Argument)[1];

            string tmp = Hasher.HashFile(file, HashType.MD5);
           
            if (tmp != updateMd5)
            {

                e.Result = false;
            }
            else
                e.Result = true;
        }

        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsDownloadState = (bool)e.Result;


            if (IsDownloadState == true)
            {
                string currentPath = this.applicationInfo.ApplicationAssembly.Location;
                string newPath = System.IO.Path.GetDirectoryName(currentPath) + "\\" + updateInfo.FileName;

                UpdateApplication(tempFile, currentPath, newPath, updateInfo.LaunchArgs);

                Environment.Exit(0);
            }

            else if (IsDownloadState == false)
            {
                MessageBox.Show("the update download was cancelled.");
            }
        }

        private void UpdateApplication(string tempFilePath, string currentPath, string newPath, string launchArgs)
        {
            string argument = "/C Choice /C Y /N /D Y /T 4 & Del /F /Q \"{0}\" & Choice /C Y /N /D Y /T 2 & Move /Y \"{1}\" \"{2}\" & Start \"\" /D \"{3}\" \"{4}\" {5}";

            ProcessStartInfo info = new ProcessStartInfo();
            info.Arguments = string.Format(argument, currentPath, tempFilePath, newPath, System.IO.Path.GetDirectoryName(newPath), System.IO.Path.GetFileName(newPath), launchArgs);
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.CreateNoWindow = true;
            info.FileName = "cmd.exe";
            info.Verb = "runas";
            Process.Start(info);
        }



        #endregion

        



    }
}