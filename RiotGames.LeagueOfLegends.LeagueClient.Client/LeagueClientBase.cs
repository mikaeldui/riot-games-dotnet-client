using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiotGames.Messaging;

namespace RiotGames.LeagueOfLegends.LeagueClient
{
    public abstract class LeagueClientBase : IDisposable
    {
        //internal const string LEAGUE_CLIENT_DEFAULT_PROCESS_NAME = "LeagueClient";
        private const string LEAGUE_CLIENT_USERNAME = "riot";
        internal readonly LeagueClientHttpClient HttpClient;
        internal readonly RmsEventRouter EventRouter;

        internal LeagueClientBase(LeagueClientHttpClient httpClient, RmsEventRouter eventRouter)
        {
            HttpClient = httpClient;
            EventRouter = eventRouter;
        }

        protected LeagueClientBase(string processName = LeagueClientLockFile.LEAGUECLIENT_DEFAULT_PROCESS_NAME, string lockfilePath = LeagueClientLockFile.LEAGUECLIENT_DEFAULT_LOCKFILE_PATH)
        {
            var lockfile = lockfilePath == LeagueClientLockFile.LEAGUECLIENT_DEFAULT_LOCKFILE_PATH
                ? LeagueClientLockFile.FromProcess(processName)
                : LeagueClientLockFile.FromPath(lockfilePath);

            HttpClient = new LeagueClientHttpClient(LEAGUE_CLIENT_USERNAME, lockfile.Password, lockfile.Port);
            EventRouter = new RmsEventRouter(LEAGUE_CLIENT_USERNAME, lockfile.Password, lockfile.Port);
        }

        public virtual void Dispose()
        {
            HttpClient.Dispose();
            EventRouter.Dispose();
        }
    }
}
