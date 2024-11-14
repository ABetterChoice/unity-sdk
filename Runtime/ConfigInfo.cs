using System;
using UnityEngine;

namespace AbcSDKSpace
{
    public class ConfigInfo
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public ExperimentInfo RelativeExperiment { get; set; }

        /// <summary>
        /// Returns the Value as a string.
        /// </summary>
        public string GetStringValue()
        {
            return Value;
        }

        public string GetStringValue(string defaultValue)
        {
            return Value == "" ? default : Value;
        }

        /// <summary>
        /// Attempts to parse the Value as an integer.
        /// Throws a FormatException if parsing fails.
        /// </summary>
        public int GetIntValue()
        {
            return int.Parse(Value);
        }

        /// <summary>
        /// Attempts to parse the Value as an integer.
        /// Returns defaultValue if parsing fails.
        /// </summary>
        public int GetIntValue(int defaultValue)
        {
            if (int.TryParse(Value, out int result))
            {
                return result;
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Attempts to parse the Value as a boolean.
        /// Throws a FormatException if parsing fails.
        /// </summary>
        public bool GetBoolValue()
        {
            return bool.Parse(Value);
        }

        /// <summary>
        /// Attempts to parse the Value as a boolean.
        /// Returns defaultValue if parsing fails.
        /// </summary>
        public bool GetBoolValue(bool defaultValue)
        {
            if (bool.TryParse(Value, out bool result))
            {
                return result;
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Attempts to parse the Value as a float.
        /// Throws a FormatException if parsing fails.
        /// </summary>
        public float GetFloatValue()
        {
            return float.Parse(Value);
        }

        /// <summary>
        /// Attempts to parse the Value as a float.
        /// Returns defaultValue if parsing fails.
        /// </summary>
        public float GetFloatValue(float defaultValue)
        {
            if (float.TryParse(Value, out float result))
            {
                return result;
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Attempts to parse the Value as a double.
        /// Throws a FormatException if parsing fails.
        /// </summary>
        public double GetDoubleValue()
        {
            return double.Parse(Value);
        }

        /// <summary>
        /// Attempts to parse the Value as a double.
        /// Returns defaultValue if parsing fails.
        /// </summary>
        public double GetDoubleValue(double defaultValue)
        {
            if (double.TryParse(Value, out double result))
            {
                return result;
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Attempts to parse the Value as a long.
        /// Throws a FormatException if parsing fails.
        /// </summary>
        public long GetLongValue()
        {
            return long.Parse(Value);
        }

        /// <summary>
        /// Attempts to parse the Value as a long.
        /// Returns defaultValue if parsing fails.
        /// </summary>
        public long GetLongValue(long defaultValue)
        {
            if (long.TryParse(Value, out long result))
            {
                return result;
            }
            else
            {
                return defaultValue;
            }
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