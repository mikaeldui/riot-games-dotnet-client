using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RiotGames.LeagueOfLegends;

[TestClass]
public class LeagueOfLegendsTestBase
{
    /// <summary>
    ///     The summoner name used for the tests, in conjunction with <see cref="SERVER" />. Used to populate
    ///     <see cref="SUMMONER" /> and other static properties before the tests begin.
    /// </summary>
    public const string SUMMONER_NAME = "DevOps Activist";

    private static readonly string RIOT_GAMES_API_TOKEN =
        Environment.GetEnvironmentVariable("RIOT_GAMES_API_TOKEN") ?? string.Empty;

    /// <summary>
    ///     The server for <see cref="SUMMONER_NAME" />.
    /// </summary>
    public static readonly Server SERVER = Server.EUW; // Where DevOps Activist is.

    private static Summoner? _summoner;

    /// <summary>
    ///     Reconstructed for each and every test.
    /// </summary>
    protected LeagueOfLegendsClient Client = GetClient();

    protected LeagueOfLegendsTestBase()
    {
    } // Because ClassInitialize doesn't work in abstract classes...

    /// <summary>
    ///     The <see cref="Summoner" /> object for <see cref="SUMMONER_NAME" />.
    /// </summary>
    public static Summoner SUMMONER => _summoner ?? throw new Exception("_summoner not set.");

    /// <summary>
    ///     The encrypted summoner ID for <see cref="SUMMONER_NAME" />.
    /// </summary>
    public static string SUMMONER_ID_ENCRYPTED => SUMMONER.Id;

    /// <summary>
    ///     The encrypted account ID for <see cref="SUMMONER_NAME" />.
    /// </summary>
    public static string ACCOUNT_ID_ENCRTYPED => SUMMONER.EncryptedAccountId;

    /// <summary>
    ///     The encrypted PUUID for <see cref="SUMMONER_NAME" />.
    /// </summary>
    public static string PUUID_ENCRYPTED => SUMMONER.EncryptedPuuid;

    [AssemblyInitialize] // because ClassInitialize doesn't work in abstract classes...
    public static void Initialize(TestContext _)
    {
        if (RIOT_GAMES_API_TOKEN == string.Empty) return;
        using var client = GetClient();
        _summoner = client.GetSummonerByNameAsync(SUMMONER_NAME).Result;
    }

    /// <summary>
    ///     If overridden, call this first if <see cref="RIOT_GAMES_API_TOKEN" /> is required.
    /// </summary>
    [TestInitialize]
    public virtual void Setup()
    {
        if (RIOT_GAMES_API_TOKEN == string.Empty) // These tests can't be run without a valid token.
            Assert.Inconclusive("RIOT_GAMES_API_TOKEN not set.");

        Client = GetClient();
    }

    [TestCleanup]
    public virtual void TearDown()
    {
        Client.Dispose();
    }

    public static LeagueOfLegendsClient GetClient()
    {
        return new(RIOT_GAMES_API_TOKEN, SERVER);
    }
}