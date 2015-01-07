using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecGames
{
    class Price_overview
    {
        public string currency { get; set; }
        public int final { get; set; }
        public Price_overview()
        {
            currency = "Free";
            final = 0;
        }
    }
}
