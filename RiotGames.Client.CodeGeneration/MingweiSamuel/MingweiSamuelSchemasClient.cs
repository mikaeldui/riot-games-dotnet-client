using System.Net.Http.Json;
using MingweiSamuel.Lcu;
using MingweiSamuel.RiotApi;

namespace MingweiSamuel;

internal static class MingweiSamuelSchemasClient
{
    public static async Task<RiotApiOpenApiSchema> GetRiotOpenApiSchemaAsync(string version = "3.0.0")
    {
        return await _getOpenApiSchemaAsync<RiotApiOpenApiSchema>(
            $"http://www.mingweisamuel.com/riotapi-schema/openapi-{version}.json");
    }

    public static async Task<LcuApiOpenApiSchema> GetLcuOpenApiSchemaAsync()
    {
        return await _getOpenApiSchemaAsync<LcuApiOpenApiSchema>(
            "https://www.mingweisamuel.com/lcu-schema/lcu/openapi.json");
    }

    private static async Task<T> _getOpenApiSchemaAsync<T>(string uri)
    {
        using var client = new HttpClient();
        var schema = await client.GetFromJsonAsync<T>(uri);
        return schema ?? throw new Exception("We didn't get any schema from the server!");
    }
}