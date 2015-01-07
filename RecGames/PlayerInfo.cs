using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.Data.SqlClient;
using System.Data;
using System.Data.Sql;

namespace RecGames
{
    class PlayerInfo
    {
        private const int quantidadeTagsFrequentes = 5;
        private const string steamKey = "3E2BA9478DC190757ABE4D1DABEA9802";
        private string playerDetails, ownedGames, recentlyPlayedGames;
        private string steamId;
        private HtmlDocument doc = new HtmlDocument();
        public PlayerInfo(string id)
        {
            steamId = id;
        }
        public PlayerInfo() { }
        public void getPlayerInfo(Player player)
        {
            using (WebClient client = new WebClient())
            {
                //steamId = "76561197960435530";
                playerDetails = client.DownloadString(@"http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=" + steamKey + "&steamids=" + steamId);
            }

            JObject jObject = JObject.Parse(playerDetails);
            JObject jObjectResponse = (JObject)jObject["response"];
            JArray jArrayPlayers = (JArray)jObjectResponse["players"];
            if (jArrayPlayers.Count == 0) 
            {
                throw new LoginException("Invalid SteamID");
            }
            JObject jObjectPlayers = (JObject)jArrayPlayers[0];

            player.steam_id = (long)jObjectPlayers["steamid"];
            player.steam_name = (string)jObjectPlayers["personaname"];
            player.real_name = (string)jObjectPlayers["realname"];
            player.profile_url = (string)jObjectPlayers["profileurl"];

            player.ToString();
        }

        public void getPlayerOwnedGames(Player player)
        {
            using (WebClient client = new WebClient())
            {
                //steamId = "76561197960435530";
                ownedGames = client.DownloadString(@"http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key=" + steamKey + "&steamid=" + steamId + "&include_appinfo=1&include_played_free_games=1&format=json");
            }

            JObject jObject = JObject.Parse(ownedGames);
            JObject jObjectResponse = (JObject)jObject["response"];
            JArray jArrayOwnedGames = (JArray)jObjectResponse["games"];

            for (int i = 0; i < jArrayOwnedGames.Count; i++)
            {
                JObject jObjectOwnedGames = (JObject)jArrayOwnedGames[i];
                int appId = (int)jObjectOwnedGames["appid"];
                int playtime = (int)jObjectOwnedGames["playtime_forever"];
                string name = (string)jObjectOwnedGames["name"];

                player.ownedGames.Add(appId, playtime);
                player.myGames.Add(name);
            }

            player.ownedGames = player.ownedGames.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            int count = player.ownedGames.Count;
            player.ToString();
        }

        public void getPlayerRecentlyPlayedGames(Player player)
        {
            using (WebClient client = new WebClient())
            {

                //steamId = "76561197960435530";
                //steamId = "76561198064458950";
                recentlyPlayedGames = client.DownloadString(@"http://api.steampowered.com/IPlayerService/GetRecentlyPlayedGames/v0001/?key=" + steamKey + "&steamid=" + steamId);
            }

            JObject jObject = JObject.Parse(recentlyPlayedGames);
            JObject jObjectResponse = (JObject)jObject["response"];
            int totalCount = (int)jObjectResponse["total_count"];
            JArray jArrayRecentlyPlayedGames = (JArray)jObjectResponse["games"];

            if (totalCount > 0)
            {
                for (int i = 0; i < jArrayRecentlyPlayedGames.Count; i++)
                {
                    JObject jObjectRecentlyPlayedGames = (JObject)jArrayRecentlyPlayedGames[i];
                    RecentlyPlayedGames playedGames = new RecentlyPlayedGames();
                    playedGames.steam_appid = (int)jObjectRecentlyPlayedGames["appid"];
                    playedGames.name = (string)jObjectRecentlyPlayedGames["name"];
                    playedGames.playtime = (int)jObjectRecentlyPlayedGames["playtime_forever"];
                    playedGames.playtime2weeks = (int)jObjectRecentlyPlayedGames["playtime_2weeks"];

                    player.recentlyPlayedGames.Add(playedGames);
                }

                player.recentlyPlayedGames = player.recentlyPlayedGames.OrderByDescending(x => x.playtime2weeks).ToList();
            }

            player.ToString();
        }

        public void getPlayerDefiningTags(List<int> tags, Player player) {
            var frequency = tags.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
            frequency.ToString();
            frequency = frequency.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=|DataDirectory|GamesInfo.mdf;Integrated Security=True");
            for (int i = 0; i < quantidadeTagsFrequentes; i++)
            {
                string sql = "SELECT t.* FROM Tags as t WHERE t.Id = @tagId";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add("@tagId", SqlDbType.Int).Value = frequency.Keys.ElementAt(i);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        player.definingTags.Add(reader.GetInt32(0), reader.GetString(1));
                        Console.WriteLine(reader.GetString(1));
                    }
                }
                reader.Close();
                con.Close();
            }
        }
    }
}
