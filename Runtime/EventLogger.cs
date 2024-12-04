using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using UnityEngine.SceneManagement;

namespace AbcSDKSpace
{
    internal class EventLogger : MonoBehaviour
    {
        private List<EventLog> _eventLogQueue = new List<EventLog>();
        private List<ExposureData> _exposureDataQueue = new List<ExposureData>();

        private IEnumerator _flushCoroutine;
        private float waitTimeInterval = 3f;
        private int maxBufferSize = 10;
        private Config _config;
        private ControlData _controlData;
        private Dictionary<string, string> _systemProperties;
        private User _user;

        private readonly object _eventLogQueueLock = new object();
        private readonly object _exposureLogQueueLock = new object();

        internal void Init(Config config)
        {
            _config = config;
            _flushCoroutine = PeriodicFlush();
            StartCoroutine(_flushCoroutine);
        }

        internal void UpdateConfig(Config config)
        {
            _config = config;
        }

        internal void UpdateControlData(ControlData data)
        {
            _controlData = data;
        }

        internal void UpdateSystemProperties(Dictionary<string, string> p)
        {
            _systemProperties = p;
        }

        internal void EnqueueEvent(EventLog entry)
        {
            // 实时事件上报
            if (_controlData != null && _controlData.ReportTimeInterval <= 0)
            {
                StartCoroutine(LogEventDispatcherWrapper(entry));
                
                return;
            }
            
            // 添加事件日志到队列
            _eventLogQueue.Add(entry);
            if (_eventLogQueue.Count >= maxBufferSize)
            {
                // 创建快照并清空当前队列
                var snapshot = new List<EventLog>();
                lock (_eventLogQueueLock)
                {
                    snapshot = new List<EventLog>(_eventLogQueue);
                    _eventLogQueue.Clear();
                }

                // 启动协程，发送日志
                StartCoroutine(FlushEvents(snapshot));
            }
        }

        internal void EnqueueExposure(ExposureData entry)
        {
            if (entry.GroupId < 0)
            {
                return;
            }

            // 添加曝光数据到队列
            _exposureDataQueue.Add(entry);
            if (_exposureDataQueue.Count >= maxBufferSize)
            {
                // 创建快照并清空当前队列
                var snapshot = new List<ExposureData>();
                lock (_exposureLogQueueLock)
                {
                    snapshot = new List<ExposureData>(_exposureDataQueue);
                    _exposureDataQueue.Clear();
                }

                // 启动协程，发送曝光数据
                StartCoroutine(FlushExposures(snapshot));
            }
        }
        
        private IEnumerator PeriodicFlush()
        {
            while (true)
            {
                float interval = waitTimeInterval;
                if (_controlData != null && _controlData.ReportTimeInterval > 0)
                {
                    interval = _controlData.ReportTimeInterval;
                }

                yield return new WaitForSeconds(interval);

                if (_eventLogQueue.Count > 0)
                {
                    var eventSnapshot = new List<EventLog>();
                    lock (_eventLogQueueLock)
                    {
                        eventSnapshot = new List<EventLog>(_eventLogQueue);
                        _eventLogQueue.Clear();
                    }

                    StartCoroutine(FlushEvents(eventSnapshot));
                }

                if (_exposureDataQueue.Count > 0)
                {
                    var exposureSnapshot = new List<ExposureData>();
                    lock (_exposureLogQueueLock)
                    {
                        exposureSnapshot = new List<ExposureData>(_exposureDataQueue);
                        _exposureDataQueue.Clear();
                    }

                    StartCoroutine(FlushExposures(exposureSnapshot));
                }
            }
        }

        private IEnumerator FlushEvents(List<EventLog> snapshot)
        {
            if (snapshot.Count == 0)
            {
                yield break;
            }

            foreach (EventLog e in snapshot)
            {
                // 调用包装器方法
                try
                {
                    StartCoroutine(LogEventDispatcherWrapper(e));
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error in FlushEvents: {ex}");
                    // 先丢弃
                    // 处理未发送的数据，例如重新加入队列
                    // _eventLogQueue.Add(e);
                }
            }

            yield return null;
        }

        private IEnumerator FlushExposures(List<ExposureData> snapshot)
        {
            if (snapshot.Count == 0)
            {
                yield break;
            }
            
            // 调用包装器方法
            StartCoroutine(LogExposureDispatcherWrapper(BuildBatchExposuresReq(snapshot)));

            yield return null;
        }

        private BatchLogExposureReq BuildBatchExposuresReq(List<ExposureData> snapshot)
        {
            // 构建请求
            BatchLogExposureReq req = new BatchLogExposureReq();
            
            req.AppID = this._config.GameId;
            req.EventCode = "$mg_exp";
            req.EventTime = TokenUtils.GetTimestamp();
            req.EventType = EventType.EventTypeExperiment;
            req.EventStatus = EventStatus.EventStatusFormal;
            req.UserInfo = new UserInfo
            {
                DeviceID = this._config.DeviceId,
                UserID = this._config.UnitId
            };
            req.SdkInfo = new SDKInfo()
            {
                Version = Constant.SdkVersionS,
                Platform = "Unity"
            };
            req.Properties = new Properties();
            req.Properties.BuildProperties(_systemProperties);
            req.Exposures = new List<ExposureData>(snapshot);

            return req;
        } 

