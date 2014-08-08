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
using System.Windows.Threading;

namespace Orange
{
	/// <summary>
	/// DialogMsg.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class DialogMsg : UserControl
	{
        private DispatcherTimer dt;
        public DialogMsg(string title, string content)
		{
			this.InitializeComponent();
            title_content.Text = title;
            dialog_Content.Text = content;

            dt = new DispatcherTimer();
            dt.Interval = new TimeSpan(0, 0, 3);
            dt.Tick+=Dialog_dt_Tick;
            dt.Start();


		}
        private void Dialog_dt_Tick(object sender, EventArgs e)
        {
            dt.Stop();
            HideThisUsercontrol();
        }
        private void LayoutRoot_MouseDown(object sender, MouseButtonEventArgs e)
        {
            HideThisUsercontrol();
        }

        private void confirmBtn_Click(object sender, RoutedEventArgs e)
        {
            HideThisUsercontrol();
        }

        private void HideThisUsercontrol()
        {
            MsgBroker.MsgBrokerMsg arg = new MsgBroker.MsgBrokerMsg();
            arg.MsgOPCode = Orange.MsgBroker.UI_CONTROL.HIDE_TOP_GRID;

            (Application.Current as App).msgBroker.SendMessage(arg);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            dt.Tick -= Dialog_dt_Tick;
        }

 

	}
}