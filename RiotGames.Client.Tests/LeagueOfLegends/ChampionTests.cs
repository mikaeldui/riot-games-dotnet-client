using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.LeagueOfLegends
{
    [TestClass]
    public class ChampionTests : LeagueOfLegendsTestBase
    {
        [TestMethod]
        public async Task GetChampionRotations()
        {
            var rotation = await Client.GetPlatformChampionRotationsAsync();
            Assert.IsNotNull(rotation);
            Assert.AreNotEqual(default, rotation.MaxNewPlayerLevel);

            Assert.AreNotEqual(default, rotation.FreeChampionIds.Count);
            foreach (var championId in rotation.FreeChampionIds)
                Assert.AreNotEqual(default, championId);

            Assert.AreNotEqual(default, rotation.FreeChampionIdsForNewPlayers.Count);
            foreach (var championId in rotation.FreeChampionIdsForNewPlayers)
                Assert.AreNotEqual(default, championId);
        }
    }
}
