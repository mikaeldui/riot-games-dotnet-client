using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.LeagueOfLegends
{
    [TestClass]
    public class MatchTests : LeagueOfLegendsTestBase
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private static LeagueOfLegendsReadOnlyCollection<string> MATCH_IDS;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private static string MATCH_ID => MATCH_IDS.Last();

        [ClassInitialize]
        public static void InitializeMatchTests(TestContext _)
        {
            using var client = GetClient();
            MATCH_IDS = client.GetMatchIdsAsync(PUUID_ENCRYPTED).Result; 
        }

        [TestMethod]
        public void GetMatchIdsAsync()
        {
            // MatchIds received by ClassInitialize.
            Assert.AreNotEqual(default, MATCH_IDS.Count);
            foreach (var matchId in MATCH_IDS)
                Assert.IsFalse(string.IsNullOrWhiteSpace(matchId));
        }

        [TestMethod]
        public async Task GetMatchAsync()
        {
            var match = await Client.GetMatchAsync(MATCH_ID);
            Assert.IsNotNull(match);
        }

        [TestMethod]
        public async Task GetMatchTimeline()
        {
            var timeline = await Client.GetMatchTimelineAsync(MATCH_ID);
            Assert.IsNotNull(timeline);
        }
    }
}
