using System.Diagnostics;
using MingweiSamuel;

namespace RiotGames.Client.CodeGeneration.LeagueClient;

internal static class LeagueClientModelsHelper
{
    [DebuggerStepThrough]
    internal static string FixGamePrefixes(this string input)
    {
        return input.RemoveStart("Lol").ReplaceStart("Tft", "TeamfightTactics");
    }

    public static string GetTypeName(this OpenApiComponentPropertyObject property)
    {
        return property.Ref != null
            ? OpenApiComponentHelper.GetTypeNameFromRef(property.Ref)
            : OpenApiComponentHelper.GetTypeNameFromString((property.Format ?? property.Type) ??
                                                           throw new InvalidOperationException());
    }

    public static string GetTypeName(this OpenApiSchemaObject schema)
    {
        return OpenApiComponentHelper.GetTypeName(schema);
    }
}