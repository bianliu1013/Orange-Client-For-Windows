using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orange.MsgBroker
{
    class MsgMap
    {
        
    }

    class MESSAGE_MAP
    {
        public const int KPOP_CART         = 1001;
        public const int ENGLISH_POP_CHART = 1002;
        public const int JPOP_CHART        = 1003;
        public const int NEW_SONGS         = 1004;

       
    }

    class UI_CONTROL
    {
        public const int PROGRESS_SHOW = 1005;
        public const int PROGRESS_HIDE = 1006;

        public const int SHOW_TOP_GRID = 1007;
        public const int HIDE_TOP_GRID = 1008;

        public const int SET_CONVERT_PROGRESS_VALUE = 1009;
        public const int FINISH_CONVERT_PROGRESS = 1010;
    }
}
