using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

#nullable disable 

namespace RiotGames.Client.CodeGeneration.LeagueClient
{
    internal class LeagueClientHelp
    {
        public Dictionary<string, string> Events { get; set; }
        public Dictionary<string, string> Types { get; set; }
        public Dictionary<string, string> Functions { get; set; }

        public static async Task<LeagueClientHelp> DownloadAsync()
        {
            using var client = new HttpClient();
            var schema = await client.GetFromJsonAsync<LeagueClientHelp>("https://www.mingweisamuel.com/lcu-schema/lcu/help.json");
            return schema ?? throw new Exception("We didn't get any help.json from the server!");
        }
    }
}

#nullable restore