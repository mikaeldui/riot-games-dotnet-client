namespace RiotGames;

/// <summary>
///     Usually thrown if the required route hasn't been specified.
/// </summary>
[Serializable]
public class RiotGamesRouteException : Exception
{
    public RiotGamesRouteException(string routeType) : base(
        $"The {routeType} has not been specified so you can't call {routeType} specific endpoints. Try reconstructing the client with the {routeType} set.")
    {
    }
}
