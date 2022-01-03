// See https://aka.ms/new-console-template for more information
using MingweiSamuel;
using RiotGames.Client.CodeGeneration;
using Schema = System.Collections.Generic.KeyValuePair<string, MingweiSamuel.RiotApiOpenApiSchema.ComponentsObject.SchemaObject>;


Console.WriteLine("Welcome to the RiotGames.Client.CodeGeneration program!");

Console.WriteLine("Downloading the Open API specification");

var schema = await RiotApiSchemaClient.GetOpenApiSchemaAsync();

Console.WriteLine($"Downloaded spec file containing {schema.Paths?.Count} paths and {schema.Components?.Schemas?.Count} component schemas.");

var pathsGrouping = schema.Paths?.GroupByGame();

Console.WriteLine($"Found {pathsGrouping?.Count()} path bases");

if (pathsGrouping != null)

    foreach (var group in pathsGrouping)
    {
        ClientGenerator cg;
        switch (group.Key)
        {
            case "riot":
                cg = new ClientGenerator(Client.RiotGames);
                cg.AddPathsAsEndpoints(group);
                FileWriter.WriteFile(Client.RiotGames, FileType.Client, cg.GenerateCode());
                break;
            case "lol":
                cg = new ClientGenerator(Client.LeagueOfLegends);
                cg.AddPathsAsEndpoints(group);
                FileWriter.WriteFile(Client.LeagueOfLegends, FileType.Client, cg.GenerateCode());
                break;
            case "lor":
                cg = new ClientGenerator(Client.LegendsOfRuneterra);
                cg.AddPathsAsEndpoints(group);
                FileWriter.WriteFile(Client.LegendsOfRuneterra, FileType.Client, cg.GenerateCode());
                break;
            case "tft":
                cg = new ClientGenerator(Client.TeamfightTactics);
                cg.AddPathsAsEndpoints(group);
                FileWriter.WriteFile(Client.TeamfightTactics, FileType.Client, cg.GenerateCode());
                break;
            case "val":
                cg = new ClientGenerator(Client.Valorant);
                cg.AddPathsAsEndpoints(group);
                FileWriter.WriteFile(Client.Valorant, FileType.Client, cg.GenerateCode());
                break;
        }

        Console.WriteLine(group.Key);
    }

var pathsWithSchema = schema.Paths.WhereReferenceNotNull().ToArray();

var groupedSchemas = schema.Components?.Schemas.GroupBy(s =>
{
    // We sometimes have to find the parents parent etc.
    Schema current = s;
    while (true)
    {
        var game = pathsWithSchema.WhereReferencesSchema(current).FirstOrDefault().GetGame();

        if (game != null)
            return game;

        var parents = schema.Components.Schemas.WhereReferencesSchema(current);

        if (!parents.Any())
            return null;

        current = parents.FirstOrDefault();
    }
});

if (groupedSchemas == null)
    Console.WriteLine("Couldn't group schemas!");
else
{
    Console.WriteLine($"Found {groupedSchemas.Count()} component bases");

    foreach (var group in groupedSchemas)
    {
        ModelGenerator dg;
        switch (group.Key)
        {
            case "riot":
                dg = new ModelGenerator(Client.RiotGames);
                dg.AddDtos(group);
                FileWriter.WriteFile(Client.RiotGames, FileType.Models, dg.GenerateCode());
                break;
            case "lol":
                dg = new ModelGenerator(Client.LeagueOfLegends);
                dg.AddDtos(group);
                FileWriter.WriteFile(Client.LeagueOfLegends, FileType.Models, dg.GenerateCode());
                break;
            case "lor":
                dg = new ModelGenerator(Client.LegendsOfRuneterra);
                dg.AddDtos(group);
                FileWriter.WriteFile(Client.LegendsOfRuneterra, FileType.Models, dg.GenerateCode());
                break;
            case "tft":
                dg = new ModelGenerator(Client.TeamfightTactics);
                dg.AddDtos(group);
                FileWriter.WriteFile(Client.TeamfightTactics, FileType.Models, dg.GenerateCode());
                break;
            case "val":
                dg = new ModelGenerator(Client.Valorant);
                dg.AddDtos(group);
                FileWriter.WriteFile(Client.Valorant, FileType.Models, dg.GenerateCode());
                break;
        }

        Console.WriteLine(group.Key);
    }
}