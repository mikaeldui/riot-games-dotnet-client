using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.LeagueOfLegends.LeagueClient
{
    internal class LeagueClientLockfile
    {
        internal const string LEAGUECLIENT_DEFAULT_LOCKFILE_PATH = @"C:\Riot Games\League of Legends\lockfile";
        internal const string LEAGUECLIENT_DEFAULT_PROCESS_NAME = "LeagueClient";

        private LeagueClientLockfile(string processName, ulong processId, ushort port, string password, string protocol)
        {
            ProcessName = processName;
            ProcessId = processId;
            Port = port;
            Password = password;
            Protocol = protocol;
        }

        public string ProcessName { get; }
        public ulong ProcessId { get; }
        public ushort Port { get; }
        public string Password { get; }
        public string Protocol { get; }

        public static LeagueClientLockfile FromPath(string path = LEAGUECLIENT_DEFAULT_LOCKFILE_PATH)
        {
            var content = _fileReadAllText(path);
            var splitContent = content.Split(':');
            return new LeagueClientLockfile(splitContent[0], UInt64.Parse(splitContent[1]), UInt16.Parse(splitContent[2]), splitContent[3], splitContent[4]);
        }

        private static string _fileReadAllText(string path)
        {
            string text;
            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }
            return text;
        }

        public static LeagueClientLockfile FromProcess(string processName = LEAGUECLIENT_DEFAULT_PROCESS_NAME)
        {
            var process = Process.GetProcessesByName(processName).Single();

            if (process.MainModule == null)
                throw new ArgumentException($"The process with the name of {processName} doesn't have any main module.");

            var processDirectory = Path.GetDirectoryName(process.MainModule.FileName);

            if (processDirectory == null)
                throw new ArgumentException($"Unable to find a directory for the main module of the process {processName}.");

            return FromPath(Path.Combine(processDirectory, "lockfile"));
        }
    }
}
