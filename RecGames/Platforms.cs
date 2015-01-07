using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecGames
{
    class Platforms
    {
        public bool windows { get; set; }
        public bool mac { get; set; }
        public bool linux { get; set; }
        public String supported;
        public String platformsString()
        {
            supported = "";
            if(windows)
            {
                supported += "windows ";
            }
            if(mac)
            {
                supported += "mac ";
            }
            if(linux)
            {
                supported += "linux";
            }
            return supported;
        }
    }
}
