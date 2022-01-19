using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.LeagueOfLegends.DataDragon.Tests
{
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
}
