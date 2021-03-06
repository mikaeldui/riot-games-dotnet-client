using Humanizer;
using MingweiSamuel;
using MingweiSamuel.Lcu;

namespace RiotGames.Client.CodeGeneration.LeagueClient;

using Path =
    KeyValuePair<string, OpenApiPathObject<OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>,
        OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>,
        OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, LcuParameterObject, LcuSchemaObject>>;
using Paths =
    IEnumerable<KeyValuePair<string, OpenApiPathObject<OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>,
        OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>,
        OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, LcuParameterObject, LcuSchemaObject>>>;

internal static class LeagueClientEndpointsHelper
{
    static LeagueClientEndpointsHelper()
    {
        LeagueClientHacks.Activate();
    }

    public static string GetNameFromPath(string path, bool? isPlural)
    {
        var parts = path
            //.ReplaceStart("/lol-", "/").ReplaceStart("/Tft", "/TeamfightTactics")
            .Split('/', StringSplitOptions.RemoveEmptyEntries);
        parts.Replace("{plugin}", "plugin");
        parts = parts.Where(s => !s.StartsWith('{')).ToArray();

        if (parts[1].StartsWith('v') && char.IsDigit(parts[1][1]))
        {
            parts = parts.Length > 2 ? parts.Skip(2).ToArray() : parts.SkipLast(1).ToArray();
        }

        if (parts.Length == 1)
        {
            var name = _toName(parts[0]);
            if (isPlural != null)
            {
                return isPlural == true ? name.Pluralize(false) : name.Singularize(false);
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
            {
                isPlural = null;
            }
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

            if (firstParts.Any(fp => secondParts.Select(sp => sp.Singularize(false)).Contains(fp)))
            {
                if (isPlural != null)
                {
                    if (isPlural.Value) secondParts.PluralizeLast(false);
                    else secondParts.SingularizeLast(false);
                }

                if (firstPart == "summoner" && parts.Length > 2 && parts[2].StartsWith("by-"))
                    return _toName(firstPart) + _toName(parts[2]);

                return _toName(string.Join('-', secondParts));
            }
        }

        // Pluralize it
        {
            var dtoParts = secondPart.SplitAndRemoveEmptyEntries('-');
            if (isPlural != null)
            {
                if (isPlural.Value && lastPart == null)
                    dtoParts.PluralizeLast(false);
                else
                    dtoParts.SingularizeLast(false);
            }

            secondPart = string.Join("-", dtoParts);
        }
        {
            if (lastPart != null && isPlural != null)
            {
                lastPart = isPlural.Value ? lastPart.Pluralize(false) : lastPart.Singularize(false);
            }
        }

        if (firstPart == secondPart || firstPart == secondPart.Singularize(false))
            if (lastPart != null)
                return firstPart._toName() + lastPart._toName();

        return firstPart._toName() + secondPart._toName() + lastPart?._toName();
    }

    public static string GetTypeName(this OpenApiSchemaObject schema)
    {
        var name = OpenApiComponentHelper.GetTypeName(schema);
        if (name.EndsWith("[]"))
            name = $"LeagueClientCollection<{name.RemoveEnd("[]")}>";

        return name;
    }


    public static string GetTypeName(this LcuParameterObject parameter)
    {
        return OpenApiComponentHelper.GetTypeNameFromString((parameter.Format ?? parameter.Type) ??
                                                            throw new InvalidOperationException());
    }

    private static string _toName(this string name)
    {
        return name.Replace(LeagueClientHacks.EndpointWordCompilations).ToPascalCase();
    }

    public static string? GetGame(this Path path)
    {
        return path.Key?.SplitAndRemoveEmptyEntries('/')?.First();
    }
}