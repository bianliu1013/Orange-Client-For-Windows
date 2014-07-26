using MahApps.Metro.Controls;
using Orange.MsgBroker;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Json;
using System.IO;
using System.Web;
using System.Security.Permissions;
using System.Reflection;
using Orange.DataManager;
using System.Threading;
using System.Windows.Threading;
using Orange.Util;
using System.Security.Cryptography;
using Microsoft.Win32;
using System.Windows.Controls.Primitives;
using SmartUpdate;

namespace Orange
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]

    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private bool dragStarted = false;
        private bool IsLeftPanelState = false;
        private MusicCollection musicCollection;
        private MusicCollection myPlayListCollection;
        private List<MusicItem> playlist = new List<MusicItem>();
        private MusicItem CurrentItem;
        private CheckBeforeClosing check;
        private Storyboard HideLeftPanelStoryboard;
        private Storyboard ShowLeftPanelStoryboard;
        private Storyboard HideTopGridStoryboard;
        private Storyboard ShowTopGridStoryboard;
        private string queryString;
        private DispatcherTimer dt;
        private double totalTime;
        private double currentTime;

        private int cur_page;

        public MainWindow()
        {
            InitializeComponent();
            
            initStoryboard();
            initUserConfig();

            musicCollection = new MusicCollection();
            myPlayListCollection = new MusicCollection();
            LoadConfig();
      
            main_menu.SetMusicCollection(musicCollection);
           
            result_musiclist.DataContext = musicCollection;
            myPlayList.DataContext = myPlayListCollection;

            webBrowser.Navigated += webBrowser_Navigated;
            
            WebBrowserHelper.ClearCache();
                
            String url = "http://115.71.236.224:8081/static/YouTubePlayer.html";
            webBrowser.Navigate(url);

            (Application.Current as App).msgBroker.MessageReceived += msgBroker_MessageReceived;

            //String volume = webBrowser.InvokeScript("getVolume").ToString();
            dt = new DispatcherTimer();
            dt.Interval = new TimeSpan(0, 0, 0, 0, 500);
            dt.Tick += dt_Tick;

            myPlayListCollection.CollectionChanged += myPlayListCollection_CollectionChanged;
        }



        private void initUserConfig()
        {
            if(UI_Flag.IsShowingVideo){
                ShowVideoBtn.Content = "HideVideo";
            }
            else { ShowVideoBtn.Content = "ShowVideo"; }
        }

        void myPlayListCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            playlist.Clear();
            CopyPlayList();
        }

        void dt_Tick(object sender, EventArgs e)
        {
            if (!dragStarted)
            {
                totalTime = Double.Parse(webBrowser.InvokeScript("getDuration").ToString());
                currentTime = Double.Parse(webBrowser.InvokeScript("getCurrentTime").ToString());
                
                if(totalTime!=0.0)
                {
                    PlayerSlider.Maximum = totalTime;
                    PlayerSlider.Value = currentTime;

                    TimeSpan ctime = TimeSpan.FromSeconds(currentTime);
                    TimeSpan endtime = TimeSpan.FromSeconds(totalTime);

                    currentTimeTxb.Text = ctime.ToString(@"mm\:ss"); 
                    endTimeTxb.Text = endtime.ToString(@"mm\:ss");

                    if(totalTime == currentTime)
                    {
                        EndMusic();
                    }
                }
                
            }
           
        }

        private void EndMusic()
        {
           // MessageBox.Show("끗");
            dt.Stop();

            if(Config.REPEAT==1)
            {
                for(int i=0 ; i<playlist.Count; i++)
                {
                    if(playlist[i].Equals(CurrentItem))
                    {
                        
                        if(i == playlist.Count-1)
                        {
                            CurrentItem = playlist[0];
                        }
                        else
                        {
                            CurrentItem = playlist[i + 1];
                        }

                        PlayMusic(CurrentItem);

                        return;
                    }
                }
            }
            else if (Config.REPEAT == 0)
            {
                for (int i = 0; i < playlist.Count; i++)
                {
                    if (playlist[i].Equals(CurrentItem))
                    {

                        if (i != playlist.Count - 1)
                        {
                            CurrentItem = playlist[i + 1];
                        }
                        else
                        {
                            Player_State.IsPlaying = false;
                            return;
                        }

                        PlayMusic(CurrentItem);
                        return;
                    }
                }
            }
            else if(Config.REPEAT == 2)
            {
                PlayMusic(CurrentItem);
                return;
            }
        }

        private void SelectCurrentMusicItemInPlayList(MusicItem item)
        {            
            int idx = myPlayListCollection.IndexOf(item);
            myPlayList.SelectedIndex = idx;
        }

        private void CopyPlayList()
        {
            try
            {
                playlist = myPlayListCollection.ToList();

                if (Config.IsShffle)
                {
                    playlist = Shuffle.Randomize(playlist);
                    playlist.Remove(CurrentItem);
                    playlist.Insert(0, CurrentItem);
                }
            }catch(Exception e)
            { MessageBox.Show(e.Message); }
            
        }

      

        void msgBroker_MessageReceived(object sender, MsgBroker.MsgBrokerEventArgs e)
        {
            switch(e.Message.MsgOPCode)
            {
                case MESSAGE_MAP.KPOP_CART:
                    break;
                case UI_CONTROL.PROGRESS_SHOW:
                      if (IsLeftPanelState)
                      {
                          HideLeftPanelStoryboard.Begin();
                          IsLeftPanelState = false;
                      }
                      main_page.Visibility = Visibility.Visible;
                      main_page.SetProgressRing(true, 0);
                    break;

                case UI_CONTROL.PROGRESS_HIDE:
                     main_page.SetProgressRing(false, 0);
                     main_page.Visibility = Visibility.Collapsed;

                     Search_ScrollViewer.ScrollToHome();
                        
                    if(UI_Flag.IsChart)
                    {
                        morebtn.Visibility = Visibility.Hidden;
                    }
                    

                    break;

                case UI_CONTROL.SHOW_TOP_GRID:
                    if (!UI_Flag.IsShowingTopGrid)
                    {
                        
                        webBrowser.Visibility = Visibility.Hidden;
                                      
                        top_content.Children.Clear();
                        top_content.Children.Add((UserControl)e.Message.MsgBody);
                        ShowTopGridStoryboard.Begin();

                        UI_Flag.IsShowingTopGrid = true;
                    }                   
                    
                    break;
                case UI_CONTROL.HIDE_TOP_GRID:
                    if (UI_Flag.IsShowingTopGrid)
                    {
                        HideTopGridStoryboard.Begin();
                        UI_Flag.IsShowingTopGrid = false;

                        if(UI_Flag.IsShowingVideo && Player_State.IsPlaying)
                        {
                            webBrowser.Visibility = Visibility.Visible;   
                        }
                    }
                    break;

                case UI_CONTROL.SetTopmost:
                    this.Topmost = true;
                    this.Activate();
                    break;

                case UI_CONTROL.DisableTopmost:
                    this.Topmost = false;
                    this.Activate();
                    break;
                case UI_CONTROL.RefreshMyplayList:
                    myPlayList.DataContext= null;
                        myPlayList.DataContext = myPlayListCollection;
                    break;
            }
        }

        private void initStoryboard()
        {
            HideLeftPanelStoryboard = this.Resources["left_panel_hide"] as Storyboard;
            ShowLeftPanelStoryboard = this.Resources["left_panel_show"] as Storyboard;
            HideTopGridStoryboard = this.Resources["Hide_top_content"] as Storyboard;
            HideTopGridStoryboard.Completed += HideTopGridStoryboard_Completed;
            ShowTopGridStoryboard = this.Resources["show_top_content"] as Storyboard;
        }

        void HideTopGridStoryboard_Completed(object sender, EventArgs e)
        {
            top_content.Children.Clear();
        }

        private void MenuBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DrawerMenu();
        }

        private void DrawerMenu()
        {
            // TODO: 여기에 구현된 이벤트 처리기를 추가하십시오.
            if (IsLeftPanelState)
            {
                HideLeftPanelStoryboard.Begin();
                IsLeftPanelState = false;
            }
            else
            {

                ShowLeftPanelStoryboard.Begin();
                IsLeftPanelState = true;
            }
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            webBrowser.Visibility= Visibility.Visible;
        }

        private void Search_Button_Click(object sender, RoutedEventArgs e)
        {
            SearchOperation();
            morebtn.Visibility = Visibility.Hidden;
           // MessageBox.Show(resultURL + " " + resultTitle);
        }

        private void SearchOperation(){

            //MessageBox.Show(resultURL + " " + resultTitle);
            if (IsLeftPanelState)
            {

                HideLeftPanelStoryboard.Begin();
                IsLeftPanelState = false;
            }

            Orange.Util.UI_Flag.IsChart = false;
            main_page.Visibility = Visibility.Visible;
            main_page.SetProgressRing(true, 0);
            Search_ScrollViewer.ScrollToHome();
            queryString = searchBox.Text.ToString();
            cur_page = 1;
            
            musicCollection.Clear();

            Thread thread = new Thread(new ThreadStart(SearchingThread));
            thread.Start();

        }


        private void SearchingThread()
        {
            try
            {

                string url = "http://115.71.236.224:8081/searchMusicVideoInformationForPage?query=";
                string query = url + queryString +"&page=" + cur_page;
                JsonArrayCollection items = JSONHelper.getJSONArray(query);
                
                if (items.Count > 0)
                {
                    //MessageBox.Show(col.Count.ToString());

                    Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
                    {
                        
                        foreach(JsonObjectCollection item in items)
                        {
                            
                            string resultURL = item["url"].GetValue().ToString().Replace("http://www.youtube.com/watch?v=", "");
                            string resultPlayTime = item["time"].GetValue().ToString();
                            string resultTitle = item["title"].GetValue().ToString();
                            MusicItem mitem = new MusicItem();
                            mitem.title = resultTitle;
                            mitem.playTime = resultPlayTime;
                            mitem.url = resultURL;
                            musicCollection.Add(mitem);
                        }                      

                        main_page.SetProgressRing(false, 0);
                        main_page.Visibility = Visibility.Collapsed;

                    }));

                }
                else
                {
                    Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
                    {
                        MessageBox.Show("검색 결과가 없습니다");
                        main_page.SetProgressRing(false, 0);
                        main_page.Visibility = Visibility.Visible;

                    }));

                }
            }catch(Exception e)
            { MessageBox.Show(e.Message);
                  Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
                    {
            main_page.SetProgressRing(false, 0);
            main_page.Visibility = Visibility.Visible;
                    }));

            }          
       
        }

        private void morebtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            main_page.Visibility = Visibility.Visible;
            main_page.SetProgressRing(true, 1);
            cur_page++;


           

            Thread thread = new Thread(new ThreadStart(SearchingThread));
            thread.Start();
