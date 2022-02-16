# Riot Games .NET Client (unofficial)
[![Daily Test (@main)](https://github.com/mikaeldui/riot-games-dotnet-client/actions/workflows/daily-test.main.yml/badge.svg)](https://github.com/mikaeldui/riot-games-dotnet-client/actions/workflows/daily-test.main.yml)
[![.NET](https://github.com/mikaeldui/riot-games-dotnet-client/actions/workflows/dotnet.yml/badge.svg)](https://github.com/mikaeldui/riot-games-dotnet-client/actions/workflows/dotnet.yml)
[![CodeQL](https://github.com/mikaeldui/riot-games-dotnet-client/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/mikaeldui/riot-games-dotnet-client/actions/workflows/codeql-analysis.yml)

![image](https://user-images.githubusercontent.com/3706841/147928421-b25b3ddd-c774-4240-bf10-863321b05bb4.png)

An unofficial [.NET][dotnet] Client for [Riot Games][riot] and their games [League of Legends][lol], [Legends of Runeterra][lor], [Teamfight Tactics][tft] and [Valorant][val].

✅ Purpose-built Open API client generator

✅ Vanilla C# -  Code generation done with Roslyn.

✅ Auto-generated overloads for CLR objects.

✅ Continuously tested against the live API.

✅ All tests required to be successful before deploy.

✅ Performance tested - the extra classes have no measurable impact.

✅ Optimized - *you call the `HttpClient` almost directly.*

✅ [Embedded symbols](https://github.com/Turnerj/dotnet-library-checklist#embedding-symbols) and Source link - see the source code while debugging.

✅ [Deterministic build](https://github.com/Turnerj/dotnet-library-checklist#enable-deterministic-builds) - byte-for-byte output is identical across compilations.

❌ POST, PUT, DELETE and queries - for that, use other libraries for now (e.g. for RGAPI, [Camille](https://github.com/MingweiSamuel/Camille), and or LCU, [PoniLCU](https://github.com/Ponita0/PoniLCU)).

❌ Game API - for that, use other libraries for now (e.g. [Camille](https://github.com/MingweiSamuel/Camille)).

# Sponsors
A big thank you for your support!

[![JetBrains Logo (Main) logo](https://resources.jetbrains.com/storage/products/company/brand/logos/jb_beam.svg)](https://jb.gg/OpenSourceSupport)

## Demo

You can see the library running in your browser using Blazor at [masteries.quest](https://masteries.quest), which has a Cloudflare Workers proxy between the browser and Riot Games API.

## Download

You can find the latest releases on [GitHub Packages](https://github.com/mikaeldui/riot-games-dotnet-client/packages/1184018) and on [NuGet.org](https://www.nuget.org/packages/MikaelDui.RiotGames.Client).

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
using LeagueOfLegendsClient client = new("RGAPI-SUPERSECRET", Server.NA);

var summoner = await client.GetSummonerByNameAsync("Some summoner name");
var masteries = await client.GetMasteriesAsync(summoner);
foreach(var mastery in masteries)
    Console.WriteLine($"Champion #{mastery.ChampionId}: {mastery.championPoints} points");

```

### Getting [Legends of Runeterra][lor] [leadersboard](https://developer.riotgames.com/apis#lor-ranked-v1/GET_getLeaderboards)

```C#
using RiotGames.LegendsOfRuneterra;
using LegendsOfRuneterraClient client = new("RGAPI-SUPERSECRET", RegionRoute.AMERICAS);

var leaderboards = await client.GetRankedLeaderboardsAsync();
foreach(var player in leaderboards.Players)
    Console.WriteLine($"Player #{player.Name}: {player.Lp} LP");

```

### Getting [Teamfight Tactics][tft] [league entires](https://developer.riotgames.com/apis#tft-league-v1/GET_getLeagueEntriesForSummoner)

```C#
using RiotGames.TeamfightTactics;
using TeamfightTacticsClient client = new("RGAPI-SUPERSECRET", PlatformRoute.NA1);

var leagueEntries = await client.GetLeagueEntriesAsync("some-summoner-ID");
foreach(var entry in leagueEntries)
    Console.WriteLine($"Player #{entry.SummonerName}: {player.LeaguePoints} LP");

```

### Getting a [Valorant][val] [match](https://developer.riotgames.com/apis#val-match-v1/GET_getMatch)

```C#
using RiotGames.Valorant;
using ValorantClient client = new("RGAPI-SUPERSECRET", ValPlatformRoute.EU);

var match = await client.GetMatchAsync("some-match-ID");
foreach(var player in match.Players)
    Console.WriteLine($"Player #{player.Title} played champion #{player.ChampionId}");

```

### Doing multi-game stuff
Using the [`RiotGamesClient`](https://github.com/mikaeldui/riot-games-dotnet-client/blob/main/RiotGames.Client/RiotGamesClient.cs) class gives you easy access to multiple game clients at once.

```C#
using RiotGames;
using RiotGamesClient client = new("RGAPI-SUPERSECRET", PlatformRoute.NA1, ValPlatformRoute.NA);

var lolSummoner = await client.LeagueOfLegends.GetSummonerByNameAsync("some-summoner-name");
var lolMasteries = await client.LeagueOfLegends.GetMasteriesAsync(summoner);
foreach(var mastery in lolMasteries)
    Console.WriteLine($"Champion #{mastery.ChampionId}: {mastery.championPoints} points");

var lorLeaderboards = await client.LegendsOfRuneterra.GetRankedLeaderboardsAsync();
foreach(var player in lorLeaderboards.Players)
    Console.WriteLine($"Player #{player.Name}: {player.Lp} LP");

```

### Getting the current [League of Legends][lol] champ select
You can use the [`LeagueClient`](https://github.com/mikaeldui/riot-games-dotnet-client/blob/main/RiotGames.Client/LeagueOfLegends/LeagueClient/LeagueClient.cs) to communicate directly with the League Client (aka LCU).

```C#
using RiotGames.LeagueOfLegends.LeagueClient;
using LeagueClient.LeagueOfLegendsClient client = new();

var session = await client.ChampSelect.GetSessionAsync();
foreach(var teamMember in session.MyTeam)
{
    var summoner = await client.Summoners.GetSummonerAsync(teamMember);
    Console.WriteLine($"Team member: {summoner.DisplayName}");
}

```

## Generated Code
The generated code looks like this:

![image](https://user-images.githubusercontent.com/3706841/150069049-e768f7f3-fa19-4eeb-8c2a-353e3e33a578.png)

## Sub-packages
The client includes these sub-packages:
- [MikaelDui.RiotGames.Core](https://github.com/mikaeldui/riot-games-dotnet-core)
- [MikaelDui.RiotGames.LeagueOfLegends.Core](https://github.com/mikaeldui/riot-games-league-of-legends-dotnet-core)
- [MikaelDui.RiotGames.LeagueOfLegends.LeagueClient.LockFile](https://github.com/mikaeldui/riot-games-league-of-legends-league-client-lock-file-dotnet)

## Notice from [Riot Games][riot]

[Riot Games .NET Client (unofficial)][rgdc] isn't endorsed by [Riot Games][riot] and doesn't reflect the views or opinions of [Riot Games][riot] or anyone officially involved in producing or managing [Riot Games][riot] properties. [Riot Games][riot], and all associated properties are trademarks or registered trademarks of [Riot Games, Inc][riot].

[rgdc]: https://github.com/mikaeldui/riot-games-dotnet-client "Riot Games .NET Client (unofficial)"
[riot]: https://www.riotgames.com/ "Riot Games"
[lol]: https://www.leagueoflegends.com/ "League of Legends"
[lor]: https://playruneterra.com/ "Legends of Runeterra"
[tft]: https://teamfighttactics.leagueoflegends.com/ "Teamfight Tactics"
[val]: https://playvalorant.com/ "Valorant"
[dotnet]: https://dotnet.microsoft.com/ ".NET"
