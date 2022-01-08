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
    using LcuPathObject = OpenApiPathObject<OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, LcuParameterObject, LcuSchemaObject>;
    using LcuMethodObject = OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>;

    using Path = KeyValuePair<string, OpenApiPathObject<OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, LcuParameterObject, LcuSchemaObject>>;
    using Paths = IEnumerable<KeyValuePair<string, OpenApiPathObject<OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, LcuParameterObject, LcuSchemaObject>>>;
    using ClientHelper = LeagueClientEndpointsHelper;

    internal class LeagueClientEndpointsGenerator : OpenApiEndpointGeneratorBase<LcuPathObject, LcuMethodObject, LcuMethodObject, LcuMethodObject, LcuParameterObject, LcuSchemaObject>
    {
        protected const string LEAGUECLIENT_CLASS_IDENTIFIER = "LeagueClient";
        protected string HttpClientIdentifier = "HttpClient";

        public readonly string ClassName;

        protected string[] Enums;

        private List<MemberDeclarationSyntax[]> _moduleProperties = new();
        private List<ClassDeclarationSyntax> _moduleClasses = new();

        public LeagueClientEndpointsGenerator(string[] enums) : this(LEAGUECLIENT_CLASS_IDENTIFIER, enums) { }

        private LeagueClientEndpointsGenerator(string className, string[] enums, bool partialClass = true) : base(className, partialClass)
        {
            ClassName = className;
            Enums = enums;
        }

        public virtual void AddGroupsAsNestedClassesWithEndpoints(IEnumerable<IGrouping<string?, Path>> groupedPaths, string? className = null)
        {
            var nullGroup = groupedPaths.FirstOrDefault(g => g.Key == null);

            if (nullGroup != null)
                AddPathsAsEndpoints(nullGroup);

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

                var moduleGenerator = new LeagueClientModuleGenerator(className ?? group.Key.RemoveChars('{', '}').ToPascalCase(), Enums, methodVersionSuffix: versionSuffix);

                moduleGenerator.AddPathsAsEndpoints(group);
                _moduleProperties.Add(moduleGenerator.FieldAndProperty);
                _moduleClasses.Add(moduleGenerator.ClassDeclaration);
                Console.WriteLine($"League Client: Generated client module for module {moduleGenerator.ModuleName}.");
            }
        }

        protected override EndpointDefinition? GetMethodObjectToEndpointDefinition(LcuMethodObject getMethodObject, string path, LcuPathObject pathObject)
        {
            OpenApiResponseObject<LcuSchemaObject>? response200 = null;
            getMethodObject?.Responses?.TryGetValue("200", out response200);
            if (response200 == null)
                return null; //TODO: Implement response 204.
            var responseSchema = response200.Content?.First().Value.Schema;
            bool isArrayResponse = responseSchema?.Type == "array";
            var nameFromPath =  GetNameFromPath(path, isArrayResponse);

            Dictionary<string, string>? pathParameters = ToPathParameters(getMethodObject?.Parameters);

            string returnType = responseSchema.GetTypeName();
            string? typeArgument = StatementHelper.TypeArgument(returnType.Remove("[]"));
            var baseMethod = Enums.Contains(returnType) ? "GetEnumAsync" : GetRiotHttpClientMethod(HttpMethod.Get, returnType, ref typeArgument);

            if (pathParameters != null)
                {
                    pathParameters.ReplaceKeys(LeagueClientHacks.ReservedIdentifiers);

                    // Add "$" so it can be interpolated.
                    path = $"$\"{path.Replace(LeagueClientHacks.ReservedPathParameters)}\"";
                }
                else
                    path = "\"" + path + "\"";

            return new EndpointDefinition("Get" + nameFromPath, returnType, HttpClientIdentifier, baseMethod, typeArgument, path, null, pathParameters);
        }

        protected override EndpointDefinition? PostMethodObjectToEndpointDefinition(LcuMethodObject postMethodObject, string path, LcuPathObject pathObject)
        {
            throw new NotImplementedException();
        }

        protected override EndpointDefinition? PutMethodObjectToEndpointDefinition(LcuMethodObject putMethodObject, string path, LcuPathObject pathObject)
        {
            throw new NotImplementedException();
        }

        protected virtual string GetNameFromPath(string path, bool isArrayResponse)
        {
            return ClientHelper.GetNameFromPath(path, isArrayResponse);
        }

        public override string GenerateCode()
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

        protected virtual Dictionary<string, string>? ToPathParameters(LcuParameterObject[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
                return null;

            if (!parameters.All(p => p.In is "path" or "header" or "query"))
                Debugger.Break();

#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            return parameters
                .GroupBy(p => p.Name).Select(g => g.First())
                .Where(p => p.In is not "header" and not "query")
                .ToDictionary(p => p.Name, p => p.GetTypeName());
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.

        }


        internal class LeagueClientModuleGenerator : LeagueClientEndpointsGenerator
        {
            public readonly string ModuleName;

            private readonly FieldDeclarationSyntax _fieldDeclaration;

            private readonly PropertyDeclarationSyntax _propertyDeclaration;

            protected bool MethodVersionSuffix;
            protected Dictionary<string, string?[]>? VersionsByPaths = null;

            public LeagueClientModuleGenerator(string moduleName, string[] enums, bool methodVersionSuffix) : base(moduleName + "Client", enums, partialClass: false) // For now, adding Client suffix.
            {
                ModuleName = moduleName.ToPascalCase();
                MethodVersionSuffix = methodVersionSuffix;

                FieldDeclarationSyntax parentField = FieldHelper.CreatePrivateField(LEAGUECLIENT_CLASS_IDENTIFIER, "_parent");
                ClassDeclaration = ClassDeclaration.AddMembers(parentField);
                HttpClientIdentifier = "_parent." + HttpClientIdentifier;

                string fieldName = "_" + moduleName.ToCamelCase();
                _fieldDeclaration = FieldHelper.CreatePrivateField(ClassName, fieldName);
                _propertyDeclaration = PropertyHelper.CreateFieldBackedPublicReadOnlyProperty(ClassName, ModuleName, fieldName, "this");

                ClassDeclaration = ClassDeclaration.AddMembers(ClassHelper.CreateInternalConstructor(ClassName, LEAGUECLIENT_CLASS_IDENTIFIER, "leagueClient", "_parent"));                            
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

            public override void AddGroupsAsNestedClassesWithEndpoints(IEnumerable<IGrouping<string?, Path>> groupedPaths, string? className = null) => throw new NotImplementedException();

            public override string GenerateCode() => throw new NotImplementedException();
        }
    }
}
