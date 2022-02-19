using System.Dynamic;
using System.Net;
using System.Net.Http.Json;
using System.Reflection;

namespace RiotGames;

internal class RiotGamesHttpClient<TObjectBase> : IDisposable
{
    private static readonly string USER_AGENT;

    internal HttpClient HttpClient;

    static RiotGamesHttpClient()
    {
        var client = UserAgent.From(typeof(RiotGamesHttpClient<TObjectBase>).GetTypeInfo().Assembly);

        var entryAssembly = Assembly.GetEntryAssembly();
        if (entryAssembly != null)
            client.DependentProduct = UserAgent.From(entryAssembly);

        USER_AGENT = client.ToString();
    }

    protected internal RiotGamesHttpClient()
    {
        HttpClient = new HttpClient();
        HttpClient.DefaultRequestHeaders.Add("User-Agent", USER_AGENT);
    }

    protected internal RiotGamesHttpClient(HttpClientHandler httpClientHandler)
    {
        HttpClient = new HttpClient(httpClientHandler);
        HttpClient.DefaultRequestHeaders.Add("User-Agent", USER_AGENT);
    }

    internal Uri BaseAddress
    {
        get => HttpClient.BaseAddress;
        set => HttpClient.BaseAddress = value;
    }

    public void Dispose()
    {
        HttpClient.Dispose();
    }

    internal async Task<TResult> GetAsync<TResult>(string requestUri, CancellationToken cancellationToken = default)
        where TResult : TObjectBase
    {
        return await HttpClient.GetFromJsonAsync<TResult>(requestUri, cancellationToken) ??
               throw new RiotGamesException("Nothing was returned from the API.");
    }

    internal async Task<T> GetSystemTypeAsync<T>(string requestUri, CancellationToken cancellationToken = default)
    {
        switch (Type.GetTypeCode(typeof(T)))
        {
            case TypeCode.String:
                return (T) (object) await HttpClient.GetStringAsync(requestUri, cancellationToken);
            case TypeCode.Int32:
                return (T) (object) int.Parse(await HttpClient.GetStringAsync(requestUri, cancellationToken));
            case TypeCode.Int64:
                return (T) (object) long.Parse(await HttpClient.GetStringAsync(requestUri, cancellationToken));
            case TypeCode.Double:
                return (T) (object) double.Parse(await HttpClient.GetStringAsync(requestUri, cancellationToken));
        }

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8603 // Possible null reference return.
        if (typeof(T) == typeof(string[]))
            return (T) (object) await HttpClient.GetFromJsonAsync<string[]>(requestUri, cancellationToken);

        if (typeof(T) == typeof(int[]))
            return (T) (object) await HttpClient.GetFromJsonAsync<int[]>(requestUri, cancellationToken);

        if (typeof(T) == typeof(ExpandoObject))
            return (T) (object) await HttpClient.GetFromJsonAsync<ExpandoObject>(requestUri, cancellationToken);

        if (typeof(T) == typeof(ExpandoObject[]))
            return (T) (object) await HttpClient.GetFromJsonAsync<ExpandoObject[]>(requestUri, cancellationToken);
#pragma warning restore CS8603 // Possible null reference return.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

        throw new NotImplementedException("This system type hasn't been implemented in GetSystemTypeAsync<T>.");
    }

    internal async Task<T> GetEnumAsync<T>(string requestUri, CancellationToken cancellationToken = default)
        where T : Enum
    {
        return (T) Enum.Parse(typeof(T), await HttpClient.GetStringAsync(requestUri, cancellationToken));
    }

    internal async Task<TResult?> PostAsync<TValue, TResult>(string requestUri, TValue value,
        CancellationToken cancellationToken = default)
        where TValue : TObjectBase
        where TResult : TObjectBase
    {
        return await HttpClient.PostAsJsonAsync<TValue, TResult>(requestUri, value);
    }

    internal async Task<TResult?> PutAsync<TValue, TResult>(string requestUri, TValue value)
        where TValue : TObjectBase
        where TResult : TObjectBase
    {
        return await HttpClient.PutAsJsonAsync<TValue, TResult>(requestUri, value);
    }
}