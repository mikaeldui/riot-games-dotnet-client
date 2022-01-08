using Humanizer;
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
    using Path = KeyValuePair<string, OpenApiPathObject<OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, LcuParameterObject, LcuSchemaObject>>;
    using Paths = IEnumerable<KeyValuePair<string, OpenApiPathObject<OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, LcuParameterObject, LcuSchemaObject>>>;

    internal static class LeagueClientEndpointsHelper
    {
        static LeagueClientEndpointsHelper() => LeagueClientHacks.Activate();

        public static string GetNameFromPath(string path, bool? isPlural)
        {
            var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries).Replace("{plugin}", "plugin").Where(s => !s.StartsWith('{')).ToArray();

            if (parts[1].StartsWith('v') && char.IsDigit(parts[1][1]))
            {
                if (parts.Length > 2)
                    parts = parts.Skip(2).ToArray();
                else
                    parts = parts.SkipLast(1).ToArray();

            }

            if (parts.Length == 1)
            {
                var name = _toName(parts[0]);
                if (isPlural != null)
                {
                    if (isPlural == true)
                        return name.Pluralize();
                    else
                        return name.Singularize();
                }

                return name;
            }


            string firstPart;
            string? secondPart;
            string? lastPart = null;
            {
                firstPart = parts[0];
                // [1] is "v1" or similiar.
                secondPart = parts[1];
                if (parts.Length <= 2)
                    isPlural = null;
                else if (!parts.Last().StartsWith('{'))
                {
                    lastPart = parts.Last();
                    if (LeagueClientHacks.EndpointTypeSuffixes.Contains(lastPart))
                        lastPart = parts.SkipLast(1).Last() + "-" + lastPart;
                }
            }

            // Make sure the secondPart is kebabed.
            //secondPart = secondPart.Replace(Hacks.EndpointWordCompilations);

            // Check if we just need the first part
            if (lastPart == null && firstPart != "current-summoner")
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

                    if (firstPart == "summoner" && parts.Length > 2 && parts[2].StartsWith("by-"))
                        return _toName(firstPart) + _toName(parts[2]);

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
                    return firstPart._toName() + lastPart._toName();
            }

            return firstPart._toName() + secondPart._toName() + lastPart?._toName();
        }


        public static string GetTypeName(this LcuParameterObject parameter)
        {
            return OpenApiComponentHelper.GetTypeNameFromString(parameter.Format ?? parameter.Type);
        }

        private static string _toName(this string name)
        {
            return name.Replace(LeagueClientHacks.EndpointWordCompilations).ToPascalCase();
        }

        public static string? GetGame(this Path path) =>
            path.Key?.SplitAndRemoveEmptyEntries('/')?.First();
    }
}
