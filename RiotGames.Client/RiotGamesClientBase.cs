using Camille.Enums;

namespace RiotGames
{
    public abstract class RiotGamesClientBase<TObjectBase> : IDisposable
        where TObjectBase : RiotGamesObject
    {
        private RegionalRoute? _region;
        private PlatformRoute? _platform;
        private ValPlatformRoute? _valPlatform;
        private RiotGamesApiHttpClient<TObjectBase>? _regionalClient;
        private RiotGamesApiHttpClient<TObjectBase>? _platformClient;

        //// Will have to investigate if the RiotGamesClient can give its HttpClient to its children.
        //internal RiotGamesClientBase(RiotHttpClient<TObjectBase>? regionalClient, RiotHttpClient<TObjectBase>? platformClient)
        //{
        //    _regionalClient = regionalClient;
        //    _platformClient = platformClient;
        //}

        internal RiotGamesClientBase(string apiKey, RegionalRoute region, ValPlatformRoute? valPlatform = null)
        {
            _region = region;
            _regionalClient = new RiotGamesApiHttpClient<TObjectBase>(apiKey, region);
        }

        internal RiotGamesClientBase(string apiKey, PlatformRoute platform, bool createRegionalClient = true)
        {
            _platform = platform;
            _platformClient = new RiotGamesApiHttpClient<TObjectBase>(apiKey, platform);

            if (createRegionalClient)
            {
                _region = RouteUtils.ToRegional(platform);
                _regionalClient = new RiotGamesApiHttpClient<TObjectBase>(apiKey, (RegionalRoute)_region);
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