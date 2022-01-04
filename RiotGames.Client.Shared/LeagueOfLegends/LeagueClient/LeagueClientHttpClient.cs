using System.Net.Http.Headers;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace RiotGames.LeagueOfLegends.LeagueClient
{
    public class LeagueClientHttpClient : HttpClient
    {
        public LeagueClientHttpClient(string username, string password, ushort port) : base(new LeagueClientHttpClientHandler())
        {
            DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Basic", 
                    Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}")));
            BaseAddress = new UriBuilder("https", "127.0.0.1", port).Uri;
        }
    }

    public class LeagueClientHttpClientHandler : HttpClientHandler
    {
        public LeagueClientHttpClientHandler()
        {
            ClientCertificateOptions = ClientCertificateOption.Manual;
            ServerCertificateCustomValidationCallback = _serverCertificateCustomValidationCallback;
        }

        private bool _serverCertificateCustomValidationCallback(HttpRequestMessage message, X509Certificate2? certificate, X509Chain? chain, SslPolicyErrors errors)
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
