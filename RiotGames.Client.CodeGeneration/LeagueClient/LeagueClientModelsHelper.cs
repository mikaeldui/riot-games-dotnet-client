using MingweiSamuel.Lcu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.Client.CodeGeneration.LeagueClient
{
    internal static class LeagueClientModelsHelper
    {
        public static readonly IReadOnlyDictionary<(string typeName, string identifier), string> BasicInterfaces =
            new Dictionary<(string typeName, string identifier), string>()
            {
                { ("string?", "Puuid"), "IPuuid" },
                { ("long?", "SummonerId"), "ISummonerId" },
                { ("long?", "AccountId"), "IAccountId" },
                { ("long?", "PlayerId"), "IPlayerId" },
                { ("int?", "ChampionId"), "IChampionId" },
                { ("long?", "GameId"), "IGameId" },
                { ("int?", "MapId"), "IMapId" }
            };
    }
}
