using Humanizer.Inflections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.Client.CodeGeneration.RiotGamesApi
{
    /// <summary>
    /// Contains hacks to make everything work a little better. 
    /// These probably needs to be updated regularly.
    /// </summary>
    internal static class RiotApiHacks
    {
        static RiotApiHacks()
        {
            Vocabularies.Default.AddUncountable("data");
            Vocabularies.Default.AddUncountable("me");
        }

        public static void Activate() => Console.WriteLine("Hacks activated!");

        public static readonly IReadOnlyDictionary<string, string> EndpointWordCompilations = new Dictionary<string, string>
        {
            {"challengerleagues", "challenger-leagues" },
            {"grandmasterleagues", "grand-master-leagues" },
            {"masterleagues", "master-leagues" },
            {"grandmaster", "grand-master" },
            {"matchlist", "match-list" }
        };

        public static readonly IReadOnlyDictionary<string, string> EndpointsWithDuplicateSchemas = new Dictionary<string, string>
        {
            { "league-exp-v4.", "LeagueExp" },
            { "tournament-stub-v4.", "TournamentStub"},
            { "spectator-v4.", "Spectator" },
            { "clash-v1.Team", "Clash" },
            { "lor-ranked-v1.Player", "Ranked" },
            { "val-ranked-v1.Player", "Ranked" },
            { "val-status-v1.Content", "Status" }
        };

        public static readonly IReadOnlyDictionary<(string typeName, string identifier), string> BasicInterfaces =
            new Dictionary<(string typeName, string identifier), string>()
            {
                { ("string?", "EncryptedPuuid"), "IEncryptedPuuid" },
                { ("string?", "EncryptedAccountId"), "IEncryptedAccountId" },
                { ("string?", "EncryptedSummonerId"), "IEncryptedSummonerId" },
                { ("int?", "TournamentId"), "ITournamentId" }
            };

        // Because Riot has yet to update their specs.
        public static readonly IReadOnlyDictionary<string, string> OldPropertyNames = new Dictionary<string, string>
        {
            { "Puuid", "EncryptedPuuid" },
            { "AccountId", "EncryptedAccountId" },
            { "SummonerId", "EncryptedSummonerId" }
        };
    }
}
