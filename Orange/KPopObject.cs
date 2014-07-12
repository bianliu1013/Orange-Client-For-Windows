using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orange
{
    public class KPopObject
    {
        private String title;
        private String singer;

        public KPopObject(String title, String singer)
        {
            setTitle(title);
            setSinger(singer);
        }

        public void setTitle(String title)
        {
            this.title = title;
        }

        public void setSinger(String singer)
        {
            this.singer = singer;
        }

        public String getTitle()
        {
            return title;
        }

        public String getSinger()
        {
            return singer;
        }
    }
}
