using System.Net.Http.Headers;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Net.Http.Json;
using System.Dynamic;

namespace RiotGames.LeagueOfLegends.LeagueClient
{
    internal class LeagueClientHttpClient : RiotGamesHttpClient<ILeagueClientObject>
    {
        internal LeagueClientHttpClient(string username, string password, ushort port) : base(new LeagueClientHttpClientHandler())
        {
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}")));
            HttpClient.BaseAddress = new UriBuilder("https", "127.0.0.1", port, "/").Uri;
        }

        public async Task<dynamic> GetDynamicAsync(string requestUri)
        {
            var result = await HttpClient.GetFromJsonAsync<ExpandoObject>(requestUri);

            if (result == null)
                throw new Exception("The HttpClient result was null!");

            return result;
        }
    }

    internal class LeagueClientHttpClientHandler : HttpClientHandler
    {
        public LeagueClientHttpClientHandler()
        {
            ClientCertificateOptions = ClientCertificateOption.Manual;
            ServerCertificateCustomValidationCallback = _serverCertificateCustomValidationCallback;
        }

        private static bool _serverCertificateCustomValidationCallback(HttpRequestMessage message, X509Certificate2? certificate, X509Chain? chain, SslPolicyErrors errors)
        {
            if (certificate == null) return false;
            if (errors == SslPolicyErrors.None) return true;

            using X509Chain privateChain = new();
            privateChain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            privateChain.ChainPolicy.ExtraStore.Add(RiotGamesRootCertificate.X509Certificate2); // Add root certificate.
            privateChain.Build(certificate);

            return privateChain.ChainStatus.Length == 1 && privateChain.ChainStatus[0].Status == X509ChainStatusFlags.UntrustedRoot;
        }
    }
}
