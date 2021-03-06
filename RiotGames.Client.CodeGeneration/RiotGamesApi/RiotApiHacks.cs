using Humanizer.Inflections;

namespace RiotGames.Client.CodeGeneration.RiotGamesApi;

/// <summary>
///     Contains hacks to make everything work a little better.
///     These probably needs to be updated regularly.
/// </summary>
internal static class RiotApiHacks
{
    public static readonly IReadOnlyDictionary<string, string> EndpointWordCompilations = new Dictionary<string, string>
    {
        {"challengerleagues", "challenger-leagues"},
        {"grandmasterleagues", "grand-master-leagues"},
        {"masterleagues", "master-leagues"},
        {"grandmaster", "grand-master"},
        {"matchlist", "match-list"}
    };

    public static readonly IReadOnlyDictionary<string, string> EndpointsWithDuplicateSchemas =
        new Dictionary<string, string>
        {
            {"league-exp-v4.", "LeagueExp"},
            {"tournament-stub-v4.", "TournamentStub"},
            {"spectator-v4.", "Spectator"},
            {"clash-v1.Team", "Clash"},
            {"lor-ranked-v1.Player", "Ranked"},
            {"val-ranked-v1.Player", "Ranked"},
            {"val-status-v1.Content", "Status"}
        };

    public static readonly IReadOnlyDictionary<string, string> ParameterIdentifierTypos = new Dictionary<string, string>
    {
        {"encryptedPUUID", "encryptedPuuid"}
    };

    public static readonly IReadOnlyDictionary<string, string> PathParameterIdentifierTypos =
        ParameterIdentifierTypos.ToDictionary(kvp => $"{{{kvp.Key}}}", kvp => $"{{{kvp.Value}}}");

    public static readonly
        IReadOnlyDictionary<string, IReadOnlyDictionary<(string typeName, string identifier), string>> BasicInterfaces =
            new Dictionary<string, IReadOnlyDictionary<(string typeName, string identifier), string>>
            {
                {
                    "RiotGames", new Dictionary<(string typeName, string identifier), string>
                    {
                        {("string", "EncryptedPuuid"), "IEncryptedPuuid"},
                        {("string", "EncryptedAccountId"), "IEncryptedAccountId"},
                        {("string", "EncryptedSummonerId"), "IEncryptedSummonerId"}
                    }
                },
                {
                    "LeagueOfLegends", new Dictionary<(string typeName, string identifier), string>
                    {
                        {("string", "LeagueId"), "ILeagueId"},
                        {("string", "MatchId"), "IMatchId"},
                        {("int", "TournamentId"), "ITournamentId"}
                    }
                },
                {
                    "LegendsOfRuneterra", new Dictionary<(string typeName, string identifier), string>
                    {
                        {("string", "MatchId"), "IMatchId"}
                    }
                },
                {
                    "TeamfightTactics", new Dictionary<(string typeName, string identifier), string>
                    {
                        {("string", "LeagueId"), "ILeagueId"},
                        {("string", "MatchId"), "IMatchId"}
                    }
                },
                {
                    "Valorant", new Dictionary<(string typeName, string identifier), string>
                    {
                        {("string", "MatchId"), "IMatchId"}
                    }
                }
            };

    // Because Riot has yet to update their specs.
    public static readonly IReadOnlyDictionary<string, string> BadPropertyIdentifiers = new Dictionary<string, string>
    {
        {"Puuid", "EncryptedPuuid"},
        {"AccountId", "EncryptedAccountId"},
        {"SummonerId", "EncryptedSummonerId"},
        {"12AssistStreakCount", "DozenAssistStreakCount"} // And this idk.
    };

    public static readonly IReadOnlyDictionary<string, string> BadParameterIdentifiers =
        BadPropertyIdentifiers.ToDictionary(kvp => kvp.Key.ToCamelCase(), kvp => kvp.Value.ToCamelCase());

    public static readonly IReadOnlyDictionary<string, string> OldPathParameterIdentifiers =
        BadParameterIdentifiers.ToDictionary(kvp => $"{{{kvp.Key}}}", kvp => $"{{{kvp.Value}}}");

    static RiotApiHacks()
    {
        Vocabularies.Default.AddUncountable("data");
        Vocabularies.Default.AddUncountable("me");
    }

    public static void Activate()
    {
        Console.WriteLine("Hacks activated!");
    }

    public static bool TryGetBasicInterfaceIdentifier(
        this IReadOnlyDictionary<string, IReadOnlyDictionary<(string typeName, string identifier), string>>
            basicInterfaces,
        Client client, string propertyTypeName, string propertyIdentifier, out string interfaceIdentifier)
    {
#pragma warning disable CS8601 // Possible null reference assignment.
        // Start with game-specific interface
        if (basicInterfaces.TryGetValue(client.ToString(), out var clientBasicInterfaces)
            && clientBasicInterfaces.TryGetValue((propertyTypeName.RemoveEnd("?"), propertyIdentifier),
                out interfaceIdentifier))
            return true;
        // If not found, then maybe there's a Riot interface that could be used
        return BasicInterfaces[Client.RiotGames.ToString()]
            .TryGetValue((propertyTypeName.RemoveEnd("?"), propertyIdentifier), out interfaceIdentifier);
#pragma warning restore CS8601 // Possible null reference assignment.
    }
}