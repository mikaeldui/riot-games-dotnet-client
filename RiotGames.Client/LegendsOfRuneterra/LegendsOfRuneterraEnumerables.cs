using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;

namespace RiotGames.LegendsOfRuneterra
{
    [JsonReadOnlyCollection, DebuggerDisplay("Count = {Count}")]
    public class LegendsOfRuneterraReadOnlyCollection<TValue> : ReadOnlyCollection<TValue>, ILegendsOfRuneterraObject
    {
        public LegendsOfRuneterraReadOnlyCollection(IList<TValue> list) : base(list)
        {
        }
    }

    [JsonReadOnlyDictionary, DebuggerDisplay("Count = {Count}")]
    public class LegendsOfRuneterraReadOnlyDictionary<TKey, TValue> : ReadOnlyDictionary<TKey, TValue>, ILegendsOfRuneterraObject
        where TKey : notnull
    {
        public LegendsOfRuneterraReadOnlyDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary)
        {
        }
    }
}
