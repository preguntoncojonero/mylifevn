using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MyLife.Serialization
{
    public static class Base64Serializer
    {
        public static string Serialize(object obj)
        {
            using (var memStream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(memStream, obj);

                return Convert.ToBase64String(memStream.ToArray());
            }
        }

        public static T Deserialize<T>(string str)
        {
            using (var memStream = new MemoryStream(Convert.FromBase64String(str)))
            {
                return (T) new BinaryFormatter().Deserialize(memStream);
            }
        }

        public static object Deserialize(string str)
        {
            return Deserialize<object>(str);
        }
    }
}