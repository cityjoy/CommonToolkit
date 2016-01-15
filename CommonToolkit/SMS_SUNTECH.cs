using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  CommonToolKit
{
    public class SMS_SUNTECH
    {
        #region 用户名和密码
        /// <summary>
        /// 获取用户名
        /// </summary>
        private static string LoginId
        {
            get
            {
                return "headway01";
            }
        }
        /// <summary>
        /// 获取密码
        /// </summary>
        private static string LoginPwd
        {
            get
            {
                return "headway123";
            }
        }
        /// <summary>
        /// 获取密码
        /// </summary>
        private static string Key
        {
            get
            {
                return "00000";
            }
        }
        #endregion
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="mobiles">手机号集合（多个以英文逗号分隔）</param>
        /// <param name="smsContent">短信内容</param>
        /// <param name="errorMsg">错误消息</param>
        /// <returns></returns>
        public static bool MT(string mobiles, string smsContent,ref string errorMsg)
        {
            errorMsg = string.Empty;
            if (mobiles.Length >= 11)
            {
                //手机号集合
                List<string> listMobile = GetMobileList(mobiles);
                foreach (string item in listMobile)
                {
                    //对短信内容进行编码
                    smsContent = System.Web.HttpUtility.UrlEncode(smsContent, System.Text.Encoding.GetEncoding("GB2312"));
                    //加密KEY
                    string key = GetKey(mobiles);
                    //发送短信需要使用的URL
                    string sendUrl = string.Format("http://http.asp.sh.cn/MT.do?Username={0}&Password={1}&Mobile={2}&Content={3}&Keyword={4}", LoginId, LoginPwd, mobiles, smsContent, key);
                    //发送且获取状态
                    string sendStatus = GetResponseResults(sendUrl);

                    switch (sendStatus)
                    {
                        case "0":
                            errorMsg += string.Empty;
                            break;
                        case "-1":
                            errorMsg += "用户名或密码验证错误；";
                            break;
                        case "-2":
                            errorMsg += "发送短信余额不足；";
                            break;
                        case "-3":
                            errorMsg += "号码超容（单次发送不大于100个号码）；";
                            break;
                        case "-4":
                            errorMsg += "内容非法（内容含被过滤的关键字）；";
                            break;
                        case "-5":
                            errorMsg += "内容超长；";
                            break;
                        case "-6":
                            errorMsg += "密钥验证出错；";
                            break;
                        case "-9":
                            errorMsg += "函数入参不正确（某参数为空或参数数据类型不正确）；";
                            break;
                        default:
                            errorMsg += "未知错误；";
                            break;
                    }
                }
                if (errorMsg == string.Empty)
                {
                    return true;
                }
                else
                {
                    errorMsg = "发送短信发生错误，但部分信息可能已发送成功。错误消息如下：" + errorMsg;
                    return false;
                }
            }
            else
            {
                errorMsg = "您没有指定接受者手机号码或指定的号码无效";
                return false;
            }
        }

        /// <summary>
        /// 获取短信集合（20个为一组）
        /// </summary>
        /// <param name="mobiles"></param>
        /// <returns></returns>
        private static List<string> GetMobileList(string mobiles)
        {
            //手机号集合
            string[] mobileList = mobiles.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            List<string> listMobile = new List<string>();
            string mob = string.Empty;
            for (int i = 1; i <= mobileList.Length; i++)
            {
                int index = i - 1;
                mob += mob != string.Empty ? "," + mobileList[index] : mobileList[index];
                if (i % 20 == 0)
                {
                    listMobile.Add(mob);
                    mob = string.Empty;
                }
            }
            if (mob != string.Empty)
            {
                listMobile.Add(mob);
            }
            return listMobile;
        }


        /// <summary>
        /// 加密KEY:
        ///     * 密钥，密钥用于验证用户执行该操作的合法性，如果您使用的是测试帐户，可将Keyword值设置为空，
        ///     * 如果您是正式用户，客服人员会告知您帐户密钥。得到密钥后，您需要在本地进行相关的加密处理，
        ///     * 加密规则为：Mobile参数值的前8位 + Mobile参数值的后10位 + 密钥，三者以字符串形式相加，
        ///     * 将相加的结果用MD5的32位加密方式加密后得出的值即为Keyword的值。例如，帐户密钥为abcxyz，
        ///     * 同时发送短信给13636389987和13166057339(此时Mobile的值为“13636389987,13166057339”)，
        ///     * 根据规则，三者相加后得出136363893166057339abcxyz，再用MD5的32位加密方式加密后
        ///     * 得出D177F229627442CBDD0321E0F55FB965，此值即为Keyword的值。
        /// </summary>
        /// <param name="mobiles">接受者手机号</param>
        /// <returns></returns>
        private static string GetKey(string mobiles)
        {
            string start = mobiles.Substring(0, 8);
            string end = mobiles.Substring(mobiles.Length-10, 10);
            string key = start + end + Key;
            key = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(key, "MD5").ToLower().Substring(0, 32);
            key = key.ToUpper();
            return key;
        }

        /// <summary>
        /// 获取输出结果
        /// </summary>
        /// <param name="urlString"></param>
        /// <returns></returns>
        private static string GetResponseResults(string urlString)
        {
            string sMsg = "";		//引用的返回字符串
            try
            {
                System.Net.HttpWebResponse rs = (System.Net.HttpWebResponse)System.Net.HttpWebRequest.Create(urlString).GetResponse();
                System.IO.StreamReader sr = new System.IO.StreamReader(rs.GetResponseStream(), System.Text.Encoding.Default);
                sMsg = sr.ReadToEnd();
            }
            catch
            {
                return sMsg;
            }
            return sMsg;
        }
    }
}
