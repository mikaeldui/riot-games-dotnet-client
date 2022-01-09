using MingweiSamuel;
using MingweiSamuel.Lcu;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.Client.CodeGeneration.LeagueClient
{
    internal static class LeagueClientModelsHelper
    {
        [DebuggerStepThrough]
        internal static string FixGamePrefixes(this string input) =>
            input.RemoveStart("Lol").ReplaceStart("Tft", "TeamfightTactics");

        public static string GetTypeName(this OpenApiComponentPropertyObject property)
        {
            if (property.Ref != null)
                return OpenApiComponentHelper.GetTypeNameFromRef(property.Ref)
                    //.FixGamePrefixes()
                    ;

            return OpenApiComponentHelper.GetTypeNameFromString(property.Format ?? property.Type)
                //.FixGamePrefixes()
                ;
        }

        public static string GetTypeName(this OpenApiSchemaObject schema)
        {
            return OpenApiComponentHelper.GetTypeName(schema)
                //.FixGamePrefixes()
                ;
        }
    }
}
