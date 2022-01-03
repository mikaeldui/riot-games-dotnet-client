using MingweiSamuel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MingweiSamuel.RiotApiOpenApiSchema.ComponentsObject;

namespace RiotGames.Client.CodeGeneration
{
    internal static class ModelHelper
    {
        private static string _removeDtoSuffix(string dtoName) =>
            dtoName.Remove("Dto").Remove("DTO");

        // RiotApiOpenApiSchema.PathObject.MethodObject.ResponseObject.ContentObject.SchemaObject

        public static string GetTypeNameFromRef(string @ref)
        {
            return Hacks.EndpointsWithDuplicateSchemas.FirstOrDefault(kvp => @ref.Contains(kvp.Key)).Value + _removeDtoSuffix(@ref?.Split('.')?.Last());
        }

        public static string GetTypeName(this RiotApiOpenApiSchema.PathObject.MethodObject.ResponseObject.ContentObject.SchemaObject schema)
        {
            if (schema.Ref != null)
                return GetTypeNameFromRef(schema.Ref);
            else if (schema.XType == null && schema.Type == null)
                Debugger.Break();
            else
                switch (schema.Type)
                {
                    case "array":
                        return _removeDtoSuffix(schema.XType.Remove("Set[").Remove("List[").TrimEnd(']')) + "[]";
                    case "integer":
                        if (schema.XType == null)
                            throw new Exception("Schema.XType is null for some reason.");
                        return schema.XType;
                    case "string":
                        return schema.Type;
                }

            throw new Exception("Couldn't figure out the response type.");
        }

        public static string GetTypeName(this SchemaObject.PropertyObject property)
        {
            if (property.Ref != null)
                return GetTypeNameFromRef(property.Ref);            

            var name = ModelHelper._removeDtoSuffix(property.XType ?? property.Format ?? property.Type);

            return name switch
            {
                "int32" => "int",
                "int64" => "long",
                "boolean" => "bool",
                _ => name,
            };
        }    
    }
}
