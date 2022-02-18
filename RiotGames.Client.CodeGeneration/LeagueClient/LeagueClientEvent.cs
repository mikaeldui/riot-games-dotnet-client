using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace RiotGames.Client.CodeGeneration.LeagueClient
{
    internal static class LeagueClientEvent
    {
        public static MemberDeclarationSyntax[] RmsEvent(string topic, string identifier, string typeName)
        {
            identifier = identifier.ReplaceStart("Get", "On");

            var privateEventIdentifier = '_' + identifier.ToCamelCase() + "Changed";
            var publicEventIdentifier = identifier.ToPascalCase() + "Changed";


            var privateEvent = EventFieldDeclaration(List<AttributeListSyntax>(), SyntaxKind.PrivateKeyword.ToTokenList(),
                SyntaxFactory.VariableDeclaration(
                    SyntaxFactory.ParseTypeName($"LeagueClientEventHandler<{typeName}>"),
                    SyntaxFactory.SeparatedList<VariableDeclaratorSyntax>(new[]
                        {
                            SyntaxFactory.VariableDeclarator(privateEventIdentifier)
                        }
                    )
                ));

            var publicEvent = EventFieldDeclaration(List<AttributeListSyntax>(), SyntaxKind.PublicKeyword.ToTokenList(),
                SyntaxFactory.VariableDeclaration(
                    SyntaxFactory.ParseTypeName($"LeagueClientEventHandler<{typeName}>"),
                    SyntaxFactory.SeparatedList<VariableDeclaratorSyntax>(new[]
                        {
                            SyntaxFactory.VariableDeclarator(identifier.ToPascalCase() + "Changed")
                        }
                    )
                ));

            var public2Event = EventDeclaration(ParseTypeName($"LeagueClientEventHandler<{typeName}>"),
                    Identifier(publicEventIdentifier))
                .WithModifiers(SyntaxKind.PublicKeyword.ToTokenList())
                .WithAccessorList(AccessorList(new SyntaxList<AccessorDeclarationSyntax>(new[]
                {
                    AccessorDeclaration(SyntaxKind.AddAccessorDeclaration, ParseStatement("if (" + privateEventIdentifier + " == null)\r\n" +
                        "    EventRouter.Subscribe(\"" + topic + "\",\r\n"+
                        "(LolChampSelectChampSelectSession args) => " + privateEventIdentifier + "?.Invoke(this, args));\r\n\r\n"+
                        "" + privateEventIdentifier + " += value;").ToBlock()),
                    AccessorDeclaration(SyntaxKind.RemoveAccessorDeclaration, ParseStatement("" + privateEventIdentifier + " -= value;\r\n\r\n"+
                        "if (" + privateEventIdentifier + " == null)\r\n"+
                        "    EventRouter.Unsubscribe(\"" + topic + "\");\r\n").ToBlock())
                })));

            return new MemberDeclarationSyntax[] {privateEvent, public2Event};
        }
    }
}
