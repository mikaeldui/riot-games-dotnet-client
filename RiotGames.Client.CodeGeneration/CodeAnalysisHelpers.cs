using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.Client.CodeGeneration
{
    using SF = SyntaxFactory;

    internal static class NamespaceHelper
    {
        public static NamespaceDeclarationSyntax CreateNamespaceDeclaration(params string[] @namespace) =>
            SF.NamespaceDeclaration(SF.ParseName(string.Join('.', @namespace))).NormalizeWhitespace();
    }

    internal static class ClassHelper
    {
        public static ClassDeclarationSyntax CreatePublicClass(string @class) =>
            SF.ClassDeclaration(@class).AddModifiers(SF.Token(SyntaxKind.PublicKeyword));

        public static ClassDeclarationSyntax CreatePublicPartialClass(string @class) =>
            SF.ClassDeclaration(@class).AddModifiers(SF.Token(SyntaxKind.PublicKeyword), SF.Token(SyntaxKind.PartialKeyword));

        public static ClassDeclarationSyntax CreatePublicClassWithBaseType(string @class, string baseType) =>
            CreatePublicClass(@class).AddBaseListTypes(SF.SimpleBaseType(SF.ParseTypeName(baseType)));

        public static ConstructorDeclarationSyntax CreateInternalConstructor(string className, string parameterType, string parameterIdentifier, string fieldIdentifier) =>
            SF.ConstructorDeclaration(className)
                .AddParameterListParameters(ParameterHelper.CreateParameter(parameterType, parameterIdentifier))
                .WithBody(SF.Block(SF.ParseStatement($"{fieldIdentifier} = {parameterIdentifier};")))
                .AddModifiers(SF.Token(SyntaxKind.InternalKeyword));
    }

    internal static class EnumHelper
    {
        public static EnumDeclarationSyntax CreatePublicEnum(string identifier, params string[] members) =>
            SF.EnumDeclaration(SF.Identifier(identifier))
                .AddModifiers(SF.Token(SyntaxKind.PublicKeyword))
                .AddMembers(members.Select(m => SF.EnumMemberDeclaration(SF.Identifier(m))).ToArray());
    }

    internal static class AttributeHelper
    {
        public static AttributeSyntax CreateAttribute(string identifier) =>
            SyntaxFactory.Attribute(SyntaxFactory.IdentifierName(identifier));

        public static AttributeSyntax CreateAttribute(string identifier, string stringArgument) =>
            CreateAttribute(identifier).WithArgumentList(SyntaxFactory.ParseAttributeArgumentList($"(\"{stringArgument}\")"));
    }

    internal static class FieldHelper
    {
        public static FieldDeclarationSyntax CreatePrivateField(string type, string identifier) =>
            SF.FieldDeclaration(
                    SF.VariableDeclaration(
                        SF.ParseTypeName(type),
                        SF.SeparatedList(new[] { SF.VariableDeclarator(SF.Identifier(identifier.StartsWith('_') ? identifier : '_' + identifier)) })
                    ))
                    .AddModifiers(SF.Token(SyntaxKind.PrivateKeyword));
    }

    internal static class PropertyHelper
    {
        public static PropertyDeclarationSyntax CreatePublicProperty(string typeName, string identifier) =>
            SF.PropertyDeclaration(SF.ParseTypeName(typeName), identifier)
                .AddModifiers(SF.Token(SyntaxKind.PublicKeyword))
                .AddAccessorListAccessors(
                    SF.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(SF.Token(SyntaxKind.SemicolonToken)),
                    SF.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(SF.Token(SyntaxKind.SemicolonToken)));

        public static PropertyDeclarationSyntax CreateFieldBackedPublicReadOnlyProperty(string typeName, string identifier, string fieldIdentifier, string? typeConstructorArgument = null) =>
            SF.PropertyDeclaration(SF.ParseTypeName(typeName), SF.Identifier(identifier))
                .AddModifiers(SF.Token(SyntaxKind.PublicKeyword))
                .AddAccessorListAccessors(
                    SF.AccessorDeclaration(
                        SyntaxKind.GetAccessorDeclaration,
                        SF.Block(SF.ParseStatement($"if ({fieldIdentifier} == null)\n{fieldIdentifier} = new {typeName}({typeConstructorArgument});\n\nreturn {fieldIdentifier};"))));
    }

    internal static class MethodHelper
    {
        public static MethodDeclarationSyntax? PublicAsyncTask(string returnType, string methodName, StatementSyntax bodyStatement, Dictionary<string, string>? parameters = null)
        {
            var methodDeclaration = SF.MethodDeclaration(
                SF.ParseTypeName("Task" + StatementHelper.TypeArgument(returnType)), 
                methodName.EndWith("Async"))
                .AddModifiers(SF.Token(SyntaxKind.PublicKeyword), SF.Token(SyntaxKind.AsyncKeyword))
                .WithBody(SF.Block(bodyStatement));

            if (parameters != null)
                methodDeclaration = methodDeclaration.AddParameterListParameters(parameters.ToParameters());

            return methodDeclaration;
        }
    }

    internal static class ParameterHelper
    {
        public static ParameterSyntax CreateParameter(string typeName, string identifier) =>
            SF.Parameter(SF.Identifier(identifier)).WithType(SF.ParseTypeName(typeName));

        public static ParameterSyntax[] ToParameters(this IEnumerable<KeyValuePair<string, string>> parameters) =>
            parameters.Select(p => CreateParameter(p.Value, p.Key)).ToArray();
    }

    internal static class StatementHelper
    {
        public static string TypeArgument(string returnType) => $"<{returnType}>";
        public static string TypeArgument(string valueType, string returnType) => $"<{valueType}, {returnType}>";
        public static StatementSyntax ReturnAwait(string objectName, string methodName, string? typeArgument, params string[] arguments) =>
            SF.ParseStatement($"return await {objectName}.{methodName}{typeArgument}({string.Join(", ", arguments)});");
        public static StatementSyntax SameClassReturnAwait(string methodName, string? typeArgument, params string[] arguments) =>
            SF.ParseStatement($"return await {methodName}{typeArgument}({string.Join(", ", arguments)});");
    }
}