// 
//             MusicItem mitem = new MusicItem();
//             mitem.title = "test";
//             mitem.playTime = "33:33";
//             mitem.url = "test";
//             musicCollection.Add(mitem);
//             musicCollection.Add(mitem);
//             musicCollection.Add(mitem);
//             musicCollection.Add(mitem);
//             musicCollection.Add(mitem);
//             musicCollection.Add(mitem);
//             musicCollection.Add(mitem);
// 
//             sbMax++;
        }


        private void webBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        public void HideScriptErrors(WebBrowser wb, bool Hide)
        {
            FieldInfo fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fiComWebBrowser == null) return;
            object objComWebBrowser = fiComWebBrowser.GetValue(wb);
            if (objComWebBrowser == null) return;
            objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { Hide });
        }

        private void webBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            HideScriptErrors(webBrowser, true);

            check = new CheckBeforeClosing(new AppInfo(this));
            check.CheckUpdate();
         
        }
             

        private void searchBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
        	if(e.Key == Key.Enter)
            {
                SearchOperation();
            }
        }

        #region The result of searching

        private void Load_Music_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            result_musiclist.SelectedItem = (e.OriginalSource as FrameworkElement).DataContext;
            if (result_musiclist.SelectedIndex != -1)
            {

                MusicItem item = (MusicItem)result_musiclist.SelectedItem;
                PlayMusic(item);
                //MessageBox.Show(item.title);

            }
        }

        private void ADD_PlayList_Click(object sender, System.Windows.RoutedEventArgs e)
        {            
            result_musiclist.SelectedItem = (e.OriginalSource as FrameworkElement).DataContext;

            if (result_musiclist.SelectedIndex != -1)
            {
                MusicItem item = (MusicItem)result_musiclist.SelectedItem;
                myPlayListCollection.Add(item);

            }
        }


        private void result_selected_add(object sender, RoutedEventArgs e)
        {
            if (result_musiclist.SelectedIndex != -1)
            {
               foreach (MusicItem item in result_musiclist.SelectedItems)
               {
                   myPlayListCollection.Add(item);

                   
               }
               result_musiclist.SelectedIndex = -1;
            }
        }

        private void result_all_select(object sender, System.Windows.RoutedEventArgs e)
        {
            result_musiclist.SelectAll();
        }

        private void result_musiclist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = ((FrameworkElement)e.OriginalSource).DataContext as MusicItem;
            if (item != null)
            {
                //MessageBox.Show(item.title);
               // myPlayListCollection.Add(item);
                PlayMusic(item);

                //MessageBox.Show(item.title + " " + item.url);
            }
        }
        #endregion


        #region play list event
        private void Show_Video_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            myPlayList.SelectedItem = (e.OriginalSource as FrameworkElement).DataContext;

            if (myPlayList.SelectedIndex != -1)
            {
                MusicItem item = (MusicItem)myPlayList.SelectedItem;

   
                    webBrowser.Visibility = Visibility.Visible;
                    PlayMusic(item);
   
                
                //string delimiter = "http://www.youtube.com/watch?v=";
               // string s = item.url.Replace("http://www.youtube.com/watch?v=","");
                //MessageBox.Show(target);


            }
        }

        private void myPlayList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = ((FrameworkElement)e.OriginalSource).DataContext as MusicItem;
            if (item != null)
            {
               // MessageBox.Show("Item's Double Click handled!");
                PlayMusic(item);

                CopyPlayList();
                
            }

        }


        private void Lyric_PlayList_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            myPlayList.SelectedItem = (e.OriginalSource as FrameworkElement).DataContext;

            if (myPlayList.SelectedIndex != -1)
            {
                MusicItem item = (MusicItem)myPlayList.SelectedItem;

                //MessageBox.Show(item.title);
                //myPlayListCollection.Add(item);
             
            }

        }

        private void URL_Copy_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            myPlayList.SelectedItem = (e.OriginalSource as FrameworkElement).DataContext;

            if (myPlayList.SelectedIndex != -1)
            {
                MusicItem item = (MusicItem)myPlayList.SelectedItem;

                string strURL = "http://www.youtube.com/watch?v=" + item.url;
                Clipboard.SetText(strURL);

                MessageBox.Show(strURL);
                //myPlayListCollection.Add(item);

            }

        }

        private void MP3_Convert_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            myPlayList.SelectedItem = (e.OriginalSource as FrameworkElement).DataContext;

            if (myPlayList.SelectedIndex != -1)
            {
                  var dialog = new System.Windows.Forms.FolderBrowserDialog();
                  System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if(result == System.Windows.Forms.DialogResult.OK)
                {

                    MusicItem item = (MusicItem)myPlayList.SelectedItem;
                    ConvertMP3.PATH = dialog.SelectedPath;
                    //MessageBox.Show(path);

                    ConvertMP3.URL = "http://www.youtube.com/watch?v=" + item.url;

                    MsgBroker.MsgBrokerMsg arg = new MsgBroker.MsgBrokerMsg();
                    arg.MsgOPCode = UI_CONTROL.SHOW_TOP_GRID;
                    arg.MsgBody = new ConvertingProgress();
                    (Application.Current as App).msgBroker.SendMessage(arg);


                    ConvertMP3.worker(this, ConvertMP3.URL, ConvertMP3.PATH, WKIND.CONVERT);
                 
                  
                }
                  
                // string mUrl = "http://www.youtube.com/watch?v="+ item.url;

               // ConvertMP3.worker(mUrl, )

                //MessageBox.Show(item.title);
                //myPlayListCollection.Add(item);

            }

        }




        #endregion


        #region player controller

        private void play(object sender, RoutedEventArgs e)
        {
            //var document = webBrowser.Document;
            //webBrowser.Document.GetType().InvokeMember("pauseVideo", BindingFlags.InvokeMethod, null, document, null);
            if(Player_State.IsPlaying)
            {
                webBrowser.InvokeScript("playVideo");
                dt.Start();
            }
            else
            {
                if (myPlayList.SelectedIndex != -1)
                {

                    MusicItem item = (MusicItem)myPlayList.SelectedItem;
                    PlayMusic(item);
                    //MessageBox.Show(item.title);
                    webBrowser.InvokeScript("playVideo");
                    dt.Start();
                    Player_State.IsPlaying = true;
                }                
            }
            
           
            //PlayBtn.Template = (ControlTemplate)FindResource("PauseButtonControlTemplate");
        }

        private void pause(object sender, System.Windows.RoutedEventArgs e)
        {
            webBrowser.InvokeScript("pauseVideo");
            dt.Stop();
        }

        private void Next_Music(object sender, RoutedEventArgs e)
        {
            EndMusic();
        }

        private void Previous_music(object sender, RoutedEventArgs e)
        {
            dt.Stop();

            if(Config.REPEAT==2)
            {
                PlayMusic(CurrentItem);
                return;
            }

            for (int i = 0; i < playlist.Count; i++)
            {
                 
                
                if (playlist[i].Equals(CurrentItem))
                 {

                        if (i != 0)
                        {
                           CurrentItem = playlist[i - 1];
                        }else if(i==0)
                        {
                            CurrentItem = playlist[playlist.Count-1];
                        }

                        PlayMusic(CurrentItem);
                        return;
                    }
            }
            
        }

        private void set_shuffle(object sender, RoutedEventArgs e)
        {
            if(Config.IsShffle)
            {
                Config.IsShffle = false;
                ShuffleBtn.Template = (ControlTemplate)FindResource("NonShuffleButtonControlTemplate");
                ShuffleBtn.ToolTip = "순차듣기";
            }
            else
            {
                Config.IsShffle = true;                
                ShuffleBtn.Template = (ControlTemplate)FindResource("ShuffleButtonControlTemplate");
                ShuffleBtn.ToolTip = "섞어듣기";
            }
            CopyPlayList();
        }

        private void set_repeat(object sender, RoutedEventArgs e)
        {
            
            // DEFAULT 0, ALL REPEAT 1, SINGLE REPEAT 2
            switch ((++Config.REPEAT)%3)
            {
                case 0:
                    repeatBtn.Template = (ControlTemplate)FindResource("DefaultRepeatButtonControlTemplate");
                    repeatBtn.ToolTip = "반복없음";
                    Config.REPEAT = 0;
                    break;
                case 1:
                    repeatBtn.Template = (ControlTemplate)FindResource("RepeatButtonControlTemplate");
                    repeatBtn.ToolTip = "전체반복";
                    Config.REPEAT = 1;
                    break;
                case 2:
                    repeatBtn.Template = (ControlTemplate)FindResource("SingleRepeatButtonControlTemplate");
                    repeatBtn.ToolTip = "한곡만 반복";
                    Config.REPEAT = 2;
                    break;
            }
             

        }

        private void Show_video_in_control(object sender, RoutedEventArgs e)
        {
            if(webBrowser.Visibility == Visibility.Visible)
            {
                webBrowser.Visibility = Visibility.Hidden;
                ShowVideoBtn.Content = "ShowVideo";
                UI_Flag.IsShowingVideo = false;

            }else{
                webBrowser.Visibility = Visibility.Visible;
                ShowVideoBtn.Content = "HideVideo";
                UI_Flag.IsShowingVideo = true;
            }            
        }



        private void Mute(object sender, System.Windows.RoutedEventArgs e)
        {
            if (webBrowser.IsLoaded && Player_State.IsPlaying)
            {
                if (Config.IsMute)
                {
                    Config.IsMute = false;
                    VolumeBtn.Template = (ControlTemplate)FindResource("VolButtonControlTemplate");

                    webBrowser.InvokeScript("unMute");
                }
                else
                {
                    Config.IsMute = true;
                    VolumeBtn.Template = (ControlTemplate)FindResource("MuteButtonControlTemplate");
                    webBrowser.InvokeScript("mute");
                }
            }
        }
        #endregion

        #region playlist event controller
        private void top_list(object sender, RoutedEventArgs e)
        {
            if (myPlayList.SelectedIndex != -1)
            {
                MusicItem item = (MusicItem)myPlayList.SelectedItem;


               // MessageBox.Show(item.title);
                myPlayListCollection.Remove(item);
                myPlayListCollection.Insert(0, item);

                myPlayList.SelectedItem = item;
            }
        }

        private void up_list(object sender, RoutedEventArgs e)
        {
            int idx = myPlayList.SelectedIndex;
            if (myPlayList.SelectedIndex != -1)
            {
                if (myPlayList.SelectedIndex == 0)
                    return;

                MusicItem item = (MusicItem)myPlayList.SelectedItem;


                // MessageBox.Show(item.title);
                myPlayListCollection.Remove(item);
                myPlayListCollection.Insert(idx-1, item);

                myPlayList.SelectedItem = item;
            }
        }

        private void down_list(object sender, RoutedEventArgs e)
        {
            int idx = myPlayList.SelectedIndex;
            if (myPlayList.SelectedIndex != -1)
            {
                if (myPlayList.SelectedIndex == myPlayListCollection.Count-1)
                    return;

                MusicItem item = (MusicItem)myPlayList.SelectedItem;


                // MessageBox.Show(item.title);
                myPlayListCollection.Remove(item);
                myPlayListCollection.Insert(idx + 1, item);

                myPlayList.SelectedItem = item;
            }
        }

        private void bottom_list(object sender, RoutedEventArgs e)
        {
            int idx = myPlayList.SelectedIndex;
            if (myPlayList.SelectedIndex != -1)
            {
                if (myPlayList.SelectedIndex == myPlayListCollection.Count-1)
                    return;

                MusicItem item = (MusicItem)myPlayList.SelectedItem;


                // MessageBox.Show(item.title);
                myPlayListCollection.Remove(item);
                myPlayListCollection.Insert(myPlayListCollection.Count, item);

                myPlayList.SelectedItem = item;
            }
        }

        private void save_list(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfDialog = new SaveFileDialog();
            sfDialog.FileName = "Untitled";
            sfDialog.Title = "Save";
            sfDialog.Filter = "orm files (*.orm)|*.orm";
            sfDialog.OverwritePrompt = true;
            sfDialog.CheckPathExists = true;

            
            if (sfDialog.ShowDialog() == true)
            {

                File.WriteAllText(sfDialog.FileName, Security.Encrypt(Newtonsoft.Json.JsonConvert.SerializeObject(myPlayListCollection)));
            }
        }

        private void open_list(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofDialog = new OpenFileDialog();
            Nullable<bool> result = ofDialog.ShowDialog();
            ofDialog.DefaultExt = "*.orm";
            ofDialog.Filter = "orm files (*.orm)|*.orm";


            if (result == true)
            {
                
                string fileName = ofDialog.FileName;
                try
                {
                    string json = Security.Decrypt(File.ReadAllText(fileName));
                    var playerList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MusicItem>>(json);
                    myPlayListCollection.Clear();
                    foreach (var it in playerList)
                    {
                        myPlayListCollection.Add(it);
                    }
                }catch(Exception)
                {
                    MessageBox.Show("파일 형식이 맞지 않습니다");
                }           
                                
            }
        }


        private void delete_list(object sender, RoutedEventArgs e)
        {
            if (myPlayList.SelectedIndex != -1)
            {
                List<MusicItem> it = new List<MusicItem>();
                foreach (MusicItem item in myPlayList.SelectedItems)
                {
                    it.Add(item);
                }
                foreach (MusicItem item in it)
                {
                    myPlayListCollection.Remove(item);
                }
                it.Clear();
            }
        }
        #endregion



        #region Slider Controller
        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!dragStarted)
            {
                if (webBrowser.IsLoaded && Player_State.IsPlaying && !Config.IsMute)
                {
                    webBrowser.InvokeScript("setVolume", new String[] { VolumeSlider.Value.ToString() });
                    Player_State.VolumeValue = VolumeSlider.Value.ToString();
                }
                
            }

            
        }
        private void VolumeSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {

            if (webBrowser.IsLoaded && Player_State.IsPlaying && !Config.IsMute && IsActive)
            {
                webBrowser.InvokeScript("setVolume", new String[] { VolumeSlider.Value.ToString() });
                Player_State.VolumeValue = VolumeSlider.Value.ToString();
            }
            this.dragStarted = false;

        }

        private void VolumeSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            this.dragStarted = true;
        }


        
        private void PlayerSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            
                //MessageBox.Show("DragCompleted");
                //webBrowser.InvokeScript("loadVideoById", new String[] { PlayerSlider.Value.ToString() });
                webBrowser.InvokeScript("seekTo", new String[] { PlayerSlider.Value.ToString() });
                Player_State.IsPlaying = true;
                dt.Start();

            this.dragStarted = false;
        }

        private void PlayerSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!dragStarted)
            {
//                 webBrowser.InvokeScript("seekTo", new String[] { PlayerSlider.Value.ToString() });
//                 Player_State.IsPlaying = true;
//                 dt.Start();
            }
        }

        private void PlayerSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
                this.dragStarted = true;
                dt.Stop();           
        }
        

        private void PlayerSlider_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if(!dragStarted)
            {
                dt.Stop();
               // e.Handled = true;
            }      

        }

        private void PlayerSlider_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!dragStarted)
            {
                webBrowser.InvokeScript("seekTo", new String[] { PlayerSlider.Value.ToString() });
                Player_State.IsPlaying = true;
                dt.Start();
                e.Handled = true;
            }      
        }



        #endregion

        private void PlayMusic(MusicItem item)
        {
            try {
                if (UI_Flag.IsShowingVideo)
                {
                    webBrowser.Visibility = Visibility.Visible;
                }
                else { webBrowser.Visibility = Visibility.Hidden; }
               
                WebBrowserHelper.ClearCache();
                
                webBrowser.InvokeScript("loadVideoById", new String[] { item.url });
             
                dt.Start();
                Player_State.IsPlaying = true;
                webBrowser.InvokeScript("setVolume", new String[] { Player_State.VolumeValue });

                CurrentItem = item;
                Music_title.Text = item.title;
                SelectCurrentMusicItemInPlayList(item);
            }
            catch (Exception e) { 
                MessageBox.Show("다시 시도 해보세요.\n\n" + e.Message.ToString());    
                
            }
            
        }



        private void Information_Click(object sender, RoutedEventArgs e)
        {
            //information_uc.Visibility = Visibility.Visible;
            

            MsgBroker.MsgBrokerMsg arg = new MsgBroker.MsgBrokerMsg();
            arg.MsgOPCode = UI_CONTROL.SHOW_TOP_GRID;
            arg.MsgBody = new information_usercontrol(this);
            (Application.Current as App).msgBroker.SendMessage(arg);
        }

        private void myPlayList_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.Delete:
                    if (myPlayList.SelectedIndex != -1)
                    {
                        List<MusicItem> it = new List<MusicItem>();
                       // myPlayList.DataContext = null;
                        foreach (MusicItem item in myPlayList.SelectedItems)
                        {
                            it.Add(item);                          
                        }                       
                        foreach (MusicItem item in it)
                        {
                            myPlayListCollection.Remove(item);
                        }
                        it.Clear();
                    }
                    break;
                case Key.Up:
                    if (myPlayList.SelectedIndex > 0 )
                    {                        
                        myPlayList.SelectedIndex = myPlayList.SelectedIndex - 1;                        
                    }
                    break;
                case Key.Down:
                    if (myPlayList.SelectedIndex != -1 || myPlayList.SelectedIndex != (myPlayListCollection.Count-1) )
                    {
                        myPlayList.SelectedIndex = myPlayList.SelectedIndex +1;
                    }
                    break;
            }

        }


        private void Search_ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void test_click(object sender, RoutedEventArgs e)
        {
            string msg = "";
            for(int i=0; i<playlist.Count; i++)
            {
                msg += playlist[i].title + " | ";
            }

            MessageBox.Show(msg);
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (check.IsAvailableUpdate)
            {
                UpdateWindow updatewin = new UpdateWindow();
                updatewin.initSmartUpdate(new AppInfo(this));
                bool result = (bool)updatewin.ShowDialog();

                if(result==true)
                {

                }
                else if (result == false)
                {
                    //MessageBox.Show("the update download was cancelled.");
                }
            }

           // SaveTempList();
            SaveConfig();
            musicCollection.Clear();
            myPlayListCollection.Clear();
            WebBrowserHelper.ClearCache();
            webBrowser.Dispose();
            (Application.Current as App).msgBroker.MessageReceived -= msgBroker_MessageReceived;

        }
        private void LoadConfig()
        {
            Config.IsShffle = Properties.Settings.Default.IsShffle;
            if (!Config.IsShffle)
            {
                ShuffleBtn.Template = (ControlTemplate)FindResource("NonShuffleButtonControlTemplate");
                ShuffleBtn.ToolTip = "순차듣기";

                
            }
            else
            {
                ShuffleBtn.Template = (ControlTemplate)FindResource("ShuffleButtonControlTemplate");
                ShuffleBtn.ToolTip = "섞어듣기";
            }

            Config.REPEAT = Properties.Settings.Default.REPEAT;
            switch (Config.REPEAT)
            {
                case 0:
                    repeatBtn.Template = (ControlTemplate)FindResource("DefaultRepeatButtonControlTemplate");
                    repeatBtn.ToolTip = "반복없음";
                    break;
                case 1:
                    repeatBtn.Template = (ControlTemplate)FindResource("RepeatButtonControlTemplate");
                    repeatBtn.ToolTip = "전체반복";
                    break;
                case 2:
                    repeatBtn.Template = (ControlTemplate)FindResource("SingleRepeatButtonControlTemplate");
                    repeatBtn.ToolTip = "한곡만 반복";
                    break;
            }
            Config.IsMute = Properties.Settings.Default.IsMute;
            Config.Language_for_Orange = Properties.Settings.Default.Language_for_Orange;
            
//             UI_Flag.IsShowingTopGrid = Properties.Settings.Default.IsShowingTopGrid;
//             UI_Flag.IsShowingVideo = Properties.Settings.Default.IsShowingVideo;
//             
//             Player_State.IsPlaying = Properties.Settings.Default.IsPlaying;
//             Player_State.IsEndOfPoint = Properties.Settings.Default.IsEndOfPoint;
//             Player_State.VolumeValue = Properties.Settings.Default.VolumeValue;

//             string fileName = System.AppDomain.CurrentDomain.BaseDirectory + "templist.tmp";
//             if(File.Exists(fileName))
//             {
//                 try
//                 {
//                     string json = Security.Decrypt(File.ReadAllText(fileName));
//                     var playerList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MusicItem>>(json);
//                     myPlayListCollection.Clear();
//                     foreach (var it in playerList)
//                     {
//                         myPlayListCollection.Add(it);
//                     }
//                 }
//                 catch (Exception e)
//                 {
//                     MessageBox.Show(e.Message);
//                 }       
//             }
            if (!Properties.Settings.Default.Playlist.Equals(""))
            {
                try
                {
                    string json = Security.Decrypt(Properties.Settings.Default.Playlist);
                    var playerList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MusicItem>>(json);
                    myPlayListCollection.Clear();
                    foreach (var it in playerList)
                    {
                        myPlayListCollection.Add(it);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }    
            }
             
            
        }

        private void SaveConfig()
        {
       
            Properties.Settings.Default.IsShffle = Config.IsShffle;
            Properties.Settings.Default.REPEAT =  Config.REPEAT;
            Properties.Settings.Default.IsMute =  Config.IsMute;
            Properties.Settings.Default.Language_for_Orange = Config.Language_for_Orange;
            Properties.Settings.Default.Playlist =Security.Encrypt(Newtonsoft.Json.JsonConvert.SerializeObject(myPlayListCollection));
            Properties.Settings.Default.Save();
        }

        private void SaveTempList()
        {
            File.WriteAllText(System.AppDomain.CurrentDomain.BaseDirectory + "templist.tmp", Security.Encrypt(Newtonsoft.Json.JsonConvert.SerializeObject(myPlayListCollection)));
        }

        private void Config_Click(object sender, RoutedEventArgs e)
        {
            MsgBroker.MsgBrokerMsg arg = new MsgBroker.MsgBrokerMsg();
            arg.MsgOPCode = UI_CONTROL.SHOW_TOP_GRID;
            arg.MsgBody = new Preferences();
            (Application.Current as App).msgBroker.SendMessage(arg);
        }



        private void currentTagNotContactsList_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            ScrollBar sb = e.OriginalSource as ScrollBar;

            if (sb.Orientation == Orientation.Horizontal)
                return;

            if (sb.Value == sb.Maximum && !UI_Flag.IsChart)
            {
                morebtn.Visibility = Visibility.Visible;
            }          
        }

        private void Bn_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(Config.GIFT_URL);
        }

        private void OnListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Handled)
                return;

            ListViewItem item = Orange.Util.VisualTreeHelper.FindParent<ListViewItem>((DependencyObject)e.OriginalSource);

            
            if (item == null)
                return;
            if (item.Focusable && !item.IsFocused)
                item.Focus();


            MessageBox.Show("test");
        }


        private void Remove_PlaylistItem_Click(object sender, RoutedEventArgs e)
        {
            if (myPlayList.SelectedIndex != -1)
            {
                List<MusicItem> it = new List<MusicItem>();
                foreach (MusicItem item in myPlayList.SelectedItems)
                {
                    it.Add(item);
                }
                foreach (MusicItem item in it)
                {
                    myPlayListCollection.Remove(item);
                }
                it.Clear();
            }
        }

        private void Rename_PlaylistItem_Click(object sender, RoutedEventArgs e)
        {
            if (myPlayList.SelectedIndex != -1)
            {                
                MusicItem item = (MusicItem)myPlayList.SelectedItem;
                MsgBroker.MsgBrokerMsg arg = new MsgBroker.MsgBrokerMsg();
                arg.MsgOPCode = UI_CONTROL.SHOW_TOP_GRID;
                arg.MsgBody = new rename_usercontrol(item);
                (Application.Current as App).msgBroker.SendMessage(arg);
            }
        }
    }
}
