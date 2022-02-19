using System.ComponentModel;

namespace RiotGames.LeagueOfLegends.DataDragon;

public partial class DataDragonClient
{
    private DataDragonCdnClient? _cdn;

    public DataDragonCdnClient Cdn => _cdn ??= new DataDragonCdnClient(this);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public class DataDragonCdnClient
    {
        private readonly DataDragonClient _parent;

        internal DataDragonCdnClient(DataDragonClient parent)
        {
            _parent = parent;
        }

        public async Task<DataDragonCollection<string>> GetLanguagesAsync()
        {
            return await _parent.HttpClient.GetAsync<DataDragonCollection<string>>("cdn/languages.json");
        }
    }
}