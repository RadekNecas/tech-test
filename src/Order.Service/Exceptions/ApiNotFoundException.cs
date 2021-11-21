using System;
using System.Runtime.Serialization;

namespace Order.Service.Exceptions
{
    public class ApiNotFoundException : Exception
    {
        public ApiNotFoundException()
        {
        }

        public ApiNotFoundException(string message) : base(message)
        {
        }

        public ApiNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ApiNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
