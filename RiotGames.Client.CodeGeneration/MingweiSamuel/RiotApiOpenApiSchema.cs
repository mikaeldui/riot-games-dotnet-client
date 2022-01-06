using System.Diagnostics;
using System.Text.Json.Serialization;

namespace MingweiSamuel.RiotApi
{
    [DebuggerDisplay("Paths = {Paths.Count} Components.Schema = {Components.Schemas.Count}")]
    internal class RiotApiOpenApiSchema : 
        OpenApiSchema<RiotApiPathObject, RiotApiMethodObject, RiotApiPostMethodObject, 
            RiotApiPutMethodObject, RiotApiParameterObject, RiotApiSchemaObject, 
            RiotApiComponentsObject, RiotApiComponentSchemaObject, RiotApiComponentPropertyObject>
    {
        public Dictionary<string, object[]>[]? Security { get; set; }
    }

    #region Path

    [DebuggerDisplay("X-Endpoint = {XEndpoint}")]
    internal class RiotApiPathObject : OpenApiPathObject<RiotApiMethodObject, RiotApiPostMethodObject, RiotApiPutMethodObject, RiotApiParameterObject,
        RiotApiSchemaObject>
    {
        [JsonPropertyName("x-endpoint")] public string? XEndpoint { get; set; }
        [JsonPropertyName("x-platforms-available")] public string[]? XPlatformsAvailable { get; set; }
        [JsonPropertyName("x-route-enum")] public string? XRouteEnum { get; set; }
    }

    [DebuggerDisplay("OperationId = {OperationId}")]
    internal class RiotApiMethodObject : OpenApiMethodObject<RiotApiParameterObject, RiotApiSchemaObject>
    {
        [JsonPropertyName("x-nullable-404")] public bool XNullable404 { get; set; }
        [JsonPropertyName("x-endpoint")] public string? XEndpoint { get; set; }
        [JsonPropertyName("x-platforms-available")] public string[]? XPlatformsAvailable { get; set; }
        [JsonPropertyName("x-route-enum")] public string? XRouteEnum { get; set; }
        public string? Summary { get; set; }
        public ExternalDocsObject? ExternalDocs { get; set; }
        public string? Description { get; set; }

        [DebuggerDisplay("Url = {Url}")]
        internal class ExternalDocsObject
        {
            public string? Description { get; set; }
            public Uri? Url { get; set; }
        }
    }

    internal class RiotApiParameterObject : OpenApiParameterObject
    {
        public SchemaObject? Schema { get; set; }
        public string? Description { get; set; }

        [DebuggerDisplay("X-Type = {XType} Type = {Type}")]
        internal class SchemaObject
        {
            [JsonPropertyName("x-type")] public string? XType { get; set; }
            public string? Type { get; set; }
        }
    }

    [DebuggerDisplay("X-Type = {XType} $ref = {Ref}")]
    internal class RiotApiSchemaObject : OpenApiSchemaObject
    {
        [JsonPropertyName("x-type")] public string? XType { get; set; }
    }

    internal class RiotApiPostMethodObject : RiotApiMethodObject
    {
        public RequestBodyObject? RequestBody { get; set; }

        [DebuggerDisplay("Required = {Required}")]
        internal class RequestBodyObject : OpenApiResponseObject<RiotApiSchemaObject>
        {
            public bool? Required { get; set; }
        }
    }

    internal class RiotApiPutMethodObject : RiotApiPostMethodObject
    {

    }

    #endregion Path

    [DebuggerDisplay("Schemas = {Schemas.Count} SecuritySchemes = {SecuritySchemes.Count}")]
    internal class RiotApiComponentsObject : OpenApiComponentsObject<RiotApiComponentSchemaObject, RiotApiComponentPropertyObject>
    {
        public Dictionary<string, SecuritySchemeObject>? SecuritySchemes { get; set; }


        [DebuggerDisplay("Name = {Name} Type = {Type} Description = {Description}")]
        internal class SecuritySchemeObject
        {
            public string? Type { get; set; }
            public string? Description { get; set; }
            public string? Name { get; set; }
            public string? In { get; set; }
        }
    }

    [DebuggerDisplay("Title = {Title} Properties = {Properties.Count}")]
    internal class RiotApiComponentSchemaObject : OpenApiComponentSchemaObject<RiotApiComponentPropertyObject>
    {
        public string? Title { get; set; }
        public string? Name { get; set; }
        public string[]? Required { get; set; }
    }

    [DebuggerDisplay("X-Type = {XType} Type = {Type} Properties = {Properties.Count} $ref = {Ref}")]
    internal class RiotApiComponentPropertyObject : OpenApiComponentPropertyObject
    {
        [JsonPropertyName("x-type")] public string? XType { get; set; }
        [JsonPropertyName("x-enum")] public string? XEnum { get; set; }
        public string? Description { get; set; }
        public Dictionary<string, PropertiesObject>? Properties { get; set; }

        [DebuggerDisplay("Type = {Type}")]
        internal class PropertiesObject
        {
            public string? Type { get; set; }
        }
    }
}
