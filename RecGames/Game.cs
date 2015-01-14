using System;
using System.Collections.Generic;

namespace RecGames
{
    class Game
    {
        public Game()
        {
            Tags = new List<string>();
        }

        public int SteamAppId { get; set; }
        public string Name { get; set; }
        public PriceOverview PriceOverview { get; set; }
        public string ControllerSupport { get; set; }
        public Platforms Platforms { get; set; }
        public List<string> Developers { get; set; }
        public List<string> Publishers { get; set; }
        public List<string> Tags { get; set; }
        public Categories[] Categories { get; set; }
        public int TotalRecommendations { get; set; }
        public int MetacriticScore { get; set; }

        public string FormatDevelopers()
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

        public string FormatPublishers()
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
