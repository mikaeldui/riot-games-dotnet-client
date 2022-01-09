# Riot Games .NET Client (unofficial)
[![.NET](https://github.com/mikaeldui/riot-games-dotnet-client/actions/workflows/dotnet.yml/badge.svg)](https://github.com/mikaeldui/riot-games-dotnet-client/actions/workflows/dotnet.yml)
[![CodeQL](https://github.com/mikaeldui/riot-games-dotnet-client/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/mikaeldui/riot-games-dotnet-client/actions/workflows/codeql-analysis.yml)

![image](https://user-images.githubusercontent.com/3706841/147928421-b25b3ddd-c774-4240-bf10-863321b05bb4.png)

An unofficial [.NET][dotnet] Client for [Riot Games][riot] and their games [League of Legends][lol], [Legends of Runeterra][lor], [Teamfight Tactics][tft] and [Valorant][val].

It features a purpose-built Open API client generator and is written with Vanilla C#.

There are auto-generated overloads so that you can pass a CLR object as parameter to the clients.

## Download

You can find the latest releases [here on GitHub](https://github.com/mikaeldui/riot-games-dotnet-client/releases) and [on NuGet](https://www.nuget.org/packages/MikaelDui.RiotGames.Client).

To install the latest version of the package, type the following in the **package manager** console:

    Install-Package MikaelDui.RiotGames.Client
        
Or use the **.NET CLI** reference the **latest stable** and stay **up-to-date**:

    dotnet add package MikaelDui.RiotGames.Client --version *
    
You can also use a **PackageReference** to stay **up-to-date** with the **latest stable** build:

    <PackageReference Include="MikaelDui.RiotGames.Client" Version="*" />

## Examples

### Getting [League of Legends][lol] [masteries](https://developer.riotgames.com/apis#champion-mastery-v4/GET_getChampionMasteryScore)

```C#
using RiotGames.LeagueOfLegends;
using LeagueOfLegends client = new("ABCD-ABCD-ABCD-ABCD", PlatformRoute.NA1);

var summoner = await client.GetSummonerByNameAsync("Some summoner name");
var masteries = await client.GetMasteriesAsync(summoner);
foreach(var mastery in masteries)
    Console.PrintLine($"Champion #{mastery.ChampionId}: {mastery.championPoints} points");

```

### Getting [Legends of Runeterra][lor] [leadersboard](https://developer.riotgames.com/apis#lor-ranked-v1/GET_getLeaderboards)

```C#
using RiotGames.LegendsOfRuneterra;
using LegendsOfRuneterraClient client = new("ABCD-ABCD-ABCD-ABCD", RegionRoute.AMERICAS);

var leaderboards = await client.GetRankedLeaderboardsAsync();
foreach(var player in leaderboards.Players)
    Console.PrintLine($"Player #{player.Name}: {player.Lp} LP");

```

### Getting [Teamfight Tactics][tft] [league entires](https://developer.riotgames.com/apis#tft-league-v1/GET_getLeagueEntriesForSummoner)

```C#
using RiotGames.TeamfightTactics;
using TeamfightTacticsClient client = new("ABCD-ABCD-ABCD-ABCD", PlatformRoute.NA1);

var leagueEntries = await client.GetLeagueEntiresAsync("some-summoner-ID");
foreach(var entry in leagueEntries)
    Console.PrintLine($"Player #{entry.SummonerName}: {player.LeaguePoints} LP");

```

### Getting a [Valorant][val] [match](https://developer.riotgames.com/apis#val-match-v1/GET_getMatch)

```C#
using RiotGames.Valorant;
using ValorantClient client = new("ABCD-ABCD-ABCD-ABCD", ValPlatformRoute.EU);

var match = await client.GetMatchAsync("some-match-ID");
foreach(var player in match.Players)
    Console.PrintLine($"Player #{player.Title} played champion #{player.ChampionId}");

```

### Doing multi-game stuff
Using the [`RiotGamesClient`](https://github.com/mikaeldui/riot-games-dotnet-client/blob/main/RiotGames.Client/RiotGamesClient.cs) class gives you easy access to multiple game clients at once.

```C#
using RiotGames;
using RiotGamesClient client = new("ABCD-ABCD-ABCD-ABCD", PlatformRoute.NA1, ValPlatformRoute.NA);

var lolSummoner = await client.LeagueOfLegends.GetSummonerByNameAsync("some-summoner-name");
var lolMasteries = await client.LeagueOfLegends.GetMasteriesAsync(summoner);
foreach(var mastery in lolMasteries)
    Console.PrintLine($"Champion #{mastery.ChampionId}: {mastery.championPoints} points");

var lorLeaderboards = await client.LegendsOfRuneterra.GetRankedLeaderboardsAsync();
foreach(var player in lorLeaderboards.Players)
    Console.PrintLine($"Player #{player.Name}: {player.Lp} LP");

```

### Getting the current League of Legends champ select
You can use the [`LeagueClient`](https://github.com/mikaeldui/riot-games-dotnet-client/blob/main/RiotGames.Client/LeagueOfLegends/LeagueClient/LeagueClient.cs) to communicate directly with the League Client (aka LCU).

```C#
using RiotGames.LeagueOfLegends.LeagueClient;
using LeagueClient.LeagueOfLegendsClient client = new();

var session = await client.ChampSelect.GetSessionAsync();
foreach(var teamMember in session.MyTeam)
{
    var summoner = await client.Summoners.GetSummonerAsync(teamMember);
    Console.PrintLine($"Team member: {summoner.DisplayName}");
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
