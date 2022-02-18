using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.Client.CodeGeneration
{
    internal enum FileType
    {
        Client,
        Models
    }

    internal static class FileWriter
    {
        public static void WriteFile(RiotGamesApi.Client client, FileType fileType, string contents)
        {
            var folder = Path.Combine(GetAssemblyDirectory(), @"../../../../", "RiotGames.Client");

            if (client != RiotGamesApi.Client.RiotGames)
                folder = Path.Combine(folder, client.ToString());

            File.WriteAllText(Path.Combine(folder, $"{client}{fileType}.g.cs"), contents);
        }

        public static void WriteLeagueClientFile(string contents, string? subClass = null)
        {
            var folder = Path.Combine(GetAssemblyDirectory(), @"../../../../", "RiotGames.LeagueOfLegends.LeagueClient.Client");

            string? suffix = null;
            if (subClass != null)
                suffix = "." + subClass;

            File.WriteAllText(Path.Combine(folder, $"LeagueClient{suffix}.g.cs"), contents);
        }

        public static void WriteLeagueClientModelsFile(string contents)
        {
            var folder = Path.Combine(GetAssemblyDirectory(), @"../../../../", "RiotGames.LeagueOfLegends.LeagueClient.Client");

            File.WriteAllText(Path.Combine(folder, $"LeagueClientModels.g.cs"), contents);
        }

        private static string GetAssemblyDirectory()
        {
            var path = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
            return path ?? throw new Exception("No idea why executing assembly path is null.");
        }
    }
}
