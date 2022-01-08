using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.TeamfightTactics
{
    public interface ITeamfightTacticsLeagueId
    {
        public string? LeagueId { get; set; }
    }

    public interface ITeamfightTacticsMatchId
    {
        public string? MatchId { get; set; }
    }
}
