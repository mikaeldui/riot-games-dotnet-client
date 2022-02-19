using System.ComponentModel;

namespace RiotGames.Valorant;

[EditorBrowsable(EditorBrowsableState.Never)]
public interface IValorantObject : IRiotGamesObject
{
}

public interface IMatchId : IValorantObject
{
    public string MatchId { get; set; }
}