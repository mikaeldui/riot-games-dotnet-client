using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.LeagueOfLegends
{
    /// <summary>
    /// For https://developer.riotgames.com/apis#summoner-v4
    /// </summary>
    [TestClass]
    public class SummonerTests : LeagueOfLegendsTestBase
    {
        [TestMethod]
        public async Task GetSummonerByNameAsync()
        {
            // Summoner is populated by the base class with GetSummonerByNameAsync.
            AssertProperties(SUMMONER);
        }

        [TestMethod]
        public async Task GetSummonerByAccountAsync()
        {
            var summoner = await Client.GetSummonerByAccountAsync(ACCOUNT_ID_ENCRTYPED);
            AssertProperties(summoner);
        }

        [TestMethod]
        public async Task GetSummonerByPuuidAsync()
        {
            var summoner = await Client.GetSummonerByPuuidAsync(PUUID_ENCRYPTED);
            AssertProperties(summoner);
        }

        [TestMethod]
        public async Task GetSummonerAsync()
        {
            var summoner = await Client.GetSummonerAsync(SUMMONER_ID_ENCRYPTED);
            AssertProperties(summoner);
        }

        public static void AssertProperties(Summoner summoner)
        {
            Assert.IsNotNull(summoner);
            Assert.IsNotNull(summoner.Id);
            Assert.IsNotNull(summoner.EncryptedAccountId);
            Assert.IsNotNull(summoner.EncryptedPuuid);
            Assert.AreNotEqual(default, summoner.ProfileIconId);
            Assert.AreNotEqual(default, summoner.SummonerLevel);
            Assert.AreNotEqual(default, summoner.RevisionDate);
        }
    }
}
