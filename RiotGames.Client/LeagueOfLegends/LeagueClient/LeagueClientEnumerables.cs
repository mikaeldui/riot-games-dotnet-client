using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;

namespace RiotGames.LeagueOfLegends.LeagueClient
{
    [JsonIReadOnlyCollection, DebuggerDisplay("Count = {Count}")]
    public class LeagueClientCollection<TValue> : ReadOnlyCollection<TValue>, ILeagueClientObject
    {
        public LeagueClientCollection(IList<TValue> list) : base(list)
        {
        }
    }

    [JsonIReadOnlyDictionary, DebuggerDisplay("Count = {Count}")]
    public class LeagueClientDictionary<TKey, TValue> : ReadOnlyDictionary<TKey, TValue>, ILeagueClientObject
        where TKey : notnull
    {
        public LeagueClientDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary)
        {
        }
    }
}
