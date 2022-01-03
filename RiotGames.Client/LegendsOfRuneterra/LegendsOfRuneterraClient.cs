using Camille.Enums;

namespace RiotGames.LegendsOfRuneterra
{
    public partial class LegendsOfRuneterraClient : RiotGamesClientBase<LegendsOfRuneterraObject>
    {
        public LegendsOfRuneterraClient(string apiKey, RegionalRoute region) : base(apiKey, region)
        {

        }
    }
}