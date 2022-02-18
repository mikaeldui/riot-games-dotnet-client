using System;
using System.Collections.Generic;
using System.Text;
using System.Net.WebSockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;
using RiotGames.Messaging;

namespace RiotGames.LeagueOfLegends.LeagueClient
{
    public abstract partial class LeagueClientBase
    {
        internal readonly RmsClient? WampClient;
    }

    public partial class LeagueClient
    {
        /// <summary>
        /// Will be made internal soon. Open and Close it yourself.
        /// </summary>
        public new RmsClient WampClient => base.WampClient ?? throw new InvalidOperationException();
    }
}
