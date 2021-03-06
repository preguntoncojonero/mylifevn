﻿using System;

namespace MyLife.Serialization
{
    /// <summary>
    /// Represents a globally unique identifier (GUID) with a shorter string value
    /// </summary>
    public struct ShortGuid
    {
        /// <summary>
        /// A read-only instance of the ShortGuid class whose value is guaranteed to be all zeroes
        /// </summary>
        public static readonly ShortGuid Empty = new ShortGuid(Guid.Empty);

        private Guid guid;
        private string value;

        /// <summary>
        /// Creates a ShortGuid from a base64 encoded string
        /// </summary>
        /// <param name="value">The encoded guid as a base64 string</param>
        public ShortGuid(string value)
        {
            this.value = value;
            guid = Decode(value);
        }

        public ShortGuid(Guid guid)
        {
            this.guid = guid;
            value = Encode(guid);
        }

        public Guid Guid
        {
            get { return guid; }
            set
            {
                if (guid == value) return;
                guid = value;
                this.value = Encode(value);
            }
        }

        public string Value
        {
            get { return value; }
            set
            {
                if (this.value == value) return;
                this.value = value;
                guid = Decode(value);
            }
        }

        /// <summary>
        /// Creates a new instance of a Guid using the string value, then returns the base64 encoded version of the Guid
        /// </summary>
        /// <param name="value">An actual Guid string (i.e. not a ShortGuid)</param>
        /// <returns></returns>
        public static string Encode(string value)
        {
            return Encode(new Guid(value));
        }

        /// <summary>
        /// Encodes the given Guid as a base64 string that is 22 characters long
        /// </summary>
        /// <param name="guid">The Guid to encode</param>
        /// <returns></returns>
        private static string Encode(Guid guid)
        {
            var encoded = Convert.ToBase64String(guid.ToByteArray());
            encoded = encoded.Replace("/", "_").Replace("+", "-");
            return encoded.Substring(0, 22);
        }

        /// <summary>
        /// Decode the given base64 string
        /// </summary>
        /// <param name="value">The base64 encoded string of a Guid</param>
        /// <returns></returns>
        public static Guid Decode(string value)
        {
            value = value.Replace("_", "/").Replace("-", "+");
            var buffer = Convert.FromBase64String(value + "==");
            return new Guid(buffer);
        }

        public override string ToString()
        {
            return value;
        }

        public override int GetHashCode()
        {
            return guid.GetHashCode();
        }

        /// <summary>
        /// Returns a value indicating whether this instance and a specified Object represent the same type and value
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is ShortGuid)

                return guid.Equals(((ShortGuid) obj).guid);

            if (obj is Guid)
                return guid.Equals((Guid) obj);

            if (obj is string)
                return guid.Equals(((ShortGuid) obj).guid);

            return false;
        }

        /// <summary>
        /// Determines if both ShortGuids have the same underlying Guid value
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(ShortGuid x, ShortGuid y)
        {
            if ((object) x == null) return (object) y == null;
            return x.guid == y.guid;
        }

        /// <summary>
        /// Determines if both ShortGuids do not have the same underlying Guid value
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(ShortGuid x, ShortGuid y)
        {
            return !(x == y);
        }

        /// <summary>
        /// Implicitly converts the ShortGuid to it's string equivilent
        /// </summary>
        /// <param name="shortGuid"></param>
        /// <returns></returns>
        public static implicit operator string(ShortGuid shortGuid)
        {
            return shortGuid.value;
        }

        /// <summary>
        /// Implicitly converts the ShortGuid to it's Guid equivilent
        /// </summary>
        /// <param name="shortGuid"></param>
        /// <returns></returns>
        public static implicit operator Guid(ShortGuid shortGuid)
        {
            return shortGuid.guid;
        }

        /// <summary>
        /// Implicitly converts the string to a ShortGuid
        /// </summary>
        /// <param name="shortGuid"></param>
        /// <returns></returns>
        public static implicit operator ShortGuid(string shortGuid)
        {
            return new ShortGuid(shortGuid);
        }

        /// <summary>
        /// Implicitly converts the Guid to a ShortGuid
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static implicit operator ShortGuid(Guid guid)
        {
            return new ShortGuid(guid);
        }
    }
}