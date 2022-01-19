using Camille.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Threading.Tasks;

namespace RiotGames.Client.Tests
{
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
}