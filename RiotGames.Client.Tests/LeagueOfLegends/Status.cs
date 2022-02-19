using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RiotGames.LeagueOfLegends;

[TestClass]
public class StatusTests : LeagueOfLegendsTestBase
{
    [TestMethod]
    public async Task GetPlatformDataAsync()
    {
        var platformData = await Client.GetStatusPlatformDataAsync();
        Assert.IsNotNull(platformData);
        Assert.IsFalse(string.IsNullOrWhiteSpace(platformData.Id));
        Assert.IsFalse(string.IsNullOrWhiteSpace(platformData.Name));
    }
}