using Orange.MsgBroker;
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
	/// Preferences.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class Preferences : UserControl
	{
		public Preferences()
		{            
			this.InitializeComponent();
            initPrefer();
		}

        private void initPrefer()
        {
            TopmostToggleSwitch.IsChecked = Config.IsTopMost;
            TopmostToggleSwitch.IsCheckedChanged+=TopmostToggleSwitch_IsCheckedChanged;
            TopmostToggleSwitch.Header = LanguagePack.TopMost();

            if (Properties.Settings.Default.Language_for_Orange == 0)
                LanguageCombobox.SelectedIndex = 1;
            else if (Properties.Settings.Default.Language_for_Orange == 1)
                LanguageCombobox.SelectedIndex = 0;

        }


		private void confirmBtn_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            HideThisUsercontrol();
		}

        private void TopmostToggleSwitch_IsCheckedChanged(object sender, EventArgs e)
        {
            if(Config.IsTopMost)
            {
                TopmostToggleSwitch.IsChecked = Config.IsTopMost = false;
                MsgBroker.MsgBrokerMsg arg = new MsgBroker.MsgBrokerMsg();
                arg.MsgOPCode = UI_CONTROL.DisableTopmost;
                (Application.Current as App).msgBroker.SendMessage(arg);
            }
            else
            {
                TopmostToggleSwitch.IsChecked = Config.IsTopMost = true;
                MsgBroker.MsgBrokerMsg arg = new MsgBroker.MsgBrokerMsg();
                arg.MsgOPCode = UI_CONTROL.SetTopmost;
                (Application.Current as App).msgBroker.SendMessage(arg);
            }
        }

        private void LayoutRoot_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	// TODO: 여기에 구현된 이벤트 처리기를 추가하십시오.
            e.Handled = true;
            HideThisUsercontrol();
        }

        private void HideThisUsercontrol()
        {            
            MsgBroker.MsgBrokerMsg arg = new MsgBroker.MsgBrokerMsg();
            arg.MsgOPCode = UI_CONTROL.HIDE_TOP_GRID;

            (Application.Current as App).msgBroker.SendMessage(arg);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            TopmostToggleSwitch.IsCheckedChanged -= TopmostToggleSwitch_IsCheckedChanged;
        }

        private void LanguageCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(LanguageCombobox.SelectedIndex != -1)
            {
                if (LanguageCombobox.SelectedIndex == 0)
                    Properties.Settings.Default.Language_for_Orange = 1;
                if (LanguageCombobox.SelectedIndex == 1)
                    Properties.Settings.Default.Language_for_Orange = 0;

                Orange.Util.LanguagePack.TYPE = Properties.Settings.Default.Language_for_Orange;

                TopmostToggleSwitch.Header = LanguagePack.TopMost();
                MsgBroker.MsgBrokerMsg arg = new MsgBroker.MsgBrokerMsg();
                arg.MsgOPCode = UI_CONTROL.SET_LANGUAGE;

                (Application.Current as App).msgBroker.SendMessage(arg);

            }
        }
	}
}