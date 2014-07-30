using Orange.Util;
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
	/// SharePlayListUserControl.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class SharePlayListUserControl : UserControl
	{
		public SharePlayListUserControl()
		{
			this.InitializeComponent();

            warn_text.Text = LanguagePack.SetWarnShare();
		}

        private void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            HideThisUsercontrol();

        }

        private void confirm_Click(object sender, RoutedEventArgs e)
        {
            if(InputTxb.Text.Trim().Equals(""))
            {
                MessageBox.Show("The content is empty");
                return;
            }
            MsgBroker.MsgBrokerMsg arg = new MsgBroker.MsgBrokerMsg();
            arg.MsgOPCode = Orange.MsgBroker.MESSAGE_MAP.UPLOAD_PLAYLIST;
            arg.MsgBody = InputTxb.Text;
            (Application.Current as App).msgBroker.SendMessage(arg);

            HideThisUsercontrol();
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
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