using MingweiSamuel;
using MingweiSamuel.Lcu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace RiotGames.Client.CodeGeneration.LeagueClient
{
    using Schema = KeyValuePair<string, LcuComponentSchemaObject>;

    internal class LeagueClientModelsGenerator
    {
        NamespaceDeclarationSyntax _namespace;
        private bool _usingSystemTextJsonSerialization = false;

        public LeagueClientModelsGenerator()
        {
            _namespace = NamespaceHelper.CreateNamespaceDeclaration("RiotGames.LeagueOfLegends.LeagueClient");
        }

        private void _addDto(string identifier, LcuComponentSchemaObject schema)
        {
            MemberDeclarationSyntax member;

            if (schema.Enum != null)
            {
                member = EnumHelper.CreatePublicEnum(identifier, schema.Enum.ToPascalCase());
            }
            else
            {
                var @class = ClassHelper.CreatePublicClassWithBaseType(identifier, $"LeagueClientObject");

                var properties = schema.Properties.Select(kv =>
                {
                    string propertyIdentifier = kv.Key.ToPascalCase();
                    if (propertyIdentifier == identifier)
                        propertyIdentifier = propertyIdentifier.Pluralize();

                    string? jsonProperty = null;
                    if (propertyIdentifier.All(char.IsDigit))
                    {
                        jsonProperty = propertyIdentifier;
                        propertyIdentifier = "X" + propertyIdentifier;
                    }

                    string typeName;

                    if (kv.Value.Type == "array")
                        typeName = kv.Value.Items.GetTypeName() + "[]";
                    else
                        typeName = kv.Value.GetTypeName();

                    typeName += "?"; // Make nullable

                return _simpleProperty(typeName, propertyIdentifier, jsonProperty);
                }).ToArray();

                member = @class.AddMembers(properties);
            }


            // Add the class to the namespace.
            _namespace = _namespace.AddMembers(member);
        }

        public void AddDtos(IEnumerable<Schema> schemas)
        {
            string[] neededDtoSuffixes = schemas.Select(s => OpenApiComponentHelper.GetTypeNameFromRef(s.Key))
                .GroupBy(x => x).Where(g => g.Count() > 1).Select(y => y.Key).ToArray();

            foreach (var schema in schemas)
            {
                var identifier = OpenApiComponentHelper.GetTypeNameFromRef(schema.Key);
                if (neededDtoSuffixes.Contains(identifier))
                    identifier = OpenApiComponentHelper.GetTypeNameFromRef(schema.Key, false);

                _addDto(identifier, schema.Value);
            }
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
