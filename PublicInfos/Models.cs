using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using me.cqp.luohuaming.GenshinSign.Sdk.Cqp.EventArgs;
using Newtonsoft.Json;

namespace PublicInfos
{
    public enum SignStatus
    {
        OK,
        Fail,
        AlreadySign,
        CookieUnusable,
        RetrySuccess,
        RetryFail
    }
    public class AppConfig
    {
        public DateTime StartTime { get; set; } = new DateTime(1970, 1, 1, 10, 0, 0);
        public List<long> BroadcastGroup { get; set; } = new List<long>();
        public int WaitSecond { get; set; } = 30;
        public DateTime LastSign { get; set; } = new DateTime(1970, 1, 1, 10, 0, 0);
        public static void SaveConfig()
        {
            File.WriteAllText(MainSave.ConfigPath, JsonConvert.SerializeObject(MainSave.AppConfig));
        }
        public static AppConfig LoadConfig()
        {
            if (File.Exists(MainSave.ConfigPath) is false)
                return new AppConfig();
            return JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(MainSave.ConfigPath));
        }
        public bool VerifySignTime(DateTime time)
        {
            return time.Hour == StartTime.Hour && time.Minute == StartTime.Minute && time.Second == StartTime.Second;
        }
    }

    public class CookieObject
    {
        public long UID { get; set; } = 0;
        public long QQID { get; set; } = 0;
        public string NickName { get; set; } = "";
        public string Cookie { get; set; } = "";
        public bool Useable { get; set; } = true;
        public string Region { get; set; } = "";
        public static void SaveObject()
        {
            File.WriteAllText(MainSave.CookiePath, JsonConvert.SerializeObject(MainSave.CookieList));
        }
        public static List<CookieObject> LoadObject()
        {
            return JsonConvert.DeserializeObject<List<CookieObject>>(File.ReadAllText(MainSave.CookiePath));
        }
    }
    #region SignResult
    public class SignResult
    {
        public int retcode { get; set; }
        public string message { get; set; }
        public SignResultData data { get; set; }
    }
    [JsonObject("data")]
    public class SignResultData
    {
        public int total_sign_day { get; set; }
        public string today { get; set; }
        public bool is_sign { get; set; }
        public bool first_bind { get; set; }
        public bool is_sub { get; set; }
        public bool month_first { get; set; }
    }
    #endregion
    #region SignAwardList
    public class SignAwardList
    {
        public int retcode { get; set; }
        public string message { get; set; }
        public SignAwardListData data { get; set; }
    }
    [JsonObject("data")]
    public class SignAwardListData
    {
        public int month { get; set; }
        public Award[] awards { get; set; }
    }

    public class Award
    {
        public string icon { get; set; }
        public string name { get; set; }
        public int cnt { get; set; }
    }
    #endregion
    #region RoleInfo
    public class RoleInfo
    {
        public int retcode { get; set; }
        public string message { get; set; }
        public RoleInfoData data { get; set; }
    }
    [JsonObject("data")]
    public class RoleInfoData
    {
        public RoleInfoList[] list { get; set; }
    }
    [JsonObject("list")]
    public class RoleInfoList
    {
        public string game_biz { get; set; }
        public string region { get; set; }
        public string game_uid { get; set; }
        public string nickname { get; set; }
        public int level { get; set; }
        public bool is_chosen { get; set; }
        public string region_name { get; set; }
        public bool is_official { get; set; }
    }
    #endregion
    public interface IOrderModel
    {
        string GetOrderStr();
        bool Judge(string destStr);
        FunctionResult Progress(CQGroupMessageEventArgs e);
        FunctionResult Progress(CQPrivateMessageEventArgs e);
    }
}
