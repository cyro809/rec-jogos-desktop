using System;
using System.Collections.Generic;

namespace RecGames
{
    class Game
    {
        public List<string> tags = new List<string>();
        public float recommendation_score = 0.0f;

        public int SteamAppId { get; set; }
        public string Name { get; set; }
        public PriceOverview PriceOverview { get; set; }
        public string ControllerSupport { get; set; }
        public Platforms Platforms { get; set; }
        public List<string> Developers { get; set; }
        public List<string> Publishers { get; set; }
        public Categories[] Categories { get; set; }
        public int TotalRecommendations { get; set; }
        public int MetacriticScore { get; set; }

        public string ShowDevelopers()
        {
            try
            {
                string developer = Developers[0];
                if (Developers.Count > 1)
                {
                    for (int index = 1; index < Developers.Count; index++)
                    {
                        developer += "," + Developers[index];
                    }
                }
                return developer;
            }
            catch(System.NullReferenceException)
            {
                return String.Empty;
            }            
        }

        public string ShowPublishers()
        {
            try
            {
                string publisher = Publishers[0];
                if (Publishers.Count > 1)
                {
                    for (int index = 1; index < Publishers.Count; index++)
                    {
                        publisher += "," + Publishers[index];
                    }
                }

                return publisher;
            }
            catch (System.NullReferenceException)
            {
                return String.Empty;
            }
        }
    }
}
