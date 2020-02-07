using System;
using System.Runtime.Serialization;

namespace ExpressionEvalService.BL
{
    [Serializable]
    public class ExpresionException : Exception
    {
        public ExpresionException()
        {
        }

        public ExpresionException(string message) : base(message)
        {
        }

        public ExpresionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ExpresionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}