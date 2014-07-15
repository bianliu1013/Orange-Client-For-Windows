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
		public information_usercontrol()
		{
			this.InitializeComponent();
		}

		private void Image_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
            e.Handled = true;

            if(e.ClickCount > 3)
            {
                name.Visibility = Visibility.Visible;
            }
            else
            {

            }
		}

		private void LayoutRoot_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
            e.Handled = true;
            this.Visibility = Visibility.Collapsed;
		}
	}
}