using Camille.Enums;
using System.ComponentModel;

namespace RiotGames
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class RiotGamesApiHttpClient
    {
        /// <summary>
        /// Default it "https://{0}.api.riotgames.com" where {0} is replaced by the region, e.g. "euw1".
        /// </summary>
        public static string BaseAddressFormat { get; set; } = "https://{0}.api.riotgames.com";

        private RiotGamesApiHttpClient()
        {
        }
    }

    internal class RiotGamesApiHttpClient<TObjectBase> : RiotGamesHttpClient<TObjectBase>
    {
        private RiotGamesApiHttpClient(string apiKey, string route)
        {
            HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Riot-Token", apiKey);
            BaseAddress = new Uri(string.Format(RiotGamesApiHttpClient.BaseAddressFormat, route));
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