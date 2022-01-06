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
        protected bool VersionSuffix;

        private List<MemberDeclarationSyntax[]> _moduleProperties = new();
        private List<ClassDeclarationSyntax> _moduleClasses = new();

        public LeagueClientEndpointsGenerator(string[] enums) : this(LEAGUECLIENT_CLASS_IDENTIFIER, enums) { }

        private LeagueClientEndpointsGenerator(string className, string[] enums, bool partialClass = true, bool versionSuffix = false)
        {
            ClassName = className;
            Enums = enums;
            VersionSuffix = versionSuffix;
            if (partialClass)
                ClassDeclaration = ClassHelper.CreatePublicPartialClass(className);
            else
                // For the nested classes.
                ClassDeclaration = ClassHelper.CreatePublicClass(className);
        }

        protected virtual void AddEndpoint(string methodIdentifier, HttpMethod httpMethod, string requestUri, string returnType, string? requestType = null, Dictionary<string, string>? pathParameters = null)
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

        protected virtual void AddPathAsEndpoints(Path path, string? methodNameSuffix = null)
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
                var nameFromPath = ClientHelper.GetNameFromPath(path.Key, isArrayReponse) + methodNameSuffix;

                Dictionary<string, string?>? pathParameters = null;

                if (poGet is { Parameters: not null, Parameters: { Length: > 0 } })
                {
                    if (!poGet.Parameters.All(p => p.In is "path" or "header" or "query"))
                        Debugger.Break();

                    pathParameters = poGet.Parameters
                        .GroupBy(p => p.Name).Select(g => g.First())
                        .Where(p => p.In is not "header" and not "query").ToDictionary(p => p.Name, p => p.GetTypeName());
                }

                AddEndpoint("Get" + nameFromPath, HttpMethod.Get, path.Key, responseSchema.GetTypeName(), pathParameters: pathParameters);
            }
        }

        protected void AddPathsAsEndpoints(Paths paths, bool methodVersionSuffix)
        {
            paths = paths.ToArray();

            Dictionary<string, string?[]>? versionsByPaths = null;
            if (methodVersionSuffix)
                versionsByPaths = paths
                    .Select(p =>
                    {
                        var version = p.Key.SplitAndRemoveEmptyEntries('/')[1];
                        if (version[0] != 'v' || !char.IsDigit(version[1]))
                            version = null;

                        return (version, string.Join('/', p.Key.SplitAndRemoveEmptyEntries('/').Skip(2)));
                    })
                    .GroupBy(t => t.Item2)
                    .ToDictionary(g => g.Key, g => g.Select(t => t.Item1).OrderBy(s => s)
                    .ToArray());

            foreach (var path in paths)
            {
                // Detect duplicated methods
                if (methodVersionSuffix)
                {
                    var pathVersions = versionsByPaths[string.Join('/', path.Key.SplitAndRemoveEmptyEntries('/').Skip(2))];
                    if (pathVersions.Length != 1)
                    {
                        var thisPathVersion = pathVersions.Where(pv => pv != null).SingleOrDefault(pv => path.Key.Contains(pv));
                        AddPathAsEndpoints(path, thisPathVersion);
                        continue;
                    }
                }

                AddPathAsEndpoints(path);
            }
        }

        public void AddGroupsAsNestedClassesWithEndpoints(IEnumerable<IGrouping<string?, Path>> groupedPaths, string? className = null)
        {
            var nullGroup = groupedPaths.FirstOrDefault(g => g.Key == null);

            if (nullGroup != null)
                AddPathsAsEndpoints(nullGroup, false);

            foreach (var group in groupedPaths.Where(g => g.Key != null))
            {
                bool versionSuffix = false;
                var versioned = group.GroupBy(P =>
                {
                    var secondPart = P.Key.SplitAndRemoveEmptyEntries('/')[1];
                    if (secondPart[0] == 'v' && char.IsDigit(secondPart[1]))
                        return secondPart;
                    else return null;
                });

                if (versioned.Count() > 1)
                {
                    var firstVersionCount = versioned.First().Count();
                    var secondVersionCount = versioned.ElementAt(1).Count();
                    if (Math.Abs(firstVersionCount - secondVersionCount) > (((float)Math.Max(firstVersionCount, secondVersionCount)) / 5) * 2)
                    {
                        // Just use a suffix for there methods
                        versionSuffix = true;
                    }
                    else
                    {
                        // Separate modules
                        foreach(var versionedModule in versioned)
                        {
                            AddGroupsAsNestedClassesWithEndpoints(versionedModule.GroupByModule(), 
                                group.Key.RemoveChars('{', '}').ToPascalCase() + versionedModule.Key?.ToUpper());
                        }

                        continue;
                    }
                }

                var moduleGenerator = new LeagueClientModuleGenerator(className ?? group.Key.RemoveChars('{', '}').ToPascalCase(), Enums, versionSuffix: versionSuffix);
                moduleGenerator.AddPathsAsEndpoints(group, versionSuffix);
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

            public LeagueClientModuleGenerator(string moduleName, string[] enums, bool versionSuffix) : base(moduleName + "Client", enums, partialClass: false, versionSuffix: versionSuffix) // For now, adding Client suffix.
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
