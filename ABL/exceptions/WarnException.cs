using System;
using System.Runtime.Serialization;

namespace ABL.Exceptions
{
    public class WarnException : Exception
    {
        public WarnException()
            : base()
        {
            Logger.Log(LogLevel.Warn, this.Message, this);
        }

        public WarnException(string message)
            : base(message)
        {
            Logger.Log(LogLevel.Warn, message, this);
        }

        protected WarnException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public WarnException(string message, Exception innerException)
            : base(message, innerException)
        {
            Logger.Log(LogLevel.Warn, message, innerException);
        }

    }
   
}
