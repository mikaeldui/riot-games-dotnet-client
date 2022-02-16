using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.LeagueOfLegends.LeagueClient
{
    public abstract partial class LeagueClientBase : IDisposable
    {
        //internal const string LEAGUE_CLIENT_DEFAULT_PROCESS_NAME = "LeagueClient";
        private const string LEAGUE_CLIENT_USERNAME = "riot";
        internal readonly LeagueClientHttpClient HttpClient;

        internal LeagueClientBase(LeagueClientHttpClient httpClient) => HttpClient = httpClient;

        protected LeagueClientBase(string processName = LeagueClientLockFile.LEAGUECLIENT_DEFAULT_PROCESS_NAME, string lockfilePath = LeagueClientLockFile.LEAGUECLIENT_DEFAULT_LOCKFILE_PATH)
        {
            LeagueClientLockFile lockfile = lockfilePath == LeagueClientLockFile.LEAGUECLIENT_DEFAULT_LOCKFILE_PATH
                ? LeagueClientLockFile.FromProcess(processName)
                : LeagueClientLockFile.FromPath(lockfilePath);

            HttpClient = new LeagueClientHttpClient(LEAGUE_CLIENT_USERNAME, lockfile.Password, lockfile.Port);
#if !NETSTANDARD2_0
            WampClient = new LeagueClientWampClient(LEAGUE_CLIENT_USERNAME, lockfile.Password, lockfile.Port);
#endif
        }

        public virtual void Dispose()
        {
            HttpClient.Dispose();
#if !NETSTANDARD2_0
            WampClient?.Dispose();
#endif
        }
    }
}
