using Camille.Enums;

namespace RiotGames
{
    public abstract class RiotGamesClientBase<TObjectBase> : IDisposable
        where TObjectBase : RiotGamesObject
    {
        private RegionalRoute? _region;
        private PlatformRoute? _platform;
        private ValPlatformRoute? _valPlatform;
        private RiotGamesHttpClient<TObjectBase>? _regionalClient;
        private RiotGamesHttpClient<TObjectBase>? _platformClient;

        //// Will have to investigate if the RiotGamesClient can give its HttpClient to its children.
        //internal RiotGamesClientBase(RiotHttpClient<TObjectBase>? regionalClient, RiotHttpClient<TObjectBase>? platformClient)
        //{
        //    _regionalClient = regionalClient;
        //    _platformClient = platformClient;
        //}

        internal RiotGamesClientBase(string apiKey, RegionalRoute region, ValPlatformRoute? valPlatform = null)
        {
            _region = region;
            _regionalClient = new RiotGamesHttpClient<TObjectBase>(apiKey, region);
        }

        internal RiotGamesClientBase(string apiKey, PlatformRoute platform, bool createRegionalClient = true)
        {
            _platform = platform;
            _platformClient = new RiotGamesHttpClient<TObjectBase>(apiKey, platform);

            if (createRegionalClient)
            {
                _region = RouteUtils.ToRegional(platform);
                _regionalClient = new RiotGamesHttpClient<TObjectBase>(apiKey, (RegionalRoute)_region);
            }
        }

        /// <summary>
        /// Does NOT create a region client as Valorant doesn't use that yet.
        /// </summary>
        internal RiotGamesClientBase(string apiKey, ValPlatformRoute platform)
        {
            _valPlatform = platform;
            _platformClient = new RiotGamesHttpClient<TObjectBase>(apiKey, platform);
        }

        public RegionalRoute? Region => _region;
        public PlatformRoute? Platform => _platform;
        public ValPlatformRoute? ValPlatform => _valPlatform;

        internal RiotGamesHttpClient<TObjectBase> RegionalClient            
        {
            get
            {
                if (_regionalClient == null)
                    throw new RiotGamesRouteException("region");

                return _regionalClient;
            }        
        }

        internal RiotGamesHttpClient<TObjectBase> PlatformClient
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