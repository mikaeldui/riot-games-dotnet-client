// See https://aka.ms/new-console-template for more information
using MingweiSamuel;

Console.WriteLine("Welcome to the RiotGames.Client.CodeGeneration program!");

Console.WriteLine("Downloading the Open API specification");

var schema = await RiotApiSchemaClient.GetOpenApiSchemaAsync();

Console.WriteLine($"Downloaded spec file containing {schema?.Paths?.Count} paths and {schema?.Components?.Schemas?.Count} component schemas.");