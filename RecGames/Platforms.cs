using System;

namespace RecGames
{
    class Platforms
    {
        public string PlatformsSupported { get; set; }
        public bool Windows { get; set; }
        public bool Mac { get; set; }
        public bool Linux { get; set; }
        
        public string SupportedPlatforms()
        {
            PlatformsSupported = String.Empty;
            if(Windows)
            {
                PlatformsSupported += "Windows ";
            }
            if(Mac)
            {
                PlatformsSupported += "Mac ";
            }
            if(Linux)
            {
                PlatformsSupported += "Linux ";
            }
            return PlatformsSupported;
        }
    }
}
