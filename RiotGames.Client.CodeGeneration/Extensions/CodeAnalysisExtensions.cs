using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RiotGames.Client.CodeGeneration.RiotGamesApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static RiotGames.Client.CodeGeneration.CodeAnalysisHelper;


namespace RiotGames.Client.CodeGeneration
{
    using SF = SyntaxFactory;
    [DebuggerStepThrough]
    internal static class CodeAnalysisExtensions
    {
        #region Namespace

        public static NamespaceDeclarationSyntax AddUsing(this NamespaceDeclarationSyntax @namespace, string usingNamespace) =>
            @namespace.AddUsings(SF.UsingDirective(SF.ParseName(usingNamespace)));

        public static NamespaceDeclarationSyntax AddSystemTextJsonSerializationUsing(this NamespaceDeclarationSyntax @namespace) =>
            @namespace.AddUsing("System.Text.Json.Serialization");

        public static NamespaceDeclarationSyntax AddSystemDynamicUsing(this NamespaceDeclarationSyntax @namespace) =>
            @namespace.AddUsing("System.Dynamic");

        #endregion Namespace

        #region Class
        public static ClassDeclarationSyntax AddPublicAsyncTask(this ClassDeclarationSyntax classDeclaration, string returnType, string methodIdentifier, StatementSyntax bodyStatement, Dictionary<string, string>? parameters = null) =>
            classDeclaration.AddMembers(PublicAsyncTaskDeclaration(returnType, methodIdentifier, bodyStatement, parameters));

        public static ClassDeclarationSyntax AddBaseType(this ClassDeclarationSyntax classDeclaration, string baseTypeName) =>
            classDeclaration.AddBaseListTypes(SF.SimpleBaseType(SF.ParseTypeName(baseTypeName)));

        #endregion Class

        #region Constructor

        public static ConstructorDeclarationSyntax WithParameter(this ConstructorDeclarationSyntax constructor, string parameterType, string parameterIdentifier) =>
            constructor.AddParameterListParameters(Parameter(parameterType, parameterIdentifier));

        public static ConstructorDeclarationSyntax WithBody(this ConstructorDeclarationSyntax constructor, string bodyStatement) =>
            constructor.WithBody(SF.Block(SF.ParseStatement(bodyStatement)));

        public static ConstructorDeclarationSyntax WithBaseConstructorInitializer(this ConstructorDeclarationSyntax constructor, params string[] argumentIdentifiers) =>
            constructor.WithInitializer(SF.ConstructorInitializer(SyntaxKind.BaseConstructorInitializer)
                    .AddArgumentListArguments(argumentIdentifiers.Select(ai => SF.Argument(SF.IdentifierName(ai))).ToArray()));

        public static ConstructorDeclarationSyntax WithModifier(this ConstructorDeclarationSyntax constructor, SyntaxKind modifier) =>
            constructor.AddModifiers(Token(modifier));

        #endregion Constructor

        #region Enum

        public static EnumDeclarationSyntax AddJsonStringEnumAttribute(this EnumDeclarationSyntax @enum) =>
            @enum.AddAttribute(Attribute("JsonStringEnum"));

        #endregion Enum

        #region Attribute

        public static T AddAttribute<T>(this T member, AttributeSyntax attribute) where T : MemberDeclarationSyntax =>
            (T)member.AddAttributeLists(SF.AttributeList(SF.SingletonSeparatedList<AttributeSyntax>(attribute)));

        #endregion Attribute

        #region Property

        public static PropertyDeclarationSyntax AddJsonPropertyNameAttribute(this PropertyDeclarationSyntax property, string name) =>
            property.AddAttribute(Attribute("JsonPropertyName", name));

        #endregion Property

        #region Field

        public static FieldDeclarationSyntax AddModifier(this FieldDeclarationSyntax fieldDeclaration, SyntaxKind modifier) =>
            fieldDeclaration.AddModifiers(SF.Token(modifier));

        #endregion Field

        #region Parameter

        public static ParameterSyntax[] ToParameters(this IEnumerable<KeyValuePair<string, string>> parameters) =>
            parameters.Select(p => Parameter(p.Value, p.Key)).ToArray();

        #endregion Parameter

    }
}
