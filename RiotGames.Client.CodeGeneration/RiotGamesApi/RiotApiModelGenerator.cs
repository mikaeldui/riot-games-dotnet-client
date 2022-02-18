using Humanizer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MingweiSamuel;
using MingweiSamuel.RiotApi;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static RiotGames.Client.CodeGeneration.CodeAnalysisHelper;

namespace RiotGames.Client.CodeGeneration.RiotGamesApi
{
    using Schema = KeyValuePair<string, RiotApiComponentSchemaObject>;

    internal class RiotApiModelGenerator
    {
        NamespaceDeclarationSyntax _namespace;
        private Client _client;
        private bool _usingSystemTextJsonSerialization = false;

        public RiotApiModelGenerator(Client client)
        {
            _client = client;

            // Ensure we don't get RiotGames.RiotGames.
            var @namespace = new string[] { "RiotGames", client.ToString() }.Distinct().ToArray();

            _namespace = NamespaceDeclaration(@namespace);
        }

        public void AddDto(Schema schema)
        {
            var schemaObject = schema.Value;
            var className = RiotApiModelsHelper.GetTypeNameFromRef(schema.Key);
            var classDeclaration = PublicClassDeclarationWithBaseType(className, $"{_client}Object");

            var properties = schemaObject.Properties.Select(kv =>
            {
                string identifier = kv.Key.ToPascalCase();
                if (identifier == className)
                    identifier = identifier.Pluralize();
                string? jsonProperty = null;
                if (identifier.All(char.IsDigit))
                {
                    jsonProperty = identifier;
                    identifier = "X" + identifier;
                }

                string typeName = kv.Value.GetTypeName(_client);

                //typeName += "?"; // Make nullable

                if (RiotApiHacks.BadPropertyIdentifiers.TryGetValue(identifier, out string? newIdentifier))
                {
                    jsonProperty = identifier;
                    identifier = newIdentifier;
                }

                if (RiotApiHacks.BasicInterfaces.TryGetBasicInterfaceIdentifier(_client, typeName, identifier, out string? interfaceIdentifier))
                    classDeclaration = classDeclaration.AddBaseType(interfaceIdentifier);

                return _simpleProperty(typeName, identifier, jsonProperty);
            }).ToArray();

            classDeclaration = classDeclaration.AddMembers(properties);

            // Add the class to the namespace.
            _namespace = _namespace.AddMembers(classDeclaration);
        }

        public void AddDtos(IEnumerable<Schema> schemas)
        {
            foreach(var schema in schemas)
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
}
