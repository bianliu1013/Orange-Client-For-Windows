using SmartUpdate;
using System;
using System.Collections.Generic;
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
        
        public AppInfo(Window win)
        {
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
            get { return new Uri(Config.UPDATE_URL); }
        }

        public Window Context
        {
            get { return win; }
        }
    }
}
