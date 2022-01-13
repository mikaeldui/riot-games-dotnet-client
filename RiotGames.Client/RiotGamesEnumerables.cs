using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;

namespace RiotGames
{
    [JsonIReadOnlyCollection, DebuggerDisplay("Count = {Count}")]
    public class RiotGamesCollection<TValue> : ReadOnlyCollection<TValue>, IRiotGamesObject
    {
        public RiotGamesCollection(IList<TValue> list) : base(list)
        {
        }
    }

    [JsonIReadOnlyDictionary, DebuggerDisplay("Count = {Count}")]
    public class RiotGamesDictionary<TKey, TValue> : ReadOnlyDictionary<TKey, TValue>, IRiotGamesObject
        where TKey : notnull
    {
        public RiotGamesDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary)
        {
        }
    }
}
