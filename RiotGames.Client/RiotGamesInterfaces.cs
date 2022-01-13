using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames
{
    // See https://developer.riotgames.com/apis#summoner-v4/GET_getBySummonerName
    public interface IEncryptedPuuid : IRiotGamesObject
    {
        public string EncryptedPuuid { get; set; }
    }

    // See https://developer.riotgames.com/apis#summoner-v4/GET_getBySummonerName
    public interface IEncryptedAccountId : IRiotGamesObject
    {
        public string EncryptedAccountId { get; set; }
    }

    // See https://developer.riotgames.com/apis#summoner-v4/GET_getBySummonerName
    public interface IEncryptedSummonerId : IRiotGamesObject
    {
        public string EncryptedSummonerId { get; set; }
    }
}
