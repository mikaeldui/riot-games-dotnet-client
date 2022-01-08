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

        protected override void AddEndpoint(EndpointDefinition? endpoint)
        {
            base.AddEndpoint(endpoint);

            if (endpoint == null)
                return;

            // Maybe we have a basic interface and can add an overlaod.
            if (endpoint.Identifier.StartsWith("Get") && endpoint.RequestPathParameters != null && endpoint.RequestPathParameters.Count == 1)
            {
                var parameter = endpoint.RequestPathParameters.Single();

                if (RiotApiHacks.BasicInterfaces.TryGetBasicInterfaceIdentifier(_client, parameter.Value + "?", parameter.Key.ToPascalCase(), out string interfaceIdentifier))
                {
                    string methodIdentifier = endpoint.Identifier;
                    string parameterIdentifier = interfaceIdentifier[1..].RemoveStart(_client.ToString()).RemoveStart("Encrypted").RemoveEnd("Id");

                    if (endpoint.Identifier.Contains("By"))
                    {
                        var methodIdentifierParts = methodIdentifier.Split("By");
                        methodIdentifier = methodIdentifierParts[0];
                        parameterIdentifier = methodIdentifierParts[1];
                    }

                    var method = MethodHelper.PublicAsyncTask(endpoint.ReturnTypeName, methodIdentifier,
                        StatementHelper.ReturnAwait(null, endpoint.Identifier.EndWith("Async"), null, parameterIdentifier.ToCamelCase() + '.' + parameter.Key.ToPascalCase()),
                        new Dictionary<string, string> { { parameterIdentifier.ToCamelCase(), interfaceIdentifier } });

                    ClassDeclaration = ClassDeclaration.AddMembers(method);
                }
            }
        }

        protected override EndpointDefinition? GetMethodObjectToEndpointDefinition(RiotApiMethodObject getMethodObject, string path, RiotApiPathObject pathObject)
        {
            //if (path.Key == "/lol/match/v5/matches/{matchId}/timeline") Debugger.Break();

            var responseSchema = getMethodObject?.Responses?["200"]?.Content?.First().Value.Schema;
            bool isArrayReponse = responseSchema?.Type == "array";
            var methodIdentifier = "Get" + ClientHelper.GetNameFromPath(path, isArrayReponse);
            bool isPlatform = pathObject.XRouteEnum != "regional";

            string clientName = isPlatform ? "PlatformClient" : "RegionalClient";

            string returnType = responseSchema.GetTypeName();
            string? typeArgument = StatementHelper.TypeArgument(returnType.Remove("[]"));

            string httpClientMethod = GetRiotHttpClientMethod(HttpMethod.Get, returnType, ref typeArgument);

            var pathParameters = ToPathParameters(getMethodObject?.Parameters);

            // Addd quotes for C# string
            path = $"\"{path}\"";

            if (pathParameters != null)
            {
                // Add "$" for string interpolation.
                path = $"${path}";

                // Start with typos
                pathParameters.ReplaceKeys(RiotApiHacks.ParameterIdentifierTypos);
                path = path.Replace(RiotApiHacks.PathParameterIdentifierTypos);

                pathParameters.ReplaceKeys(RiotApiHacks.OldParameterIdentifiers);
                path = path.Replace(RiotApiHacks.OldPathParameterIdentifiers);
            }

            return new EndpointDefinition(methodIdentifier, returnType, 
                clientName, httpClientMethod,typeArgument, path, null, pathParameters);            
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

#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            return parameters
                .Where(p => p.In is not "header" and not "query")
                .ToDictionary(p => p.Name, p => (p.Schema.XType ?? p.Schema.Type).ToLower());
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
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
