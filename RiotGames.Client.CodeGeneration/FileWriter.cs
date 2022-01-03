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
        public static void WriteFile(Client client, FileType fileType, string contents)
        {
            string folder = Path.Combine(GetAssemblyDirectory(), @"..\..\..\..\", "RiotGames.Client");

            if (client != Client.RiotGames)
                folder = Path.Combine(folder, client.ToString());

            File.WriteAllText(Path.Combine(folder, $"{client}{fileType}.g.cs"), contents);
        }

        private static string GetAssemblyDirectory()
        {
            string codeBase = Assembly.GetExecutingAssembly().Location;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }
    }
}
