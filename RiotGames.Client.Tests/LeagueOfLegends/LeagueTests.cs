using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.LeagueOfLegends
{
    [TestClass]
    public class LeagueTests : LeagueOfLegendsTestBase
    {
        [TestMethod]
        public async Task GetEntriesBySummonerAsync()
        {
            var entries = await Client.GetLeagueEntriesAsync(SUMMONER_ID_ENCRYPTED);
            Assert.IsNotNull(entries);
            Assert.AreNotEqual(default, entries.Count);
            foreach (var entry in entries)
                AssertProperties(entry);
        }

        public static void AssertProperties(LeagueEntry entry)
        {
            Assert.IsNotNull(entry);
            Assert.AreNotEqual(default, entry.Wins);
            Assert.AreNotEqual(default, entry.LeaguePoints);
            Assert.AreNotEqual(default, entry.Losses);
            Assert.IsFalse(string.IsNullOrWhiteSpace(entry.EncryptedSummonerId));
            Assert.IsFalse(string.IsNullOrWhiteSpace(entry.LeagueId));
            Assert.IsFalse(string.IsNullOrWhiteSpace(entry.QueueType));
            Assert.IsFalse(string.IsNullOrWhiteSpace(entry.Rank));
            Assert.IsFalse(string.IsNullOrWhiteSpace(entry.SummonerName));
            Assert.IsFalse(string.IsNullOrWhiteSpace(entry.Tier));
            if (entry.MiniSeries != null)
                AssertProperties(entry.MiniSeries);
        }

        private static void AssertProperties(MiniSeries miniSeries)
        {
            Assert.IsNotNull(miniSeries);
            Assert.IsFalse(string.IsNullOrWhiteSpace(miniSeries.Progress));
        }
    }
}
