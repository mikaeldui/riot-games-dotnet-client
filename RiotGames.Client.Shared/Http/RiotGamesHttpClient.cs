using RiotGames.LeagueOfLegends.LeagueClient;
using System.Net.Http.Json;
using System.Reflection;

namespace RiotGames
{
    internal class RiotGamesHttpClient<TObjectBase> : IDisposable
    {
        private static readonly string? VERSION = typeof(RiotGamesHttpClient<>).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;

        protected HttpClient HttpClient;

        internal protected RiotGamesHttpClient()
        {
            HttpClient = new HttpClient();
            HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", $"MikaelDui.RiotGames.Client/{VERSION} (https://github.com/mikaeldui/riot-games-dotnet-client)");
        }

        internal protected RiotGamesHttpClient(HttpClientHandler httpClientHandler)
        {
            HttpClient = new HttpClient(httpClientHandler);
            HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", $"MikaelDui.RiotGames.Client/{VERSION} (https://github.com/mikaeldui/riot-games-dotnet-client)");
        }

        internal async Task<TResult?> GetAsync<TResult>(string? requestUri)
            where TResult : TObjectBase =>
            await HttpClient.GetFromJsonAsync<TResult>(requestUri);

        internal async Task<TResult[]?> GetArrayAsync<TResult>(string? requestUri)
            where TResult : TObjectBase =>
            await HttpClient.GetFromJsonAsync<TResult[]>(requestUri);

        internal async Task<string> GetStringAsync(string? requestUri) =>
            await HttpClient.GetStringAsync(requestUri);

        internal async Task<int> GetIntAsync(string? requestUri) =>
            int.Parse(await GetStringAsync(requestUri));

        internal async Task<string[]?> GetStringArrayAsync(string? requestUri) =>
            await HttpClient.GetFromJsonAsync<string[]>(requestUri);

        internal async Task<TResult?> PostAsync<TValue, TResult>(string? requestUri, TValue value)
            where TValue : TObjectBase
            where TResult : TObjectBase =>
            await HttpClient.PostAsJsonAsync<TValue, TResult>(requestUri, value);

        internal async Task<TResult?> PutAsync<TValue, TResult>(string? requestUri, TValue value)
            where TValue : TObjectBase
            where TResult : TObjectBase =>
            await HttpClient.PutAsJsonAsync<TValue, TResult>(requestUri, value);

        public void Dispose() => HttpClient.Dispose();
    }
}