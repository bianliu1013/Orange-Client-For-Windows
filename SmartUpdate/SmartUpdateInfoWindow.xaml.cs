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
using System.Windows.Shapes;

namespace SmartUpdate
{
    /// <summary>
    /// SmartUpdateInfoWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SmartUpdateInfoWindow : Window
    {
        internal SmartUpdateInfoWindow(ISmartUpdatable applicationInfo, SmartUpdateXml updateInfo)
        {
            InitializeComponent();

            if (applicationInfo.ApplicationIcon != null)
                this.Icon = applicationInfo.ApplicationIcon;

            this.Title = applicationInfo.ApplicationName + " - Update Info";
            this.lblVersions.Content = String.Format("Current Version: {0}\nUpdate Version: {1}", applicationInfo.ApplicationAssembly.GetName().Version.ToString(),
                updateInfo.Version.ToString());
            this.txtDescription.Document.Blocks.Add(new Paragraph(new Run(updateInfo.Description)));
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
