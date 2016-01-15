using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Web;

namespace  CommonToolKit
{
    public class CommonFunction
    {
        /// <summary>
        /// 获取当前时间字符串(年,月,日,时,分,秒,3位毫秒)
        /// </summary>
        public static string AccurateTime
        {
            get
            {
                string time = DateTime.Now.ToString("yMdhhmmssfff");
                return time;
            }
        }
        /// <summary>
        /// 去除URL指定参数
        /// </summary>
        /// <param name="url">要处理的URL</param>
        /// <param name="param">要去除的参数</param>
        /// <returns></returns>
        public static string ReplaceUrl(string url, string param)
        {
            string url1 = url;
            if (url.IndexOf(param) > 0)
            {
                if (url.IndexOf("&", url.IndexOf(param) + param.Length) > 0)
                {
                    string urlLeft = url.Substring(0, url.IndexOf(param) - 1);
                    string urlRight = url.Substring(url.IndexOf("&", url.IndexOf(param) + param.Length) + 1);
                    if (urlLeft.LastIndexOf("?") != -1 || urlLeft.LastIndexOf("&") != -1) { urlLeft += "&"; }
                    else { urlLeft += "?"; }
                    url1 = urlLeft + urlRight;
                }
                else { url1 = url.Substring(0, url.IndexOf(param) - 1); }
            }
            return url1;
        }
        /// <summary>
        /// 中英文混合字符串截取
        /// </summary>
        /// <param name="p_SrcString">要截取的字符串</param>
        /// <param name="p_Length">截取长度</param>
        /// <param name="IfClearHtml">是否去除html代码</param>
        /// <returns></returns>
        public static string GetSubString(string p_SrcString, int p_Length,bool IfClearHtml)
        {
            string myResult = string.Empty;
            if (IfClearHtml) { myResult = NoHTML(p_SrcString); p_SrcString = myResult; }
            else { myResult = p_SrcString; }
            if (p_Length >= 0)
            {
                byte[] bsSrcString = System.Text.Encoding.GetEncoding("GB2312").GetBytes(p_SrcString);
                if (bsSrcString.Length >= p_Length)
                {
                    int nRealLength = p_Length;
                    int[] anResultFlag = new int[p_Length];
                    byte[] bsResult = null;
                    int nFlag = 0;
                    for (int i = 0; i < p_Length; i++)
                    {
                        if (bsSrcString[i] > 127)
                        {
                            nFlag++;
                            if (nFlag == 3)
                                nFlag = 1;
                        }
                        else
                            nFlag = 0;
                        anResultFlag[i] = nFlag;
                    }
                    if ((bsSrcString[p_Length - 1] > 127) && (anResultFlag[p_Length - 1] == 1))
                        nRealLength = p_Length + 1;
                    bsResult = new byte[nRealLength];
                    Array.Copy(bsSrcString, bsResult, nRealLength);
                    myResult = System.Text.Encoding.GetEncoding("GB2312").GetString(bsResult);
                }
            }
            return myResult == p_SrcString ? myResult : myResult + "...";
        }

        ///   <summary>   
        ///   去除HTML标记   
        ///   </summary>   
        ///   <param   name="NoHTML">包括HTML的源码   </param>   
        ///   <returns>已经去除后的文字</returns>   
        public static string NoHTML(string Htmlstring)
        {
            //删除脚本   
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML   
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", "   ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);

            Htmlstring.Replace("<", "");
            Htmlstring.Replace(">", "");
            Htmlstring.Replace("\r\n", "");
            Htmlstring = HttpContext.Current.Server.HtmlEncode(Htmlstring).Trim();

            return Htmlstring;
        }

        /// <summary>
        /// 取得HTML中所有图片的 URL。
        /// </summary>
        /// <param name="sHtmlText">HTML代码</param>
        /// <returns>图片的URL列表</returns>
        public static string[] GetHtmlImgList(string sHtmlText)
        {
            // 定义正则表达式用来匹配 img 标签
            Regex regImg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);

            // 搜索匹配的字符串
            MatchCollection matches = regImg.Matches(sHtmlText);

            int i = 0;
            string[] sUrlList = new string[matches.Count];

            // 取得匹配项列表
            foreach (Match match in matches)
                sUrlList[i++] = match.Groups["imgUrl"].Value;

