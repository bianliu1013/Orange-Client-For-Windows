using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Orange.DataManager
{
    public class MusicCollection : ObservableCollection<MusicItem>
    {
        public MusicCollection() { }

        public MusicCollection(IEnumerable<MusicItem> collection):base(collection){}
    }
}
