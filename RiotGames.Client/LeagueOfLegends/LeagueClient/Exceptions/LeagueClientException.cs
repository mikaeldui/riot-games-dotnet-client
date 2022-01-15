using System;
using System.Collections.Generic;
using System.Text;

namespace RiotGames.LeagueOfLegends.LeagueClient
{
    public class LeagueClientException : Exception
    {
        public LeagueClientException(string message) : base(message)
        {
        }
    }
}
