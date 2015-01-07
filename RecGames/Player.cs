using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace RecGames
{
    class Player
    {
        public long steam_id { get; set; }
        public string steam_name { get; set; }
        public string real_name { get; set; }
        public string profile_url { get; set; }
        public List<string> tags = new List<string>();
        public List<string> myGames = new List<string>();
        public Dictionary<int, int> ownedGames = new Dictionary<int, int>();
        public List<RecentlyPlayedGames> recentlyPlayedGames = new List<RecentlyPlayedGames>();
        public Dictionary<int, string> definingTags = new Dictionary<int, string>();
    }
}
