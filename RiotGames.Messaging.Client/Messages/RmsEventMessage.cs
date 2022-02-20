using System.Diagnostics;
using System.Net.WebSockets.Wamp;
using System.Text.Json;

namespace RiotGames.Messaging;

/// <summary>
///     Will be made internal soon.
/// </summary>
[DebuggerDisplay("Topic = {Topic} ChangeType = {ChangeType} Uri = {Uri} : {Data}")]
public class RmsEventMessage : WampMessage<RmsTypeCode>
{
    public RmsEventMessage(RmsTypeCode messageCode, params JsonElement[] elements) : base(messageCode, elements)
    {
        Topic = elements[0].GetString() ?? throw new RmsException("The WAMP event message didn't have any topic!");
        Data = elements[1].GetProperty("data");
        ChangeType = Enum.Parse<RmsChangeType>(elements[1].GetProperty("changeType").GetString());
        Uri = new Uri(
            elements[1].GetProperty("uri").GetString() ??
            throw new RmsException("The event message didn't have any Uri."), UriKind.Relative);
    }

    public string Topic { get; }

    /// <summary>
    ///     The "Data" will stay as a JsonElement until I've figured out a good way to map it to a type. Feel free to
    ///     contribute on GitHub!
    /// </summary>
    public JsonElement Data { get; }

    public RmsChangeType ChangeType { get; }

    public Uri Uri { get; }
}