using System;
using ABL.Enums;
namespace ABL
{
    public static class EnumExtend
    {
        /// <summary>
        /// 获取小写
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string ToLower(this Enum e) { return e.ToString().ToLower(); }

        /// <summary>
        /// 获取大写
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string ToUpper(this Enum e) { return e.ToString().ToUpper(); }

        /// <summary>
        /// 获取枚举的值
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static int Value(this Enum e) { return e.GetHashCode(); }

        /// <summary>
        /// 获取描述信息
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string Description(this Enum e)
        {
            return EnumHelper.Helper.Description(e);
        }
    }
}
