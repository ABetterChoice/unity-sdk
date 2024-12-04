using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbcSDKSpace
{
    public static class ABetterChoiceAPI
    {
        private static bool isInitializing = false;
        private static bool isInitialized = false;
        private static Project _projectInstance;
        private static Queue<Action<Result>> initCallbacks = new Queue<Action<Result>>();

        private static string pendingUnitID;

        /// <summary>
        /// 接受回调函数的 Init 方法
        /// </summary>
        /// <param name="config"></param>
        /// <param name="callback"></param>
        public static void Init(Config config, Action<Result> callback)
        {
            if (isInitialized)
            {
                // 初始化已经完成，直接回调成功结果
                callback(new Result(StatusCode.Success));
                return;
            }

            // 将回调添加到队列
            initCallbacks.Enqueue(callback);

            if (!isInitializing)
            {
                isInitializing = true;
                // 启动初始化协程
                CoroutineRunner.Instance.StartCoroutine(InitializeCoroutine(config));
            }
        }

        private static IEnumerator InitializeCoroutine(Config config)
        {
            if (_projectInstance == null)
            {
                // 创建 GameObject 并添加 Project 组件
                GameObject go = new GameObject("AbcProject");
                UnityEngine.Object.DontDestroyOnLoad(go);
                _projectInstance = go.AddComponent<Project>();
            }

            Result result = null;

            // 使用 login 的 id 作为 uinitid
            if (string.IsNullOrEmpty(config.UnitId) && string.IsNullOrEmpty(pendingUnitID))
            {
                config.UnitId = pendingUnitID;
            }
            
            // 调用 Project 的 Init 协程
            yield return _projectInstance.Init(config, res =>
            {
                result = res;
            });

            isInitialized = result?.statusCode == StatusCode.Success;

            // 执行 login
            if (!string.IsNullOrEmpty(pendingUnitID))
            {
                _projectInstance.UpdateUnitID(pendingUnitID);
            }
            
            // 调用所有等待的回调函数
            while (initCallbacks.Count > 0)
            {
                var cb = initCallbacks.Dequeue();

                cb(result);
            }

            isInitializing = false;
        }

        public static bool Login(string unitID)
        {
            if (string.IsNullOrEmpty(unitID))
            {
                Debug.LogError("unitID cannot be null or empty");
                return false;
            }

            if (_projectInstance != null)
            {
                // 如果 _projectInstance 已创建，直接更新 unitID
                _projectInstance.UpdateUnitID(unitID);
                return true;
            }
            else
            {
                // 如果正在初始化，暂存 unitID
                pendingUnitID = unitID;
                return true;
            }

            return true;
        }

        public static ExperimentInfo GetExperiment(string layerKey)
        {
            if (!EnsureInitialized())
            {
                Debug.LogError("Abc not initialized");
                return null;
            }

            if (string.IsNullOrEmpty(layerKey))
            {
                Debug.LogError("layerKey cannot be null or empty");
                return null;
            }

            var result = _projectInstance.GetExperimentByLayerKey(layerKey);
            
            return result?.ConvertToExperimentInfo();
        }

        public static ConfigInfo GetConfig(string key)
        {
            if (!EnsureInitialized())
            {
                Debug.LogError("Abc not initialized");
                return null;
            }

            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("key cannot be null or empty");
                return null;
            }

            var result = _projectInstance.GetDynamicConfig(key);
            
            return result?.ConvertToConfigInfo();
        }

        public static bool LogExperimentExposure(ExperimentInfo exp)
        {
            if (!EnsureInitialized())
            {
                Debug.LogError("Abc not initialized");
                return false;
            }

            if (exp == null)
            {
                Debug.LogError("exp cannot be null or empty");
                return false;
            }

            _projectInstance.LogExposure(new ExposureData(exp.ExpId, ExposureType.ExposureTypeManual));
            return true;
        }

        public static bool SetCommonProperties(Dictionary<string, string> properties)
        {
            if (!EnsureInitialized())
            {
                Debug.LogError("Abc not initialized");
                return false;
            }

            if (properties == null)
            {
                Debug.LogError("properties cannot be null");
                return false;
            }

            _projectInstance.SetCommonProperties(properties);
            return true;
        }

        public static bool Track(string eventCode, Dictionary<string, string> eventValue)
        {
            if (!EnsureInitialized())
            {
                Debug.LogError("Abc not initialized");
                return false;
            }

            if (string.IsNullOrEmpty(eventCode))
            {
                Debug.LogError("eventCode cannot be null or empty");
                return false;
            }

            if (eventValue == null)
            {
                eventValue = new Dictionary<string, string>();
            }

            _projectInstance.LogEvent(new EventLog()
            {
                EventCode = eventCode,
                EventValue = eventValue
            });
            return true;
        }

        private static bool EnsureInitialized()
        {
            if (!isInitialized)
            {
                Debug.LogError("Please initialize Abc by calling Abc.Init() before using this method.");
                return false;
            }

            return true;
        }
    }

    public class EventLog
    {
        public string EventCode { get; set; }
        
        public User User { get; set; }
        
        public Dictionary<string, string> Properties { get; set; }
        
        public Dictionary<string, string> EventValue { get; set; }
        
        public string EventTime { get; set; }

        public EventLog()
        {
            Properties = new Dictionary<string, string>();
            EventValue = new Dictionary<string, string>();
        }
    }

    public class CoroutineRunner : MonoBehaviour
    {
        private static CoroutineRunner _instance;

        public static CoroutineRunner Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("CoroutineRunner");
                    DontDestroyOnLoad(go);
                    _instance = go.AddComponent<CoroutineRunner>();
                }

                return _instance;
            }
        }
    }
}