using System.Diagnostics;
using RiotGames.Messaging;
// ReSharper disable UnusedMember.Global

namespace RiotGames.LeagueOfLegends.LeagueClient;

/// <summary>
///     You can use it to communicate with the League Client. Didn't want to name it LeagueClientClient.
/// </summary>
public partial class LeagueClient
{
    private Process? _process;

    /// <summary>
    /// Occurs when the League Client exits.
    /// </summary>
    public event EventHandler Exited
    {
        add
        {
            _process ??= LockFile.GetProcess();
            _process.EnableRaisingEvents = true;

            _process.Exited += value;

        }
        remove
        {
            _process!.Exited -= value;

            //if (_process.Exited == null)
            //{
            //    _process.Dispose();
            //    _process = null;
            //}
        }
    }
}