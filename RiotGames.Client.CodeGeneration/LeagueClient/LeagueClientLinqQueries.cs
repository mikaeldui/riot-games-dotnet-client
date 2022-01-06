using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MingweiSamuel;
using MingweiSamuel.Lcu;

namespace RiotGames.Client.CodeGeneration.LeagueClient
{
    using Path = KeyValuePair<string, OpenApiPathObject<OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, LcuParameterObject, LcuSchemaObject>>;
    using Paths = IEnumerable<KeyValuePair<string, OpenApiPathObject<OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, LcuParameterObject, LcuSchemaObject>>>;

    internal static class LeagueClientLinqQueries
    {
        public static IEnumerable<IGrouping<string?, Path>> GroupByModule(this Paths paths) => 
            paths.GroupBy(p =>
                {
                    if (p.Key.Count(c => c == '/') == 1)
                        return null;

                    return p.Key.SplitAndRemoveEmptyEntries('/')[0];
                });
    }
}
