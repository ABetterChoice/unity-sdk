using System;
using UnityEngine;

namespace AbcSDKSpace
{
    public class ConfigInfo
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public ExperimentInfo RelativeExperiment { get; set; }

        public string GetStringValue(string defaultValue = default)
        {
            return Value == "" ? defaultValue : Value;
        }

        /// <summary>
        /// Attempts to parse the Value as an integer.
        /// Returns defaultValue if parsing fails.
        /// </summary>
        public int GetIntValue(int defaultValue = default)
        {
            if (int.TryParse(Value, out int result))
            {
                return result;
            }

            return defaultValue;
        }

        /// <summary>
        /// Attempts to parse the Value as a boolean.
        /// Returns defaultValue if parsing fails.
        /// </summary>
        public bool GetBoolValue(bool defaultValue = default)
        {
            if (bool.TryParse(Value, out bool result))
            {
                return result;
            }

            if (int.TryParse(Value, out int intValue))
            {
                return intValue != 0;
            }
            
            return defaultValue;
        }

        /// <summary>
        /// Attempts to parse the Value as a float.
        /// Returns defaultValue if parsing fails.
        /// </summary>
        public float GetFloatValue(float defaultValue = default)
        {
            if (float.TryParse(Value, out float result))
            {
                return result;
            }

            return defaultValue;
        }

        /// <summary>
        /// Attempts to parse the Value as a double.
        /// Returns defaultValue if parsing fails.
        /// </summary>
        public double GetDoubleValue(double defaultValue = default)
        {
            if (double.TryParse(Value, out double result))
            {
                return result;
            }

            return defaultValue;
        }

        /// <summary>
        /// Attempts to parse the Value as a long.
        /// Returns defaultValue if parsing fails.
        /// </summary>
        public long GetLongValue(long defaultValue = default)
        {
            if (long.TryParse(Value, out long result))
            {
                return result;
            }

            return defaultValue;
        }

        /// <summary>
        /// Attempts to parse the Value as a JSON array and returns it as an array of strings.
        /// </summary>
        public string[] GetArrayValue()
        {
            try
            {
                return JsonUtility.FromJson<Wrapper<string>>(WrapArray(Value)).Items;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error parsing Value as array: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Attempts to parse the Value as a JSON array and returns it as an array of the specified type.
        /// </summary>
        public T[] GetArrayValue<T>()
        {
            try
            {
                return JsonUtility.FromJson<Wrapper<T>>(WrapArray(Value)).Items;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error parsing Value as array of {typeof(T)}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Attempts to parse the Value as JSON and returns it as an object of type T.
        /// </summary>
        public T GetJsonValue<T>()
        {
            try
            {
                return JsonUtility.FromJson<T>(Value);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error parsing Value as {typeof(T)}: {ex.Message}");
                return default(T);
            }
        }

        /// <summary>
        /// Helper method to wrap a JSON array string into a JSON object for Unity's JsonUtility.
        /// </summary>
        private string WrapArray(string jsonArray)
        {
            return $"{{\"Items\":{jsonArray}}}";
        }

        /// <summary>
        /// Wrapper class for JSON array parsing.
        /// </summary>
        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }
}