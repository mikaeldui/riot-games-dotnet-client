## Legends of Runeterra .NET Client (unofficial)

Part of `MikaelDui.RiotGames.Client`.

### Installation

Use the following command to install it:

    dotnet add MikaelDui.RiotGames.Client --version *
    
### Example

    using RiotGames.LegendsOfRuneterra;
    using LegendsOfRuneterraClient client = new("ABCD-ABCD-ABCD-ABCD", RegionRoute.AMERICAS);

    var leaderboards = await client.GetRankedLeaderboardsAsync();
    foreach(var player in leaderboards.Players)
      Console.PrintLine($"Player #{player.Name}: {player.Lp} LP");
