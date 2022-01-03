using Humanizer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MingweiSamuel;

namespace RiotGames.Client.CodeGeneration
{
    using Schema = KeyValuePair<string, RiotApiOpenApiSchema.ComponentsObject.SchemaObject>;

    internal class ModelGenerator
    {
        NamespaceDeclarationSyntax _namespace;
        private Client _client;

        public ModelGenerator(Client client)
        {
            _client = client;

            _namespace = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(string.Join('.', new string[] { "RiotGames", client.ToString() }.Distinct()))).NormalizeWhitespace();

            _namespace = _namespace.AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Text.Json.Serialization")));
        }

        public void AddDto(Schema schema)
        {
            var schemaObject = schema.Value;
            var className = ModelHelper.GetTypeNameFromRef(schema.Key);
            //  Create a class: (class RiotGamesClient)
            var classDeclaration = SyntaxFactory.ClassDeclaration(className);

            // Add the public modifier: (public partial class RiotGamesClient)
            classDeclaration = classDeclaration.AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            //// Inherit BaseEntity<T> and implement IHaveIdentity: (public class Order : BaseEntity<T>, IHaveIdentity)
            classDeclaration = classDeclaration.AddBaseListTypes(
                SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName($"{_client}Object")));

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

                if (kv.Value.Type == "array" || kv.Value.XType == "array")                
                    return _simpleProperty(kv.Value.Items.GetTypeName() + "[]?", identifier, jsonProperty);                

                return _simpleProperty(kv.Value.GetTypeName() + "?", identifier, jsonProperty);
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
            var property = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(typeName), identifier)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddAccessorListAccessors(
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));

            if (jsonProperty == null)
                return property;

            else
                return property.AddAttributeLists(
                    SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList<AttributeSyntax>(
                        SyntaxFactory.Attribute(SyntaxFactory.IdentifierName("JsonPropertyName"))
                        .WithArgumentList(SyntaxFactory.ParseAttributeArgumentList($"(\"{jsonProperty}\")")))));

        }
    }
}
