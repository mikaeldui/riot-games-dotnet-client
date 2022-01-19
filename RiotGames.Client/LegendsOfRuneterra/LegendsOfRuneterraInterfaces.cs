using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.LegendsOfRuneterra
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface ILegendsOfRuneterraObject : IRiotGamesObject
    {
    }

    public interface IMatchId : ILegendsOfRuneterraObject
    {
        public string MatchId { get; set; }
    }
}
