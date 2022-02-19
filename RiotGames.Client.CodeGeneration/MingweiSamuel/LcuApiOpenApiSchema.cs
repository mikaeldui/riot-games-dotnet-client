namespace MingweiSamuel.Lcu;

using LcuMethod = OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>;
using LcuComponentsObject = OpenApiComponentsObject<LcuComponentSchemaObject, LcuComponentPropertyObject>;

internal class LcuApiOpenApiSchema : OpenApiSchema<
    OpenApiPathObject<LcuMethod, LcuMethod, LcuMethod, LcuParameterObject, LcuSchemaObject>,
    LcuMethod, LcuMethod, LcuMethod, LcuParameterObject, LcuSchemaObject, LcuComponentsObject,
    LcuComponentSchemaObject, LcuComponentPropertyObject>
{
}

internal class LcuSchemaObject : OpenApiSchemaObject
{
    public object? AdditionalProperties { get; set; }
}

internal class LcuParameterObject : OpenApiParameterObject
{
    public string? Type { get; set; }
    public string? Format { get; set; }
}

internal class LcuComponentSchemaObject : OpenApiComponentSchemaObject<LcuComponentPropertyObject>
{
    public string[]? Enum { get; set; }
}

internal class LcuComponentPropertyObject : OpenApiComponentPropertyObject
{
    public object? AdditionalProperties { get; set; }
}