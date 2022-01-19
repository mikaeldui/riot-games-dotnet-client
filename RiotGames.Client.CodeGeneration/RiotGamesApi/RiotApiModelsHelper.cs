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
    internal static class RiotApiModelsHelper
    {
        public static string GetTypeNameFromRef(string @ref)
        {
            @ref = RiotApiHacks.EndpointsWithDuplicateSchemas
                .FirstOrDefault(kvp => @ref.Contains(kvp.Key))
                .Value + @ref?.Split('.')?.Last();
            return OpenApiComponentHelper.GetTypeNameFromRef(@ref);
        }

        public static string GetTypeName(this RiotApiSchemaObject schema, Client client)
        {
            if (schema.Ref != null)
                return GetTypeNameFromRef(schema.Ref);
            else if (schema.XType == null && schema.Type == null)
                Debugger.Break();
            else
                switch (schema.Type)
                {
                    case "array":
                        return $"{client}ReadOnlyCollection<{schema.XType.RemoveStart("Set[").RemoveStart("List[").TrimEnd(']').RemoveDtoSuffix()}>";
                    case "integer":
                        if (schema.XType == null)
                            throw new Exception("Schema.XType is null for some reason.");
                        return schema.XType;
                    case "string":
                        return schema.Type;
                }

            throw new Exception("Couldn't figure out the response type.");
        }

        public static string GetTypeName(this OpenApiComponentPropertyObject property, Client client)
        {
            if (property.Type == "array" || (property as RiotApiComponentPropertyObject)?.XType == "array")
                return $"{client}ReadOnlyCollection<{GetTypeName(property.Items, client)}>";

            if (property.Ref != null)
                return GetTypeNameFromRef(property.Ref);            

            var name = ((property as RiotApiComponentPropertyObject)?.XType ?? property.Format ?? property.Type).RemoveDtoSuffix();

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
