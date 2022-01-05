// See https://aka.ms/new-console-template for more information

using System.Diagnostics;

Console.WriteLine("Welcome to the RiotGames.Client.CodeGeneration program!");

if (!Debugger.IsAttached)
{
    // Let this still run on CI..
    await RiotGames.Client.CodeGeneration.RiotGamesApi.RiotApiRunner.GenerateCodeAsync();
}
else
{
    await RiotGames.Client.CodeGeneration.LeagueClient.LeagueClientRunner.GenerateCodeAsync();
}


Console.WriteLine("Program done!");