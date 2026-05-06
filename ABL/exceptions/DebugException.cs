using System;
using System.Runtime.Serialization;

namespace ABL.Exceptions
{
    public class DebugException:Exception
    {
        public DebugException()
            : base()
        {
            Logger.Log(LogLevel.Debug, this.Message, this);
        }

        public DebugException(string message)
            : base(message)
        {
            Logger.Log(LogLevel.Debug, message, this);
        }

        protected DebugException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public DebugException(string message, Exception innerException)
            : base(message, innerException)
        {
            Logger.Log(LogLevel.Debug, message, innerException);
        }

    }
}