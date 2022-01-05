using MingweiSamuel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.Client.CodeGeneration
{
    internal static class OpenApiComponentHelper
    {
        public static string RemoveDtoSuffix(this string dtoName) =>
            dtoName.Remove("Dto").Remove("DTO");

        public static string GetTypeNameFromRef(string @ref)
        {
            //return RiotApiHacks.EndpointsWithDuplicateSchemas.FirstOrDefault(kvp => @ref.Contains(kvp.Key)).Value + (@ref?.Split('.')?.Last()).RemoveDtoSuffix();
            @ref = @ref.SplitAndRemoveEmptyEntries('/').Last();
            return @ref.RemoveDtoSuffix();
        }

        public static string GetTypeName(this OpenApiComponentPropertyObject property)
        {
            //if (property.Ref != null)
            //    return GetTypeNameFromRef(property.Ref);

            return GetTypeNameFromString(property.Format ?? property.Type);
        }

        public static string GetTypeName(this OpenApiSchemaObject schema)
        {
            if (schema.Ref != null)
                return GetTypeNameFromRef(schema.Ref);
            else if (schema.Type == null)
                Debugger.Break();
            else
                switch (schema.Type)
                {
                    case "array":
                        return GetTypeName(schema.Items) + "[]";
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
                _ => typeName,
            };
        }
    }
}
