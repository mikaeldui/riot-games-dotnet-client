using RiotGames.LeagueOfLegends.LeagueClient;
using System.Dynamic;
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

        internal async Task<T> GetSystemTypeAsync<T>(string? requestUri)
        {
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.String:
                    return (T)(object)await HttpClient.GetStringAsync(requestUri);                    
                case TypeCode.Int32:
                    return (T)(object)int.Parse(await HttpClient.GetStringAsync(requestUri));
                case TypeCode.Int64:
                    return (T)(object)long.Parse(await HttpClient.GetStringAsync(requestUri));
                case TypeCode.Double:
                    return (T)(object)double.Parse(await HttpClient.GetStringAsync(requestUri));
            }

            if (typeof(T) == typeof(string[]))
                return (T)(object)await HttpClient.GetFromJsonAsync<string[]>(requestUri);

            if (typeof(T) == typeof(int[]))
                return (T)(object)await HttpClient.GetFromJsonAsync<int[]>(requestUri);

            if (typeof(T) == typeof(dynamic))
                return (T)(object)await HttpClient.GetFromJsonAsync<ExpandoObject>(requestUri);

            throw new NotImplementedException("This system type hasn't been implemented in GetSystemTypeAsync<T>.");
        }

        internal async Task<T> GetEnumAsync<T>(string? requestUri) where T : Enum => 
            (T)Enum.Parse(typeof(T), await HttpClient.GetStringAsync(requestUri));

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