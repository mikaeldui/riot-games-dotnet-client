using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.LeagueOfLegends
{
    public interface ISummonerId
    {
        public string? SummonerId { get; set; }
    }

    public interface ITournamentId
    {
        public int? TournamentId { get; set; }
    }
}
