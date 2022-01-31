using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace RiotGames.LeagueOfLegends
{
    /// <summary>
    /// For https://developer.riotgames.com/apis#champion-mastery-v4.
    /// </summary>
    [TestClass]
    public class ChampionMasteryTests : LeagueOfLegendsTestBase
    {
        [TestMethod]
        public async Task GetChampionMasteries()
        {
            var masteries = await Client.GetChampionMasteriesAsync(SUMMONER_ID_ENCRYPTED);
            Assert.IsNotNull(masteries);
            Assert.AreNotEqual(0, masteries.Count);
            foreach(var mastery in masteries)
                AssertProperties(mastery);
        }

        [TestMethod]
        public async Task GetChampionMasteryByChampion()
        {
            var mastery = await Client.GetChampionMasteryAsync(1, SUMMONER_ID_ENCRYPTED);
            AssertProperties(mastery);
        }

        [TestMethod]
        public async Task GetChampionMasteryScore()
        {
            var score = await Client.GetChampionMasteryScoreAsync(SUMMONER_ID_ENCRYPTED);
            Assert.AreNotEqual(default, score);
        }

        public static void AssertProperties(ChampionMastery mastery)
        {
            Assert.IsNotNull(mastery);
            Assert.IsNotNull(mastery.EncryptedSummonerId);
            Assert.AreNotEqual(default, mastery.ChampionId);
            Assert.AreNotEqual(default, mastery.ChampionLevel);
            Assert.AreNotEqual(default, mastery.ChampionPoints);
            Assert.AreNotEqual(default, mastery.LastPlayTime);
        }
    }
}