            return sUrlList;
        }
        /// <summary>
        /// 获取HTML中第一个图片的路径
        /// </summary>
        /// <param name="htmlContent"></param>
        /// <returns></returns>
        public static string GetFirstImgSrc(string htmlContent)
        {
            string[] str = GetHtmlImgList(htmlContent);
            if (str.Length > 0)
            {
                return str[0];
            }
            return string.Empty;
        }
        /// <summary>
        /// 获取HTML中最后一个图片的路径
        /// </summary>
        /// <param name="htmlContent"></param>
        /// <returns></returns>
        public static string GetLastImgSrc(string htmlContent)
        {
            string[] str = GetHtmlImgList(htmlContent);
            if (str.Length > 0)
            {
                return str[str.Length - 1];
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取HTML中第一个图片的路径
        /// </summary>
        /// <param name="htmlContent"></param>
        /// <returns></returns>
        public static string GetFirstImgSrc(string[] htmlContent)
        {
            string src = string.Empty;
            foreach (string item in htmlContent)
            {
                string[] str = GetHtmlImgList(item);
                if (str.Length > 0)
                    src= str[0];
                if (src != string.Empty) { break; }
            }
            return src;
        }

        /// <summary>
        /// 获得某个字符串在另个字符串第一次出现时前面所有字符
        /// </summary>
        /// <param name="strOriginal">要处理的字符</param>
        /// <param name="strSymbol">符号</param>
        /// <returns>返回值</returns>
        public static string GetFirstStr(string strOriginal, string strSymbol)
        {
            int strPlace = strOriginal.IndexOf(strSymbol);
            if (strPlace != -1)
                strOriginal = strOriginal.Substring(0, strPlace);
            return strOriginal;
        }
        /// <summary>
        ///  获得两个字符之间最后一次出现时的所有字符
        /// </summary>
        /// <param name="strOriginal">要处理的字符</param>
        /// <param name="strFirst">最前哪个字符</param>
        /// <param name="strLast">最后哪个字符</param>
        /// <returns>返回值</returns>
        public static string GetTwoMiddleLastStr(string strOriginal, string strFirst, string strLast)
        {
            try
            {
                strOriginal = GetLastStr(strOriginal, strFirst);
                strOriginal = GetFirstStr(strOriginal, strLast);
                return strOriginal;
            }
            catch
            {
                return "";
            }
        }
        /// <summary>
        /// 获得某个字符串在另个字符串最后一次出现时后面所有字符
        /// </summary>
        /// <param name="strOriginal">要处理的字符</param>
        /// <param name="strSymbol">符号</param>
        /// <returns>返回值</returns>
        public static string GetLastStr(string strOriginal, string strSymbol)
        {
            try
            {
                int strPlace = strOriginal.LastIndexOf(strSymbol) + strSymbol.Length;
                strOriginal = strOriginal.Substring(strPlace);
                return strOriginal;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 获取本机IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetIp()
        {
            //可以透过代理服务器
            string userIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (userIP == null || userIP == "")
            {
                //没有代理服务器,如果有代理服务器获取的是代理服务器的IP
                userIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            return userIP;
        }

        /// <summary>
        /// 获取有超链接的文字信息
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="url">链接Url</param>
        /// <param name="target">跳转方式(默认为当前页面),注:_blank表示新窗口</param>
        /// <param name="titleLength">标题显示长度,如果为0则全部显示</param>
        /// <returns>返回带超链接的文字信息</returns>
        public static string GetOneLinkString(string title,string url,string target,int titleLength)
        {
            if (titleLength != 0)
                title = GetSubString(title, titleLength, false);

            string linkString = string.Format("<a href=\"{0}\" title=\"{1}\" target=\"{2}\">{1}</a>", url, title, target);
            if(target==string.Empty)
                linkString = string.Format("<a href=\"{0}\" title=\"{1}\">{1}</a>", url, title);
            return linkString;
        }

        /// <summary>
        /// 获取年差
        /// </summary>
        /// <param name="oldDateTime">旧时间</param>
        /// <returns></returns>
        public static int YearDiffer(DateTime oldDateTime)
        {
            DateTime currDt = DateTime.Now;
            int currYear = currDt.Year;
            int yearCount = 0;
            int days = (currDt - oldDateTime).Days;
            int years = days / 365;
            for (int i = 0; i < years; i++)
                if ((currYear - i) % 4 == 0)
                    days -= 1;
            yearCount = days / 365;
            return yearCount + 1;
        }
    }
}
