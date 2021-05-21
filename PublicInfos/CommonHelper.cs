using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using me.cqp.luohuaming.GenshinSign.Sdk.Cqp.Model;
using me.cqp.luohuaming.GenshinSign.Tool.IniConfig;

namespace me.cqp.luohuaming.GenshinSign.PublicInfos
{
    public static class CommonHelper
    {
        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }
        /// <summary>
        /// 获取CQ码中的图片网址
        /// </summary>
        /// <param name="imageCQCode">需要解析的图片CQ码</param>
        /// <returns></returns>
        public static string GetImageURL(string imageCQCode)
        {
            string path = MainSave.ImageDirectory + CQCode.Parse(imageCQCode)[0].Items["file"] + ".cqimg";
            IniConfig image = new IniConfig(path);
            image.Load();
            return image.Object["image"]["url"].ToString();
        }
        public static string GetAppImageDirectory()
        {
            var ImageDirectory = Path.Combine(Environment.CurrentDirectory, "data", "image\\");
            return ImageDirectory;
        }
        public static string LinkGetURL(string baseurl, List<(string, string)> queryargs)
        {
            if (baseurl.EndsWith("?") is false)
                baseurl += "?";
            foreach (var item in queryargs)
            {
                baseurl += $"{item.Item1}={item.Item2}&";
            }
            return baseurl.Substring(0, baseurl.Length - 1);
        }
        public static CookieCollection CookieStr2Collection(string cookie)
        {
            CookieCollection collection = new CookieCollection();
            if (string.IsNullOrEmpty(cookie) || cookie.Length < 5 || cookie.IndexOf("=") < 0) return collection;
            var cookieC = cookie.Split(';');
            foreach(var item in cookieC)
            {
                var res = item.Trim().Split('=');
                collection.Add(new Cookie(res[0], res[1]));
            }
            return collection;
        }
    }
}
