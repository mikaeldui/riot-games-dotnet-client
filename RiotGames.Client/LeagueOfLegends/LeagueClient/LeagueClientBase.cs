using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.LeagueOfLegends.LeagueClient
{
    public abstract class LeagueClientBase : IDisposable
    {
        //internal const string LEAGUECLIENT_DEFAULT_PROCESS_NAME = "LeagueClient";
        private const string LEAGUECLIENT_USERNAME = "riot";
        internal readonly LeagueClientHttpClient HttpClient;

        internal LeagueClientBase(LeagueClientHttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        protected LeagueClientBase(string processName = LeagueClientLockfile.LEAGUECLIENT_DEFAULT_PROCESS_NAME, string lockfilePath = LeagueClientLockfile.LEAGUECLIENT_DEFAULT_LOCKFILE_PATH)
        {
            LeagueClientLockfile lockfile = lockfilePath == LeagueClientLockfile.LEAGUECLIENT_DEFAULT_LOCKFILE_PATH
                ? LeagueClientLockfile.FromProcess(processName)
                : LeagueClientLockfile.FromPath(lockfilePath);

            HttpClient = new LeagueClientHttpClient(LEAGUECLIENT_USERNAME, lockfile.Password, lockfile.Port);
        }

        public virtual void Dispose() => HttpClient.Dispose();
    }
}
