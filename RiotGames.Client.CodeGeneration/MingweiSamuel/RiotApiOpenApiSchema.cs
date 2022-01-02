using System.Text.Json.Serialization;

namespace MingweiSamuel
{
    internal class RiotApiOpenApiSchema
    {
        public Dictionary<string, PathObject>? Paths { get; set; }

        public ComponentsObject? Components { get; set; }

        public Dictionary<string, object[]>[]? Security { get; set; }

        internal class PathObject
        {
            public MethodObject? Get { get; set; }
            public PostMethodObject? Post { get; set; }
            public PutMethodObject? Put { get; set; }

            public string? XEndpoint { get; set; }
            public string[]? XPlatformsAvailable { get; set; }
            public string? XRouteEnum { get; set; }

            internal class MethodObject
            {
                public string[]? Tags { get; set; }
                public string? Summary { get; set; }
                public ExternalDocsObject? ExternalDocs { get; set; }
                public Dictionary<string, ResponseObject>? Responses { get; set; }
                public string? OperationId { get; set; }
                public bool XNullable404 { get; set; }
                public string? Description { get; set; }
                public ParametersObject[]? Parameters { get; set; }
                public string? XEndpoint { get; set; }
                public string[]? XPlatformsAvailable { get; set; }
                public string? XRouteEnum { get; set; }

                internal class ExternalDocsObject
                {
                    public string? Description { get; set; }
                    public Uri? Url { get; set; }
                }

                internal class ResponseObject
                {
                    public string? Description { get; set; }
                    public Dictionary<string, ContentObject>? Content { get; set; }

                    internal class ContentObject
                    {
                        public SchemaObject? Schema { get; set; }

                        internal class SchemaObject
                        {
                            [JsonPropertyName("$ref")]
                            public string? Ref { get; set; }
                            [JsonPropertyName("x-type")]
                            public string? XType { get; set; }
                            public string? Format { get; set; }
                            public string? Type { get; set; }
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

                internal class RequestBodyObject : ResponseObject
                {
                    public bool? Required { get; set; }
                }
            }

            internal class PutMethodObject : PostMethodObject
            {

            }
        }

        internal class ComponentsObject
        {
            public Dictionary<string, SchemaObject>? Schemas { get; set; }
            public Dictionary<string, SecuritySchemeObject>? SecuritySchemes { get; set; }
        
            internal class SchemaObject
            {
                public string? Type { get; set; }
                public string? Title { get; set; }
                public Dictionary<string, PropertyObject>? Properties { get; set; }

                internal class PropertyObject
                {
                    public string? Type { get; set; }
                    public string? Format { get; set; }
                    public Dictionary<string, string>? Items { get; set; }
                    public string? XType { get; set; }
                    public string[]? Enum { get; set; }
                    public string? Description { get; set; }
                    public string? XEnum { get; set; }
                    public Dictionary<string, PropertyObject.PropertiesObject>? Properties { get; set; }

                    internal class PropertiesObject
                    {
                        public string? Type { get; set; }
                    }
                }
                public string[]? Required { get; set; }
            }

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
