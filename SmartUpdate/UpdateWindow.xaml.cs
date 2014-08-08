using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SmartUpdate
{
    /// <summary>
    /// UpdateWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UpdateWindow : Window
    {
        private bool IsDownloadState;
        private SmartUpdateXml updateInfo;
        private SmartUpdater updater;
        private WebClient webClient;
        private BackgroundWorker bgWorker;
        private string tempFile;
        private ISmartUpdatable applicationInfo;


        private string application_path;
        private string download_uri;

        private List<string> file_list;
        string argument;

        private int complete_file_cnt;
        private int total_file_cnt;

        internal string Argument
        {
            get { return this.argument; }
        }

        public UpdateWindow()
        {
            InitializeComponent();

            tempFile = System.IO.Path.GetTempFileName();

        }

        private async Task DownloadMultipleFileAsync()
        {
            argument = "/C Choice /C Y /N /D Y /T 3";
            IsDownloadState = true;
            await Task.WhenAll(file_list.Select(file_name => DownloadFileAsync(file_name)));

            if (IsDownloadState == true)
            {
                string path = this.applicationInfo.ApplicationAssembly.Location;
                argument = argument + "& Start \"\" /D \"" + System.IO.Path.GetDirectoryName(path) + "\" \"" + System.IO.Path.GetFileName(path) + "\"";               
                UpdateApplication(argument);
                System.Environment.Exit(0);
            }

            else if (IsDownloadState == false)
            {
                MessageBox.Show("the update download was cancelled.");
            }
        }

        private async Task DownloadFileAsync(string file_name)
        {
            Uri uri = new Uri(download_uri + file_name);

            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(webClient_DownloadProgressChanged);
                    
                    string temp_path = System.IO.Path.GetTempFileName();
                    await webClient.DownloadFileTaskAsync(uri, temp_path);
                    string target_path = System.IO.Path.GetDirectoryName(application_path) + "\\" + file_name;

                    complete_file_cnt++;
                    this.progressText.Text = String.Format("Downloaded {0} of {1}", /*다운받은 갯수*/complete_file_cnt, /*전체 파일 갯수*/total_file_cnt);

                    argument = argument + " & Move /Y \"" + temp_path + "\" \"" + target_path + "\"";
                    
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Fail : " + ex.Message);
                IsDownloadState = false;
            }
        }

        public void initSmartUpdate(ISmartUpdatable applicationInfo)
        {
            this.applicationInfo = applicationInfo;

            updater = new SmartUpdater(applicationInfo, this);



            if (updater != null)
                updater.CheckUpdate();
            else
            { MessageBox.Show("init problem"); }

        }

        private void update_Click(object sender, System.Windows.RoutedEventArgs e)
        {
           
            if (updateInfo != null)
            {
                try
                {
                    DownloadMultipleFileAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }

        private void cancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public void SetVersionState(SmartUpdateXml updateXml)
        {
            updateText.Text = updateXml.Version.ToString();
            this.updateInfo = updateXml;

            this.application_path = applicationInfo.ApplicationAssembly.Location;
            this.download_uri = updateInfo.Uri.ToString();
            this.file_list = updateInfo.FileList;

            this.complete_file_cnt = 0;
            this.total_file_cnt = file_list.Count;

            complete_file_cnt++;
            this.progressText.Text = String.Format("Downloaded {0} of {1}", /*다운받은 갯수*/complete_file_cnt, /*전체 파일 갯수*/total_file_cnt);


        }

        private void Rectangle_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
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
                MessageBox.Show("the update download was cancelled.");
                this.DialogResult = false;
 
              
            }
            else if (e.Cancelled)
            {
                MessageBox.Show("the update download was cancelled.");
                this.DialogResult = false;
            }
            else
            {
                updateText.Text = "Verifying Download...";
                //progressBar.Style = ProgressBarStyle.Marquee;
                bgWorker.RunWorkerAsync(new string[] { this.tempFile, updateInfo.MD5 });
            }


        }

//         private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
//         {
//             string file = ((string[])e.Argument)[0];
//             string updateMd5 = ((string[])e.Argument)[1];
// 
//             string tmp = Hasher.HashFile(file, HashType.MD5);
//             if (tmp != updateMd5)
//             {
// 
//                 e.Result = false;
//             }
//             else
//                 e.Result = true;
//         }
// 
//         private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
//         {
//             IsDownloadState = (bool)e.Result;
// 
// 
//             if (IsDownloadState == true)
//             {
//                 string currentPath = this.applicationInfo.ApplicationAssembly.Location;
//                 string newPath = System.IO.Path.GetDirectoryName(currentPath) + "\\" + updateInfo.FileName;
// 
//                 UpdateApplication(tempFile, currentPath, newPath, updateInfo.LaunchArgs);
// 
//                 Environment.Exit(0);
//             }
// 
//             else if (IsDownloadState == false)
//             {
//                 MessageBox.Show("the update download was cancelled.");
//             }
//         }


        //private void UpdateApplication(string tempFilePath, string currentPath, string newPath, string launchArgs)
        private void UpdateApplication(string argument)
        {
            //string argument = "/C Choice /C Y /N /D Y /T 4 & Del /F /Q \"{0}\" & Choice /C Y /N /D Y /T 2 & Move /Y \"{1}\" \"{2}\" & Start \"\" /D \"{3}\" \"{4}\" {5}";

            ProcessStartInfo info = new ProcessStartInfo();
            //info.Arguments = string.Format(argument, currentPath, tempFilePath, newPath, Path.GetDirectoryName(newPath), Path.GetFileName(newPath), launchArgs);
            info.Arguments = argument;
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.CreateNoWindow = true;
            info.FileName = "cmd.exe";
            Process.Start(info);
        }

        #endregion

        
    }
}
