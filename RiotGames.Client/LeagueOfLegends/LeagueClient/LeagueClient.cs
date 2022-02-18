using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.LeagueOfLegends.LeagueClient
{
    /// <summary>
    /// You can use it to communicate with the League Client. Didn't want to name it LeagueClientClient.
    /// </summary>
    public partial class LeagueClient : LeagueClientBase
    {
        internal LeagueClient(LeagueClientHttpClient httpClient) : base(httpClient)
        {
        }

        public LeagueClient(string processName = LeagueClientLockFile.LEAGUECLIENT_DEFAULT_PROCESS_NAME, string lockfilePath = LeagueClientLockFile.LEAGUECLIENT_DEFAULT_LOCKFILE_PATH) : base(processName, lockfilePath)
        {
        }

        private LeagueOfLegendsClient? _leagueOfLegendsClient;
        public LeagueOfLegendsClient LeagueOfLegends => _leagueOfLegendsClient ??= new LeagueOfLegendsClient(this);

        private TeamfightTacticsClient? _teamfightTactics;
        public TeamfightTacticsClient TeamfightTactics => _teamfightTactics ??= new TeamfightTacticsClient(this);

        public partial class LeagueOfLegendsClient : LeagueClientBase
        {
            /// <summary>
            /// Use if you don't need <see cref="LeagueClient"/> endpoints and <see cref="TeamfightTacticsClient"/> endpoints.
            /// </summary>
            public LeagueOfLegendsClient(string processName = LeagueClientLockFile.LEAGUECLIENT_DEFAULT_PROCESS_NAME, string lockfilePath = LeagueClientLockFile.LEAGUECLIENT_DEFAULT_LOCKFILE_PATH) : base(processName, lockfilePath)
            {

            }
        }

        public partial class TeamfightTacticsClient : LeagueClientBase
        {
            /// <summary>
            /// Use if you don't need <see cref="LeagueClient"/> endpoints and <see cref="LeagueOfLegendsClient"/> endpoints.
            /// </summary>
            public TeamfightTacticsClient(string processName = LeagueClientLockFile.LEAGUECLIENT_DEFAULT_PROCESS_NAME, string lockfilePath = LeagueClientLockFile.LEAGUECLIENT_DEFAULT_LOCKFILE_PATH) : base(processName, lockfilePath)
            {

            }
        }
    }
}
