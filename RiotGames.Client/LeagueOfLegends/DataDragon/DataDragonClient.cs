using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.LeagueOfLegends.DataDragon
{
    public partial class DataDragonClient : IDisposable
    {
        public const string DATA_DRAGON_BASE_ADDRESS = "https://ddragon.leagueoflegends.com/";

        internal RiotGamesHttpClient<IDataDragonObject> HttpClient = new();

        public DataDragonClient() => HttpClient.BaseAddress = new Uri(DATA_DRAGON_BASE_ADDRESS);

        public async Task<DataDragonRealm> GetRealmAsync(string region) => await HttpClient.GetAsync<DataDragonRealm>($"realms/{region.ToLower()}.json");

        public void Dispose() => HttpClient.Dispose();
    }
}
