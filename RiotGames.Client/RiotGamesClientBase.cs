using Camille.Enums;
using System.ComponentModel;

namespace RiotGames
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class RiotGamesClientBase<TObjectBase> : IDisposable
        where TObjectBase : IRiotGamesObject
    {
        private readonly RiotGamesApiHttpClient<TObjectBase>? _regionalClient;
        private readonly RiotGamesApiHttpClient<TObjectBase>? _platformClient;

        internal RiotGamesClientBase(string apiKey, RegionalRoute region, ValPlatformRoute? valPlatform = null)
        {
            Region = region;
            ValPlatform = valPlatform;
            _regionalClient = new RiotGamesApiHttpClient<TObjectBase>(apiKey, region);
            if (valPlatform != null)
                _platformClient = new RiotGamesApiHttpClient<TObjectBase>(apiKey, (ValPlatformRoute)valPlatform);
        }

        internal RiotGamesClientBase(string apiKey, PlatformRoute platform, RegionalRoute? region = null, bool createRegionalClient = true)
        {
            Platform = platform;
            _platformClient = new RiotGamesApiHttpClient<TObjectBase>(apiKey, platform);

            if (!createRegionalClient) return;
            region ??= platform.ToRegional();
            Region = region;
            _regionalClient = new RiotGamesApiHttpClient<TObjectBase>(apiKey, (RegionalRoute)region);
        }

        /// <summary>
        /// Does NOT create a region client as Valorant doesn't use that yet.
        /// </summary>
        internal RiotGamesClientBase(string apiKey, ValPlatformRoute platform)
        {
            ValPlatform = platform;
            _platformClient = new RiotGamesApiHttpClient<TObjectBase>(apiKey, platform);
        }

        public RegionalRoute? Region { get; }

        public PlatformRoute? Platform { get; }

        public ValPlatformRoute? ValPlatform { get; }

        internal RiotGamesApiHttpClient<TObjectBase> RegionalClient => _regionalClient ?? throw new RiotGamesRouteException("region");

        internal RiotGamesApiHttpClient<TObjectBase> PlatformClient => _platformClient ?? throw new RiotGamesRouteException("platform");

        public virtual void Dispose()
        {
            _regionalClient?.Dispose();
            _platformClient?.Dispose();
        }
    }
}
