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
        ClientGenerator cg;
        switch (group.Key)
        {
            case "riot":
                cg = new ClientGenerator(Client.RiotGames);
                foreach (var path in group)
                {
                    cg.AddPathAsEndpoints(path);
                }
                Console.WriteLine(cg.GenerateCode());
                break;
            case "lol":
                cg = new ClientGenerator(Client.LeagueOfLegends);
                foreach (var path in group)
                {
                    cg.AddPathAsEndpoints(path);
                }
                Console.WriteLine(cg.GenerateCode());
                break;
            case "lor":
                cg = new ClientGenerator(Client.LegendsOfRuneterra);
                foreach (var path in group)
                {
                    cg.AddPathAsEndpoints(path);
                }
                Console.WriteLine(cg.GenerateCode());
                break;
            case "tft":
                cg = new ClientGenerator(Client.TeamfightTactics);
                foreach (var path in group)
                {
                    cg.AddPathAsEndpoints(path);
                }
                Console.WriteLine(cg.GenerateCode());
                break;
            case "val":
                cg = new ClientGenerator(Client.Valorant);
                foreach (var path in group)
                {
                    cg.AddPathAsEndpoints(path);
                }
                Console.WriteLine(cg.GenerateCode());
                break;
        }

        Console.WriteLine(group.Key);
    }