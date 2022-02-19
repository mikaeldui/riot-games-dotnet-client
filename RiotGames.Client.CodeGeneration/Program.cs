// See https://aka.ms/new-console-template for more information

using RiotGames.Client.CodeGeneration.LeagueClient;
using RiotGames.Client.CodeGeneration.RiotGamesApi;

Console.WriteLine("Welcome to the RiotGames.Client.CodeGeneration program!");

// TODO: Get latest version from https://ddragon.leagueoflegends.com/api/versions.json

// Let this still run on CI..
await RiotApiRunner.GenerateCodeAsync();

await LeagueClientRunner.GenerateCodeAsync();


Console.WriteLine("Program done!");