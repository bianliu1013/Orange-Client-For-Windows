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

namespace Orange
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]

    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private bool IsLeftPanelState = false;
 

        private MusicCollection musicCollection;
        private Storyboard HideLeftPanelStoryboard;
        private Storyboard ShowLeftPanelStoryboard;
        private string queryString;
        public MainWindow()
        {
            InitializeComponent();
            initStoryboard();
            musicCollection = new MusicCollection();
            main_menu.SetMusicCollection(musicCollection);
           
            result_musiclist.DataContext = musicCollection;

          //  webBrowser.Navigated += webBrowser_Navigated;
          //  webBrowser.Navigate("http://115.71.236.224:8081/static/YouTubePlayer.html");
            (Application.Current as App).msgBroker.MessageReceived += msgBroker_MessageReceived;
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
                            string resultURL = item["url"].GetValue().ToString();
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


        private void play(object sender, RoutedEventArgs e)
        {
            //var document = webBrowser.Document;
            //webBrowser.Document.GetType().InvokeMember("pauseVideo", BindingFlags.InvokeMethod, null, document, null);
            webBrowser.InvokeScript("pauseVideo");
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

        private void Load_Music_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (result_musiclist.SelectedIndex != -1)
            {

                MusicItem item = (MusicItem)result_musiclist.SelectedItem;

                MessageBox.Show(item.title);

            }
        }

        private void ADD_PlayList_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //ListViewItem selected = sender as ListViewItem;
            

            return;
            if (result_musiclist.SelectedIndex != -1)
            {

                MusicItem item = (MusicItem)result_musiclist.SelectedItem;

                MessageBox.Show(item.title);

            }
        }
    }
}
