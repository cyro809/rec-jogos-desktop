﻿using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.IO;
using System.Data.SqlClient;
using System.Data;

namespace RecGames
{
    class GameDataBase
    {
        Game game = new Game();
        public string title;
        HtmlDocument htmlDocument = new HtmlDocument();
        List<string> tags = new List<string>();

        const string LocalDataBasePath = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=|DataDirectory|GamesInfo.mdf;Integrated Security=True; Connection Timeout=30";
        const string DefineGameTagsQuerie = "SELECT DISTINCT t.Tag FROM Game_Tags AS gt, Tags As t WHERE gt.Id_game = @gameId AND gt.Id_tags = t.Id";
        const string GetRecommendedGamesQuerie = "SELECT DISTINCT g.* FROM Game AS g, Game_Tags AS gt WHERE g.Id = gt.Id_game AND gt.Id_tags = @tagId";
        const string GetTagsFromMostPlayedGamesQuerie = "SELECT t.Id FROM Game_Tags AS gt, Tags as t WHERE gt.Id_game = @gameId AND gt.Id_tags = t.Id";

        SqlConnection sqlConnection = new SqlConnection(LocalDataBasePath);

        public string AppDetails { get; set; }

        public void ReadList()
        {
            using (StreamReader reader = new StreamReader("gameList.txt"))
            {
                string[] list = reader.ReadToEnd().Split('\n');
                for (int i = 0; i < list.Length; i++)
                {
                    int x = int.Parse(list[i]);

                    Console.WriteLine(list[i]);

                    GetTags(x);
                }
            }
        }

        public void GetTags(int id)
        {
            game = new Game();
            using (WebClient webClient = new WebClient())
            {
                string html = webClient.DownloadString("http://store.steampowered.com/app/" + id);

                htmlDocument.LoadHtml(html);
                AppDetails = webClient.DownloadString(@"http://store.steampowered.com/api/appdetails/?appids=" + id);

                string identification = id.ToString();

                identification = "\"" + identification + "\"";

                AppDetails = Regex.Replace(AppDetails, String.Concat("{", identification, ":", "{", @"""success""", ":true,", @"""data""", ":"), "");
                AppDetails = Regex.Replace(AppDetails, @"}}}}", "}}");
            }

            //Tags
            HtmlNode htmlNode = htmlDocument.DocumentNode.SelectSingleNode("//*[@id='game_highlights']/div[2]/div/div[5]/div[2]");

            try
            {
                string temp = htmlNode.InnerText;

                title = Regex.Replace(temp, "\t", "");
                title = Regex.Replace(title, @"( |\r?\n)\1+", "$1");
                tags = title.Split('\n').ToList();
            }
            catch (System.NullReferenceException)
            {
                Console.WriteLine("Não possui tags");
            }

            game = JsonConvert.DeserializeObject<Game>(AppDetails);
            game.Tags = tags;

            SaveGameDB();
        }

