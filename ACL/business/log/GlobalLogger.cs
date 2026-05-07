using System;
using System.Collections.Generic;
using System.Text;

namespace ACL.business.log
{
    public class GlobalLogger
    {
        public static ILog? Log { get; set; }

        public static void Debug(string message)
        {
            Log?.Debug(message);
        }
        public static void Info(string message)
        {
            Log?.Info(message);
        }
        public static void Warn(string message)
        {
            Log?.Warn(message);
        }
        public static void Error(string message)
        {
            Log?.Error(message);
        }
        public static void Fatal(string message)
        {
            Log?.Fatal(message);
        }
    }
}
