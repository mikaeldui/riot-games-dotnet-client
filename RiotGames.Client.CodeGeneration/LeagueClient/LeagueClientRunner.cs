using MingweiSamuel.Lcu;
using RiotGames.Client.CodeGeneration.LeagueClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.Client.CodeGeneration.LeagueClient
{
    internal static class LeagueClientRunner
    {
        public static async Task GenerateCodeAsync()
        {
            // TODO: Download certificate.

            _console("League Client: Downloading the Open API specification.");

            var schema = await MingweiSamuel.MingweiSamuelSchemasClient.GetLcuOpenApiSchemaAsync();

            _console($"League Client: Downloaded spec file containing {schema.Paths?.Count} paths and {schema.Components?.Schemas?.Count} component schemas.");

            _generateModels(schema, out var enums);

            var help = await LeagueClientHelp.DownloadAsync();

            _console($"Downloaded help.json containing {help.Events.Count} events, {help.Types.Count} types and {help.Functions.Count} functions.");

            _generateEndpoints(schema, enums, help.Events);

            // TODO: WebSockets
        }

        private static void _generateModels(LcuApiOpenApiSchema schema, out string[] enums)
        {
            // TODO: Maybe group them by module and put them in separate namespaces.

            var generator = new LeagueClientModelsGenerator();
            generator.AddDtos(schema?.Components?.Schemas ?? throw new InvalidOperationException());
            enums = generator.GetEnums();
            FileWriter.WriteLeagueClientModelsFile(generator.GenerateCode());
        }

        private static void _generateEndpoints(LcuApiOpenApiSchema schema, string[] enums, Dictionary<string, string> events)
        {
            var groupedPaths = (schema.Paths ?? throw new InvalidOperationException()).GroupByModule();
            var groupedEvents = events.GroupByModule();

            var generator = new LeagueClientEndpointsGenerator(enums);
            generator.AddGroupsAsNestedClassesWithEndpoints(groupedPaths, groupedEvents);
            var code = generator.GenerateCode();
            FileWriter.WriteLeagueClientFile(code, null);
        }

        private static void _console(string message) => Console.WriteLine("League Client: " + message);
    }
}
