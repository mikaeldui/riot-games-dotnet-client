using System;
using System.Collections.Generic;
using System.Text;
using System.Net.WebSockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;

namespace RiotGames.LeagueOfLegends.LeagueClient
{
    public class LeagueClientWebSocketClient : IDisposable
    {
        readonly ClientWebSocket _client;
        readonly Uri _uri;

        internal LeagueClientWebSocketClient(LeagueClientLockfile lockfile) : this("riot", lockfile.Password, lockfile.Port)
        {
        }

        public LeagueClientWebSocketClient(string username, string password, ushort port)
        {
            _uri = new UriBuilder("wss", "127.0.0.1", port).Uri;
            _client = new();
            _client.Options.SetRequestHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}")));
            _client.Options.RemoteCertificateValidationCallback = _remoteCertificateValidationCallback;
        }

        private static bool _remoteCertificateValidationCallback(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors errors)
        {
            if (certificate == null) return false;
            if (errors == SslPolicyErrors.None) return true;

            using X509Chain privateChain = new();
            privateChain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            privateChain.ChainPolicy.ExtraStore.Add(RiotGamesRootCertificate.X509Certificate2); // Add root certificate.
            privateChain.Build((X509Certificate2) certificate);

            return privateChain.ChainStatus.Length == 1 && privateChain.ChainStatus[0].Status == X509ChainStatusFlags.UntrustedRoot;
        }

        public async Task Connect()
        {
            await _client.ConnectAsync(_uri, default);
        }

        public async Task Subscribe(string topic)
        {
            await _client.SendStringAsync($"[5, \"{topic}\"]");
        }

        public async Task Unsubscribe(string topic)
        {
            await _client.SendStringAsync($"[6, \"{topic}\"]");
        }

        public async Task<string> Receive()
        {
            string? @string = "";

            for (int i = 0; i < 10 && string.IsNullOrEmpty(@string); i++)
                @string = await ReceiveStringAsync(_client);

            if (!@string.StartsWith("[8"))
                throw new Exception("Didn't receive \"[8\"!");

            return @string;
        }

        public async Task Disconnect()
        {
            await _client.CloseAsync(WebSocketCloseStatus.NormalClosure, null, default);
        }

        public void Dispose() => _client.Dispose();

        public static async Task<string> ReceiveStringAsync(WebSocket webSocket, CancellationToken cancellationToken = default)
        {
            ArraySegment<byte> buffer = new(new byte[1024 * 4]);
            var result = await webSocket.ReceiveAsync(buffer, cancellationToken);
            return result.MessageType switch
            {
                WebSocketMessageType.Text => Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count).Trim('\0'),
                WebSocketMessageType.Close => throw new WebSocketException(WebSocketError.ConnectionClosedPrematurely),
                _ => throw new WebSocketException(WebSocketError.InvalidMessageType),
            };
        }
    }

    public partial class LeagueClient
    {

    }
}
