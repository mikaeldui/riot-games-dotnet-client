using RiotGames.LeagueOfLegends;
using RiotGames.LegendsOfRuneterra;
using RiotGames.TeamfightTactics;
using RiotGames.Valorant;

namespace RiotGames
{
    public partial class RiotGamesClient : RiotGamesClientBase
    {
        private LeagueOfLegendsClient? _leagueOfLegends;
        private LegendsOfRuneterraClient? _legendsOfRuneterra;
        private TeamfightTacticsClient? _teamfightTactics;
        private ValorantClient? _valorant;
        public RiotGamesClient(string apiKey) : base(apiKey)
        {

        }

        public LeagueOfLegendsClient LeagueOfLegends
        {
            get
            {
                if (_leagueOfLegends == null)
                    _leagueOfLegends = new LeagueOfLegendsClient(_apiKey);
                return _leagueOfLegends;
            }
        }
        public LegendsOfRuneterraClient LegendsOfRuneterra
        {
            get
            {
                if (_legendsOfRuneterra == null)
                    _legendsOfRuneterra = new LegendsOfRuneterraClient(_apiKey);
                return _legendsOfRuneterra;
            }
        }
        public TeamfightTacticsClient TeamfightTactics
        {
            get
            {
                if (_teamfightTactics == null)
                    _teamfightTactics = new TeamfightTacticsClient(_apiKey);
                return _teamfightTactics;
            }
        }
        public ValorantClient Valorant
        {
            get
            {
                if (_valorant == null)
                    _valorant = new ValorantClient(_apiKey);
                return _valorant;
            }
        }
    }
}