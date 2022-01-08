using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RiotGames.Client.CodeGeneration.RiotGamesApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.Client.CodeGeneration
{
    using SF = SyntaxFactory;
    [DebuggerStepThrough]
    internal static class CodeAnalysisExtensions
    {
        public static ClassDeclarationSyntax AddPublicAsyncTask(this ClassDeclarationSyntax classDeclaration, string returnType, string methodIdentifier, StatementSyntax bodyStatement, Dictionary<string, string>? parameters = null) =>
            classDeclaration.AddMembers(MethodHelper.PublicAsyncTask(returnType, methodIdentifier, bodyStatement, parameters));

        public static ClassDeclarationSyntax AddBaseType(this ClassDeclarationSyntax classDeclaration, string baseTypeName) =>
            classDeclaration.AddBaseListTypes(SF.SimpleBaseType(SF.ParseTypeName(baseTypeName)));

        public static T AddAttribute<T>(this T member, AttributeSyntax attribute) where T : MemberDeclarationSyntax =>
            (T) member.AddAttributeLists(SF.AttributeList(SF.SingletonSeparatedList<AttributeSyntax>(attribute)));

        public static PropertyDeclarationSyntax AddJsonPropertyNameAttribute(this PropertyDeclarationSyntax property, string name) =>
            property.AddAttribute(AttributeHelper.CreateAttribute("JsonPropertyName", name));

        public static EnumDeclarationSyntax AddJsonStringEnumAttribute(this EnumDeclarationSyntax @enum) =>
            @enum.AddAttribute(AttributeHelper.CreateAttribute("JsonStringEnum"));

        public static NamespaceDeclarationSyntax AddUsing(this NamespaceDeclarationSyntax @namespace, string usingNamespace) =>
            @namespace.AddUsings(SF.UsingDirective(SF.ParseName(usingNamespace)));

        public static NamespaceDeclarationSyntax AddSystemTextJsonSerializationUsing(this NamespaceDeclarationSyntax @namespace) =>
            @namespace.AddUsing("System.Text.Json.Serialization");

        public static NamespaceDeclarationSyntax AddSystemDynamicUsing(this NamespaceDeclarationSyntax @namespace) =>
            @namespace.AddUsing("System.Dynamic");
    }
}
