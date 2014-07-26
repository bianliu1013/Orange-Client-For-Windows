using Orange.DataManager;
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
	/// rename_usercontrol.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class rename_usercontrol : UserControl
	{
        private MusicItem item;
		public rename_usercontrol(MusicItem _item)
		{
			this.InitializeComponent();

            item = _item;

            RenameTxb.Text = item.title;
		}

		private void confirm_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            item.title = RenameTxb.Text;
            MsgBroker.MsgBrokerMsg arg = new MsgBroker.MsgBrokerMsg();
            arg.MsgOPCode = Orange.MsgBroker.UI_CONTROL.RefreshMyplayList;

            (Application.Current as App).msgBroker.SendMessage(arg);
            CloseUserControl();
		}

		private void cancel_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            CloseUserControl();
		}

		private void LayoutRoot_Copy_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
            CloseUserControl();
		}

        private void CloseUserControl()
        {
            MsgBroker.MsgBrokerMsg arg = new MsgBroker.MsgBrokerMsg();
            arg.MsgOPCode = Orange.MsgBroker.UI_CONTROL.HIDE_TOP_GRID;

            (Application.Current as App).msgBroker.SendMessage(arg);
        }
	}
}