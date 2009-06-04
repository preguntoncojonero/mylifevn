using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Globalization;

namespace MyLife.Web.Security
{
    public static class SecUtility
    {
        public static void CheckArrayParameter(ref string[] param, bool checkForNull, bool checkIfEmpty,
                                               bool checkForCommas, int maxSize, string paramName)
        {
            if (param == null)
            {
                throw new ArgumentNullException(paramName);
            }
            if (param.Length < 1)
            {
                throw new ArgumentException(string.Format("The array parameter '{0}' should not be empty.", paramName));
            }
            var hashtable = new Hashtable(param.Length);
            for (var i = param.Length - 1; i >= 0; i--)
            {
                CheckParameter(ref param[i], checkForNull, checkIfEmpty, checkForCommas, maxSize,
                               paramName + "[ " + i.ToString(CultureInfo.InvariantCulture) + " ]");
                if (hashtable.Contains(param[i]))
                {
                    throw new ArgumentException(
                        string.Format("The array '{0}' should not contain duplicate values.", paramName));
                }
                hashtable.Add(param[i], param[i]);
            }
        }

        public static void CheckParameter(ref string param, bool checkForNull, bool checkIfEmpty, bool checkForCommas,
                                          int maxSize, string paramName)
        {
            if (param == null)
            {
                if (checkForNull)
                {
                    throw new ArgumentNullException(paramName);
                }
            }
            else
            {
                param = param.Trim();
                if (checkIfEmpty && (param.Length < 1))
                {
                    throw new ArgumentException(string.Format("The parameter '{0}' must not be empty.", paramName));
                }
                if ((maxSize > 0) && (param.Length > maxSize))
                {
                    throw new ArgumentException(
                        string.Format("The parameter '{0}' is too long: it must not exceed {1} chars in length.",
                                      paramName, maxSize.ToString(CultureInfo.InvariantCulture)));
                }
                if (checkForCommas && param.Contains(","))
                {
                    throw new ArgumentException(string.Format("The parameter '{0}' must not contain commas.", paramName));
                }
            }
        }

        public static void CheckPasswordParameter(ref string param, int maxSize, string paramName)
        {
            if (param == null)
            {
                throw new ArgumentNullException(paramName);
            }
            if (param.Length < 1)
            {
                throw new ArgumentException(string.Format("The parameter '{0}' must not be empty.", paramName));
            }
            if ((maxSize > 0) && (param.Length > maxSize))
            {
                throw new ArgumentException(
                    string.Format("The parameter '{0}' is too long: it must not exceed {1} chars in length.", paramName,
                                  maxSize.ToString(CultureInfo.InvariantCulture)));
            }
        }

        public static bool GetBooleanValue(NameValueCollection config, string valueName, bool defaultValue)
        {
            bool flag;
            var str = config[valueName];
            if (str == null)
            {
                return defaultValue;
            }
            if (!bool.TryParse(str, out flag))
            {
                throw new ProviderException(
                    string.Format("The value must be boolean (true or false) for property '{0}'.", valueName));
            }
            return flag;
        }

        public static int GetIntValue(NameValueCollection config, string valueName, int defaultValue, bool zeroAllowed,
                                      int maxValueAllowed)
        {
            int num;
            var s = config[valueName];
            if (s == null)
            {
                return defaultValue;
            }
            if (!int.TryParse(s, out num))
            {
                if (zeroAllowed)
                {
                    throw new ProviderException(
                        string.Format("The value must be a non-negative 32-bit integer for property '{0}'.", valueName));
                }
                throw new ProviderException(
                    string.Format("The value must be a positive 32-bit integer for property '{0}'.", valueName));
            }
            if (zeroAllowed && (num < 0))
            {
                throw new ProviderException(
                    string.Format("The value must be a non-negative 32-bit integer for property '{0}'.", valueName));
            }
            if (!zeroAllowed && (num <= 0))
            {
                throw new ProviderException(
                    string.Format("The value must be a positive 32-bit integer for property '{0}'.", valueName));
            }
            if ((maxValueAllowed > 0) && (num > maxValueAllowed))
            {
                throw new ProviderException(
                    string.Format("The value '{0}' can not be greater than '{1}'.", valueName,
                                  maxValueAllowed.ToString(CultureInfo.InvariantCulture)));
            }
            return num;
        }

        public static bool ValidateParameter(ref string param, bool checkForNull, bool checkIfEmpty,
                                             bool checkForCommas, int maxSize)
        {
            if (param == null)
            {
                return !checkForNull;
            }
            param = param.Trim();
            return
                (((!checkIfEmpty || (param.Length >= 1)) && ((maxSize <= 0) || (param.Length <= maxSize))) &&
                 (!checkForCommas || !param.Contains(",")));
        }

        public static bool ValidatePasswordParameter(ref string param, int maxSize)
        {
            if (param == null)
            {
                return false;
            }
            if (param.Length < 1)
            {
                return false;
            }
            if ((maxSize > 0) && (param.Length > maxSize))
            {
                return false;
            }
            return true;
        }
    }
}