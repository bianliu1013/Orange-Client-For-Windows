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
using Orange.MsgBroker;
using System.Windows.Threading;
using Orange.DataManager;
using System.Threading;

namespace Orange
{
	/// <summary>
	/// main_menuControl.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class main_menuControl : UserControl
	{
        private string url;
        private MusicCollection musicCollection;
		public main_menuControl()
		{
			this.InitializeComponent();
		}

        public void SetMusicCollection(MusicCollection collection)
        {
            musicCollection = collection;
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
            url = "http://115.71.236.224:8081/getBillboardChart";

            musicCollection.Clear();
            MsgBroker.MsgBrokerMsg arg = new MsgBroker.MsgBrokerMsg();
            arg.MsgOPCode = UI_CONTROL.PROGRESS_SHOW;
            (Application.Current as App).msgBroker.SendMessage(arg);
            Thread thread = new Thread(new ThreadStart(ParsingThread));
            thread.Start();
		}

		private void mn_kpop_btn_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            url = "http://115.71.236.224:8081/getMelonChart";

            musicCollection.Clear();
            MsgBroker.MsgBrokerMsg arg = new MsgBroker.MsgBrokerMsg();
            arg.MsgOPCode = UI_CONTROL.PROGRESS_SHOW;
            (Application.Current as App).msgBroker.SendMessage(arg);
            Thread thread = new Thread(new ThreadStart(ParsingThread));
            thread.Start();

		}

        private void ParsingThread()
        {
             
            try
            {
                JsonArrayCollection items = JSONHelper.getJSONArray(url);

                Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
                {
                foreach (JsonObjectCollection item in items)
                {
                    string title = item["title"].GetValue().ToString();
                    string singer = item["singer"].GetValue().ToString();
                    ////수정해야                
                    // string mUrl = item["singer"].GetValue().ToString();
                    // string playTime = item["singer"].GetValue().ToString();
                    string mUrl = item["url"].GetValue().ToString().Replace("http://www.youtube.com/watch?v=", ""); ;
                    string playTime = item["time"].GetValue().ToString();


                    MusicItem mitem = new MusicItem();
                    mitem.title = title + " - " + singer;
                    mitem.url = mUrl;
                    mitem.playTime = playTime;
                    musicCollection.Add(mitem);
                }

           


               MsgBroker.MsgBrokerMsg arg = new MsgBroker.MsgBrokerMsg();
               arg.MsgOPCode = UI_CONTROL.PROGRESS_HIDE;
               (Application.Current as App).msgBroker.SendMessage(arg);

               }));
            }
            catch (Exception e) { MessageBox.Show(e.Message);

                        Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
               {
                     MsgBroker.MsgBrokerMsg arg = new MsgBroker.MsgBrokerMsg();
                     arg.MsgOPCode = UI_CONTROL.PROGRESS_HIDE;
                     (Application.Current as App).msgBroker.SendMessage(arg);
               }));
            
               }
        
           
        }

		private void mn_jpop_btn_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            url = "http://115.71.236.224:8081/getOriconChart";

            musicCollection.Clear();
            MsgBroker.MsgBrokerMsg arg = new MsgBroker.MsgBrokerMsg();
            arg.MsgOPCode = UI_CONTROL.PROGRESS_SHOW;
            (Application.Current as App).msgBroker.SendMessage(arg);
            Thread thread = new Thread(new ThreadStart(ParsingThread));
            thread.Start();
		}
	}
}