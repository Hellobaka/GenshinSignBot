using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using me.cqp.luohuaming.GenshinSign.Tool.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PublicInfos;

namespace me.cqp.luohuaming.GenshinSign.Code
{
    public static class ApiHelper
    {
        private const string act_id = "e202009291139501";
        private const string API_SALT = "h8w582wxwgqvahcdkpvdhbh2w9casgfl";
        private const string API_APP_VERSION = "2.3.0";
        private const string API_CLIENT_TYPE = "5";
        private const string game_biz = "hk4e_cn";

        private static string GetUserGameRolesByCookie { get; set; } = $"https://api-takumi.mihoyo.com/binding/api/getUserGameRolesByCookie?game_biz={game_biz}";
        private static string GetSignRewardInfo { get; set; } = "https://api-takumi.mihoyo.com/event/bbs_sign_reward/info";
        private static string GetSignRewardList { get; set; } = "https://api-takumi.mihoyo.com/event/bbs_sign_reward/home";
        private static string PostSignInfo { get; set; } = "https://api-takumi.mihoyo.com/event/bbs_sign_reward/sign";

        public static string GetTodaySignReward(CookieObject cookie)
        {
            return GetTodaySignReward(cookie.Cookie);
        }
        public static RoleInfo GetRoleByCookie(CookieObject cookie)
        {
            return GetRoleByCookie(cookie.Cookie);
        }
        public static RoleInfo GetRoleByCookie(string cookie)
        {
            var http = GetNonKeyHttp(cookie);
            return JsonConvert.DeserializeObject<RoleInfo>(http.DownloadString(GetUserGameRolesByCookie));
        }
        public static string GetTodaySignReward(string cookie)
        {
            var http = GetNonKeyHttp(cookie, true);
            string url = CommonHelper.LinkGetURL(GetSignRewardList, new List<(string, string)> { ("act_id", act_id) });
            var awards = JsonConvert.DeserializeObject<SignAwardList>(http.DownloadString(url));
            return awards.data.awards[DateTime.Now.Day - 1].name + $" ×{awards.data.awards[DateTime.Now.Day - 1].cnt}";
        }
        private static HttpWebClient GetNonKeyHttp(string cookie, bool userdeviceid = false)
        {
            HttpWebClient http = new HttpWebClient
            {
                Encoding = Encoding.UTF8,
                Referer = $"https://webstatic.mihoyo.com/bbs/event/signin-ys/index.html?bbs_auth_required=true&act_id={act_id}&utm_source=bbs&utm_medium=mys&utm_campaign=icon",
                UserAgent = "Mozilla/5.0 (Linux; Android 6.0.1; MuMu Build/V417IR; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/52.0.2743.100 Mobile Safari/537.36 miHoYoBBS/2.7.0",
            };
            http.Headers.Add("X-Requested-With", "com.mihoyo.hyperion");
            http.Headers.Add("Origin", "https://webstatic.mihoyo.com");
            if (userdeviceid)
                http.Headers.Add("x-rpc-device_id", Guid.NewGuid().ToString());
            http.CookieCollection = CommonHelper.CookieStr2Collection(cookie);
            return http;
        }
        private static HttpWebClient GetNonKeyHttp(CookieObject cookie)
        {
            return GetNonKeyHttp(cookie.Cookie);
        }

