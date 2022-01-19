using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;

namespace RiotGames.LeagueOfLegends.DataDragon
{
    [JsonReadOnlyCollection, DebuggerDisplay("Count = {Count}")]
    public class DataDragonCollection<TValue> : LeagueOfLegendsReadOnlyCollection<TValue>, IDataDragonObject
    {
        public DataDragonCollection(IList<TValue> list) : base(list)
        {
        }
    }
}
