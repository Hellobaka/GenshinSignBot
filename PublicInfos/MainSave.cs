using System;
using System.Collections.Generic;
using System.IO;
using me.cqp.luohuaming.GenshinSign.Sdk.Cqp;
using me.cqp.luohuaming.GenshinSign.Tool.Http;

namespace me.cqp.luohuaming.GenshinSign.PublicInfos
{
    public static class MainSave
    {
        /// <summary>
        /// 保存各种事件的数组
        /// </summary>
        public static List<IOrderModel> Instances { get; set; } = new List<IOrderModel>();
        public static CQLog CQLog { get; set; }
        public static CQApi CQApi { get; set; }
        public static string AppDirectory { get; set; }
        public static string ImageDirectory { get; set; }
        public static string CookiePath { get; set; }
        public static string ConfigPath { get; set; }
        public static List<CookieObject> CookieList { get; set; } = new List<CookieObject>();
        public static bool TodaySignFlag
        {
            get
            {
                return AppConfig.LastSign.Day == DateTime.Now.Day 
                    && AppConfig.LastSign.Month == DateTime.Now.Month 
                    && AppConfig.LastSign.Year == DateTime.Now.Year;
            }
            set 
            {
                if (value)
                {
                    AppConfig.LastSign = DateTime.Now;
                }
                else
                {
                    AppConfig.LastSign = new DateTime();
                }
                AppConfig.SaveConfig();
            }
        }
        public static AppConfig AppConfig { get; set; } = AppConfig.LoadConfig();
    }
}