        private static HttpWebClient GetWithKeyHttp(CookieObject cookie)
        {
            return GetWithKeyHttp(cookie.Cookie);
        }
        private static HttpWebClient GetWithKeyHttp(string cookie)
        {
            HttpWebClient http = new HttpWebClient
            {
                Encoding = Encoding.UTF8,
                Referer = $"https://webstatic.mihoyo.com/bbs/event/signin-ys/index.html?bbs_auth_required=true&act_id={act_id}&utm_source=bbs&utm_medium=mys&utm_campaign=icon",
                UserAgent = "Mozilla/5.0 (Linux; Android 6.0.1; MuMu Build/V417IR; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/52.0.2743.100 Mobile Safari/537.36 miHoYoBBS/2.7.0",
            };
            http.Headers.Add("X-Requested-With", "com.mihoyo.hyperion");
            http.Headers.Add("x-rpc-client_type", API_CLIENT_TYPE);
            http.Headers.Add("x-rpc-app_version", API_APP_VERSION);
            http.Headers.Add("DS", CreateDynamicSecret());
            http.Headers.Add("x-rpc-device_id", Guid.NewGuid().ToString());
            http.Headers.Add("Origin", "https://webstatic.mihoyo.com");

            http.CookieCollection = CommonHelper.CookieStr2Collection(cookie);
            return http;
        }
        private static string CreateDynamicSecret()
        {
            long time = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            string random = CreateRandomString(6);
            string check = ComputeMd5($"salt={API_SALT}&t={time}&r={random}");

            return $"{time},{random},{check}";
        }
        private static string CreateRandomString(int length)
        {
            StringBuilder builder = new StringBuilder(length);

            const string randomStringTemplate = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                int pos = random.Next(0, randomStringTemplate.Length);
                builder.Append(randomStringTemplate[pos]);
            }

