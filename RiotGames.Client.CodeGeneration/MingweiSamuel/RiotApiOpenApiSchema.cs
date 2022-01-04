using System.Diagnostics;
using System.Text.Json.Serialization;

namespace MingweiSamuel
{
    [DebuggerDisplay("Paths = {Paths.Count} Components.Schema = {Components.Schemas.Count}")]
    internal class RiotApiOpenApiSchema
    {
        public Dictionary<string, PathObject>? Paths { get; set; }

        public ComponentsObject? Components { get; set; }

        public Dictionary<string, object[]>[]? Security { get; set; }

        [DebuggerDisplay("XEndpoint")]
        internal class PathObject
        {
            public MethodObject? Get { get; set; }
            public PostMethodObject? Post { get; set; }
            public PutMethodObject? Put { get; set; }
            [JsonPropertyName("x-endpoint")] public string? XEndpoint { get; set; }
            [JsonPropertyName("x-platforms-available")] public string[]? XPlatformsAvailable { get; set; }
            [JsonPropertyName("x-route-enum")] public string? XRouteEnum { get; set; }

            [DebuggerDisplay("OperationId = {OperationId}")]
            internal class MethodObject
            {
                public string[]? Tags { get; set; }
                public string? Summary { get; set; }
                public ExternalDocsObject? ExternalDocs { get; set; }
                public Dictionary<string, ResponseObject>? Responses { get; set; }
                public string? OperationId { get; set; }
                [JsonPropertyName("x-nullable-404")] public bool XNullable404 { get; set; }
                public string? Description { get; set; }
                public ParametersObject[]? Parameters { get; set; }
                [JsonPropertyName("x-endpoint")] public string? XEndpoint { get; set; }
                [JsonPropertyName("x-platforms-available")] public string[]? XPlatformsAvailable { get; set; }
                [JsonPropertyName("x-route-enum")] public string? XRouteEnum { get; set; }

                [DebuggerDisplay("Url = {Url}")]
                internal class ExternalDocsObject
                {
                    public string? Description { get; set; }
                    public Uri? Url { get; set; }
                }

                [DebuggerDisplay("Description = {Description}")]
                internal class ResponseObject
                {
                    public string? Description { get; set; }
                    public Dictionary<string, ContentObject>? Content { get; set; }

                    internal class ContentObject
                    {
                        public SchemaObject? Schema { get; set; }

                        [DebuggerDisplay("X-Type = {XType} $ref = {Ref}")]
                        internal class SchemaObject
                        {
                            [JsonPropertyName("$ref")]
                            public string? Ref { get; set; }
                            [JsonPropertyName("x-type")]
                            public string? XType { get; set; }
                            public string? Format { get; set; }
                            public string? Type { get; set; }
                            /// <summary>
                            /// Set if XType if array.
                            /// </summary>
                            public SchemaObject? Items { get; set; }
                        }
                    }
                }

                internal class ParametersObject
                {
                    public string? Name { get; set; }
                    public string? In { get; set; }
                    public bool? Required { get; set; }
                    public SchemaObject? Schema { get; set; }
                    public string? Description { get; set; }

                    [DebuggerDisplay("X-Type = {XType} Type = {Type}")]
                    internal class SchemaObject
                    {
                        public string? Type { get; set; }
                        [JsonPropertyName("x-type")]
                        public string? XType { get; set; }
                    }
                }
            }

            internal class PostMethodObject : MethodObject
            {
                public RequestBodyObject? RequestBody { get; set; }

                [DebuggerDisplay("Required = {Required}")]
                internal class RequestBodyObject : ResponseObject
                {
                    public bool? Required { get; set; }
                }
            }

            internal class PutMethodObject : PostMethodObject
            {

            }
        }

        [DebuggerDisplay("Schemas = {Schemas.Count} SecuritySchemes = {SecuritySchemes.Count}")]
        internal class ComponentsObject
        {
            public Dictionary<string, SchemaObject>? Schemas { get; set; }
            public Dictionary<string, SecuritySchemeObject>? SecuritySchemes { get; set; }
        
            [DebuggerDisplay("Title = {Title} Properties = {Properties.Count}")]
            internal class SchemaObject
            {
                public string? Type { get; set; }
                public string? Title { get; set; }
                public string? Name { get; set; }
                public Dictionary<string, PropertyObject>? Properties { get; set; }

                [DebuggerDisplay("X-Type = {XType} Type = {Type} Properties = {Properties.Count} $ref = {Ref}")]
                internal class PropertyObject
                {
                    [JsonPropertyName("$ref")]
                    public string? Ref { get; set; }
                    public string? Type { get; set; }
                    public string? Format { get; set; }
                    public PropertyObject? Items { get; set; }
                    [JsonPropertyName("x-type")] public string? XType { get; set; }
                    public string[]? Enum { get; set; }
                    public string? Description { get; set; }
                    [JsonPropertyName("x-enum")] public string? XEnum { get; set; }
                    public Dictionary<string, PropertyObject.PropertiesObject>? Properties { get; set; }

                    [DebuggerDisplay("Type = {Type}")]
                    internal class PropertiesObject
                    {
                        public string? Type { get; set; }
                    }
                }
                public string[]? Required { get; set; }
            }

            [DebuggerDisplay("Name = {Name} Type = {Type} Description = {Description}")]
            internal class SecuritySchemeObject
            {
                public string? Type { get; set; }
                public string? Description { get; set; }
                public string? Name { get; set; }
                public string? In { get; set; }
            }
        }
    }
}
