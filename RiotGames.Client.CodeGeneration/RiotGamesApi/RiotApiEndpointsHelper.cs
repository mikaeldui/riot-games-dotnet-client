using Humanizer;
using MingweiSamuel.RiotApi;

namespace RiotGames.Client.CodeGeneration.RiotGamesApi;

using Path = KeyValuePair<string, RiotApiPathObject>;
using Paths = IEnumerable<KeyValuePair<string, RiotApiPathObject>>;

internal static class RiotApiEndpointsHelper
{
    static RiotApiEndpointsHelper()
    {
        RiotApiHacks.Activate();
    }

    public static string? GetNameFromPath(string path, bool? isPlural)
    {
        var parts = path.SplitAndRemoveEmptyEntries('/')
            .Skip(1).ToArray(); // Skip "riot" or "lol"
        string firstPart;
        string secondPart;
        string? lastPart = null;
        {
            firstPart = parts[0];
            // [1] is "v1" or similar.
            secondPart = parts[2];
            if (parts.Length <= 3)
                isPlural = null;
            else if (!parts.Last().StartsWith('{'))
                lastPart = parts.Last();
        }

        // Make sure the secondPart is kebabed.
        secondPart = secondPart.Replace(RiotApiHacks.EndpointWordCompilations);

        // Check if we just need the first part
        if (lastPart == null)
        {
            var firstParts = firstPart.SplitAndRemoveEmptyEntries('-');
            var secondParts = secondPart.SplitAndRemoveEmptyEntries('-');

            if (firstParts.Any(fp => secondParts.Select(sp => sp.Singularize(false)).Contains(fp)))
            {
                if (isPlural != null)
                {
                    if (isPlural.Value) secondParts.PluralizeLast();
                    else secondParts.SingularizeLast(false);
                }

                if (firstPart == "summoner" && parts.Length > 3 && parts[3].StartsWith("by-"))
                    return ToName(firstPart) + ToName(parts[3]);

                return ToName(string.Join('-', secondParts));
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
                    dtoParts.SingularizeLast(false);
            }

            secondPart = string.Join("-", dtoParts);
        }
        {
            if (lastPart != null && isPlural != null)
                lastPart = isPlural.Value ? lastPart.Pluralize() : lastPart.Singularize(false);
        }

        if (firstPart == secondPart || firstPart == secondPart.Singularize(false))
            if (lastPart != null)
                return ToName(firstPart) + ToName(lastPart);

        return ToName(firstPart) + ToName(secondPart) + (lastPart != null ? ToName(lastPart) : "");
    }

    public static string ToName(string name)
    {
        return name.Replace(RiotApiHacks.EndpointWordCompilations).ToPascalCase();
    }

    public static string? GetGame(this Path path)
    {
        return path.Key?.SplitAndRemoveEmptyEntries('/')?.First();
    }
}