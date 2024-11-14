using System;
using System.Collections.Generic;
using AbcSDKSpace;


namespace PlatformSdk
{
    public class PlatformSDKManager
    {
        private static IPlatformSDK instance;

        public static IPlatformSDK Instance
        {
            get
            {
                if (instance == null)
                {
#if ABC_WECHAT_MINIGAME
                instance = new WeChatSDK();
#elif ABC_BYTEDANCE_MINIGAME
                instance = new ByteDanceSDK();
#else
                    UnityEngine.Debug.LogWarning("未定义平台宏定义，默认使用微信小游戏平台。");
                    instance = new WeChatSDK();
#endif
                }
                return instance;
            }
        }
    }
    
    public interface IPlatformSDK
    {
        // 初始化系统属性
        void InitSystemProperties(Dictionary<string, string> systemProperties);
        // 分享事件
        void InitAutoTrackShare(Action<EventLog> log);
        void InitAutoTrackShow(Action<EventLog> log);
        void InitAutoTrackHide(Action<EventLog> log, Action flushAll);
        void TrackShow(Action<EventLog> log);
    }
}