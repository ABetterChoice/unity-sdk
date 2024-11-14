using System.Collections.Generic;
using SimpleJSON;

namespace AbcSDKSpace
{
    public enum RetCode
    {
        Unknown = 0,
        Success = 100,
        NoPermission = 101,
        TrafficLimit = 102,
        AppIDErr = 103,
        ServerErr = 104,
        GuidErr = 105
    }

    public enum DataUpdateType
    {
        DataUpdateTypeUnknown = 0,
        DataUpdateTypeAll = 1,
        DataUpdateTypeNoNeed = 2,
        DataUpdateTypeDiff = 3
    }

    public enum ReportInfra
    {
        ReportInfraUnspecified = 0,
        ReportInfraAtta = 100,
        ReportInfraDatong = 101,
        ReportInfraBeacon = 102
    }

    public enum ExposureType
    {
        ExposureTypeUnknown = 0,

        // sdk 自动上报
        ExposureTypeAutomatic = 1,

        // 用户手动上报
        ExposureTypeManual = 2
    }

    public enum EventType
    {
        EventTypeUnknown = 0,
        EventTypeExperiment = 1
    }

    public enum EventStatus
    {
        EventStatusUnknown = 0,
        // 正式
        EventStatusFormal = 1,
        // 调试
        EventStatusDebug = 2
    }

    public enum EventServerCode
    {
        EventServerCodeSuccess = 0,                          // 正常返回
        EventServerCodeNoPermission = 1001,                 // 无权限
        EventServerCodeTrafficLimit = 1002,                 // 限流返回
        EventServerCodeInvalidProjectID = 1003,            // 入参 projectID 出错
        EventServerCodeServerErr = 1004,                    // 服务器处理异常
        EventServerCodeInvalidParam = 1005,                 // 非法参数
        EventServerCodeSameVersion = 2001                   // 版本未更新
    }

    public class Token
    {
        public string TokenName { get; set; }
        
        /// <summary>
        /// 序列化方法
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            JSONObject json = new JSONObject();
            json["tokenName"] = this.TokenName;
            return json.ToString();
        }
        
