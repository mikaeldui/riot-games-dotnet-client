using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.TeamfightTactics
{
    public interface ITeamfightTacticsObject : IRiotGamesObject
    {
    }

    public interface ITeamfightTacticsLeagueId : ITeamfightTacticsObject
    {
        public string LeagueId { get; set; }
    }

    public interface ITeamfightTacticsMatchId : ITeamfightTacticsObject
    {
        public string MatchId { get; set; }
    }
}
