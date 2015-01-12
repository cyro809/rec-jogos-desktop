namespace RecGames
{
    class PriceOverview
    {
        //melhorar nome das variaveis
        public string Currency { get; set; }
        public int Final { get; set; }

        public PriceOverview()
        {
            Currency = "Free";
            Final = 0;
        }
    }
}
