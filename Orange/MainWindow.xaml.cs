using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Orange
{
    
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private bool IsLeftPanelState = false;
        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void MenuBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	// TODO: 여기에 구현된 이벤트 처리기를 추가하십시오.
            if(IsLeftPanelState)
            {
                Storyboard HideLeftPanelStoryboard = this.Resources["left_panel_hide"] as Storyboard;
                HideLeftPanelStoryboard.Begin();
                IsLeftPanelState = false;
            }
            else
            {
                Storyboard ShowLeftPanelStoryboard = this.Resources["left_panel_show"] as Storyboard;
                ShowLeftPanelStoryboard.Begin();
                IsLeftPanelState = true;
            }
        }
    }
}
