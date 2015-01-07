using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            LoginWindowsForm fLogin = new LoginWindowsForm();

            if (fLogin.ShowDialog() == DialogResult.OK)
            {
                Application.Run(new MainWindowsForm(playerID));
            }
            else
            {
                Application.Exit();
            }
        }

        public static bool validateSteamID(string id) 
        {
            PlayerInfo playerInfo = new PlayerInfo(id);

            try
            {
                playerInfo.getPlayerInfo(player);
            }
            catch(LoginException e)
            {
                throw new LoginException(e.Message);
            }

            return true;
        }

        public static void loadPlayerInformations(string id) {
            GameDataBase g = new GameDataBase();
            PlayerInfo playerInfo = new PlayerInfo(id);
            List<int> tags = new List<int>();

            playerInfo.getPlayerInfo(player);
            playerInfo.getPlayerOwnedGames(player);
            playerInfo.getPlayerRecentlyPlayedGames(player);

            tags = g.getTagsMostPlayedGames(player);
            playerInfo.getPlayerDefiningTags(tags, player);
        }

        public static void beginRecommendation(string id)
        {
            GameDataBase g = new GameDataBase();
            PlayerInfo playerInfo = new PlayerInfo(id);
            List<int> tags = new List<int>();
            List<Game> recommendedGames = new List<Game>();
            List<int> topRecommendedGames = new List<int>();

            recommendedGames = g.getRecommendedGames(player);
            topRecommendedGames = g.recommendationsScore(recommendedGames, tags, id);

            List<Game> gamesToJustify = new List<Game>();

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < recommendedGames.Count; j++)
                {
                    if (recommendedGames.ElementAt(j).steam_appid == topRecommendedGames.ElementAt(i))
                    {
                        gamesToJustify.Add(recommendedGames.ElementAt(j));
                    }
                }
            }

            string gameTags = "";
            for (int i = 0; i < gamesToJustify.ElementAt(0).tags.Count; i++)
            {
                gameTags += gamesToJustify.ElementAt(0).tags.ElementAt(i) + " ";
            }
            string urlGame = String.Format("http://store.steampowered.com/app/{0}/", gamesToJustify.ElementAt(0).steam_appid);
            justification = String.Format("Estamos recomendando o jogo {0} pois vimos que ele tem: {1}. Se quiser saber mais sobre: {2}", gamesToJustify.ElementAt(0).name, gameTags, urlGame);


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
