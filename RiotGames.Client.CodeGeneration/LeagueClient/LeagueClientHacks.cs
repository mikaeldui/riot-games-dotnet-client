using Humanizer.Inflections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.Client.CodeGeneration.LeagueClient
{
    internal static class LeagueClientHacks
    {
        static LeagueClientHacks()
        {
            ReservedPathParameters = ReservedIdentifiers.ToDictionary(kvp => "{" + kvp.Key + "}", kvp => "{" + kvp.Value + "}");
            Vocabularies.Default.AddUncountable("status");
        }

        public static void Activate() => Console.WriteLine("Hacks activated!");

        public static readonly IReadOnlyDictionary<string, string> ReservedIdentifiers = new Dictionary<string, string>
        {
            {"namespace", "@namespace" },
            {"product-id", "productId" }
        };

        public static readonly IReadOnlyDictionary<string, string> ReservedPathParameters;
    }
}
