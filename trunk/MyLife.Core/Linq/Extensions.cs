using System;
using System.Collections.Generic;
using System.Linq;

namespace MyLife.Linq
{
    public static class Extensions
    {
        #region Object

        public static IList<string> ToProperties(this object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException();
            }

            var retval = new List<string>();
            foreach (var property in obj.GetType().GetProperties())
            {
                retval.Add(property.Name);
            }
            return retval;
        }

        #endregion

        #region Enumerable

        public static TSource SelectSingle<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException();
            }

            var list = source.Where(predicate);

            foreach (var item in list)
            {
                return item;
            }

            return default(TSource);
        }

        public static string ToString<TSource>(this IEnumerable<TSource> source, Func<TSource, string> predicate,
                                               string separate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            var retval = "";
            foreach (var item in source)
            {
                retval += predicate(item) + separate;
            }

            if (retval.Length > 0)
            {
                retval = retval.Substring(0, retval.Length - separate.Length);
            }
            return retval;
        }

        public static string ToString<T>(this IEnumerable<T> source, string separator)
        {
            if (source == null)
                throw new ArgumentException("source can not be null.");

            if (string.IsNullOrEmpty(separator))
                throw new ArgumentException("separator can not be null or empty.");

            var array = (from s in source select s.ToString()).ToArray();
            return string.Join(separator, array);
        }

        public static TResult[] ToArray<TSource, TResult>(this IEnumerable<TSource> source,
                                                          Func<TSource, TResult> predicate)
        {
            if (source == null)
            {
                return null;
            }

            var retval = new List<TResult>();
            foreach (var item in source)
            {
                retval.Add(predicate(item));
            }
            return retval.ToArray();
        }

        public static bool ToBoolean(this IEnumerable<bool> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            foreach (var item in source)
            {
                if (item)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsNullOrEmpty<TSource>(this IEnumerable<TSource> source)
        {
            return source == null || source.Count() == 0;
        }

        public static bool GetLastValue(this IEnumerable<bool> source)
        {
            return source.GetLastValue(false);
        }

        public static bool GetLastValue(this IEnumerable<bool> source, bool defaultValue)
        {
            if (source.IsNullOrEmpty())
            {
                return defaultValue;
            }

            return source.ElementAt(source.Count() - 1);
        }

        #endregion
    }
}