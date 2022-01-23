using Camille.Enums;
using System.ComponentModel;

namespace RiotGames
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class RiotGamesClientBase<TObjectBase> : IDisposable
        where TObjectBase : IRiotGamesObject
    {
        private readonly RegionalRoute? _region;
        private readonly PlatformRoute? _platform;
        private readonly ValPlatformRoute? _valPlatform;
        private readonly RiotGamesApiHttpClient<TObjectBase>? _regionalClient;
        private readonly RiotGamesApiHttpClient<TObjectBase>? _platformClient;

        internal RiotGamesClientBase(string apiKey, RegionalRoute region, ValPlatformRoute? valPlatform = null)
        {
            _region = region;
            _valPlatform = valPlatform;
            _regionalClient = new RiotGamesApiHttpClient<TObjectBase>(apiKey, region);
            if (valPlatform != null)
                _platformClient = new RiotGamesApiHttpClient<TObjectBase>(apiKey, (ValPlatformRoute)valPlatform);
        }

        internal RiotGamesClientBase(string apiKey, PlatformRoute platform, RegionalRoute? region = null, bool createRegionalClient = true)
        {
            _platform = platform;
            _platformClient = new RiotGamesApiHttpClient<TObjectBase>(apiKey, platform);

            if (createRegionalClient)
            {
                if (region == null)
                    region = RouteUtils.ToRegional(platform);
                _region = region;
                _regionalClient = new RiotGamesApiHttpClient<TObjectBase>(apiKey, (RegionalRoute)region);
            }
        }

        /// <summary>
        /// Does NOT create a region client as Valorant doesn't use that yet.
        /// </summary>
        internal RiotGamesClientBase(string apiKey, ValPlatformRoute platform)
        {
            _valPlatform = platform;
            _platformClient = new RiotGamesApiHttpClient<TObjectBase>(apiKey, platform);
        }

        public RegionalRoute? Region => _region;
        public PlatformRoute? Platform => _platform;
        public ValPlatformRoute? ValPlatform => _valPlatform;

        internal RiotGamesApiHttpClient<TObjectBase> RegionalClient            
        {
            get
            {
                if (_regionalClient == null)
                    throw new RiotGamesRouteException("region");

                return _regionalClient;
            }        
        }

        internal RiotGamesApiHttpClient<TObjectBase> PlatformClient
        {
            get
            {
                if (_platformClient == null)
                    throw new RiotGamesRouteException("platform");

                return _platformClient;
            }
        }

        public virtual void Dispose()
        {
            _regionalClient?.Dispose();
            _platformClient?.Dispose();
        }
    }
}
