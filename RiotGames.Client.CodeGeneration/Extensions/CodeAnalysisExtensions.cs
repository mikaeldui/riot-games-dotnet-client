﻿using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RiotGames.Client.CodeGeneration.RiotGamesApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.Client.CodeGeneration
{
    internal static class CodeAnalysisExtensions
    {
        public static ClassDeclarationSyntax AddPublicAsyncTask(this ClassDeclarationSyntax classDeclaration, string returnType, string methodIdentifier, StatementSyntax bodyStatement, Dictionary<string, string>? parameters = null)
        {
            var methodDeclaration = MethodHelper.PublicAsyncTask(returnType, methodIdentifier, bodyStatement, parameters);
            return classDeclaration.AddMembers(methodDeclaration);
        }

        public static PropertyDeclarationSyntax AddJsonPropertyNameAttribute(this PropertyDeclarationSyntax property, string name) =>
            property.AddAttributeLists(
                    SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList<AttributeSyntax>(
                        SyntaxFactory.Attribute(SyntaxFactory.IdentifierName("JsonPropertyName"))
                        .WithArgumentList(SyntaxFactory.ParseAttributeArgumentList($"(\"{name}\")")))));

        public static NamespaceDeclarationSyntax AddUsing(this NamespaceDeclarationSyntax @namespace, string usingNamespace) =>
            @namespace.AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(usingNamespace)));

        public static NamespaceDeclarationSyntax AddSystemTextJsonSerializationUsing(this NamespaceDeclarationSyntax @namespace) =>
            @namespace.AddUsing("System.Text.Json.Serialization");

        public static NamespaceDeclarationSyntax AddSystemDynamicUsing(this NamespaceDeclarationSyntax @namespace) =>
            @namespace.AddUsing("System.Dynamic");
    }
}
