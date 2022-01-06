using Humanizer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MingweiSamuel;
using MingweiSamuel.RiotApi;

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

            _namespace = NamespaceHelper.CreateNamespaceDeclaration(@namespace);
        }

        public void AddDto(Schema schema)
        {
            var schemaObject = schema.Value;
            var className = RiotApiModelsHelper.GetTypeNameFromRef(schema.Key);
            var classDeclaration = ClassHelper.CreatePublicClassWithBaseType(className, $"{_client}Object");

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

                string typeName;

                if (kv.Value.Type == "array" || kv.Value.XType == "array")
                    typeName = kv.Value.Items.GetTypeName() + "[]";
                else
                    typeName = kv.Value.GetTypeName();

                typeName += "?"; // Make nullable

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
            var property = PropertyHelper.CreatePublicProperty(typeName, identifier);

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
