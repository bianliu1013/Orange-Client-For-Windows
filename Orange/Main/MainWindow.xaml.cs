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
        private Storyboard HideLeftPanelStoryboard;
        private Storyboard ShowLeftPanelStoryboard;
        private string queryString;
        
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

            String volume = webBrowser.InvokeScript("getVolume").ToString();
            
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

                MessageBox.Show(item.title);

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
                MessageBox.Show("Item's Double Click handled!");
                

                //MessageBox.Show(item.title);
                myPlayListCollection.Add(item);
                WebBrowserHelper.ClearCache();

                webBrowser.InvokeScript("loadVideoById", new String[] { item.url });
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
                    WebBrowserHelper.ClearCache();
                
                    webBrowser.InvokeScript("loadVideoById", new String[] { item.url });
                    
   
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
                MessageBox.Show("Item's Double Click handled!");
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

        }

        private void Previous_music(object sender, RoutedEventArgs e)
        {

        }

        private void set_shuffle(object sender, RoutedEventArgs e)
        {

        }

        private void set_repeat(object sender, RoutedEventArgs e)
        {

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

        }

        private void open_list(object sender, RoutedEventArgs e)
        {

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

        private void MetroWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            musicCollection.Clear();
            myPlayListCollection.Clear();
            WebBrowserHelper.ClearCache();
            webBrowser.Dispose();
            (Application.Current as App).msgBroker.MessageReceived -= msgBroker_MessageReceived;

        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void PlayerSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {            
            this.dragStarted = false;
            MessageBox.Show("DragCompleted");
        }

        private void PlayerSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!dragStarted)
            {
                MessageBox.Show("Dragging");
            }
        }

        private void PlayerSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            this.dragStarted = true;
        }



    }
}
