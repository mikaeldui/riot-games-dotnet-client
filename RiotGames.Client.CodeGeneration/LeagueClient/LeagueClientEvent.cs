using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace RiotGames.Client.CodeGeneration.LeagueClient;

internal static class LeagueClientEvent
{
    public static MemberDeclarationSyntax[] RmsEvent(string topic, string identifier, string typeName)
    {
        identifier = identifier.RemoveStart("Get");

        var privateEventIdentifier = '_' + identifier.ToCamelCase() + "Changed";
        var publicEventIdentifier = identifier.ToPascalCase() + "Changed";


        var privateEvent = EventFieldDeclaration(List<AttributeListSyntax>(), SyntaxKind.PrivateKeyword.ToTokenList(),
            VariableDeclaration(
                ParseTypeName($"LeagueClientEventHandler<{typeName}>"),
                SeparatedList(new[]
                    {
                        VariableDeclarator(privateEventIdentifier)
                    }
                )
            ));

        var publicEvent = EventDeclaration(ParseTypeName($"LeagueClientEventHandler<{typeName}>"),
                Identifier(publicEventIdentifier))
            .WithModifiers(SyntaxKind.PublicKeyword.ToTokenList())
            .WithAccessorList(AccessorList(new SyntaxList<AccessorDeclarationSyntax>(new[]
            {
                AccessorDeclaration(SyntaxKind.AddAccessorDeclaration, ParseStatement(
                        $"if ({privateEventIdentifier} == null) EventRouter.Subscribe(\"{topic}\", (RmsEventType eventType, {typeName} args) => {privateEventIdentifier}?.Invoke(this, eventType, args)); {privateEventIdentifier} += value;")
                    .ToBlock()),
                AccessorDeclaration(SyntaxKind.RemoveAccessorDeclaration, ParseStatement(
                        $"{privateEventIdentifier} -= value; if ({privateEventIdentifier} == null) EventRouter.Unsubscribe(\"{topic}\"); ")
                    .ToBlock())
            })));

        return new MemberDeclarationSyntax[] {privateEvent, publicEvent};
    }
}