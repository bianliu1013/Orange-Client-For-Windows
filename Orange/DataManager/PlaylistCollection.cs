using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orange.DataManager
{
    public class PlaylistCollection : ObservableCollection<PlaylistItem>
    {
        public PlaylistCollection() { }

        public PlaylistCollection(IEnumerable<PlaylistItem> collection) : base(collection) { }
    }
    class Playlist
    {
        public string chart_name { get; set; }
        public List<MusicItem> chart_list { get; set; }
    }

}
