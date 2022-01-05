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

            var groupedPaths = schema.Paths.GroupByModule();

            var generator = new LeagueClientPathsGenerator();

            generator.AddGroupsAsNestedClassesWithEndpoints(groupedPaths);
            var code = generator.GenerateCode();
            FileWriter.WriteLeagueClientFile(code, null);

            //Console.WriteLine("Getting API specification from LCU");

            //var lockfile = LeagueOfLegends.LeagueClient.LeagueClientLockfile.FromProcess();
            //using (var client = new LeagueOfLegends.LeagueClient.LeagueClientHttpClient("riot", lockfile.Password, lockfile.Port))
            //{
            //    var openApiSpec = await client.GetStringAsync("/swagger/v1/api-docs");

            //    Debugger.Break();
            //}
        }

        private static void _console(string message) => Console.WriteLine("League Client: " + message);
    }
}
