using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;

namespace RiotGames.LegendsOfRuneterra
{
    [JsonIReadOnlyCollection, DebuggerDisplay("Count = {Count}")]
    public class LegendsOfRuneterraCollection<TValue> : RiotGamesCollection<TValue>, ILegendsOfRuneterraObject
    {
        public LegendsOfRuneterraCollection(IList<TValue> list) : base(list)
        {
        }
    }

    [JsonIReadOnlyDictionary, DebuggerDisplay("Count = {Count}")]
    public class LegendsOfRuneterraDictionary<TKey, TValue> : RiotGamesDictionary<TKey, TValue>, ILegendsOfRuneterraObject
        where TKey : notnull
    {
        public LegendsOfRuneterraDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary)
        {
        }
    }
}
