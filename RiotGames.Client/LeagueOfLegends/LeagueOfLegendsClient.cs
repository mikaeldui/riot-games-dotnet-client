using Camille.Enums;

namespace RiotGames.LeagueOfLegends
{
    public partial class LeagueOfLegendsClient : RiotGamesClientBase<LeagueOfLegendsObject>
    {
        /// <summary>
        /// Some endpoints need to know the platform and will throw exceptions if used.
        /// </summary>
        public LeagueOfLegendsClient(string apiKey, RegionalRoute region) : base(apiKey, region)
        {

        }

        public LeagueOfLegendsClient(string apiKey, PlatformRoute platform) : base(apiKey, platform)
        {

        }
    }
}