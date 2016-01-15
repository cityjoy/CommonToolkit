using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace ZP.Project.Toolkit
{
    public static class Expand
    {
        #region 字符串处理
        /// <summary>
        /// 中英文混合字符串截取
        /// </summary>
        /// <param name="p_SrcString">要截取的字符串</param>
        /// <param name="p_Length">截取长度</param>
        /// <returns></returns>
        public static string Substr(this string p_SrcString, int p_Length)
        {
            return Common.GetSubString(p_SrcString, p_Length);
        }

        public static string ToString<T>(this T[] list, string splitStr)
        {
            splitStr = splitStr ?? string.Empty;
            string str = string.Empty;
            if (list != null && list.Length > 0)
            {
                foreach (object item in list)
                {
                    str += str != string.Empty ? splitStr : string.Empty;
                    str += item.ToString();
                }
            }
            return str;
        }
        #endregion

        #region 保存或获取页面暂存数据
        /// <summary>
        /// 保存页面暂存数据
        /// </summary>
        /// <param name="phoneApplicationPage">手机应用程序页</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void SaveState(this PhoneApplicationPage phoneApplicationPage, string key, object value)
        {
            if (phoneApplicationPage.State.ContainsKey(key))
            {
                phoneApplicationPage.State.Remove(key);
            }
            phoneApplicationPage.State.Add(key, value);
        }

        /// <summary>
        /// 获取页面暂存数据
        /// </summary>
        /// <typeparam name="T">类</typeparam>
        /// <param name="phoneApplicationPage">手机应用程序页</param>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        public static T LoadState<T>(this PhoneApplicationPage phoneApplicationPage, string key)
            where T : class
        {
            if (phoneApplicationPage.State.ContainsKey(key))
            {
                return (T)phoneApplicationPage.State[key];
            }
            return default(T);
        }
        #endregion
    }
}
