using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orange
{
    public class KPopList
    {
        private List<KPopObject> kpopList;

        public KPopList()
        {
            kpopList = new List<KPopObject>();
        }

        public List<KPopObject> getKPopList()
        {
            return kpopList;
        }

        public KPopObject getKPop(int i)
        {
            return kpopList[i];
        }
    }
}
