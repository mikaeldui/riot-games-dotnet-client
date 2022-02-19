using Humanizer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MingweiSamuel.RiotApi;
using static RiotGames.Client.CodeGeneration.CodeAnalysisHelper;

namespace RiotGames.Client.CodeGeneration.RiotGamesApi;

using Schema = KeyValuePair<string, RiotApiComponentSchemaObject>;

internal class RiotApiModelGenerator
{
    private readonly Client _client;
    private NamespaceDeclarationSyntax _namespace;
    private bool _usingSystemTextJsonSerialization;

    public RiotApiModelGenerator(Client client)
    {
        _client = client;

        // Ensure we don't get RiotGames.RiotGames.
        var @namespace = new[] {"RiotGames", client.ToString()}.Distinct().ToArray();

        _namespace = NamespaceDeclaration(@namespace);
    }

    public void AddDto(Schema schema)
    {
        var (@ref, schemaObject) = schema;
        var className = RiotApiModelsHelper.GetTypeNameFromRef(@ref);
        var classDeclaration = PublicClassDeclarationWithBaseType(className, $"{_client}Object");

        var properties = schemaObject.Properties.Select(kv =>
        {
            var (key, propertyObject) = kv;
            var identifier = key.ToPascalCase();
            if (identifier == className)
                identifier = identifier.Pluralize();
            string? jsonProperty = null;
            if (identifier.All(char.IsDigit))
            {
                jsonProperty = identifier;
                identifier = "X" + identifier;
            }

            var typeName = propertyObject.GetTypeName(_client);

            //typeName += "?"; // Make nullable

            if (RiotApiHacks.BadPropertyIdentifiers.TryGetValue(identifier, out var newIdentifier))
            {
                jsonProperty = identifier;
                identifier = newIdentifier;
            }

            if (RiotApiHacks.BasicInterfaces.TryGetBasicInterfaceIdentifier(_client, typeName, identifier,
                    out var interfaceIdentifier))
                classDeclaration = classDeclaration.AddBaseType(interfaceIdentifier);

            return _simpleProperty(typeName, identifier, jsonProperty);
        }).ToArray();

        classDeclaration = classDeclaration.AddMembers(properties);

        // Add the class to the namespace.
        _namespace = _namespace.AddMembers(classDeclaration);
    }

    public void AddDtos(IEnumerable<Schema> schemas)
    {
        foreach (var schema in schemas)
            AddDto(schema);
    }

    public string GenerateCode()
    {
        // Normalize and get code as string.
        return _namespace.NormalizeWhitespace().ToFullString();
    }

    private PropertyDeclarationSyntax _simpleProperty(string typeName, string identifier, string? jsonProperty = null)
    {
        var property = PublicPropertyDeclaration(typeName, identifier);

        if (jsonProperty != null)
        {
            // We need to add this namespace for the attribute.
            if (!_usingSystemTextJsonSerialization)
            {
                _usingSystemTextJsonSerialization = true;
                _namespace = _namespace.AddSystemTextJsonSerializationUsing();
            }

            property = property.AddJsonPropertyNameAttribute(jsonProperty);
        }

        return property;
    }
}