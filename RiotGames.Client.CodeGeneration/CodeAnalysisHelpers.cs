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
    internal static class NamespaceHelper
    {
        public static NamespaceDeclarationSyntax CreateNamespaceDeclaration(params string[] @namespace) =>
            SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(string.Join('.', @namespace))).NormalizeWhitespace();
    }

    internal static class ClassHelper
    {
        public static ClassDeclarationSyntax CreatePublicClass(string @class) =>
            SyntaxFactory.ClassDeclaration(@class).AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

        public static ClassDeclarationSyntax CreatePublicPartialClass(string @class) =>
            SyntaxFactory.ClassDeclaration(@class).AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.PartialKeyword));

        public static ClassDeclarationSyntax CreatePublicClassWithBaseType(string @class, string baseType) =>
            CreatePublicClass(@class).AddBaseListTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(baseType)));
    }

    internal static class PropertyHelper
    {
        public static PropertyDeclarationSyntax CreatePublicProperty(string typeName, string identifier) =>
            SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(typeName), identifier)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddAccessorListAccessors(
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));
    }

    internal static class MethodHelper
    {
        public static MethodDeclarationSyntax? PublicAsyncTask(string returnType, string methodName, StatementSyntax bodyStatement, Dictionary<string, string>? parameters = null)
        {
            var methodDeclaration = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("Task<" + returnType + ">"), methodName.EndWith("Async"))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.AsyncKeyword))
                .WithBody(SyntaxFactory.Block(bodyStatement));

            if (parameters != null)
                foreach(var parameter in parameters)
                    methodDeclaration = methodDeclaration.AddParameterListParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier(parameter.Key)).WithType(SyntaxFactory.ParseTypeName(parameter.Value)));

            return methodDeclaration;
        }
    }

    internal static class StatementHelper
    {
        public static string TypeArgument(string returnType) => $"<{returnType.Remove("[]")}>";
        public static string TypeArgument(string valueType, string returnType) => $"<{valueType}, {returnType}>";
        public static StatementSyntax ReturnAwait(string objectName, string methodName, string? typeArgument, params string[] arguments) =>
            SyntaxFactory.ParseStatement($"return await {objectName}.{methodName}{typeArgument}({string.Join(", ", arguments)});");
    }
}
