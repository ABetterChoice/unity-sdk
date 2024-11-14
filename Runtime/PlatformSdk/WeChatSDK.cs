#if ABC_WECHAT_MINIGAME

using System;
using System.Collections.Generic;
using AbcSDKSpace;
using UnityEngine;
using WeChatWASM;

namespace PlatformSdk
{
    public class WeChatSDK : IPlatformSDK
    {
        
        private DateTime _lastShowTime;
        private DateTime _lastHideTime;
        
        public void InitSystemProperties(Dictionary<string, string> systemProperties)
        {
            // 由于 WebGL 单线程环境，不需要加锁
            WX.GetNetworkType(new GetNetworkTypeOption()
            {
                success = result => { systemProperties[Constant.EventPropertiesNetworkTypeS] = result.networkType; }
            });

            try
            {
                var deviceInfo = WX.GetDeviceInfo();
                systemProperties[Constant.EventPropertiesDeviceModelS] = deviceInfo.model;
                systemProperties[Constant.EventPropertiesManufacturerS] = deviceInfo.brand;
                systemProperties[Constant.EventPropertiesOsS] = deviceInfo.platform;
                systemProperties[Constant.EventPropertiesOSVersionS] = deviceInfo.system;

                var windowInfo = WX.GetWindowInfo();
                systemProperties[Constant.EventPropertiesScreenWidthS] = windowInfo.screenWidth.ToString();
                systemProperties[Constant.EventPropertiesScreenHeightS] = windowInfo.screenHeight.ToString();

                var baseInfo = WX.GetAppBaseInfo();
                systemProperties[Constant.EventPropertiesSystemLanguageS] = baseInfo.language;

                var enterInfo = WX.GetEnterOptionsSync();
                systemProperties[Constant.EventPropertiesSceneS] = enterInfo.scene.ToString();

                var accountInfo = WX.GetAccountInfoSync();
                systemProperties[Constant.EventPropertiesMgVersionS] = accountInfo.miniProgram.version;
                systemProperties[Constant.EventPropertiesMgPlatformS] = "WXMG";
            }
            catch (Exception ex)
            {
                Debug.LogError($"InitSystemProperties error: {ex}");
            }
        }
        
        public void InitAutoTrackShare(Action<EventLog> log)
        {
            WX.OnShareAppMessage(new WXShareAppMessageParam(), result =>
            {
                log(new EventLog()
                {
                    EventCode = "$mgShare"
                });
            });
            WX.OnShareTimeline(result =>
            {
                log(new EventLog()
                {
                    EventCode = "$mgShare"
                });
            });
        }
        
        public void TrackShow(Action<EventLog> log)
        {
            var lanuch = WX.GetLaunchOptionsSync();
            log(new EventLog()
            {
                EventCode = "$mgShow",
                EventValue = new Dictionary<string, string>
                {
                    { "scene", lanuch.scene.ToString() }
                }
            });
            _lastShowTime = DateTime.Now;
        }
        
        public  void InitAutoTrackShow(Action<EventLog> log)
        {
            
            WX.GetAppBaseInfo();
            // 获取冷启动的时候的参数
            var launch = WX.GetLaunchOptionsSync();
            // 冷启动上报
            log(new EventLog()
            {
                EventCode = "$mgShow",
                EventValue = new Dictionary<string, string>
                {
                    { "scene", launch.scene.ToString() }
                }
            });
            _lastShowTime = DateTime.Now;
            // 注册热启动埋点事件
            WX.OnShow(result =>
            {
                var now = DateTime.Now;
                if ((now - _lastShowTime).TotalMilliseconds <= Constant.ThrottleIntervalD)
                {
                    return;
                }

                // 热启动上报
                log(new EventLog()
                {
                    EventCode = "$mgShow",
                    EventValue = new Dictionary<string, string>
                    {
                        { "scene", result.scene.ToString() }
                    }
                });
                _lastShowTime = now;
            });
        }
        
        public void InitAutoTrackHide(Action<EventLog> log, Action flushALL)
        {
            WX.OnHide(result =>
            {
                var now = DateTime.Now;
                if ((now - _lastHideTime).TotalMilliseconds <= Constant.ThrottleIntervalD)
                {
                    return;
                }

                log(new EventLog()
                {
                    EventCode = "$mgHide"
                });
                // 游戏进入后台之后强制上报所有数据
                flushALL();
                _lastHideTime = now;
            });
        }
    }
}

#endif