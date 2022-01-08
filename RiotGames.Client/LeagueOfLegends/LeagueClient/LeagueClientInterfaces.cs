using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.LeagueOfLegends.LeagueClient
{
    public interface ISummonerId
    {
        public long? SummonerId { get; set; }
    }

    public interface IPuuid
    {
        public string? Puuid { get; set; }
    }

    public interface IAccountId
    {
        public long? AccountId { get; set; }
    }

    public interface IPlayerId
    {
        public long? PlayerId { get; set; }
    }

    public interface IChampionId
    {
        public int? ChampionId { get; set; }
    }

    public interface IGameId
    {
        public long? GameId { get; set; }
    }

    public interface IMapId
    {
        public int? MapId { get; set; }
    }
}
