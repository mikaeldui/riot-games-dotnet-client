using RiotGames.Messaging;

namespace RiotGames.LeagueOfLegends.LeagueClient;

public delegate void LeagueClientEventHandler<T>(object sender, LeagueClientEventArgs<T> args);

public class LeagueClientEventArgs<T> : RmsEventArgs<T>
{
    public LeagueClientEventArgs(RmsEventType eventType, T data) : base(eventType, data)
    {
    }
}

internal static class LeagueClientEventExtensions
{
    internal static void Invoke<T>(this LeagueClientEventHandler<T> eventHandler, object sender,
        RmsEventType eventType, T data) =>
        eventHandler.Invoke(sender, new LeagueClientEventArgs<T>(eventType, data));
}