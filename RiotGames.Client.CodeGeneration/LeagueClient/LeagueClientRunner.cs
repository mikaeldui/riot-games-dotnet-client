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

            _generateModels(schema, out string[] enums);

            _generateEndpoints(schema, enums);

            // TODO: WebSockets
        }

        private static void _generateModels(LcuApiOpenApiSchema schema, out string[] enums)
        {
            // TODO: Maybe group them by module and put them in separate namespaces.

            var generator = new LeagueClientModelsGenerator();
            generator.AddDtos(schema.Components.Schemas);
            enums = generator.GetEnums();
            FileWriter.WriteLeagueClientModelsFile(generator.GenerateCode());
        }

        private static void _generateEndpoints(LcuApiOpenApiSchema schema, string[] enums)
        {
            var groupedPaths = schema.Paths.GroupByModule();

            var generator = new LeagueClientEndpointsGenerator(enums);
            generator.AddGroupsAsNestedClassesWithEndpoints(groupedPaths);
            var code = generator.GenerateCode();
            FileWriter.WriteLeagueClientFile(code, null);
        }

        private static void _console(string message) => Console.WriteLine("League Client: " + message);
    }
}
