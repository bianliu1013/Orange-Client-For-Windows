using System;
using System.Collections.Generic;
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
using System.Net;
using System.Net.Json;
using System.IO;
using Orange.MsgBroker;
using System.Windows.Threading;
using Orange.DataManager;
using System.Threading;
using System.Windows.Controls.Primitives;
using Orange.Util;
using Orange.Main.DialogUserControls;

namespace Orange
{
	/// <summary>
	/// main_menuControl.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class main_menuControl : UserControl
	{
        private int cur_page;
        private string query;
        private string queryString;
        private string url;
        private string url_yotube;
        private MusicCollection musicCollection;
        private PlaylistCollection playCollection;
        private MyfavoritelistMgr myFavoriteMgr;
        private MsgBroker.MsgBrokerMsg arg;
        public bool IsFavoritePanel { get; set; }
		public main_menuControl()
		{
            this.InitializeComponent();
            IsFavoritePanel = false;
            arg = new MsgBroker.MsgBrokerMsg();
            playCollection = new PlaylistCollection();
            myFavoriteMgr = MyfavoritelistMgr.instance();
            result_playlist.DataContext = playCollection;
            favorite_playlist.DataContext = myFavoriteMgr.MyfavoriteCollection;

            favoritelist_Content.Visibility = Visibility.Hidden;
		}

        public void SetMusicCollection(MusicCollection collection)
        {
            musicCollection = collection;
        }



		private void mn_pop_btn_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            url = "http://115.71.236.224:8081/getBillboardChart";

            musicCollection.Clear();
            MsgBroker.MsgBrokerMsg arg = new MsgBroker.MsgBrokerMsg();
            arg.MsgOPCode = UI_CONTROL.PROGRESS_SHOW;
            Orange.Util.UI_Flag.IsChart = true;
            (Application.Current as App).msgBroker.SendMessage(arg);
            Thread thread = new Thread(new ThreadStart(ParsingThread));
            thread.Start();
		}

		private void mn_kpop_btn_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            url = "http://115.71.236.224:8081/getMelonChart";

            musicCollection.Clear();
            MsgBroker.MsgBrokerMsg arg = new MsgBroker.MsgBrokerMsg();
            arg.MsgOPCode = UI_CONTROL.PROGRESS_SHOW;
            Orange.Util.UI_Flag.IsChart = true;
            (Application.Current as App).msgBroker.SendMessage(arg);
            
            Thread thread = new Thread(new ThreadStart(ParsingThread));
            
