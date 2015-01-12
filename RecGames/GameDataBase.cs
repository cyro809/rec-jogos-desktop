using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Data.Sql;
namespace RecGames
{
    class GameDataBase
    {
        Game game = new Game();
        public string titulo;
        HtmlDocument doc = new HtmlDocument();
        List<string> tags = new List<string>();
        SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=|DataDirectory|GamesInfo.mdf;Integrated Security=True");
        public string appDetails { get; set; }

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
            using (WebClient client = new WebClient())
            {
                string html = client.DownloadString("http://store.steampowered.com/app/" + id);

                doc.LoadHtml(html);
                appDetails = client.DownloadString(@"http://store.steampowered.com/api/appdetails/?appids=" + id);

                string id_string = id.ToString();

                id_string = "\"" + id_string + "\"";

                appDetails = Regex.Replace(appDetails, String.Concat("{", id_string, ":", "{", @"""success""", ":true,", @"""data""", ":"), "");
                appDetails = Regex.Replace(appDetails, @"}}}}", "}}");
            }

            //Tags
            HtmlNode no = doc.DocumentNode.SelectSingleNode("//*[@id='game_highlights']/div[2]/div/div[5]/div[2]");

            try
            {
                string temp = no.InnerText;

                titulo = Regex.Replace(temp, "\t", "");
                titulo = Regex.Replace(titulo, @"( |\r?\n)\1+", "$1");
                tags = titulo.Split('\n').ToList();
            }
            catch (System.NullReferenceException)
            {
                Console.WriteLine("Não possui tags");
            }

            game = JsonConvert.DeserializeObject<Game>(appDetails);
            game.tags = tags;

            saveGameDB();
        }

