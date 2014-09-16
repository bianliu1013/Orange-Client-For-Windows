using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orange.Util
{
    class Config
    {
        
        public const string SKey = "b0d9b872";

        public static bool IsShffle = false;
        public static int REPEAT = 1; // DEFAULT 0, ALL REPEAT 1, SINGLE REPEAT 2
        public static bool IsMute = false;
        public static int Language_for_Orange = 0;
        public static bool IsTopMost = false;

        public static bool IsAvailableUpdate = false;
    }

    class UI_Flag
    {
        public static bool IsShowingTopGrid = false;
        public static bool IsShowingVideo = true;
        public static bool IsChart = false;
        public static bool IsVideoOption = false;
    }

    class Player_State
    {        
        public static bool IsPlaying = false;       // 플레이어 재생중
        public static bool IsEndOfPoint = false;
        public static string VolumeValue = "80";
        public static int playCount = 0;
    }

    class URL
    {
        //public const string YOUTUBEPLAYER_URL = "http://psbworld.com/YouTubePlayer_v3.htm";
        //public const string YOUTUBEPLAYER_URL = "http://psbworld.com/YouTubePlayer.htm";
        public const string YOUTUBEPLAYER_URL = "http://115.71.236.224:8081/static/YouTubePlayer_v3.htm";
        public const string YOUTUBEPLAYER_URL_RELOAD = "http://115.71.236.224:8081/static/YouTubePlayer_v3_reload.htm";
        //public const string YOUTUBEPLAYER_URL_RELOAD = "http://psbworld.com/YouTubePlayer_v3_reload.htm";
        //public const string YOUTUBEPLAYER_URL = "http://115.71.236.224:8081/static/YouTubePlayer.htm";
        //public const string YOUTUBEPLAYER_URL = "http://giftorange.com/YouTubePlayer.htm";
        public const string UPDATE_URL = "http://115.71.236.224/~jcsla/OrangePlayerUpdate.xml";
        public const string GIFT_URL = "http://giftorange.com";
    }
}
