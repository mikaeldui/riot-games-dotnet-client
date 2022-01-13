using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.LegendsOfRuneterra
{
    public interface ILegendsOfRuneterraObject : IRiotGamesObject
    {
    }

    public interface ILegendsOfRuneterraMatchId : ILegendsOfRuneterraObject
    {
        public string MatchId { get; set; }
    }
}
