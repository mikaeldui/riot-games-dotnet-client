using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;


namespace RiotGames.Client.CodeGeneration.LeagueClient.Generators
{
    internal class LcuClientGenerator
    {
        NamespaceDeclarationSyntax _namespace;
        ClassDeclarationSyntax _classDeclaration;

        public LcuClientGenerator()
        {
            _namespace = NamespaceHelper.CreateNamespaceDeclaration("RiotGames.LeagueOfLegends.LeagueClient");

            _classDeclaration = ClassHelper.CreatePublicPartialClass($"LeagueClient");
        }

        public void AddEndpoint(string methodIdentifier, bool isPlatform, HttpMethod httpMethod, string requestUri, string returnType, string? requestType = null, Dictionary<string, string>? pathParameters = null)
        {
            // Long time since I did an XOR, this might not work.
            if (httpMethod == HttpMethod.Get ^ requestType == null)
                throw new ArgumentException("If the httpMetod is get then requestType must be null. If it is post or put then it must be set.");

            // Create a stament with the body of a method.
            StatementSyntax bodyStatement;
            {
                string clientName = isPlatform ? "PlatformClient" : "RegionalClient";
                string? typeArgument = httpMethod == HttpMethod.Get ? StatementHelper.TypeArgument(returnType.Remove("[]")) : StatementHelper.TypeArgument(requestType, returnType.Remove("[]"));
                string? specificMethod = null;

                // Current work-around, added some type specific HTTP methods.
                if (returnType.EndsWith("[]"))
                    if (returnType.StartsWith("string"))
                    {
                        specificMethod = "StringArray";
                        typeArgument = null;
                    }
                    else
                        specificMethod = "Array";
                else if (returnType == "int")
                {
                    specificMethod = "Int";
                    typeArgument = null;
                }
                else if (returnType == "string")
                {
                    specificMethod = "String";
                    typeArgument = null;
                }

                string baseMethod = httpMethod.ToString().ToPascalCase() + specificMethod + "Async";

                // Add "$" if it's to be interpolated.
                string requestUriArgument = pathParameters == null ? $"\"{requestUri}\"" : $"$\"{requestUri}\"";

                if (httpMethod == HttpMethod.Get)
                    bodyStatement = StatementHelper.ReturnAwait(clientName, baseMethod, typeArgument, requestUriArgument);
                else
                    bodyStatement = StatementHelper.ReturnAwait(clientName, baseMethod, typeArgument, requestUriArgument, "value");
            }

            _classDeclaration = _classDeclaration.AddPublicAsyncTask(returnType, methodIdentifier, bodyStatement, pathParameters);
        }

        public string GenerateCode()
        {
            // Add the class to the namespace.
            _namespace = _namespace.AddMembers(_classDeclaration);

            // Normalize and get code as string.
            return _namespace
                .NormalizeWhitespace()
                .ToFullString();
        }
    }
}
