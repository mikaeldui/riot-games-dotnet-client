using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace RiotGames.LeagueOfLegends.DataDragon
{
    public partial class DataDragonClient
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public class DataDragonApiClient
        {
            DataDragonClient _parent;

            internal DataDragonApiClient(DataDragonClient parent)
            {
                _parent = parent;
            }

            public async Task<DataDragonCollection<string>> GetVersionAsync()
            {
                return await _parent.HttpClient.GetAsync<DataDragonCollection<string>>("/api/versions.json");
            }
        }
    }
}
