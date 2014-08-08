using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Reflection;

namespace SmartUpdate
{
    public interface ISmartUpdatable
    {
        string ApplicationName { get; }
        string ApplicationID { get; }
        Assembly ApplicationAssembly { get; }
        ImageSource ApplicationIcon { get; }
        Uri UpdateXmlLocation { get; }
        Window Context { get; }
        Version appId { get; }
    }
}
