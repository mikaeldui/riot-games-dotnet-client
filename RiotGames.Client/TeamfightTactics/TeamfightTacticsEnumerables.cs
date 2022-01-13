using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;

namespace RiotGames.TeamfightTactics
{
    [JsonIReadOnlyCollection, DebuggerDisplay("Count = {Count}")]
    public class TeamfightTacticsCollection<TValue> : RiotGamesCollection<TValue>, ITeamfightTacticsObject
    {
        public TeamfightTacticsCollection(IList<TValue> list) : base(list)
        {
        }
    }

    [JsonIReadOnlyDictionary, DebuggerDisplay("Count = {Count}")]
    public class TeamfightTacticsDictionary<TKey, TValue> : RiotGamesDictionary<TKey, TValue>, ITeamfightTacticsObject
        where TKey : notnull
    {
        public TeamfightTacticsDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary)
        {
        }
    }
}
