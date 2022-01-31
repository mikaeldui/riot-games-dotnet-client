﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace RiotGames.LeagueOfLegends
{
    [TestClass]
    public class LeagueOfLegendsTestBase
    {
        private static readonly string RIOT_GAMES_API_TOKEN = Environment.GetEnvironmentVariable("RIOT_GAMES_API_TOKEN") ?? string.Empty;

        /// <summary>
        /// The summoner name used for the tests, in conjunction with <see cref="SERVER"/>. Used to populate <see cref="SUMMONER"/> and other static properties before the tests begin.
        /// </summary>
        public const string SUMMONER_NAME = "DevOps Activist";

        /// <summary>
        /// The server for <see cref="SUMMONER_NAME"/>.
        /// </summary>
        public static readonly Server SERVER = Server.EUW; // Where DevOps Activist is.
        private static Summoner? _summoner;

        /// <summary>
        /// The <see cref="Summoner"/> object for <see cref="SUMMONER_NAME"/>.
        /// </summary>
        public static Summoner SUMMONER => _summoner ?? throw new Exception("_summoner not set.");

        /// <summary>
        /// The encrypted summoner ID for <see cref="SUMMONER_NAME"/>.
        /// </summary>
        public static string SUMMONER_ID_ENCRYPTED => SUMMONER.Id;

        /// <summary>
        /// The encrypted account ID for <see cref="SUMMONER_NAME"/>.
        /// </summary>
        public static string ACCOUNT_ID_ENCRTYPED => SUMMONER.EncryptedAccountId;

        /// <summary>
        /// The encrypted PUUID for <see cref="SUMMONER_NAME"/>.
        /// </summary>
        public static string PUUID_ENCRYPTED => SUMMONER.EncryptedPuuid;

        protected LeagueOfLegendsTestBase() { }

        [AssemblyInitialize] // because ClassInitialize doesn't work in derived classes...
        public static void Initialize(TestContext _)
        {
            if (RIOT_GAMES_API_TOKEN != String.Empty) // These tests can't be run without a valid token.
            {
                using LeagueOfLegendsClient client = new(RIOT_GAMES_API_TOKEN, SERVER);
                _summoner = client.GetSummonerByNameAsync(SUMMONER_NAME).Result;
            }
        }

        /// <summary>
        /// Reconstructed for each and every test.
        /// </summary>
        protected LeagueOfLegendsClient Client = new(RIOT_GAMES_API_TOKEN, SERVER);

        /// <summary>
        /// If overridden, call this first if <see cref="RIOT_GAMES_API_TOKEN"/> is required.
        /// </summary>
        [TestInitialize]
        public virtual void Setup()
        {
            if (RIOT_GAMES_API_TOKEN == String.Empty) // These tests can't be run without a valid token.
                Assert.Inconclusive("RIOT_GAMES_API_TOKEN not set.");

            Client = new(RIOT_GAMES_API_TOKEN, SERVER);
        }

        [TestCleanup]
        public virtual void TearDown() => Client.Dispose();
    }
}
