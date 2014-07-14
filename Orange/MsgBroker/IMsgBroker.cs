using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orange.MsgBroker
{
   public delegate void MessageReceviedEventHandler(object sender, MsgBrokerEventArgs e);

    /// <summary>
    /// Interface for MsgBroker object
    /// </summary>
    public interface IMsgBroker
    {
        void SendMessage(MsgBrokerMsg msg);
        event MessageReceviedEventHandler MessageReceived;
    }
}
