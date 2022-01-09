using Camille.Enums;

namespace RiotGames.TeamfightTactics
{
    public partial class TeamfightTacticsClient : RiotGamesClientBase<TeamfightTacticsObject>
    {
        /// <summary>
        /// Some endpoints need to know the platform and will throw exceptions if used.
        /// </summary>
        public TeamfightTacticsClient(string apiKey, RegionalRoute region) : base(apiKey, region)
        {
        }

        public TeamfightTacticsClient(string apiKey, PlatformRoute platform) : base(apiKey, platform)
        {
        }
    }
}