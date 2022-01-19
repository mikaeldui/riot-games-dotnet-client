using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace RiotGames.LeagueOfLegends.DataDragon
{
    public class DataDragonException : LeagueOfLegendsException
    {
        public DataDragonException()
        {
        }

        public DataDragonException(string message) : base(message)
        {
        }

        public DataDragonException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DataDragonException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
