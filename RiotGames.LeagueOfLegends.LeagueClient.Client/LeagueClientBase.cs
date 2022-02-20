using RiotGames.Messaging;

namespace RiotGames.LeagueOfLegends.LeagueClient;

public abstract class LeagueClientBase : IDisposable
{
    //internal const string LEAGUE_CLIENT_DEFAULT_PROCESS_NAME = "LeagueClient";
    private const string LEAGUE_CLIENT_USERNAME = "riot";
    internal readonly RmsEventRouter EventRouter;
    internal readonly LeagueClientHttpClient HttpClient;
    protected LeagueClientLockFile LockFile;

    internal LeagueClientBase(LeagueClientHttpClient httpClient, RmsEventRouter eventRouter)
    {
        HttpClient = httpClient;
        EventRouter = eventRouter;
    }

    protected LeagueClientBase(string processName = LeagueClientLockFile.LEAGUE_CLIENT_DEFAULT_PROCESS_NAME,
        string lockfilePath = LeagueClientLockFile.LEAGUE_CLIENT_DEFAULT_LOCKFILE_PATH)
    {
        LockFile = lockfilePath == LeagueClientLockFile.LEAGUE_CLIENT_DEFAULT_LOCKFILE_PATH
            ? LeagueClientLockFile.FromProcess(processName)
            : LeagueClientLockFile.FromPath(lockfilePath);

        HttpClient = new LeagueClientHttpClient(LEAGUE_CLIENT_USERNAME, LockFile.Password, LockFile.Port);
        EventRouter = new RmsEventRouter(LEAGUE_CLIENT_USERNAME, LockFile.Password, LockFile.Port);
    }

    public virtual void Dispose()
    {
        HttpClient.Dispose();
        EventRouter.Dispose();
    }
}