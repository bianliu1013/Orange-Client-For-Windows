using Orange.MsgBroker;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Orange
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        public MessageBroker msgBroker { get; set; }
        private Mutex _mutex = null;
        private string mutexName = "Orange YOUTUBE Player 2014-07-30";
 
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            msgBroker = new MessageBroker();
            bool isCreatedNew = false;
            try
            {
                _mutex = new Mutex(true, mutexName, out isCreatedNew);


                if (isCreatedNew)
                {
                    base.OnStartup(e);
                }
                else
                {
                    MessageBox.Show("Application already started.", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                    Application.Current.Shutdown();
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace + "\n\n" + "Application Existing...", "Exception thrown");
                Application.Current.Shutdown();
            }
        }
    }
}
