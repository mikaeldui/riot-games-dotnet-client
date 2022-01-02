# Riot Games .NET Client
An unofficial .NET Client for Riot Games and their games League of Legends, Legends of Runeterra, Teamfight Tactics and Valorant.

## Examples

### Getting League of Legends masteries

```
using RiotGames.LeagueOfLegends;

using (var client = new LeagueOfLegendsClient("ABCD-ABCD-ABCD-ABCD"))
{
    var masteries = await client.GetMasteriesAsync("some-summoner-ID");
    foreach(var mastery in masteries)
        Console.PrintLine($"Champion #{mastery.ChampionId}: {mastery.championPoints} points");
}
```
