using System.Net.Http;
using System.Threading.Tasks;
using Camille.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RiotGames;

[TestClass]
public class RiotGamesClientTests
{
    [TestMethod]
    public void ConstructTheClient()
    {
        _ = new RiotGamesClient("", PlatformRoute.EUW1);
    }

    [TestMethod]
    [ExpectedException(typeof(HttpRequestException))]
    public async Task TryWithBadApiKey()
    {
        using RiotGamesClient europeanClient = new("bad-key", RegionalRoute.EUROPE);
        _ = await europeanClient.GetAccountAsync("bad-puuid");
    }
}