using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiotGames.LeagueOfLegends.LeagueClient;

namespace RiotGames.Client.Tests.LeagueOfLegendsTests.LeagueClientTests
{
    [TestClass]
    public class LeagueClientTests
    {
#if false
        [TestMethod]
        public async Task GetChampSelectSummoner()
        {
            using LeagueClient client = new();

            var session = await client.LolChampSelect.GetSessionAsync();

            var summoner1 = await client.LolSummoner.GetSummonerAsync((long) session.MyTeam[0].SummonerId);
        }
#endif
    }
}
