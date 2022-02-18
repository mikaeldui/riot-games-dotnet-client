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
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static RiotGames.Client.CodeGeneration.CodeAnalysisHelper;

namespace RiotGames.Client.CodeGeneration.LeagueClient
{
    using LcuPathObject = OpenApiPathObject<OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, LcuParameterObject, LcuSchemaObject>;
    using LcuMethodObject = OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>;

    using Path = KeyValuePair<string, OpenApiPathObject<OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, LcuParameterObject, LcuSchemaObject>>;
    using Paths = IEnumerable<KeyValuePair<string, OpenApiPathObject<OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, LcuParameterObject, LcuSchemaObject>>>;
    using ClientHelper = LeagueClientEndpointsHelper;

    internal class LeagueClientEndpointsGenerator : OpenApiEndpointGeneratorBase<LcuPathObject, LcuMethodObject, LcuMethodObject, LcuMethodObject, LcuParameterObject, LcuSchemaObject>
    {
        protected const string LEAGUECLIENT_CLASS_IDENTIFIER = "LeagueClient";
        protected const string LEAGUECLIENTBASE_CLASS_IDENTIFIER = "LeagueClientBase";

        public readonly string ClassName;

        protected string[] Enums;

        private readonly List<MemberDeclarationSyntax[]> _moduleProperties = new();
        private readonly List<ClassDeclarationSyntax> _moduleClasses = new();
        private IGrouping<string, KeyValuePair<string, string>>? _eventGroup;

        public LeagueClientEndpointsGenerator(string[] enums) : this(LEAGUECLIENT_CLASS_IDENTIFIER, enums) { }

        private LeagueClientEndpointsGenerator(string className, string[] enums, bool partialClass = true) : base(className, partialClass)
        {
            ClassName = className;
            Enums = enums;
        }

        protected override void AddEndpoint(EndpointDefinition? endpoint)
        {
            base.AddEndpoint(endpoint);

            if (endpoint == null)
                return;

            // Maybe we have a basic interface and can add an overlaod.
            if (endpoint.Identifier.StartsWith("Get") && endpoint.RequestPathParameters != null && endpoint.RequestPathParameters.Count == 1)
            {
                var parameter = endpoint.RequestPathParameters.Single();

                if (LeagueClientHacks.BasicInterfaces.TryGetBasicInterfaceIdentifier("LeagueClient", parameter.Value + "?", parameter.Key.ToPascalCase(), out string interfaceIdentifier))
                {
                    string methodIdentifier = endpoint.Identifier;
                    string parameterIdentifier = interfaceIdentifier[1..].RemoveEnd("Id");

                    if (endpoint.Identifier.Contains("By"))
                    {
                        var methodIdentifierParts = methodIdentifier.Split("By");
                        methodIdentifier = methodIdentifierParts[0];
                        parameterIdentifier = methodIdentifierParts[1];
                    }

                    var method = CancellablePublicAsyncTaskDeclaration(endpoint.ReturnTypeName, methodIdentifier,
                        CancellableReturnAwaitStatement(null, endpoint.Identifier.EndWith("Async"), null, parameterIdentifier.ToCamelCase() + '.' + parameter.Key.ToPascalCase()),
                        new Dictionary<string, string> { { parameterIdentifier.ToCamelCase(), interfaceIdentifier } });



                    Class = Class.AddMembers(method);
                }
            }
        }

        private void _addGroupsAsNestedClassesWithEndpoints(IEnumerable<IGrouping<string?, Path>> groupedPaths, IEnumerable<IGrouping<string, KeyValuePair<string, string>>>? groupedEvents, string? className = null)
        {
            foreach (var group in groupedPaths.Where(g => g.Key != null))
            {
                var eventGroup = groupedEvents?.SingleOrDefault(g => g.Key == group.Key);

                bool versionSuffix = false;
                var versioned = group.GroupBy(P =>
                {
                    var secondPart = P.Key.SplitAndRemoveEmptyEntries('/')[1];
                    if (secondPart[0] == 'v' && char.IsDigit(secondPart[1]))
                        return secondPart;
                    else return null;
                });

                var versionedEvents = eventGroup?.GroupBy(e =>
                {
                    var thirdPart = e.Key.SplitAndRemoveEmptyEntries('_')[2];
                    if (thirdPart[0] == 'v' && char.IsDigit(thirdPart[1]))
                        return thirdPart;
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
                        foreach (var versionedModule in versioned)
                        {
                            var versionedModuleEvents = versionedEvents?.Where(ve => ve.Key == versionedModule.Key);
                            _addGroupsAsNestedClassesWithEndpoints(versionedModule.GroupByModule(), versionedModuleEvents?.SelectMany(e => e)?.GroupByModule(),
                                group.Key.RemoveChars('{', '}').ToPascalCase() + versionedModule.Key?.ToUpper());
                        }

                        continue;
                    }
                }

                var moduleGenerator = new LeagueClientModuleGenerator((className ?? group.Key.RemoveChars('{', '}').ToPascalCase())
                    .FixGamePrefixes(), Enums, methodVersionSuffix: versionSuffix, generateConstructor: true);

                moduleGenerator.AddPathsAsEndpoints(group, eventGroup);
                _moduleProperties.Add(moduleGenerator.FieldAndProperty);
                _moduleClasses.Add(moduleGenerator.Class);
                Console.WriteLine($"League Client: Generated client module for module {moduleGenerator.ModuleName}.");
            }
        }

