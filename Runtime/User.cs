using System.Collections.Generic;

namespace AbcSDKSpace
{
    public class User
    {
        public Dictionary<string, string> Properties;
        
        public string UserID { get; set; }
        
        public string DeviceID { get; set; }

        public User()
        {
            Properties = new Dictionary<string, string>();
        }

        public User(string userID)
        {
            UserID = userID;
            Properties = new Dictionary<string, string>();
        }

        public static string Uuid(string gameID)
        {
            var key = $"_{gameID}_abc_uuid";
            
#if ABC_BYTEDANCE_MINIGAME
            var uuid = StarkSDKSpace.StarkSDK.API.PlayerPrefs.GetString(key, "");
#else
            var uuid = PlayerPrefs.GetString(key, "");
#endif
            
            if (uuid == "")
            {
                uuid = Utils.Uuid();
#if ABC_BYTEDANCE_MINIGAME
                StarkSDKSpace.StarkSDK.API.PlayerPrefs.SetString(key, uuid);
#else
                PlayerPrefs.SetString(key, uuid);
#endif
            }

            return uuid;
        }

        public static string GetUnitID(string gameID)
        {
            var key = $"_{gameID}_abc_unitID";
#if ABC_BYTEDANCE_MINIGAME
            return StarkSDKSpace.StarkSDK.API.PlayerPrefs.GetString(key, "");
#else
            return PlayerPrefs.GetString(key, "");
#endif
        }

        public static void SetUnitID(string gameID, string unitID)
        {
            if (unitID == "")
            {
                return;
            }
            var key = $"_{gameID}_abc_unitID";
#if ABC_BYTEDANCE_MINIGAME
            StarkSDKSpace.StarkSDK.API.PlayerPrefs.SetString(key, unitID);
#else
            PlayerPrefs.SetString(key, unitID);
#endif
            return;
        }
    }
}