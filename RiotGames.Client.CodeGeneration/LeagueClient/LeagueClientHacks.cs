using Humanizer.Inflections;

namespace RiotGames.Client.CodeGeneration.LeagueClient;

internal static class LeagueClientHacks
{
    public static readonly IReadOnlyDictionary<string, string> ParameterIdentifierTypos = new Dictionary<string, string>
    {
        //{ "encryptedPUUID", "encryptedPuuid" }
    };

    public static readonly IReadOnlyDictionary<string, string> PathParameterIdentifierTypos =
        ParameterIdentifierTypos.ToDictionary(kvp => $"{{{kvp.Key}}}", kvp => $"{{{kvp.Value}}}");

    public static readonly
        IReadOnlyDictionary<string, IReadOnlyDictionary<(string typeName, string identifier), string>> BasicInterfaces =
            new Dictionary<string, IReadOnlyDictionary<(string typeName, string identifier), string>>
            {
                {
                    "LeagueClient", new Dictionary<(string typeName, string identifier), string>
                    {
                        {("long", "SummonerId"), "ISummonerId"},
                        {("string", "Puuid"), "IPuuid"},
                        {("long", "AccountId"), "IAccountId"},
                        {("long", "PlayerId"), "IPlayerId"},
                        {("int", "ChampionId"), "IChampionId"},
                        {("long", "GameId"), "IGameId"},
                        {("int", "MapId"), "IMapId"}
                    }
                }
                //{
                //    "LeagueOfLegends", new Dictionary<(string typeName, string identifier), string>
                //    {
                //        { ("string", "LeagueId"), "ILeagueOfLegendsLeagueId" },
                //        { ("string", "MatchId"), "ILeagueOfLegendsMatchId" },
                //        { ("int", "TournamentId"), "ILeagueOfLegendsTournamentId" }
                //    }
                //},
                //{
                //    "TeamfightTactics", new Dictionary<(string typeName, string identifier), string>
                //    {
                //        { ("string", "LeagueId"), "ITeamfightTacticsLeagueId" },
                //        { ("string", "MatchId"), "ITeamfightTacticsMatchId" }
                //    }
                //}
            };


    public static readonly IReadOnlyDictionary<string, string> ReservedIdentifiers = new Dictionary<string, string>
    {
        {"namespace", "@namespace"},
        {"product-id", "productId"}
    };

    public static readonly IReadOnlyDictionary<string, string> ReservedPathParameters;

    public static readonly IReadOnlyDictionary<string, string> EndpointWordCompilations = new Dictionary<string, string>
    {
        {"currentTournamentIds", "current-tournament-ids"},
        {"historyandwinners", "history-and-winners"},
        {"iconconfig", "icon-config"},
        {"matchhistory", "match-history"},
        {"thirdparty", "third-party"},
        {"stateInfo", "state-info"},
        {"didreset", "did-reset"},
        {"getlocation", "get-location"},
        {"whereami", "where-am-i"},
        {"champSelectInventory", "champ-select-inventory"},
        {"signedInventory", "signed-inventory"},
        {"tournamentlogos", "tournament-logos"},
        {"signedInventoryCache", "signed-inventory-cache"},
        {"signedWallet", "signed-wallet"},
        {"playtime", "play-time"},
        {"gamemode", "game-mode"},
        {"currentpage", "current-page"},
        {"customizationlimits", "customization-limits"},
        {"servicesettings", "service-settings"},
        {"currentSequenceEvent", "current-sequence-event"},
        {"userinfo", "user-info"},
        {"catalogByInstanceIds", "catalog-by-instance-ids"},
        {"getStoreUrl", "get-store-url"},
        {"giftablefriends", "giftable-friends"},
        {"itemKeysFromInstanceIds", "item-keys-from-instance-ids"},
        {"itemKeysFromOfferIds", "item-keys-from-offer-ids"},
        {"lastPage", "last-page"},
        {"paymentDetails", "payment-details"},
        {"hubFooterColors", "hub-footer-colors"},
        {"storePromos", "store-promos"},
        {"battlepass", "battle-pass"},
        {"swagger.json", "swagger-json"},
        {"openapi.json", "openapi-json"},
        {"riotclient", "riot-client"}
    };

    public static readonly string[] EndpointTypeSuffixes =
    {
        "as-string"
    };

    static LeagueClientHacks()
    {
        ReservedPathParameters =
            ReservedIdentifiers.ToDictionary(kvp => "{" + kvp.Key + "}", kvp => "{" + kvp.Value + "}");
        Vocabularies.Default.AddUncountable("status");
    }

    public static void Activate()
    {
        Console.WriteLine("Hacks activated!");
    }

    /// <param name="client">Either "LeagueClient", "LeagueOfLegends" or "TeamfightTactics"</param>
    public static bool TryGetBasicInterfaceIdentifier(
        this IReadOnlyDictionary<string, IReadOnlyDictionary<(string typeName, string identifier), string>>
            basicInterfaces,
        string client, string propertyTypeName, string propertyIdentifier, out string interfaceIdentifier)
    {
#pragma warning disable CS8601 // Possible null reference assignment.
        // Start with game-specific interface
        if (basicInterfaces.TryGetValue(client, out var clientBasicInterfaces)
            && clientBasicInterfaces.TryGetValue((propertyTypeName.RemoveEnd("?"), propertyIdentifier),
                out interfaceIdentifier))
            return true;
        // If not found, then maybe there's a Riot interface that could be used
        if (BasicInterfaces["LeagueClient"]
            .TryGetValue((propertyTypeName.RemoveEnd("?"), propertyIdentifier), out interfaceIdentifier))
            return true;
#pragma warning restore CS8601 // Possible null reference assignment.

        return false;
    }
}