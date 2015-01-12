using System.Collections.Generic;

namespace RecGames
{
    class Player
    {
        //duvida: checar se é ok instanciar variáveis no modelo
        public List<string> tags = new List<string>();
        public List<string> myGames = new List<string>();
        public Dictionary<int, int> ownedGames = new Dictionary<int, int>();
        public List<RecentlyPlayedGames> recentlyPlayedGames = new List<RecentlyPlayedGames>();
        public Dictionary<int, string> definingTags = new Dictionary<int, string>();

        public long SteamId { get; set; }
        public string SteamName { get; set; }
        public string RealName { get; set; }
        public string ProfileUrl { get; set; }
    }
}
