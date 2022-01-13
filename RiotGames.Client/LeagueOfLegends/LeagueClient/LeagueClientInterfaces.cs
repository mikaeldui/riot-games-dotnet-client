using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.LeagueOfLegends.LeagueClient
{
    public interface ILeagueClientObject : ILeagueOfLegendsObject
    {
    }

    public interface ISummonerId : ILeagueClientObject
    {
        public long SummonerId { get; set; }
    }

    public interface IPuuid : ILeagueClientObject
    {
        public string Puuid { get; set; }
    }

    public interface IAccountId : ILeagueClientObject
    {
        public long AccountId { get; set; }
    }

    public interface IPlayerId : ILeagueClientObject
    {
        public long PlayerId { get; set; }
    }

    public interface IChampionId : ILeagueClientObject
    {
        public int ChampionId { get; set; }
    }

    public interface IGameId : ILeagueClientObject
    {
        public long GameId { get; set; }
    }

    public interface IMapId : ILeagueClientObject
    {
        public int MapId { get; set; }
    }
}
