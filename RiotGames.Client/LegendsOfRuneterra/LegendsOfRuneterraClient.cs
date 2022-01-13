using Camille.Enums;

namespace RiotGames.LegendsOfRuneterra
{
    public partial class LegendsOfRuneterraClient : RiotGamesClientBase<ILegendsOfRuneterraObject>
    {
        public LegendsOfRuneterraClient(string apiKey, RegionalRoute region) : base(apiKey, region)
        {
        }
    }
}