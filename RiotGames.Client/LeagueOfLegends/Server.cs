using System.ComponentModel;
using System.Globalization;
using System.Net;
using Camille.Enums;

namespace RiotGames.LeagueOfLegends;
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
///     Can be cast to and from <see cref="LeagueOfLegends.Platform" />. Not to be confused with <see cref="Region" />.
/// </summary>
/// <remarks>From https://leagueoflegends.fandom.com/wiki/Servers </remarks>
public class Server : LeagueOfLegendsObject
{
    [EditorBrowsable(EditorBrowsableState.Always)]
    public static readonly Server BR = new(
        "Brazil",
        nameof(BR),
        PlatformRoute.BR1,
        new DateTime(2012, 9, 13),
        new[] {"pt-BR"},
        "São Paulo, SP, Brazil",
        new[] {"104.160.152.3"}
    );

    [EditorBrowsable(EditorBrowsableState.Always)]
    public static readonly Server EUNE = new(
        "Europe Nordic & East",
        nameof(EUNE),
        PlatformRoute.EUN1,
        new DateTime(2010, 7, 13),
        new[] {"cs-CZ", "en-US", "hu-HU", "pl-PL"},
        "Frankfurt, Germany",
        new[] {"104.160.142.3"}
    );

    [EditorBrowsable(EditorBrowsableState.Always)]
    public static readonly Server EUW = new(
        "Europe West",
        nameof(EUW),
        PlatformRoute.EUW1,
        new DateTime(2012, 9, 13),
        new[] {"en-US", "de-DE", "es-ES", "fr-FR", "it-IT"},
        "Amsterdam, Netherlands",
        new[] {"104.160.141.3"}
    );

    [EditorBrowsable(EditorBrowsableState.Always)]
    public static readonly Server JP = new(
        "Japan",
        nameof(JP),
        PlatformRoute.JP1,
        new DateTime(2016, 3, 15),
        new[] {"ja-JP"},
        "Tokyo, Japan",
        Array.Empty<string>()
    );

    [EditorBrowsable(EditorBrowsableState.Always)]
    public static readonly Server KR = new(
        "Republic of Korea",
        nameof(KR),
        PlatformRoute.KR,
        new DateTime(2011, 12, 12),
        new[] {"ko-KR"},
        "Seoul, South Korea",
        Array.Empty<string>()
    );

    [EditorBrowsable(EditorBrowsableState.Always)]
    public static readonly Server LAN = new(
        "Latin America North",
        nameof(LAN),
        PlatformRoute.LA1,
        new DateTime(2013, 6, 15),
        new[] {"es-MX"},
        "Miami, FL, United States",
        new[] {"104.160.136.3"}
    );

    [EditorBrowsable(EditorBrowsableState.Always)]
    public static readonly Server LAS = new(
        "Latin America South",
        nameof(LAS),
        PlatformRoute.LA2,
        new DateTime(2013, 6, 5),
        new[] {"es-AR"},
        "Santiago, Chile",
        Array.Empty<string>()
    );

    [EditorBrowsable(EditorBrowsableState.Always)]
    public static readonly Server NA = new(
        "North America",
        nameof(NA),
        PlatformRoute.NA1,
        new DateTime(2009, 10, 27),
        new[] {"en-US"},
        "Chicago, Illinois, United States",
        new[] {"104.160.131.3", "104.160.131.1"}
    );

    [EditorBrowsable(EditorBrowsableState.Always)]
    public static readonly Server OCE = new(
        "Oceania",
        nameof(OCE),
        PlatformRoute.OC1,
        new DateTime(2013, 4, 17),
        new[] {"en-AU"},
        "Sydney, Australia",
        new[] {"104.160.156.1"}
    );

    [EditorBrowsable(EditorBrowsableState.Always)]
    public static readonly Server TR = new(
        "Turkey",
        nameof(TR),
        PlatformRoute.TR1,
        new DateTime(2012, 9, 27),
        new[] {"tr-TR"},
        "Istanbul, Turkey",
        Array.Empty<string>()
    );

    [EditorBrowsable(EditorBrowsableState.Always)]
    public static readonly Server RU = new(
        "Russia",
        nameof(RU),
        PlatformRoute.RU,
        new DateTime(2013, 4, 17),
        new[] {"ru-RU"},
        "München, Germany",
        new[] {"162.249.73.2"}
    );

    [EditorBrowsable(EditorBrowsableState.Always)]
    public static readonly Server PBE = new(
        "Public Beta Environment",
        nameof(PBE),
        PlatformRoute.PBE1,
        new DateTime(2009, 4, 1),
        new[] {"en-US"},
        "Los Angeles, CA, United States",
        Array.Empty<string>()
    );

    [EditorBrowsable(EditorBrowsableState.Always)]
    public static readonly LeagueOfLegendsReadOnlyCollection<Server> All = new(new[]
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

    public Server(string name, string abbreviation, PlatformRoute platform, DateTime release, string[] languageCodes,
        string location, string[] ipAddresses)
    {
        Name = name;
        Abbreviation = abbreviation;
        Platform = platform;
        Release = release;
        Languages = new LeagueOfLegendsReadOnlyCollection<CultureInfo>(languageCodes.Select(l => new CultureInfo(l))
            .ToList());
        Location = location;
        IPAddresses =
            new LeagueOfLegendsReadOnlyCollection<IPAddress>(ipAddresses.Select(IPAddress.Parse).ToList());
    }

    /// <summary>
    ///     E.g. "Brazil".
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     E.g. "BR".
    /// </summary>
    public string Abbreviation { get; }

    public PlatformRoute Platform { get; }
    public DateTime Release { get; }
    public LeagueOfLegendsReadOnlyCollection<CultureInfo> Languages { get; }

    /// <summary>
    ///     E.g. "São Paulo, SP, Brazil".
    /// </summary>
    public string Location { get; }

    public LeagueOfLegendsReadOnlyCollection<IPAddress> IPAddresses { get; }

    [EditorBrowsable(EditorBrowsableState.Always)]
    public static bool TryParse(string value, out Server server)
    {
        if (Enum.TryParse<PlatformRoute>(value, true, out var platform))
        {
            server = platform;
            return true;
        }

        server = All.FirstOrDefault(s =>
            s.Name.Equals(value, StringComparison.OrdinalIgnoreCase) ||
            s.Abbreviation.Equals(value, StringComparison.OrdinalIgnoreCase) ||
            s.Location.Equals(value, StringComparison.OrdinalIgnoreCase) ||
            IPAddress.TryParse(value, out var ipAddress) && s.IPAddresses.Any(ip => ip == ipAddress)
        );

        return server != default;
    }

    /// <summary>
    ///     Tries to parse a server identifier.
    /// </summary>
    /// <param name="value">E.g. "euw1" or "EUNE" or "Amsterdam, Netherlands".</param>
    /// <exception cref="ArgumentException">Thrown if the value isn't a valid server identifier.</exception>
    [EditorBrowsable(EditorBrowsableState.Always)]
    public static Server Parse(string value)
    {
        if (!TryParse(value, out var server))
            throw new ArgumentException("Not a valid server identifier.", nameof(value));

        return server;
    }

    public static implicit operator PlatformRoute(Server s)
    {
        return s.Platform;
    }

    public static implicit operator Server(PlatformRoute p)
    {
        return p switch
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