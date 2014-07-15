using Orange.MsgBroker;
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
	/// ConvertingProgress.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class ConvertingProgress : UserControl
	{
		public ConvertingProgress()
		{
			this.InitializeComponent();
            ConvertProgressBar.Maximum = 100;
            ConvertProgressBar.Minimum = 0;
            (Application.Current as App).msgBroker.MessageReceived += msgBroker_MessageReceived;
		}

        void msgBroker_MessageReceived(object sender, MsgBroker.MsgBrokerEventArgs e)
        {
            switch(e.Message.MsgOPCode)
            {
                case UI_CONTROL.SET_CONVERT_PROGRESS_VALUE:
                    ConvertProgressBar.Value = (double)e.Message.MsgBody;
                    state_rate.Text = e.Message.MsgBody.ToString() + " %";
                    break;
                case UI_CONTROL.FINISH_CONVERT_PROGRESS:

                    confirmBtn.Visibility= Visibility.Visible;
                    
                    break;
            }
        }

        private void state_rate_Unloaded(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).msgBroker.MessageReceived -= msgBroker_MessageReceived;
        }

        private void confirmBtn_Click(object sender, RoutedEventArgs e)
        {
            MsgBroker.MsgBrokerMsg arg = new MsgBroker.MsgBrokerMsg();
            arg.MsgOPCode = UI_CONTROL.HIDE_TOP_GRID;

            (Application.Current as App).msgBroker.SendMessage(arg);
        }


        
	}
}