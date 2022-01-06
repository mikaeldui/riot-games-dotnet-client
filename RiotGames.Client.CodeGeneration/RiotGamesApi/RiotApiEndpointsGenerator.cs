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
    using ClientHelper = RiotApiEndpointsHelper;

    internal enum Client
    {
        RiotGames,
        LeagueOfLegends,
        LegendsOfRuneterra,
        TeamfightTactics,
        Valorant
    }

    internal class RiotApiEndpointsGenerator : OpenApiEndpointGeneratorBase<RiotApiPathObject, RiotApiMethodObject, RiotApiPostMethodObject, RiotApiPutMethodObject, RiotApiParameterObject, RiotApiSchemaObject>
    {
        Client _client;

        public RiotApiEndpointsGenerator(Client client) : base($"{client}Client", true)
        {
            _client = client;
        }

        protected override EndpointDefinition? GetMethodObjectToEndpointDefinition(RiotApiMethodObject getMethodObject, string path, RiotApiPathObject pathObject)
        {
            //if (path.Key == "/lol/match/v5/matches/{matchId}/timeline") Debugger.Break();

            var responseSchema = getMethodObject?.Responses?["200"]?.Content?.First().Value.Schema;
            bool isArrayReponse = responseSchema?.Type == "array";
            var nameFromPath = ClientHelper.GetNameFromPath(path, isArrayReponse);
            bool isPlatform = pathObject.XRouteEnum != "regional";

            string clientName = isPlatform ? "PlatformClient" : "RegionalClient";

            string returnType = responseSchema.GetTypeName();
            string? typeArgument = StatementHelper.TypeArgument(returnType.Remove("[]"));

            string httpClientMethod = GetRiotHttpClientMethod(HttpMethod.Get, returnType, ref typeArgument);

            var pathParameters = ToPathParameters(getMethodObject?.Parameters);
            // Add "$" if it's to be interpolated.

            string requestUriArgument = pathParameters == null ? $"\"{path}\"" : $"$\"{path}\"";

            return new EndpointDefinition(
                "Get" + nameFromPath, 
                responseSchema.GetTypeName(), 
                clientName, 
                httpClientMethod,
                typeArgument, 
                requestUriArgument, 
                null, 
                pathParameters);            
        }

        public override string GenerateCode()
        {
            // Ensure we don't get RiotGames.RiotGames.
            var namespaceIdentifier = new string[] { "RiotGames", _client.ToString() }.Distinct().ToArray();

            var @namespace = NamespaceHelper.CreateNamespaceDeclaration(namespaceIdentifier);

            // Add the class to the namespace.
            @namespace = @namespace.AddMembers(ClassDeclaration);

            // Normalize and get code as string.
            return @namespace
                .NormalizeWhitespace()
                .ToFullString();
        }

        protected virtual Dictionary<string, string>? ToPathParameters(RiotApiParameterObject[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
                return null;

            if (!parameters.All(p => p.In is "path" or "header" or "query"))
                Debugger.Break();

            return parameters
                .Where(p => p.In is not "header" and not "query")
                .ToDictionary(p => p.Name, p => p.Schema.XType ?? p.Schema.Type);
        }

        protected override EndpointDefinition? PostMethodObjectToEndpointDefinition(RiotApiPostMethodObject postMethodObject, string path, RiotApiPathObject pathObject)
        {
            throw new NotImplementedException();
        }

        protected override EndpointDefinition? PutMethodObjectToEndpointDefinition(RiotApiPutMethodObject putMethodObject, string path, RiotApiPathObject pathObject)
        {
            throw new NotImplementedException();
        }
    }
}
