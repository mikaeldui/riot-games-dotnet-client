using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;

namespace RiotGames.TeamfightTactics
{
    [JsonReadOnlyCollection, DebuggerDisplay("Count = {Count}")]
    public class TeamfightTacticsReadOnlyCollection<TValue> : ReadOnlyCollection<TValue>, ITeamfightTacticsObject
    {
        public TeamfightTacticsReadOnlyCollection(IList<TValue> list) : base(list)
        {
        }
    }

    [JsonReadOnlyDictionary, DebuggerDisplay("Count = {Count}")]
    public class TeamfightTacticsReadOnlyDictionary<TKey, TValue> : ReadOnlyDictionary<TKey, TValue>, ITeamfightTacticsObject
        where TKey : notnull
    {
        public TeamfightTacticsReadOnlyDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary)
        {
        }
    }
}
