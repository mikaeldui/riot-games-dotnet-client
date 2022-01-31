using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.LeagueOfLegends
{
    [TestClass]
    public class SpectatorTests : LeagueOfLegendsTestBase
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private static SpectatorFeaturedGames FEATURED_GAMES;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private static string ACTIVE_GAME_SUMMONER_NAME => FEATURED_GAMES.GameList.First().Participants.First().SummonerName;

        [ClassInitialize]
        public static void InitializeSpectator(TestContext _)
        {
            using var client = GetClient();
            FEATURED_GAMES = client.GetSpectatorFeaturedGamesAsync().Result;
        }

        [TestMethod]
        public void GetFeaturedGamesAsync()
        {
            // FEATURED_GAMES set by ClassInitialize.
            Assert.IsNotNull(FEATURED_GAMES);
        }

        [TestMethod]
        public async Task GetActiveGamesBySummonerAsync()
        {
            var summoner = await Client.GetSummonerByNameAsync(ACTIVE_GAME_SUMMONER_NAME);
            var activeGame = await Client.GetSpectatorActiveGameAsync(summoner.Id);
            Assert.IsNotNull(activeGame);
        }
    }
}
