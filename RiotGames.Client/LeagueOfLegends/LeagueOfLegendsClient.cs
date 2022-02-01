using Camille.Enums;
using System.Diagnostics;

namespace RiotGames.LeagueOfLegends
{
    [DebuggerDisplay("Region = {_region} Platform = {_platform}")]
    public partial class LeagueOfLegendsClient : RiotGamesClientBase<ILeagueOfLegendsObject>
    {
        /// <summary>
        /// Some endpoints need to know the platform and will throw exceptions if used.
        /// </summary>
        public LeagueOfLegendsClient(string apiKey, RegionalRoute region) : base(apiKey, region)
        {
        }

        public LeagueOfLegendsClient(string apiKey, Server server) : base(apiKey, server)
        {
        }
    }
}