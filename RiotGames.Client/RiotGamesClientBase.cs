using System.Net.Http.Json;

namespace RiotGames
{
    public abstract class RiotGamesClientBase<TObjectBase> : IDisposable
        where TObjectBase : RiotGamesObject
    {
        protected HttpClient _httpClient;
        protected string _apiKey;

        public RiotGamesClientBase(string apiKey)
        {
            _httpClient = new HttpClient();
            _apiKey = apiKey;
        }

        protected async Task<TResult?> GetAsync<TResult>(string? requestUri) 
            where TResult : TObjectBase =>
            await _httpClient.GetFromJsonAsync<TResult>(requestUri);

        protected async Task<TResult?> PostAsync<TValue, TResult>(string? requestUri, TValue value)
            where TValue : TObjectBase
            where TResult : TObjectBase =>
            await _httpClient.PostAsJsonAsync<TValue, TResult>(requestUri, value);

        protected async Task<TResult?> PutAsync<TValue, TResult>(string? requestUri, TValue value)
            where TValue : TObjectBase
            where TResult : TObjectBase =>
            await _httpClient.PutAsJsonAsync<TValue, TResult>(requestUri, value);

        public void Dispose() => _httpClient.Dispose();
    }
}