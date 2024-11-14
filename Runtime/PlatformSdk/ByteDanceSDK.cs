#if ABC_BYTEDANCE_MINIGAME

using System;
using System.Collections.Generic;
using AbcSDKSpace;
using StarkSDKSpace;
using UnityEngine;


namespace PlatformSdk
{
    public class ByteDanceSDK : IPlatformSDK
    {
        
        private DateTime _lastShowTime;
        private DateTime _lastHideTime;
        
        public void InitSystemProperties(Dictionary<string, string> systemProperties)
        {
            var deviceInfo = StarkSDK.API.GetSystemInfo();
            
            systemProperties[Constant.EventPropertiesDeviceModelS] = deviceInfo.model;
            systemProperties[Constant.EventPropertiesManufacturerS] = deviceInfo.brand;
            systemProperties[Constant.EventPropertiesOsS] = deviceInfo.platform;
            systemProperties[Constant.EventPropertiesOSVersionS] = deviceInfo.system;
            systemProperties[Constant.EventPropertiesScreenWidthS] = deviceInfo.screenWidth.ToString();
            systemProperties[Constant.EventPropertiesScreenHeightS] = deviceInfo.screenHeight.ToString();
            systemProperties[Constant.EventPropertiesSystemLanguageS] = deviceInfo.language;
            systemProperties[Constant.EventPropertiesMgPlatformS] = deviceInfo.hostName;
            systemProperties[Constant.EventPropertiesSceneS] = StarkSDK.API.GetLaunchOptionsSync().Scene;
            systemProperties[Constant.EventPropertiesMgVersionS] = StarkSDK.API.GameVersion;
        }

        // 分享事件
        public void InitAutoTrackShare(Action<EventLog> log)
        {
            StarkSDK.API.GetStarkShare().OnShareAppMessage(Result =>
            {
                Debug.Log("share begin");
                log(new EventLog()
                {
                    EventCode = "$mgShare"
                });
                return null;
            });
        }

        public void TrackShow(Action<EventLog> log)
        {
            var lanuch = StarkSDK.API.GetLaunchOptionsSync();
            log(new EventLog()
            {
                EventCode = "$mgShow",
                EventValue = new Dictionary<string, string>
                {
                    { "scene", lanuch.Scene }
                }
            });
            _lastShowTime = DateTime.Now;
        }
        
        public void InitAutoTrackShow(Action<EventLog> log)
        {
            var lanuch = StarkSDK.API.GetLaunchOptionsSync();
            log(new EventLog()
            {
                EventCode = "$mgShow",
                EventValue = new Dictionary<string, string>
                {
                    { "scene", lanuch.Scene }
                }
            });
            _lastShowTime = DateTime.Now;
            StarkSDK.API.GetStarkAppLifeCycle().OnShowWithDict += result =>
            {
                var now = DateTime.Now;
                
                if ((now - _lastShowTime).TotalMilliseconds <= Constant.ThrottleIntervalD)
                {
                    return;
                }
                
                string scene = "";

                if (result.TryGetValue("scene", out object value) && value is string sceneValue)
                {
                    scene = sceneValue;
                    Debug.Log($"scene: ${scene}");
                }
                
                // 热启动上报
                log(new EventLog()
                {
                    EventCode = "$mgShow",
                    EventValue = new Dictionary<string, string>
                    {
                        { "scene", scene }
                    }
                });
                _lastShowTime = now; 
            };
        }

        public void InitAutoTrackHide(Action<EventLog> log, Action flushAll)
        {
            StarkSDK.API.GetStarkAppLifeCycle().OnHide += () =>
            {
                var now = DateTime.Now;
                
                if (_lastHideTime != null && (now - _lastHideTime).TotalMilliseconds <= Constant.ThrottleIntervalD)
                {
                    return;
                }

                log(new EventLog()
                {
                    EventCode = "$mgHide"
                });
                // 游戏进入后台之后强制上报所有数据
                flushAll();
                _lastHideTime = now;
            };
        }
    }
}

#endif