using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace RiotGames.LegendsOfRuneterra;

[JsonReadOnlyCollection]
[DebuggerDisplay("Count = {Count}")]
[EditorBrowsable(EditorBrowsableState.Never)]
public class LegendsOfRuneterraReadOnlyCollection<TValue> : ReadOnlyCollection<TValue>, ILegendsOfRuneterraObject
{
    public LegendsOfRuneterraReadOnlyCollection(IList<TValue> list) : base(list)
    {
    }
}

[JsonReadOnlyDictionary]
[DebuggerDisplay("Count = {Count}")]
[EditorBrowsable(EditorBrowsableState.Never)]
public class LegendsOfRuneterraReadOnlyDictionary<TKey, TValue> : ReadOnlyDictionary<TKey, TValue>,
    ILegendsOfRuneterraObject
    where TKey : notnull
{
    public LegendsOfRuneterraReadOnlyDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary)
    {
    }
}