        public virtual void AddGroupsAsNestedClassesWithEndpoints(IEnumerable<IGrouping<string?, Path>> groupedPaths, IEnumerable<IGrouping<string, KeyValuePair<string, string>>> groupedEvents, string? className = null)
        {
            // Root group
            var nullGroup = groupedPaths.FirstOrDefault(g => g.Key == null);

            if (nullGroup != null)
                AddPathsAsEndpoints(nullGroup);

            {
                var lolGroups = groupedPaths.Where(g => g is { Key: not null, Key: not "lol-tft" } && g.Key.StartsWith("lol-"));
                var lolEvents = groupedEvents.Where(g => g.Key.StartsWith("lol-"));
                var generator = new LeagueClientEndpointsGenerator("LeagueOfLegendsClient", Enums, true);
                generator.Class = generator.Class.AddMembers(
                    InternalConstructorDeclaration("LeagueOfLegendsClient")
                    .WithBody(Block())
                    .WithParameter(LEAGUECLIENTBASE_CLASS_IDENTIFIER, "leagueClient")
                    .WithBaseConstructorInitializer("leagueClient.HttpClient", "leagueClient.EventRouter"));
                generator._addGroupsAsNestedClassesWithEndpoints(lolGroups, lolEvents);
                generator._addNestedMembersToClass();
                Class = Class.AddMembers(generator.Class);
            }

            {
                var tftPaths = groupedPaths.Single(g => g is { Key: "lol-tft" });
                var generator = new LeagueClientModuleGenerator("TeamfightTactics", Enums, false, false);
                generator.Class = generator.Class.AddMembers(
                    InternalConstructorDeclaration("TeamfightTacticsClient")
                    .WithBody(Block())
                    .WithParameter(LEAGUECLIENTBASE_CLASS_IDENTIFIER, "leagueClient")
                    .WithBaseConstructorInitializer("leagueClient.HttpClient", "leagueClient.EventRouter"));
                generator.AddPathsAsEndpoints(tftPaths);
                Class = Class.AddMembers(generator.Class.AddModifiers(Token(SyntaxKind.PartialKeyword)));
            }

            groupedPaths = groupedPaths.Where(g => g is { Key: not null, Key: not "lol-tft" } && !g.Key.StartsWith("lol-"));

            _addGroupsAsNestedClassesWithEndpoints(groupedPaths, null, className);
        }

        protected override EndpointDefinition? GetMethodObjectToEndpointDefinition(LcuMethodObject getMethodObject, string path, LcuPathObject pathObject)
        {
            OpenApiResponseObject<LcuSchemaObject>? response200 = null;
            getMethodObject?.Responses?.TryGetValue("200", out response200);
            if (response200 == null)
                return null; //TODO: Implement response 204.
            var responseSchema = response200.Content?.First().Value.Schema;
            var isArrayResponse = responseSchema?.Type == "array";
            var nameFromPath =  GetNameFromPath(path, isArrayResponse);

            var pathParameters = ToPathParameters(getMethodObject?.Parameters);

            var returnType = LeagueClientEndpointsHelper.GetTypeName(responseSchema);
            var typeArgument = TypeArgumentStatement(returnType.Remove("[]"));
            var baseMethod = Enums.Contains(returnType) ? "GetEnumAsync" : GetRiotHttpClientMethod(HttpMethod.Get, returnType, ref typeArgument);

            if (pathParameters != null)
            {
                pathParameters.ReplaceKeys(LeagueClientHacks.ReservedIdentifiers);

                // Add "$" so it can be interpolated.
                path = $"$\"{path.Replace(LeagueClientHacks.ReservedPathParameters)}\"";
            }
            else
                path = "\"" + path + "\"";

            if (_eventGroup != null && _eventGroup.Any(e => e.Key.EndsWith(path.SplitAndRemoveEmptyEntries('/').Last().TrimEnd('\"'))))
            {
                var @event = _eventGroup.First(e => e.Key.EndsWith(path.SplitAndRemoveEmptyEntries('/').Last().TrimEnd('\"')));
                Class = Class.AddMembers(LeagueClientEvent.RmsEvent(@event.Key, nameFromPath, returnType));
            }

            return new EndpointDefinition("Get" + nameFromPath, returnType, "HttpClient", baseMethod, typeArgument, path, null, pathParameters);
        }

