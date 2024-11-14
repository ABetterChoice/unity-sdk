using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using Object = System.Object;

namespace AbcSDKSpace
{
    internal class TokenUtils : MonoBehaviour
    {
        internal static string GetTimestamp()
        {
            DateTime utcNow = DateTime.UtcNow;
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long timestampInSeconds = (long)(utcNow - epoch).TotalSeconds;
            return timestampInSeconds.ToString();
        }

        internal static string GetAppKeyFromToken(string tokenKey)
        {
            var items = tokenKey.Split(".");
            if (items.Length != 3)
            {
                return "";
            }

            var base64String = items[1];
            if (base64String.Length % 4 > 0)
            {
                base64String = base64String.PadRight(base64String.Length + 4 - base64String.Length % 4, '=');
            }
            
            string plainTxt = Encoding.UTF8.GetString(Convert.FromBase64String(base64String));
            var token = Token.Deserialize(plainTxt);
            return token.TokenName;
        }

        internal static string GenSecretKey(string timestamp, string tokenKey, string ak)
        {
            return CalculateMD5Hash(tokenKey + ak + timestamp);
        }

        internal static string CalculateMD5Hash(string input)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    sb.Append(data[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }
    }
}