using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ABL.Extends
{
    class BasicParseContext
    {
        static IObjectFormat fmt = new DefaultFormat();
        public static T Parse<T>(object obj)
        {
            return Convert(obj.GetType(), obj, null, typeof(T)).As<T>();
        }

        public static object Parse(object obj, Type type)
        {
            return Convert(obj.GetType(), obj, null, type);
        }

        static object Convert(Type inType, object value, string format, Type outType)
        {
            return fmt.Format(inType, value, format, outType);
        }
    }
}
