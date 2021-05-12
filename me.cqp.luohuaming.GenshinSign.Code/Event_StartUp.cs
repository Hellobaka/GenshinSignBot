using System.Collections.Generic;
using System.IO;
using me.cqp.luohuaming.GenshinSign.Code.OrderFunctions;
using me.cqp.luohuaming.GenshinSign.Sdk.Cqp.EventArgs;
using me.cqp.luohuaming.GenshinSign.Sdk.Cqp.Interface;
using PublicInfos;

namespace me.cqp.luohuaming.GenshinSign.Code
{
    public class Event_StartUp : ICQStartup
    {
        public void CQStartup(object sender, CQStartupEventArgs e)
        {
            MainSave.AppDirectory = e.CQApi.AppDirectory;
            MainSave.CQApi = e.CQApi;
            MainSave.CQLog = e.CQLog;
            MainSave.ImageDirectory = CommonHelper.GetAppImageDirectory();
            MainSave.CookiePath = Path.Combine(e.CQApi.AppDirectory, "Cookies.json");
            MainSave.ConfigPath = Path.Combine(e.CQApi.AppDirectory, "Config.json");
            //这里写处理逻辑
            MainSave.Instances.Add(new SignMain());//这里需要将指令实例化填在这里
            if (File.Exists(MainSave.ConfigPath))
            {
                MainSave.AppConfig = AppConfig.LoadConfig();
            }
            else
            {
                MainSave.AppConfig = new AppConfig();
                AppConfig.SaveConfig();
            }
            if (File.Exists(MainSave.CookiePath))
            {
                MainSave.CookieList = CookieObject.LoadObject();
            }
            else
            {
                MainSave.CookieList = new List<CookieObject>();
                CookieObject.SaveObject();
            }
            ApiHelper.StartSignJob();
        }
    }
}
