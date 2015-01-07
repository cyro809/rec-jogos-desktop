using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace RecGames
{
    class Game
    {
        public int steam_appid { get; set; }
        public string name { get; set; }
        public Price_overview price_overview { get; set; }
        public string controller_support { get; set; }
        public Platforms platforms { get; set; }
        public List<string> developers { get; set; }
        public List<string> publishers { get; set; }
        public Categories[] categories { get; set; }
        public Recommendations recommendations { get; set; }
        public Metacritic metacritic { get; set; }
        public List<string> tags = new List<string>();
        public float recommendation_score = 0.0f;
        public String developersString()
        {
            try
            {
                string dev = developers[0];
                if (developers.Count > 1)
                {
                    for (int i = 1; i < developers.Count; i++)
                    {
                        dev += "," + developers[i];
                    }
                }
                return dev;
            }
            catch(System.NullReferenceException)
            {
                return "";
            }
            
            
        }
        public String publishersString()
        {
            try
            {
                string pub = publishers[0];
                if (publishers.Count > 1)
                {
                    for (int i = 1; i < publishers.Count; i++)
                    {
                        pub += "," + publishers[i];
                    }
                }

                return pub;
            }
            catch (System.NullReferenceException)
            {
                return "";
            }
        }
    }
}
