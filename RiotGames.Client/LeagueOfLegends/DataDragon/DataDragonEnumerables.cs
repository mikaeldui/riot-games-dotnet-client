using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace RiotGames.LeagueOfLegends.DataDragon;

[JsonReadOnlyCollection]
[DebuggerDisplay("Count = {Count}")]
[EditorBrowsable(EditorBrowsableState.Never)]
public class DataDragonCollection<TValue> : LeagueOfLegendsReadOnlyCollection<TValue>, IDataDragonObject
{
    public DataDragonCollection(IList<TValue> list) : base(list)
    {
    }
}