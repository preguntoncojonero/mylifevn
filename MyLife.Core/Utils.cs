using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using Microsoft.Practices.EnterpriseLibrary.Security.Cryptography;
using Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration;

namespace MyLife
{
    public static class Utils
    {
        public static string GenerateUniqueKey()
        {
            return GenerateUniqueKey(10);
        }

        public static string GenerateUniqueKey(int length)
        {
            var key = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var chars = key.ToCharArray();
            var data = new byte[length];
            var crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            var result = new StringBuilder(length);
            foreach (var b in data)
            {
                result.Append(chars[b%(chars.Length - 1)]);
            }
            return result.ToString().ToLowerInvariant();
        }

        /// <summary>
        /// Read binary dat from file
        /// </summary>
        /// <param name="path">File path</param>
        /// <returns>Data of file</returns>
        public static byte[] ReadFileBinary(string path)
        {
            try
            {
                var stream = new FileStream(path, FileMode.Open);
                var reader = new BinaryReader(stream);
                var data = reader.ReadBytes((int) stream.Length);
                reader.Close();
                stream.Close();
                return data;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Writer binary data into file
        /// </summary>
        /// <param name="path">File path</param>
        /// <param name="data">Data</param>
        /// <returns>Return True if writer success and else</returns>
        public static bool WriterFileBinary(string path, byte[] data)
        {
            try
            {
                var stream = new FileStream(path, FileMode.Create);
                var writer = new BinaryWriter(stream);
                writer.Write(data);
                writer.Flush();
                writer.Close();
                stream.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void GenerateSymmetricKey(Stream stream)
        {
            var settings = ConfigurationManager.GetSection("securityCryptographyConfiguration") as CryptographySettings;
            var provider =
                settings.SymmetricCryptoProviders.Get(settings.DefaultSymmetricCryptoProviderName) as
                SymmetricAlgorithmProviderData;
            var protectedKey = KeyManager.GenerateSymmetricKey(provider.AlgorithmType, DataProtectionScope.LocalMachine);
            IKeyWriter writer = new KeyReaderWriter();
            writer.Write(stream, protectedKey);
        }

        public static string GetContentType(string extension)
        {
            try
            {
                return MyLifeContext.Settings.MimeTypes[extension].ContentType;
            }
            catch (Exception)
            {
                return "application/octet-stream";
            }
        }

        public static string RemoveHtmlTags(string input)
        {
            return string.IsNullOrEmpty(input) ? input : Regex.Replace(input, @"<(.|\n)*?>", string.Empty);
        }

        public static A GetCustomAttribute<A>(MemberInfo memberInfo) where A : Attribute
        {
            return Attribute.GetCustomAttribute(memberInfo, typeof (A)) as A;
        }

        /// <summary>
        /// Retrieves the value of the specified property of the specified object.
        /// </summary>
        /// <param name="container">The object that contains the property.</param>
        /// <param name="name">The name of the property that contains the value to retrieve.</param>
        /// <returns>The value of the specified property.</returns>
        public static object GetPropertyValue(object container, string name)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            var descriptor = TypeDescriptor.GetProperties(container).Find(name, true);
            if (descriptor == null)
            {
                throw new NullReferenceException("Object does not exist property " + name);
            }
            return descriptor.GetValue(container);
        }

        public static int CalcTotalPages(int total, int size)
        {
            if (size <= 0)
            {
                throw new ArgumentOutOfRangeException("size");
            }

            if (total <= 0)
            {
                return 1;
            }

            return (int) Math.Ceiling(total/(double) size);
        }

        public static Expression<Func<TElement, bool>> BuildContainsExpression<TElement, TValue>(
            Expression<Func<TElement, TValue>> valueSelector, IEnumerable<TValue> values)
        {
            if (null == valueSelector)
            {
                throw new ArgumentNullException("valueSelector");
            }
            if (null == values)
            {
                throw new ArgumentNullException("values");
            }
            var parameter = valueSelector.Parameters.Single();
            if (!values.Any())
            {
                return e => false;
            }
            var equals =
                values.Select(
                    value =>
                    (Expression) Expression.Equal(valueSelector.Body, Expression.Constant(value, typeof (TValue))));
            var body = equals.Aggregate(Expression.Or);
            return Expression.Lambda<Func<TElement, bool>>(body, parameter);
        }

        public static T DynamicCast<T>(object obj)
        {
            return (T) DynamicCast(obj, typeof (T));
        }

        public static object DynamicCast(object obj, Type conversionType)
        {
            if (obj == null || conversionType == null)
            {
                throw new ArgumentNullException();
            }

            var method =
                GetMethod(conversionType, "op_Implicit", BindingFlags.Static | BindingFlags.Public, conversionType,
                          obj.GetType()) ??
                GetMethod(conversionType, "op_Explicit", BindingFlags.Static | BindingFlags.Public, conversionType,
                          obj.GetType());

            if (method == null)
            {
                throw new InvalidCastException(string.Format("Invalid cast from '{0}' to '{1}'.", obj.GetType(),
                                                             conversionType));
            }

            return method.Invoke(null, new[] {obj});
        }

        /// <summary>
        /// Get MethodInfo of type
        /// </summary>
        /// <param name="type">Search type</param>
        /// <param name="methodName">Method name</param>
        /// <param name="bindingFlags"></param>
        /// <param name="return">Return type</param>
        /// <param name="input">Input type</param>
        public static MethodInfo GetMethod(Type type, string methodName, BindingFlags bindingFlags, Type @return,
                                           Type input)
        {
            return
                type.GetMethods(bindingFlags).FirstOrDefault(
                    method =>
                    method.Name == methodName && method.ReturnType == @return && method.GetParameters().Count() == 1 &&
                    method.GetParameters()[0].ParameterType == input);
        }

        public static string Gravatar(string email, int size)
        {
            return string.Format("<img class=\"gravatar\" src=\"{0}\" width=\"{1}\" height=\"{1}\" alt=\"\" />", GravatarUrl(email, size),
                                 size);
        }

        public static string GravatarUrl(string email, int size)
        {
            if (string.IsNullOrEmpty(email))
            {
                return null;
            }
            var md5 = new MD5CryptoServiceProvider();
            var bytes = md5.ComputeHash(Encoding.ASCII.GetBytes(email.ToLowerInvariant()));
            var hash = new StringBuilder();
            for (var i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return string.Format("http://www.gravatar.com/avatar.php?gravatar_id={0}&amp;rating=G&amp;size={1}", hash,
                                 size);
        }

        public static string ComputeHash(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return null;
            }
            var md5 = new MD5CryptoServiceProvider();
            var bytes = md5.ComputeHash(Encoding.ASCII.GetBytes(email.ToLowerInvariant()));
            var hash = new StringBuilder();
            for (var i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }

        public static string GravatarUrl(string hash)
        {
            return string.Format("http://www.gravatar.com/avatar.php?gravatar_id={0}&amp;rating=G", hash);
        }

        public static string[] GetCities()
        {
            var key = "MyLife_GetCities";
            var cache = CacheFactory.GetCacheManager(MyLifeContext.Settings.CacheProvider);
            if (cache.Contains(key))
            {
                return (string[]) cache[key];
            }
            var document = XDocument.Load(Path.Combine(MyLifeContext.WorkingFolder, "App_Data\\Cities.xml"));
            var cities = document.Descendants("City").ToList().Select(item => item.Value).ToArray();
            cache.Add(key, cities);
            return cities;
        }

        #region Extensions

        public static bool NullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }

        #endregion

        #region Json

        public static string GetJsonString(object obj)
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(obj);
        }

        #endregion
    }
}