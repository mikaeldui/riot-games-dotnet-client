using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MingweiSamuel.Lcu
{
    using LcuMethod = OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>;
    using LcuComponentsObject = OpenApiComponentsObject<OpenApiComponentSchemaObject<LcuComponentPropertyObject>, LcuComponentPropertyObject>;
    using LcuComponentSchema = OpenApiComponentSchemaObject<LcuComponentPropertyObject>;

    internal class LcuApiOpenApiSchema : OpenApiSchema<
        OpenApiPathObject<LcuMethod, LcuMethod, LcuMethod, LcuParameterObject, LcuSchemaObject>,
        LcuMethod, LcuMethod, LcuMethod, LcuParameterObject, LcuSchemaObject, LcuComponentsObject,
        LcuComponentSchema, LcuComponentPropertyObject>
    {
    }

    internal class LcuSchemaObject : OpenApiSchemaObject
    {
        public object? AdditionalProperties { get; set; }
    }

    internal class LcuParameterObject : OpenApiParameterObject
    {
        public string? Type { get; set; }
    }

    internal class LcuComponentPropertyObject : OpenApiComponentPropertyObject
    {
        public object? AdditionalProperties { get; set; }
    }
}