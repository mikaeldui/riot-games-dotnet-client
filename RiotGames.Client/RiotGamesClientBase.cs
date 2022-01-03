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

        public async Task<TResult?> GetAsync<TResult>(string? requestUri) 
            where TResult : TObjectBase =>
            await _httpClient.GetFromJsonAsync<TResult>(requestUri);

        protected async Task<TResult[]?> GetArrayAsync<TResult>(string? requestUri)
            where TResult : TObjectBase =>
            await _httpClient.GetFromJsonAsync<TResult[]>(requestUri);

        protected async Task<string> GetStringAsync(string? requestUri) =>
            await _httpClient.GetStringAsync(requestUri);

        protected async Task<int> GetIntAsync(string? requestUri) =>
            int.Parse(await GetStringAsync(requestUri));

        protected async Task<string[]?> GetStringArrayAsync(string? requestUri) =>
            await _httpClient.GetFromJsonAsync<string[]>(requestUri);

        public async Task<TResult?> PostAsync<TValue, TResult>(string? requestUri, TValue value)
            where TValue : TObjectBase
            where TResult : TObjectBase =>
            await _httpClient.PostAsJsonAsync<TValue, TResult>(requestUri, value);

        public async Task<TResult?> PutAsync<TValue, TResult>(string? requestUri, TValue value)
            where TValue : TObjectBase
            where TResult : TObjectBase =>
            await _httpClient.PutAsJsonAsync<TValue, TResult>(requestUri, value);

        public virtual void Dispose() => _httpClient.Dispose();
    }
}