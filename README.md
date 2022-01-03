# Riot Games .NET Client
An unofficial .NET Client for Riot Games and their games League of Legends, Legends of Runeterra, Teamfight Tactics and Valorant.

It differs from the [Camille](https://github.com/MingweiSamuel/Camille) project by being more minimalistic.

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

### Getting Legends of Runeterra leadersboard

```
using RiotGames.LeagueOfLegends;

using (var client = new LegendsOfRuneterraClient("ABCD-ABCD-ABCD-ABCD"))
{
    var leaderboards = await client.GetRankedLeaderboardsAsync();
    foreach(var player in leaderboards.Players)
        Console.PrintLine($"Player #{player.Name}: {player.Lp} LP");
}
```
