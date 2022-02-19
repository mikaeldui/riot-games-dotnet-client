using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RiotGames.LeagueOfLegends.LeagueClient;

[TestClass]
public class LeagueClientTests
{
#if false
        [TestMethod]
        public async Task GetChampSelectSummoner()
        {
            using LeagueClient client = new();

            //var session = await client.LolChampSelect.GetSessionAsync();

            //var summoner1 = await client.LolSummoner.GetSummonerAsync((long) session.MyTeam[0].SummonerId);

            client.LeagueOfLegends.ChampSelect.TeamBoostChanged += (sender, args) =>
            {

            }
        }

#endif
}