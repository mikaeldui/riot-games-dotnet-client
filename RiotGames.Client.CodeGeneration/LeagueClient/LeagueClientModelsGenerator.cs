using Humanizer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MingweiSamuel.Lcu;
using static RiotGames.Client.CodeGeneration.CodeAnalysisHelper;

namespace RiotGames.Client.CodeGeneration.LeagueClient;

using Schema = KeyValuePair<string, LcuComponentSchemaObject>;

internal class LeagueClientModelsGenerator
{
    private NamespaceDeclarationSyntax _namespace;
    private bool _usingSystemTextJsonSerialization;

    public LeagueClientModelsGenerator()
    {
        _namespace = NamespaceDeclaration("RiotGames.LeagueOfLegends.LeagueClient");
    }

    private void _addDto(string identifier, LcuComponentSchemaObject schema)
    {
        MemberDeclarationSyntax member;

        if (schema.Enum != null)
        {
            if (!_usingSystemTextJsonSerialization)
            {
                _usingSystemTextJsonSerialization = true;
                _namespace = _namespace.AddSystemTextJsonSerializationUsing();
            }

            member = PublicEnumDeclaration(identifier, schema.Enum.ToPascalCase())
                .AddJsonStringEnumAttribute();
        }
        else
        {
            var @class = PublicClassDeclarationWithBaseType(identifier, "LeagueClientObject");

            var properties = (schema.Properties ?? throw new InvalidOperationException()).Select(kv =>
            {
                var propertyIdentifier = kv.Key.ToPascalCase();
                if (propertyIdentifier == identifier)
                    propertyIdentifier = propertyIdentifier.Pluralize();

                string? jsonProperty = null;
                if (propertyIdentifier.All(char.IsDigit))
                {
                    jsonProperty = propertyIdentifier;
                    propertyIdentifier = "X" + propertyIdentifier;
                }

                var typeName = kv.Value.Type == "array"
                    ? $"LeagueClientCollection<{(kv.Value.Items ?? throw new InvalidOperationException()).GetTypeName()}>"
                    : kv.Value.GetTypeName();

                //typeName += "?"; // Make nullable

                // TODO: fix client name
                if (LeagueClientHacks.BasicInterfaces.TryGetBasicInterfaceIdentifier("LeagueClient", typeName,
                        propertyIdentifier, out var @interface))
                    @class = @class.AddBaseType(@interface);

                return _simpleProperty(typeName, propertyIdentifier, jsonProperty);
            }).ToArray();

            member = @class.AddMembers(properties);
        }


        // Add the class to the namespace.
        _namespace = _namespace.AddMembers(member);
    }

    public void AddDtos(IEnumerable<Schema> schemas)
    {
        var neededDtoSuffixes = schemas.Select(s => OpenApiComponentHelper.GetTypeNameFromRef(s.Key))
            .GroupBy(x => x).Where(g => g.Count() > 1).Select(y => y.Key).ToArray();

        foreach (var (@ref, schemaObject) in schemas)
        {
            var identifier = OpenApiComponentHelper.GetTypeNameFromRef(@ref);
            if (neededDtoSuffixes.Contains(identifier))
                identifier = OpenApiComponentHelper.GetTypeNameFromRef(@ref, false);

            _addDto(identifier, schemaObject);
        }
    }

    public string GenerateCode()
    {
        // Normalize and get code as string.
        return _namespace.NormalizeWhitespace().ToFullString();
    }

    public string[] GetEnums()
    {
        return _namespace.Members
            .OfType<EnumDeclarationSyntax>()
            .Select(e => e.Identifier.ToString()).ToArray();
    }

    private PropertyDeclarationSyntax _simpleProperty(string typeName, string identifier, string? jsonProperty = null)
    {
        var property = PublicPropertyDeclaration(typeName, identifier);

        if (jsonProperty == null) return property;

        // We need to add this namespace for the attribute.
        if (_usingSystemTextJsonSerialization) return property.AddJsonPropertyNameAttribute(jsonProperty);

        _usingSystemTextJsonSerialization = true;
        _namespace = _namespace.AddSystemTextJsonSerializationUsing();

        return property.AddJsonPropertyNameAttribute(jsonProperty);
    }
}