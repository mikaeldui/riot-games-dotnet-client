using Camille.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.LeagueOfLegends
{
    [TestClass]
    public class ServerTests
    {
        [TestMethod]
        public void TryParse()
        {
            if (!Server.TryParse("euw", out Server server))
                Assert.Fail("Couldn't parse euw.");

            Assert.IsNotNull(server);
            Assert.AreEqual(Server.EUW, server);

            if (!Server.TryParse("euw1", out server))
                Assert.Fail("Couldn't parse euw1.");

            Assert.IsNotNull(server);
            Assert.AreEqual(Server.EUW, server);
        }

        [TestMethod]
        public void Parse()
        {
            var server = Server.Parse("euw");

            Assert.IsNotNull(server);
            Assert.AreEqual(Server.EUW, server);

            server = Server.Parse("euw1");

            Assert.IsNotNull(server);
            Assert.AreEqual(Server.EUW, server);
        }

        [TestMethod]
        public void AllCount()
        {
            Assert.AreEqual(Enum.GetValues<PlatformRoute>().Length, Server.All.Count);
        }
    }
}
