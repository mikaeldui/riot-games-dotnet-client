using System.Net.Http.Json;

namespace RiotGames
{
    public abstract class RiotGamesClientBase : IDisposable
    {
        protected HttpClient _httpClient;
        protected string _apiKey;

        public RiotGamesClientBase(string apiKey)
        {
            _httpClient = new HttpClient();
            _apiKey = apiKey;
        }

        protected async Task<TResult?> GetAsync<TResult>(string? requestUrl) where TResult : RiotGamesObject =>
            await _httpClient.GetFromJsonAsync<TResult>(requestUrl);

        protected async Task<TResult?> PostAsync<TValue, TResult>(string? requestUrl, TValue value) where TValue : RiotGamesObject where TResult : RiotGamesObject
        {
            var response = await _httpClient.PostAsJsonAsync(requestUrl, value);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResult>();
        }

        protected async Task<TResult?> PutAsync<TValue, TResult>(string? requestUrl, TValue value) where TValue: RiotGamesObject where TResult : RiotGamesObject
        {
            var response = await _httpClient.PutAsJsonAsync(requestUrl, value);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResult>();
        }

        public void Dispose() => _httpClient.Dispose();
    }
}