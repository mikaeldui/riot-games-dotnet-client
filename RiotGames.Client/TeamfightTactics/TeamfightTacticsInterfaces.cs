using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.TeamfightTactics
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface ITeamfightTacticsObject : IRiotGamesObject
    {
    }

    public interface ILeagueId : ITeamfightTacticsObject
    {
        public string LeagueId { get; set; }
    }

    public interface IMatchId : ITeamfightTacticsObject
    {
        public string MatchId { get; set; }
    }
}
