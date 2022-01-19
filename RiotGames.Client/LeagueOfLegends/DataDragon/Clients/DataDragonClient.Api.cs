using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace RiotGames.LeagueOfLegends.DataDragon
{
    public partial class DataDragonClient
    {
        private DataDragonApiClient? _api;

        public DataDragonApiClient Api
        {
            get
            {
                if (_api == null)
                    _api = new DataDragonApiClient(this);
                return _api;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class DataDragonApiClient
        {
            readonly DataDragonClient _parent;

            internal DataDragonApiClient(DataDragonClient parent)
            {
                _parent = parent;
            }

            public async Task<DataDragonCollection<string>> GetVersionsAsync()
            {
                return await _parent.HttpClient.GetAsync<DataDragonCollection<string>>("/api/versions.json");
            }
        }
    }
}
