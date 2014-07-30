using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orange.DataManager
{
    public class MyfavoritelistMgr
    {
        private static MyfavoritelistMgr _instance;
        public PlaylistCollection MyfavoriteCollection { get; set;}
        protected MyfavoritelistMgr() {
            MyfavoriteCollection = new PlaylistCollection();
        }

        public static MyfavoritelistMgr instance()
        {
            if (_instance == null)
                _instance = new MyfavoritelistMgr();

            return _instance;
        }
        
    }
}
