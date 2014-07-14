using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orange.MsgBroker
{
    public class MessageBroker : IMsgBroker
    {
        #region SendMessage Methid
        public void SendMessage(MsgBrokerMsg msg)
        {
            MsgBrokerEventArgs args;
            args = new MsgBrokerEventArgs(msg.MsgName, msg);

            RaiseMessageRecevied(args);
        }
        #endregion

        #region MessageReceived Event
        public event MessageReceviedEventHandler MessageReceived;

        protected void RaiseMessageRecevied(MsgBrokerEventArgs e)
        {
            if (MessageReceived != null)
                MessageReceived(this, e);
        }
        #endregion



    }
}
