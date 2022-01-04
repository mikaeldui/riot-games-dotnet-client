using MingweiSamuel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.Client.CodeGeneration.RiotGamesApi
{
    using Path = KeyValuePair<string, RiotApiPathObject>;
    using Paths = IEnumerable<KeyValuePair<string, RiotApiPathObject>>;
    using Schema = KeyValuePair<string, RiotApiComponentSchemaObject>;
    using Schemas = Dictionary<string, RiotApiComponentSchemaObject>;

    internal static class LinqQueries
    {
        public static Paths WhereReferencesSchema(this Paths paths, Schema schema) =>
            paths.Where(p =>
            {
                var cSchema = p.Value?.Get?.Responses?["200"].Content.First().Value.Schema;
                if (cSchema == null) return false;
                string? @ref;
                if (cSchema.Type == "array")
                    @ref = cSchema.Items?.Ref;
                else
                    @ref = cSchema.Ref;
                return @ref.Remove("#/components/schemas/") == schema.Key;
            });

        public static IEnumerable<Schema> WhereReferencesSchema(this Schemas schemas, Schema schema) =>
            schemas.Where(s =>

                s.Value.Properties.Any(p =>
                {
                    string? @ref;
                    if (p.Value.Type == "array")
                        @ref = p.Value.Items?.Ref;
                    else
                        @ref = p.Value.Ref;

                    return @ref?.Remove("#/components/schemas/") == schema.Key;
                })
            );

        public static Paths WhereReferenceNotNull(this Paths paths) =>
            paths.Where(p =>
                p.Value.Get?.Responses?["200"]?.Content?.First().Value?.Schema?.Ref != null ||
                p.Value.Get?.Responses?["200"]?.Content?.First().Value?.Schema?.Items?.Ref != null);

        public static IEnumerable<IGrouping<string, Path>> GroupByGame(this Paths paths) =>
            paths.GroupBy(p => p.Key.SplitAndRemoveEmptyEntries('/').First());

    }
}