        /// <summary>
        /// 包装器方法，处理异常
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        private IEnumerator LogEventDispatcherWrapper(EventLog entry)
        {
            var enumerator = LogEventDispatcher(BuildLogEventReq(entry));

            while (true)
            {
                try
                {
                    if (!enumerator.MoveNext())
                    {
                        yield break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Exception in LogEventDispatcher: {ex}");
                    yield break;
                }

                yield return enumerator.Current;
            }
        }
        
        private LogEventReq BuildLogEventReq(EventLog entry)
        {
            LogEventReq req = new LogEventReq();
            
            req.AppID = this._config.GameId;
            req.EventCode = entry?.EventCode;
            req.EventTime = entry?.EventTime;
            req.EventValue = entry?.EventValue ?? new Dictionary<string, string>();
            req.Properties = new Properties();
            req.Properties.BuildProperties(entry?.Properties);
            req.EventStatus = EventStatus.EventStatusFormal;
            req.SDKInfo = new SDKInfo()
            {
                Version = Constant.SdkVersionS,
                Platform = "Unity"
            };
            
            req.UserInfo = new UserInfo
            {
                DeviceID = entry?.User?.DeviceID,
                UserID = entry?.User?.UserID,
                ExtraData = entry?.User?.Properties
            };

            return req;
        }

        /// <summary>
        /// 包装器方法，处理异常
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        private IEnumerator LogExposureDispatcherWrapper(BatchLogExposureReq req)
        {
            var enumerator = LogExposureDispatcher(req);

            while (true)
            {
                try
                {
                    if (!enumerator.MoveNext())
                    {
                        yield break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Exception in LogExposureDispatcher: {ex}");
                    yield break;
                }

                yield return enumerator.Current;
            }
        }

        /// <summary>
        /// 原始的协程方法，不包含 try-catch
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        private IEnumerator LogEventDispatcher(LogEventReq req)
        {
            // 创建 UnityWebRequest 对象
            var webRequest = BuildLogEventWebRequest(req);

            // 发送请求并等待完成
            yield return webRequest.SendWebRequest();

            // 处理响应
            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"[LogEventDispatcher] UnityWebRequest error: {webRequest.error}");
            }
            else
            {
                string content = webRequest.downloadHandler.text;
                var res = CommonResp.Deserialize(content);
                if (res.Code != "EVENT_SERVER_CODE_SUCCESS")
                {
                    Debug.LogError($"retCode={res.Code}, message={res.Message}");
                }
            }
        }

        private UnityWebRequest BuildLogEventWebRequest(LogEventReq req)
        {
            // 创建 UnityWebRequest 对象
            UnityWebRequest webRequest = new UnityWebRequest("https://data.abetterchoice.cn/v2/event/log", "POST");
            byte[] jsonToSend = Encoding.UTF8.GetBytes(req.Serialize());
            webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            // 添加自定义的头部信息
            Utils.SetRequestHeaders(webRequest, this._config.ApiKey);

            // 设置超时时间
            webRequest.timeout = 5;
            
            return webRequest;
        }

        /// <summary>
        /// 原始的协程方法，不包含 try-catch
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        private IEnumerator LogExposureDispatcher(BatchLogExposureReq req)
        {
            // 创建 UnityWebRequest 对象
            var webRequest = BuildLogExposureWebRequest(req);

            // 发送请求并等待完成
            yield return webRequest.SendWebRequest();

            // 处理响应
            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"[LogExposureDispatcher] UnityWebRequest error: {webRequest.error}");
            }
            else
            {
                string content = webRequest.downloadHandler.text;
                var res = CommonResp.Deserialize(content);
                if (res.Code != "EVENT_SERVER_CODE_SUCCESS")
                {
                    Debug.LogError($"retCode={res.Code}, message={res.Message}");
                }
            }
        }

        private UnityWebRequest BuildLogExposureWebRequest(BatchLogExposureReq req)
        {
            UnityWebRequest webRequest = new UnityWebRequest("https://data.abetterchoice.cn/v2/exposure/log", "POST");
            string jsonData = req.Serialize();
            byte[] jsonToSend = Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            // 添加自定义的头部信息
            Utils.SetRequestHeaders(webRequest, this._config.ApiKey);

            // 设置超时时间
            webRequest.timeout = 5;

            return webRequest;
        }

        private void OnDestroy()
        {
            StopCoroutine(_flushCoroutine);
            FlushALL();
        }

        public void FlushALL()
        {
            // 处理剩余的事件日志
            var eventSnapshot = new List<EventLog>();
            lock (_eventLogQueueLock)
            {
                eventSnapshot = new List<EventLog>(_eventLogQueue);
                _eventLogQueue.Clear();
            }

            StartCoroutine(FlushEvents(eventSnapshot));

            // 处理剩余的曝光数据
            var exposureSnapshot = new List<ExposureData>();
            lock (_exposureLogQueueLock)
            {
                exposureSnapshot = new List<ExposureData>(_exposureDataQueue);
                _exposureDataQueue.Clear();
            }

            StartCoroutine(FlushExposures(exposureSnapshot));
        }
        
    }
}