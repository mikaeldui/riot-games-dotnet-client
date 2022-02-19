using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace RiotGames.Client.CodeGeneration;

using SF = SyntaxFactory;

[DebuggerStepThrough]
internal static class CodeAnalysisHelper
{
    public static NamespaceDeclarationSyntax NamespaceDeclaration(params string[] @namespace)
    {
        return SF.NamespaceDeclaration(ParseName(string.Join('.', @namespace))).NormalizeWhitespace();
    }

    public static ClassDeclarationSyntax PublicClassDeclaration(string identifier)
    {
        return ClassDeclaration(identifier).WithModifier(SyntaxKind.PublicKeyword);
    }

    public static ClassDeclarationSyntax PublicPartialClassDeclaration(string identifier)
    {
        return ClassDeclaration(identifier).WithModifiers(SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword);
    }

    public static ClassDeclarationSyntax PublicClassDeclarationWithBaseType(string identifier, string baseTypeName)
    {
        return PublicClassDeclaration(identifier).WithBaseType(baseTypeName);
    }

    public static ConstructorDeclarationSyntax ConstructorDeclaration(string identifier, string parameterType,
        string parameterIdentifier)
    {
        return SF.ConstructorDeclaration(identifier).WithParameter(parameterType, parameterIdentifier);
    }

    public static ConstructorDeclarationSyntax InternalConstructorDeclaration(string identifier)
    {
        return SF.ConstructorDeclaration(identifier).WithModifier(SyntaxKind.InternalKeyword);
    }

    public static ConstructorDeclarationSyntax InternalConstructorDeclaration(string identifier, string parameterType,
        string parameterIdentifier, string fieldIdentifier, string? parameterProperty = null)
    {
        return ConstructorDeclaration(identifier, parameterType, parameterIdentifier)
            .WithModifier(SyntaxKind.InternalKeyword)
            .WithBody(
                $"{fieldIdentifier} = {parameterIdentifier + (parameterProperty == null ? null : '.' + parameterProperty)};");
    }

    // TODO: Write a better one
    public static ConstructorDeclarationSyntax InternalConstructorDeclaration(string identifier, string parameterType,
        string parameterIdentifier, string fieldIdentifier, string? parameterProperty, string fieldIdentifier2,
        string parameterProperty2)
    {
        return ConstructorDeclaration(identifier, parameterType, parameterIdentifier)
            .WithModifier(SyntaxKind.InternalKeyword)
            .WithBody(
                $"{fieldIdentifier} = {parameterIdentifier + '.' + parameterProperty}; {fieldIdentifier2} = {parameterIdentifier + '.' + parameterProperty2};");
    }

    public static EnumDeclarationSyntax PublicEnumDeclaration(string identifier, params string[] members)
    {
        return EnumDeclaration(Identifier(identifier)).WithModifiers(SyntaxKind.PublicKeyword.ToTokenList())
            .AddMembers(members.Select(m => EnumMemberDeclaration(Identifier(m))).ToArray());
    }

    public static FieldDeclarationSyntax FieldDeclaration(string typeName, string identifier)
    {
        return SF.FieldDeclaration(
            VariableDeclaration(
                ParseTypeName(typeName),
                SeparatedList(new[] {VariableDeclarator(Identifier(identifier))})
            ));
    }

    public static FieldDeclarationSyntax PrivateFieldDeclaration(string typeName, string identifier)
    {
        return FieldDeclaration(typeName, identifier.StartWith("_")).WithModifier(SyntaxKind.PrivateKeyword);
    }

    public static FieldDeclarationSyntax PrivateReadOnlyFieldDeclaration(string typeName, string identifier)
    {
        return PrivateFieldDeclaration(typeName, identifier).AddModifier(SyntaxKind.ReadOnlyKeyword);
    }

    public static FieldDeclarationSyntax InternalFieldDeclaration(string typeName, string identifier)
    {
        return FieldDeclaration(typeName, identifier).WithModifier(SyntaxKind.InternalKeyword);
    }

