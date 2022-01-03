# Riot Games .NET Client
An unofficial .NET Client for Riot Games and their games League of Legends, Legends of Runeterra, Teamfight Tactics and Valorant.

It features a purpose-built Open API client generator and is written with Vanilla C#.

It's easily extendible. You can leverage it's type safety and inherit the clients or just use it GetAsync/PostAsync/PutAsync method if you need to do something that's not supported.

## Download

You can find the latest alpha releases under [here on GitHub](https://github.com/mikaeldui/riot-games-dotnet-client/releases). It'll soon be available on NuGet.

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
using RiotGames.LegendsOfRuneterra;

using (var client = new LegendsOfRuneterraClient("ABCD-ABCD-ABCD-ABCD"))
{
    var leaderboards = await client.GetRankedLeaderboardsAsync();
    foreach(var player in leaderboards.Players)
        Console.PrintLine($"Player #{player.Name}: {player.Lp} LP");
}
```
