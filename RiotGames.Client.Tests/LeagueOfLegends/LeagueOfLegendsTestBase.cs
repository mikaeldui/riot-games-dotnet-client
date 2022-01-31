using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace RiotGames.LeagueOfLegends
{
    [TestClass]
    public class LeagueOfLegendsTestBase
    {
        public static readonly string RIOT_GAMES_API_TOKEN = Environment.GetEnvironmentVariable("RIOT_GAMES_API_TOKEN") ?? string.Empty;
        public static readonly Server SERVER = Server.EUW; // Where DevOps Activist is.
        public const string SUMMONER_NAME = "DevOps Activist";
        private static Summoner? _summoner;
        public static Summoner SUMMONER => _summoner ?? throw new Exception("_summoner not set.");
        public static string SUMMONER_ID_ENCRYPTED => SUMMONER.Id;
        public static string ACCOUNT_ID_ENCRTYPED => SUMMONER.EncryptedAccountId;
        public static string PUUID_ENCRYPTED => SUMMONER.EncryptedPuuid;

        protected LeagueOfLegendsTestBase() { }

        [AssemblyInitialize]
        public static void Initialize(TestContext _)
        {
            if (RIOT_GAMES_API_TOKEN == String.Empty) // These tests can't be run without a valid token.
                Assert.Inconclusive("RIOT_GAMES_API_TOKEN not set.");

            using LeagueOfLegendsClient client = new(RIOT_GAMES_API_TOKEN, SERVER);
            _summoner = client.GetSummonerByNameAsync(SUMMONER_NAME).Result;
        }

        protected LeagueOfLegendsClient Client = new(RIOT_GAMES_API_TOKEN, SERVER);

        [TestInitialize]
        public void Setup() => Client = new(RIOT_GAMES_API_TOKEN, SERVER);

        [TestCleanup]
        public void TearDown() => Client.Dispose();
    }
}
