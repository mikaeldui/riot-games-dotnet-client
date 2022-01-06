using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RiotGames
{
    internal class JsonStringEnumAttribute : JsonConverterAttribute
    {
        public JsonStringEnumAttribute() : base(typeof(JsonStringEnumConverter))
        {

        }
    }
}
