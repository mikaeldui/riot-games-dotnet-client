namespace RiotGames.LeagueOfLegends.LeagueClient;

public interface ILeagueClientObject : ILeagueOfLegendsObject
{
}

public interface IPlayerId : ILeagueClientObject
{
    public long PlayerId { get; set; }
}

public interface IChampionId : ILeagueClientObject
{
    public int ChampionId { get; set; }
}

public interface IGameId : ILeagueClientObject
{
    public long GameId { get; set; }
}

public interface IMapId : ILeagueClientObject
{
    public int MapId { get; set; }
}