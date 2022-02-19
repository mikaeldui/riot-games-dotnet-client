using System.Diagnostics;
using Camille.Enums;
using RiotGames.LeagueOfLegends;
using RiotGames.LegendsOfRuneterra;
using RiotGames.TeamfightTactics;
using RiotGames.Valorant;

namespace RiotGames;

[DebuggerDisplay("Region = {Region} Platform = {Platform} ValPlatform = {_valPlatform}")]
public partial class RiotGamesClient : RiotGamesClientBase<IRiotGamesObject>
{
    private readonly string _apiKey;
    private readonly ValPlatformRoute? _valPlatform;

    private LeagueOfLegendsClient? _leagueOfLegends;
    private LegendsOfRuneterraClient? _legendsOfRuneterra;
    private TeamfightTacticsClient? _teamfightTactics;
    private ValorantClient? _valorant;

    /// <summary>
    ///     Using this constructor is useful for when you only need region-specific API. If you're only using /riot endpoints
    ///     then use the region closest to you.
    /// </summary>
    public RiotGamesClient(string apiKey, RegionalRoute region, ValPlatformRoute? valPlatform = null) : base(apiKey,
        region)
    {
        _apiKey = apiKey;
        _valPlatform = valPlatform;
    }

    /// <summary>
    ///     This will set the region to the one for the platform, which might not be closest to you. For the best performance
    ///     on those endpoints, use the regional constructor.
    /// </summary>
    public RiotGamesClient(string apiKey, PlatformRoute platform, RegionalRoute? region = null,
        ValPlatformRoute? valPlatform = null) : base(apiKey, platform, region)
    {
        _apiKey = apiKey;
        _valPlatform = valPlatform;
    }

    [Obsolete("Use the ValorantClient directly instead.", true)]
    public RiotGamesClient(string apiKey, ValPlatformRoute valPlatform) : base(apiKey, valPlatform)
    {
        _apiKey = apiKey;
        _valPlatform = valPlatform;
    }

    public LeagueOfLegendsClient LeagueOfLegends
    {
        get
        {
            if (_leagueOfLegends != null) return _leagueOfLegends;

            if (Region == null)
                throw new RiotGamesRouteException("region");

            _leagueOfLegends = Platform == null
                ? new LeagueOfLegendsClient(_apiKey, (RegionalRoute) Region)
                : new LeagueOfLegendsClient(_apiKey, (PlatformRoute) Platform);

            return _leagueOfLegends;
        }
    }

    public LegendsOfRuneterraClient LegendsOfRuneterra
    {
        get
        {
            if (_legendsOfRuneterra == null)
            {
                if (Region == null)
                    throw new RiotGamesRouteException("region");

                _legendsOfRuneterra = new LegendsOfRuneterraClient(_apiKey, (RegionalRoute) Region);
            }

            return _legendsOfRuneterra;
        }
    }

    public TeamfightTacticsClient TeamfightTactics
    {
        get
        {
            if (_teamfightTactics == null)
            {
                if (Region == null)
                    throw new RiotGamesRouteException("region");

                _teamfightTactics = Platform == null ? new TeamfightTacticsClient(_apiKey, (RegionalRoute) Region) : new TeamfightTacticsClient(_apiKey, (PlatformRoute) Platform);
            }

            return _teamfightTactics;
        }
    }

    public ValorantClient Valorant
    {
        get
        {
            if (_valorant == null)
            {
                if (_valPlatform == null)
                    throw new RiotGamesRouteException("val-platform");

                _valorant = new ValorantClient(_apiKey, (ValPlatformRoute) _valPlatform);
            }

            return _valorant;
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        _leagueOfLegends?.Dispose();
        _legendsOfRuneterra?.Dispose();
        _teamfightTactics?.Dispose();
        _valorant?.Dispose();
    }
}