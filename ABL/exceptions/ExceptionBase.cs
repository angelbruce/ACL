using System;
using System.Runtime.Serialization;

namespace ABL.Exceptions
{
    public class ExceptionBase:Exception
    {
        public ExceptionBase()
            : base()
        {
            Logger.Log(LogLevel.Error, this.Message, this);
        }

        public ExceptionBase(string message)
            : base(message)
        {
            Logger.Log(LogLevel.Error, message, this);
        }

        protected ExceptionBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public ExceptionBase(string message, Exception innerException)
            : base(message, innerException)
        {
            Logger.Log(LogLevel.Error, message, innerException);
        }

    }
}
