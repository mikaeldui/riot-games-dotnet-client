using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MingweiSamuel.RiotApiOpenApiSchema.ComponentsObject;

namespace RiotGames.Client.CodeGeneration
{
    internal static class ModelHelper
    {
        public static string RemoveDtoSuffix(string dtoName) =>
            dtoName.Remove("Dto").Remove("DTO");
    
        public static string GetTypeName(this SchemaObject.PropertyObject property)
        {
            var name = ModelHelper.RemoveDtoSuffix(property.XType ?? property.Ref?.Split('.')?.Last() ?? property.Format ?? property.Type);

            switch (name)
            {
                case "int32":
                    return "int";
                case "int64":
                    return "long";
                case "boolean":
                    return "bool";
                default:
                    return name;
            }
        }
    
    }
}
