using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.LeagueOfLegends.LeagueClient.Tests
{
    [TestClass]
    public class LeagueClientWebSocketsTests
    {
        private static readonly bool _isLeagueClientRunning = Process.GetProcesses().Any(p => p.ProcessName == LeagueClientLockFile.LEAGUECLIENT_DEFAULT_PROCESS_NAME);
        
        [TestMethod]
        public async Task TestSomething()
        {
            if (!_isLeagueClientRunning) Assert.Inconclusive("No running League Client found.");

            var lockFile = LeagueClientLockFile.FromProcess();
            using LeagueClientWampClient client = new("riot", lockFile.Password, lockFile.Port);

            await client.ConnectAsync();
            await client.SubscribeAsync("OnJsonApiEvent");

            for (int i = 0; i < 10; i++)
            {
                var result = await client.ReceiveAsync();
                Assert.IsNotNull(result);
            }

            await client.CloseAsync();
        }
    }
}
