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
using System.Net;
using System.Net.Json;
using System.IO;

namespace Orange
{
	/// <summary>
	/// main_menuControl.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class main_menuControl : UserControl
	{
        KPopList kpopListObject = MainWindow.kpopList;

		public main_menuControl()
		{
			this.InitializeComponent();
		}

		private void mn_new_btn_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			// TODO: 여기에 구현된 이벤트 처리기를 추가하십시오.
		}

		private void mn_myplaylist_btn_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			// TODO: 여기에 구현된 이벤트 처리기를 추가하십시오.
		}

		private void mn_pop_btn_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			// TODO: 여기에 구현된 이벤트 처리기를 추가하십시오.
		}

		private void mn_kpop_btn_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            string url = "http://115.71.236.224:8081/getMelonChart";

            JsonArrayCollection items = JSONHelper.getJSONArray(url);

            kpopListObject.getKPopList().Clear();
            foreach (JsonObjectCollection item in items)
            {
                string title = item["title"].GetValue().ToString();
                string singer = item["singer"].GetValue().ToString();

                KPopObject kpopObject = new KPopObject(title, singer);
                kpopListObject.getKPopList().Add(kpopObject);
            }

            string s = null;
            for (int i = 0; i < MainWindow.kpopList.getKPopList().Count; i++)
            {
                s += kpopListObject.getKPop(i).getTitle() + " " + kpopListObject.getKPop(i).getSinger() + "\n";
            }

            MessageBox.Show(s);
		}

		private void mn_jpop_btn_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			// TODO: 여기에 구현된 이벤트 처리기를 추가하십시오.
		}
	}
}