using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orange.Util
{
    class Config
    {
        public static bool IsShffle = false;
        public static int REPEAT = 0; // DEFAULT 0, ALL REPEAT 1, SINGLE REPEAT 2
        public static bool IsMute = false;        
    }

    class UI_Flag
    {
        public static bool IsShowingTopGrid = false;
        public static bool IsShowingVideo = true;
    }

    class Player_State
    {        
        public static bool IsPlaying = false;       // 플레이어 재생중
        public static bool IsEndOfPoint = false;
        public static string VolumeValue = "80";
    }
}
