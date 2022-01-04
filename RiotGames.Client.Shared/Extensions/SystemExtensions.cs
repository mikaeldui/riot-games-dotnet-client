using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames
{
    internal static class SystemExtensions
    {
        public static byte[] ToByteArray(this Stream stream)
        {
            if (stream is MemoryStream ms)
                return ms.ToArray();
            else
            {
                using MemoryStream ms2 = new();
                stream.CopyTo(ms2);
                return ms2.ToArray();
            }
        }
    }
}
