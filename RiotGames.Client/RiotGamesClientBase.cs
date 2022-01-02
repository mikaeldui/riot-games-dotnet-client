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

        protected async Task<T?> GetAsync<T>(string? requestUrl) =>
            await _httpClient.GetFromJsonAsync<T>(requestUrl);

        public void Dispose() => _httpClient.Dispose();
    }
}