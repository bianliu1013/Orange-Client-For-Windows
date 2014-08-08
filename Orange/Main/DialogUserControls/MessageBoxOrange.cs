using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Orange.Main.DialogUserControls
{
    class MessageBoxOrange
    {
        public static void ShowDialog(string title, string content)
        {
            MsgBroker.MsgBrokerMsg arg = new MsgBroker.MsgBrokerMsg();
            arg.MsgOPCode = Orange.MsgBroker.UI_CONTROL.SHOW_TOP_GRID;
            arg.MsgBody = new DialogMsg(title, content);
            (Application.Current as App).msgBroker.SendMessage(arg);
        }
    }
}
