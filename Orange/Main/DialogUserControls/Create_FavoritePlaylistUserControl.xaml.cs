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

namespace Orange
{
	/// <summary>
	/// Create_FavoritePlaylistUserControl.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class Create_FavoritePlaylistUserControl : UserControl
	{
		public Create_FavoritePlaylistUserControl()
		{
			this.InitializeComponent();
		}

        private void confirm_Click(object sender, RoutedEventArgs e)
        {
            MsgBroker.MsgBrokerMsg arg = new MsgBroker.MsgBrokerMsg();
            arg.MsgOPCode = Orange.MsgBroker.MESSAGE_MAP.CREATE_FAVORITE_PLAYLIST;
            if (InputTxb.Text.Trim().Equals(""))
            {
                MessageBox.Show("The content is empty");
                return;
            }
            arg.MsgBody = InputTxb.Text;
            (Application.Current as App).msgBroker.SendMessage(arg);
            HideThisUsercontrol();
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            HideThisUsercontrol();
        }

        private void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            HideThisUsercontrol();
        }

        private void HideThisUsercontrol()
        {
            MsgBroker.MsgBrokerMsg arg = new MsgBroker.MsgBrokerMsg();
            arg.MsgOPCode = Orange.MsgBroker.UI_CONTROL.HIDE_TOP_GRID;

            (Application.Current as App).msgBroker.SendMessage(arg);
        }
	}
}