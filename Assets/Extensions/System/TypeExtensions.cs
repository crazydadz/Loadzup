using System;
using System.Collections.Generic;

namespace Silphid.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsAssignableFrom<T>(this Type This)
        {
            return This.IsAssignableFrom(typeof(T));
        }

        public static bool IsAssignableTo(this Type This, Type toType)
        {
            return toType.IsAssignableFrom(This);
        }

        public static bool IsAssignableTo<T>(this Type This)
        {
            return typeof(T).IsAssignableFrom(This);
        }

        public static bool Is<T>(this Type This)
        {
            return This == typeof(T);
        }

        public static IEnumerable<Type> Ancestors(this Type type)
        {
            type = type?.BaseType;
            while (type != null)
            {
                yield return type;
                type = type.BaseType;
            }
        }

        public static IEnumerable<Type> SelfAndAncestors(this Type type)
        {
            while (type != null)
            {
                yield return type;
                type = type.BaseType;
            }
        }
    }
}