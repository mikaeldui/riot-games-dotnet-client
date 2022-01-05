using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.Client.CodeGeneration.LeagueClient
{
    internal class LeagueClientHacks
    {
        public static readonly IReadOnlyDictionary<string, string> ReservedIdentifiers = new Dictionary<string, string>
        {
            {"namespace", "@namespace" },
            {"product-id", "productId" }
        };

        public static readonly IReadOnlyDictionary<string, string> ReservedPathParameters;

        static LeagueClientHacks()
        {
            ReservedPathParameters = ReservedIdentifiers.ToDictionary(kvp => "{" + kvp.Key + "}", kvp => "{" + kvp.Value + "}");
        }
    }
}
