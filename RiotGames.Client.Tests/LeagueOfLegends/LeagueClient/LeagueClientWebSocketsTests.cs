using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiotGames.Messaging;

namespace RiotGames.LeagueOfLegends.LeagueClient;

[TestClass]
public class LeagueClientWebSocketsTests
{
    private static readonly bool IS_LEAGUE_CLIENT_RUNNING = Process.GetProcesses()
        .Any(p => p.ProcessName == LeagueClientLockFile.LEAGUE_CLIENT_DEFAULT_PROCESS_NAME);

    [TestMethod]
    public async Task TestSomething()
    {
        if (!IS_LEAGUE_CLIENT_RUNNING) Assert.Inconclusive("No running League Client found.");

        var lockFile = LeagueClientLockFile.FromProcess();
        using RmsClient client = new("riot", lockFile.Password, lockFile.Port);

        await client.ConnectAsync();
        await client.SubscribeAsync("OnJsonApiEvent");

        for (var i = 0; i < 10; i++)
        {
            var result = await client.ReceiveAsync();
            Assert.IsNotNull(result);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(result.Topic));
            Assert.IsTrue(!string.IsNullOrWhiteSpace(result.Uri.ToString()));
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(RmsTypeCode.Event, result.MessageCode);
        }

        await client.CloseAsync();
    }
}