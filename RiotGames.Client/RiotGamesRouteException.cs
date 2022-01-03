using System.Runtime.Serialization;

namespace RiotGames
{
    [Serializable]
    internal class RiotGamesRouteException : Exception
    {
        public RiotGamesRouteException(string routeType) : base($"The {routeType} has not been specified so you can't call {routeType} specific endpoints. Try reconstructing the client with the {routeType} set.")
        {
        }
    }
}