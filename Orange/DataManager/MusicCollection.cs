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
    class MusicJson
    {

        public List<JsonItem> play_list { get; set; }
        public string page_cnt { get; set; }
    }

    public class JsonItem
    {
        public string title { get; set; }
        public string hits_count { get; set; }
    }
}
