using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;

namespace RiotGames.LeagueOfLegends
{
    [JsonIReadOnlyCollection, DebuggerDisplay("Count = {Count}")]
    public class LeagueOfLegendsCollection<TValue> : RiotGamesCollection<TValue>, ILeagueOfLegendsObject
    {
        public LeagueOfLegendsCollection(IList<TValue> list) : base(list)
        {
        }
    }

    [JsonIReadOnlyDictionary, DebuggerDisplay("Count = {Count}")]
    public class LeagueOfLegendsDictionary<TKey, TValue> : RiotGamesDictionary<TKey, TValue>, ILeagueOfLegendsObject
        where TKey : notnull
    {
        public LeagueOfLegendsDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary)
        {
        }
    }
}
