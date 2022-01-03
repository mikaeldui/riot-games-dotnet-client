# Riot Games .NET Client (unofficial)
[![.NET](https://github.com/mikaeldui/riot-games-dotnet-client/actions/workflows/dotnet.yml/badge.svg)](https://github.com/mikaeldui/riot-games-dotnet-client/actions/workflows/dotnet.yml)

![image](https://user-images.githubusercontent.com/3706841/147928421-b25b3ddd-c774-4240-bf10-863321b05bb4.png)


An unofficial .NET Client for Riot Games and their games League of Legends, Legends of Runeterra, Teamfight Tactics and Valorant.

It features a purpose-built Open API client generator and is written with Vanilla C#.

It's easily extendible. You can leverage it's type safety and inherit the clients or just use its GetAsync/PostAsync/PutAsync methods if you need to do something that's not supported.

## Download

You can find the latest alpha releases [here on GitHub](https://github.com/mikaeldui/riot-games-dotnet-client/releases). It'll soon be available on NuGet.

## Examples

### Getting League of Legends masteries

```C#
using RiotGames.LeagueOfLegends;

using (var client = new LeagueOfLegendsClient("ABCD-ABCD-ABCD-ABCD"))
{
    var masteries = await client.GetMasteriesAsync("some-summoner-ID");
    foreach(var mastery in masteries)
        Console.PrintLine($"Champion #{mastery.ChampionId}: {mastery.championPoints} points");
}
```

### Getting Legends of Runeterra leadersboard

```C#
using RiotGames.LegendsOfRuneterra;

using (var client = new LegendsOfRuneterraClient("ABCD-ABCD-ABCD-ABCD"))
{
    var leaderboards = await client.GetRankedLeaderboardsAsync();
    foreach(var player in leaderboards.Players)
        Console.PrintLine($"Player #{player.Name}: {player.Lp} LP");
}
```


### Doing multi-game stuff
Using the `RiotGamesClient` class gives you easy access to multiple game clients at once.

```C#
using RiotGames;

using (var client = new RiotGamesClient("ABCD-ABCD-ABCD-ABCD"))
{
    var lolMasteries = await client.LeagueOfLegends.GetMasteriesAsync("some-summoner-ID");
    foreach(var mastery in lolMasteries)
        Console.PrintLine($"Champion #{mastery.ChampionId}: {mastery.championPoints} points");

    var lorLeaderboards = await client.LegendsOfRuneterra.GetRankedLeaderboardsAsync();
    foreach(var player in lorLeaderboards.Players)
        Console.PrintLine($"Player #{player.Name}: {player.Lp} LP");
}
```
