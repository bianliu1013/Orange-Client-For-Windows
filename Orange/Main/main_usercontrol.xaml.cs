using Orange.DataManager;
using Orange.MsgBroker;
using Orange.Util;
using System;
using System.Collections.Generic;
using System.Net.Json;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Orange
{
	/// <summary>
	/// main_usercontrol.xaml에 대한 상호 작용 논리
	/// </summary>
	/// 
	/// 
	public partial class main_usercontrol : UserControl
	{
       
        private MusicCollection musicCollection;
		public main_usercontrol()
		{
			this.InitializeComponent();
            ProgressRing.IsActive= false;

            
		}

        public void SetProgressRing(bool state, int type)
        {
            if (header.Visibility == Visibility.Visible)
            {
                header.Visibility = Visibility.Collapsed;
            }
            if(type == 0)
            {
                whiteGrid.Visibility = Visibility.Visible;
                ProgressRing.IsActive = state;
            }else if(type == 1)
            {
                ProgressRing.IsActive = state;
                whiteGrid.Visibility = Visibility.Hidden;
            }
            
        }

        public void InitFavoriteListview(MusicCollection mc)
        {
            musicCollection = mc;
            MyfavoritelistMgr favoritelistMgr = MyfavoritelistMgr.instance();
            favorite_playlist.DataContext = favoritelistMgr.MyfavoriteCollection;
        }

        private void favlist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (favorite_playlist.SelectedIndex != -1)
            {
                PlaylistItem item = (PlaylistItem)favorite_playlist.SelectedItem;

                favoritelist_Loadfile(item.name);

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


        private void favoritelist_Loadfile(string nameoflist)
        {

            MsgBroker.MsgBrokerMsg arg = new MsgBroker.MsgBrokerMsg();
            arg.MsgOPCode = MESSAGE_MAP.LOAD_ITEMS_IN_FAVORITE_PLAYLIST;
            arg.MsgBody = nameoflist;
            (Application.Current as App).msgBroker.SendMessage(arg);

        }

        private void favorite_ScrollViewer_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
	}
}