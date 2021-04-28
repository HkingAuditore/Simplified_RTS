using System;

namespace StaticClasses
{
    public static class ClassObjectHelper
    {
        /// <summary>
        ///     类型转换
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ChangeType<T>(this object obj)
        {
            return (T) Convert.ChangeType(obj, typeof(T));
        }
    }
}