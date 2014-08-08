using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orange.MsgBroker
{
    public class MsgBrokerEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor for MessageBorkerEventArgs class
        /// </summary>
        public MsgBrokerEventArgs() { }
        public MsgBrokerEventArgs(string name, MsgBrokerMsg body)
        {
            MsgName = name;
            Message = body;
        }


        public string MsgName { get; set; }
        public MsgBrokerMsg Message { get; set; }
    }
}
