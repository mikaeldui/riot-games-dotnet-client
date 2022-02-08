using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static RiotGames.Client.CodeGeneration.CodeAnalysisHelper;

namespace RiotGames.Client.CodeGeneration.LeagueClient
{
    internal static class LeagueClientMethod
    {
        public static MethodDeclarationSyntax Rest(string returnTypeName, string methodIdentifier, string parameterIdentifier,
            string endpointIdentifier, string parameterKey, string interfaceIdentifier) =>
            CancellablePublicAsyncTaskDeclaration(returnTypeName, methodIdentifier,
                        CancellableReturnAwaitStatement(null, endpointIdentifier.EndWith("Async"), null, parameterIdentifier.ToCamelCase() + '.' + parameterKey.ToPascalCase()),
                        new Dictionary<string, string> { { parameterIdentifier.ToCamelCase(), interfaceIdentifier } });
    }
}

namespace RiotGames.Client.CodeGeneration
{
    internal class QueryParameter
    {
        public QueryParameter(string type, string identifier)
        {
            Type = type;
            Identifier = identifier;
        }

        public string Type { get; set; }
        public string Identifier { get; set; }

        public static implicit operator ParameterSyntax(QueryParameter p) => Parameter(p.Type, p.Identifier);
    }

    internal class OptionalQueryParameter : QueryParameter
    {
        public OptionalQueryParameter(string type, string identifier, string defaultValue = "default") : base(type, identifier) => 
            DefaultValue = defaultValue;

        public string DefaultValue { get; set; }

        public static implicit operator ParameterSyntax(OptionalQueryParameter p) => 
            Parameter(default, default, ParseTypeName(p.Type.EndWith("?")), Identifier(p.Identifier), EqualsValueClause(ParseExpression(p.DefaultValue)));
    }
}
