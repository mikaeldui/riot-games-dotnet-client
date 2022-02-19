using System.Diagnostics;
using System.Text.Json.Serialization;

namespace MingweiSamuel;

internal class OpenApiSchema<
    TPath, TGetMethodObject, TPostMethodObject, TPutMethodObject, TParameter, TSchema,
    TComponents, TComponentSchema, TComponentProperty>
    where TPath : OpenApiPathObject<TGetMethodObject, TPostMethodObject, TPutMethodObject, TParameter, TSchema>
    where TGetMethodObject : OpenApiMethodObject<TParameter, TSchema>
    where TPostMethodObject : OpenApiMethodObject<TParameter, TSchema>
    where TPutMethodObject : OpenApiMethodObject<TParameter, TSchema>
    where TParameter : OpenApiParameterObject
    where TSchema : OpenApiSchemaObject
    where TComponents : OpenApiComponentsObject<TComponentSchema, TComponentProperty>
    where TComponentSchema : OpenApiComponentSchemaObject<TComponentProperty>
    where TComponentProperty : OpenApiComponentPropertyObject
{
    public Dictionary<string, TPath>? Paths { get; set; }
    public TComponents? Components { get; set; }
}

#region Open API Paths

internal class OpenApiPathObject<TGetMethodObject, TPostMethodObject, TPutMethodObject, TParameter, TSchema>
    where TGetMethodObject : OpenApiMethodObject<TParameter, TSchema>
    where TPostMethodObject : OpenApiMethodObject<TParameter, TSchema>
    where TPutMethodObject : OpenApiMethodObject<TParameter, TSchema>
    where TParameter : OpenApiParameterObject
    where TSchema : OpenApiSchemaObject
{
    public TGetMethodObject? Get { get; set; }
    public TPostMethodObject? Post { get; set; }
    public TPutMethodObject? Put { get; set; }
}

internal class OpenApiMethodObject<TParameter, TSchema>
    where TParameter : OpenApiParameterObject
    where TSchema : OpenApiSchemaObject
{
    public string? OperationId { get; set; }
    public string[]? Tags { get; set; }
    public TParameter[]? Parameters { get; set; }
    public Dictionary<string, OpenApiResponseObject<TSchema>>? Responses { get; set; }
}

internal class OpenApiParameterObject
{
    public string? Name { get; set; }
    public string? In { get; set; }
    public bool? Required { get; set; }
}

[DebuggerDisplay("Description = {Description}")]
internal class OpenApiResponseObject<TSchema>
    where TSchema : OpenApiSchemaObject
{
    public string? Description { get; set; }
    public Dictionary<string, OpenApiContentObject<TSchema>>? Content { get; set; }
}

internal class OpenApiContentObject<TSchema>
    where TSchema : OpenApiSchemaObject
{
    public TSchema? Schema { get; set; }
}

internal class OpenApiSchemaObject
{
    [JsonPropertyName("$ref")] public string? Ref { get; set; }
    public OpenApiSchemaObject? Items { get; set; }
    public string? Type { get; set; }
    public string? Format { get; set; }
}

#endregion Open API Paths

#region Open API Components

[DebuggerDisplay("Schemas = {Schemas.Count}")]
internal class OpenApiComponentsObject<TComponentSchema, TComponentProperty>
    where TComponentSchema : OpenApiComponentSchemaObject<TComponentProperty>
    where TComponentProperty : OpenApiComponentPropertyObject
{
    public Dictionary<string, TComponentSchema>? Schemas { get; set; }
}

internal class OpenApiComponentSchemaObject<TProperty>
    where TProperty : OpenApiComponentPropertyObject
{
    public string? Type { get; set; }
    public Dictionary<string, TProperty>? Properties { get; set; }
}

[DebuggerDisplay("Type = {Type} $ref = {Ref}")]
internal class OpenApiComponentPropertyObject
{
    [JsonPropertyName("$ref")] public string? Ref { get; set; }
    public string? Type { get; set; }
    public string? Format { get; set; }
    public string[]? Enum { get; set; }
    public OpenApiComponentPropertyObject? Items { get; set; }
}

#endregion Open API Component