using SmartUpdate;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Orange.Util
{
    public class AppInfo : ISmartUpdatable
    {
        private Window win;
        private Version version;
        public AppInfo(Window win)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            version = Version.Parse(fvi.FileVersion);
            this.win = win;
        }

        public string ApplicationName
        {
            get { return "Update Orange"; }
        }

        public string ApplicationID
        {
            get { return "Orange"; }
        }

        public Version appId
        {
            get { return version; }
        }

        public System.Reflection.Assembly ApplicationAssembly
        {

            get { return Assembly.GetExecutingAssembly(); }
        }

        public ImageSource ApplicationIcon
        {
            get { return win.Icon; }
        }

        public Uri UpdateXmlLocation
        {
            get { return new Uri(URL.UPDATE_URL); }
        }

        public Window Context
        {
            get { return win; }
        }
    }
}
