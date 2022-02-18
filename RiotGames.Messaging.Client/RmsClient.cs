using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Net.WebSockets.Wamp;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

namespace RiotGames.Messaging;

/// <summary>
/// A client for the Riot Messaging Service, used by the League Client.
/// </summary>
public class RmsClient : WampSubscriberClient<RmsTypeCode>
{
    private static readonly string USER_AGENT;

    static RmsClient()
    {
        var wampClient = UserAgent.From(typeof(WampSubscriberClient).GetTypeInfo().Assembly);
        wampClient.DependentProduct = UserAgent.From(typeof(RmsClient).GetTypeInfo().Assembly);

        try
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
                wampClient.DependentProduct.DependentProduct = UserAgent.From(entryAssembly);
        }
        catch
        {
            // ignore
        }

        USER_AGENT = wampClient.ToString();
    }

    private readonly ushort _port;

    public RmsClient(string username, string password, ushort port)
    {
        _port = port;
        UseClientWebSocketOptions(options =>
        {
            options.Credentials = new NetworkCredential(username, password);
            options.RemoteCertificateValidationCallback = _remoteCertificateValidationCallback;
            options.SetRequestHeader("User-Agent", USER_AGENT);
        });
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

    // ReSharper disable once UnusedMember.Global
    public async Task ConnectAsync() => await ConnectAsync($"wss://127.0.0.1:{_port}/");

    protected override WampMessage<RmsTypeCode> OnMessageReceived(RmsTypeCode messageCode, JsonElement[] elements) => messageCode switch
    {
        // Should always be, but who knows?
        RmsTypeCode.Event => new RmsEventMessage(messageCode, elements),
        _ => base.OnMessageReceived(messageCode, elements)
    };

    // ReSharper disable once UnusedMember.Global
    public new async Task<RmsEventMessage> ReceiveAsync(CancellationToken cancellationToken = default) =>
        (RmsEventMessage)await base.ReceiveAsync(cancellationToken);
}