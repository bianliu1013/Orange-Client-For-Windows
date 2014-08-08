using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orange.Util
{
    class LanguagePack
    {
        // Kor    0
        // Eng    1
        // Jap    2
        // French 3

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
                case 3:
                    return "Aléatoire";
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
                    return "曲順で再生";
                case 3:
                    return "Aléatoire off";
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
                    return "繰り返し無し";
                case 3:
                    return "Répéter par défaut";
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
                    return "繰り返し";
                case 3:
                    return "Répéter";
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
                    return "1回繰り返し";
                case 3:
                    return "Répéter une fois";
            }
            return "Repeat single songs";
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
                case 3:
                    return "Précédent";
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
                case 3:
                    return "jouer";
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
                case 3:
                    return "pause";

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
                case 3:
                    return "suivant";
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
                    return "Save playlist";
                case 2:
                    return "保存";
                case 3:
                    return "Sauvegarder la liste de lecture";
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
                    return "Load playlist";
                case 2:
                    return "読み込む";
                case 3:
                    return "Télécharger la liste de lecture";
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
                case 3: return "Supprimer";
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
                case 3: return "plus d'articles";
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
                case 3:
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
                    return "Select All Items";
                case 2:
                    return "全曲選択";
                case 3:
                    return "Select All Items";
                    //return "Sélectionner tous les éléments";
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
                    return "Add Selected Items";
                case 2:
                    return "プレイリストに追加";
                case 3:
                    return "Add Selected Items";
                    //return "Ajouter les éléments sélectionnés";
            }
            return "Add Selected Items";
        }

        public static string PlaylistControl_MoveToTop()
        {
            switch (TYPE)
            {
                case 0:
                    return "맨 위로";
                case 1:
                    return "Move to top";
                case 2:
                    return "一番上へ移動";
                case 3:
                    return "Haut de la page ";
            }
            return "Move to top";
        }

        public static string PlaylistControl_Previous()
        {
            switch (TYPE)
            {
                case 0:
                    return "위로";
                case 1:
                    return "Previous";
                case 2:
                    return "上へ移動";
                case 3:
                    return "Précédent";
            }
            return "Previous";
        }

        public static string PlaylistControl_Next()
        {
            switch (TYPE)
            {
                case 0:
                    return "아래로";
                case 1:
                    return "Next";
                case 2:
                    return "下へ移動";
                case 3:
                    return "Suivant";
            }
            return "Next";
        }

        public static string PlaylistControl_SkipToLast()
        {
            switch (TYPE)
            {
                case 0:
                    return "맨 아래로";
                case 1:
                    return "Skip to last";
                case 2:
                    return "一番下へ移動";
                case 3:
                    return "Passer à la dernière piste";
            }
            return "Skip to last";
        }

        public static string SetBookMark()
        {
            switch (TYPE)
            {
                case 0:
                    return "플레이리스트 즐겨찾기 추가";
                case 1:
                case 2:
                case 3:
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
                case 3:
                    return "Orange Player sauvegarde vos listes de lecture qui ont été téléchargées temporairement au serveur par d'autres usagers. Si vous désirez sauvegarder une liste de lecture de façon permanente, vous devez ajouter \"Ma liste favorite\".";

            }
            return "Orange Player saves the playlists which are uploaded by users on server temporarily. \n If you want to save the playlists permanently, you need to add [My favorite list]";

        }
    }
}
