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
    internal static class PathHelper
    {
        static PathHelper()
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

        public static void AddPathAsEndpoints(this ClientGenerator cg, KeyValuePair<string, RiotApiOpenApiSchema.PathObject> path)
        {
            // if (path.Key == "/lol/league/v4/challengerleagues/by-queue/{queue}") Debugger.Break();

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

                    pathParameters = poGet.Parameters.Where(p => p.In != "header" && p.In != "query").ToDictionary(p => p.Name, p => p.Schema.Type);
                }

                cg.AddEndpoint("Get" + nameFromPath, HttpMethod.Get, path.Key, _responseType(responseSchema), pathParameters: pathParameters);
            }
        }

        private static string _getNameFromPath(string path, bool? isPlural)
        {
            string firstPart;
            string secondPart;
            {
                var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries)
                    .Skip(1).ToArray(); // Skip "riot" or "lol"
                firstPart = parts[0];
                // [1] is "v1" or similiar.
                secondPart = parts[2];
                if (parts.Length <= 3)
                    isPlural = null;
            }

            // Make sure the secondPart is kebabed.
            secondPart = secondPart.Replace(_knownWords);

            // Check if we just need the first part
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
                    if (isPlural.Value)
                        dtoParts.PluralizeLast();
                    else
                        dtoParts.SingularizeLast();
                }
                secondPart = String.Join("-", dtoParts);
            }

            return _toName(firstPart) + _toName(secondPart);                
        }

        private static string _toName(string name) => name.Replace(_knownWords).ToPascalCase();

        private static string _responseType(RiotApiOpenApiSchema.PathObject.MethodObject.ResponseObject.ContentObject.SchemaObject schema)
        {
            if (schema.Ref != null)
                return schema.XType.Remove("Dto").Remove("DTO");

            if (schema.XType == null && schema.Type == null)
                Debugger.Break();
            else
                switch (schema.Type)
                {
                    case "array":
                        return schema.XType.Remove("Set[").Remove("List[").TrimEnd(']').Remove("Dto").Remove("DTO") + "[]";
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
    }
}
