using System;
using UnityEngine;

namespace StaticClasses
{
    public  static class classObjectHelper
    {
        public static T ChangeType<T>(this object obj)
        {
            return (T)Convert.ChangeType(obj, typeof(T));
        }
    }
}