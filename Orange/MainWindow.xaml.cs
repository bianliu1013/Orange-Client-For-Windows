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
using System.Net;
using System.Net.Json;
using System.IO;
using System.Web;
using System.Security.Permissions;
using System.Reflection;

namespace Orange
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]

    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private bool IsLeftPanelState = false;

        public static KPopList kpopList = new KPopList();
        

        public MainWindow()
        {
            InitializeComponent();

            webBrowser.Navigated += webBrowser_Navigated;
            webBrowser.Navigate("http://115.71.236.224:8081/static/YouTubePlayer.html");
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

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            webBrowser.Visibility= Visibility.Visible;
        }

        private void Search_Button_Click(object sender, RoutedEventArgs e)
        {
            string url = "http://115.71.236.224:8081/searchMusicVideoInformation?query=";
            string queryString = searchBox.Text.ToString();

            string query = url + queryString;

            JsonObjectCollection col = JSONHelper.getJson(query);

            string resultURL = (string)col["url"].GetValue();
            string resultTitle = (string)col["title"].GetValue();

            MessageBox.Show(resultURL + " " + resultTitle);
        }

        private void play(object sender, RoutedEventArgs e)
        {
            //var document = webBrowser.Document;
            //webBrowser.Document.GetType().InvokeMember("pauseVideo", BindingFlags.InvokeMethod, null, document, null);
            webBrowser.InvokeScript("pauseVideo");
        }

        private void webBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        public void HideScriptErrors(WebBrowser wb, bool Hide)
        {
            FieldInfo fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fiComWebBrowser == null) return;
            object objComWebBrowser = fiComWebBrowser.GetValue(wb);
            if (objComWebBrowser == null) return;
            objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { Hide });
        }

        private void webBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            HideScriptErrors(webBrowser, true);
        }
    }
}
