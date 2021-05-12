using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace me.cqp.luohuaming.GenshinSign.Code.Tests
{
    [TestClass()]
    public class ApiHelperTests
    {
        private static string cookie = @"aliyungf_tc=afb3d5699c4fca008847f81767dd7ddc09d46c4584c3a7944717074002cd5cb8; UM_distinctid=178fc37e896169-0eff84da31a95d-33382005-64140-178fc37e897181; ltoken=rCm7SxYCx2pmpqdmFiMFVUEYA4n2zvdTJm446Xlx; ltuid=73743492; login_ticket=X955lLm3IW7t0OgqGNdmnpD91vsDtonK4OeVzoCd; _MHYUUID=6c65676c-1d9e-4e2c-94e2-1f0d72db4375; account_id=73743492; cookie_token=IWuuu7NTTgpzxJ6e40lcjKws7IpLXiqz30rqHl3f; _ga=GA1.1.1549218605.1619139223; _ga_KJ6J9V9VZQ=GS1.1.1620781788.1.0.1620781788.0";
        [TestMethod()]
        public void GetTodaySignRewardTest()
        {
            Debug.WriteLine(ApiHelper.GetTodaySignReward(cookie));
        }

        [TestMethod()]
        public void GetRoleByCookieTest()
        {
            Debug.WriteLine(ApiHelper.GetRoleByCookie(cookie).message);
        }

        [TestMethod()]
        public void DoSignTest()
        {
            Assert.AreEqual(
                ApiHelper.DoSign(new PublicInfos.CookieObject { Cookie = cookie, UID = 100367198, Region = "cn_gf01" })
                , PublicInfos.SignStatus.AlreadySign);
        }

        [TestMethod()]
        public void VerifyCookieTest()
        {
            Assert.AreEqual(ApiHelper.VerifyCookie(cookie), true);
        }

        [TestMethod()]
        public void GetUIDMarkTest()
        {
            Debug.WriteLine(ApiHelper.GetUIDMark(100367198));
        }
    }
}