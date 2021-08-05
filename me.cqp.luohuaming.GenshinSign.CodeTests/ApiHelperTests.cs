using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace me.cqp.luohuaming.GenshinSign.Code.Tests
{
    [TestClass()]
    public class ApiHelperTests
    {
        private static string cookie = @"";
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
            //long uid = 100011789;
            long uid = 100386600;
            Assert.AreEqual(
                ApiHelper.DoSign(new PublicInfos.CookieObject { Cookie = cookie, UID = uid, Region = "cn_gf01" })
                , PublicInfos.SignStatus.OK);
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