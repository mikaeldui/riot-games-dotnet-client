using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RiotGames.LeagueOfLegends
{
    // TODO: Make structs


    /// <summary>
    /// Regional Routing Values for the Riot Games API. Not to be confused with <see cref="Platform"/>.
    /// </summary>
    /// <remarks>From https://developer.riotgames.com/docs/lol</remarks>
    public enum Region
    {
        [Display(Name = "Americas")]
        AMERICAS,
        [Display(Name = "Asia")]
        ASIA,
        [Display(Name = "Europe")]
        EUROPE
    }

    /// <summary>
    /// A.k.a. "server". Not to be confused with <see cref="Region"/>.
    /// </summary>
    public enum Platform
    {
        [Display(Name = "BR"), Description("Brazil")]
        BR1,
        [Display(Name = "EUN"), Description("Europe Nordic & East")]
        EUN1,
        [Display(Name = "EUW"), Description("Europe West")]
        EUW1,
        [Display(Name = "JP"), Description("Turkey")]
        JP1,
        [Display(Name = "KR"), Description("Republic of Korea")]
        KR,
        [Display(Name = "LAN"), Description("Latin America North")]
        LA1,
        [Display(Name = "LAS"), Description("Latin America South")]
        LA2,
        [Display(Name = "NA"), Description("North America")]
        NA1,
        [Display(Name = "OC"), Description("Oceania")]
        OC1,
        [Display(Name = "TR"), Description("Turkey")]
        TR1,
        [Display(Name = "RU"), Description("Oceania")]
        RU
    }
}
