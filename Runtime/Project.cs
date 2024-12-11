#define UNITY_WEBGL

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using PlatformSdk;

namespace AbcSDKSpace
{
    internal class Project : MonoBehaviour
    {
        private User _user;
        private Config _config;

        private bool _shutdown = false;

        // 移除 volatile 关键字，WebGL 单线程环境下不需要
        private Dictionary<string, Experiment> _expData;
        private Dictionary<string, FeatureFlag> _dynamicConfigData;
        private ControlData _controlData;
        private GetEventSettingResp _eventSettingResp;

        private EventLogger _eventLogger;
        private GameObject _abcGameObject;

        private Dictionary<string, string> _commonProperties = new();
        private Dictionary<string, string> _systemProperties = new Dictionary<string, string>();
        
        private DateTime _lastShowTime;
        private DateTime _lastHideTime;

        public IEnumerator Init(Config config, Action<Result> callback)
        {
            this._config = config;
            this._config.DeviceId = User.Uuid(this._config.GameId);
            if (string.IsNullOrEmpty(this._config.UnitId))
            {
                this._config.UnitId = User.GetUnitID(this._config.GameId);
            }
            else
            {
                User.SetUnitID(this._config.GameId, this._config.UnitId);
            }
            
            InitEventLogger();
            
            try
            {
                PlatformSDKManager.Instance.InitSystemProperties(_systemProperties);
                if (_eventLogger != null)
                {
                    _eventLogger.UpdateSystemProperties(_systemProperties);
                }

                if (this._config.AutoTrack.MgShare)
                {
                    PlatformSDKManager.Instance.InitAutoTrackShare(LogEvent);
                }

                if (this._config.AutoTrack.MgShow)
                {
                    PlatformSDKManager.Instance.InitAutoTrackShow(LogEvent);
                }

                if (this._config.AutoTrack.MgHide)
                {
                    PlatformSDKManager.Instance.InitAutoTrackHide(LogEvent, _eventLogger.FlushALL);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"{e}");
            }


            Result result = null;

            // 调用 DoInit 协程
            yield return StartCoroutine(DoInit(r => { result = r; }));

            if (result?.statusCode == StatusCode.Success && this._config.EnableAutoPoll)
            {
                StartCoroutine(Watch());
            }

            // 回调返回结果
            callback(result);
        }

        public void UpdateUnitID(string unitID)
        {
            // WebGL 单线程环境，不需要加锁
            if (_config.UnitId == unitID)
            {
                return;
            }
            _config.UnitId = unitID;
            _eventLogger.UpdateConfig(_config);
            User.SetUnitID(this._config.GameId, unitID);
            if (this._config.AutoTrack.MgShow)
            {
                PlatformSDKManager.Instance.TrackShow(LogEvent);
            }
        }

        private IEnumerator Watch()
        {
            while (true)
            {
                yield return new WaitForSeconds(
                    _controlData.RefreshDuration <= 0 ? 600 : _controlData.RefreshDuration);

                if (_shutdown)
                {
                    yield break;
                }

                // 调用 DoInit 协程并等待结果
                Result result = null;
                
                yield return StartCoroutine(DoInit(r =>
                {
                    result = r;
                }));

                if (result?.statusCode != StatusCode.Success)
                {
                    Debug.LogError($"DoInit fail: {result?.message}");
                }
            }
        }

        public void SetCommonProperties(Dictionary<string, string> commonProperties)
        {
            this._commonProperties = commonProperties;
        }

        private void InitEventLogger()
        {
            _abcGameObject = new GameObject("AbcEvent");
            _eventLogger = _abcGameObject.AddComponent<EventLogger>();
            UnityEngine.Object.DontDestroyOnLoad(_abcGameObject);
            _eventLogger.Init(this._config);
            
            // 启动心跳事件
            if (_config.AutoTrack.MgHide)
            {
                StartCoroutine(Heartbeat());
            }
        }
        
        private IEnumerator Heartbeat()
        {
            while (!_shutdown)
            {
                // 每隔2分钟执行一次
                yield return new WaitForSeconds(120);
                // 记录心跳事件
                LogEvent(new EventLog
                {
                    EventCode = "$mgHeart",
                    EventValue = new Dictionary<string, string> {}
                });
            }
        }

