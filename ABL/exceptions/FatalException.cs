using System;
using System.Runtime.Serialization;

namespace ABL.Exceptions
{
    public class FatalException : Exception
    {
        public FatalException()
            : base()
        {
            Logger.Log(LogLevel.Fatal, this.Message, this);
        }

        public FatalException(string message)
            : base(message)
        {
            Logger.Log(LogLevel.Fatal, message, this);
        }

        protected FatalException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public FatalException(string message, Exception innerException)
            : base(message, innerException)
        {
            Logger.Log(LogLevel.Fatal, message, innerException);
        }

    }
}