using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanizer;
using MingweiSamuel;
using Humanizer.Inflections;

namespace RiotGames.Client.CodeGeneration
{
    using Path = KeyValuePair<string, RiotApiOpenApiSchema.PathObject>;
    using Paths = IEnumerable<KeyValuePair<string, RiotApiOpenApiSchema.PathObject>>;
    using SchemaObject = RiotApiOpenApiSchema.ComponentsObject.SchemaObject;
    using Schema = KeyValuePair<string, RiotApiOpenApiSchema.ComponentsObject.SchemaObject>;
    using Schemas = Dictionary<string, RiotApiOpenApiSchema.ComponentsObject.SchemaObject>;

    internal static class ClientHelper
    {
        static ClientHelper()
        {
            Vocabularies.Default.AddUncountable("data");
        }

        private static readonly IReadOnlyDictionary<string, string> _knownWords = new Dictionary<string, string>
        {
            {"challengerleagues", "challenger-leagues" },
            {"grandmasterleagues", "grand-master-leagues" },
            {"masterleagues", "master-leagues" },
            {"grandmaster", "grand-master" },
            {"matchlist", "match-list" }
        };

        public static void AddPathAsEndpoints(this ClientGenerator cg, Path path)
        {
            //if (path.Key == "/lol/match/v5/matches/{matchId}/timeline") Debugger.Break();

            var po = path.Value;
            var poGet = po.Get;
            if (poGet != null)
            {
                var operationId = poGet.OperationId;
                var responseSchema = poGet?.Responses?["200"]?.Content?.First().Value.Schema;
                bool isArrayReponse = responseSchema?.Type == "array";
                var nameFromPath = _getNameFromPath(path.Key, isArrayReponse);

                Dictionary<string, string>? pathParameters = null;

                if (poGet.Parameters != null && poGet.Parameters.Length > 0)
                {
                    if (!poGet.Parameters.All(p => p.In == "path" || p.In == "header" || p.In == "query"))
                        Debugger.Break();

                    pathParameters = poGet.Parameters.Where(p => p.In != "header" && p.In != "query").ToDictionary(p => p.Name, p => p.Schema.XType ?? p.Schema.Type);
                }

                cg.AddEndpoint("Get" + nameFromPath, HttpMethod.Get, path.Key, _responseType(responseSchema), pathParameters: pathParameters);
            }
        }

        public static void AddPathsAsEndpoints(this ClientGenerator cg, Paths paths)
        {
            foreach (var path in paths)
                cg.AddPathAsEndpoints(path);
        }

        private static string _getNameFromPath(string path, bool? isPlural)
        {
            string firstPart;
            string secondPart;
            string? lastPart = null;
            {
                var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries)
                    .Skip(1).ToArray(); // Skip "riot" or "lol"
                firstPart = parts[0];
                // [1] is "v1" or similiar.
                secondPart = parts[2];
                if (parts.Length <= 3)
                    isPlural = null;
                else if (!parts.Last().StartsWith('{'))
                    lastPart = parts.Last();
            }

            // Make sure the secondPart is kebabed.
            secondPart = secondPart.Replace(_knownWords);

            // Check if we just need the first part
            if (lastPart == null)
            {
                var firstParts = firstPart.SplitAndRemoveEmptyEntries('-');
                var secondParts = secondPart.SplitAndRemoveEmptyEntries('-');

                if (firstParts.Any(fp => secondParts.Select(sp => sp.Singularize()).Contains(fp)))
                {
                    if (isPlural != null)
                    {
                        if (isPlural.Value) secondParts.PluralizeLast();
                        else secondParts.SingularizeLast();
                    }
                    return _toName(String.Join('-', secondParts));
                }
            }

            // Pluralize it
            {
                var dtoParts = secondPart.SplitAndRemoveEmptyEntries('-');
                if (isPlural != null)
                {
                    if (isPlural.Value && lastPart == null)
                        dtoParts.PluralizeLast();
                    else
                        dtoParts.SingularizeLast();
                }
                secondPart = String.Join("-", dtoParts);
            }
            {
                if (lastPart != null && isPlural != null)
                {
                    if (isPlural.Value)
                        lastPart = lastPart.Pluralize();
                    else
                        lastPart = lastPart.Singularize();
                }
            }

            if (lastPart != null && firstPart == secondPart)
                return _toName(firstPart) + _toName(lastPart);

            return _toName(firstPart) + _toName(secondPart) + _toName(lastPart);                
        }

        private static string _toName(string name)
        {
            if (name == null)
                return null;
            return name.Replace(_knownWords).ToPascalCase();
        }

        private static string _responseType(RiotApiOpenApiSchema.PathObject.MethodObject.ResponseObject.ContentObject.SchemaObject schema)
        {
            if (schema.Ref != null)
                return ModelHelper.RemoveDtoSuffix(schema.XType);

            if (schema.XType == null && schema.Type == null)
                Debugger.Break();
            else
                switch (schema.Type)
                {
                    case "array":
                        return ModelHelper.RemoveDtoSuffix(schema.XType.Remove("Set[").Remove("List[").TrimEnd(']')) + "[]";
                    case "integer":
                        return schema.XType;
                    case "string":
                        return schema.Type;
                    default:
                        Debugger.Break();
                        break;
                }

            return "bla";
        }

        public static string? GetGame(this Path path) =>
            path.Key?.SplitAndRemoveEmptyEntries('/')?.First();

        public static Paths WhereReferencesSchema(this Paths paths, Schema schema) =>
            paths.Where(p =>
                {
                    var cSchema = p.Value?.Get?.Responses?["200"].Content.First().Value.Schema;
                    if (cSchema == null) return false;
                    string @ref;
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
                    string @ref;
                    if (p.Value.Type == "array")
                        @ref = p.Value.Items.Ref;
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
