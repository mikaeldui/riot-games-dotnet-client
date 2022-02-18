﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using RiotGames.LeagueOfLegends.LeagueClient;

namespace RiotGames.Messaging
{
    /// <summary>
    /// Maybe not the best name, but this class is a special RmsClient used for routing received messages to .NET events.
    /// </summary>
    public class RmsEventRouter : IDisposable
    {
        private readonly ConcurrentDictionary<string, Action<JsonElement>> _subscriptions = new();
        private readonly SemaphoreSlim _connectionLock = new(1, 1);
        private readonly RmsClient _rmsClient;
        private CancellationTokenSource? _cancellationTokenSource = new();

        private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

        public RmsEventRouter(string username, string password, ushort port)
        {
            _rmsClient = new RmsClient(username, password, port);
        }

        /// <summary>
        /// Will open a connection if there isn't one.
        /// </summary>
        public void Subscribe<TData>(string topic, Action<TData> handler) => Task.Run(async () =>
        {
            await _connectionLock.WaitAsync();
            try
            {
                if (_subscriptions.IsEmpty) // Then the connection is closed.
                {
                    _cancellationTokenSource?.Dispose();
                    _cancellationTokenSource = new CancellationTokenSource();

                    await _rmsClient.ConnectAsync(); // TODO: CancellationToken
                    Console.WriteLine("WampClient connected");

                    _ = Task.Run(async () =>
                    {
                        while (!_cancellationTokenSource.IsCancellationRequested)
                        {
                            var response = await _rmsClient.ReceiveAsync();
                            Console.WriteLine("Message received");

                            if (!_subscriptions.TryGetValue(response.Topic, out var subscriber)) continue;
                            subscriber?.Invoke(response.Data);
                        }
                    }, _cancellationTokenSource.Token).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                throw new RmsException("Couldn't open an RMS connection! See innerException.", ex);
            }
            finally
            {
                _connectionLock.Release();
            }

            if (_subscriptions.TryAdd(topic, data => handler.Invoke(data.Deserialize<TData>(_jsonSerializerOptions) ?? throw new RmsException("Couldn't deserialize the event payload."))))
            {
                await _rmsClient.SubscribeAsync(topic);
                Console.WriteLine("Subscribed to topic: " + topic);
            }
            else
            {
                throw new RmsException("Couldn't add topic subscriber!");
            }
        }).ConfigureAwait(false);

        /// <summary>
        /// Will close the connection if this was the last topic.
        /// </summary>
        public void Unsubscribe(string topic) => Task.Run(async () =>
        {
            await _rmsClient.UnsubscribeAsync("OnJsonApiEvent_lol-champ-select_v1_session");
            if (_subscriptions.TryRemove(topic, out _))
            {
                Console.WriteLine("Unsubscribed");
            }

            await _connectionLock.WaitAsync();
            try
            {
                if (_subscriptions.IsEmpty) // Then the connection should be closed.
                {
                    _cancellationTokenSource!.Cancel();
                    await _rmsClient.CloseAsync();
                    Console.WriteLine("WampClient closed");
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("remote")) return;
                throw new RmsException("Something unexpected happened during closure of RMS connection! See innerException.", ex);
            }
            finally
            {
                _connectionLock.Release();
            }
        }).ConfigureAwait(false);

        public void Dispose()
        {
            _connectionLock.Dispose();
            _rmsClient.Dispose();
            _cancellationTokenSource?.Dispose();
        }
    }
}