            thread.Start();
		}

        private void ParsingThread()
        {
            Thread.Sleep(1000);
            try
            {
                JsonArrayCollection items = JSONHelper.getJSONArray(url_yotube);

                Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
                {
                foreach (JsonObjectCollection item in items)
                {
                    string title = item["title"].GetValue().ToString();
                   
                    ////수정해야                
                    // string mUrl = item["singer"].GetValue().ToString();
                    // string playTime = item["singer"].GetValue().ToString();
                    string mUrl = item["url"].GetValue().ToString().Replace("http://www.youtube.com/watch?v=", ""); ;
                    string playTime = item["time"].GetValue().ToString();


                    MusicItem mitem = new MusicItem();
                    mitem.title = title;
                    mitem.url = mUrl;
                    mitem.time = playTime;
                    musicCollection.Add(mitem);

                  
                }

              }));

                Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
                {
                    HideDrawerMenu();

               }));
            }
            catch (Exception e) { 
                
                

               Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
               {
                   MessageBoxOrange.ShowDialog("Exception", e.Message);
                     HideDrawerMenu();
               }));
            
               }
        
           
        }

		private void mn_jpop_btn_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            url = "http://115.71.236.224:8081/getOriconChart";

            musicCollection.Clear();
            Orange.Util.UI_Flag.IsChart = true;
            arg.MsgOPCode = UI_CONTROL.PROGRESS_SHOW;
            
            (Application.Current as App).msgBroker.SendMessage(arg);
            Thread thread = new Thread(new ThreadStart(ParsingThread));
            thread.Start();
		}

		private void Search_ScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
		{
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
		}
        private void currentTagNotContactsList_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            ScrollBar sb = e.OriginalSource as ScrollBar;

            if (sb.Orientation == Orientation.Horizontal)
                return;

            if (sb.Value == sb.Maximum)
            {
                
            }
        }


        private void ParsingThread_For_addfavorite()
        {
            Thread.Sleep(1000);
            try
            {
                JsonArrayCollection items = JSONHelper.getJSONArray(url);
                MusicCollection tmp_musicCollection = new MusicCollection();
                Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
                {
                    foreach (JsonObjectCollection item in items)
                    {
                        string title = item["title"].GetValue().ToString();
                        ////수정해야                
                        // string mUrl = item["singer"].GetValue().ToString();
                        // string playTime = item["singer"].GetValue().ToString();
                        string mUrl = item["url"].GetValue().ToString().Replace("http://www.youtube.com/watch?v=", ""); ;
                        string playTime = item["time"].GetValue().ToString();


                        MusicItem mitem = new MusicItem();
                        mitem.title = title;
                        mitem.url = mUrl;
                        mitem.time = playTime;
                        tmp_musicCollection.Add(mitem);                        

                    }

                    PlaylistItem pitem = (PlaylistItem)result_playlist.SelectedItem;
                    String path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    string filedirectory = path + @"\OrangePlayer\favoritePlaylist";
                   // string filedirectory = System.AppDomain.CurrentDomain.BaseDirectory + "/favoritePlaylist";

                    if (!Directory.Exists(filedirectory))
                        Directory.CreateDirectory(filedirectory);


                    string filepath = string.Format("{0}/{1}.orm", filedirectory, pitem.name);
                    //System.AppDomain.CurrentDomain.BaseDirectory 
                    //System.IO.Directory.GetCurrentDirectory();
                    File.WriteAllText(filepath, Security.Encrypt(Newtonsoft.Json.JsonConvert.SerializeObject(tmp_musicCollection)));

                    if (File.Exists(filepath))
                    {
                        PlaylistItem item = new PlaylistItem();
                        item.name = pitem.name;
                        item.filePath = filepath;

                        myFavoriteMgr.MyfavoriteCollection.Add(item);

                    }
                    else { MessageBoxOrange.ShowDialog("Exception", "failed to create the file"); }

                    
                }));

            }
            catch (Exception e)
            {
                Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
                {
                MessageBoxOrange.ShowDialog("Exception", e.Message);

                }));
            }
            Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
            {
            ProgressRing.IsActive = false;
            progressGrid.Visibility = Visibility.Collapsed;
            }));
        }
    
        private void mn_hotlist_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            HotPlaylist();
        }

        private void mn_sharelist_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MsgBroker.MsgBrokerMsg arg = new MsgBroker.MsgBrokerMsg();
            arg.MsgOPCode = UI_CONTROL.SHOW_TOP_GRID;
            arg.MsgBody = new SharePlayListUserControl();
            (Application.Current as App).msgBroker.SendMessage(arg);
        }

        private void mn_whatsnew_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            RecentPlaylist();
        }

        private void mn_myfavoritelist_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	if(IsFavoritePanel)
            {
                HideFavoriteList();
            }
            else
            {
                //열기
                ShowFavoriteList();
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchOperation();
        }

        private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
                SearchOperation();
        }

        private void SearchOperation()
        {
            queryString = searchBox.Text;
            if (queryString.Trim().Equals(""))
                return;

            cur_page = 1;

            ProgressRing.IsActive = true;
            progressGrid.Visibility = Visibility.Visible;

            Search_ScrollViewer.ScrollToHome();

            playCollection.Clear();

            string url = "http://115.71.236.224:8081/searchPlayList?query=";
            query = url + queryString + "&page="+cur_page;

            Thread thread = new Thread(new ThreadStart(SearchingThread));
            thread.Start();
        }

        public void RecentPlaylist()
        {
            queryString = searchBox.Text;
            cur_page = 1;

            ProgressRing.IsActive = true;
            progressGrid.Visibility = Visibility.Visible;

            Search_ScrollViewer.ScrollToHome();

            url = "http://115.71.236.224:8081/getRecentPlayList?page=";
            query = url + cur_page;

            playCollection.Clear();

            Thread thread = new Thread(new ThreadStart(SearchingThread));
            thread.Start();
        }

        private void HotPlaylist()
        {
            queryString = searchBox.Text;
            cur_page = 1;

            ProgressRing.IsActive = true;
            progressGrid.Visibility = Visibility.Visible;

            Search_ScrollViewer.ScrollToHome();

            string url = "http://115.71.236.224:8081/getHighHitCountPlayList";
            query = url;

            morebtn.Visibility = Visibility.Collapsed;

            playCollection.Clear();

            Thread thread = new Thread(new ThreadStart(SearchingThread));
            thread.Start();
        }

        private void SearchingThread()
        {
            try
            {
               MusicJson playerList = Newtonsoft.Json.JsonConvert.DeserializeObject<MusicJson>(JSONHelper.getJsondata(query));
               List<JsonItem> items = playerList.play_list;
               int total_page = Int32.Parse(playerList.page_cnt);
//                Console.WriteLine("{0}", total);
//                foreach(var it in items)
//                {
//                    Console.WriteLine("{0} {1}", it.title, it.hits_count);
//                    
//                }
               if (items.Count > 0)
                {
                    //MessageBoxOrange.ShowDialog(col.Count.ToString());

                    Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
                    {

                        foreach (JsonItem item in items)
                        {

                            string resultTitle = item.title;
                            string resultHitcount = item.hits_count;
                            PlaylistItem mitem = new PlaylistItem();
                            mitem.name = resultTitle;
                            mitem.hits_count = resultHitcount;
                            playCollection.Add(mitem);
                        }
                      
                        if (cur_page < total_page)
                        {
                            morebtn.Visibility = Visibility.Visible;
                        }
                        else if (cur_page == total_page)
                        {
                            morebtn.Visibility = Visibility.Collapsed;
                        }

                        ProgressRing.IsActive = false;
                        progressGrid.Visibility = Visibility.Collapsed;

                    }));

                }
                else
                {
                    Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
                    {
                        MessageBoxOrange.ShowDialog("Warning", "There is no any the result.");
                        ProgressRing.IsActive = false;
                        progressGrid.Visibility = Visibility.Collapsed;
                    }));

                }
            }
            catch (Exception e)
            {
               
                
                Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
                {
                    MessageBoxOrange.ShowDialog("Warning", e.Message);
                    ProgressRing.IsActive = false;
                    progressGrid.Visibility = Visibility.Collapsed;
                }));
            }
        }

        private void morebtn_Click(object sender, RoutedEventArgs e)
        {
            ProgressRing.IsActive = true;
            progressGrid.Visibility = Visibility.Visible;
            cur_page++;
            query = url + cur_page;
            Thread thread = new Thread(new ThreadStart(SearchingThread));
            thread.Start();
        }



        private void Add_favorite_Click(object sender, RoutedEventArgs e)
        {
            result_playlist.SelectedItem = (e.OriginalSource as FrameworkElement).DataContext;
            if(result_playlist.SelectedIndex!=-1)
            {


                PlaylistItem item = (PlaylistItem)result_playlist.SelectedItem;

                foreach (var it in myFavoriteMgr.MyfavoriteCollection)
                {
                    if (it.name.Equals(item.name))
                    {
                        MessageBoxOrange.ShowDialog("Warning", "The item is overlap in your favorite list.");
                        
                        return;
                    }
                }
                url = "http://115.71.236.224:8081/getPlayList?title=" + item.name;
                ProgressRing.IsActive = true;
                progressGrid.Visibility = Visibility.Visible;
                Thread thread = new Thread(new ThreadStart(ParsingThread_For_addfavorite));
                thread.Start();

                if (!IsFavoritePanel)
                {
                    ShowFavoriteList();
                }
            }
            

            
        }

        private void Load_list_Click(object sender, RoutedEventArgs e)
        {
            result_playlist.SelectedItem = (e.OriginalSource as FrameworkElement).DataContext;
            if(result_playlist.SelectedIndex!=-1)
            {
                PlaylistItem item = (PlaylistItem)result_playlist.SelectedItem;
                Load_Playlist(item.name);

            }
            
        }

        private void musiclist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (result_playlist.SelectedIndex != -1)
            {
                PlaylistItem item = (PlaylistItem)result_playlist.SelectedItem;
               
                Load_Playlist(item.name);

            }
        }

        private void Load_Playlist(string list_name)
        {
            HideFavoriteList();

            /*
            #   특정 플레이 리스트 요청
                /getPlayList?title="단어"
                response-> getMelonChart했을때랑랑 같음
            */


            url_yotube = "http://115.71.236.224:8081/getPlayList?title=" + list_name;

            musicCollection.Clear();
            MsgBroker.MsgBrokerMsg arg = new MsgBroker.MsgBrokerMsg();
            arg.MsgOPCode = UI_CONTROL.PROGRESS_SHOW;
            Orange.Util.UI_Flag.IsChart = true;
            (Application.Current as App).msgBroker.SendMessage(arg);

            Thread thread = new Thread(new ThreadStart(ParsingThread));

            thread.Start();
        }

        private void top_list(object sender, RoutedEventArgs e)
        {
            if (favorite_playlist.SelectedIndex != -1)
            {
                PlaylistItem item = (PlaylistItem)favorite_playlist.SelectedItem;


                // MessageBoxOrange.ShowDialog(item.title);
                myFavoriteMgr.MyfavoriteCollection.Remove(item);
                myFavoriteMgr.MyfavoriteCollection.Insert(0, item);

                favorite_playlist.SelectedItem = item;
            }
        }

        private void up_list(object sender, RoutedEventArgs e)
        {
            int idx = favorite_playlist.SelectedIndex;
            if (favorite_playlist.SelectedIndex != -1)
            {
                if (favorite_playlist.SelectedIndex == 0)
                    return;

                PlaylistItem item = (PlaylistItem)favorite_playlist.SelectedItem;


                // MessageBoxOrange.ShowDialog(item.title);
                myFavoriteMgr.MyfavoriteCollection.Remove(item);
                myFavoriteMgr.MyfavoriteCollection.Insert(idx - 1, item);

                favorite_playlist.SelectedItem = item;
            }
        }

        private void down_list(object sender, RoutedEventArgs e)
        {
            int idx = favorite_playlist.SelectedIndex;
            if (favorite_playlist.SelectedIndex != -1)
            {
                if (favorite_playlist.SelectedIndex == myFavoriteMgr.MyfavoriteCollection.Count - 1)
                    return;

                PlaylistItem item = (PlaylistItem)favorite_playlist.SelectedItem;


                // MessageBoxOrange.ShowDialog(item.title);
                myFavoriteMgr.MyfavoriteCollection.Remove(item);
                myFavoriteMgr.MyfavoriteCollection.Insert(idx + 1, item);

                favorite_playlist.SelectedItem = item;
            }
        }

        private void bottom_list(object sender, RoutedEventArgs e)
        {
            int idx = favorite_playlist.SelectedIndex;
            if (favorite_playlist.SelectedIndex != -1)
            {
                if (favorite_playlist.SelectedIndex == myFavoriteMgr.MyfavoriteCollection.Count - 1)
                    return;

                PlaylistItem item = (PlaylistItem)favorite_playlist.SelectedItem;


                // MessageBoxOrange.ShowDialog(item.title);
                myFavoriteMgr.MyfavoriteCollection.Remove(item);
                myFavoriteMgr.MyfavoriteCollection.Insert(myFavoriteMgr.MyfavoriteCollection.Count, item);

                favorite_playlist.SelectedItem = item;
            }
        }


        private void delete_list(object sender, RoutedEventArgs e)
        {
            if (favorite_playlist.SelectedIndex != -1)
            {
                List<PlaylistItem> it = new List<PlaylistItem>();
                foreach (PlaylistItem item in favorite_playlist.SelectedItems)
                {
                    it.Add(item);
                }
                foreach (PlaylistItem item in it)
                {
                    try
                    {
                        String path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        string filedirectory = path + @"\OrangePlayer\favoritePlaylist";
                        //string filedirectory = System.AppDomain.CurrentDomain.BaseDirectory + "/favoritePlaylist";

                        string filepath = string.Format("{0}/{1}.orm", filedirectory, item.name);

                        if (File.Exists(filepath))
                        {
                            myFavoriteMgr.MyfavoriteCollection.Remove(item);
                            File.Delete(filepath);
                        }else
                            myFavoriteMgr.MyfavoriteCollection.Remove(item);
                    }catch(Exception ex)
                    {
                        
                        MessageBoxOrange.ShowDialog("Exception", ex.Message);
                    }
                    
                }
                it.Clear();
            }
        }

        private void newFavorite_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MsgBroker.MsgBrokerMsg arg = new MsgBroker.MsgBrokerMsg();
            arg.MsgOPCode = UI_CONTROL.SHOW_TOP_GRID;
            arg.MsgBody = new Create_FavoritePlaylistUserControl();
            (Application.Current as App).msgBroker.SendMessage(arg);
        }

        private void sharelist_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	// TODO: 여기에 구현된 이벤트 처리기를 추가하십시오.
        }
       
        private void CloseFavorite_Click(object sender, RoutedEventArgs e)
        {
            HideFavoriteList();
        }

        private void HideDrawerMenu()
        {
            HideFavoriteList();

            arg.MsgOPCode = UI_CONTROL.PROGRESS_HIDE;
            (Application.Current as App).msgBroker.SendMessage(arg);
        }

        public void ShowFavoriteList()
        {
            if (!IsFavoritePanel)
            {
                favoritelist_Content.Visibility = Visibility.Visible;
                IsFavoritePanel = true;
                arg.MsgOPCode = UI_CONTROL.HIDE_VIDEO;
                (Application.Current as App).msgBroker.SendMessage(arg);
            }
        }

        public void HideFavoriteList()
        {
            if (IsFavoritePanel)
            {
                //닫기
                favoritelist_Content.Visibility = Visibility.Hidden;
                IsFavoritePanel = false;
                arg.MsgOPCode = UI_CONTROL.SHOW_VIDEO;
                (Application.Current as App).msgBroker.SendMessage(arg);
            }
        }

        private void favorite_Load_list_Click(object sender, RoutedEventArgs e)
        {
            favorite_playlist.SelectedItem = (e.OriginalSource as FrameworkElement).DataContext;
            if (favorite_playlist.SelectedIndex != -1)
            {
                PlaylistItem item = (PlaylistItem)favorite_playlist.SelectedItem;
                favoritelist_Loadfile(item.name);
            }
        }

        private void favlist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (favorite_playlist.SelectedIndex != -1)
            {
                PlaylistItem item = (PlaylistItem)favorite_playlist.SelectedItem;
                favoritelist_Loadfile(item.name);
            }
        }

        private void favoritelist_Loadfile(string nameoflist)
        {
            HideFavoriteList();      
            MsgBroker.MsgBrokerMsg arg = new MsgBroker.MsgBrokerMsg();
            arg.MsgOPCode = MESSAGE_MAP.LOAD_ITEMS_IN_FAVORITE_PLAYLIST;
            arg.MsgBody = nameoflist;
            (Application.Current as App).msgBroker.SendMessage(arg);

           

        }

        private void myfavPlayList_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Delete:
                    if (favorite_playlist.SelectedIndex != -1)
                    {
                        List<PlaylistItem> it = new List<PlaylistItem>();
                        // myPlayList.DataContext = null;
                        foreach (PlaylistItem item in favorite_playlist.SelectedItems)
                        {
                            it.Add(item);
                        }
                        foreach (PlaylistItem item in it)
                        {

                            try
                            {
                                String path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                                string filedirectory = path + @"\OrangePlayer\favoritePlaylist";
                                //string filedirectory = System.AppDomain.CurrentDomain.BaseDirectory + "/favoritePlaylist";

                                string filepath = string.Format("{0}/{1}.orm", filedirectory, item.name);

                                if (File.Exists(filepath))
                                {
                                    myFavoriteMgr.MyfavoriteCollection.Remove(item);
                                    File.Delete(filepath);
                                }else
                                    myFavoriteMgr.MyfavoriteCollection.Remove(item);
                            }
                            catch (Exception ex)
                            {
                                MessageBoxOrange.ShowDialog("Exception",ex.Message);
                            }
                        }
                        it.Clear();
                    }
                    break;               
            }

        }
        


        public void SetMenuLanguage()
        {
            switch (LanguagePack.TYPE)
            {
                case 0:
                    mn_myfavoritelist.Template = (ControlTemplate)FindResource("Menu1btnControlTemplate_kor");
                    mn_sharelist.Template = (ControlTemplate)FindResource("SharemylistButtonControlTemplate_kor");
                    mn_whatsnew.Template = (ControlTemplate)FindResource("NewSongsButtonControlTemplate_kor");
                    mn_hotlist.Template = (ControlTemplate)FindResource("HotplaylistButtonControlTemplate_kor");
                    break;
                case 1:
                    mn_myfavoritelist.Template = (ControlTemplate)FindResource("Menu1btnControlTemplate");
                    mn_sharelist.Template = (ControlTemplate)FindResource("SharemylistButtonControlTemplate");
                    mn_whatsnew.Template = (ControlTemplate)FindResource("NewSongsButtonControlTemplate");
                    mn_hotlist.Template = (ControlTemplate)FindResource("HotplaylistButtonControlTemplate");
                    break;
                case 2:
                    mn_myfavoritelist.Template = (ControlTemplate)FindResource("Menu1btnControlTemplate_jap");
                    mn_sharelist.Template = (ControlTemplate)FindResource("SharemylistButtonControlTemplate_jap");
                    mn_whatsnew.Template = (ControlTemplate)FindResource("NewSongsButtonControlTemplate_jap");
                    mn_hotlist.Template = (ControlTemplate)FindResource("HotplaylistButtonControlTemplate_jap");
                    break;
                case 3:
                    mn_myfavoritelist.Template = (ControlTemplate)FindResource("Menu1btnControlTemplate_fre");
                    mn_sharelist.Template = (ControlTemplate)FindResource("SharemylistButtonControlTemplate_fre");
                    mn_whatsnew.Template = (ControlTemplate)FindResource("NewSongsButtonControlTemplate_fre");
                    mn_hotlist.Template = (ControlTemplate)FindResource("HotplaylistButtonControlTemplate_fre");
                    break;
                default:
                    mn_myfavoritelist.Template = (ControlTemplate)FindResource("Menu1btnControlTemplate");
                    mn_sharelist.Template = (ControlTemplate)FindResource("SharemylistButtonControlTemplate");
                    mn_whatsnew.Template = (ControlTemplate)FindResource("NewSongsButtonControlTemplate");
                    mn_hotlist.Template = (ControlTemplate)FindResource("HotplaylistButtonControlTemplate");
                    break;
            }
        }

       
	}
}