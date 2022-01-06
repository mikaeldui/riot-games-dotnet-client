using Microsoft.CodeAnalysis.CSharp.Syntax;
using MingweiSamuel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.Client.CodeGeneration
{
    internal abstract class OpenApiEndpointGeneratorBase<TPathObject, TGetMethodObject, TPostMethodObject, TPutMethodObject, TParameter, TSchema>
        where TPathObject : OpenApiPathObject<TGetMethodObject, TPostMethodObject, TPutMethodObject, TParameter, TSchema>
        where TGetMethodObject : OpenApiMethodObject<TParameter, TSchema>
        where TPostMethodObject : OpenApiMethodObject<TParameter, TSchema>
        where TPutMethodObject : OpenApiMethodObject<TParameter, TSchema>
        where TParameter : OpenApiParameterObject
        where TSchema : OpenApiSchemaObject
    {
        protected ClassDeclarationSyntax ClassDeclaration;

        protected OpenApiEndpointGeneratorBase(string className, bool partialClass = false)
        {
            if (partialClass)
                ClassDeclaration = ClassHelper.CreatePublicPartialClass(className);
            else
                ClassDeclaration = ClassHelper.CreatePublicClass(className);
        }

        protected void AddEndpoint(EndpointDefinition? endpoint)
        {
            if (endpoint == null)
                return;

            //// Long time since I did an XOR, this might not work.
            //if (httpMethod == HttpMethod.Get ^ requestType == null)
            //    throw new ArgumentException("If the httpMetod is get then requestType must be null. If it is post or put then it must be set.");

            StatementSyntax bodyStatement;
            if (endpoint.RequestValueType == null)
                bodyStatement = StatementHelper.ReturnAwait(endpoint.HttpClientIdentifier, endpoint.HttpClientMethod, endpoint.HttpReturnType, endpoint.RequestUri);
            else
                bodyStatement = StatementHelper.ReturnAwait(endpoint.HttpClientIdentifier, endpoint.HttpClientMethod, endpoint.HttpReturnType, endpoint.RequestUri, "value");

            ClassDeclaration = ClassDeclaration.AddPublicAsyncTask(endpoint.ReturnTypeName, endpoint.Identifier.EndWith("Async"), bodyStatement, endpoint.RequestPathParameters);
        }

        public virtual void AddPathAsEndpoints(KeyValuePair<string, TPathObject> path)
        {
            var pathObject = path.Value;
            if (pathObject.Get != null)
                AddEndpoint(GetMethodObjectToEndpointDefinition(pathObject.Get, path.Key, pathObject));

            //if (pathObject.Post != null)
            //    AddEndpoint(PostMethodObjectToEndpointDefinition(pathObject.Post, path.Key, pathObject));

            //if (pathObject.Put != null)
            //    AddEndpoint(PutMethodObjectToEndpointDefinition(pathObject.Put, path.Key, pathObject));
        }

        public virtual void AddPathsAsEndpoints(IEnumerable<KeyValuePair<string, TPathObject>> paths)
        {
            foreach (var path in paths)
                AddPathAsEndpoints(path);
        }

        protected abstract EndpointDefinition? GetMethodObjectToEndpointDefinition(TGetMethodObject getMethodObject, string path, TPathObject pathObject);
        protected abstract EndpointDefinition? PostMethodObjectToEndpointDefinition(TPostMethodObject postMethodObject, string path, TPathObject pathObject);
        protected abstract EndpointDefinition? PutMethodObjectToEndpointDefinition(TPutMethodObject putMethodObject, string path, TPathObject pathObject);

        //protected virtual Dictionary<string, string>? ToPathParameters(OpenApiParameterObject[] parameters)
        //{
        //    if (parameters.Length == 0)
        //        return null;

        //    if (!parameters.All(p => p.In is "path" or "header" or "query"))
        //        Debugger.Break();

        //    return parameters.Where(p => p.In is not "header" and not "query").ToDictionary(p => p.Name, p => p.Schema.XType ?? p.Schema.Type);
        //}

        protected virtual string GetRiotHttpClientMethod(HttpMethod httpMethod, string returnType, ref string? typeArgument)
        {
            returnType = returnType.Replace("object", "dynamic");

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

            return "Get" + specificMethod + "Async";
        }

        public abstract string GenerateCode();

        protected class EndpointDefinition
        {
            public string Identifier;
            public string ReturnTypeName;
            public string? HttpClientIdentifier;
            public string HttpClientMethod;
            public string HttpReturnType;
            public string RequestUri;
            public string? RequestValueType;
            public Dictionary<string, string>? RequestPathParameters = null;

            public EndpointDefinition(string identifier, string returnTypeName, string? httpClientIdentifier, string httpClientMethod, string httpReturnType, string requestUri, string? requestValueType, Dictionary<string, string>? requestPathParameters)
            {
                Identifier = identifier;
                ReturnTypeName = returnTypeName;
                HttpClientIdentifier = httpClientIdentifier;
                HttpClientMethod = httpClientMethod;
                HttpReturnType = httpReturnType;
                RequestUri = requestUri;
                RequestValueType = requestValueType;
                RequestPathParameters = requestPathParameters;
            }
        }
    }
}
