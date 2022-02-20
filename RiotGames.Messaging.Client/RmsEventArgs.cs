namespace RiotGames.Messaging;

public class RmsEventArgs<T> : EventArgs
{
    public RmsEventArgs(RmsEventType eventType, T data)
    {
        EventType = eventType;
        Data = data;
    }

    public RmsEventType EventType { get; }
    public T Data { get; }
}