        public List<int> GetTagsFromMostPlayedGames(Player player)
        {
            List<int> tags = new List<int>();

            for (int i = 0; i < player.OwnedGames.Count; i++)
            {
                if (player.OwnedGames.ElementAt(i).Value > 0)
                {
                    SqlCommand sqlCommand = new SqlCommand(GetTagsFromMostPlayedGamesQuerie, sqlConnection);

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
                SqlCommand sqlCommand = new SqlCommand(GetRecommendedGamesQuerie, sqlConnection);

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
            
            SqlCommand sqlCommand = new SqlCommand(DefineGameTagsQuerie, sqlConnection2);

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

        public void SaveGameDB()
        {
            string sqlQuery = "INSERT INTO game (id, name, metacritic, recommendations, platforms, publishers, developers) VALUES (@id, @name,@metacritic, @recommendations, @platforms, @publishers, @developers)";
            SqlCommand sqlCommand = new SqlCommand(sqlQuery, sqlConnection);

            sqlCommand.Parameters.Add("@id", SqlDbType.Int).Value = game.SteamAppId;
            sqlCommand.Parameters.Add("@name", SqlDbType.VarChar, 50).Value = game.Name;

            try
            {
                sqlCommand.Parameters.Add("@metacritic", SqlDbType.Int).Value = game.MetacriticScore;
            }
            catch (System.NullReferenceException)
            {
                sqlCommand.Parameters.RemoveAt("@metacritic");
                sqlCommand.Parameters.Add("@metacritic", SqlDbType.Int).Value = -1;
            }

            try
            {
                sqlCommand.Parameters.Add("@recommendations", SqlDbType.Int).Value = game.TotalRecommendations;
            }
            catch (System.NullReferenceException)
            {
                sqlCommand.Parameters.RemoveAt("@recommendations");
                sqlCommand.Parameters.Add("@recommendations", SqlDbType.Int).Value = -1;
            }

            sqlCommand.Parameters.Add("@platforms", SqlDbType.VarChar, 50).Value = game.Platforms.SupportedPlatforms();
            sqlCommand.Parameters.Add("@publishers", SqlDbType.VarChar, 50).Value = game.FormatPublishers();
            sqlCommand.Parameters.Add("@developers", SqlDbType.VarChar, 50).Value = game.FormatDevelopers();

            sqlConnection.Open();

            try
            {
                sqlCommand.ExecuteNonQuery();
            }
            catch (System.Data.SqlClient.SqlException)
            {

            }

            sqlConnection.Close();
            SavePrice();
        }

        public void SavePrice()
        {
            sqlConnection.Open();

            string sqlQuery = "INSERT INTO price(id,currency,value) VALUES(@id,@currency,@value)";
            SqlCommand sqlCommand = new SqlCommand(sqlQuery, sqlConnection);

            sqlCommand.Parameters.Add("@id", SqlDbType.Int).Value = game.SteamAppId;

            try
            {
                sqlCommand.Parameters.Add("@currency", SqlDbType.VarChar, 40).Value = game.PriceOverview.Currency;
                sqlCommand.Parameters.Add("@value", SqlDbType.Int).Value = game.PriceOverview.Final;
            }
            catch (System.NullReferenceException)
            {
                sqlCommand.Parameters.RemoveAt("@currency");
                sqlCommand.Parameters.Add("@currency", SqlDbType.VarChar, 40).Value = "Free";
                sqlCommand.Parameters.Add("@value", SqlDbType.Int).Value = 0;
            }

            try
            {
                sqlCommand.ExecuteNonQuery();
            }
            catch (System.Data.SqlClient.SqlException)
            {

            }

            sqlConnection.Close();
            SaveTagDB();
        }

        public void SaveTagDB()
        {
            sqlConnection.Open();

            string sqlQuery = "INSERT INTO tags VALUES (@tag)";
            SqlCommand sqlCommand = new SqlCommand(sqlQuery, sqlConnection);

            for (int i = 1; i < tags.Count - 2; i++)
            {
                try
                {
                    sqlCommand = new SqlCommand(sqlQuery, sqlConnection);
                    sqlCommand.Parameters.Add("@tag", SqlDbType.VarChar, 40).Value = tags[i];
                    sqlCommand.ExecuteNonQuery();
                }
                catch (System.Data.SqlClient.SqlException)
                {
                    Console.WriteLine("Duplicate tag");
                }
            }

            String sqlQuery2 = @"INSERT INTO [Game_tags] (id_game,id_tags) VALUES(@id, (SELECT Id FROM TAGS Where Tag=@tag))";

            for (int i = 1; i < tags.Count - 2; i++)
            {
                sqlCommand = new SqlCommand(sqlQuery2, sqlConnection);
                sqlCommand.Parameters.Add("@id", SqlDbType.Int).Value = game.SteamAppId;
                sqlCommand.Parameters.Add("@tag", SqlDbType.VarChar, 40).Value = tags[i];
                sqlCommand.ExecuteScalar();
            }

            tags.Clear();
            sqlConnection.Close();
        }

        public void SaveInfo()
        {
            Console.WriteLine(game.Name);
            Console.WriteLine(game.SteamAppId);

            try
            {
                Console.WriteLine(game.PriceOverview.Currency);
                Console.WriteLine(game.PriceOverview.Final);
            }
            catch (System.NullReferenceException)
            {
                Console.WriteLine("Free-to-play");
            }

            Console.WriteLine(game.Categories[0].Id);
            Console.WriteLine(game.Categories[0].Description);

            using (StreamWriter arquivo = File.AppendText(@"games\" + game.SteamAppId.ToString() + ".txt"))
            {
                arquivo.WriteLine("Name: " + game.Name);
                arquivo.WriteLine("Steam ID: " + game.SteamAppId);

                try
                {
                    arquivo.WriteLine("Price: " + game.PriceOverview.Final + "(" + game.PriceOverview.Currency + ")");
                }
                catch (System.NullReferenceException)
                {
                    arquivo.WriteLine("Price: Free-to-play");
                }

                arquivo.WriteLine("Tags:");

                for (int i = 0; i < tags.Count; i++)
                {
                    arquivo.WriteLine(tags[i]);
                }
            }

            htmlDocument = new HtmlDocument();
        }
    }
}
