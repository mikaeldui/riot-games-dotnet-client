using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanizer;
using MingweiSamuel;

namespace RiotGames.Client.CodeGeneration.RiotGamesApi
{
    using Path = KeyValuePair<string, RiotApiPathObject>;
    using Paths = IEnumerable<KeyValuePair<string, RiotApiPathObject>>;

    internal static class ClientHelper
    {
        static ClientHelper() => Hacks.Activate();

        public static void AddPathAsEndpoints(this ClientGenerator cg, Path path)
        {
            //if (path.Key == "/lol/match/v5/matches/{matchId}/timeline") Debugger.Break();

            var po = path.Value;
            var poGet = po.Get;
            if (poGet != null)
            {
                var responseSchema = poGet?.Responses?["200"]?.Content?.First().Value.Schema;
                bool isArrayReponse = responseSchema?.Type == "array";
                var nameFromPath = _getNameFromPath(path.Key, isArrayReponse);
                bool isPlatform = path.Value.XRouteEnum != "regional";

                Dictionary<string, string?>? pathParameters = null;

                if (poGet is { Parameters: not null, Parameters: { Length: > 0 } })
                {
                    if (!poGet.Parameters.All(p => p.In is "path" or "header" or "query"))
                        Debugger.Break();

                    pathParameters = poGet.Parameters.Where(p => p.In is not "header" and not "query").ToDictionary(p => p.Name, p => p.Schema?.XType ?? p.Schema?.Type);
                }

                cg.AddEndpoint("Get" + nameFromPath, isPlatform, HttpMethod.Get, path.Key, responseSchema.GetTypeName(), pathParameters: pathParameters);
            }
        }

        public static void AddPathsAsEndpoints(this ClientGenerator cg, Paths paths)
        {
            foreach (var path in paths)
                cg.AddPathAsEndpoints(path);
        }

        private static string? _getNameFromPath(string path, bool? isPlural)
        {
            var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries)
                .Skip(1).ToArray(); // Skip "riot" or "lol"
            string firstPart;
            string secondPart;
            string? lastPart = null;
            {                
                firstPart = parts[0];
                // [1] is "v1" or similiar.
                secondPart = parts[2];
                if (parts.Length <= 3)
                    isPlural = null;
                else if (!parts.Last().StartsWith('{'))
                    lastPart = parts.Last();
            }

            // Make sure the secondPart is kebabed.
            secondPart = secondPart.Replace(Hacks.EndpointWordCompilations);

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

                    if (firstPart == "summoner" && parts.Length > 3 && parts[3].StartsWith("by-"))
                        return _toName(firstPart) + _toName(parts[3]);

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

            if (firstPart == secondPart || firstPart == secondPart.Singularize())
            {
                if (lastPart != null)
                    return _toName(firstPart) + _toName(lastPart);
            }

            return _toName(firstPart) + _toName(secondPart) + _toName(lastPart);                
        }

        private static string? _toName(string name)
        {
            if (name == null)
                return null;
            return name.Replace(Hacks.EndpointWordCompilations).ToPascalCase();
        }

        public static string? GetGame(this Path path) =>
            path.Key?.SplitAndRemoveEmptyEntries('/')?.First();
    }
}
