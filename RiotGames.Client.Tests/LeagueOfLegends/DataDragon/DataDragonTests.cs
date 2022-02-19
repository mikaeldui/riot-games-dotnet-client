using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RiotGames.LeagueOfLegends.DataDragon;

[TestClass]
public class DataDragonTests
{
    [TestMethod]
    public async Task GetRealm()
    {
        DataDragonClient client = new();
        var euw = await client.GetRealmAsync("EUW");
        Assert.IsNotNull(euw);
        Assert.IsNotNull(euw.N);
    }

    [TestMethod]
    public async Task GetVersions()
    {
        DataDragonClient client = new();
        var versions = await client.Api.GetVersionsAsync();
        Assert.IsNotNull(versions);
        Assert.IsTrue(versions.Count > 0);
    }

    [TestMethod]
    public async Task GetLanguages()
    {
        DataDragonClient client = new();
        var languages = await client.Cdn.GetLanguagesAsync();
        Assert.IsNotNull(languages);
        Assert.IsTrue(languages.Count > 0);
    }
}