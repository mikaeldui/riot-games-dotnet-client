using System.ComponentModel;

namespace RiotGames.TeamfightTactics;

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