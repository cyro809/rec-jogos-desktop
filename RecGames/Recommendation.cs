using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecGames
{
    class Recommendation
    {
        Game game = new Game();
        public string title;
        HtmlDocument htmlDocument = new HtmlDocument();
        List<string> tags = new List<string>();

        const string LocalDataBasePath = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=|DataDirectory|GamesInfo.mdf;Integrated Security=True; Connection Timeout=30";
        const string DefineGameTagsQuery = "SELECT DISTINCT t.Tag FROM Game_Tags AS gt, Tags As t WHERE gt.Id_game = @gameId AND gt.Id_tags = t.Id";
        const string GetRecommendedGamesQuery = "SELECT DISTINCT g.* FROM Game AS g, Game_Tags AS gt WHERE g.Id = gt.Id_game AND gt.Id_tags = @tagId";
        const string GetTagsFromMostPlayedGamesQuery = "SELECT t.Id FROM Game_Tags AS gt, Tags as t WHERE gt.Id_game = @gameId AND gt.Id_tags = t.Id";

        SqlConnection sqlConnection = new SqlConnection(LocalDataBasePath);

        public string AppDetails { get; set; }


        public List<int> GetTagsFromMostPlayedGames(Player player)
        {
            List<int> tags = new List<int>();

            for (int i = 0; i < player.OwnedGames.Count; i++)
            {
                if (player.OwnedGames.ElementAt(i).Value > 0)
                {
                    SqlCommand sqlCommand = new SqlCommand(GetTagsFromMostPlayedGamesQuery, sqlConnection);

                    sqlCommand.Parameters.Add("@gameId", SqlDbType.Int).Value = player.OwnedGames.ElementAt(i).Key;

                    //as vezes acontece System.Data.SqlClient.SqlException' por Tempo Limite de Conexão Expirado
                    sqlConnection.Open();

                    SqlDataReader sqlDataRader = sqlCommand.ExecuteReader();

                    if (sqlDataRader.HasRows)
                    {
                        while (sqlDataRader.Read())
                        {
                            tags.Add(sqlDataRader.GetInt32(0));
                        }
                    }

                    sqlDataRader.Close();
                    sqlConnection.Close();
                }
            }

            return tags;
        }

        public List<Game> GetRecommendedGames(Player player)
        {
            List<Game> recommendedGames = new List<Game>();

            for (int i = 0; i < player.DefiningTags.Count; i++)
            {
                SqlCommand sqlCommand = new SqlCommand(GetRecommendedGamesQuery, sqlConnection);

                sqlCommand.Parameters.Add("@tagId", SqlDbType.Int).Value = player.DefiningTags.Keys.ElementAt(i);

                sqlConnection.Open();

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        AddRecommendedGames(player, sqlDataReader, recommendedGames);
                    }
                }
                sqlDataReader.Close();
                sqlConnection.Close();
            }

            return recommendedGames;
        }

        private void AddRecommendedGames(Player player, SqlDataReader sqlDataReader, List<Game> recommendedGames)
        {
            Game game = new Game();
            game.SteamAppId = sqlDataReader.GetInt32(0);
            if (!player.OwnedGames.Keys.Contains(game.SteamAppId))
            {
                game.Name = sqlDataReader.GetString(1);

                Platforms platforms = new Platforms();
                platforms.PlatformsSupported = sqlDataReader.GetString(3);
                game.Platforms = platforms;

                List<string> developers = new List<string>();
                developers.Add(sqlDataReader.GetString(4));
                game.Developers = developers;

                List<string> publishers = new List<string>();
                publishers.Add(sqlDataReader.GetString(5));
                game.Publishers = publishers;

                game.TotalRecommendations = sqlDataReader.GetInt32(6); ;

                game.MetacriticScore = sqlDataReader.GetInt32(7); ;

                DefineGameTags(game);
                recommendedGames.Add(game);
            }
        }

        public void DefineGameTags(Game game)
        {
            SqlConnection sqlConnection2 = new SqlConnection(LocalDataBasePath);

            SqlCommand sqlCommand = new SqlCommand(DefineGameTagsQuery, sqlConnection2);

            sqlCommand.Parameters.Add("@gameId", SqlDbType.Int).Value = game.SteamAppId;

            sqlConnection2.Open();

            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

            if (sqlDataReader.HasRows)
            {
                while (sqlDataReader.Read())
                {
                    game.Tags.Add(sqlDataReader.GetString(0));
                }
            }

            sqlDataReader.Close();
            sqlConnection2.Close();
        }

        public List<int> RecommendationsScore(List<Game> recommendedGames, List<int> gamerTags, string id)
        {

            Dictionary<int, float> recommendedGameScore = new Dictionary<int, float>();
            List<int> gameTagsCount = new List<int>();
            Dictionary<int, int> gameMetacritic = new Dictionary<int, int>();
            Dictionary<int, int> gameRecommendations = new Dictionary<int, int>();

            foreach (Game game in recommendedGames)
            {
                gameTagsCount.Add(game.SteamAppId);

                try
                {
                    gameMetacritic.Add(game.SteamAppId, game.MetacriticScore);
                    gameRecommendations.Add(game.SteamAppId, game.TotalRecommendations);
                }
                catch (System.ArgumentException)
                {

                }
            }

            var frequency = gameTagsCount.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());

            foreach (KeyValuePair<int, int> pair in frequency)
            {
                float recommendation_score = 0.0f;

                recommendation_score += pair.Value * 10.0f;
                recommendation_score += gameMetacritic[pair.Key] * 0.25f;
                recommendation_score += gameRecommendations[pair.Key] * 0.000025f;

                recommendedGameScore.Add(pair.Key, recommendation_score);
            }

            List<int> topRecommendedGames = new List<int>();

            //alterar o modo como o diretorio eh acessado
            using (StreamWriter arquivo = File.AppendText(@"recommendation\player " + id + ".txt"))
            {
                foreach (KeyValuePair<int, float> pair in recommendedGameScore.OrderByDescending(key => key.Value))
                {
                    arquivo.WriteLine("App ID: " + pair.Key.ToString() + " Score: " + pair.Value.ToString() + "\n");

                    topRecommendedGames.Add(pair.Key);
                }
            }

            return topRecommendedGames;
        }
    }
}