    public static FieldDeclarationSyntax InternalReadOnlyFieldDeclaration(string typeName, string identifier)
    {
        return InternalFieldDeclaration(typeName, identifier).AddModifier(SyntaxKind.ReadOnlyKeyword);
    }

    public static PropertyDeclarationSyntax PublicPropertyDeclaration(string typeName, string identifier)
    {
        return PropertyDeclaration(ParseTypeName(typeName), identifier)
            .WithModifier(SyntaxKind.PublicKeyword)
            .AddAccessorListAccessors(
                AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));
    }

    public static PropertyDeclarationSyntax FieldBackedPublicReadOnlyPropertyDeclaration(string typeName,
        string identifier, string fieldIdentifier, string? typeConstructorArgument = null)
    {
        return PropertyDeclaration(ParseTypeName(typeName), Identifier(identifier))
            .WithModifier(SyntaxKind.PublicKeyword)
            .WithAccessor(SyntaxKind.GetAccessorDeclaration,
                $"if ({fieldIdentifier} == null)\n{fieldIdentifier} = new {typeName}({typeConstructorArgument});\n\nreturn {fieldIdentifier};");
    }

    public static MethodDeclarationSyntax PublicAsyncTaskDeclaration(string returnType, string methodName,
        StatementSyntax bodyStatement, params ParameterSyntax[] parameters)
    {
        var methodDeclaration = MethodDeclaration(ParseTypeName("Task" + TypeArgumentStatement(returnType)),
                methodName.EndWith("Async"))
            .WithModifiers(SyntaxKind.PublicKeyword, SyntaxKind.AsyncKeyword)
            .WithBody(bodyStatement.ToBlock());

        methodDeclaration = methodDeclaration.AddParameterListParameters(parameters);

        return methodDeclaration;
    }

    public static MethodDeclarationSyntax PublicAsyncTaskDeclaration(string returnType, string methodName,
        StatementSyntax bodyStatement, Dictionary<string, string>? parameters = null)
    {
        return PublicAsyncTaskDeclaration(returnType, methodName, bodyStatement,
            parameters?.ToParameters()?.ToArray() ?? Array.Empty<ParameterSyntax>());
    }

    public static MethodDeclarationSyntax CancellablePublicAsyncTaskDeclaration(string returnType, string methodName,
        StatementSyntax bodyStatement, params ParameterSyntax[] parameters)
    {
        var @params = parameters.ToList();

        @params.Add(SyntaxFactory.Parameter(default, default, ParseTypeName("CancellationToken"),
            Identifier("cancellationToken"), EqualsValueClause(ParseExpression("default"))));

        return PublicAsyncTaskDeclaration(returnType, methodName, bodyStatement, @params.ToArray());
    }

    public static MethodDeclarationSyntax CancellablePublicAsyncTaskDeclaration(string returnType, string methodName,
        StatementSyntax bodyStatement, Dictionary<string, string>? parameters = null)
    {
        return CancellablePublicAsyncTaskDeclaration(returnType, methodName, bodyStatement,
            parameters?.ToParameters()?.ToArray() ?? Array.Empty<ParameterSyntax>());
    }

    public static ParameterSyntax Parameter(string typeName, string identifier)
    {
        return SF.Parameter(Identifier(identifier)).WithType(ParseTypeName(typeName));
    }

    public static string TypeArgumentStatement(string returnType)
    {
        return $"<{returnType}>";
    }

    public static string TypeArgumentStatement(string valueType, string returnType)
    {
        return $"<{valueType}, {returnType}>";
    }

    private static StatementSyntax ReturnAwaitStatement(string? objectName, string methodName, string? typeArgument,
        params string[] arguments)
    {
        return ParseStatement(objectName == null ? $"return await {methodName}{typeArgument}({string.Join(", ", arguments)});" : $"return await {objectName}.{methodName}{typeArgument}({string.Join(", ", arguments)});");
    }

    public static StatementSyntax CancellableReturnAwaitStatement(string? objectName, string methodName,
        string? typeArgument, params string[] arguments)
    {
        var argumentsList = arguments.ToList();
        argumentsList.Add("cancellationToken");
        return ReturnAwaitStatement(objectName, methodName, typeArgument, argumentsList.ToArray());
    }
}