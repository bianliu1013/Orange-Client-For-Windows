using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Net;
using System.ComponentModel;
using System.IO;

namespace SmartUpdate
{
    /// <summary>
    /// SmartUpdateDownloadWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SmartUpdateDownloadWindow : Window
    {
        public WebClient webClient;
        private BackgroundWorker bgWorker;
        private string tempFile;
        private string md5;

        internal string TempFilePath
        {
            get { return this.tempFile; }
        }

        internal SmartUpdateDownloadWindow(Uri location, string md5, ImageSource programIcon)
        {
            InitializeComponent();

            if (programIcon != null)
                this.Icon = programIcon;

            tempFile = Path.GetTempFileName();

            this.md5 = md5;

            webClient = new WebClient();
            webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(webClient_DownloadProgressChanged);
            webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(webClient_DownloadFileCompleted);

            bgWorker = new BackgroundWorker();
            bgWorker.DoWork += new DoWorkEventHandler(bgWorker_DoWork);
            bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorker_RunWorkerCompleted);

            try { webClient.DownloadFileAsync(location, this.tempFile); }
            catch { this.DialogResult = false; this.Close(); }
        }

        private void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.progressBar.Value = e.ProgressPercentage;
            this.lblProgress.Content = String.Format("Downloaded {0} of {1}", FormatBytes(e.BytesReceived, 1, true), FormatBytes(e.TotalBytesToReceive, 1, true));
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
                this.DialogResult = false;
                this.Close();
            }
            else if (e.Cancelled)
            {
                this.DialogResult = false;
                this.Close();
            }
            else
            {
                lblProgress.Content = "Verifying Download...";
                //progressBar.Style = ProgressBarStyle.Marquee;

                bgWorker.RunWorkerAsync(new string[] { this.tempFile, this.md5 });
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
            this.DialogResult = (bool)e.Result;
            this.Close();
        }

        //private void SmartUpdateDownloadForm_FormClosed(object sender, FormClosedEventArgs e)
        //{
        //    if (webClient.IsBusy)
        //    {
        //        webClient.CancelAsync();
        //        this.DialogResult = DialogResult.Abort;
        //    }

        //    if (bgWorker.IsBusy)
        //    {
        //        bgWorker.CancelAsync();
        //        this.DialogResult = DialogResult.Abort;
        //    }
        //}
    }
}
