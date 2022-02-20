using RiotGames.Messaging;

namespace RiotGames.LeagueOfLegends.LeagueClient;

public delegate void LeagueClientEventHandler<T>(object sender, LeagueClientEventArgs<T> args);

public class LeagueClientEventArgs<T> : RmsEventArgs<T>
{
    public LeagueClientEventArgs(RmsChangeType changeType, T data) : base(changeType, data)
    {
    }
}

internal static class LeagueClientEventExtensions
{
    internal static void Invoke<T>(this LeagueClientEventHandler<T> eventHandler, object sender,
        RmsChangeType changeType, T data) =>
        eventHandler.Invoke(sender, new LeagueClientEventArgs<T>(changeType, data));
}