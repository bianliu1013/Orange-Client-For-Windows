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

        private Storyboard HideLeftPanelStoryboard;
        private Storyboard ShowLeftPanelStoryboard;
        private string queryString;
        private DispatcherTimer dt;

        public MainWindow()
        {
            InitializeComponent();
            initStoryboard();
            musicCollection = new MusicCollection();
            myPlayListCollection = new MusicCollection();

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

        void myPlayListCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            playlist.Clear();
            CopyPlayList();
        }

        void dt_Tick(object sender, EventArgs e)
        {
            if (!dragStarted)
            {
                double totalTime = Double.Parse(webBrowser.InvokeScript("getDuration").ToString());
                double currentTime = Double.Parse(webBrowser.InvokeScript("getCurrentTime").ToString());


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

        private void CopyPlayList()
        {            
            playlist = myPlayListCollection.ToList();

            if(Config.IsShffle)
                playlist = Shuffle.Randomize(playlist);
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
                      main_page.SetProgressRing(true);
                    break;
                case UI_CONTROL.PROGRESS_HIDE:
                     main_page.SetProgressRing(false);
                     main_page.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void initStoryboard()
        {
            HideLeftPanelStoryboard = this.Resources["left_panel_hide"] as Storyboard;
            ShowLeftPanelStoryboard = this.Resources["left_panel_show"] as Storyboard;
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
           // MessageBox.Show(resultURL + " " + resultTitle);
        }

        private void SearchOperation(){

            //MessageBox.Show(resultURL + " " + resultTitle);
            if (IsLeftPanelState)
            {

                HideLeftPanelStoryboard.Begin();
                IsLeftPanelState = false;
            }


            main_page.Visibility = Visibility.Visible;
            main_page.SetProgressRing(true);

            queryString = searchBox.Text.ToString();
            Thread thread = new Thread(new ThreadStart(SearchingThread));
            thread.Start();

        }


        private void SearchingThread()
        {
            try
            {
                string url = "http://115.71.236.224:8081/searchMusicVideoInformation?query=";
         
                string query = url + queryString;
                JsonArrayCollection items = JSONHelper.getJSONArray(query);


                if (items.Count > 0)
                {
                    //MessageBox.Show(col.Count.ToString());


                    Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
                    {
                        musicCollection.Clear();
                        foreach(JsonObjectCollection item in items)
                        {
                            
                            string resultURL = item["url"].GetValue().ToString().Replace("http://www.youtube.com/watch?v=", "");
                            //string resultPlayTime = item["playTime"].GetValue().ToString();
                            string resultTitle = item["title"].GetValue().ToString();
                            MusicItem mitem = new MusicItem();
                            mitem.title = resultTitle;
                            mitem.playTime = "00:00";
                            mitem.url = resultURL;
                            musicCollection.Add(mitem);

                        }                      

                        main_page.SetProgressRing(false);
                        main_page.Visibility = Visibility.Collapsed;

                    }));

                }
                else
                {
                    Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
                    {
                        MessageBox.Show("검색 결과가 없습니다");
                        main_page.SetProgressRing(false);
                        main_page.Visibility = Visibility.Visible;

                    }));

                }
            }catch(Exception e)
            { MessageBox.Show(e.Message);
                  Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
                    {
            main_page.SetProgressRing(false);
            main_page.Visibility = Visibility.Visible;
                    }));

            }          
       
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

                //MessageBox.Show(item.title);
                myPlayListCollection.Add(item);
            }
        }

        private void result_musiclist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = ((FrameworkElement)e.OriginalSource).DataContext as MusicItem;
            if (item != null)
            {
               // MessageBox.Show("Item's Double Click handled!");
                

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
            result_musiclist.SelectedItem = (e.OriginalSource as FrameworkElement).DataContext;

            if (result_musiclist.SelectedIndex != -1)
            {
                MusicItem item = (MusicItem)result_musiclist.SelectedItem;

                //MessageBox.Show(item.title);



   
                    webBrowser.Visibility = Visibility.Visible;
                    PlayMusic(item);
   
                //string target = item.url;
                
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
            }

        }


        private void Lyric_PlayList_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            result_musiclist.SelectedItem = (e.OriginalSource as FrameworkElement).DataContext;

            if (result_musiclist.SelectedIndex != -1)
            {
                MusicItem item = (MusicItem)result_musiclist.SelectedItem;

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
            webBrowser.InvokeScript("playVideo");


            //PlayBtn.Template = (ControlTemplate)FindResource("PauseButtonControlTemplate");
        }

        private void pause(object sender, System.Windows.RoutedEventArgs e)
        {
            webBrowser.InvokeScript("pauseVideo");
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
            }
            else
            {
                Config.IsShffle = true;                
                ShuffleBtn.Template = (ControlTemplate)FindResource("ShuffleButtonControlTemplate");
            }
        }

        private void set_repeat(object sender, RoutedEventArgs e)
        {
            // DEFAULT 0, ALL REPEAT 1, SINGLE REPEAT 2
            switch (++Config.REPEAT)
            {
                case 0:
                    repeatBtn.Template = (ControlTemplate)FindResource("DefaultRepeatButtonControlTemplate");
                    break;
                case 1:
                    repeatBtn.Template = (ControlTemplate)FindResource("RepeatButtonControlTemplate");
                    break;
                case 2:
                    repeatBtn.Template = (ControlTemplate)FindResource("SingleRepeatButtonControlTemplate");
                    break;
            }
        }

        private void Show_video_in_control(object sender, RoutedEventArgs e)
        {
            if(webBrowser.Visibility == Visibility.Visible)
            {
                webBrowser.Visibility = Visibility.Hidden;
                ShowVideoBtn.Content = "ShowVideo";
            }else{
                webBrowser.Visibility = Visibility.Visible;
                ShowVideoBtn.Content = "HideVideo";
            }            
        }



        private void Mute(object sender, System.Windows.RoutedEventArgs e)
        {
            if (webBrowser.IsLoaded)
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
                MusicItem item = (MusicItem)myPlayList.SelectedItem;
                myPlayListCollection.Remove(item);
            }
        }
        #endregion



        #region Slider Controller
        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (webBrowser.IsLoaded)
            {
                webBrowser.InvokeScript("setVolume", new String[] { VolumeSlider.Value.ToString() }); 
            }
            
        }

        private void PlayerSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {            
            this.dragStarted = false;
            //MessageBox.Show("DragCompleted");
            //webBrowser.InvokeScript("loadVideoById", new String[] { PlayerSlider.Value.ToString() });
            webBrowser.InvokeScript("seekTo", new String[] { PlayerSlider.Value.ToString() });
            dt.Start();
        }

        private void PlayerSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!dragStarted)
            {
                //MessageBox.Show("Dragging");
            }
        }

        private void PlayerSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            this.dragStarted = true;
            dt.Stop();           

        }

        #endregion

        private void PlayMusic(MusicItem item)
        {
            WebBrowserHelper.ClearCache();

            webBrowser.InvokeScript("loadVideoById", new String[] { item.url });
            dt.Start();

            CurrentItem = item;
            
        }

        private void MetroWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            musicCollection.Clear();
            myPlayListCollection.Clear();
            WebBrowserHelper.ClearCache();
            webBrowser.Dispose();
            (Application.Current as App).msgBroker.MessageReceived -= msgBroker_MessageReceived;

        }
       
    }
}
