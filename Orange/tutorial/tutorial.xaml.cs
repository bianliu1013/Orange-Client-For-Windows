using Orange.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
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
	/// tutorial.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class tutorial : UserControl
	{
        MsgBroker.MsgBrokerMsg arg;
		public tutorial()
		{
			this.InitializeComponent();
            arg = new MsgBroker.MsgBrokerMsg();

            CountInstall();
            
		}

        private void CountInstall()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://115.71.236.224:8081/addInstalledCount");            
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();            
        }

		private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            if(languageCb.SelectedIndex!=-1)
            {
                if (languageCb.SelectedIndex == 0)
                    Properties.Settings.Default.Language_for_Orange = 1;
                if (languageCb.SelectedIndex == 1)
                    Properties.Settings.Default.Language_for_Orange = 0;
                if (languageCb.SelectedIndex == 2)
                    Properties.Settings.Default.Language_for_Orange = 2;
                if (languageCb.SelectedIndex == 3)
                    Properties.Settings.Default.Language_for_Orange = 3;

                Orange.Util.LanguagePack.TYPE = Properties.Settings.Default.Language_for_Orange;
                Config.Language_for_Orange = Orange.Util.LanguagePack.TYPE;
                arg.MsgOPCode = Orange.MsgBroker.UI_CONTROL.SET_INIT_AFTER_TUTORIAL;
                (Application.Current as App).msgBroker.SendMessage(arg);
                arg.MsgOPCode = Orange.MsgBroker.UI_CONTROL.HIDE_TOP_GRID;
                (Application.Current as App).msgBroker.SendMessage(arg);
                arg.MsgOPCode = Orange.MsgBroker.UI_CONTROL.OPEN_DRAWERMENU;
                (Application.Current as App).msgBroker.SendMessage(arg);
            }

		}
	}
}