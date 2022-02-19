using Camille.Enums;

namespace RiotGames.Valorant;

public partial class ValorantClient : RiotGamesClientBase<IValorantObject>
{
    public ValorantClient(string apiKey, ValPlatformRoute platform) : base(apiKey, platform)
    {
    }
}