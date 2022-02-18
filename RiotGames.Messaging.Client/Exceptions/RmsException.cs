using System;
using System.Collections.Generic;
using System.Text;

namespace RiotGames.Messaging;

/// <summary>
/// Base exception for Riot Messaging Service.
/// </summary>
public class RmsException : RiotGamesException
{
    public RmsException(string message) : base(message)
    {
    }

    public RmsException(string message, Exception innerException) : base(message, innerException)
    {
    }
}