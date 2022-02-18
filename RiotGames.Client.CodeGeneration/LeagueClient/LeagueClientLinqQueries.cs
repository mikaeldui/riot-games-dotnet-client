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
            paths.GroupBy(p => p.Key.Count(c => c == '/') == 1 ? null : p.Key.SplitAndRemoveEmptyEntries('/')[0]);

        /// <summary>
        /// Groups events by module.
        /// </summary>
        public static IEnumerable<IGrouping<string, KeyValuePair<string, string>>> GroupByModule(this IEnumerable<KeyValuePair<string, string>> events)
        {
            var jsonApiEvents = events.Where(e => e.Key.StartsWith("OnJsonApiEvent_"));
            return jsonApiEvents.GroupBy(e => e.Key.RemoveStart("OnJsonApiEvent_").SplitAndRemoveEmptyEntries('_')[0]);
        }
    }

    internal static class LeagueClientRandomExtensions
    {
        public static string EventToPath(this string @event) => @event.Replace("OnJsonApiEvent", "").Replace("_", "/");

        public static bool EventEqualsPath(this string @event, string path) => @event.EventToPath() == path.Trim('\"');
    }
}