        protected override EndpointDefinition? PostMethodObjectToEndpointDefinition(LcuMethodObject postMethodObject, string path, LcuPathObject pathObject) => throw new NotImplementedException();

        protected override EndpointDefinition? PutMethodObjectToEndpointDefinition(LcuMethodObject putMethodObject, string path, LcuPathObject pathObject) => throw new NotImplementedException();

        protected virtual string GetNameFromPath(string path, bool isArrayResponse) => ClientHelper.GetNameFromPath(path, isArrayResponse);

        private void _addNestedMembersToClass()
        {
            var nestedMembers = _moduleProperties.SelectMany(ps => ps).Select(p => (MemberDeclarationSyntax)p).Concat(_moduleClasses).ToArray();

            Class = Class.AddMembers(nestedMembers);
        }

        public override string GenerateCode()
        {
            _addNestedMembersToClass();
            var @namespace = NamespaceDeclaration("RiotGames.LeagueOfLegends.LeagueClient");
            @namespace = @namespace.AddSystemDynamicUsing();
            @namespace = @namespace.AddUsing("RiotGames.Messaging");

            @namespace = @namespace.AddMembers(Class);

            // Normalize and get code as string.
            return @namespace
                .NormalizeWhitespace()
                .ToFullString();
        }

        protected virtual Dictionary<string, string>? ToPathParameters(LcuParameterObject[] parameters)
        {
            if (parameters.Length == 0)
                return null;

            if (!parameters.All(p => p.In is "path" or "header" or "query"))
                Debugger.Break();

#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
            return parameters
                .GroupBy(p => p.Name).Select(g => g.First())
                .Where(p => p.In is not "header" and not "query")
                .ToDictionary(p => p.Name, p => p.GetTypeName());
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.

        }


        internal class LeagueClientModuleGenerator : LeagueClientEndpointsGenerator
        {
            public readonly string ModuleName;

            private readonly FieldDeclarationSyntax _fieldDeclaration;

            private readonly PropertyDeclarationSyntax _propertyDeclaration;

            protected bool MethodVersionSuffix;
            protected Dictionary<string, string?[]>? VersionsByPaths;

            public LeagueClientModuleGenerator(string moduleName, string[] enums, bool methodVersionSuffix, bool generateConstructor) : base(moduleName + "Client", enums, partialClass: false) // For now, adding Client suffix.
            {
                ModuleName = moduleName.ToPascalCase();
                MethodVersionSuffix = methodVersionSuffix;

                if (generateConstructor)
                {
                    FieldDeclarationSyntax httpClientField = InternalReadOnlyFieldDeclaration("LeagueClientHttpClient", "HttpClient");
                    Class = Class.AddMembers(httpClientField);

                    var constructor = InternalConstructorDeclaration(ClassName, LEAGUECLIENTBASE_CLASS_IDENTIFIER, "leagueClient", "HttpClient", "HttpClient");
                    Class = Class.AddMembers(constructor);
                }

                string fieldName = "_" + moduleName.ToCamelCase();
                _fieldDeclaration = PrivateFieldDeclaration(ClassName, fieldName);
                _propertyDeclaration = FieldBackedPublicReadOnlyPropertyDeclaration(ClassName, ModuleName, fieldName, "this");
            }

            public override void AddPathsAsEndpoints(Paths paths)
            {
                if (MethodVersionSuffix)
                {
                    VersionsByPaths = paths
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
                }

                base.AddPathsAsEndpoints(paths);
            }

            protected override string GetNameFromPath(string path, bool isArrayResponse)
            {
                string? methodNameSuffix = null;

                if (MethodVersionSuffix)
                {
                    if (VersionsByPaths == null)
                        throw new Exception("VersionsByPaths isn't set!");

                    var pathVersions = VersionsByPaths[string.Join('/', path.SplitAndRemoveEmptyEntries('/').Skip(2))];
                    if (pathVersions.Length != 1)
                    {
                        var thisPathVersion = pathVersions.Where(pv => pv != null).SingleOrDefault(pv => path.Contains(pv));
                        methodNameSuffix = thisPathVersion;
                    }
                }

                return base.GetNameFromPath(path, isArrayResponse) + methodNameSuffix;
            }

            public MemberDeclarationSyntax[] FieldAndProperty => new MemberDeclarationSyntax[] { _fieldDeclaration, _propertyDeclaration };

            public override void AddGroupsAsNestedClassesWithEndpoints(IEnumerable<IGrouping<string?, Path>> groupedPaths, IEnumerable<IGrouping<string, KeyValuePair<string, string>>> groupedEvents, string? className = null) => throw new NotImplementedException();

            public override string GenerateCode() => throw new NotImplementedException();

            internal void AddPathsAsEndpoints(IGrouping<string?, Path> group, IGrouping<string, KeyValuePair<string, string>>? eventGroup)
            {
                _eventGroup = eventGroup;
                AddPathsAsEndpoints(group);
            }
        }
    }
}
