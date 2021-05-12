using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using me.cqp.luohuaming.GenshinSign.Sdk.Cqp.EventArgs;
using PublicInfos;

namespace me.cqp.luohuaming.GenshinSign.Code.OrderFunctions
{
    public class SignMain : IOrderModel
    {
        private static Dictionary<long, SetCookieStep> QQ2RoleInfo = new Dictionary<long, SetCookieStep>();
        public string GetOrderStr() => "";

        public bool Judge(string destStr) => true;//这里判断是否能触发指令

        public FunctionResult Progress(CQGroupMessageEventArgs e)//群聊处理
        {
            FunctionResult result = new FunctionResult
            {
                Result = false,
                SendFlag = false,
            };
            return result;
        }
        public FunctionResult Progress(CQPrivateMessageEventArgs e)//私聊处理
        {
            FunctionResult result = new FunctionResult
            {
                Result = true,
                SendFlag = true,
            };
            SendText sendText = new SendText
            {
                SendID = e.FromQQ,
            };
            result.SendObject.Add(sendText);

            if (QQ2RoleInfo.ContainsKey(e.FromQQ))
            {
                if (e.Message.Text == "#进度重置")
                {
                    QQ2RoleInfo.Remove(e.FromQQ);
                    sendText.MsgToSend.Add("进度已重置，请重新输入 #原神签到");
                    return result;
                }
                string reply = QQ2RoleInfo[e.FromQQ].GetReply(e.Message.Text);
                if (string.IsNullOrWhiteSpace(reply))
                {
                    QQ2RoleInfo.Remove(e.FromQQ);
                }
                else
                {
                    sendText.MsgToSend.Add(reply);
                }
            }
            else
            {
                if (e.Message.Text == "#原神签到")
                {
                    QQ2RoleInfo.Add(e.FromQQ, new SetCookieStep());
                    sendText.MsgToSend.Add(QQ2RoleInfo[e.FromQQ].GetReply(e.Message.Text));
                }
            }
            return result;
        }
        public class SetCookieStep
        {
            public enum StepOrder
            {
                Non,
                InputCookie,
                SelectCharacter,
                Done
            }
            public CookieObject RoleInfo { get; set; } = new CookieObject();
            public StepOrder Order { get; set; }
            private RoleInfo RawRoleInfo { get; set; } = new RoleInfo();
            public string GetReply(string msg)
            {
                switch (Order)
                {
                    case StepOrder.Non:
                        Order = StepOrder.InputCookie;
                        return "感谢你使用原神签到机，首先请输入你的米游社Cookie，不知道如何获取请询问Bot管理员";
                    case StepOrder.InputCookie:
                        if (string.IsNullOrWhiteSpace(msg))
                            return "无效回复，请输入米游社Cookie，需要重置进度请输入 #进度重置";
                        var result = ApiHelper.GetRoleByCookie(msg);
                        if (result.retcode == 0)
                        {
                            RoleInfo.Cookie = msg;
                            RawRoleInfo = result;
                            var ls = RawRoleInfo.data.list;
                            if (ls.Length == 0)
                            {
                                Order = StepOrder.Done;
                                RoleInfo = null;
                                return "未检索到角色，请先去米游社绑定你的原神角色";
                            }
                            else if (ls.Length == 1)
                            {
                                Order = StepOrder.Done;
                                var role = ls[0];
                                RoleInfo.NickName = role.nickname;
                                RoleInfo.UID = Convert.ToInt64(role.game_uid);
                                RoleInfo.Region = role.region;
                                MainSave.CookieList.Add(RoleInfo);
                                CookieObject.SaveObject();
                                return $"UID: {ApiHelper.GetUIDMark(RoleInfo.UID)} 昵称: {RoleInfo.NickName} 绑定成功！\n每天会帮你进行签到的ヽ(￣▽￣)ﾉ";
                            }
                            else
                            {
                                Order = StepOrder.SelectCharacter;
                                StringBuilder sb = new StringBuilder();
                                sb.AppendLine("检索到多个角色，请输入序号来确认角色");
                                int Characterindex = 1;
                                foreach (var item in ls)
                                {
                                    sb.AppendLine($"{Characterindex}. UID:{item.game_uid} 昵称:{item.nickname}");
                                }
                                return sb.ToString();
                            }
                        }
                        else
                            return "Cookie校验失败，请确认获取方式是否正确";
                    case StepOrder.SelectCharacter:
                        if(int.TryParse(msg, out int index))
                        {
                            Order = StepOrder.Done;
                            var role = RawRoleInfo.data.list[index];
                            RoleInfo.NickName = role.nickname;
                            RoleInfo.UID = Convert.ToInt64(role.game_uid);
                            RoleInfo.Region = role.region;
                            MainSave.CookieList.Add(RoleInfo);
                            CookieObject.SaveObject();
                            return $"UID: {ApiHelper.GetUIDMark(RoleInfo.UID)} 昵称: {RoleInfo.NickName} 绑定成功！\n每天会帮你进行签到的ヽ(￣▽￣)ﾉ";
                        }
                        else
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine("请输入数字序号，需要重置进度请输入 #进度重置");
                            int Characterindex = 1;
                            foreach (var item in RawRoleInfo.data.list)
                            {
                                sb.AppendLine($"{Characterindex}. UID:{item.game_uid} 昵称:{item.nickname}");
                            }
                            return sb.ToString();
                        }
                    case StepOrder.Done:
                    default:
                        return "";
                }
            }
        }
    }
}
