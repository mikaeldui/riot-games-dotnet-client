using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.LeagueOfLegends
{
    public interface ILeagueOfLegendsLeagueId
    {
        public string LeagueId { get; set; }
    }

    public interface ILeagueOfLegendsMatchId
    {
        public string MatchId { get; set; }
    }

    public interface ILeagueOfLegendsTournamentId
    {
        public int TournamentId { get; set; }
    }
}
