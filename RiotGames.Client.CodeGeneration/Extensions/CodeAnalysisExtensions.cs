using System.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RiotGames.Client.CodeGeneration;

using static CodeAnalysisHelper;

[DebuggerStepThrough]
internal static class CodeAnalysisExtensions
{
    public static NamespaceDeclarationSyntax AddSystemTextJsonSerializationUsing(
        this NamespaceDeclarationSyntax @namespace)
    {
        return @namespace.AddUsing("System.Text.Json.Serialization");
    }

    public static NamespaceDeclarationSyntax AddSystemDynamicUsing(this NamespaceDeclarationSyntax @namespace)
    {
        return @namespace.AddUsing("System.Dynamic");
    }

    public static EnumDeclarationSyntax AddJsonStringEnumAttribute(this EnumDeclarationSyntax @enum)
    {
        return @enum.AddAttribute("JsonStringEnum");
    }

    public static PropertyDeclarationSyntax AddJsonPropertyNameAttribute(this PropertyDeclarationSyntax property,
        string name)
    {
        return property.AddAttribute("JsonPropertyName", name);
    }

    public static ClassDeclarationSyntax AddPublicAsyncTask(this ClassDeclarationSyntax @class, string returnType,
        string methodIdentifier, StatementSyntax bodyStatement, Dictionary<string, string>? parameters = null)
    {
        return @class.AddMembers(PublicAsyncTaskDeclaration(returnType, methodIdentifier, bodyStatement, parameters));
    }

    public static ClassDeclarationSyntax AddCancellablePublicAsyncTask(this ClassDeclarationSyntax @class,
        string returnType, string methodIdentifier, StatementSyntax bodyStatement,
        Dictionary<string, string>? parameters = null)
    {
        return @class.AddMembers(
            CancellablePublicAsyncTaskDeclaration(returnType, methodIdentifier, bodyStatement, parameters));
    }
}