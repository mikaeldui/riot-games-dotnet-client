namespace RiotGames.Messaging;

public class RmsEventArgs<T> : EventArgs
{
    public RmsEventArgs(RmsChangeType changeType, T data)
    {
        ChangeType = changeType;
        Data = data;
    }

    public RmsChangeType ChangeType { get; }
    public T Data { get; }
}