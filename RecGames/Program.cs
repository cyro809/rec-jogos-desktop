using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace RecGames
{
    class Program
    {
        public static Player player = new Player();
        public static string justification;
        public static string playerID;

        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            LoginWindowsForm loginWindowsForm = new LoginWindowsForm();

            if (loginWindowsForm.ShowDialog() == DialogResult.OK)
            {
                Application.Run(new MainWindowsForm(playerID));
            }
            else
            {
                Application.Exit();
            }
        }

        public static bool ValidateSteamID(string id) 
        {
            PlayerInfo playerInfo = new PlayerInfo(id);

            try
            {
                playerInfo.GetPlayerInfo(player);
            }
            catch(LoginException e)
            {
                throw new LoginException(e.Message);
            }

            return true;
        }

        public static void LoadPlayerInformations(string id) {
            GameDataBase gameDataBase = new GameDataBase();
            PlayerInfo playerInfo = new PlayerInfo(id);
            List<int> tags = new List<int>();

            playerInfo.GetPlayerInfo(player);
            playerInfo.GetPlayerOwnedGames(player);
            playerInfo.GetPlayerRecentlyPlayedGames(player);

            tags = gameDataBase.GetTagsFromMostPlayedGames(player);
            playerInfo.GetPlayerDefiningTags(tags, player);
        }

        public static void BeginRecommendation(string id)
        {
            GameDataBase gameDataBase = new GameDataBase();
            PlayerInfo playerInfo = new PlayerInfo(id);
            List<int> tags = new List<int>();
            List<Game> recommendedGames = new List<Game>();
            List<int> topRecommendedGames = new List<int>();

            recommendedGames = gameDataBase.GetRecommendedGames(player);
            topRecommendedGames = gameDataBase.RecommendationsScore(recommendedGames, tags, id);

            List<Game> gamesToJustify = new List<Game>();

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < recommendedGames.Count; j++)
                {
                    if (recommendedGames.ElementAt(j).SteamAppId == topRecommendedGames.ElementAt(i))
                    {
                        gamesToJustify.Add(recommendedGames.ElementAt(j));
                    }
                }
            }

            string gameTags = String.Empty;
            for (int i = 0; i < gamesToJustify.ElementAt(0).Tags.Count; i++)
            {
                gameTags += gamesToJustify.ElementAt(0).Tags.ElementAt(i) + " ";
            }
            string urlGame = String.Format("http://store.steampowered.com/app/{0}/", gamesToJustify.ElementAt(0).SteamAppId);
            justification = String.Format("Estamos recomendando o jogo {0} pois vimos que ele tem: {1}. Se quiser saber mais sobre: {2}", gamesToJustify.ElementAt(0).Name, gameTags, urlGame);


            Console.Write(justification);

            using (StreamWriter arquivo = File.AppendText(@"recommendation\recomendString.txt"))
            {
                arquivo.WriteLine(justification);
            }
            Application.DoEvents();
            System.Threading.Thread.Sleep(2000);
        }
    }
}
