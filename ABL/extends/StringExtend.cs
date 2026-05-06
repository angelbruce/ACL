using System.Linq;
namespace ABL
{
    public static class StringExtend
    {
        /// <summary>
        /// 字节长度
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int ByteLength(this string str)
        {
            return str == null ? 0 : System.Text.Encoding.Default.GetBytes(str).Count();
        }

        /// <summary>
        /// 转换到枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this string str)
        {
            return ABL.Enums.EnumHelper.Helper.Parse<T>(str);
        }

        /// <summary>
        /// 转换到枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this string str, bool ignoreCase)
        {
            return ABL.Enums.EnumHelper.Helper.Parse<T>(str, ignoreCase);
        }


        /// <summary>
        /// 通过说明转换到枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="desc"></param>
        /// <returns></returns>
        public static T DescToEnum<T>(this string desc)
        {
            return ABL.Enums.EnumHelper.Helper.ParseDesc<T>(desc);
        }

        /// <summary>
        /// 通过说明转换到枚举（不区分大小写）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="desc"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static T DescToEnum<T>(this string desc, bool ignoreCase)
        {
            return ABL.Enums.EnumHelper.Helper.ParseDesc<T>(desc, ignoreCase);
        }

        /// <summary>
        /// 首字母大写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Capitalize(this string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            char c = str[0];
            short delt = 'A'-'a';
            char first=  c;
            if (c >= 'a' && c <= 'z') first = (char)(delt + (short)c);
            if (str.Length == 1) return first.ToString();
            return string.Format("{0}{1}", first, str.Substring(1));
        }

        public static string ToSafety(this string str)
        {
            if (string.IsNullOrEmpty(str)) return str;

            return str.Replace("'", "''").Replace("%", "%%") ;
        }
    }
}
