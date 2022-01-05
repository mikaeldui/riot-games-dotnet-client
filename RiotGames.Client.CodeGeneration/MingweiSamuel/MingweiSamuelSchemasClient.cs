using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MingweiSamuel.RiotApi;
using MingweiSamuel.Lcu;

namespace MingweiSamuel
{
    internal static class MingweiSamuelSchemasClient
    {
        public static async Task<RiotApiOpenApiSchema> GetRiotOpenApiSchemaAsync(string version = "3.0.0") =>
            await _getOpenApiSchemaAsync<RiotApiOpenApiSchema>($"http://www.mingweisamuel.com/riotapi-schema/openapi-{version}.json");

        public static async Task<LcuApiOpenApiSchema> GetLcuOpenApiSchemaAsync() =>
            await _getOpenApiSchemaAsync<LcuApiOpenApiSchema>("https://www.mingweisamuel.com/lcu-schema/lcu/openapi.json");

        private static async Task<T> _getOpenApiSchemaAsync<T>(string uri)
        {
            using (var client = new HttpClient())
            {
                var schema = await client.GetFromJsonAsync<T>(uri);

                if (schema == null)
                    throw new Exception("We didn't get any schema from the server!");

                return schema;
            }
        }
    }
}
