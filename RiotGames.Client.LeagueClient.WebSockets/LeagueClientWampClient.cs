using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Net.WebSockets.Wamp;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

namespace RiotGames.LeagueOfLegends.LeagueClient
{
    /// <summary>
    /// Will be made internal soon.
    /// </summary>
    public class LeagueClientWampClient : WampSubscriberClient<LeagueClientWampMessageTypeCode>
    {
        private static readonly string USER_AGENT;

        static LeagueClientWampClient()
        {
            var wampClient = UserAgent.From(typeof(WampSubscriberClient).GetTypeInfo().Assembly);
            wampClient.Name = "MikaelDui.Net.WebSockets.Wamp";
            wampClient.DependentProduct = UserAgent.From(typeof(LeagueClientWampClient).GetTypeInfo().Assembly);
            wampClient.DependentProduct.Name = "MikaelDui.RiotGames.Client";

            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
                wampClient.DependentProduct.DependentProduct = UserAgent.From(entryAssembly);            

            USER_AGENT = wampClient.ToString();
        }

        private readonly ushort _port;

        public LeagueClientWampClient(string username, string password, ushort port)
        {
            _port = port;
            Options.Credentials = new NetworkCredential(username, password);
            Options.RemoteCertificateValidationCallback = _remoteCertificateValidationCallback;
            Options.SetRequestHeader("User-Agent", USER_AGENT);
        }

        private static bool _remoteCertificateValidationCallback(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors errors)
        {
            if (certificate == null) return false;
            if (errors == SslPolicyErrors.None) return true;

            using X509Chain privateChain = new();
            privateChain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            privateChain.ChainPolicy.ExtraStore.Add(RiotGamesRootCertificate.X509Certificate2); // Add root certificate.
            privateChain.Build((X509Certificate2)certificate);

            return privateChain.ChainStatus.Length == 1 && privateChain.ChainStatus[0].Status == X509ChainStatusFlags.UntrustedRoot;
        }

        public async Task ConnectAsync() => await ConnectAsync($"wss://127.0.0.1:{_port}/");

        protected override WampMessage<LeagueClientWampMessageTypeCode> OnMessageReceived(LeagueClientWampMessageTypeCode messageCode, JsonElement[] elements) => messageCode switch
        {
            // Should always be, but who knows?
            LeagueClientWampMessageTypeCode.Event => new LeagueClientWampEventMessage(messageCode, elements),
            _ => base.OnMessageReceived(messageCode, elements)
        };

        public new async Task<LeagueClientWampEventMessage> ReceiveAsync(CancellationToken cancellationToken = default) =>
            (LeagueClientWampEventMessage)await base.ReceiveAsync(cancellationToken);
    }

    /// <summary>
    /// Will be made internal soon.
    /// </summary>
    [DebuggerDisplay("Topic = {Topic} EventType = {EventType} Uri = {Uri} : {Data}")]
    public class LeagueClientWampEventMessage : WampMessage<LeagueClientWampMessageTypeCode>
    {
        public LeagueClientWampEventMessage(LeagueClientWampMessageTypeCode messageCode, params JsonElement[] elements) : base(messageCode, elements)
        {
            Topic = elements[0].GetString() ?? throw new LeagueClientException("The WAMP event message didn't have any topic!");
            Data = elements[1].GetProperty("data");
            EventType = (LeagueClientWampEventType) Enum.Parse(typeof(LeagueClientWampEventType), elements[1].GetProperty("eventType").GetString());
            Uri = new Uri(elements[1].GetProperty("uri").GetString(), UriKind.Relative);
        }

        public string Topic { get; }

        public JsonElement Data { get; }

        public LeagueClientWampEventType EventType { get; }

        public Uri Uri { get; }
    }

    public enum LeagueClientWampEventType
    {
        Create,
        Update,
        Delete
    }
}
