using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;

namespace RiotGames.Valorant
{
    [JsonReadOnlyCollection, DebuggerDisplay("Count = {Count}")]
    public class ValorantReadOnlyCollection<TValue> : ReadOnlyCollection<TValue>, IValorantObject
    {
        public ValorantReadOnlyCollection(IList<TValue> list) : base(list)
        {
        }
    }

    [JsonReadOnlyDictionary, DebuggerDisplay("Count = {Count}")]
    public class ValorantReadOnlyDictionary<TKey, TValue> : ReadOnlyDictionary<TKey, TValue>, IValorantObject
        where TKey : notnull
    {
        public ValorantReadOnlyDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary)
        {
        }
    }
}
