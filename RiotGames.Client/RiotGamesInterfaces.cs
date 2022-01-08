using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames
{
    // See https://developer.riotgames.com/apis#summoner-v4/GET_getBySummonerName
    public interface IEncryptedPuuid
    {
        public string? EncryptedPuuid { get; set; }
    }

    // See https://developer.riotgames.com/apis#summoner-v4/GET_getBySummonerName
    public interface IEncryptedAccountId
    {
        public string? EncryptedAccountId { get; set; }
    }

    // See https://developer.riotgames.com/apis#summoner-v4/GET_getBySummonerName
    public interface IEncryptedSummonerId
    {
        public string? EncryptedSummonerId { get; set; }
    }
}
