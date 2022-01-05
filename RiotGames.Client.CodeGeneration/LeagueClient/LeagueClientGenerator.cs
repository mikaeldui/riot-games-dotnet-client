using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using MingweiSamuel;
using MingweiSamuel.Lcu;
using System.Diagnostics;

namespace RiotGames.Client.CodeGeneration.LeagueClient
{
    using Path = KeyValuePair<string, OpenApiPathObject<OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, LcuParameterObject, LcuSchemaObject>>;
    using Paths = IEnumerable<KeyValuePair<string, OpenApiPathObject<OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, LcuParameterObject, LcuSchemaObject>>>;
    using ClientHelper = LeagueClientHelper;

    internal class LeagueClientGenerator
    {
        private const string HTTPCLIENT_NAME = "HttpClient";

        ClassDeclarationSyntax _classDeclaration;
        public string? SubClass;

        public LeagueClientGenerator(string? subClass = null)
        {
            if (subClass == null)
                _classDeclaration = _leagueClientClass();
            else
            {
                SubClass = subClass.ToPascalCase();
                _classDeclaration = ClassHelper.CreatePublicClass(subClass + "Client");
            }
        }

        public void AddEndpoint(string methodIdentifier, HttpMethod httpMethod, string requestUri, string returnType, string? requestType = null, Dictionary<string, string>? pathParameters = null)
        {
            // Long time since I did an XOR, this might not work.
            if (httpMethod == HttpMethod.Get ^ requestType == null)
                throw new ArgumentException("If the httpMetod is get then requestType must be null. If it is post or put then it must be set.");

            // Create a stament with the body of a method.
            StatementSyntax bodyStatement;
            {
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

                string httpClient = HTTPCLIENT_NAME;

                if (SubClass != null)
                    httpClient = "_parent." + httpClient;

                if (httpMethod == HttpMethod.Get)
                    bodyStatement = StatementHelper.ReturnAwait(httpClient, baseMethod, typeArgument, requestUriArgument);
                else
                    bodyStatement = StatementHelper.ReturnAwait(httpClient, baseMethod, typeArgument, requestUriArgument, "value");
            }

            _classDeclaration = _classDeclaration.AddPublicAsyncTask(returnType, methodIdentifier, bodyStatement, pathParameters);
        }

        public void AddPathAsEndpoints(Path path)
        {
            //if (path.Key == "/lol/match/v5/matches/{matchId}/timeline") Debugger.Break();

            var po = path.Value;
            var poGet = po.Get;
            if (poGet != null)
            {
                OpenApiResponseObject<LcuSchemaObject>? response200 = null;
                poGet?.Responses?.TryGetValue("200", out response200);
                if (response200 == null)
                    return; //TODO: Implement response 204.
                var responseSchema = response200.Content?.First().Value.Schema;
                bool isArrayReponse = responseSchema?.Type == "array";
                var nameFromPath = ClientHelper.GetNameFromPath(path.Key, isArrayReponse);

                Dictionary<string, string?>? pathParameters = null;

                if (poGet is { Parameters: not null, Parameters: { Length: > 0 } })
                {
                    if (!poGet.Parameters.All(p => p.In is "path" or "header" or "query"))
                        Debugger.Break();

                    pathParameters = poGet.Parameters
                        .GroupBy(p => p.Name).Select(g => g.First())
                        .Where(p => p.In is not "header" and not "query").ToDictionary(p => p.Name, p => p.Type);
                }

                AddEndpoint("Get" + nameFromPath, HttpMethod.Get, path.Key, responseSchema.GetTypeName(), pathParameters: pathParameters);
            }
        }

        public void AddPathsAsEndpoints(Paths paths)
        {
            foreach (var path in paths)
                AddPathAsEndpoints(path);
        }

        private ClassDeclarationSyntax _leagueClientClass() => ClassHelper.CreatePublicPartialClass($"LeagueClient");

        public string GenerateCode()
        {
            var @namespace = NamespaceHelper.CreateNamespaceDeclaration("RiotGames.LeagueOfLegends.LeagueClient");

            if (SubClass == null)
                @namespace = @namespace.AddMembers(_classDeclaration);
            else
            {
                var rootClass = _leagueClientClass();

                string fieldName = "_" + SubClass.ToCamelCase();
                FieldDeclarationSyntax subClassField = FieldHelper.CreatePrivateField(SubClass + "Client", fieldName);
                PropertyDeclarationSyntax subClassProperty = PropertyHelper.CreateFieldBackedPublicReadOnlyProperty(SubClass + "Client", SubClass, fieldName, "this");

                FieldDeclarationSyntax parentField = FieldHelper.CreatePrivateField("LeagueClient", "_parent");

                _classDeclaration = _classDeclaration.AddMembers(parentField);
                _classDeclaration = _classDeclaration.AddMembers(ClassHelper.CreateInternalConstructor(SubClass + "Client", "LeagueClient", "leagueClient", "_parent"));

                rootClass = rootClass.AddMembers(subClassField, subClassProperty, _classDeclaration);

                @namespace = @namespace.AddMembers(rootClass);
            }

            // Normalize and get code as string.
            return @namespace
                .NormalizeWhitespace()
                .ToFullString();
        }
    }
}
