using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanizer;
using MingweiSamuel;

namespace RiotGames.Client.CodeGeneration
{
    internal static class PathHelper
    {
        private static readonly IReadOnlyDictionary<string, string> _knownWords = new Dictionary<string, string>
        {
            {"challengerleagues", "challenger-leagues" },
            {"grandmasterleagues", "grand-master-leagues" },
            {"masterleagues", "master-leagues" },
            {"grandmaster", "grand-master" }
        };

        public static void AddPathAsEndpoints(this ClientGenerator cg, KeyValuePair<string, RiotApiOpenApiSchema.PathObject> path)
        {
            var po = path.Value;
            var poGet = po.Get;
            if (poGet != null)
            {
                var operationId = poGet.OperationId;
                var responseSchema = poGet?.Responses?["200"]?.Content?.First().Value.Schema;
                bool isArrayReponse = responseSchema?.XType == "array";
                var nameFromPath = _getNameFromPath(path.Key, isArrayReponse);

                cg.AddEndpoint(nameFromPath, HttpMethod.Get, path.Key, _responseType(responseSchema));
            }
        }

        private static string _getNameFromPath(string path, bool isPlural)
        {
            var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries)
                .Skip(1).ToArray(); // Skip "riot" or "lol"

            var apiEnd = parts[0].SplitAndRemoveEmptyEntries('-').Last();
            // [1] is "v1" or similiar.
            var dtoEnd = parts[2].SplitAndRemoveEmptyEntries('-').Last();

            if (apiEnd == dtoEnd.Singularize())
                return _toName(parts[0]);

            parts[2] = parts[2].Replace(_knownWords);

            {
                var dtoParts = parts[2].SplitAndRemoveEmptyEntries('-');
                if (isPlural)                
                    dtoParts[dtoParts.Length - 1] = dtoParts[dtoParts.Length - 1].Pluralize();                
                else
                    dtoParts[dtoParts.Length - 1] = dtoParts[dtoParts.Length - 1].Singularize();
                parts[2] = String.Join("-", parts[2]);
            }

            return _toName(parts[0]) + _toName(parts[2]);                
        }

        private static string _toName(string name) => name.Replace(_knownWords).ToPascalCase();

        private static string _responseType(RiotApiOpenApiSchema.PathObject.MethodObject.ResponseObject.ContentObject.SchemaObject schema)
        {
            if (schema.Ref != null)
                return schema.XType.Remove("Dto");

            if (schema.Format == null || schema.XType == null)
                Debugger.Break();
            else
                switch (schema.Type)
                {
                    case "array":
                        return schema.Format.Substring(6, schema.Format.Length - 6) + "[]";
                    case "integer":
                        return schema.XType;
                    default:
                        Debugger.Break();
                        break;
                }

            return "bla";
        }
    }
}
