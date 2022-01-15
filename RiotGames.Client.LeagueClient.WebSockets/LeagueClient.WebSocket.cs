﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net.WebSockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;

namespace RiotGames.LeagueOfLegends.LeagueClient
{
    public abstract partial class LeagueClientBase
    {
        internal readonly LeagueClientWampClient? WampClient;
    }

    public abstract partial class LeagueClient
    {
        /// <summary>
        /// Will be made internal soon. Open and Close it yourself.
        /// </summary>
        public LeagueClientWampClient WasmClient => WasmClient;
    }
}
