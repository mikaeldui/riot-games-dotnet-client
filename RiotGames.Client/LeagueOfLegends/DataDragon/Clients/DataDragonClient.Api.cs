using System.ComponentModel;

namespace RiotGames.LeagueOfLegends.DataDragon;

public partial class DataDragonClient
{
    private DataDragonApiClient? _api;

    public DataDragonApiClient Api => _api ??= new DataDragonApiClient(this);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public class DataDragonApiClient
    {
        private readonly DataDragonClient _parent;

        internal DataDragonApiClient(DataDragonClient parent)
        {
            _parent = parent;
        }

        public async Task<DataDragonCollection<string>> GetVersionsAsync()
        {
            return await _parent.HttpClient.GetAsync<DataDragonCollection<string>>("api/versions.json");
        }
    }
}