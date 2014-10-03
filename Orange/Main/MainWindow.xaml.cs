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
using Orange.Main.DialogUserControls;
using System.Diagnostics;

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
        private bool initSuccess = false;
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
        private MyfavoritelistMgr myFavoriteMgr;

        private DispatcherTimer Showvideodt;
        private DispatcherTimer dt;

        private System.Windows.Forms.NotifyIcon notify;
        private System.Windows.Forms.MenuItem NotifyCurrentItem;
        private int cur_page;

        public MainWindow()
        {
            InitializeComponent();
            init_progress.Visibility = Visibility.Visible;
            
            initStoryboard();
            initUserConfig();

            myFavoriteMgr = MyfavoritelistMgr.instance();
            musicCollection = new MusicCollection();
            myPlayListCollection = new MusicCollection();
            check = new CheckBeforeClosing(new AppInfo(this));
            LoadConfig();
            initLanguage();
            main_menu.SetMusicCollection(musicCollection);
           
            result_musiclist.DataContext = musicCollection;
            myPlayList.DataContext = myPlayListCollection;
            main_page.InitFavoriteListview(myPlayListCollection);

            webBrowser.Navigated += webBrowser_Navigated;
                        
            WebBrowserHelper.ClearCache();

            
            webBrowser.Navigate(URL.YOUTUBEPLAYER_URL);


            (Application.Current as App).msgBroker.MessageReceived += msgBroker_MessageReceived;

            //String volume = webBrowser.InvokeScript("getVolume").ToString();
            dt = new DispatcherTimer();
            dt.Interval = new TimeSpan(0, 0, 10);
            dt.Tick += dt_Tick;
            dt.Start();
            myPlayListCollection.CollectionChanged += myPlayListCollection_CollectionChanged;

            

        }

        public void onReadyWebBrowser()
        {
            
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
                catch (Exception ex)
                {
                    MessageBoxOrange.ShowDialog("Exception", ex.Message);
                }
            }
            InitTextBlock.Visibility = Visibility.Collapsed;
            loadingTextBlock.Visibility = Visibility.Visible;
            init_progress.Visibility = Visibility.Collapsed;

            Configbtn.IsEnabled = true;
            Informationbtn.IsEnabled = true;

            if (!Properties.Settings.Default.IsFirst)
            {
                Showvideodt.Start();
            }
            else
            {
                tutorialgrid.Children.Add(new tutorial());
            }

            initSuccess = true;
            dt.Stop();
            dt.IsEnabled = false;

            initNotify();
        }

        public void onReadyWebBrowser_reload()
        {           
            init_progress.Visibility = Visibility.Collapsed;
            Configbtn.IsEnabled = true;
            Informationbtn.IsEnabled = true;

            webBrowser.InvokeScript("loadVideoById", new String[] { CurrentItem.url });

            if (NotifyCurrentItem.Checked)
                setBalloonTip(CurrentItem.title);
            Player_State.playCount++;

            Player_State.IsPlaying = true;
            webBrowser.InvokeScript("setVolume", new String[] { Player_State.VolumeValue });

            if (Config.IsMute)
                webBrowser.InvokeScript("Mute");

            Music_title.Text = CurrentItem.title;
            SelectCurrentMusicItemInPlayList(CurrentItem);
            myPlayList.ScrollIntoView(CurrentItem);
            webBrowser.Visibility = Visibility.Visible;
        }
    



        private void initLanguage()
        {
            Previousbtn.ToolTip = LanguagePack.Previous();
            Nextbtn.ToolTip = LanguagePack.Next();
            PlayBtn.ToolTip = LanguagePack.l_Play();
            pauseBtn.ToolTip = LanguagePack.l_Pause();
            savebtn.ToolTip = LanguagePack.Save();
            openbtn.ToolTip = LanguagePack.Open();
            deletebtn.ToolTip = LanguagePack.Delete_item();
            bookmarkbtn.ToolTip = LanguagePack.SetBookMark();
            morebtn.Content = LanguagePack.MoreItems();
            ms_list_top.ToolTip = LanguagePack.PlaylistControl_MoveToTop();
            ms_list_up.ToolTip = LanguagePack.PlaylistControl_Previous();
            ms_list_down.ToolTip = LanguagePack.PlaylistControl_Next();
            ms_list_bottom.ToolTip = LanguagePack.PlaylistControl_SkipToLast();
            
            if(NotifyCurrentItem!=null)
                NotifyCurrentItem.Text = LanguagePack.NotifyText();

            selectallitemsbtn.Content = LanguagePack.SelectAllItems();
            addselecteditembtn.Content = LanguagePack.AddSelectedItems();

            if (Config.IsShffle)
            {
                ShuffleBtn.ToolTip = LanguagePack.Shuffle();
            }
            else
            {
                ShuffleBtn.ToolTip = LanguagePack.PlayStraight();
            }


            // DEFAULT 0, ALL REPEAT 1, SINGLE REPEAT 2
            switch (Properties.Settings.Default.REPEAT)
            {
                case 0:
                    //repeatBtn.ToolTip = LanguagePack.NormalMode();
                    //break;
                    Properties.Settings.Default.REPEAT = 1;
                    repeatBtn.ToolTip = LanguagePack.RepeatAllSongs();
                    break;
                case 1:
                    repeatBtn.ToolTip = LanguagePack.RepeatAllSongs();
                    break;
                case 2:
                    repeatBtn.ToolTip = LanguagePack.RepeatOneSong();
                    break;
            }

            main_menu.SetMenuLanguage();
            

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
            if(!initSuccess)
            {
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
                    catch (Exception ex)
                    {
                        MessageBoxOrange.ShowDialog("Exception", ex.Message);
                    }
                }

                init_progress.Visibility = Visibility.Collapsed;
                Configbtn.IsEnabled = true;
                Informationbtn.IsEnabled = true;

                if (!Properties.Settings.Default.IsFirst)
                {
                    Showvideodt.Start();
                }
                else
                {
                    tutorialgrid.Children.Add(new tutorial());
                }
            }

            //MessageBoxOrange.ShowDialog("Warning", "You need to install adobe flash player");

            dt.Stop();
            dt.IsEnabled = false;
           
        }


        public void SetDurationtime(string currrentTime, string durationTime)
        {

            if (!dragStarted)
            {
                Double totalTime = Double.Parse(durationTime);
                Double currentTime = Double.Parse(currrentTime);
                

                Console.WriteLine("{0} | {1}", currentTime, totalTime);

                if (totalTime != 0.0)
                {
                    PlayerSlider.Maximum = totalTime;
                    PlayerSlider.Value = currentTime;

                    TimeSpan ctime = TimeSpan.FromSeconds(currentTime);
                    TimeSpan endtime = TimeSpan.FromSeconds(totalTime);

                    //currentTimeTxb.Text = ctime.ToString(@"mm\:ss");
                    //endTimeTxb.Text = endtime.ToString(@"mm\:ss");
                    currentTimeTxb.Text = ConvertTimespanToString.ToReadableString(ctime);
                    endTimeTxb.Text = "/" + ConvertTimespanToString.ToReadableString(endtime);
// 
                    if (totalTime == currentTime)
                    {
                        NextMusic();
                    }
                }

            }
        }

        public void NextMusic()
        {
            if (CurrentItem == null)
                return;
           // MessageBoxOrange.ShowDialog("끗");
            dt.Stop();

            if (Properties.Settings.Default.REPEAT <= 1)
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
//             else if (Config.REPEAT == 0)
//             {
//                 for (int i = 0; i < playlist.Count; i++)
//                 {
//                     if (playlist[i].Equals(CurrentItem))
//                     {
// 
//                         if (i != playlist.Count - 1)
//                         {
//                             CurrentItem = playlist[i + 1];
//                         }
//                         else
//                         {
//                             Player_State.IsPlaying = false;
//                             return;
//                         }
// 
//                         PlayMusic(CurrentItem);
//                         return;
//                     }
//                 }
//             }
            else if (Properties.Settings.Default.REPEAT == 2)
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
            {
                MessageBoxOrange.ShowDialog("Warning", e.Message.ToString());
            }
            
        }

        private void initNotify ()
        {
            try
            {
                System.Windows.Forms.ContextMenu menu = new System.Windows.Forms.ContextMenu();

                System.Windows.Forms.MenuItem item_Open = new System.Windows.Forms.MenuItem();
                item_Open.Index = 0;
                item_Open.Text = "Activate Orange";
                item_Open.Click += item1_Open_Click;

                NotifyCurrentItem = new System.Windows.Forms.MenuItem();
                NotifyCurrentItem.Index = 1;
                NotifyCurrentItem.Checked = Properties.Settings.Default.IsNotifyInfo;
                NotifyCurrentItem.Text = LanguagePack.NotifyText();
                NotifyCurrentItem.Click += delegate(object sender, EventArgs args)
                {
                    if (NotifyCurrentItem.Checked) { NotifyCurrentItem.Checked = false; }
                    else { NotifyCurrentItem.Checked = true; }
                };

                System.Windows.Forms.MenuItem item_Previous = new System.Windows.Forms.MenuItem();
                item_Previous.Index = 2;
                item_Previous.Text = "Previous";
                item_Previous.Click += delegate(object sender, EventArgs args)
                {
                    Previous_item();
                };
                
                System.Windows.Forms.MenuItem item_Next = new System.Windows.Forms.MenuItem();
                item_Next.Index = 3;
                item_Next.Text = "Next";
                item_Next.Click += delegate(object sender, EventArgs args)
                {
                    NextMusic();
                };

                System.Windows.Forms.MenuItem item_Play = new System.Windows.Forms.MenuItem();
                item_Play.Index = 4;
                item_Play.Text = "Play";
                item_Play.Click += delegate(object sender, EventArgs args) {
                    if (Player_State.IsPlaying)
                    {
                        webBrowser.InvokeScript("playVideo");

                    }
                    else
                    {
                        if (myPlayList.SelectedIndex != -1)
                        {

                            MusicItem item = (MusicItem)myPlayList.SelectedItem;
                            PlayMusic(item);
                            //MessageBoxOrange.ShowDialog(item.title);
                            webBrowser.InvokeScript("playVideo");

                            Player_State.IsPlaying = true;
                        }
                    }
            

                };

                System.Windows.Forms.MenuItem item_Pause = new System.Windows.Forms.MenuItem();
                item_Pause.Index = 5;
                item_Pause.Text = "Pause";
                item_Pause.Click += delegate(object sender, EventArgs args) { webBrowser.InvokeScript("pauseVideo"); };

                System.Windows.Forms.MenuItem item_Close = new System.Windows.Forms.MenuItem();
                item_Close.Index = 6;
                item_Close.Text = "Exit";
                item_Close.Click += item_Close_Click;

                //menu.MenuItems.Add(item_Open);
                menu.MenuItems.Add(NotifyCurrentItem);
                menu.MenuItems.Add("-");
                menu.MenuItems.Add(item_Previous);
                menu.MenuItems.Add(item_Next);
                menu.MenuItems.Add(item_Play);
                menu.MenuItems.Add(item_Pause);
                menu.MenuItems.Add("-");
                menu.MenuItems.Add(item_Close);

                notify = new System.Windows.Forms.NotifyIcon();
                Stream iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/Orange;component/icon.ico")).Stream;
                notify.Icon = new System.Drawing.Icon(iconStream); ;
                notify.ContextMenu = menu;
                notify.Visible = true;

                if (Properties.Settings.Default.IsTray)
                {
                    notify.Visible = true;
                }
                else
                {
                    notify.Visible = false;
                }
                notify.DoubleClick += notify_DoubleClick;
                setBalloonTip("Welcome to Orange player");
            }
            catch (Exception ex){
                MessageBoxOrange.ShowDialog("Exception", ex.ToString());
            }
        }

        void setBalloonTip(String msg)
        {
            if(notify!=null)
            {
                notify.BalloonTipTitle = "Orange YOUTUBE player";
                notify.BalloonTipText = msg;
                notify.ShowBalloonTip(2000);
            }
            
        }

        void notify_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            Activate();
            this.WindowState = WindowState.Normal;
            
        }

        void item_Close_Click(object sender, EventArgs e)
        {
            this.Hide();      
            Close();
        }

        void item1_Open_Click(object sender, EventArgs e)
        {
            this.Show();
            Activate();
            this.WindowState = WindowState.Normal;
        }

        void msgBroker_MessageReceived(object sender, MsgBroker.MsgBrokerEventArgs e)
        {
            switch(e.Message.MsgOPCode)
            {
                case MESSAGE_MAP.UPLOAD_PLAYLIST:

                    uploadplaylist((string)e.Message.MsgBody);

                    break;
                case MESSAGE_MAP.CREATE_FAVORITE_PLAYLIST:

                    Create_favorite_playlist((string)e.Message.MsgBody);

                    break;

                case MESSAGE_MAP.LOAD_ITEMS_IN_FAVORITE_PLAYLIST:

                    Load_favorite_playlist((string)e.Message.MsgBody);

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
                        morebtn.Visibility = Visibility.Collapsed;
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

                        if(UI_Flag.IsShowingVideo && Player_State.IsPlaying && !main_menu.IsFavoritePanel)
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
                case UI_CONTROL.SET_INIT_AFTER_TUTORIAL:
                    initLanguage();
                    Properties.Settings.Default.IsFirst = false;
                    Showvideodt.Start();
                    tutorialgrid.Children.Clear();
                    tutorialgrid.Visibility = Visibility.Collapsed;
                    
                    break;
                case UI_CONTROL.SET_LANGUAGE:
                    initLanguage();
                    break;
                case UI_CONTROL.ACTIVEMOREBTN:
                    morebtn.Visibility = Visibility.Visible;
                    break;
                case UI_CONTROL.SHOW_VIDEO:
                    if (UI_Flag.IsShowingVideo && Player_State.IsPlaying)
                    {
                        webBrowser.Visibility = Visibility.Visible;
                    }
                    
                    break;
                case UI_CONTROL.HIDE_VIDEO:
                    webBrowser.Visibility = Visibility.Hidden;
                    break;

                case UI_CONTROL.OPEN_DRAWERMENU:
                    DrawerMenu();
                    break;

                case UI_CONTROL.ACTIVETRAY:
                    notify.Visible = true;
                    setBalloonTip(LanguagePack.TrayActivateText());
                    break;
                case UI_CONTROL.DEACTIVETRAY:
                    notify.Visible = false;
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
                main_menu.HideFavoriteList();
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
            webBrowser.Visibility = Visibility.Visible;
        }

        private void Search_Button_Click(object sender, RoutedEventArgs e)
        {
            SearchOperation();
            morebtn.Visibility = Visibility.Collapsed;
           // MessageBoxOrange.ShowDialog(resultURL + " " + resultTitle);
        }

        private void SearchOperation(){

            //MessageBoxOrange.ShowDialog(resultURL + " " + resultTitle);
            if (IsLeftPanelState)
            {

                HideLeftPanelStoryboard.Begin();
                IsLeftPanelState = false;
            }

            Orange.Util.UI_Flag.IsChart = false;
            main_page.Visibility = Visibility.Visible;
        
            if (searchBox.Text.ToString().Trim().Equals(""))
            {
                return;
            }


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
                    //MessageBoxOrange.ShowDialog(col.Count.ToString());

                    Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
                    {
                        
                        foreach(JsonObjectCollection item in items)
                        {
                            
                            string resultURL = item["url"].GetValue().ToString().Replace("http://www.youtube.com/watch?v=", "");
                            //string resultPlayTime = ConvertTimespanToString.ToReadableString(item["time"].GetValue().ToString());
                            string resultPlayTime = item["time"].GetValue().ToString();
                            string resultTitle = item["title"].GetValue().ToString();
                            MusicItem mitem = new MusicItem();
                            mitem.title = resultTitle;
                            mitem.time = resultPlayTime;
                            mitem.url = resultURL;
                            musicCollection.Add(mitem);

                           
                        }
                        if (items.Count >= 17)
                        {
                            morebtn.Visibility = Visibility.Visible;
                        }
                        main_page.SetProgressRing(false, 0);
                        main_page.Visibility = Visibility.Collapsed;

                    }));

                }
                else
                {
                    Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
                    {
                        MessageBoxOrange.ShowDialog("Warning", "There is no result");
                        
                        main_page.SetProgressRing(false, 0);
                        main_page.Visibility = Visibility.Visible;

                    }));

                }
            }catch(Exception e)
            { 
                  Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
                    {
                        MessageBoxOrange.ShowDialog("Exception", e.Message);
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

        }


        private void webBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            ((WebBrowser)sender).ObjectForScripting = new HtmlInteropClass();
             Showvideodt = new DispatcherTimer();
             Showvideodt.Interval = new TimeSpan(0, 0, 0, 0, 200);
             Showvideodt.Tick += Showvideodt_Tick;

             
        }

        void Showvideodt_Tick(object sender, EventArgs e)
        {
            webBrowser.Visibility = Visibility.Visible;
            Showvideodt.Stop();
            Showvideodt.IsEnabled = false;
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
                //MessageBoxOrange.ShowDialog(item.title);

            }
        }

        private void ADD_PlayList_Click(object sender, System.Windows.RoutedEventArgs e)
        {            
            result_musiclist.SelectedItem = (e.OriginalSource as FrameworkElement).DataContext;

            if (result_musiclist.SelectedIndex != -1)
            {
                MusicItem item = (MusicItem)result_musiclist.SelectedItem;
                myPlayListCollection.Add(item);
                myPlayList.SelectedItem = item;
                myPlayList.ScrollIntoView(item);
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
                //MessageBoxOrange.ShowDialog(item.title);
                myPlayListCollection.Add(item);
                PlayMusic(item);

                //MessageBoxOrange.ShowDialog(item.title + " " + item.url);
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
                //MessageBoxOrange.ShowDialog(target);


            }
        }

        private void myPlayList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = ((FrameworkElement)e.OriginalSource).DataContext as MusicItem;
            if (item != null)
            {
               // MessageBoxOrange.ShowDialog("Item's Double Click handled!");
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

                //MessageBoxOrange.ShowDialog(item.title);
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

                MessageBoxOrange.ShowDialog("This address was copied", strURL);
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
                    //MessageBoxOrange.ShowDialog(path);

                    ConvertMP3.URL = "http://www.youtube.com/watch?v=" + item.url;

                    MsgBroker.MsgBrokerMsg arg = new MsgBroker.MsgBrokerMsg();
                    arg.MsgOPCode = UI_CONTROL.SHOW_TOP_GRID;
                    arg.MsgBody = new ConvertingProgress();
                    (Application.Current as App).msgBroker.SendMessage(arg);


                    ConvertMP3.worker(this, ConvertMP3.URL, ConvertMP3.PATH, WKIND.CONVERT);
                 
                  
                }
                  
                // string mUrl = "http://www.youtube.com/watch?v="+ item.url;

               // ConvertMP3.worker(mUrl, )

                //MessageBoxOrange.ShowDialog(item.title);
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
                
            }
            else
            {
                if (myPlayList.SelectedIndex != -1)
                {

                    MusicItem item = (MusicItem)myPlayList.SelectedItem;
                    PlayMusic(item);
                    //MessageBoxOrange.ShowDialog(item.title);
                    webBrowser.InvokeScript("playVideo");
                
                    Player_State.IsPlaying = true;
                }                
            }
            
           
            //PlayBtn.Template = (ControlTemplate)FindResource("PauseButtonControlTemplate");
        }

        private void pause(object sender, System.Windows.RoutedEventArgs e)
        {
            webBrowser.InvokeScript("pauseVideo");
            
        }

        private void Next_Music(object sender, RoutedEventArgs e)
        {
            NextMusic();
        }

        private void Previous_music(object sender, RoutedEventArgs e)
        {


            Previous_item();
            
        }

        private void Previous_item()
        {
            if (CurrentItem == null)
                return;
            if (Properties.Settings.Default.REPEAT == 2)
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
                    }
                    else if (i == 0)
                    {
                        CurrentItem = playlist[playlist.Count - 1];
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
                ShuffleBtn.ToolTip = LanguagePack.PlayStraight();
            }
            else
            {
                Config.IsShffle = true;                
                ShuffleBtn.Template = (ControlTemplate)FindResource("ShuffleButtonControlTemplate");
                ShuffleBtn.ToolTip = LanguagePack.Shuffle();
            }
            CopyPlayList();
        }

        private void set_repeat(object sender, RoutedEventArgs e)
        {
            
            // DEFAULT 0, ALL REPEAT 1, SINGLE REPEAT 2
//             switch ((++Config.REPEAT)%3)
//             {
//                 case 0:
//                     repeatBtn.Template = (ControlTemplate)FindResource("DefaultRepeatButtonControlTemplate");
//                     repeatBtn.ToolTip = LanguagePack.NormalMode();
//                     Config.REPEAT = 0;
//                     break;
//                 case 1:
//                     repeatBtn.Template = (ControlTemplate)FindResource("RepeatButtonControlTemplate");
//                     repeatBtn.ToolTip = LanguagePack.RepeatAllSongs();
//                     Config.REPEAT = 1;
//                     break;
//                 case 2:
//                     repeatBtn.Template = (ControlTemplate)FindResource("SingleRepeatButtonControlTemplate");
//                     repeatBtn.ToolTip = LanguagePack.RepeatOneSong();
//                     Config.REPEAT = 2;
//                     break;
//             }

            if (Properties.Settings.Default.REPEAT == 1)
            {
                repeatBtn.Template = (ControlTemplate)FindResource("SingleRepeatButtonControlTemplate");
                repeatBtn.ToolTip = LanguagePack.RepeatOneSong();

                Properties.Settings.Default.REPEAT = 2;
            }
            else if (Properties.Settings.Default.REPEAT == 2)
            {
                repeatBtn.Template = (ControlTemplate)FindResource("DefaultRepeatButtonControlTemplate");
                repeatBtn.ToolTip = LanguagePack.RepeatAllSongs();


                Properties.Settings.Default.REPEAT = 1;
            
            }
             

        }

        private void Show_video_in_control(object sender, RoutedEventArgs e)
        {
            if (webBrowser.Visibility == Visibility.Visible)
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
            if (Player_State.IsPlaying)
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


               // MessageBoxOrange.ShowDialog(item.title);
                myPlayListCollection.Remove(item);
                myPlayListCollection.Insert(0, item);

                myPlayList.SelectedItem = item;

                myPlayList.ScrollIntoView(item);
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


                // MessageBoxOrange.ShowDialog(item.title);
                myPlayListCollection.Remove(item);
                myPlayListCollection.Insert(idx-1, item);

                myPlayList.SelectedItem = item;

                myPlayList.ScrollIntoView(item);
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


                // MessageBoxOrange.ShowDialog(item.title);
                myPlayListCollection.Remove(item);
                myPlayListCollection.Insert(idx + 1, item);

                myPlayList.SelectedItem = item;
                myPlayList.ScrollIntoView(item);
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


                // MessageBoxOrange.ShowDialog(item.title);
                myPlayListCollection.Remove(item);
                myPlayListCollection.Insert(myPlayListCollection.Count, item);

                myPlayList.SelectedItem = item;
                myPlayList.ScrollIntoView(item);
            }
        }

        private void save_list(object sender, RoutedEventArgs e)
        {
            
            string fn = DateTime.Now.ToString("yyyyMMdd_HHmmss");    //2013-07-16 14:10:26
            SaveFileDialog sfDialog = new SaveFileDialog();
            sfDialog.FileName = fn;
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

                    MessageBoxOrange.ShowDialog("Warning","File type is not correct");
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
                if (Player_State.IsPlaying && !Config.IsMute)
                {
                    webBrowser.InvokeScript("setVolume", new String[] { VolumeSlider.Value.ToString() });
                    Player_State.VolumeValue = VolumeSlider.Value.ToString();
                }
                
            }

            
        }
        private void VolumeSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {

            if (Player_State.IsPlaying && !Config.IsMute && IsActive)
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
            if (dragStarted)
            {
                webBrowser.InvokeScript("seekTo", new String[] { PlayerSlider.Value.ToString() });
                Player_State.IsPlaying = true;

            }
            this.dragStarted = false;
        }

        private void PlayerSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!dragStarted)
            {

            }

        }
        private void PlayerSlider_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.dragStarted = true;
        }

        private void PlayerSlider_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.dragStarted = true;
            }
        }


        private void PlayerSlider_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (dragStarted)
            {
                webBrowser.InvokeScript("seekTo", new String[] { PlayerSlider.Value.ToString() });
                Player_State.IsPlaying = true;

            }
            this.dragStarted = false;
        }



        private void PlayerSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
                 this.dragStarted = true;
      
        }
        

        #endregion


        
        private void PlayMusic(MusicItem item)
        {
            try {


                if (item == null)
                    return;

                if (UI_Flag.IsShowingVideo && !main_menu.IsFavoritePanel)
                {
                    if (top_content.Children.Count == 0)
                    {
                        webBrowser.Visibility = Visibility.Visible;
                    }
                    
                }
                else if (!UI_Flag.IsShowingVideo || main_menu.IsFavoritePanel)
                { webBrowser.Visibility = Visibility.Hidden; }


                if (Player_State.playCount == 30)
                {
                   
                    webBrowser.Visibility = Visibility.Hidden;
                    WebBrowserHelper.ClearCache();
                    //webBrowser.Refresh();
                    webBrowser.Navigate(URL.YOUTUBEPLAYER_URL_RELOAD);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    Player_State.playCount = 0;

                    Console.WriteLine("Refresh");

                    init_progress.Visibility = Visibility.Visible; 
                    Configbtn.IsEnabled = false;
                    Informationbtn.IsEnabled = false;
                    CurrentItem = item;
                }
                else
                {
                    webBrowser.InvokeScript("loadVideoById", new String[] { item.url });
                    if (NotifyCurrentItem.Checked)
                        setBalloonTip(item.title);


                    Player_State.playCount++;

                    Player_State.IsPlaying = true;
                    webBrowser.InvokeScript("setVolume", new String[] { Player_State.VolumeValue });

                    if (Config.IsMute)
                        webBrowser.InvokeScript("Mute");

                    CurrentItem = item;
                    Music_title.Text = item.title;
                    SelectCurrentMusicItemInPlayList(item);
                    myPlayList.ScrollIntoView(item);
                }




            }
            catch (Exception e) {
                MessageBoxOrange.ShowDialog("Warning", "Try again.\n\n" + e.Message.ToString());
                
                
            }
            
        }



        private void Information_Click(object sender, RoutedEventArgs e)
        {
            //information_uc.Visibility = Visibility.Visible;
            
            if(!Properties.Settings.Default.IsFirst)
            {
                MsgBroker.MsgBrokerMsg arg = new MsgBroker.MsgBrokerMsg();
                arg.MsgOPCode = UI_CONTROL.SHOW_TOP_GRID;
                arg.MsgBody = new information_usercontrol(this);
                (Application.Current as App).msgBroker.SendMessage(arg);
            }
            
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
                case Key.Enter:
                    if (myPlayList.SelectedIndex != -1)
                    {
                        PlayMusic((MusicItem)myPlayList.SelectedItem);
                        CopyPlayList();
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

            MessageBoxOrange.ShowDialog("Warning",msg);
        }
            

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            
            
            if (Properties.Settings.Default.IsTray && this.Visibility == Visibility.Visible) {

                this.Hide();

                setBalloonTip(LanguagePack.TrayActivateText());
                e.Cancel = true;    
    
            }
            else
            {
                ExitProcessing();
            }

            
        }

        private void ExitProcessing()
        {
            if (check.IsAvailableUpdate)
            {

                UpdateWindow updatewin = new UpdateWindow();
                updatewin.initSmartUpdate(new AppInfo(this));
                bool result = (bool)updatewin.ShowDialog();

                if (result == true)
                {

                }
                else if (result == false)
                {
                    //MessageBoxOrange.ShowDialog("the update download was cancelled.");
                }
            }
            notify.Visible = false;
            notify.Dispose();
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
            Config.Language_for_Orange = Properties.Settings.Default.Language_for_Orange;
            LanguagePack.TYPE = Config.Language_for_Orange;
            Config.IsShffle = Properties.Settings.Default.IsShffle;
            if (!Config.IsShffle)
            {
                ShuffleBtn.Template = (ControlTemplate)FindResource("NonShuffleButtonControlTemplate");
                ShuffleBtn.ToolTip = LanguagePack.PlayStraight();               
            }
            else
            {
                ShuffleBtn.Template = (ControlTemplate)FindResource("ShuffleButtonControlTemplate");
                ShuffleBtn.ToolTip = LanguagePack.Shuffle();
            }

            switch (Properties.Settings.Default.REPEAT)
            {
                case 0:
                    Properties.Settings.Default.REPEAT = 1;
                    repeatBtn.Template = (ControlTemplate)FindResource("DefaultRepeatButtonControlTemplate");
                    repeatBtn.ToolTip = LanguagePack.RepeatAllSongs();
                    break;
                
                case 1:
                    repeatBtn.Template = (ControlTemplate)FindResource("DefaultRepeatButtonControlTemplate");
                    repeatBtn.ToolTip = LanguagePack.RepeatAllSongs();
                    break;
                case 2:
                    repeatBtn.Template = (ControlTemplate)FindResource("SingleRepeatButtonControlTemplate");
                    repeatBtn.ToolTip = LanguagePack.RepeatOneSong();
                    break;
            }
            Config.IsMute = Properties.Settings.Default.IsMute;
            

               try
                {
                    String path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    string filedirectory = path + @"\OrangePlayer\favoritePlaylist";
                        
                    //string filepath = string.Format("{0}/{1}.orm", filedirectory, filename);

                        // 지정한 경로에 폴더가 존재 하는지 확인 합니다.
                    if (System.IO.Directory.Exists(filedirectory))
                    {
                              // DirectoryInfo Class 를 생성 합니다.
                              System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(filedirectory);
                             // foreach 구문을 이용하여 폴더 내부에 있는 파일정보를 가져옵니다.
                             foreach (var item in di.GetFiles())
                             {
                                 if(item.Extension.Equals(".orm"))
                                 {
                                     PlaylistItem plist = new PlaylistItem();

                                     plist.name = item.Name.Remove(item.Name.Length-4);
                                     plist.filePath = string.Format("{0}/{1}.orm", filedirectory, item.Name);
                                     myFavoriteMgr.MyfavoriteCollection.Add(plist);
                                 }
//                                 // 파일 이름 출력
//                                     Console.WriteLine("FILE NAME : " + item.Name);
//                                 // 파일 타입 (확장자) 출력
//                                 Console.WriteLine("FILE TYPE : " + item.Extension);
//                                 // 파일 생성날짜 출력
//                                 Console.WriteLine("CREATE DATE : " + item.CreationTime);                   
                             }
                    }
               }catch(Exception ex)
               {
                    MessageBoxOrange.ShowDialog("Exception",ex.Message);
               }
//                     
// 
// 
// 
//             if (File.Exists(filepath))
//             {
// 
//                     string json = Security.Decrypt(Properties.Settings.Default.Playlist);
//                     var playerList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PlaylistItem>>(json);
//                     myFavoriteMgr.MyfavoriteCollection.Clear();
//                     
//                     
//                     string filedirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\OrangePlayer\favoritePlaylist";
//                     
//                     foreach (var it in playerList)
//                     {
//                         string filepath = string.Format("{0}/{1}.orm", filedirectory, it.name);
//                         if(File.Exists(filepath))
//                             myFavoriteMgr.MyfavoriteCollection.Add(it);
//                     }
//                 }
//                 catch (Exception e)
//                 {
//                     MessageBoxOrange.ShowDialog("Exception",e.Message);
//                 }    
            
             
            
        }

        private void SaveConfig()
        {
            //myFavoriteMgr.MyfavoriteCollection.Clear();
            Properties.Settings.Default.IsShffle = Config.IsShffle;
            
            Properties.Settings.Default.IsMute =  Config.IsMute;
            Properties.Settings.Default.Language_for_Orange = Orange.Util.LanguagePack.TYPE;

            Properties.Settings.Default.Playlist = Security.Encrypt(Newtonsoft.Json.JsonConvert.SerializeObject(myPlayListCollection));
            Properties.Settings.Default.Save();
        }


        private void Config_Click(object sender, RoutedEventArgs e)
        {
            if(!Properties.Settings.Default.IsFirst)
            {
                MsgBroker.MsgBrokerMsg arg = new MsgBroker.MsgBrokerMsg();
                arg.MsgOPCode = UI_CONTROL.SHOW_TOP_GRID;
                arg.MsgBody = new Preferences();
                (Application.Current as App).msgBroker.SendMessage(arg);
            }

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
            System.Diagnostics.Process.Start(URL.GIFT_URL);
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


            //MessageBoxOrange.ShowDialog("test");
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


        private void Create_favorite_playlist(string filename)
        {
            try
            {
                
                String path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string filedirectory = path + @"\OrangePlayer\favoritePlaylist";
                if (!Directory.Exists(filedirectory))
                    Directory.CreateDirectory(filedirectory);


                string filepath = string.Format("{0}/{1}.orm", filedirectory, filename);
                //System.AppDomain.CurrentDomain.BaseDirectory 
                //System.IO.Directory.GetCurrentDirectory();
                File.WriteAllText(filepath, Security.Encrypt(Newtonsoft.Json.JsonConvert.SerializeObject(myPlayListCollection)));

                if (File.Exists(filepath))
                {
                    PlaylistItem item = new PlaylistItem();
                    item.name = filename;
                    item.filePath = filepath;

                    myFavoriteMgr.MyfavoriteCollection.Add(item);

                }
                else { MessageBoxOrange.ShowDialog("Warning", "failed to create the file"); }
            }catch(Exception ex)
            {
                MessageBoxOrange.ShowDialog("Exception", ex.Message);
            }
            
        }

        private void Load_favorite_playlist(string filename)
        {
            String path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filedirectory = path + @"\OrangePlayer\favoritePlaylist";
                        
            string filepath = string.Format("{0}/{1}.orm", filedirectory, filename);


            if (File.Exists(filepath))
            {
                try
                {
                    string json = Security.Decrypt(File.ReadAllText(filepath));
                    var playerList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MusicItem>>(json);
                    myPlayListCollection.Clear();
                    foreach (var it in playerList)
                    {
                        myPlayListCollection.Add(it);
                    }
                    MessageBoxOrange.ShowDialog("SUCCESS", "Playlist was successfully loaded");
                    
                }
                catch (Exception)
                {
                    MessageBoxOrange.ShowDialog("Warning", "File type is not correct");
                    
                }

            }
            else
            {
                MessageBoxOrange.ShowDialog("Warning", "failed to create the file");
            }
                

        }

        private void createfavoritelist_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (myPlayListCollection.Count == 0)
            {
                MessageBoxOrange.ShowDialog("Warning","Playlist is empty.");
                return;
            }
                
            MsgBroker.MsgBrokerMsg arg = new MsgBroker.MsgBrokerMsg();
            arg.MsgOPCode = UI_CONTROL.SHOW_TOP_GRID;
            arg.MsgBody = new Create_FavoritePlaylistUserControl();
            (Application.Current as App).msgBroker.SendMessage(arg);
            main_menu.ShowFavoriteList();
            DrawerMenu();
            
        	
        }

        private void uploadplaylist(string title)
        {
            Playlist myChart = new Playlist();
            myChart.chart_name = title;
            myChart.chart_list = myPlayListCollection.ToList();
            if(myChart.chart_list.Count==0)
            {
                MessageBoxOrange.ShowDialog("Warning", "Upload ERROR");
                
                return;
            }

            string uri = @"http://115.71.236.224:8081/uploadPlayList";
            string request_data = Security.Encrypt(Newtonsoft.Json.JsonConvert.SerializeObject(myChart));

            WebClient webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
            webClient.Encoding = UTF8Encoding.UTF8;

            try
            {
                string responseJson = webClient.UploadString(uri, request_data);
                if (responseJson.Equals("True"))
                {
                    MessageBoxOrange.ShowDialog("Notice", "Your list has successfully uploaded");
                    main_menu.RecentPlaylist();
                }

                    
            }
            catch (Exception ex)
            {
                MessageBoxOrange.ShowDialog("Warning", ex.Message.ToString());
            }
        }

        public void ShowNoti(string s)
        {
            MessageBoxOrange.ShowDialog("Notification", s);
        }

        private void fullscreen_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MessageBoxOrange.ShowDialog("Full Screen", "Step 1 : Move the mouse point on the center of the video \n Step 2 : Double Click!!");
        }

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if(quality_combobox.SelectedIndex != -1)
            {
                ComboBoxItem qItem = (ComboBoxItem)quality_combobox.SelectedItem;
            //    MessageBox.Show(qItem.Content.ToString());
                if(Player_State.IsPlaying)
                {
                    webBrowser.InvokeScript("setQuality", new String[] { quality_combobox.SelectedValue.ToString() });
                }
                //
            }
        }

        private void videooption_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	if(!UI_Flag.IsVideoOption)
            {
                videooptionGrid.Visibility = Visibility.Visible;
                UI_Flag.IsVideoOption = true;
            }
            else
            {
                videooptionGrid.Visibility = Visibility.Hidden;
                UI_Flag.IsVideoOption = false;
            }
        }



 





    }

    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class HtmlInteropClass
    {
        public void Test(int type, string current, string duration)
        {
            switch(type)
            {
                case 0:
                    MessageBoxOrange.ShowDialog("tesT", "끝");
                    break;
                case 1:
                    Console.WriteLine("{0} | {1}", current, duration);
                    break;
            }
            
            //((MainWindow)Application.Current.MainWindow).resultTxb.Text = SomeValue;
        }

        public void SetSeekbar(string current, string duration)
        {            
             ((MainWindow)Application.Current.MainWindow).SetDurationtime(current, duration);
        }

        public void PlayerENDED()
        {
          
            ((MainWindow)Application.Current.MainWindow).NextMusic();
        }

        public void Notification_msg(string text)
        {
            ((MainWindow)Application.Current.MainWindow).ShowNoti(text);
        }

        public void OnReady()
        {
           
            ((MainWindow)Application.Current.MainWindow).onReadyWebBrowser();
        }

        public void OnReady_reload()
        {

            ((MainWindow)Application.Current.MainWindow).onReadyWebBrowser_reload();
        }

    }

}
