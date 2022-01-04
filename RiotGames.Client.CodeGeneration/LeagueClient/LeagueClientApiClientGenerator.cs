using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.Client.CodeGeneration.LeagueClient
{
    internal static class LeagueClientApiClientGenerator
    {
        public static async Task GenerateCodeAsync()
        {
            // TODO: Download certificate.

            Console.WriteLine("Getting API specification from LCU");

            var lockfile = LeagueOfLegends.LeagueClient.LeagueClientLockfile.FromProcess();
            using (var client = new LeagueOfLegends.LeagueClient.LeagueClientHttpClient("riot", lockfile.Password, lockfile.Port))
            {
                var openApiSpec = await client.GetStringAsync("/swagger/v1/api-docs");

                Debugger.Break();
            }
        }
    }
}
