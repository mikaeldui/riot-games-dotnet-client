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
    using ClientHelper = LeagueClientEndpointsHelper;

    internal class LeagueClientEndpointsGenerator
    {
        protected const string LEAGUECLIENT_CLASS_IDENTIFIER = "LeagueClient";
        protected string HttpClientIdentifier = "HttpClient";

        public readonly string ClassName;
        protected ClassDeclarationSyntax ClassDeclaration;

        protected string[] Enums;

        private List<MemberDeclarationSyntax[]> _moduleProperties = new();
        private List<ClassDeclarationSyntax> _moduleClasses = new();

        public LeagueClientEndpointsGenerator(string[] enums) : this(LEAGUECLIENT_CLASS_IDENTIFIER, enums) { }

        private LeagueClientEndpointsGenerator(string className, string[] enums, bool partialClass = true)
        {
            ClassName = className;
            Enums = enums;
            if (partialClass)
                ClassDeclaration = ClassHelper.CreatePublicPartialClass(className);
            else
                // For the nested classes.
                ClassDeclaration = ClassHelper.CreatePublicClass(className);
        }

        private void _addEndpoint(string methodIdentifier, HttpMethod httpMethod, string requestUri, string returnType, string? requestType = null, Dictionary<string, string>? pathParameters = null)
        {
            // Long time since I did an XOR, this might not work.
            if (httpMethod == HttpMethod.Get ^ requestType == null)
                throw new ArgumentException("If the httpMetod is get then requestType must be null. If it is post or put then it must be set.");

            // Create a stament with the body of a method.
            StatementSyntax bodyStatement;
            {
                returnType = returnType.Replace("object", "dynamic");

                string? typeArgument = httpMethod == HttpMethod.Get ? StatementHelper.TypeArgument(returnType.Remove("[]")) : StatementHelper.TypeArgument(requestType, returnType.Remove("[]"));
                string? specificMethod = null;

                // Current work-around, added some type specific HTTP methods.
                if (returnType.EndsWith("[]"))
                    if (returnType.StartsWith("string") || returnType.StartsWith("dynamic") || returnType.StartsWith("int") || returnType.StartsWith("long"))
                    {
                        specificMethod = "SystemType";
                        if (returnType.StartsWith("dynamic"))
                            typeArgument = StatementHelper.TypeArgument("ExpandoObject[]");
                        else
                            typeArgument = StatementHelper.TypeArgument(returnType);
                    }
                    else
                        specificMethod = "Array";
                else if (returnType is "int" or "string" or "bool" or "dynamic" or "long" or "double")
                {
                    specificMethod = "SystemType";

                    if (returnType == "dynamic")
                        typeArgument = StatementHelper.TypeArgument("ExpandoObject");
                }

                if (Enums.Contains(returnType))
                    specificMethod = "Enum";

                string baseMethod = httpMethod.ToString().ToPascalCase() + specificMethod + "Async";

                if (pathParameters != null)
                {
                    pathParameters.ReplaceKeys(LeagueClientHacks.ReservedIdentifiers);

                    // Add "$" so it can be interpolated.
                    requestUri = $"$\"{requestUri.Replace(LeagueClientHacks.ReservedPathParameters)}\"";
                }
                else
                    requestUri = "\"" + requestUri + "\"";


                if (httpMethod == HttpMethod.Get)
                    bodyStatement = StatementHelper.ReturnAwait(HttpClientIdentifier, baseMethod, typeArgument, requestUri);
                else
                    bodyStatement = StatementHelper.ReturnAwait(HttpClientIdentifier, baseMethod, typeArgument, requestUri, "value");            
            }

            ClassDeclaration = ClassDeclaration.AddPublicAsyncTask(returnType, methodIdentifier, bodyStatement, pathParameters);
        }

        private void _addPathAsEndpoints(Path path)
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
                        .Where(p => p.In is not "header" and not "query").ToDictionary(p => p.Name, p => p.GetTypeName());
                }

                _addEndpoint("Get" + nameFromPath, HttpMethod.Get, path.Key, responseSchema.GetTypeName(), pathParameters: pathParameters);
            }
        }

        protected void AddPathsAsEndpoints(Paths paths)
        {
            foreach (var path in paths)
                _addPathAsEndpoints(path);
        }

        public void AddGroupsAsNestedClassesWithEndpoints(IEnumerable<IGrouping<string?, Path>> groupedPaths)
        {
            var nullGroup = groupedPaths.First(g => g.Key == null);

            AddPathsAsEndpoints(nullGroup);

            foreach (var group in groupedPaths.Where(g => g.Key != null))
            {
                var moduleGenerator = new LeagueClientModuleGenerator(group.Key.RemoveChars('{', '}').ToPascalCase(), Enums);
                moduleGenerator.AddPathsAsEndpoints(group);
                _moduleProperties.Add(moduleGenerator.FieldAndProperty);
                _moduleClasses.Add(moduleGenerator.ClassDeclaration);
                Console.WriteLine($"League Client: Generated client module for module {moduleGenerator.ModuleName}.");
            }
        }

        public string GenerateCode()
        {
            var nestedMembers = _moduleProperties.SelectMany(ps => ps).Select(p => (MemberDeclarationSyntax)p).Concat(_moduleClasses).ToArray();

            ClassDeclaration = ClassDeclaration.AddMembers(nestedMembers);

            var @namespace = NamespaceHelper.CreateNamespaceDeclaration("RiotGames.LeagueOfLegends.LeagueClient");
            @namespace = @namespace.AddSystemDynamicUsing();

            @namespace = @namespace.AddMembers(ClassDeclaration);

            // Normalize and get code as string.
            return @namespace
                .NormalizeWhitespace()
                .ToFullString();
        }

        internal class LeagueClientModuleGenerator : LeagueClientEndpointsGenerator
        {
            public readonly string ModuleName;

            private readonly FieldDeclarationSyntax _fieldDeclaration;

            private readonly PropertyDeclarationSyntax _propertyDeclaration;

            public LeagueClientModuleGenerator(string moduleName, string[] enums) : base(moduleName + "Client", enums, partialClass: false) // For now, adding Client suffix.
            {
                ModuleName = moduleName.ToPascalCase();

                FieldDeclarationSyntax parentField = FieldHelper.CreatePrivateField(LEAGUECLIENT_CLASS_IDENTIFIER, "_parent");
                ClassDeclaration = ClassDeclaration.AddMembers(parentField);
                HttpClientIdentifier = "_parent." + HttpClientIdentifier;

                string fieldName = "_" + moduleName.ToCamelCase();
                _fieldDeclaration = FieldHelper.CreatePrivateField(ClassName, fieldName);
                _propertyDeclaration = PropertyHelper.CreateFieldBackedPublicReadOnlyProperty(ClassName, ModuleName, fieldName, "this");

                ClassDeclaration = ClassDeclaration.AddMembers(ClassHelper.CreateInternalConstructor(ClassName, LEAGUECLIENT_CLASS_IDENTIFIER, "leagueClient", "_parent"));
            }

            public MemberDeclarationSyntax[] FieldAndProperty => new MemberDeclarationSyntax[] { _fieldDeclaration, _propertyDeclaration };
        }
    }
}
