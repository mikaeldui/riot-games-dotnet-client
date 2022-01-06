using Humanizer.Inflections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.Client.CodeGeneration.LeagueClient
{
    internal static class LeagueClientHacks
    {
        static LeagueClientHacks()
        {
            ReservedPathParameters = ReservedIdentifiers.ToDictionary(kvp => "{" + kvp.Key + "}", kvp => "{" + kvp.Value + "}");
            Vocabularies.Default.AddUncountable("status");
        }

        public static void Activate() => Console.WriteLine("Hacks activated!");

        public static readonly IReadOnlyDictionary<string, string> ReservedIdentifiers = new Dictionary<string, string>
        {
            {"namespace", "@namespace" },
            {"product-id", "productId" }
        };

        public static readonly IReadOnlyDictionary<string, string> ReservedPathParameters;

        public static readonly IReadOnlyDictionary<string, string> EndpointWordCompilations = new Dictionary<string, string>
        {
            {"currentTournamentIds", "current-tournament-ids" },
            {"historyandwinners", "history-and-winners" },
            {"iconconfig", "icon-config" },
            {"matchhistory", "match-history" },
            {"thirdparty", "third-party" },
            {"stateInfo", "state-info" },
            {"didreset", "did-reset" },
            {"getlocation", "get-location" },
            {"whereami", "where-am-i" },
            {"champSelectInventory", "champ-select-inventory" },
            {"signedInventory", "signed-inventory" },
            {"tournamentlogos", "tournament-logos" },
            {"signedInventoryCache", "signed-inventory-cache" },
            {"signedWallet", "signed-wallet" },
            {"playtime", "play-time" },
            {"gamemode", "game-mode" },
            {"currentpage", "current-page" },
            {"customizationlimits", "customization-limits" },
            {"servicesettings", "service-settings" },
            {"currentSequenceEvent", "current-sequence-event" },
            {"userinfo", "user-info" },
            {"catalogByInstanceIds", "catalog-by-instance-ids" },
            {"getStoreUrl", "get-store-url" },
            {"giftablefriends", "giftable-friends" },
            {"itemKeysFromInstanceIds", "item-keys-from-instance-ids" },
            {"itemKeysFromOfferIds", "item-keys-from-offer-ids" },
            {"lastPage", "last-page" },
            {"paymentDetails", "payment-details" },
            {"hubFooterColors", "hub-footer-colors" },
            {"storePromos", "store-promos" },
            {"battlepass", "battle-pass" },
            {"swagger.json", "swagger-json" },
            {"openapi.json", "openapi-json" },
            {"riotclient", "riot-client" }
        };

        public static readonly string[] EndpointTypeSuffixes = new string[]
        {
            "as-string"
        };
    }
}
