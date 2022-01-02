using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace MingweiSamuel
{
    internal static class RiotApiSchemaClient
    {
        public static async Task<RiotApiOpenApiSchema?> GetOpenApiSchemaAsync(string version = "3.0.0")
        {
            using (var client = new HttpClient())
                return await client.GetFromJsonAsync<RiotApiOpenApiSchema>($"http://www.mingweisamuel.com/riotapi-schema/openapi-{version}.json");
        }
    }
}