        private IEnumerator DoInit(Action<Result> callback)
        {
            Result finalResult = new Result(StatusCode.Success);

            // 调用 InitCache 协程
            yield return StartCoroutine(InitCache(cacheResult =>
            {
                finalResult = cacheResult;
            }));

            if (finalResult.statusCode != StatusCode.Success)
            {
                callback(finalResult);
                yield break;
            }

            // 调用 InitDynamicConfig 协程
            yield return StartCoroutine(InitDynamicConfig(configResult =>
            {
                finalResult = configResult;
            }));
            
            if (finalResult.statusCode != StatusCode.Success)
            {
                callback(finalResult);
                yield break;
            }
            
            // 调用 InitEventSetting 协程
            yield return StartCoroutine(InitEventSetting(eventResult =>
            {
                finalResult = eventResult;
            }));
            
            // 更新上报配置
            _eventLogger.UpdateControlData(_controlData);
            
            // 返回最终结果
            callback(finalResult);
        }

        public void Shutdown()
        {
            this._shutdown = true;
            return;
        }

        public Experiment GetExperimentByLayerKey(string layerKey)
        {
            Experiment exp = _expData?.GetValueOrDefault(layerKey);
            exp ??= new Experiment();
            if (IsAutoLogExposure())
            {
                LogExposure(new ExposureData(exp.ExpId));
            }

            return exp;
        }

        private bool IsAutoLogExposure()
        {
            if (_config == null)
            {
                return false;
            }

            if (_controlData == null)
            {
                return false;
            }
            return this._config.EnableAutoExposure && _controlData.EnableReport;
        }

        public FeatureFlag GetDynamicConfig(string key)
        {
            FeatureFlag config = _dynamicConfigData?.GetValueOrDefault(key);
            if (IsAutoLogExposure() && config is { RelativeExperiment: not null })
            {
                LogExposure(new ExposureData(config.RelativeExperiment.GroupId));
            }

            return config ?? new FeatureFlag();
        }

        public void LogEvent(EventLog entry)
        {
            if (entry == null)
            {
                return;
            }

            if (_eventSettingResp != null)
            {
                EventSetting setting;
                if (_eventSettingResp.EventSetting.TryGetValue(entry.EventCode, out setting))
                {
                    if (setting != null && !setting.Enable)
                    {
                        return;
                    }
                }
            }
            
            if (entry.EventValue == null)
            {
                entry.EventValue = new();
            }

            foreach (var p in this._commonProperties)
            {
                if (p.Key == null || p.Value == null)
                {
                    continue;
                }

                entry.EventValue.TryAdd(p.Key, p.Value);
            }

            // 在 WebGL 单线程环境下，不需要锁
            entry.Properties = _systemProperties;

            entry.User = new User()
            {
                UserID = _config.UnitId,
                DeviceID = _config.DeviceId
            };

            entry.EventTime = TokenUtils.GetTimestamp();

            if (_eventLogger != null)
            {
                _eventLogger.EnqueueEvent(entry);
            }
            else
            {
                Debug.LogError("EventLogger is not initialized.");
            }
        }

        public void LogExposure(ExposureData entry)
        {
            if (entry == null)
            {
                return;
            }
            
            _eventLogger.EnqueueExposure(entry);
        }

        private IEnumerator InitCache(Action<Result> callback)
        {
            Result result = new Result(StatusCode.Success);
            GetExperimentsReq req = new GetExperimentsReq();
            req.Guid = _config.UnitId;
            req.Profiles = new();
            if (_config?.Attributes != null)
            {
                foreach (var a in this._config.Attributes)
                {
                    req.Profiles.Add(a.Key, new ProfileValues(a.Value));
                }
            }

            req.AppId = this._config.GameId;
            // 创建UnityWebRequest对象
            UnityWebRequest webRequest =
                new UnityWebRequest("https://mobile.abetterchoice.cn/tab/get_experiments", "POST");
            byte[] jsonToSend = new UTF8Encoding().GetBytes(req.Serialize());
            webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            // 添加自定义的头部信息
            Utils.SetRequestHeaders(webRequest, this._config.ApiKey);
            // 设置超时时间
            webRequest.timeout = 5;
            // 发送请求并等待结果
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                result.statusCode = StatusCode.InvalidParamErr;
                result.message = $"[InitCache] UnityWebRequest error: {webRequest.error}";
                Debug.LogError(result.message);
            }
            else
            {
                string content = webRequest.downloadHandler.text;
                var res = GetExperimentsRes.Deserialize(content);
                if (res.RetCode != RetCode.Success)
                {
                    result.statusCode = StatusCode.InvalidParamErr;
                    result.message = $"retCode={res.RetCode}, message={res.Msg}";
                    Debug.LogError(result.message);
                }
                else
                {
                    // 直接赋值，无需使用 Interlocked.Exchange
                    _controlData = res.ControlData;
                    _expData = res.ExpData;
                }
            }

