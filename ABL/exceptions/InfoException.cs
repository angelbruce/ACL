using System;
using System.Runtime.Serialization;

namespace ABL.Exceptions
{
    public class InfoException:Exception
    {
        public InfoException()
            : base()
        {
            Logger.Log(LogLevel.Info, this.Message, this);
        }

        public InfoException(string message)
            : base(message)
        {
            Logger.Log(LogLevel.Info, message, this);
        }

        protected InfoException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public InfoException(string message, Exception innerException)
            : base(message, innerException)
        {
            Logger.Log(LogLevel.Info, message, innerException);
        }

    }
}
