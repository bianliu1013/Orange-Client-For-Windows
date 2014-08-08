using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orange.MsgBroker
{
    public class MsgBrokerMsg
    {
        public MsgBrokerMsg()
        { }

        public MsgBrokerMsg(string name, object msg)
        {
            MsgName = name;
            MsgBody = msg;
        }

        public string MsgName { get; set; }
        public object MsgBody { get; set; }
        public int MsgOPCode { get; set; }
    }
}
