namespace RiotGames.LeagueOfLegends.LeagueClient;

public delegate void LeagueClientEventHandler<in T>(LeagueClient sender, T args);