            // 调用回调函数，传递结果
            callback(result);
        }

        private IEnumerator InitDynamicConfig(Action<Result> callback)
        {
            Result result = new Result(StatusCode.Success);
            GetFeaturesReq req = new();
            req.UnitID = _config.UnitId;
            req.Profiles = new();
            if (_config?.Attributes != null)
            {
                foreach (var a in this._config.Attributes)
                {
                    req.Profiles.Add(a.Key, new ProfileValues(a.Value));
                }
            }

            req.SceneIDs = new();
            req.ProjectID = this._config.GameId;
            // 创建UnityWebRequest对象
            UnityWebRequest webRequest =
                new UnityWebRequest("https://mobile.abetterchoice.cn/tab/get_feature_flags", "POST");
            byte[] jsonToSend = new UTF8Encoding().GetBytes(req.Serialize());
            webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            // 添加自定义的头部信息
            Utils.SetRequestHeaders(webRequest, this._config.ApiKey);
            // 设置超时时间
            webRequest.timeout = 5;
            // 发送请求并等待结果
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                result.statusCode = StatusCode.InvalidParamErr;
                result.message = $"[InitDynamicConfig] UnityWebRequest error: {webRequest.error}";
                Debug.LogError(result.message);
            }
            else
            {
                string content = webRequest.downloadHandler.text;
                var res = GetFeatureFlagsRsp.Deserialize(content);

                if (res.RetCode != RetCode.Success)
                {
                    result.statusCode = StatusCode.InvalidParamErr;
                    result.message = $"retCode={res.RetCode}, message={res.Msg}";
                    Debug.LogError(result.message);
                }
                else
                {
                    // 直接赋值，无需使用 Interlocked.Exchange
                    _dynamicConfigData = res.Data;
                }
            }

            // 调用回调函数，传递结果
            callback(result);
        }
        
        private IEnumerator InitEventSetting(Action<Result> callback)
        {
            Result result = new Result(StatusCode.Success);
            GetEventSettingReq req = new();
            req.Version = "";
            if (_eventSettingResp != null)
            {
                req.Version = _eventSettingResp.Version;
            }
            req.AppID = this._config.GameId;

            // 创建UnityWebRequest对象
            UnityWebRequest webRequest =
                new UnityWebRequest("https://data.abetterchoice.cn/v2/eventSetting/get", "POST");
            byte[] jsonToSend = new UTF8Encoding().GetBytes(req.Serialize());
            webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            // 添加自定义的头部信息
            Utils.SetRequestHeaders(webRequest, this._config.ApiKey);
            // 设置超时时间
            webRequest.timeout = 5;
            // 发送请求并等待结果
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                result.statusCode = StatusCode.InvalidParamErr;
                result.message = $"[InitEventSetting] UnityWebRequest error: {webRequest.error}";
                Debug.LogError(result.message);
            }
            else
            {
                string content = webRequest.downloadHandler.text;
                var res = GetEventSettingResp.Deserialize(content);

                if (res.CommonResp == null)
                {
                    result.statusCode = StatusCode.InvalidParamErr;
                    result.message = "[InitEventSetting] CommonResp is null";
                    Debug.LogError(result.message);
                }
                else if (res.CommonResp.Code == "EVENT_SERVER_CODE_SAME_VERSION")
                {
                }
                else if (res.CommonResp.Code != "EVENT_SERVER_CODE_SUCCESS")
                {
                    result.statusCode = StatusCode.InvalidParamErr;
                    result.message = $"retCode={res.CommonResp.Code}, message={res.CommonResp.Message}";
                    Debug.LogError(result.message);
                }
                else
                {
                    // 直接赋值，无需使用 Interlocked.Exchange
                    _eventSettingResp = res;
                }
            }

            // 调用回调函数，传递结果
            callback(result);
        }
    }
}