using System.Collections.Generic;

namespace AbcSDKSpace
{
    public class Config
    {
        public string GameId { get; set; }
        
        public string ApiKey { get; set; }
        
        public string ServerUrl { get; set; }
        
        public AutoTrackConfig AutoTrack { get; set; }
        
        public Environment? Env { get; set; }
        
        public bool EnableAutoExposure { get; set; }
        
        public bool EnableAutoPoll { get; set; }
        
        public Dictionary<string, List<string>> Attributes { get; set; }
        
        public int? RequestTimeout { get; set; }
        
        public string UnitId { get; set; }
        
        public string DeviceId { get; set; }
        
        public bool? EnableLog { get; set; }

        public enum Environment
        {
            Release,
            Develop
        }

        public Config()
        {
            // Initialize the dictionary to prevent null reference exceptions.
            Attributes = new Dictionary<string, List<string>>();
            AutoTrack = new AutoTrackConfig();
            EnableAutoExposure = true;
            EnableAutoPoll = true;
        }
    }

    public class AutoTrackConfig
    {
        public bool MgShow { set; get; }
        
        public bool MgHide { set; get; }
        
        public bool MgShare { set; get; }

        public AutoTrackConfig()
        {
            MgShow = true;
            MgHide = true;
            MgShare = true;
        }
    }
}