using MingweiSamuel;
using Schema = System.Collections.Generic.KeyValuePair<string, MingweiSamuel.RiotApi.RiotApiComponentSchemaObject>;


namespace RiotGames.Client.CodeGeneration.RiotGamesApi;

internal static class RiotApiRunner
{
    public static async Task GenerateCodeAsync()
    {
        Console.WriteLine("Downloading the Open API specification");

        var schema = await MingweiSamuelSchemasClient.GetRiotOpenApiSchemaAsync();

        Console.WriteLine(
            $"Downloaded spec file containing {schema.Paths?.Count} paths and {schema.Components?.Schemas?.Count} component schemas.");

        var pathsGrouping = schema.Paths?.GroupByGame();

        Console.WriteLine($"Found {pathsGrouping?.Count()} path bases");

        if (pathsGrouping != null)

            foreach (var group in pathsGrouping)
            {
                RiotApiEndpointsGenerator cg;
                switch (group.Key)
                {
                    case "riot":
                        cg = new RiotApiEndpointsGenerator(Client.RiotGames);
                        cg.AddPathsAsEndpoints(group);
                        FileWriter.WriteFile(Client.RiotGames, FileType.Client, cg.GenerateCode());
                        break;
                    case "lol":
                        cg = new RiotApiEndpointsGenerator(Client.LeagueOfLegends);
                        cg.AddPathsAsEndpoints(group);
                        FileWriter.WriteFile(Client.LeagueOfLegends, FileType.Client, cg.GenerateCode());
                        break;
                    case "lor":
                        cg = new RiotApiEndpointsGenerator(Client.LegendsOfRuneterra);
                        cg.AddPathsAsEndpoints(group);
                        FileWriter.WriteFile(Client.LegendsOfRuneterra, FileType.Client, cg.GenerateCode());
                        break;
                    case "tft":
                        cg = new RiotApiEndpointsGenerator(Client.TeamfightTactics);
                        cg.AddPathsAsEndpoints(group);
                        FileWriter.WriteFile(Client.TeamfightTactics, FileType.Client, cg.GenerateCode());
                        break;
                    case "val":
                        cg = new RiotApiEndpointsGenerator(Client.Valorant);
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
            var current = s;
            while (true)
            {
                var game = pathsWithSchema.WhereReferencesSchema(current).FirstOrDefault().GetGame();

                if (game != null)
                    return game;

                var parents = schema.Components.Schemas!.WhereReferencesSchema(current);

                if (!parents.Any())
                    return null;

                current = parents.FirstOrDefault();
            }
        });

        if (groupedSchemas == null)
        {
            Console.WriteLine("Couldn't group schemas!");
        }
        else
        {
            Console.WriteLine($"Found {groupedSchemas.Count()} component bases");

            foreach (var group in groupedSchemas)
            {
                RiotApiModelGenerator dg;
                switch (group.Key)
                {
                    case "riot":
                        dg = new RiotApiModelGenerator(Client.RiotGames);
                        dg.AddDtos(group);
                        FileWriter.WriteFile(Client.RiotGames, FileType.Models, dg.GenerateCode());
                        break;
                    case "lol":
                        dg = new RiotApiModelGenerator(Client.LeagueOfLegends);
                        dg.AddDtos(group);
                        FileWriter.WriteFile(Client.LeagueOfLegends, FileType.Models, dg.GenerateCode());
                        break;
                    case "lor":
                        dg = new RiotApiModelGenerator(Client.LegendsOfRuneterra);
                        dg.AddDtos(group);
                        FileWriter.WriteFile(Client.LegendsOfRuneterra, FileType.Models, dg.GenerateCode());
                        break;
                    case "tft":
                        dg = new RiotApiModelGenerator(Client.TeamfightTactics);
                        dg.AddDtos(group);
                        FileWriter.WriteFile(Client.TeamfightTactics, FileType.Models, dg.GenerateCode());
                        break;
                    case "val":
                        dg = new RiotApiModelGenerator(Client.Valorant);
                        dg.AddDtos(group);
                        FileWriter.WriteFile(Client.Valorant, FileType.Models, dg.GenerateCode());
                        break;
                }

                Console.WriteLine(group.Key);
            }
        }
    }
}