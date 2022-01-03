using System.Net.Http.Json;
using System.Reflection;
using Camille.Enums;

namespace RiotGames
{
    internal sealed class RiotGamesHttpClient<TObjectBase> : IDisposable
    {
        private static readonly string? VERSION = typeof(RiotGamesClientBase<>).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;

        private HttpClient _httpClient;

        private RiotGamesHttpClient(string apiKey, string apiSubDomain)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", $"MikaelDui.RiotGames.Client/{VERSION} (https://github.com/mikaeldui/riot-games-dotnet-client)");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Riot-Token", apiKey);
            _httpClient.BaseAddress = new Uri($"https://{apiSubDomain}.api.riotgames.com");
        }

        internal RiotGamesHttpClient(string apiKey, RegionalRoute region) : this(apiKey, region.ToString().ToLower())
        {

        }

        internal RiotGamesHttpClient(string apiKey, PlatformRoute platform) : this(apiKey, platform.ToString().ToLower())
        {

        }

        internal RiotGamesHttpClient(string apiKey, ValPlatformRoute platform) : this(apiKey, platform.ToString().ToLower())
        {

        }

        internal async Task<TResult?> GetAsync<TResult>(string? requestUri)
            where TResult : TObjectBase =>
            await _httpClient.GetFromJsonAsync<TResult>(requestUri);

        internal async Task<TResult[]?> GetArrayAsync<TResult>(string? requestUri)
            where TResult : TObjectBase =>
            await _httpClient.GetFromJsonAsync<TResult[]>(requestUri);

        internal async Task<string> GetStringAsync(string? requestUri) =>
            await _httpClient.GetStringAsync(requestUri);

        internal async Task<int> GetIntAsync(string? requestUri) =>
            int.Parse(await GetStringAsync(requestUri));

        internal async Task<string[]?> GetStringArrayAsync(string? requestUri) =>
            await _httpClient.GetFromJsonAsync<string[]>(requestUri);

        internal async Task<TResult?> PostAsync<TValue, TResult>(string? requestUri, TValue value)
            where TValue : TObjectBase
            where TResult : TObjectBase =>
            await _httpClient.PostAsJsonAsync<TValue, TResult>(requestUri, value);

        internal async Task<TResult?> PutAsync<TValue, TResult>(string? requestUri, TValue value)
            where TValue : TObjectBase
            where TResult : TObjectBase =>
            await _httpClient.PutAsJsonAsync<TValue, TResult>(requestUri, value);

        public void Dispose() => _httpClient.Dispose();
    }
}