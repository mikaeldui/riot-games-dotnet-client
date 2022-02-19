using System.ComponentModel;

namespace RiotGames.LegendsOfRuneterra;

[EditorBrowsable(EditorBrowsableState.Never)]
public interface ILegendsOfRuneterraObject : IRiotGamesObject
{
}

public interface IMatchId : ILegendsOfRuneterraObject
{
    public string MatchId { get; set; }
}