using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orange.Util
{
    class LanguagePack
    {
        // Kor  0
        // Eng  1
        // Jap  2
        public static int TYPE = 0;
        public static string SetShuffle()
        {
            switch(TYPE)
            {
                case 0:
                    return "섞어듣기";
                case 1:
                    return "Shuffle";
                case 2:
                    return "순차듣기";
            }
            return "Shuffle";
        }


    }
}
