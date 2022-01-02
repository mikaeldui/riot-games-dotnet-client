using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;

namespace RiotGames
{
    internal static class HttpExtensions
    {
        public static async Task<TResult?> PostAsJsonAsync<TValue, TResult>(this HttpClient httpClient, string? requestUri, TValue value)
        {
            var response = await httpClient.PostAsJsonAsync(requestUri, value);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResult>();
        }

        public static async Task<TResult?> PutAsJsonAsync<TValue, TResult>(this HttpClient httpClient, string? requestUri, TValue value)
        {
            var response = await httpClient.PutAsJsonAsync(requestUri, value);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResult>();
        }
    }
}
