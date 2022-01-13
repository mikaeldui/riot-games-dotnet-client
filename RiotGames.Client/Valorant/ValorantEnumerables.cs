using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;

namespace RiotGames.Valorant
{
    [JsonIReadOnlyCollection, DebuggerDisplay("Count = {Count}")]
    public class ValorantCollection<TValue> : ReadOnlyCollection<TValue>, IValorantObject
    {
        public ValorantCollection(IList<TValue> list) : base(list)
        {
        }
    }

    [JsonIReadOnlyDictionary, DebuggerDisplay("Count = {Count}")]
    public class ValorantDictionary<TKey, TValue> : ReadOnlyDictionary<TKey, TValue>, IValorantObject
        where TKey : notnull
    {
        public ValorantDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary)
        {
        }
    }
}
