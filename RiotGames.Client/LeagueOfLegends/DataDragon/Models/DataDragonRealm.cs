using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace RiotGames.LeagueOfLegends.DataDragon
{
    public class DataDragonRealm : DataDragonObject
    {
        public DataDragonRealmNObject N { get; set; }
        public string V { get; set; }
        public string L { get; set; }
        public Uri Cdn { get; set; }
        public string Dd { get; set; }
        public string Lg { get; set; }
        public string Css { get; set; }
        public int ProfileIconMax { get; set; }
        public string Store { get; set; }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public class DataDragonRealmNObject : DataDragonObject
    {
        public string Item { get; set; }
        public string Rune { get; set; }
        public string Mastery { get; set; }
        public string Summoner { get; set; }
        public string Champion { get; set; }
        public string ProfileIcon { get; set; }
        public string Map { get; set; }
        public string Language { get; set; }
        public string Sticker { get; set; }
    }
}
