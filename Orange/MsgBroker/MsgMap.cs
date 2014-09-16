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
        public const int CREATE_FAVORITE_PLAYLIST = 1019;
        public const int LOAD_ITEMS_IN_FAVORITE_PLAYLIST = 1020;
        public const int UPLOAD_PLAYLIST = 1021;
    }

    class UI_CONTROL
    {
        public const int PROGRESS_SHOW = 1005;
        public const int PROGRESS_HIDE = 1006;

        public const int SHOW_TOP_GRID = 1007;
        public const int HIDE_TOP_GRID = 1008;

        public const int SET_CONVERT_PROGRESS_VALUE = 1009;
        public const int FINISH_CONVERT_PROGRESS = 1010;

        public const int SetTopmost = 1011;
        
        public const int DisableTopmost = 1012;

        public const int RefreshMyplayList = 1013;

        public const int SET_INIT_AFTER_TUTORIAL = 1014;
        public const int SET_LANGUAGE = 1015;

        public const int ACTIVEMOREBTN = 1016;

        public const int HIDE_VIDEO = 1017;
        public const int SHOW_VIDEO = 1018;

        public const int OPEN_DRAWERMENU = 1022;

        public const int ACTIVETRAY = 1023;

        public const int DEACTIVETRAY = 1024;    
    }
}
