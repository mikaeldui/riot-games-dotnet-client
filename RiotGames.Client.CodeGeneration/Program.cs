// See https://aka.ms/new-console-template for more information
using MingweiSamuel;
using RiotGames.Client.CodeGeneration;

Console.WriteLine("Welcome to the RiotGames.Client.CodeGeneration program!");

Console.WriteLine("Downloading the Open API specification");

var schema = await RiotApiSchemaClient.GetOpenApiSchemaAsync();

Console.WriteLine($"Downloaded spec file containing {schema?.Paths?.Count} paths and {schema?.Components?.Schemas?.Count} component schemas.");

var pathsGrouping = schema?.Paths?.GroupBy(p => p.Key.Split('/', StringSplitOptions.RemoveEmptyEntries).First());

Console.WriteLine($"Found {pathsGrouping?.Count()} bases");

if (pathsGrouping != null)

    foreach (var group in pathsGrouping)
    {
        switch (group.Key)
        {
            case "riot":
                var cg = new ClientGenerator(Client.RiotGames);
                foreach (var path in group)
                {
                    cg.AddPathAsEndpoints(path);
                }
                Console.WriteLine(cg.GenerateCode());

                break;
            case "lol":
                break;
            case "lor":
                break;
            case "tft":
                break;
            case "val":
                break;
        }

        Console.WriteLine(group.Key);
    }