using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames
{
    public interface ISummonerId
    {
        public string? SummonerId { get; set; }
    }

    public interface IPuuid
    {
        public string? Puuid { get; set; }
    }
}
