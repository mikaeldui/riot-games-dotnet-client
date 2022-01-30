using Camille.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Net;
using System.Text;

namespace RiotGames.LeagueOfLegends
{
    ///// <summary>
    ///// Regional Routing Values for the Riot Games API. Not to be confused with <see cref="Platform"/>.
    ///// </summary>
    ///// <remarks>From https://developer.riotgames.com/docs/lol </remarks>
    //public enum Region
    //{
    //    [Display(Name = "Americas")]
    //    AMERICAS,
    //    [Display(Name = "Asia")]
    //    ASIA,
    //    [Display(Name = "Europe")]
    //    EUROPE
    //}

    /// <summary>
    /// Can be cast to and from <see cref="LeagueOfLegends.Platform"/>. Not to be confused with <see cref="Region"/>.
    /// </summary>
    /// <remarks>From https://leagueoflegends.fandom.com/wiki/Servers </remarks>
    public class Server : LeagueOfLegendsObject
    {
        public Server(string name, string abbreviation, PlatformRoute platform, DateTime release, string[] languageCodes, string location, string[] ipAddresses)
        {
            Name = name;
            Abbreviation = abbreviation;
            Platform = platform;
            Release = release;
            Languages = new LeagueOfLegendsReadOnlyCollection<CultureInfo>(languageCodes.Select(l => new CultureInfo(l)).ToList());
            Location = location;
            IPAddresses = new LeagueOfLegendsReadOnlyCollection<IPAddress>(ipAddresses.Select(ip => IPAddress.Parse(ip)).ToList());
        }

        /// <summary>
        /// E.g. "Brazil".
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// E.g. "BR".
        /// </summary>
        public string Abbreviation { get; private set; }
        public PlatformRoute Platform { get; private set; }
        public DateTime Release { get; private set; }
        public LeagueOfLegendsReadOnlyCollection<CultureInfo> Languages { get; private set; }

        /// <summary>
        /// E.g. "São Paulo, SP, Brazil".
        /// </summary>
        public string Location { get; private set; }
        public LeagueOfLegendsReadOnlyCollection<IPAddress> IPAddresses { get; private set; }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public static readonly Server BR = new(
            name: "Brazil",
            abbreviation: nameof(BR),
            platform: PlatformRoute.BR1,
            release: new DateTime(2012, 9, 13),
            languageCodes: new[] { "pt-BR" },
            location: "São Paulo, SP, Brazil",
            ipAddresses: new[] { "104.160.152.3" }
        );

        [EditorBrowsable(EditorBrowsableState.Always)]
        public static readonly Server EUNE = new(
            name: "Europe Nordic & East",
            abbreviation: nameof(EUNE),
            platform: PlatformRoute.EUN1,
            release: new DateTime(2010, 7, 13),
            languageCodes: new[] { "cs-CZ", "en-US", "hu-HU", "pl-PL" },
            location: "Frankfurt, Germany",
            ipAddresses: new[] { "104.160.142.3" }
        );

        [EditorBrowsable(EditorBrowsableState.Always)]
        public static readonly Server EUW = new(
            name: "Europe West",
            abbreviation: nameof(EUW),
            platform: PlatformRoute.EUW1,
            release: new DateTime(2012, 9, 13),
            languageCodes: new[] { "en-US", "de-DE", "es-ES", "fr-FR", "it-IT" },
            location: "Amsterdam, Netherlands",
            ipAddresses: new[] { "104.160.141.3" }
        );

        [EditorBrowsable(EditorBrowsableState.Always)]
        public static readonly Server JP = new(
            name: "Japan",
            abbreviation: nameof(JP),
            platform: PlatformRoute.JP1,
            release: new DateTime(2016, 3, 15),
            languageCodes: new[] { "ja-JP" },
            location: "Tokyo, Japan",
            ipAddresses: new string[0]
        );

        [EditorBrowsable(EditorBrowsableState.Always)]
        public static readonly Server KR = new(
            name: "Republic of Korea",
            abbreviation: nameof(KR),
            platform: PlatformRoute.KR,
            release: new DateTime(2011, 12, 12),
            languageCodes: new[] { "ko-KR" },
            location: "Seoul, South Korea",
            ipAddresses: new string[0]
        );

        [EditorBrowsable(EditorBrowsableState.Always)]
        public static readonly Server LAN = new(
            name: "Latin America North",
            abbreviation: nameof(LAN),
            platform: PlatformRoute.LA1,
            release: new DateTime(2013, 6, 15),
            languageCodes: new[] { "es-MX" },
            location: "Miami, FL, United States",
            ipAddresses: new[] { "104.160.136.3" }
        );

        [EditorBrowsable(EditorBrowsableState.Always)]
        public static readonly Server LAS = new(
            name: "Latin America South",
            abbreviation: nameof(LAS),
            platform: PlatformRoute.LA2,
            release: new DateTime(2013, 6, 5),
            languageCodes: new[] { "es-AR" },
            location: "Santiago, Chile",
            ipAddresses: new string[0]
        );

        [EditorBrowsable(EditorBrowsableState.Always)]
        public static readonly Server NA = new(
            name: "North America",
            abbreviation: nameof(NA),
            platform: PlatformRoute.NA1,
            release: new DateTime(2009, 10, 27),
            languageCodes: new[] { "en-US" },
            location: "Chicago, Illinois, United States",
            ipAddresses: new[] { "104.160.131.3", "104.160.131.1" }
        );

