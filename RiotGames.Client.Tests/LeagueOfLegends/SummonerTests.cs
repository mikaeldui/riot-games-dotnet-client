using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.LeagueOfLegends
{
    public class SummonerTests
    {
        public static void AssertProperties(Summoner summoner)
        {
            Assert.IsNotNull(summoner);
            Assert.IsNotNull(summoner.Id);
            Assert.IsNotNull(summoner.EncryptedAccountId);
            Assert.IsNotNull(summoner.EncryptedPuuid);
            Assert.AreNotEqual(default, summoner.ProfileIconId);
            Assert.AreNotEqual(default, summoner.SummonerLevel);
            Assert.AreNotEqual(default, summoner.RevisionDate);
        }
    }
}
