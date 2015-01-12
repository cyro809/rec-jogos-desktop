using System;

namespace RecGames
{
    class Platforms
    {
        public string platformsSupported;

        public bool Windows { get; set; }
        public bool Mac { get; set; }
        public bool Linux { get; set; }
        
        public string platformsString()
        {
            platformsSupported = String.Empty;
            if(Windows)
            {
                platformsSupported += "Windows ";
            }
            if(Mac)
            {
                platformsSupported += "Mac ";
            }
            if(Linux)
            {
                platformsSupported += "Linux ";
            }
            return platformsSupported;
        }
    }
}
