# Riot Games .NET Client (unofficial)
[![.NET](https://github.com/mikaeldui/riot-games-dotnet-client/actions/workflows/dotnet.yml/badge.svg)](https://github.com/mikaeldui/riot-games-dotnet-client/actions/workflows/dotnet.yml)

![image](https://user-images.githubusercontent.com/3706841/147928421-b25b3ddd-c774-4240-bf10-863321b05bb4.png)

An unofficial [.NET][dotnet] Client for [Riot Games][riot] and their games [League of Legends][lol], [Legends of Runeterra][lor], [Teamfight Tactics][tft] and [Valorant][val].

It features a purpose-built Open API client generator and is written with Vanilla C#.

It's easily extendible. You can leverage its type safety and inherit the clients or just use its GetAsync/PostAsync/PutAsync methods if you need to do something that's not supported.

## Download

You can find the latest alpha releases [here on GitHub](https://github.com/mikaeldui/riot-games-dotnet-client/releases). It'll soon be available on NuGet.

## Examples

### Getting [League of Legends][lol] masteries

```C#
using RiotGames.LeagueOfLegends;

using (var client = new LeagueOfLegendsClient("ABCD-ABCD-ABCD-ABCD"))
{
    var masteries = await client.GetMasteriesAsync("some-summoner-ID");
    foreach(var mastery in masteries)
        Console.PrintLine($"Champion #{mastery.ChampionId}: {mastery.championPoints} points");
}
```

### Getting [Legends of Runeterra][lor] leadersboard

```C#
using RiotGames.LegendsOfRuneterra;

using (var client = new LegendsOfRuneterraClient("ABCD-ABCD-ABCD-ABCD"))
{
    var leaderboards = await client.GetRankedLeaderboardsAsync();
    foreach(var player in leaderboards.Players)
        Console.PrintLine($"Player #{player.Name}: {player.Lp} LP");
}
```

### Getting [Teamfight Tactics][tft] league entires

```C#
using RiotGames.TeamfightTactics;

using (var client = new TeamfightTacticsClient("ABCD-ABCD-ABCD-ABCD"))
{
    var leagueEntries = await client.GetLeagueEntiresAsync("some-summoner-ID");
    foreach(var entry in leagueEntries)
        Console.PrintLine($"Player #{entry.SummonerName}: {player.LeaguePoints} LP");
}
```

### Getting a [Valorant][val] match

```C#
using RiotGames.Valorant;

using (var client = new ValorantClient("ABCD-ABCD-ABCD-ABCD"))
{
    var match = await client.GetMatchAsync("some-match-ID");
    foreach(var player in match.Players)
        Console.PrintLine($"Player #{player.Title} played champion #{player.ChampionId}");
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

## Notice from [Riot Games][riot]

[Riot Games .NET Client (unofficial)][rgdc] isn't endorsed by [Riot Games][riot] and doesn't reflect the views or opinions of [Riot Games][riot] or anyone officially involved in producing or managing [Riot Games][riot] properties. [Riot Games][riot], and all associated properties are trademarks or registered trademarks of [Riot Games, Inc][riot].

[rgdc]: https://github.com/mikaeldui/riot-games-dotnet-client "Riot Games .NET Client (unofficial)"
[riot]: https://www.riotgames.com/ "Riot Games"
[lol]: https://www.leagueoflegends.com/ "League of Legends"
[lor]: https://playruneterra.com/ "Legends of Runeterra"
[tft]: https://teamfighttactics.leagueoflegends.com/ "Teamfight Tactics"
[val]: https://playvalorant.com/ "Valorant"
[dotnet]: https://dotnet.microsoft.com/ ".NET"
