using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.LeagueOfLegends
{
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8604 // Possible null reference argument.

    [TestClass]
    public class LeagueOfLegendsClientTests
    {
        public static readonly string? RIOT_GAMES_API_TOKEN = Environment.GetEnvironmentVariable("RIOT_GAMES_API_TOKEN");
        public const string SUMMONER_NAME = "DevOps Activist";
        public static readonly Server SERVER = Server.EUW; // Where DevOps Activist is.

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            if (RIOT_GAMES_API_TOKEN == null) // These tests can't be run without a valid token.
                Assert.Inconclusive("RIOT_GAMES_API_TOKEN not set.");
        }

        private LeagueOfLegendsClient? _client;

        [TestInitialize]
        public void Setup()
        {
            _client = new LeagueOfLegendsClient(RIOT_GAMES_API_TOKEN, SERVER);
        }

        [TestCleanup]
        public void TearDown()
        {
            _client.Dispose();
            _client = null;
        }

        [TestMethod]
        public void Construct()
        {
            // Done in Setup...
        }

        [TestMethod]
        public async Task Summoner()
        {
            var summoner = await _client.GetSummonerByNameAsync(SUMMONER_NAME);
            SummonerTests.AssertProperties(summoner);
            summoner = await _client.GetSummonerByAccountAsync(summoner.EncryptedAccountId);
            SummonerTests.AssertProperties(summoner);
            summoner = await _client.GetSummonerByPuuidAsync(summoner.EncryptedPuuid);
            SummonerTests.AssertProperties(summoner);
            summoner = await _client.GetSummonerAsync(summoner.Id);
            SummonerTests.AssertProperties(summoner);
        }
    }
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
}
