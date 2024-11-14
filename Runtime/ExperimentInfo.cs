using System;
using System.Collections.Generic;
using UnityEngine;

namespace AbcSDKSpace
{
    public class ExperimentInfo
    {
        public int ExpId;

        internal string ExpKey;

        internal string ExpGroupKey;

        internal string ModuleCode;

        internal Dictionary<string, string> Params;

        public ExperimentInfo()
        {
            ExpId = -1;
        }

        /// <summary>
        /// Retrieves the string value associated with the specified key.
        /// If the key does not exist, returns the provided default value or null if no default is provided.
        /// </summary>
        public string GetStringValue(string key, string defaultValue = default)
        {
            if (Params != null && Params.TryGetValue(key, out var value))
            {
                return value;
            }

            return defaultValue;
        }

        /// <summary>
        /// Retrieves the Boolean value associated with the specified key.
        /// If conversion fails or the key does not exist, returns the provided default value or default(bool).
        /// </summary>
        public bool GetBoolValue(string key, bool defaultValue = default)
        {
            if (Params != null && Params.TryGetValue(key, out var value))
            {
                if (bool.TryParse(value, out var result))
                {
                    return result;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// Retrieves the integer value associated with the specified key.
        /// If conversion fails or the key does not exist, returns the provided default value or default(int).
        /// </summary>
        public int GetIntValue(string key, int defaultValue = default)
        {
            if (Params != null && Params.TryGetValue(key, out var value))
            {
                if (int.TryParse(value, out var result))
                {
                    return result;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// Retrieves the double value associated with the specified key.
        /// If conversion fails or the key does not exist, returns the provided default value or default(double).
        /// </summary>
        public double GetDoubleValue(string key, double defaultValue = default)
        {
            if (Params != null && Params.TryGetValue(key, out var value))
            {
                if (double.TryParse(value, out var result))
                {
                    return result;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// Retrieves the DateTime value associated with the specified key.
        /// If conversion fails or the key does not exist, returns the provided default value or default(DateTime).
        /// </summary>
        public DateTime GetDateTimeValue(string key, DateTime defaultValue = default)
        {
            if (Params != null && Params.TryGetValue(key, out var value))
            {
                if (DateTime.TryParse(value, out var result))
                {
                    return result;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// Retrieves the decimal value associated with the specified key.
        /// If conversion fails or the key does not exist, returns the provided default value or default(decimal).
        /// </summary>
        public decimal GetDecimalValue(string key, decimal defaultValue = default)
        {
            if (Params != null && Params.TryGetValue(key, out var value))
            {
                if (decimal.TryParse(value, out var result))
                {
                    return result;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// Retrieves the value associated with the specified key and tries to convert it to the specified type.
        /// If conversion fails or the key does not exist, returns the provided default value or default(T).
        /// </summary>
        public T GetValue<T>(string key, T defaultValue = default)
        {
            if (Params != null && Params.TryGetValue(key, out var value))
            {
                try
                {
                    // Handle nullable types
                    var targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

                    if (targetType.IsEnum)
                    {
                        if (Enum.TryParse(targetType, value, out var enumResult))
                        {
                            return (T)enumResult;
                        }
                    }
                    else
                    {
                        var convertedValue = Convert.ChangeType(value, targetType);
                        return (T)convertedValue;
                    }
                }
                catch
                {
                    // Conversion failed, fall through to return defaultValue
                }
            }

            return defaultValue;
        }
    }
}