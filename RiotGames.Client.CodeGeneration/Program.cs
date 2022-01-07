// See https://aka.ms/new-console-template for more information

using System.Diagnostics;

Console.WriteLine("Welcome to the RiotGames.Client.CodeGeneration program!");

// TODO: Get latest version from https://ddragon.leagueoflegends.com/api/versions.json

await RiotGames.Client.CodeGeneration.RiotGamesApi.RiotApiRunner.GenerateCodeAsync();

await RiotGames.Client.CodeGeneration.LeagueClient.LeagueClientRunner.GenerateCodeAsync();

Console.WriteLine("Program done!");