        [EditorBrowsable(EditorBrowsableState.Always)]
        public static readonly Server OCE = new(
            name: "Oceania",
            abbreviation: nameof(OCE),
            platform: PlatformRoute.OC1,
            release: new DateTime(2013, 4, 17),
            languageCodes: new[] { "en-AU" },
            location: "Sydney, Australia",
            ipAddresses: new[] { "104.160.156.1" }
        );

        [EditorBrowsable(EditorBrowsableState.Always)]
        public static readonly Server TR = new(
            name: "Turkey",
            abbreviation: nameof(TR),
            platform: PlatformRoute.TR1,
            release: new DateTime(2012, 9, 27),
            languageCodes: new[] { "tr-TR" },
            location: "Istanbul, Turkey",
            ipAddresses: new string[0]
        );

        [EditorBrowsable(EditorBrowsableState.Always)]
        public static readonly Server RU = new(
            name: "Russia",
            abbreviation: nameof(RU),
            platform: PlatformRoute.RU,
            release: new DateTime(2013, 4, 17),
            languageCodes: new[] { "ru-RU" },
            location: "München, Germany",
            ipAddresses: new[] { "162.249.73.2" }
        );

        [EditorBrowsable(EditorBrowsableState.Always)]
        public static readonly Server PBE = new(
            name: "Public Beta Environment",
            abbreviation: nameof(PBE),
            platform: PlatformRoute.PBE1,
            release: new DateTime(2009, 4, 1),
            languageCodes: new[] { "en-US" },
            location: "Los Angeles, CA, United States",
            ipAddresses: new string[0]
        );

        [EditorBrowsable(EditorBrowsableState.Always)]
        public static readonly LeagueOfLegendsReadOnlyCollection<Server> All = new(new Server[]
        {
            BR,
            EUNE,
            EUW,
            JP,
            KR,
            LAN,
            LAS,
            NA,
            OCE,
            TR,
            RU,
            PBE
        });

        [EditorBrowsable(EditorBrowsableState.Always)]
        public static bool TryParse(string value, out Server server)
        {
            if (Enum.TryParse<PlatformRoute>(value, true, out var platform))
            {
                server = (Server)platform;
                return true;
            }

            server = All.FirstOrDefault(s => 
                s.Name.Equals(value, StringComparison.OrdinalIgnoreCase) ||
                s.Abbreviation.Equals(value, StringComparison.OrdinalIgnoreCase) ||
                s.Location.Equals(value, StringComparison.OrdinalIgnoreCase) ||
                (IPAddress.TryParse(value, out var ipAddress) && s.IPAddresses.Any(ip => ip == ipAddress))
            );

            return server != default;
        }

        /// <summary>
        /// Tries to parse a server identifier.
        /// </summary>
        /// <param name="value">E.g. "euw1" or "EUNE" or "Amsterdam, Netherlands".</param>
        /// <exception cref="ArgumentException">Thrown if the value isn't a valid server identifier.</exception>
        [EditorBrowsable(EditorBrowsableState.Always)]
        public static Server Parse(string value)
        {
            if (!TryParse(value, out Server server))
                throw new ArgumentException("Not a valid server identifier.", nameof(value));

            return server;
        }

        public static implicit operator PlatformRoute(Server s) => s.Platform;
        public static implicit operator Server(PlatformRoute p) => p switch
        {
            PlatformRoute.BR1 => BR,
            PlatformRoute.EUN1 => EUNE,
            PlatformRoute.EUW1 => EUW,
            PlatformRoute.JP1 => JP,
            PlatformRoute.KR => KR,
            PlatformRoute.LA1 => LAN,
            PlatformRoute.LA2 => LAS,
            PlatformRoute.NA1 => NA,
            PlatformRoute.OC1 => OCE,
            PlatformRoute.TR1 => TR,
            PlatformRoute.RU => RU,
            PlatformRoute.PBE1 => PBE,
            _ => throw new NotImplementedException("This platform hasn't been implemented.")
        };
    }

    ///// <summary>
    ///// Can be cast to and from <see cref="Server"/>. Not to be confused with <see cref="Region"/>.
    ///// </summary>
    //public enum Platform
    //{
    //    [Display(Name = "BR"), Description("Brazil")]
    //    BR1,
    //    [Display(Name = "EUNE"), Description("Europe Nordic & East")]
    //    EUN1,
    //    [Display(Name = "EUW"), Description("Europe West")]
    //    EUW1,
    //    [Display(Name = "JP"), Description("Turkey")]
    //    JP1,
    //    [Display(Name = "KR"), Description("Republic of Korea")]
    //    KR,
    //    [Display(Name = "LAN"), Description("Latin America North")]
    //    LA1,
    //    [Display(Name = "LAS"), Description("Latin America South")]
    //    LA2,
    //    [Display(Name = "NA"), Description("North America")]
    //    NA1,
    //    [Display(Name = "OC"), Description("Oceania")]
    //    OC1,
    //    [Display(Name = "TR"), Description("Turkey")]
    //    TR1,
    //    [Display(Name = "RU"), Description("Oceania")]
    //    RU
    //}
}
