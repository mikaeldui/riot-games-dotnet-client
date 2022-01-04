// See https://aka.ms/new-console-template for more information

Console.WriteLine("Welcome to the RiotGames.Client.CodeGeneration program!");

await RiotGames.Client.CodeGeneration.RiotGamesApi.RiotGamesApiClientsGenerator.GenerateCodeAsync();

//await RiotGames.Client.CodeGeneration.LeagueClient.LeagueClientApiClientGenerator.GenerateCodeAsync();

Console.WriteLine("Program done!");