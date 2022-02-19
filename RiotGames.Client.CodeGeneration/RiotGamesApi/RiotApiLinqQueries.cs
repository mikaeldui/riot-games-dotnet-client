using MingweiSamuel.RiotApi;

namespace RiotGames.Client.CodeGeneration.RiotGamesApi;

using Path = KeyValuePair<string, RiotApiPathObject>;
using Paths = IEnumerable<KeyValuePair<string, RiotApiPathObject>>;
using Schema = KeyValuePair<string, RiotApiComponentSchemaObject>;
using Schemas = Dictionary<string, RiotApiComponentSchemaObject>;

internal static class RiotApiLinqQueries
{
    public static Paths WhereReferencesSchema(this Paths paths, Schema schema)
    {
        return paths.Where(p =>
        {
            var cSchema = p.Value?.Get?.Responses?["200"].Content.First().Value.Schema;
            if (cSchema == null) return false;
            var @ref = cSchema.Type == "array" ? cSchema.Items?.Ref : cSchema.Ref;
            return (@ref ?? throw new InvalidOperationException()).Remove("#/components/schemas/") == schema.Key;
        });
    }

    public static IEnumerable<Schema> WhereReferencesSchema(this Schemas schemas, Schema schema)
    {
        return schemas.Where(s =>
            (s.Value.Properties ?? throw new InvalidOperationException()).Any(p =>
            {
                string? @ref;
                if (p.Value.Type == "array")
                    @ref = p.Value.Items?.Ref;
                else
                    @ref = p.Value.Ref;

                return @ref?.Remove("#/components/schemas/") == schema.Key ||
                       p.Value?.AdditionalProperties?.Ref?.Remove("#/components/schemas/") == schema.Key;
            })
        );
    }

    public static Paths WhereReferenceNotNull(this Paths paths)
    {
        return paths.Where(p =>
            p.Value.Get?.Responses?["200"]?.Content?.First().Value?.Schema?.Ref != null ||
            p.Value.Get?.Responses?["200"]?.Content?.First().Value?.Schema?.Items?.Ref != null);
    }

    public static IEnumerable<IGrouping<string, Path>> GroupByGame(this Paths paths)
    {
        return paths.GroupBy(p => p.Key.SplitAndRemoveEmptyEntries('/').First());
    }
}