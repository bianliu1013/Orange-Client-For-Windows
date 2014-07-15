using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Windows;
using Orange.MsgBroker;
using System.Windows.Threading;

namespace Orange.Util
{
    enum WKIND { DOWN, CONVERT }
     class ConvertMP3
    {
         public static string URL;
         public static string PATH;

        public static void worker(MainWindow win, string url, string path, WKIND kind)
        {

           // string sCurrArg = null;
           // string path = null;

            //if (args.Length < 1)
            //{
            //    System.MessageBox.Show("failed.. args[] length");
            //    return;
            //}

            //sCurrArg = args[0];

            ////옵션체크
            //if (sCurrArg.Contains("-u:"))
            //{
            //    path = sCurrArg.Replace("-u:", "");
            //}

            YoutubeController uTube = new YoutubeController(url, kind);
            uTube.SetPreConvert(win, path);
            /////////////////////////
            //
            //  옵션사항 여기에 추가
            //
            /////////////////////////

            //youtube-dl 사용


            uTube.Usingthread();

        }


    }




    class YoutubeController
    {
        private string appDataPath;         // 임시폴더 주소, "~\AppData\Roaming\orange"
        private readonly string urlPath;    // Youtube Url 주소
        private WKIND kind;

        private Process prcYOUTUBEDL = new Process();

        private string fileName = "";                   // 순수 이름, ex) abc
        private string extraName = "";                  // 확장자 포함 이름, ex) abc.mp4

        //임시로..
   //     private string srcPath = "D:/abc/";     // 소스 주소
  //      private string dstPath = "D:/def/";     // 목적지 주소
        private string currPath = "";           // 현재 주소
        private string dstPath;
        private MainWindow win;
        //생성자
        public YoutubeController()
        {
            appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + "/orange";
            kind = WKIND.DOWN;
        }
        public YoutubeController(string s)
        {
            appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + "/orange";
            urlPath = s;
            kind = WKIND.DOWN;
        }
        public YoutubeController(string s, WKIND k)
        {
            appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + "/orange";
            urlPath = s;
            kind = k;
        }

        public void SetPreConvert(MainWindow win, string path)
        {
            this.win = win;
            this.dstPath = path;
        }

