using System.Collections.Generic;

namespace RecGames
{
    class Player
    {
        public Player()
        {
            Tags = new List<string>();
            MyGames = new List<string>();
            OwnedGames = new Dictionary<int, int>();
            RecentlyPlayedGames = new List<RecentlyPlayedGames>();
            DefiningTags = new Dictionary<int, string>();
        }

        public long SteamId { get; set; }
        public string SteamName { get; set; }
        public string RealName { get; set; }
        public string ProfileUrl { get; set; }
        public List<string> Tags { get; set; } // nao usado
        public List<string> MyGames { get; private set; }
        public Dictionary<int, int> OwnedGames { get; set; }
        public List<RecentlyPlayedGames> RecentlyPlayedGames { get; set; }
        public Dictionary<int, string> DefiningTags { get; private set; }
    }
}
