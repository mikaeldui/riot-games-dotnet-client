namespace RiotGames.LeagueOfLegends.LeagueClient;

public delegate void LeagueClientEventHandler<in T>(object sender, T args);