        public void Usingthread()
        {
            Thread thread = new Thread(new ThreadStart(YoutubDownload));
            thread.Start();
        }
        // Youtube-dl 사용 함수
        public void YoutubDownload()
        {
            if(urlPath == null)
            {
                MessageBox.Show("URL has not been defined.");
                return;
            }

            string strYOUTUBEDLOut;
            ProcessStartInfo psiProcInfo = new ProcessStartInfo();
            System.IO.StreamReader srYOUTUBEDL;


            dstPath = SpecificCharChange('\\', '/', dstPath); 

            string strYOUTUBEDLCmd = null;

            if (kind == WKIND.DOWN)
            {
                strYOUTUBEDLCmd = "-f best -o \"" + dstPath + "%(title)s.%(ext)s\" \"" + urlPath + "\"";
                currPath = dstPath;
            }
            else if (kind == WKIND.CONVERT)
            {
               currPath = SpecificCharChange('\\', '/', appDataPath);    // '\\' -> '/' 로 변환 
               strYOUTUBEDLCmd = "-f best -o \"" + currPath + "%(title)s.%(ext)s\" --extract-audio --audio-format mp3 \"" + urlPath + "\"";               
            }
            else { return; }

            try
            {
                psiProcInfo.FileName = "Util/youtube-dl.exe";
                psiProcInfo.Arguments = strYOUTUBEDLCmd;
                psiProcInfo.UseShellExecute = false;
                psiProcInfo.WindowStyle = ProcessWindowStyle.Hidden;
                psiProcInfo.RedirectStandardError = true;
                psiProcInfo.RedirectStandardOutput = true;
                psiProcInfo.CreateNoWindow = true;

                prcYOUTUBEDL.StartInfo = psiProcInfo;
                prcYOUTUBEDL.Start();
                srYOUTUBEDL = prcYOUTUBEDL.StandardOutput;
                
                int cnt=0;
                do
                {
                    strYOUTUBEDLOut = srYOUTUBEDL.ReadLine();
                    string download = "[download]";
                    if (strYOUTUBEDLOut != null && strYOUTUBEDLOut.TrimStart().IndexOf(download) == 0)
                    {
                        if(cnt == 0)
                        {
                            // 확장자포함 이름 추출
                            if (strYOUTUBEDLOut.TrimStart().IndexOf("downloaded") == strYOUTUBEDLOut.Length-10)
                            {
                                string text = strYOUTUBEDLOut.TrimStart().Substring(download.Length + 1 + currPath.Length);
                                extraName = text.Substring(0, text.Length - 28);
                            }
                            else 
                            {
                                string text = strYOUTUBEDLOut.TrimStart().Substring(download.Length + 14 + currPath.Length);
                                extraName = text.Substring(0, text.Length);
                            }
                            //순수이름 추출
                            fileName = extraName.Substring(0, extraName.Length - 4);                            
                            ++cnt;
                            continue;
                        }
                        try
                        {
                            string text = strYOUTUBEDLOut.TrimStart().Substring(download.Length + 1);
                            int pos = text.IndexOf("%");
                            string currProgress = text.Substring(0, pos);

                            win.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate
                            {
                                // 작업상태 체크
                                MsgBroker.MsgBrokerMsg arg = new MsgBroker.MsgBrokerMsg();
                                arg.MsgOPCode = UI_CONTROL.SET_CONVERT_PROGRESS_VALUE;
                                arg.MsgBody = Double.Parse(currProgress);

                                (Application.Current as App).msgBroker.SendMessage(arg);
                                // 프로그래스바 작업
                            }));

                        }
                        catch(Exception e)
                        {
                            MessageBox.Show(e.Message);
                        }
                    }
                } while (prcYOUTUBEDL.HasExited == false);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
             finally
            {
                if (kind == WKIND.CONVERT)
                {
                    win.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                    if (MoveFile())
                    {
                      
                            // 작업상태 체크
                            MsgBroker.MsgBrokerMsg arg = new MsgBroker.MsgBrokerMsg();
                            arg.MsgOPCode = UI_CONTROL.FINISH_CONVERT_PROGRESS;
                            (Application.Current as App).msgBroker.SendMessage(arg);
                            // 프로그래스바 작업
                    
                    }
                    }));
                }
                else if (kind == WKIND.DOWN)
                {
                    if (System.IO.File.Exists(@dstPath + extraName))
                    {
                        win.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                        {
                            // 작업상태 체크
                            MsgBroker.MsgBrokerMsg arg = new MsgBroker.MsgBrokerMsg();
                            arg.MsgOPCode = UI_CONTROL.FINISH_CONVERT_PROGRESS;
                            (Application.Current as App).msgBroker.SendMessage(arg);
                            // 프로그래스바 작업
                        }));
                    }                       
                    else
                    {
                        MessageBox.Show("Not exists video file");
                        win.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                        {
                            // 작업상태 체크
                            MsgBroker.MsgBrokerMsg arg = new MsgBroker.MsgBrokerMsg();
                            arg.MsgOPCode = UI_CONTROL.FINISH_CONVERT_PROGRESS;
                            (Application.Current as App).msgBroker.SendMessage(arg);
                            // 프로그래스바 작업
                        }));
                    }
                        
                }
            }
                
        }
        

        // 특정문자 변환 메소드
        public string SpecificCharChange(char s, char c, string path )
        {
            string[] spstring = path.Split(s);
               string newPath = null;

               for (int i = 0; i < spstring.Length; i++)
               {
                   newPath += spstring[i] + c;
               }
               return newPath;
        }

        //파일이동 메소드
        public bool MoveFile()
        {
            if (System.IO.File.Exists(@currPath + fileName + ".mp3"))
            {
                string sourceFile = @currPath + fileName + ".mp3";
                string destinationFile = @dstPath + fileName + ".mp3";
                try
                {
                    // To move a file or folder to a new location:
                    System.IO.File.Move(sourceFile, destinationFile);
                }
                catch (System.IO.IOException e)
                {
                    MessageBox.Show(e.Message);
                    MessageBox.Show("mp3 파일 이동 실패");
                    // 작업상태 체크
                    MsgBroker.MsgBrokerMsg arg = new MsgBroker.MsgBrokerMsg();
                    arg.MsgOPCode = UI_CONTROL.FINISH_CONVERT_PROGRESS;
                    (Application.Current as App).msgBroker.SendMessage(arg);
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Not exists audio file");
                // 작업상태 체크
                MsgBroker.MsgBrokerMsg arg = new MsgBroker.MsgBrokerMsg();
                arg.MsgOPCode = UI_CONTROL.FINISH_CONVERT_PROGRESS;
                (Application.Current as App).msgBroker.SendMessage(arg);
                return false;
            }
            return true;
        }

        //파일삭제 메소드
        public void DeleteFile()
        {
            if (System.IO.File.Exists(@currPath + extraName))
            {
                try
                {
                    System.IO.File.Delete(@currPath + extraName);
                }
                catch (System.IO.IOException e)
                {
                    MessageBox.Show(e.Message);
                    return;
                }
            }
        }
    }

    //public class AsyncWorker
    //{
    //    public void UpdateProgress(string progress, out int threadId)
    //    {
    //        threadId = Thread.CurrentThread.ManagedThreadId;

    //        MessageBox.Show(progress);
    //        return;
    //    }

    //    public delegate void AsyncMethodCaller(string progress, out int threadId);
    //}
}


