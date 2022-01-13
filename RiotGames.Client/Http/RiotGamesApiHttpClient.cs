using Camille.Enums;

namespace RiotGames
{
    internal class RiotGamesApiHttpClient<TObjectBase> : RiotGamesHttpClient<TObjectBase>
    {
        private RiotGamesApiHttpClient(string apiKey, string apiSubDomain)
        {
            HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Riot-Token", apiKey);
            HttpClient.BaseAddress = new Uri($"https://{apiSubDomain}.api.riotgames.com");
        }

        internal RiotGamesApiHttpClient(string apiKey, RegionalRoute region) : this(apiKey, region.ToString().ToLower())
        {
        }

        internal RiotGamesApiHttpClient(string apiKey, PlatformRoute platform) : this(apiKey, platform.ToString().ToLower())
        {
        }

        internal RiotGamesApiHttpClient(string apiKey, ValPlatformRoute platform) : this(apiKey, platform.ToString().ToLower())
        {
        }
    }
}