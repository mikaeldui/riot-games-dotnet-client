using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames
{
    internal static class RiotGamesRootCertificate
    {
        private const string RESOURCE_NAME_PARTIAL = "riotgames.pem";

        public static X509Certificate2 X509Certificate2 { get; }

        static RiotGamesRootCertificate()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames().First(n => n.EndsWith(RESOURCE_NAME_PARTIAL));

            using var stream = assembly.GetManifestResourceStream(resourceName);

            if (stream == null) throw new Exception("Couldn't find the resource riotgames.pem!");

            X509Certificate2 = new X509Certificate2(stream.ToByteArray());
        }
    }
}
