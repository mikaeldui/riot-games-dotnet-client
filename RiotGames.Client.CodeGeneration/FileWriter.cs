﻿using System;
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
            string folder = Path.Combine(GetAssemblyDirectory(), @"../../../../", "RiotGames.Client");

            if (client != Client.RiotGames)
                folder = Path.Combine(folder, client.ToString());

            File.WriteAllText(Path.Combine(folder, $"{client}{fileType}.g.cs"), contents);
        }

        private static string GetAssemblyDirectory()
        {
            var path = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
            if (path == null)
                throw new Exception("No idea why executing assembly path is null.");
            return path;
        }
    }
}
