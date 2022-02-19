using System.Diagnostics;
using MingweiSamuel;

namespace RiotGames.Client.CodeGeneration;

internal static class OpenApiComponentHelper
{
    public static string RemoveDtoSuffix(this string dtoName)
    {
        return dtoName.Remove("Dto").Remove("DTO");
    }

    public static string GetTypeNameFromRef(string @ref, bool removeDtoSuffix = true)
    {
        //return RiotApiHacks.EndpointsWithDuplicateSchemas.FirstOrDefault(kvp => @ref.Contains(kvp.Key)).Value + (@ref?.Split('.')?.Last()).RemoveDtoSuffix();
        @ref = @ref.SplitAndRemoveEmptyEntries('/').Last();
        return (removeDtoSuffix ? @ref.RemoveDtoSuffix() : @ref).ToPascalCase();
    }

    public static string GetTypeName(this OpenApiComponentPropertyObject property)
    {
        return property.Ref != null
            ? GetTypeNameFromRef(property.Ref)
            : GetTypeNameFromString((property.Format ?? property.Type) ?? throw new InvalidOperationException());
    }

    public static string GetTypeName(this OpenApiSchemaObject schema)
    {
        if (schema.Ref != null)
            return GetTypeNameFromRef(schema.Ref);
        if (schema.Type == null)
            Debugger.Break();
        else
            switch (schema.Type)
            {
                case "array":
                    return GetTypeName(schema.Items ?? throw new InvalidOperationException()) + "[]";
                case "integer":
                    return GetTypeNameFromString(schema.Format ?? "int");
                case "number":
                    return GetTypeNameFromString(schema.Format ?? "decimal");
                case "string":
                    return schema.Type;
                case "boolean":
                    return "bool";
                case "object":
                    return schema.Type;
            }

        throw new Exception("Couldn't figure out the response type.");
    }

    public static string GetTypeNameFromString(string typeName)
    {
        typeName = typeName.RemoveDtoSuffix();
        return typeName switch
        {
            "int32" => "int",
            "int64" => "long",
            "boolean" => "bool",
            "integer" => "int",
            _ => typeName
        };
    }
}