using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MingweiSamuel.RiotApi;

namespace RiotGames.Client.CodeGeneration.RiotGamesApi
{
    using Path = KeyValuePair<string, RiotApiPathObject>;
    using Paths = IEnumerable<KeyValuePair<string, RiotApiPathObject>>;
    using ClientHelper = RiotApiClientsHelper;

    internal enum Client
    {
        RiotGames,
        LeagueOfLegends,
        LegendsOfRuneterra,
        TeamfightTactics,
        Valorant
    }

    internal class RiotApiPathsGenerator
    {
        NamespaceDeclarationSyntax _namespace;
        ClassDeclarationSyntax _classDeclaration;

        public RiotApiPathsGenerator(Client client)
        {
            // Ensure we don't get RiotGames.RiotGames.
            var @namespace = new string[] { "RiotGames", client.ToString() }.Distinct().ToArray();

            _namespace = NamespaceHelper.CreateNamespaceDeclaration(@namespace);

            _classDeclaration = ClassHelper.CreatePublicPartialClass($"{client}Client");
        }

        public void AddEndpoint(string methodIdentifier, bool isPlatform, HttpMethod httpMethod, string requestUri, string returnType, string? requestType = null, Dictionary<string, string>? pathParameters = null)
        {
            // Long time since I did an XOR, this might not work.
            if (httpMethod == HttpMethod.Get ^ requestType == null)
                throw new ArgumentException("If the httpMetod is get then requestType must be null. If it is post or put then it must be set.");

            // Create a stament with the body of a method.
            StatementSyntax bodyStatement;
            {
                string clientName = isPlatform ? "PlatformClient" : "RegionalClient";
                string? typeArgument = httpMethod == HttpMethod.Get ? StatementHelper.TypeArgument(returnType.Remove("[]")) : StatementHelper.TypeArgument(requestType, returnType.Remove("[]"));
                string? specificMethod = null;

                // Current work-around, added some type specific HTTP methods.
                if (returnType.EndsWith("[]"))
                    if (returnType.StartsWith("string"))
                    {
                        specificMethod = "StringArray";
                        typeArgument = null;
                    }
                    else
                        specificMethod = "Array";
                else if (returnType == "int")
                {
                    specificMethod = "Int";
                    typeArgument = null;
                }
                else if (returnType == "string")
                {
                    specificMethod = "String";
                    typeArgument = null;
                }

                string baseMethod = httpMethod.ToString().ToPascalCase() + specificMethod + "Async";

                // Add "$" if it's to be interpolated.
                string requestUriArgument = pathParameters == null ? $"\"{requestUri}\"" : $"$\"{requestUri}\"";

                if (httpMethod == HttpMethod.Get)
                    bodyStatement = StatementHelper.ReturnAwait(clientName, baseMethod, typeArgument, requestUriArgument);
                else
                    bodyStatement = StatementHelper.ReturnAwait(clientName, baseMethod, typeArgument, requestUriArgument, "value");
            }

            _classDeclaration = _classDeclaration.AddPublicAsyncTask(returnType, methodIdentifier, bodyStatement, pathParameters);
        }

        public void AddPathAsEndpoints(Path path)
        {
            //if (path.Key == "/lol/match/v5/matches/{matchId}/timeline") Debugger.Break();

            var po = path.Value;
            var poGet = po.Get;
            if (poGet != null)
            {
                var responseSchema = poGet?.Responses?["200"]?.Content?.First().Value.Schema;
                bool isArrayReponse = responseSchema?.Type == "array";
                var nameFromPath = ClientHelper.GetNameFromPath(path.Key, isArrayReponse);
                bool isPlatform = path.Value.XRouteEnum != "regional";

                Dictionary<string, string?>? pathParameters = null;

                if (poGet is { Parameters: not null, Parameters: { Length: > 0 } })
                {
                    if (!poGet.Parameters.All(p => p.In is "path" or "header" or "query"))
                        Debugger.Break();

                    pathParameters = poGet.Parameters.Where(p => p.In is not "header" and not "query").ToDictionary(p => p.Name, p => p.Schema?.XType ?? p.Schema?.Type);
                }

                AddEndpoint("Get" + nameFromPath, isPlatform, HttpMethod.Get, path.Key, responseSchema.GetTypeName(), pathParameters: pathParameters);
            }
        }

        public void AddPathsAsEndpoints(Paths paths)
        {
            foreach (var path in paths)
                AddPathAsEndpoints(path);
        }


        public string GenerateCode()
        {
            // Add the class to the namespace.
            _namespace = _namespace.AddMembers(_classDeclaration);

            // Normalize and get code as string.
            return _namespace
                .NormalizeWhitespace()
                .ToFullString();
        }
    }
}
