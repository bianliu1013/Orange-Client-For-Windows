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
	/// main_usercontrol.xaml에 대한 상호 작용 논리
	/// </summary>
	/// 
	/// 
	public partial class main_usercontrol : UserControl
	{
		
		public main_usercontrol()
		{
			this.InitializeComponent();
            ProgressRing.IsActive= false;
			
		}

        public void SetProgressRing(bool state, int type)
        {
            if(type == 0)
            {
                whiteGrid.Visibility = Visibility.Visible;
                ProgressRing.IsActive = state;
            }else if(type == 1)
            {
                ProgressRing.IsActive = state;
                whiteGrid.Visibility = Visibility.Hidden;
            }
            
        }
	}
}