using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Data.SqlClient;
using System.Data;

namespace RecGames
{
    class PlayerInfo
    {
        private readonly int m_QuantidadeTagsFrequentes = 5;
        private const string m_SteamKey = "3E2BA9478DC190757ABE4D1DABEA9802";
        private string m_PlayerDetails, m_OwnedGames, m_RecentlyPlayedGames;
        private string m_SteamId;
        private HtmlDocument htmlDocument = new HtmlDocument();

        public PlayerInfo(string id)
        {
            m_SteamId = id;
        }

        public PlayerInfo() { }

        public void GetPlayerInfo(Player player)
        {
            using (WebClient client = new WebClient())
            {
                //steamId = "76561197960435530";
                m_PlayerDetails = client.DownloadString(@"http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=" + m_SteamKey + "&steamids=" + m_SteamId);
            }

            JObject jObject = JObject.Parse(m_PlayerDetails);
            JObject jObjectResponse = (JObject)jObject["response"];
            JArray jArrayPlayers = (JArray)jObjectResponse["players"];

            if (jArrayPlayers.Count == 0)
            {
                throw new LoginException("Invalid SteamID");
            }

            JObject jObjectPlayers = (JObject)jArrayPlayers[0];

            player.SteamId = (long)jObjectPlayers["steamid"];
            player.SteamName = (string)jObjectPlayers["personaname"];
            player.RealName = (string)jObjectPlayers["realname"];
            player.ProfileUrl = (string)jObjectPlayers["profileurl"];

            player.ToString();
        }

        public void GetPlayerOwnedGames(Player player)
        {
            using (WebClient client = new WebClient())
            {
                //steamId = "76561197960435530";
                m_OwnedGames = client.DownloadString(@"http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key=" + m_SteamKey + "&steamid=" + m_SteamId + "&include_appinfo=1&include_played_free_games=1&format=json");
            }

            JObject jObject = JObject.Parse(m_OwnedGames);
            JObject jObjectResponse = (JObject)jObject["response"];
            JArray jArrayOwnedGames = (JArray)jObjectResponse["games"];

            for (int i = 0; i < jArrayOwnedGames.Count; i++)
            {
                JObject jObjectOwnedGames = (JObject)jArrayOwnedGames[i];
                int appId = (int)jObjectOwnedGames["appid"];
                int playtime = (int)jObjectOwnedGames["playtime_forever"];
                string name = (string)jObjectOwnedGames["name"];

                player.OwnedGames.Add(appId, playtime);
                player.MyGames.Add(name);
            }

            player.OwnedGames = player.OwnedGames.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            int count = player.OwnedGames.Count;

            player.ToString();
        }

        public void GetPlayerRecentlyPlayedGames(Player player)
        {
            using (WebClient client = new WebClient())
            {
                m_RecentlyPlayedGames = client.DownloadString(@"http://api.steampowered.com/IPlayerService/GetRecentlyPlayedGames/v0001/?key=" + m_SteamKey + "&steamid=" + m_SteamId);
            }

            JObject jObject = JObject.Parse(m_RecentlyPlayedGames);
            JObject jObjectResponse = (JObject)jObject["response"];
            int totalCount = (int)jObjectResponse["total_count"];
            JArray jArrayRecentlyPlayedGames = (JArray)jObjectResponse["games"];

            if (totalCount > 0)
            {
                for (int i = 0; i < jArrayRecentlyPlayedGames.Count; i++)
                {
                    JObject jObjectRecentlyPlayedGames = (JObject)jArrayRecentlyPlayedGames[i];
                    RecentlyPlayedGames recentlyPlayedGames = new RecentlyPlayedGames();

                    recentlyPlayedGames.SteamAppId = (int)jObjectRecentlyPlayedGames["appid"];
                    recentlyPlayedGames.Name = (string)jObjectRecentlyPlayedGames["name"];
                    recentlyPlayedGames.Playtime = (int)jObjectRecentlyPlayedGames["playtime_forever"];
                    recentlyPlayedGames.PlaytimeTwoWeeks = (int)jObjectRecentlyPlayedGames["playtime_2weeks"];

                    player.RecentlyPlayedGames.Add(recentlyPlayedGames);
                }

                player.RecentlyPlayedGames = player.RecentlyPlayedGames.OrderByDescending(x => x.PlaytimeTwoWeeks).ToList();
            }

            player.ToString();
        }

        public void GetPlayerDefiningTags(List<int> tags, Player player)
        {
            var frequency = tags.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());

            frequency.ToString();
            frequency = frequency.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            SqlConnection sqlConnection = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=|DataDirectory|GamesInfo.mdf;Integrated Security=True");

            for (int i = 0; i < m_QuantidadeTagsFrequentes; i++)
            {
                string sqlQuery = "SELECT t.* FROM Tags as t WHERE t.Id = @tagId";
                SqlCommand sqlCommand = new SqlCommand(sqlQuery, sqlConnection);

                sqlCommand.Parameters.Add("@tagId", SqlDbType.Int).Value = frequency.Keys.ElementAt(i);

                sqlConnection.Open();

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        player.DefiningTags.Add(sqlDataReader.GetInt32(0), sqlDataReader.GetString(1));

                        Console.WriteLine(sqlDataReader.GetString(1));
                    }
                }

                sqlDataReader.Close();
                sqlConnection.Close();
            }
        }
    }
}
