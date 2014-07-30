using Orange.MsgBroker;
using Orange.Util;
using SmartUpdate;
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
	/// information_usercontrol.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class information_usercontrol : UserControl
	{
        private AppInfo appinfo;
		public information_usercontrol(MainWindow win)
		{
			this.InitializeComponent();

            appinfo = new AppInfo(win);

            infoUC.initSmartUpdate(appinfo, appinfo.ApplicationAssembly.GetName().Version.ToString());
    
		}

		
		private void LayoutRoot_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
            e.Handled = true;
            MsgBroker.MsgBrokerMsg arg = new MsgBroker.MsgBrokerMsg();
            arg.MsgOPCode = UI_CONTROL.HIDE_TOP_GRID;
          
            (Application.Current as App).msgBroker.SendMessage(arg);
		}

		private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			System.Diagnostics.Process.Start("https://docs.google.com/forms/d/1U7zPB99CtnbcS_GvHn0IAwOZLmq3aQ9I_kS7Il61HJE/viewform?usp=send_form#start=openform");
		}
	}
}