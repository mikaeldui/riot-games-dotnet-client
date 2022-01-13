using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.Valorant
{
    public interface IValorantObject : IRiotGamesObject
    {
    }

    public interface IValorantMatchId : IValorantObject
    {
        public string MatchId { get; set; }
    }
}
