using MingweiSamuel;
using MingweiSamuel.RiotApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.Client.CodeGeneration.RiotGamesApi
{
    internal static class RiotApiComponentHelper
    {
        // RiotApiOpenApiSchema.PathObject.MethodObject.ResponseObject.ContentObject.SchemaObject

        public static string GetTypeNameFromRef(string @ref)
        {
            var riotApiRef = RiotApiHacks.EndpointsWithDuplicateSchemas.FirstOrDefault(kvp => @ref.Contains(kvp.Key)).Value + (@ref?.Split('.')?.Last());
            return OpenApiComponentHelper.GetTypeNameFromRef(riotApiRef);
        }

        public static string GetTypeName(this RiotApiSchemaObject schema)
        {
            if (schema.Ref != null)
                return GetTypeNameFromRef(schema.Ref);
            else if (schema.XType == null && schema.Type == null)
                Debugger.Break();
            else
                switch (schema.Type)
                {
                    case "array":
                        return (schema.XType.Remove("Set[").Remove("List[").TrimEnd(']')).RemoveDtoSuffix() + "[]";
                    case "integer":
                        if (schema.XType == null)
                            throw new Exception("Schema.XType is null for some reason.");
                        return schema.XType;
                    case "string":
                        return schema.Type;
                }

            throw new Exception("Couldn't figure out the response type.");
        }

        public static string GetTypeName(this RiotApiComponentPropertyObject property)
        {
            if (property.Ref != null)
                return GetTypeNameFromRef(property.Ref);            

            var name = (property.XType ?? property.Format ?? property.Type).RemoveDtoSuffix();

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
