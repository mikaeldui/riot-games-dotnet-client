using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace RiotGames.Client.CodeGeneration
{
    using SF = SyntaxFactory;

    [DebuggerStepThrough]
    internal static class CodeAnalysisHelper
    {
        public static NamespaceDeclarationSyntax NamespaceDeclaration(params string[] @namespace) =>
            SF.NamespaceDeclaration(ParseName(string.Join('.', @namespace))).NormalizeWhitespace();

        public static ClassDeclarationSyntax PublicClassDeclaration(string identifier) =>
            ClassDeclaration(identifier).WithModifier(SyntaxKind.PublicKeyword);

        public static ClassDeclarationSyntax PublicPartialClassDeclaration(string identifier) =>
            ClassDeclaration(identifier).WithModifiers(SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword);

        public static ClassDeclarationSyntax PublicClassDeclarationWithBaseType(string identifier, string baseTypeName) =>
            PublicClassDeclaration(identifier).WithBaseType(baseTypeName);

        public static ConstructorDeclarationSyntax ConstructorDeclaration(string identifier, string parameterType, string parameterIdentifier) =>
            SF.ConstructorDeclaration(identifier).WithParameter(parameterType, parameterIdentifier);

        public static ConstructorDeclarationSyntax InternalConstructorDeclaration(string identifier) =>
            SF.ConstructorDeclaration(identifier).WithModifier(SyntaxKind.InternalKeyword);

        public static ConstructorDeclarationSyntax InternalConstructorDeclaration(string identifier, string parameterType, string parameterIdentifier, string fieldIdentifier, string? parameterProperty = null) =>
            ConstructorDeclaration(identifier, parameterType, parameterIdentifier)
                .WithModifier(SyntaxKind.InternalKeyword)
                .WithBody($"{fieldIdentifier} = {parameterIdentifier + (parameterProperty == null ? null : '.' + parameterProperty)};");

        public static EnumDeclarationSyntax PublicEnumDeclaration(string identifier, params string[] members) =>
            EnumDeclaration(Identifier(identifier)).WithModifiers(SyntaxKind.PublicKeyword.ToTokenList())
                .AddMembers(members.Select(m => EnumMemberDeclaration(Identifier(m))).ToArray());

        public static FieldDeclarationSyntax FieldDeclaration(string typeName, string identifier) =>
            SF.FieldDeclaration(
                VariableDeclaration(
                    ParseTypeName(typeName),
                    SeparatedList(new[] { VariableDeclarator(Identifier(identifier)) })
                ));

        public static FieldDeclarationSyntax PrivateFieldDeclaration(string typeName, string identifier) =>
            FieldDeclaration(typeName, identifier.StartWith("_")).WithModifier(SyntaxKind.PrivateKeyword);

        public static FieldDeclarationSyntax PrivateReadOnlyFieldDeclaration(string typeName, string identifier) =>
            PrivateFieldDeclaration(typeName, identifier).AddModifier(SyntaxKind.ReadOnlyKeyword);

        public static FieldDeclarationSyntax InternalFieldDeclaration(string typeName, string identifier) =>
            FieldDeclaration(typeName, identifier).WithModifier(SyntaxKind.InternalKeyword);

        public static FieldDeclarationSyntax InternalReadOnlyFieldDeclaration(string typeName, string identifier) =>
            InternalFieldDeclaration(typeName, identifier).AddModifier(SyntaxKind.ReadOnlyKeyword);

        public static PropertyDeclarationSyntax PublicPropertyDeclaration(string typeName, string identifier) =>
            PropertyDeclaration(ParseTypeName(typeName), identifier)
                .WithModifier(SyntaxKind.PublicKeyword)
                .AddAccessorListAccessors(
                    AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                    AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));

        public static PropertyDeclarationSyntax FieldBackedPublicReadOnlyPropertyDeclaration(string typeName, string identifier, string fieldIdentifier, string? typeConstructorArgument = null) =>
            PropertyDeclaration(ParseTypeName(typeName), Identifier(identifier))
                .WithModifier(SyntaxKind.PublicKeyword)
                .WithAccessor(SyntaxKind.GetAccessorDeclaration, $"if ({fieldIdentifier} == null)\n{fieldIdentifier} = new {typeName}({typeConstructorArgument});\n\nreturn {fieldIdentifier};");

        public static MethodDeclarationSyntax PublicAsyncTaskDeclaration(string returnType, string methodName, StatementSyntax bodyStatement, params ParameterSyntax[] parameters)
        {
            var methodDeclaration = MethodDeclaration(ParseTypeName("Task" + TypeArgumentStatement(returnType)), methodName.EndWith("Async"))
                .WithModifiers(SyntaxKind.PublicKeyword, SyntaxKind.AsyncKeyword)
                .WithBody(bodyStatement.ToBlock());

            if (parameters != null)
                methodDeclaration = methodDeclaration.AddParameterListParameters(parameters);

            return methodDeclaration;
        }

        public static MethodDeclarationSyntax PublicAsyncTaskDeclaration(string returnType, string methodName, StatementSyntax bodyStatement, Dictionary<string, string>? parameters = null) =>
            PublicAsyncTaskDeclaration(returnType, methodName, bodyStatement, parameters?.ToParameters()?.ToArray());

        public static MethodDeclarationSyntax CancellablePublicAsyncTaskDeclaration(string returnType, string methodName, StatementSyntax bodyStatement, params ParameterSyntax[] parameters)
        {
            var @params = parameters.ToList();

            @params.Add(OptionalParameter("CancellationToken", "cancellationToken"));

            return PublicAsyncTaskDeclaration(returnType, methodName, bodyStatement, @params.ToArray());
        }

        public static MethodDeclarationSyntax CancellablePublicAsyncTaskDeclaration(string returnType, string methodName, StatementSyntax bodyStatement, Dictionary<string, string>? parameters = null)
        {
            return CancellablePublicAsyncTaskDeclaration(returnType, methodName, bodyStatement, parameters?.ToParameters()?.ToArray() ?? Array.Empty<ParameterSyntax>());
        }

        public static ParameterSyntax Parameter(string typeName, string identifier) =>
            SF.Parameter(Identifier(identifier)).WithType(ParseTypeName(typeName));

        public static ParameterSyntax OptionalParameter(string typeName, string identifier, string defaultValue = "default") =>
            SF.Parameter(default, default, ParseTypeName(typeName), Identifier(identifier), EqualsValueClause(ParseExpression(defaultValue)));

        /// <summary>Formats it like "<{returnType}>".</summary>
        public static string TypeArgumentStatement(string returnType) => $"<{returnType}>";

        public static string TypeArgumentStatement(string valueType, string returnType) => $"<{valueType}, {returnType}>";

        private static StatementSyntax ReturnAwaitStatement(string? objectName, string methodName, string? typeArgument, params string[] arguments)
        {
            if (objectName == null)
                return ParseStatement($"return await {methodName}{typeArgument}({string.Join(", ", arguments)});");

            return ParseStatement($"return await {objectName}.{methodName}{typeArgument}({string.Join(", ", arguments)});");
        }

        public static StatementSyntax CancellableReturnAwaitStatement(string? objectName, string methodName, string? typeArgument, params string[] arguments)
        {
            var argumentsList = arguments.ToList();
            argumentsList.Add("cancellationToken");
            return ReturnAwaitStatement(objectName, methodName, typeArgument, argumentsList.ToArray());
        }
    }
}
