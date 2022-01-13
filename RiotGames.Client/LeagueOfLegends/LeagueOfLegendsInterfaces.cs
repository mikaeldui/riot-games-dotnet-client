using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.LeagueOfLegends
{
    public interface ILeagueOfLegendsObject : IRiotGamesObject
    {
    }

    public interface ILeagueOfLegendsLeagueId : ILeagueOfLegendsObject
    {
        public string LeagueId { get; set; }
    }

    public interface ILeagueOfLegendsMatchId : ILeagueOfLegendsObject
    {
        public string MatchId { get; set; }
    }

    public interface ILeagueOfLegendsTournamentId : ILeagueOfLegendsObject
    {
        public int TournamentId { get; set; }
    }
}
