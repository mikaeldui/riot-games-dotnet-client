using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RiotGames.Client.CodeGeneration
{
    internal enum Client
    {
        RiotGames,
        LeagueOfLegends,
        LegendsOfRuneterra,
        TeamfightTactics,
        Valorant
    }
    internal class ClientGenerator
    {
        NamespaceDeclarationSyntax _namespace;
        ClassDeclarationSyntax _classDeclaration;

        public ClientGenerator(Client client)
        {
            _namespace = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(string.Join('.', new string[] { "RiotGames", client.ToString() }.Distinct()))).NormalizeWhitespace();

            // Add System using statement: (using System)
            _namespace = _namespace.AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")));

            //  Create a class: (class RiotGamesClient)
            _classDeclaration = SyntaxFactory.ClassDeclaration($"{client}Client");

            // Add the public modifier: (public partial class RiotGamesClient)
            _classDeclaration = _classDeclaration.AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.PartialKeyword));

            //// Inherit BaseEntity<T> and implement IHaveIdentity: (public class Order : BaseEntity<T>, IHaveIdentity)
            //classDeclaration = classDeclaration.AddBaseListTypes(
            //    SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("BaseEntity<Order>")),
            //    SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("IHaveIdentity")));
        }

        public void AddEndpoint(string name, HttpMethod httpMethod, string requestUri, string returnType, string? requestType = null, Dictionary<string, string>? parameters = null)
        {
            // Long time since I did an XOR, this might not work.
            if (httpMethod == HttpMethod.Get ^ requestType == null)
                throw new ArgumentException("If the httpMetod is get then requestType must be null. If it is post or put then it must be set.");


            // Create a stament with the body of a method.
            StatementSyntax syntax;
            {
                string baseMethod = httpMethod.ToString().ToPascalCase() + "Async";

                if (httpMethod == HttpMethod.Get)
                    syntax = SyntaxFactory.ParseStatement($"return await {baseMethod}<{returnType}>(\"{requestUri}\");");
                else
                    syntax = SyntaxFactory.ParseStatement($"return await {baseMethod}<{requestType}, {returnType}>(\"{requestUri}\", value);");
            }

            // Create a method
            var methodDeclaration = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("Task<" + returnType + ">"), name.EndsWith("Async") ? name : name + "Async")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.AsyncKeyword))
                .WithBody(SyntaxFactory.Block(syntax));

            if (parameters != null)
                foreach (var parameter in parameters)
                {
                    methodDeclaration = methodDeclaration.AddParameterListParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier(parameter.Key)).WithType(SyntaxFactory.ParseTypeName(parameter.Value)));
                }

            _classDeclaration = _classDeclaration.AddMembers(methodDeclaration);
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
