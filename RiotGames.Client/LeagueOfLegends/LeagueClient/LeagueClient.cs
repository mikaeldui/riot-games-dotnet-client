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
    public partial class LeagueClient : IDisposable
    {
        //internal const string LEAGUECLIENT_DEFAULT_PROCESS_NAME = "LeagueClient";
        private const string LEAGUECLIENT_USERNAME = "riot";
        internal readonly LeagueClientHttpClient HttpClient;

        internal LeagueClient(LeagueClientHttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public LeagueClient(string processName = LeagueClientLockfile.LEAGUECLIENT_DEFAULT_PROCESS_NAME, string lockfilePath = LeagueClientLockfile.LEAGUECLIENT_DEFAULT_LOCKFILE_PATH)
        {
            LeagueClientLockfile lockfile = lockfilePath == LeagueClientLockfile.LEAGUECLIENT_DEFAULT_LOCKFILE_PATH
                ? LeagueClientLockfile.FromProcess(processName)
                : LeagueClientLockfile.FromPath(lockfilePath);

            HttpClient = new LeagueClientHttpClient(LEAGUECLIENT_USERNAME, lockfile.Password, lockfile.Port);
        }

        public void Dispose() => HttpClient.Dispose();

        public partial class LeagueOfLegendsClient : LeagueClient
        {
            /// <summary>
            /// Use if you don't need <see cref="LeagueClient"/> endpoints and <see cref="TeamfightTacticsClient"/> endpoints.
            /// </summary>
            public LeagueOfLegendsClient(string processName = LeagueClientLockfile.LEAGUECLIENT_DEFAULT_PROCESS_NAME, string lockfilePath = LeagueClientLockfile.LEAGUECLIENT_DEFAULT_LOCKFILE_PATH) : base(processName, lockfilePath)
            {

            }
        }

        public partial class TeamfightTacticsClient : LeagueClient
        {
            /// <summary>
            /// Use if you don't need <see cref="LeagueClient"/> endpoints and <see cref="LeagueOfLegendsClient"/> endpoints.
            /// </summary>
            public TeamfightTacticsClient(string processName = LeagueClientLockfile.LEAGUECLIENT_DEFAULT_PROCESS_NAME, string lockfilePath = LeagueClientLockfile.LEAGUECLIENT_DEFAULT_LOCKFILE_PATH) : base(processName, lockfilePath)
            {

            }
        }
    }
}
