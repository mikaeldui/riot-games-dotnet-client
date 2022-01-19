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
    using static SyntaxFactory;
    using static CodeAnalysisHelper;

    [DebuggerStepThrough]
    internal static class CodeAnalysisExtensions
    {
        public static NamespaceDeclarationSyntax AddSystemTextJsonSerializationUsing(this NamespaceDeclarationSyntax @namespace) =>
            @namespace.AddUsing("System.Text.Json.Serialization");

        public static NamespaceDeclarationSyntax AddSystemDynamicUsing(this NamespaceDeclarationSyntax @namespace) =>
            @namespace.AddUsing("System.Dynamic");

        public static EnumDeclarationSyntax AddJsonStringEnumAttribute(this EnumDeclarationSyntax @enum) =>
            @enum.AddAttribute("JsonStringEnum");

        public static PropertyDeclarationSyntax AddJsonPropertyNameAttribute(this PropertyDeclarationSyntax property, string name) =>
            property.AddAttribute("JsonPropertyName", name);

        public static ClassDeclarationSyntax AddPublicAsyncTask(this ClassDeclarationSyntax @class, string returnType, string methodIdentifier, StatementSyntax bodyStatement, Dictionary<string, string>? parameters = null) =>
            @class.AddMembers(PublicAsyncTaskDeclaration(returnType, methodIdentifier, bodyStatement, parameters));

        public static ClassDeclarationSyntax AddCancellablePublicAsyncTask(this ClassDeclarationSyntax @class, string returnType, string methodIdentifier, StatementSyntax bodyStatement, Dictionary<string, string>? parameters = null) =>
            @class.AddMembers(CancellablePublicAsyncTaskDeclaration(returnType, methodIdentifier, bodyStatement, parameters));
    }
}