        /// <summary>
        //// <summary>
/// 反序列化方法/// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static Token Deserialize(string jsonString)
        {
            JSONNode json = JSON.Parse(jsonString);
            Token token = new Token();
            token.TokenName = json["tokenName"];
            return token;
        }
    }

    public class FilterOptions
    {
        public List<string> SceneIds { get; set; }

        public List<string> ModuleCodes { get; set; }

        public List<string> ExpGroupKeys { get; set; }

        /// <summary>
        /// 序列化方法
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            JSONObject json = new JSONObject();

            // 序列化 SceneIds
            JSONArray sceneIdsArray = new JSONArray();
            if (SceneIds != null)
            {
                foreach (string sceneId in SceneIds)
                {
                    sceneIdsArray.Add(sceneId);
                }
            }

            json["scene_ids"] = sceneIdsArray;

            // 序列化 ModuleCodes
            JSONArray moduleCodesArray = new JSONArray();
            if (ModuleCodes != null)
            {
                foreach (string moduleCode in ModuleCodes)
                {
                    moduleCodesArray.Add(moduleCode);
                }
            }

            json["module_codes"] = moduleCodesArray;

            // 序列化 ExpGroupKeys
            JSONArray expGroupKeysArray = new JSONArray();
            if (ExpGroupKeys != null)
            {
                foreach (string expGroupKey in ExpGroupKeys)
                {
                    expGroupKeysArray.Add(expGroupKey);
                }
            }

            json["exp_group_keys"] = expGroupKeysArray;

            return json.ToString();
        }
        
        /// <summary>
        ///  反序列化方法
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static FilterOptions Deserialize(string jsonString)
        {
            JSONNode json = JSON.Parse(jsonString);
            FilterOptions filterOptions = new FilterOptions();

            // 反序列化 SceneIds
            filterOptions.SceneIds = new List<string>();
            foreach (JSONNode node in json["scene_ids"].AsArray)
            {
                filterOptions.SceneIds.Add(node.Value);
            }

            // 反序列化 ModuleCodes
            filterOptions.ModuleCodes = new List<string>();
            foreach (JSONNode node in json["module_codes"].AsArray)
            {
                filterOptions.ModuleCodes.Add(node.Value);
            }

            // 反序列化 ExpGroupKeys
            filterOptions.ExpGroupKeys = new List<string>();
            foreach (JSONNode node in json["exp_group_keys"].AsArray)
            {
                filterOptions.ExpGroupKeys.Add(node.Value);
            }

            return filterOptions;
        }
    }

    public class ProfileValues
    {
        public List<string> UserAttrs { get; set; }

        public ProfileValues()
        {
            UserAttrs = new List<string>();
        }

        public ProfileValues(List<string> values)
        {
            UserAttrs = values;
        }
        
        /// <summary>
        /// 序列化方法
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            JSONObject json = new JSONObject();

            // 序列化 UserAttrs
            JSONArray userAttrsArray = new JSONArray();
            if (UserAttrs != null)
            {
                foreach (string attr in UserAttrs)
                {
                    userAttrsArray.Add(attr);
                }
            }

            json["user_attrs"] = userAttrsArray;

            return json.ToString();
        }

        
        /// <summary>
        /// 反序列化方法
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static ProfileValues Deserialize(string jsonString)
        {
            JSONNode json = JSON.Parse(jsonString);
            ProfileValues profileValues = new ProfileValues();

            // 反序列化 UserAttrs
            profileValues.UserAttrs = new List<string>();
            foreach (JSONNode node in json["user_attrs"].AsArray)
            {
                profileValues.UserAttrs.Add(node.Value);
            }

            return profileValues;
        }
    }

    public class GetExperimentsReq
    {
        public string AppId { get; set; }

        public string Guid { get; set; }

        public Dictionary<string, ProfileValues> Profiles { get; set; }

        public Dictionary<string, string> ExtraParams { get; set; }

        public FilterOptions FilterOptions { get; set; }

        public int TimeVersion { get; set; }
        
        /// <summary>
        ///  序列化方法
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            JSONObject json = new JSONObject();
            json["app_id"] = this.AppId;
            json["guid"] = this.Guid;
            json["time_version"] = this.TimeVersion;

            // 序列化 Profiles
            JSONObject profilesJson = new JSONObject();
            if (Profiles != null)
            {
                foreach (var kvp in Profiles)
                {
                    profilesJson[kvp.Key] = JSON.Parse(kvp.Value.Serialize());
                }
            }

            json["profiles"] = profilesJson;

            // 序列化 ExtraParams
            JSONObject extraParamsJson = new JSONObject();
            if (ExtraParams != null)
            {
                foreach (var kvp in ExtraParams)
                {
                    extraParamsJson[kvp.Key] = kvp.Value;
                }
            }

            json["extra_params"] = extraParamsJson;

            // 序列化 FilterOptions
            if (FilterOptions != null)
            {
                json["filter_options"] = JSON.Parse(FilterOptions.Serialize());
            }

            return json.ToString();
        }
        
        /// <summary>
        /// 反序列化方法
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static GetExperimentsReq Deserialize(string jsonString)
        {
            JSONNode json = JSON.Parse(jsonString);
            GetExperimentsReq req = new GetExperimentsReq();
            req.AppId = json["app_id"];
            req.Guid = json["guid"];
            req.TimeVersion = json["time_version"].AsInt;

            // 反序列化 Profiles
            req.Profiles = new Dictionary<string, ProfileValues>();
            foreach (KeyValuePair<string, JSONNode> kvp in json["profiles"].AsObject)
            {
                ProfileValues profileValue = ProfileValues.Deserialize(kvp.Value.ToString());
                
                req.Profiles.Add(kvp.Key, profileValue);
            }

            // 反序列化 ExtraParams
            req.ExtraParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, JSONNode> kvp in json["extra_params"].AsObject)
            {
                req.ExtraParams.Add(kvp.Key, kvp.Value);
            }

            // 反序列化 FilterOptions
            if (json["filter_options"] != null)
            {
                req.FilterOptions = FilterOptions.Deserialize(json["filter_options"].ToString());
            }

            return req;
        }
    }

    public class Experiment
    {
        public int ExpId;


        public string ExpKey;


        public string ExpGroupKey;


        public string ModuleCode;


        public Dictionary<string, string> Params;


        public int Bucket;


        public int ModuleBucketNum;


        public float Percentage;

        public Experiment()
        {
            ExpId = -1;
        }

        public ExperimentInfo ConvertToExperimentInfo()
        {
            ExperimentInfo exp = new ExperimentInfo();
            
            exp.ExpGroupKey = this.ExpGroupKey;
            exp.ExpId = this.ExpId;
            exp.ExpKey = this.ExpKey;
            exp.ModuleCode = this.ModuleCode;
            exp.Params = this.Params;
            return exp;
        }
        
        /// <summary>
        /// 序列化方法
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            JSONObject json = new JSONObject();
            json["exp_id"] = this.ExpId;
            json["exp_key"] = this.ExpKey;
            json["exp_group_key"] = this.ExpGroupKey;
            json["module_code"] = this.ModuleCode;
            json["bucket"] = this.Bucket;
            json["module_bucket_num"] = this.ModuleBucketNum;
            json["percentage"] = this.Percentage;

            // 序列化 Params
            JSONObject paramsJson = new JSONObject();
            if (Params != null)
            {
                foreach (var kvp in Params)
                {
                    paramsJson[kvp.Key] = kvp.Value;
                }
            }

            json["params"] = paramsJson;

            return json.ToString();
        }
        
        /// <summary>
        /// 反序列化方法
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static Experiment Deserialize(string jsonString)
        {
            JSONNode json = JSON.Parse(jsonString);
            Experiment exp = new Experiment();
            exp.ExpId = json["exp_id"].AsInt;
            exp.ExpKey = json["exp_key"];
            exp.ExpGroupKey = json["exp_group_key"];
            exp.ModuleCode = json["module_code"];
            exp.Bucket = json["bucket"].AsInt;
            exp.ModuleBucketNum = json["module_bucket_num"].AsInt;
            exp.Percentage = json["percentage"].AsFloat;

            // 反序列化 Params
            exp.Params = new Dictionary<string, string>();
            foreach (KeyValuePair<string, JSONNode> kvp in json["params"].AsObject)
            {
                exp.Params.Add(kvp.Key, kvp.Value);
            }

            return exp;
        }
    }

    public class GetExperimentsRes
    {
        public RetCode RetCode { get; set; }

        public string Msg { get; set; }

        public Dictionary<string, Experiment> ExpData { get; set; }

        public ControlData ControlData { get; set; }

        public int TimeVersion { get; set; }

        public DataUpdateType DataUpdateType { get; set; }
        
        /// <summary>
        /// 序列化方法
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            JSONObject json = new JSONObject();
            json["ret_code"] = (int)this.RetCode;
            json["msg"] = this.Msg;
            json["time_version"] = this.TimeVersion;
            json["data_update_type"] = (int)this.DataUpdateType;

            // 序列化 ExpData
            JSONObject expDataJson = new JSONObject();
            if (ExpData != null)
            {
                foreach (var kvp in ExpData)
                {
                    expDataJson[kvp.Key] = JSON.Parse(kvp.Value.Serialize());
                }
            }

            json["exp_data"] = expDataJson;

            // 序列化 ControlData
            if (ControlData != null)
            {
                json["control_data"] = JSON.Parse(ControlData.Serialize());
            }

            return json.ToString();
        }
        
        /// <summary>
        /// 反序列化方法
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static GetExperimentsRes Deserialize(string jsonString)
        {
            JSONNode json = JSON.Parse(jsonString);
            GetExperimentsRes res = new GetExperimentsRes();
            res.RetCode = (RetCode)json["ret_code"].AsInt;
            res.Msg = json["msg"];
            res.TimeVersion = json["time_version"].AsInt;
            res.DataUpdateType = (DataUpdateType)json["data_update_type"].AsInt;

            // 反序列化 ExpData
            res.ExpData = new Dictionary<string, Experiment>();
            foreach (KeyValuePair<string, JSONNode> kvp in json["exp_data"].AsObject)
            {
                Experiment exp = Experiment.Deserialize(kvp.Value.ToString());
                
                res.ExpData.Add(kvp.Key, exp);
            }

            // 反序列化 ControlData
            if (json["control_data"] != null)
            {
                res.ControlData = ControlData.Deserialize(json["control_data"].ToString());
            }

            return res;
        }
    }

    public class ControlData
    {
        public int RefreshDuration { get; set; }

        public bool EnableReport { get; set; }

        public ReportInfra ReportInfra { get; set; }

        public AttaInfraConfig AttaInfraConfig { get; set; }

        public DatongInfraConfig DatongInfraConfig { get; set; }

        public BeaconInfraConfig BeaconInfraConfig { get; set; }

        public int ReportTimeInterval { get; set; }
        
        /// <summary>
        ///  序列化方法
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            JSONObject json = new JSONObject();
            json["refresh_duration"] = this.RefreshDuration;
            json["enable_report"] = this.EnableReport;
            json["report_infra"] = (int)this.ReportInfra;
            json["report_time_interval"] = this.ReportTimeInterval;

            // 序列化 AttaInfraConfig
            if (AttaInfraConfig != null)
            {
                json["atta_infra_config"] = JSON.Parse(AttaInfraConfig.Serialize());
            }

            // 序列化 DatongInfraConfig
            if (DatongInfraConfig != null)
            {
                json["datong_infra_config"] = JSON.Parse(DatongInfraConfig.Serialize());
            }

            // 序列化 BeaconInfraConfig
            if (BeaconInfraConfig != null)
            {
                json["beacon_infra_config"] = JSON.Parse(BeaconInfraConfig.Serialize());
            }

            return json.ToString();
        }
        
        /// <summary>
        /// 反序列化方法
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static ControlData Deserialize(string jsonString)
        {
            JSONNode json = JSON.Parse(jsonString);
            ControlData data = new ControlData();
            data.RefreshDuration = json["refresh_duration"].AsInt;
            data.EnableReport = json["enable_report"].AsBool;
            data.ReportInfra = (ReportInfra)json["report_infra"].AsInt;
            data.ReportTimeInterval = json["report_time_interval"].AsInt;

            // 反序列化 AttaInfraConfig
            if (json["atta_infra_config"] != null)
            {
                data.AttaInfraConfig = AttaInfraConfig.Deserialize(json["atta_infra_config"].ToString());
            }

            // 反序列化 DatongInfraConfig
            if (json["datong_infra_config"] != null)
            {
                data.DatongInfraConfig = DatongInfraConfig.Deserialize(json["datong_infra_config"].ToString());
            }

            // 反序列化 BeaconInfraConfig
            if (json["beacon_infra_config"] != null)
            {
                data.BeaconInfraConfig = BeaconInfraConfig.Deserialize(json["beacon_infra_config"].ToString());
            }

            return data;
        }
    }

    public class AttaInfraConfig
    {
        public string AttaId { get; set; }

        public string AttaToken { get; set; }

        /// <summary>
        /// 反序列化方法
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public string Serialize()
        {
            JSONObject json = new JSONObject();
            json["atta_id"] = this.AttaId;
            json["atta_token"] = this.AttaToken;
            return json.ToString();
        }

        /// <summary>
        /// 反序列化方法
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static AttaInfraConfig Deserialize(string jsonString)
        {
            JSONNode json = JSON.Parse(jsonString);
            AttaInfraConfig config = new AttaInfraConfig();
            config.AttaId = json["atta_id"];
            config.AttaToken = json["atta_token"];
            return config;
        }
    }

    public class DatongInfraConfig
    {
        public string AppKey { get; set; }

        /// <summary>
        /// 反序列化方法
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public string Serialize()
        {
            JSONObject json = new JSONObject();
            json["app_key"] = this.AppKey;
            return json.ToString();
        }

        /// <summary>
        /// 反序列化方法
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static DatongInfraConfig Deserialize(string jsonString)
        {
            JSONNode json = JSON.Parse(jsonString);
            DatongInfraConfig config = new DatongInfraConfig();
            config.AppKey = json["app_key"];
            return config;
        }
    }

    public class BeaconInfraConfig
    {
        public string AppKey { get; set; }

        /// <summary>
        /// 反序列化方法
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public string Serialize()
        {
            JSONObject json = new JSONObject();
            json["app_key"] = this.AppKey;
            return json.ToString();
        }
        
        /// <summary>
        /// 反序列化方法
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static BeaconInfraConfig Deserialize(string jsonString)
        {
            JSONNode json = JSON.Parse(jsonString);
            BeaconInfraConfig config = new BeaconInfraConfig();
            config.AppKey = json["app_key"];
            return config;
        }
    }

    public class LogEventReq
    {
        public string EventCode { get; set; }

        public string EventTime { get; set; }

        public EventStatus EventStatus { get; set; }

        public string AppID { get; set; }

        public UserInfo UserInfo { get; set; }

        public SDKInfo SDKInfo { get; set; }

        public Properties Properties { get; set; }

        public Dictionary<string, string> EventValue { get; set; }

        /// <summary>
        /// 反序列化方法
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public string Serialize()
        {
            JSONObject json = new JSONObject();
            json["event_code"] = this.EventCode;
            json["event_time"] = this.EventTime;
            json["event_status"] = (int)this.EventStatus;
            json["app_id"] = this.AppID;

            // 序列化 UserInfo
            if (UserInfo != null)
            {
                json["user"] = JSON.Parse(UserInfo.Serialize());
            }

            // 序列化 SDKInfo
            if (SDKInfo != null)
            {
                json["sdk_info"] = JSON.Parse(SDKInfo.Serialize());
            }

            // 序列化 Properties
            if (Properties != null)
            {
                json["properties"] = JSON.Parse(Properties.Serialize());
            }

            // 序列化 EventValue
            JSONObject eventValueJson = new JSONObject();
            if (EventValue != null)
            {
                foreach (var kvp in EventValue)
                {
                    eventValueJson[kvp.Key] = kvp.Value;
                }
            }

            json["event_value"] = eventValueJson;

            return json.ToString();
        }

        /// <summary>
        /// 反序列化方法
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static LogEventReq Deserialize(string jsonString)
        {
            JSONNode json = JSON.Parse(jsonString);
            LogEventReq req = new LogEventReq();
            req.EventCode = json["event_code"];
            req.EventTime = json["event_time"];
            req.EventStatus = (EventStatus)json["event_status"].AsInt;
            req.AppID = json["app_id"];

            // 反序列化 UserInfo
            if (json["user"] != null)
            {
                req.UserInfo = UserInfo.Deserialize(json["user"].ToString());
            }

            // 反序列化 SDKInfo
            if (json["sdk_info"] != null)
            {
                req.SDKInfo = SDKInfo.Deserialize(json["sdk_info"].ToString());
            }

            // 反序列化 Properties
            if (json["properties"] != null)
            {
                req.Properties = Properties.Deserialize(json["properties"].ToString());
            }

            // 反序列化 EventValue
            req.EventValue = new Dictionary<string, string>();
            foreach (KeyValuePair<string, JSONNode> kvp in json["event_value"].AsObject)
            {
                req.EventValue.Add(kvp.Key, kvp.Value);
            }

            return req;
        }
    }

    public class UserInfo
    {
        public string DeviceID { get; set; }

        public string UserID { get; set; }

        public Dictionary<string, string> ExtraData { get; set; }
        
        public string Serialize()
        {
            JSONObject json = new JSONObject();
            json["device_id"] = this.DeviceID;
            json["user_id"] = this.UserID;

            // 序列化 ExtraData
            JSONObject extraDataJson = new JSONObject();
            if (ExtraData != null)
            {
                foreach (var kvp in ExtraData)
                {
                    extraDataJson[kvp.Key] = kvp.Value;
                }
            }

            json["extra_data"] = extraDataJson;

            return json.ToString();
        }
        
        public static UserInfo Deserialize(string jsonString)
        {
            JSONNode json = JSON.Parse(jsonString);
            UserInfo userInfo = new UserInfo();
            userInfo.DeviceID = json["device_id"];
            userInfo.UserID = json["user_id"];

            // 反序列化 ExtraData
            userInfo.ExtraData = new Dictionary<string, string>();
            foreach (KeyValuePair<string, JSONNode> kvp in json["extra_data"].AsObject)
            {
                userInfo.ExtraData.Add(kvp.Key, kvp.Value);
            }

            return userInfo;
        }
    }

    public class SDKInfo
    {
        public string Version { get; set; } // 版本 v1.1.8

        public string Platform { get; set; } // 平台：ios\mp\android
        
        public string Serialize()
        {
            JSONObject json = new JSONObject();
            json["version"] = this.Version;
            json["platform"] = this.Platform;
            return json.ToString();
        }
        
        public static SDKInfo Deserialize(string jsonString)
        {
            JSONNode json = JSON.Parse(jsonString);
            SDKInfo sdkInfo = new SDKInfo();
            sdkInfo.Version = json["version"];
            sdkInfo.Platform = json["platform"];
            return sdkInfo;
        }
    }

    public class Properties
    {
        public string OsType { get; set; }

        public string OsVersion { get; set; }

        public string AppVersion { get; set; } // 平台版本

        public string AppPlatform { get; set; } // 平台：微信小游戏wx-mp、微信小程序wx-mp

        public string DeviceModel { get; set; } // iPhone 15

        public string Manufacturer { get; set; } // 制造商 apple

        public string SystemLanguage { get; set; } // 语言: zh

        public string NetworkType { get; set; } // TODO 是否枚举

        public string Ip { get; set; } // 保留，没有的话 后台自动获取 client ip

        public string Scene { get; set; } // 场景：首页\详情页\搜索页

        public long ScreenWidth { get; set; } // 屏幕宽度

        public long ScreenHeight { get; set; }

        public void BuildProperties(Dictionary<string, string> properties)
        {
            if (properties.ContainsKey(Constant.EventPropertiesOsS))
            {
                this.OsType = properties[Constant.EventPropertiesOsS];
            }

            if (properties.ContainsKey(Constant.EventPropertiesOSVersionS))
            {
                this.OsVersion = properties[Constant.EventPropertiesOsS];
            }

            if (properties.ContainsKey(Constant.EventPropertiesMgVersionS))
            {
                this.AppVersion = properties[Constant.EventPropertiesMgVersionS];
            }

            if (properties.ContainsKey(Constant.EventPropertiesDeviceModelS))
            {
                this.DeviceModel = properties[Constant.EventPropertiesDeviceModelS];
            }

            if (properties.ContainsKey(Constant.EventPropertiesManufacturerS))
            {
                this.Manufacturer = properties[Constant.EventPropertiesManufacturerS];
            }

            if (properties.ContainsKey(Constant.EventPropertiesMgPlatformS))
            {
                this.AppPlatform = properties[Constant.EventPropertiesMgPlatformS];
            }

            if (properties.ContainsKey(Constant.EventPropertiesSystemLanguageS))
            {
                this.SystemLanguage = properties[Constant.EventPropertiesSystemLanguageS];
            }

            if (properties.ContainsKey(Constant.EventPropertiesNetworkTypeS))
            {
                this.NetworkType = properties[Constant.EventPropertiesNetworkTypeS];
            }

            if (properties.ContainsKey(Constant.EventPropertiesIps))
            {
                this.Ip = properties[Constant.EventPropertiesIps];
            }

            if (properties.ContainsKey(Constant.EventPropertiesSceneS))
            {
                this.Scene = properties[Constant.EventPropertiesSceneS];
            }

            if (properties.ContainsKey(Constant.EventPropertiesScreenWidthS))
            {
                double width = 0;
                
                double.TryParse(properties[Constant.EventPropertiesScreenWidthS], out width);
                
                this.ScreenWidth = (long)width;
            }

            if (properties.ContainsKey(Constant.EventPropertiesScreenHeightS))
            {
                double height = 0;
                
                double.TryParse(properties[Constant.EventPropertiesScreenHeightS], out height);
                
                this.ScreenHeight = (long)height;
            }
        }
        
        public string Serialize()
        {
            JSONObject json = new JSONObject();
            json["os_type"] = this.OsType;
            json["os_version"] = this.OsVersion;
            json["app_version"] = this.AppVersion;
            json["app_platform"] = this.AppPlatform;
            json["device_model"] = this.DeviceModel;
            json["manufacturer"] = this.Manufacturer;
            json["system_language"] = this.SystemLanguage;
            json["network_type"] = this.NetworkType;
            json["ip"] = this.Ip;
            json["scene"] = this.Scene;
            json["screen_width"] = this.ScreenWidth;
            json["screen_height"] = this.ScreenHeight;
            return json.ToString();
        }
        
        public static Properties Deserialize(string jsonString)
        {
            JSONNode json = JSON.Parse(jsonString);
            Properties properties = new Properties();
            properties.OsType = json["os_type"];
            properties.OsVersion = json["os_version"];
            properties.AppVersion = json["app_version"];
            properties.AppPlatform = json["app_platform"];
            properties.DeviceModel = json["device_model"];
            properties.Manufacturer = json["manufacturer"];
            properties.SystemLanguage = json["system_language"];
            properties.NetworkType = json["network_type"];
            properties.Ip = json["ip"];
            properties.Scene = json["scene"];
            properties.ScreenWidth = json["screen_width"].AsLong;
            properties.ScreenHeight = json["screen_height"].AsLong;
            return properties;
        }
    }

    public class GetEventSettingReq
    {
        public string AppID { get; set; } // appId
        public string Version { get; set; } // version

        /// <summary>
        /// 序列化方法
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            JSONObject json = new JSONObject();
            json["appId"] = this.AppID;
            json["version"] = this.Version;
            return json.ToString();
        }

        /// <summary>
        /// 反序列化方法
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static GetEventSettingReq Deserialize(string jsonString)
        {
            JSONNode json = JSON.Parse(jsonString);
            GetEventSettingReq req = new GetEventSettingReq();
            req.AppID = json["appId"];
            req.Version = json["version"];
            return req;
        }
    }

    public class Filter
    {
        public List<string> Uids { get; set; } // uids

        /// <summary>
        /// 序列化方法
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            JSONObject json = new JSONObject();
            JSONArray uidsArray = new JSONArray();
            if (this.Uids != null)
            {
                foreach (var uid in this.Uids)
                {
                    uidsArray.Add(uid);
                }
            }
            json["uids"] = uidsArray;
            return json.ToString();
        }

        /// <summary>
        /// 反序列化方法
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static Filter Deserialize(string jsonString)
        {
            JSONNode json = JSON.Parse(jsonString);
            Filter filter = new Filter();
            filter.Uids = new List<string>();
            if (json["uids"] != null)
            {
                foreach (JSONNode uidNode in json["uids"].AsArray)
                {
                    filter.Uids.Add(uidNode.Value);
                }
            }
            return filter;
        }
    }

    public class EventSetting
    {
        public bool Enable { get; set; } // enable
        public string EventCode { get; set; } // eventCode
        public int SampleInterval { get; set; } // sampleInterval
        public Filter Filter { get; set; } // filter
        public Filter ExcludeFilter { get; set; } // excludeFilter

        /// <summary>
        /// 序列化方法
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            JSONObject json = new JSONObject();
            json["enable"] = this.Enable;
            json["eventCode"] = this.EventCode;
            json["sampleInterval"] = this.SampleInterval;

            // 序列化 Filter
            if (this.Filter != null)
            {
                json["filter"] = JSON.Parse(this.Filter.Serialize());
            }

            // 序列化 ExcludeFilter
            if (this.ExcludeFilter != null)
            {
                json["excludeFilter"] = JSON.Parse(this.ExcludeFilter.Serialize());
            }

            return json.ToString();
        }

        /// <summary>
        /// 反序列化方法
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static EventSetting Deserialize(string jsonString)
        {
            JSONNode json = JSON.Parse(jsonString);
            EventSetting setting = new EventSetting();
            setting.Enable = json["enable"].AsBool;
            setting.EventCode = json["eventCode"];
            setting.SampleInterval = json["sampleInterval"].AsInt;

            // 反序列化 Filter
            if (json["filter"] != null)
            {
                setting.Filter = Filter.Deserialize(json["filter"].ToString());
            }

            // 反序列化 ExcludeFilter
            if (json["excludeFilter"] != null)
            {
                setting.ExcludeFilter = Filter.Deserialize(json["excludeFilter"].ToString());
            }

            return setting;
        }
    }

    public class GetEventSettingResp
    {
        public Dictionary<string, EventSetting> EventSetting { get; set; } // eventSetting
        public EventSetting DefaultSetting { get; set; } // defaultSetting
        public CommonResp CommonResp { get; set; } // commonResp
        public string Version { get; set; } // version
        public int RefreshInterval { get; set; } // refreshInterval

        /// <summary>
        /// 序列化方法
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            JSONObject json = new JSONObject();

            // 序列化 EventSetting
            JSONObject eventSettingJson = new JSONObject();
            if (this.EventSetting != null)
            {
                foreach (var kvp in this.EventSetting)
                {
                    eventSettingJson[kvp.Key] = JSON.Parse(kvp.Value.Serialize());
                }
            }
            json["eventSetting"] = eventSettingJson;

            // 序列化 DefaultSetting
            if (this.DefaultSetting != null)
            {
                json["defaultSetting"] = JSON.Parse(this.DefaultSetting.Serialize());
            }

            // 序列化 CommonResp
            if (this.CommonResp != null)
            {
                json["commonResp"] = JSON.Parse(this.CommonResp.Serialize());
            }

            json["version"] = this.Version;
            json["refreshInterval"] = this.RefreshInterval;

            return json.ToString();
        }

        /// <summary>
        /// 反序列化方法
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static GetEventSettingResp Deserialize(string jsonString)
        {
            JSONNode json = JSON.Parse(jsonString);
            GetEventSettingResp resp = new GetEventSettingResp();

            // 反序列化 EventSetting
            resp.EventSetting = new Dictionary<string, EventSetting>();
            if (json["eventSetting"] != null)
            {
                foreach (KeyValuePair<string, JSONNode> kvp in json["eventSetting"].AsObject)
                {
                    string key = kvp.Key;
                    EventSetting value = AbcSDKSpace.EventSetting.Deserialize(kvp.Value.ToString());
                    resp.EventSetting.Add(key, value);
                }
            }

            // 反序列化 DefaultSetting
            if (json["defaultSetting"] != null)
            {
                resp.DefaultSetting = AbcSDKSpace.EventSetting.Deserialize(json["defaultSetting"].ToString());
            }

            // 反序列化 CommonResp
            if (json["commonResp"] != null)
            {
                resp.CommonResp = CommonResp.Deserialize(json["commonResp"].ToString());
            }

            resp.Version = json["version"];
            resp.RefreshInterval = json["refreshInterval"].AsInt;

            return resp;
        }
    }

    public class CommonResp
    {
        public string Code { get; set; } // 状态码


        public string Message { get; set; } // 结果信息

        public string Serialize()
        {
            JSONObject json = new JSONObject();
            json["code"] = this.Code;
            json["message"] = this.Message;
            return json.ToString();
        }

        public static CommonResp Deserialize(string jsonString)
        {
            JSONNode json = JSON.Parse(jsonString);
            CommonResp resp = new CommonResp();
            resp.Code = json["code"];
            resp.Message = json["message"];
            return resp;
        }
    }


    public class ExposureData
    {
        public long GroupId { get; set; }


        public string LayerKey { get; set; }


        public ExposureType ExposureType { get; set; }


        public UserInfo User { get; set; }


        public Dictionary<string, string> ExtraData { get; set; }

        public ExposureData()
        {
        }

        public ExposureData(long groupID, ExposureType exposureType = ExposureType.ExposureTypeAutomatic)
        {
            GroupId = groupID;
            ExposureType = exposureType;
        }

        public string Serialize()
        {
            JSONObject json = new JSONObject();
            json["group_id"] = this.GroupId;
            json["layer_key"] = this.LayerKey;
            json["exposure_type"] = (int)this.ExposureType;

            // 序列化 User
            if (User != null)
            {
                json["user"] = JSON.Parse(User.Serialize());
            }

            // 序列化 ExtraData
            JSONObject extraDataJson = new JSONObject();
            if (ExtraData != null)
            {
                foreach (var kvp in ExtraData)
                {
                    extraDataJson[kvp.Key] = kvp.Value;
                }
            }

            json["extra_data"] = extraDataJson;

            return json.ToString();
        }
        
        public static ExposureData Deserialize(string jsonString)
        {
            JSONNode json = JSON.Parse(jsonString);
            ExposureData data = new ExposureData();
            data.GroupId = json["group_id"].AsLong;
            data.LayerKey = json["layer_key"];
            data.ExposureType = (ExposureType)json["exposure_type"].AsInt;

            // 反序列化 User
            if (json["user"] != null)
            {
                data.User = UserInfo.Deserialize(json["user"].ToString());
            }

            // 反序列化 ExtraData
            data.ExtraData = new Dictionary<string, string>();
            foreach (KeyValuePair<string, JSONNode> kvp in json["extra_data"].AsObject)
            {
                data.ExtraData.Add(kvp.Key, kvp.Value);
            }

            return data;
        }
    }

    public class GetFeaturesReq
    {
        public string ProjectID;


        public string UnitID;


        public List<string> FeatureFlaNames;


        public Dictionary<string, ProfileValues> Profiles;


        public List<long> SceneIDs;

        public string Serialize()
        {
            JSONObject json = new JSONObject();
            json["project_id"] = this.ProjectID;
            json["unit_id"] = this.UnitID;

            // 序列化 FeatureFlaNames
            JSONArray featureNamesArray = new JSONArray();
            if (FeatureFlaNames != null)
            {
                foreach (var name in FeatureFlaNames)
                {
                    featureNamesArray.Add(name);
                }
            }

            json["feature_names"] = featureNamesArray;

            // 序列化 Profiles
            JSONObject profilesJson = new JSONObject();
            if (Profiles != null)
            {
                foreach (var kvp in Profiles)
                {
                    profilesJson[kvp.Key] = JSON.Parse(kvp.Value.Serialize());
                }
            }

            json["profiles"] = profilesJson;

            // 序列化 SceneIDs
            JSONArray sceneIdsArray = new JSONArray();
            if (SceneIDs != null)
            {
                foreach (var id in SceneIDs)
                {
                    sceneIdsArray.Add(id);
                }
            }

            json["scene_ids"] = sceneIdsArray;

            return json.ToString();
        }

        public static GetFeaturesReq Deserialize(string jsonString)
        {
            JSONNode json = JSON.Parse(jsonString);
            GetFeaturesReq req = new GetFeaturesReq();
            req.ProjectID = json["project_id"];
            req.UnitID = json["unit_id"];

            // 反序列化 FeatureFlaNames
            req.FeatureFlaNames = new List<string>();
            foreach (JSONNode node in json["feature_names"].AsArray)
            {
                req.FeatureFlaNames.Add(node.Value);
            }

            // 反序列化 Profiles
            req.Profiles = new Dictionary<string, ProfileValues>();
            foreach (KeyValuePair<string, JSONNode> kvp in json["profiles"].AsObject)
            {
                ProfileValues profileValue = ProfileValues.Deserialize(kvp.Value.ToString());
                
                req.Profiles.Add(kvp.Key, profileValue);
            }

            // 反序列化 SceneIDs
            req.SceneIDs = new List<long>();
            foreach (JSONNode node in json["scene_ids"].AsArray)
            {
                req.SceneIDs.Add(node.AsLong);
            }

            return req;
        }
    }

    public class RelativeExperiment
    {
        public long GroupId { get; set; }

        public string GroupKey;

        public string ExpKey { get; set; }

        public Dictionary<string, string> Params { get; set; }

        public ExperimentInfo ConvertToExperimentInfo()
        {
            ExperimentInfo exp = new ExperimentInfo();
            
            exp.ExpGroupKey = this.ExpKey;
            exp.ExpId = (int)this.GroupId;
            exp.ExpKey = this.GroupKey;
            return exp;
        }

        public string Serialize()
        {
            JSONObject json = new JSONObject();
            json["group_id"] = this.GroupId;
            json["group_key"] = this.GroupKey;
            json["exp_key"] = this.ExpKey;

            // 序列化 Params
            JSONObject paramsJson = new JSONObject();
            if (Params != null)
            {
                foreach (var kvp in Params)
                {
                    paramsJson[kvp.Key] = kvp.Value;
                }
            }

            json["params"] = paramsJson;

            return json.ToString();
        }
        
        public static RelativeExperiment Deserialize(string jsonString)
        {
            JSONNode json = JSON.Parse(jsonString);
            RelativeExperiment info = new RelativeExperiment();
            info.GroupId = json["group_id"].AsLong;
            info.GroupKey = json["group_key"];
            info.ExpKey = json["exp_key"];

            // 反序列化 Params
            info.Params = new Dictionary<string, string>();
            foreach (KeyValuePair<string, JSONNode> kvp in json["params"].AsObject)
            {
                info.Params.Add(kvp.Key, kvp.Value);
            }

            return info;
        }
    }

    public class FeatureFlag
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public RelativeExperiment RelativeExperiment { get; set; }

        public ConfigInfo ConvertToConfigInfo()
        {
            ConfigInfo result = new ConfigInfo();
            
            result.Name = this.Name;
            result.Value = this.Value;

            result.RelativeExperiment = this.RelativeExperiment?.ConvertToExperimentInfo();
            return result;
        }

        public string Serialize()
        {
            JSONObject json = new JSONObject();
            json["name"] = this.Name;
            json["value"] = this.Value;

            // 序列化 RelativeExperiment
            if (RelativeExperiment != null)
            {
                json["relative_experiment"] = JSON.Parse(RelativeExperiment.Serialize());
            }

            return json.ToString();
        }

        public static FeatureFlag Deserialize(string jsonString)
        {
            JSONNode json = JSON.Parse(jsonString);
            FeatureFlag flag = new FeatureFlag();
            flag.Name = json["name"];
            flag.Value = json["value"];

            // 反序列化 RelativeExperiment
            if (json["relative_experiment"] != null)
            {
                flag.RelativeExperiment = RelativeExperiment.Deserialize(json["relative_experiment"].ToString());
            }

            return flag;
        }
    }

    public class GetFeatureFlagsRsp
    {
        public RetCode RetCode { get; set; }

        public string Msg { get; set; }

        public Dictionary<string, FeatureFlag> Data { get; set; }

        public string Serialize()
        {
            JSONObject json = new JSONObject();
            json["ret_code"] = (int)this.RetCode;
            json["msg"] = this.Msg;

            // 序列化 Data
            JSONObject dataJson = new JSONObject();
            if (Data != null)
            {
                foreach (var kvp in Data)
                {
                    dataJson[kvp.Key] = JSON.Parse(kvp.Value.Serialize());
                }
            }

            json["data"] = dataJson;

            return json.ToString();
        }
        
        public static GetFeatureFlagsRsp Deserialize(string jsonString)
        {
            JSONNode json = JSON.Parse(jsonString);
            GetFeatureFlagsRsp rsp = new GetFeatureFlagsRsp();
            rsp.RetCode = (RetCode)json["ret_code"].AsInt;
            rsp.Msg = json["msg"];

            // 反序列化 Data
            rsp.Data = new Dictionary<string, FeatureFlag>();
            foreach (KeyValuePair<string, JSONNode> kvp in json["data"].AsObject)
            {
                FeatureFlag flag = FeatureFlag.Deserialize(kvp.Value.ToString());
                
                rsp.Data.Add(kvp.Key, flag);
            }

            return rsp;
        }
    }

    public class BatchLogExposureReq
    {
        public string EventCode { get; set; }

        public string EventTime { get; set; }

        public EventType EventType { get; set; }

        public EventStatus EventStatus { get; set; }

        public string AppID { get; set; }

        public List<ExposureData> Exposures { get; set; }

        public SDKInfo SdkInfo { get; set; }

        public Properties Properties { get; set; }

        public UserInfo UserInfo { get; set; }

        public Dictionary<string, string> EventValue { get; set; }

        public string Serialize()
        {
            JSONObject json = new JSONObject();
            json["event_code"] = this.EventCode;
            json["event_time"] = this.EventTime;
            json["event_type"] = (int)this.EventType;
            json["event_status"] = (int)this.EventStatus;
            json["app_id"] = this.AppID;

            // 序列化 Exposures
            JSONArray exposuresArray = new JSONArray();
            if (Exposures != null)
            {
                foreach (var exposure in Exposures)
                {
                    exposuresArray.Add(JSON.Parse(exposure.Serialize()));
                }
            }

            json["exposures"] = exposuresArray;

            // 序列化 SdkInfo
            if (SdkInfo != null)
            {
                json["sdk_info"] = JSON.Parse(SdkInfo.Serialize());
            }

            // 序列化 Properties
            if (Properties != null)
            {
                json["properties"] = JSON.Parse(Properties.Serialize());
            }

            // 序列化 UserInfo
            if (UserInfo != null)
            {
                json["user"] = JSON.Parse(UserInfo.Serialize());
            }

            // 序列化 EventValue
            JSONObject eventValueJson = new JSONObject();
            if (EventValue != null)
            {
                foreach (var kvp in EventValue)
                {
                    eventValueJson[kvp.Key] = kvp.Value;
                }
            }

            json["event_value"] = eventValueJson;

            return json.ToString();
        }

        public static BatchLogExposureReq Deserialize(string jsonString)
        {
            JSONNode json = JSON.Parse(jsonString);
            BatchLogExposureReq req = new BatchLogExposureReq();
            req.EventCode = json["event_code"];
            req.EventTime = json["event_time"];
            req.EventType = (EventType)json["event_type"].AsInt;
            req.EventStatus = (EventStatus)json["event_status"].AsInt;
            req.AppID = json["app_id"];

            // 反序列化 Exposures
            req.Exposures = new List<ExposureData>();
            foreach (JSONNode node in json["exposures"].AsArray)
            {
                ExposureData exposure = ExposureData.Deserialize(node.ToString());
                
                req.Exposures.Add(exposure);
            }

            // 反序列化 SdkInfo
            if (json["sdk_info"] != null)
            {
                req.SdkInfo = SDKInfo.Deserialize(json["sdk_info"].ToString());
            }

            // 反序列化 Properties
            if (json["properties"] != null)
            {
                req.Properties = Properties.Deserialize(json["properties"].ToString());
            }

            // 反序列化 UserInfo
            if (json["user"] != null)
            {
                req.UserInfo = UserInfo.Deserialize(json["user"].ToString());
            }

            // 反序列化 EventValue
            req.EventValue = new Dictionary<string, string>();
            foreach (KeyValuePair<string, JSONNode> kvp in json["event_value"].AsObject)
            {
                req.EventValue.Add(kvp.Key, kvp.Value);
            }

            return req;
        }
    }
}