            return builder.ToString();
        }
        private static string ComputeMd5(string content)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] result = md5.ComputeHash(Encoding.UTF8.GetBytes(content ?? ""));

                StringBuilder builder = new StringBuilder();
                foreach (byte b in result)
                    builder.Append(b.ToString("x2"));

                return builder.ToString();
            }
        }

        /// <summary>
        /// 进行签到
        /// </summary>
        /// <param name="user">用户的Cookie</param>
        /// <returns>是否签到成功</returns>
        public static SignStatus DoSign(CookieObject user)
        {
            var http = GetWithKeyHttp(user);
            JObject json = new JObject
            {
                {"act_id", act_id},
                {"region", user.Region},
                {"uid", user.UID.ToString()}
            };

            SignResult result = JsonConvert.DeserializeObject<SignResult>(http.UploadString(PostSignInfo, json.ToString()));
            return GetStatusByRetCode(result.retcode);
        }
        public static SignStatus GetStatusByRetCode(int retCode)
        {
            switch (retCode)
            {
                case 0:
                    return SignStatus.OK;
                case -5003:
                    return SignStatus.AlreadySign;
                default:
                    return SignStatus.Fail;
            }
        }
        /// <summary>
        /// 通过Cookie获取对象
        /// </summary>
        /// <param name="cookie">未验证的Cookie</param>
        /// <returns>CookieObject 对象 返回Null则说明Cookie无效</returns>
        public static CookieObject GetCookieObject(long QQID, string cookie, RoleInfoList data)
        {
            return new CookieObject
            {
                Cookie = cookie,
                NickName = data.nickname,
                UID = Convert.ToInt64(data.game_uid),
                Region = data.region,
                QQID = QQID
            };
        }
        public static bool VerifyCookie(string cookie)
        {
            return GetRoleByCookie(cookie).retcode == 0;
        }
        public static bool VerifyCookie(CookieObject cookie)
        {
            return VerifyCookie(cookie.Cookie);
        }
        public static void StartSignJob()
        {
            Thread thread = new Thread(() =>
            {
                MainSave.CQLog.Info("签到线程开启", $"签到线程已开启，间隔 {MainSave.AppConfig.WaitSecond} 秒");
                while (true)
                {
                    try
                    {
                        if (MainSave.AppConfig.VerifySignTime(DateTime.Now) && MainSave.TodaySignFlag is false)
                        {
                            SignFunction();
                        }
                        //如果未签到且当前时间大于签到时间90秒则进行签到
                        var t = new DateTime(1970, 1, 1, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                        if ((t - MainSave.AppConfig.StartTime).TotalSeconds > 90 && MainSave.TodaySignFlag is false)
                        {
                            SignFunction();
                        }
                    }
                    catch (Exception e)
                    {
                        MainSave.CQLog.Error("签到线程异常", e.Message, e.StackTrace);
                    }
                    Thread.Sleep(1000 * MainSave.AppConfig.WaitSecond);
                }
            });
            thread.Start();
        }
        public static string GetUIDMark(long UID)
        {
            string c = UID.ToString();
            return new string('*', c.Length - 3) + c.Substring(c.Length - 3);
        }
        private static void SignFunction()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<CookieObject> SuccessList = new List<CookieObject>();
            List<CookieObject> FailList = new List<CookieObject>();
            int alreadySignCount = 0;
            MainSave.TodaySignFlag = true;
            foreach (var item in MainSave.CookieList)
            {
                if (item.Useable is false)
                    continue;
                var result = DoSign(item);
                if (result == SignStatus.OK)
                {
                    SuccessList.Add(item);
                }
                else if (result == SignStatus.Fail)
                {
                    FailList.Add(item);
                }
                else if (result == SignStatus.AlreadySign)
                {
                    alreadySignCount++;
                }
                Thread.Sleep(5000);
            }
            StringBuilder sb = new StringBuilder();
            int successCount = SuccessList.Count, failCount = FailList.Count;
            sb.AppendLine($"原神签到小助手 - {DateTime.Now.ToLongDateString()}");
            sb.AppendLine("--------------------");
            sb.AppendLine($"签到成功了: {successCount} 个 | 失败 {failCount} 个 | 重复签到 {alreadySignCount}个");
            sb.AppendLine("--------------------");
            sb.AppendLine($"今天签到奖励: {GetTodaySignReward(MainSave.CookieList.First(x => x.Useable).Cookie)}");

            if (failCount != 0)
            {
                //检验Cookie状态
                foreach (var item in FailList)
                {
                    var flag = VerifyCookie(item);
                    SignStatus result;
                    if (flag)
                    {
                        result = DoSign(item);
                        if (result != SignStatus.OK)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                result = DoSign(item);
                                if (result == SignStatus.OK)
                                {
                                    result = SignStatus.RetrySuccess;
                                    break;
                                }
                                Thread.Sleep(5000);
                                if (i == 2)//3try fail
                                {
                                    MainSave.CQLog.Error("重新签到失败", $"QQ={item.QQID} 签到重试失败，建议联系作者或手动校验问题");
                                    result = SignStatus.RetryFail;
                                }
                            }
                        }
                        else
                            Thread.Sleep(5000);
                    }
                    else
                    {
                        result = SignStatus.CookieUnusable;
                        item.Useable = false;
                        CookieObject.SaveObject();
                    }
                    MainSave.CQLog.Info("失败校验", $"QQ={item.QQID} 的校验结果为：{GetSignStatusStr(result)}");
                }
            }
            sw.Stop();
            MainSave.CQLog.Info("今日原神签到", $"共耗时: {sw.ElapsedMilliseconds / (double)1000}s 签到成功了: {successCount} 个 失败 {failCount} 个 Cookie失效 {0} 个");
            foreach (var item in MainSave.AppConfig.BroadcastGroup)
            {
                MainSave.CQApi.SendGroupMessage(item, sb.ToString());
            }
        }
        public static string GetSignStatusStr(SignStatus status)
        {
            switch (status)
            {
                case SignStatus.OK:
                    return "签到已成功";
                case SignStatus.Fail:
                    return "签到失败，尝试校验Cookie以及重试";
                case SignStatus.CookieUnusable:
                    return "Cookie失效，已将此Cookie标记为不可用";
                case SignStatus.RetrySuccess:
                    return "签到重试成功";
                case SignStatus.RetryFail:
                    return "Cookie可用但签到重试失败";
                case SignStatus.AlreadySign:
                    return "重复签到";
                default:
                    return "-";
            }
        }
    }
}
