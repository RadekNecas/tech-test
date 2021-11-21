using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Order.Service.Exceptions
{
    public class InvalidApiParameterException : Exception
    {
        public BadRequestErrors Errors { get; } = new BadRequestErrors();


        public InvalidApiParameterException(BadRequestErrors errors)
        {
            Errors = errors;
        }

        public InvalidApiParameterException(string parameterName, string message)
        {
            Errors.AddError(parameterName, message);
        }

        public InvalidApiParameterException(string parameterName, IEnumerable<string> messages)
        {
            Errors.AddErrors(parameterName, messages);
        }

        public InvalidApiParameterException(BadRequestErrors errors, string message) : base(message)
        {
            Errors = errors;
        }

        public InvalidApiParameterException(BadRequestErrors errors, string message, Exception innerException) : base(message, innerException)
        {
            Errors = errors;
        }

        protected InvalidApiParameterException(BadRequestErrors errors, SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Errors = errors;
        }
    }
}
