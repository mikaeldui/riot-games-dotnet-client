using System.Net.Http.Json;

#nullable disable

namespace RiotGames.Client.CodeGeneration.LeagueClient;

internal class LeagueClientHelp
{
    public Dictionary<string, string> Events { get; set; }
    public Dictionary<string, string> Types { get; set; }
    public Dictionary<string, string> Functions { get; set; }

    public static async Task<LeagueClientHelp> DownloadAsync()
    {
        using var client = new HttpClient();
        var schema =
            await client.GetFromJsonAsync<LeagueClientHelp>("https://www.mingweisamuel.com/lcu-schema/lcu/help.brief.json");
        return schema ?? throw new Exception("We didn't get any help.json from the server!");
    }
}

#nullable restore
