using System;
using System.Text;
using System.Threading;
using UnityEngine.Networking;

namespace AbcSDKSpace
{
    public static class Utils
    {
        // 为每个线程创建独立的 Random 实例
        private static readonly ThreadLocal<Random> random =
            new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));

        public static string Uuid()
        {
            // 获取当前的 Unix 时间戳（毫秒）
            long visitTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            StringBuilder tenDigits = new StringBuilder();

            // 生成 10 位随机数字
            for (int i = 0; i < 10; i++)
            {
                // 生成 0 到 9 的随机整数
                int digit = random.Value.Next(0, 10);
                
                tenDigits.Append(digit);
            }

            // 拼接结果字符串
            string result = $"{tenDigits}-{visitTime}";
            
            return result;
        }

        public static void SetRequestHeaders(UnityWebRequest webRequest, string apiKey)
        {
            var et = TokenUtils.GetTimestamp();
            var ak = TokenUtils.GetAppKeyFromToken(apiKey);
            var es = TokenUtils.GenSecretKey(et, apiKey, ak);
            webRequest.SetRequestHeader("X-Token", apiKey);
            webRequest.SetRequestHeader("X-AK", ak);
            webRequest.SetRequestHeader("X-Et", et);
            webRequest.SetRequestHeader("X-Es", es);
            webRequest.SetRequestHeader("Content-Type", "application/json");
        }
    }
}