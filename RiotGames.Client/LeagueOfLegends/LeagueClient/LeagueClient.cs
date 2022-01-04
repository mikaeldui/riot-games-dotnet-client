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
    public class LeagueClient : IDisposable
    {
        internal const string LEAGUECLIENT_DEFAULT_PROCESS_NAME = "LeagueClient";
        private const string LEAGUECLIENT_USERNAME = "riot";
        private readonly LeagueClientHttpClient _client;

        public LeagueClient(string processName = LEAGUECLIENT_DEFAULT_PROCESS_NAME, string lockfilePath = LeagueClientLockfile.LEAGUECLIENT_DEFAULT_LOCKFILE_PATH)
        {
            LeagueClientLockfile lockfile = lockfilePath == LeagueClientLockfile.LEAGUECLIENT_DEFAULT_LOCKFILE_PATH
                ? LeagueClientLockfile.FromProcess(processName)
                : LeagueClientLockfile.FromPath(lockfilePath);

            _client = new LeagueClientHttpClient(LEAGUECLIENT_USERNAME, lockfile.Password, lockfile.Port);
        }

        public void Dispose() => _client.Dispose();
    }
}
