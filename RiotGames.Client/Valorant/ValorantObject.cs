using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.Valorant
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class ValorantObject : RiotGamesObject, IValorantObject
    {
    }
}
