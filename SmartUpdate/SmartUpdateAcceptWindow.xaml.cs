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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SmartUpdate
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SmartUpdateAcceptWindow : Window
    {
        private ISmartUpdatable applicationInfo;
        private SmartUpdateXml updateInfo;
        private SmartUpdateInfoWindow updateInfoWindow;

        internal SmartUpdateAcceptWindow(ISmartUpdatable applicationInfo, SmartUpdateXml updateInfo)
        {
            InitializeComponent();

            this.applicationInfo = applicationInfo;
            this.updateInfo = updateInfo;

            this.Title = this.applicationInfo.ApplicationName + " - Update Available";

            if (this.applicationInfo.ApplicationIcon != null)
                this.Icon = this.applicationInfo.ApplicationIcon;

            this.lblNewVersion.Content = string.Format("New Version: {0}", this.updateInfo.Version.ToString());
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btnDetails_Click(object sender, RoutedEventArgs e)
        {
            if (this.updateInfoWindow == null)
                this.updateInfoWindow = new SmartUpdateInfoWindow(this.applicationInfo, this.updateInfo);

            this.updateInfoWindow.ShowDialog();
        }
    }
}
