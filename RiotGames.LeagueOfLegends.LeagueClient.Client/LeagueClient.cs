using RiotGames.Messaging;

namespace RiotGames.LeagueOfLegends.LeagueClient;

/// <summary>
///     You can use it to communicate with the League Client. Didn't want to name it LeagueClientClient.
/// </summary>
public partial class LeagueClient : LeagueClientBase
{
    private LeagueOfLegendsClient? _leagueOfLegendsClient;

    private TeamfightTacticsClient? _teamfightTactics;

    internal LeagueClient(LeagueClientHttpClient httpClient, RmsEventRouter eventRouter) : base(httpClient, eventRouter)
    {
    }

    public LeagueClient(string processName = LeagueClientLockFile.LEAGUECLIENT_DEFAULT_PROCESS_NAME,
        string lockfilePath = LeagueClientLockFile.LEAGUECLIENT_DEFAULT_LOCKFILE_PATH) : base(processName, lockfilePath)
    {
    }

    public LeagueOfLegendsClient LeagueOfLegends => _leagueOfLegendsClient ??= new LeagueOfLegendsClient(this);
    public TeamfightTacticsClient TeamfightTactics => _teamfightTactics ??= new TeamfightTacticsClient(this);

    public partial class LeagueOfLegendsClient : LeagueClientBase
    {
        /// <summary>
        ///     Use if you don't need <see cref="LeagueClient" /> endpoints and <see cref="TeamfightTacticsClient" /> endpoints.
        /// </summary>
        public LeagueOfLegendsClient(string processName = LeagueClientLockFile.LEAGUECLIENT_DEFAULT_PROCESS_NAME,
            string lockfilePath = LeagueClientLockFile.LEAGUECLIENT_DEFAULT_LOCKFILE_PATH) : base(processName,
            lockfilePath)
        {
        }
    }

    public partial class TeamfightTacticsClient : LeagueClientBase
    {
        /// <summary>
        ///     Use if you don't need <see cref="LeagueClient" /> endpoints and <see cref="LeagueOfLegendsClient" /> endpoints.
        /// </summary>
        public TeamfightTacticsClient(string processName = LeagueClientLockFile.LEAGUECLIENT_DEFAULT_PROCESS_NAME,
            string lockfilePath = LeagueClientLockFile.LEAGUECLIENT_DEFAULT_LOCKFILE_PATH) : base(processName,
            lockfilePath)
        {
        }
    }
}