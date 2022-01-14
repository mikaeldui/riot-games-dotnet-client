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
        [TestMethod]
        public async Task TestSomething()
        {
            if (Debugger.IsAttached)
            {
                var lockfile = LeagueClientLockfile.FromProcess();
                using LeagueClientWebSocketClient client = new(lockfile);

                await client.Connect();
                await client.Subscribe("OnJsonApiEvent");

                for (int i = 0; i < 10; i++)
                {
                    var result = await client.Receive();
                    Console.WriteLine(result);
                }

                await client.Disconnect();
            }
        }
    }
}
