using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace RiotGames.Valorant;

[JsonReadOnlyCollection]
[DebuggerDisplay("Count = {Count}")]
[EditorBrowsable(EditorBrowsableState.Never)]
public class ValorantReadOnlyCollection<TValue> : ReadOnlyCollection<TValue>, IValorantObject
{
    public ValorantReadOnlyCollection(IList<TValue> list) : base(list)
    {
    }
}

[JsonReadOnlyDictionary]
[DebuggerDisplay("Count = {Count}")]
[EditorBrowsable(EditorBrowsableState.Never)]
public class ValorantReadOnlyDictionary<TKey, TValue> : ReadOnlyDictionary<TKey, TValue>, IValorantObject
    where TKey : notnull
{
    public ValorantReadOnlyDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary)
    {
    }
}