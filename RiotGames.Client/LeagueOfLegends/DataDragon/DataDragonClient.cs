using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.LeagueOfLegends.DataDragon
{
    internal class DataDragonClient : IDisposable
    {
        private HttpClient _httpClient = new HttpClient();

        public void Dispose() => _httpClient.Dispose();
    }
}
