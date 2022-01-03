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
            using (var europeanClient = new RiotGamesClient("bad-key", RegionalRoute.EUROPE))
            {
                var account = await europeanClient.GetAccountAsync("bad-puuid");
            }
        }
    }
}