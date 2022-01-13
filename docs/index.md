## Riot Games .NET Client (unofficial

An unofficial .NET client for Riot Games and their games League of Legends, Legends of Runeterra, Teamfight Tactics and Valorant.

### Example usage

```markdown
using RiotGames.LeagueOfLegends;
using LeagueOfLegendsClient client = new("my-secret-key", PlatformRoute.NA1);

var summoner = client.Summoners.GetSummonerByName("Some carry");
Console.WriteLine(summoner.Name);
```