        public List<int> GetTagsMostPlayedGames(Player player)
        {
            List<int> tags = new List<int>();

            for (int i = 0; i < player.ownedGames.Count; i++)
            {
                if (player.ownedGames.ElementAt(i).Value > 0)
                {
                    //fazer a querie das tags
                    string sql = "SELECT t.Id FROM Game_Tags AS gt, Tags as t WHERE gt.Id_game = @gameId AND gt.Id_tags = t.Id";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    
                    cmd.Parameters.Add("@gameId", SqlDbType.Int).Value = player.ownedGames.ElementAt(i).Key;

                    con.Open();

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            tags.Add(reader.GetInt32(0));
                        }
                    }

                    reader.Close();
                    con.Close();
                }
            }
            
            return tags;
        }

        public List<Game> getRecommendedGames(Player player)
        {
            List<Game> recommendedGames = new List<Game>();

            for(int i = 0; i < player.definingTags.Count; i++) {
                string sql = "SELECT DISTINCT g.* FROM Game AS g, Game_Tags AS gt WHERE g.Id = gt.Id_game AND gt.Id_tags = @tagId";
                SqlCommand cmd = new SqlCommand(sql, con);
                
                cmd.Parameters.Add("@tagId", SqlDbType.Int).Value = player.definingTags.Keys.ElementAt(i);

                con.Open();
                
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Game game = new Game();
                        game.SteamAppId = reader.GetInt32(0);
                        if (!player.ownedGames.Keys.Contains(game.SteamAppId))
                        {
                            game.Name = reader.GetString(1);

                            Platforms platforms = new Platforms();
                            platforms.platformsSupported = reader.GetString(3);
                            game.Platforms = platforms;
                            
                            List<string> developers = new List<string>();
                            developers.Add(reader.GetString(4));
                            game.Developers = developers;
                            
                            List<string> publishers = new List<string>();
                            publishers.Add(reader.GetString(5));
                            game.Publishers = publishers;
                            
                            game.TotalRecommendations = reader.GetInt32(6); ;

                            game.MetacriticScore = reader.GetInt32(7); ;
                            
                            DefineGameTags(game);
                            recommendedGames.Add(game);
                        }
                    }
                }
                reader.Close();
                con.Close();
            }

            return recommendedGames;
        }

        public void DefineGameTags(Game game)
        {
            SqlConnection con2 = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=|DataDirectory|GamesInfo.mdf;Integrated Security=True");
            string sql = "SELECT DISTINCT t.Tag FROM Game_Tags AS gt, Tags As t WHERE gt.Id_game = @gameId AND gt.Id_tags = t.Id";
            SqlCommand cmd = new SqlCommand(sql, con2);

            cmd.Parameters.Add("@gameId", SqlDbType.Int).Value = game.SteamAppId;

            con2.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    game.tags.Add(reader.GetString(0));
                }
            }

            reader.Close();
            con2.Close();
        }

        public List<int> RecommendationsScore(List<Game> recommendedGames, List<int> gamerTags, string id)
        {
            
            Dictionary<int,float> recommendedGameScore = new Dictionary<int,float>();
            List<int> gameTagsCount = new List<int>();
            Dictionary<int, int> game_metacritic = new Dictionary<int, int>();
            Dictionary<int, int> game_recommendations = new Dictionary<int, int>();

            foreach(Game game in recommendedGames)
            {
                gameTagsCount.Add(game.SteamAppId);

                try
                {
                    game_metacritic.Add(game.SteamAppId, game.MetacriticScore);
                    game_recommendations.Add(game.SteamAppId, game.TotalRecommendations);
                }
                catch(System.ArgumentException)
                {

                }
            }

            var frequency = gameTagsCount.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());

            foreach(KeyValuePair<int,int> p in frequency)
            {
                float recommendation_score = 0.0f;

                recommendation_score += p.Value * 10.0f;
                recommendation_score += game_metacritic[p.Key] * 0.25f;
                recommendation_score += game_recommendations[p.Key] * 0.000025f;

                recommendedGameScore.Add(p.Key, recommendation_score);
            }

            List<int> topRecommendedGames = new List<int>();

            //alterar o modo como o diretorio eh acessado
            using (StreamWriter arquivo = File.AppendText(@"recommendation\player "+id+".txt"))
            {
                foreach(KeyValuePair<int,float> p in recommendedGameScore.OrderByDescending(key => key.Value))
                {
                    arquivo.WriteLine("App ID: " + p.Key.ToString() + " Score: " + p.Value.ToString() + "\n");

                    topRecommendedGames.Add(p.Key);
                }
            }

            return topRecommendedGames;
        }

        public void saveGameDB()
        {
            string sql = "INSERT INTO game (id, name, metacritic, recommendations, platforms, publishers, developers) VALUES (@id, @name,@metacritic, @recommendations, @platforms, @publishers, @developers)";
            SqlCommand cmd = new SqlCommand(sql, con);

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = game.SteamAppId;
            cmd.Parameters.Add("@name", SqlDbType.VarChar, 50).Value = game.Name;

            try
            {
                cmd.Parameters.Add("@metacritic", SqlDbType.Int).Value = game.MetacriticScore;
            }
            catch (System.NullReferenceException)
            {
                cmd.Parameters.RemoveAt("@metacritic");
                cmd.Parameters.Add("@metacritic", SqlDbType.Int).Value = -1;
            }

            try
            {
                cmd.Parameters.Add("@recommendations", SqlDbType.Int).Value = game.TotalRecommendations;
            }
            catch (System.NullReferenceException)
            {
                cmd.Parameters.RemoveAt("@recommendations");
                cmd.Parameters.Add("@recommendations", SqlDbType.Int).Value = -1;
            }

            cmd.Parameters.Add("@platforms", SqlDbType.VarChar, 50).Value = game.Platforms.platformsString();
            cmd.Parameters.Add("@publishers", SqlDbType.VarChar, 50).Value = game.ShowPublishers();
            cmd.Parameters.Add("@developers", SqlDbType.VarChar, 50).Value = game.ShowDevelopers();

            con.Open();

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (System.Data.SqlClient.SqlException)
            {

            }

            con.Close();
            SavePrice();
        }

        public void SavePrice()
        {
            con.Open();

            string sql = "INSERT INTO price(id,currency,value) VALUES(@id,@currency,@value)";
            SqlCommand cmd = new SqlCommand(sql, con);

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = game.SteamAppId;

            try
            {
                cmd.Parameters.Add("@currency", SqlDbType.VarChar, 40).Value = game.PriceOverview.Currency;
                cmd.Parameters.Add("@value", SqlDbType.Int).Value = game.PriceOverview.Final;
            }
            catch (System.NullReferenceException)
            {
                cmd.Parameters.RemoveAt("@currency");
                cmd.Parameters.Add("@currency", SqlDbType.VarChar, 40).Value = "Free";
                cmd.Parameters.Add("@value", SqlDbType.Int).Value = 0;
            }

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (System.Data.SqlClient.SqlException)
            {

            }

            con.Close();
            SaveTagDB();
        }

        public void SaveTagDB()
        {
            con.Open();

            string sql = "INSERT INTO tags VALUES (@tag)";
            SqlCommand cmd = new SqlCommand(sql, con);

            for (int i = 1; i < tags.Count - 2; i++)
            {
                try
                {
                    cmd = new SqlCommand(sql, con);
                    cmd.Parameters.Add("@tag", SqlDbType.VarChar, 40).Value = tags[i];
                    cmd.ExecuteNonQuery();
                }
                catch (System.Data.SqlClient.SqlException)
                {
                    Console.WriteLine("Duplicate tag");
                }
            }

            String sql2 = @"INSERT INTO [Game_tags] (id_game,id_tags) VALUES(@id, (SELECT Id FROM TAGS Where Tag=@tag))";
            
            for (int i = 1; i < tags.Count - 2; i++)
            {
                cmd = new SqlCommand(sql2, con);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = game.SteamAppId;
                cmd.Parameters.Add("@tag", SqlDbType.VarChar, 40).Value = tags[i];
                cmd.ExecuteScalar();
            }

            tags.Clear();
            con.Close();
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

            doc = new HtmlDocument();
        }

        
    }
}
