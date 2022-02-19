using System.Diagnostics;
using Microsoft.CodeAnalysis;
using MingweiSamuel.RiotApi;
using static RiotGames.Client.CodeGeneration.CodeAnalysisHelper;

namespace RiotGames.Client.CodeGeneration.RiotGamesApi;

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

internal class RiotApiEndpointsGenerator : OpenApiEndpointGeneratorBase<RiotApiPathObject, RiotApiMethodObject,
    RiotApiPostMethodObject, RiotApiPutMethodObject, RiotApiParameterObject, RiotApiSchemaObject>
{
    private readonly Client _client;

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
        if (!(endpoint.Identifier ?? throw new InvalidOperationException()).StartsWith("Get") ||
            endpoint.RequestPathParameters == null ||
            (endpoint.RequestPathParameters ?? throw new InvalidOperationException()).Count != 1) return;

        var parameter = (endpoint.RequestPathParameters ?? throw new InvalidOperationException()).Single();

        if (!(RiotApiHacks.BasicInterfaces ?? throw new InvalidOperationException()).TryGetBasicInterfaceIdentifier(
                _client, parameter.Value + "?",
                (parameter.Key ?? throw new InvalidOperationException()).ToPascalCase(),
                out var interfaceIdentifier)) return;

        var methodIdentifier = endpoint.Identifier;

        var parameterIdentifier = interfaceIdentifier[1..].RemoveStart(_client.ToString()).RemoveStart("Encrypted")
            .RemoveEnd("Id");

        if ((endpoint.Identifier ?? throw new InvalidOperationException()).Contains("By"))
        {
            var methodIdentifierParts = methodIdentifier.Split("By");
            methodIdentifier = methodIdentifierParts[0];
            parameterIdentifier = methodIdentifierParts[1];
        }

        var method = CancellablePublicAsyncTaskDeclaration(endpoint.ReturnTypeName, methodIdentifier,
            CancellableReturnAwaitStatement(null,
                (endpoint.Identifier ?? throw new InvalidOperationException()).EndWith("Async"), null,
                parameterIdentifier.ToCamelCase() + '.' +
                (parameter.Key ?? throw new InvalidOperationException()).ToPascalCase()),
            new Dictionary<string, string> {{parameterIdentifier.ToCamelCase(), interfaceIdentifier}});

        Class = Class.AddMembers(method);
    }

    protected override EndpointDefinition? GetMethodObjectToEndpointDefinition(RiotApiMethodObject getMethodObject,
        string path, RiotApiPathObject pathObject)
    {
        //if (path.Key == "/lol/match/v5/matches/{matchId}/timeline") Debugger.Break();

        var responseSchema = getMethodObject?.Responses?["200"]?.Content?.First().Value.Schema;

        if (responseSchema == null)
        {
            Debugger.Break();
            throw new ArgumentException("The 200 response doesn't have any schema.");
        }

        var isArrayReponse = responseSchema.Type == "array";
        var methodIdentifier = "Get" + ClientHelper.GetNameFromPath(path, isArrayReponse);
        var isPlatform = pathObject.XRouteEnum != "regional";

        var clientName = isPlatform ? "PlatformClient" : "RegionalClient";

        var returnType = responseSchema.GetTypeName(_client);
        var typeArgument = TypeArgumentStatement(returnType);

        var httpClientMethod = GetRiotHttpClientMethod(HttpMethod.Get, returnType, ref typeArgument);

        var pathParameters = ToPathParameters(getMethodObject?.Parameters);

        // Add quotes for C# string
        path = $"\"{path}\"";

        if (pathParameters != null && pathParameters.Any())
        {
            // Add "$" for string interpolation.
            path = $"${path}";

            // Start with typos
            pathParameters.ReplaceKeys(RiotApiHacks.ParameterIdentifierTypos);
            path = path.Replace(RiotApiHacks.PathParameterIdentifierTypos);

            pathParameters.ReplaceKeys(RiotApiHacks.BadParameterIdentifiers);
            path = path.Replace(RiotApiHacks.OldPathParameterIdentifiers);
        }

        return new EndpointDefinition(methodIdentifier, returnType,
            clientName, httpClientMethod, typeArgument, path, null, pathParameters);
    }

    public override string GenerateCode()
    {
        // Ensure we don't get RiotGames.RiotGames.
        var namespaceIdentifier = new[] {"RiotGames", _client.ToString()}.Distinct().ToArray();

        var @namespace = NamespaceDeclaration(namespaceIdentifier);

        // Add the class to the namespace.
        @namespace = @namespace.AddMembers(Class);

        // Normalize and get code as string.
        return @namespace
            .NormalizeWhitespace()
            .ToFullString();
    }

    protected virtual Dictionary<string, string>? ToPathParameters(IEnumerable<RiotApiParameterObject>? parameters)
    {
        if (parameters == null || !parameters.Any())
            return default;

        if (!parameters.All(p => p.In is "path" or "header" or "query"))
            Debugger.Break();

#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        return parameters
            .Where(p => p.In is not "header" and not "query")
            .ToDictionary(p => p.Name,
                p => ((p.Schema ?? throw new InvalidOperationException()).XType ??
                      (p.Schema ?? throw new InvalidOperationException()).Type).ToLower());
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
    }

    protected override EndpointDefinition? PostMethodObjectToEndpointDefinition(
        RiotApiPostMethodObject postMethodObject, string path, RiotApiPathObject pathObject)
    {
        throw new NotImplementedException();
    }

    protected override EndpointDefinition? PutMethodObjectToEndpointDefinition(RiotApiPutMethodObject putMethodObject,
        string path, RiotApiPathObject pathObject)
    {
        throw new NotImplementedException();
    }
}