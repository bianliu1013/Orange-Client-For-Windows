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
        public static int TYPE =1;

        
        public static string Shuffle()
        {
            switch(TYPE)
            {
                case 0:
                    return "섞어듣기";
                case 1:
                    return "Shuffle";
                case 2:
                    return "シャッフル";
            }
            return "Shuffle";
        }

        public static string PlayStraight()
        {
            switch (TYPE)
            {
                case 0:
                    return "순차 듣기";
                case 1:
                    return "PlayStraight";
                case 2:
                    return "順次リスニング";
            }
            return "PlayStraight";
        }
        //Play all files in order.
        public static string NormalMode()
        {
            switch (TYPE)
            {
                case 0:
                    return "반복 없음";
                case 1:
                    return "Normal mode";
                case 2:
                    return "通常モード";
            }
            return "Normal mode";
        }

        //Play all files repeatedly.
        public static string RepeatAllSongs()
        {
            switch (TYPE)
            {
                case 0:
                    return "전체반복";
                case 1:
                    return "Repeat all songs";
                case 2:
                    return "すべての曲を繰り返し";
            }
            return "Repeat all songs";
        }

        //Play one song repeatedly.
        public static string RepeatOneSong()
        {
            switch (TYPE)
            {
                case 0:
                    return "한곡만 반복";
                case 1:
                    return "Repeat single song";
                case 2:
                    return "1曲を繰り返し";
            }
            return "Repeat all songs";
        }

        public static string Previous()
        {
            switch (TYPE)
            {
                case 0:
                    return "이전";
                case 1:
                    return "Previous";
                case 2:
                    return "前";
            }
            return "Previous";
        }

        public static string l_Play()
        {
            switch (TYPE)
            {
                case 0:
                    return "재생";
                case 1:
                    return "Play";
                case 2:
                    return "再生";
            }
            return "Play";
        }

        public static string l_Pause()
        {
            switch (TYPE)
            {
                case 0:
                    return "일시정지";
                case 1:
                    return "Pause";
                case 2:
                    return "ブレーク";
            }
            return "Pause";
        }


        public static string Next()
        {
            switch (TYPE)
            {
                case 0:
                    return "다음";
                case 1:
                    return "Next";
                case 2:
                    return "以下";
            }
            return "Next";
        }

        public static string Save()
        {
            switch (TYPE)
            {
                case 0:
                    return "저장하기";
                case 1:
                    return "Save";
                case 2:
                    return "保存";
            }
            return "Save";
        }


        public static string Open()
        {
            switch (TYPE)
            {
                case 0:
                    return "목록 파일 열기";
                case 1:
                    return "Open";
                case 2:
                    return "以下";
            }
            return "Open";
        }


        public static string Delete_item()
        {
            switch (TYPE)
            {
                case 0:
                    return "지우기";
                case 1:
                    return "Delete";
                case 2:
                    return "削除";
            }
            return "Delete";
        }


        public static string MoreItems()
        {
            switch (TYPE)
            {
                case 0:
                    return "더 찾아보기";
                case 1:
                    return "More items";
                case 2:
                    return "詳細をみる";
            }
            return "More items";
        }

        public static string TopMost()
        {
            switch (TYPE)
            {
                case 0:
                    return "항상 위";
                case 1:
                case 2:
                    return "Top Most";
            }
            return "Top Most";
        }
        public static string SelectAllItems()
        {
            switch (TYPE)
            {
                case 0:
                    return "전체 선택";
                case 1:
                case 2:
                    return "Select All Items";
            }
            return "Select All Items";
        }

        public static string AddSelectedItems()
        {
            switch (TYPE)
            {
                case 0:
                    return "선택 추가";
                case 1:
                case 2:
                    return "Add Selected Items";
            }
            return "Add Selected Items";
        }

        public static string SetBookMark()
        {
            switch (TYPE)
            {
                case 0:
                    return "플레이리스트 즐겨찾기 추가";
                case 1:
                case 2:
                    return "Add Bookmark playlist";
            }
            return "Add Bookmark playlist";
        }

        public static string SetWarnShare()
        {
            switch(TYPE)
            {
                case 0:
                    return "올린 플레이 리스트들은 임시적으로 서버에 보관합니다. \n영구적으로 보관하려면 [플레이리스트 즐겨찾기]에 보관해두세요.";
                case 1:
                case 2:
                    return "Orange Player saves the playlists which are uploaded by users on server temporarily. \n If you want to save the playlists permanently, you need to add [My favorite list]";

            }
            return "Orange Player saves the playlists which are uploaded by users on server temporarily. \n If you want to save the playlists permanently, you need to add [My favorite list]";

        }
    }
}
