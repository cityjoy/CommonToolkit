using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;

namespace  CommonToolKit
{
    public enum RegexMode
    {
        /// <summary>
        /// 电子邮件
        /// </summary>
        Email,
        /// <summary>
        /// 年龄
        /// </summary>
        Age,
        /// <summary>
        /// 整数
        /// </summary>
        Integer,
        /// <summary>
        /// 正整数
        /// </summary>
        PositiveInteger,
        /// <summary>
        /// 浮点数
        /// </summary>
        FloatingPointNumbers,
        /// <summary>
        /// IP地址
        /// </summary>
        IP,
        /// <summary>
        /// 中文
        /// </summary>
        Chinese,
        /// <summary>
        /// 网址 
        /// </summary>
        URL,
        /// <summary>
        /// 邮编 
        /// </summary>
        PostCode


    }
    public class DataValidateHelper
    {
        /// <summary>
        /// 验证是否为null或string.Empty
        /// </summary>
        /// <param name="value"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static void IsNullOrEmpty(string value, string title, ref bool isContinue, ref string errorMsg)
        {
            if (isContinue)
            {
                if (value == null || value.Trim() == string.Empty)
                {
                    errorMsg = string.Format("{0}不可为空!", title);
                    isContinue = false;
                }
            }
            errorMsg = isContinue ? string.Empty : errorMsg;
        }
        /// <summary>
        /// 验证是否为null或string.Empty
        /// </summary>
        /// <param name="value"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static void IsNullOrEmpty(TextBox textBox, string title, ref bool isContinue, ref string errorMsg)
        {
            string value = textBox.Text.Trim();
            if (isContinue)
            {
                if (value == null || value.Trim() == string.Empty)
                {
                    errorMsg = string.Format("{0}不可为空!", title);
                    isContinue = false;
                    textBox.Focus();
                }
            }
            errorMsg = isContinue ? string.Empty : errorMsg;
        }
        /// <summary>
        /// 正则验证(注意:第一句验证,isContinue需设置为true)
        /// </summary>
        /// <param name="value">要验证的值</param>
        /// <param name="title">错误消息标题</param>
        /// <param name="validateEmpty">数据为空字符时是否验证</param>
        /// <param name="regexMode">验证模式</param>
        /// <param name="isContinue">是否继续验证</param>
        /// <param name="errorMsg">验证失败显示的消息</param>
        /// <returns></returns>
        public static void RegexValidate(string value, string title,bool validateEmpty, RegexMode regexMode, ref bool isContinue, ref string errorMsg)
        {
            if (isContinue && validateEmpty)
            {
                isContinue = Regex.IsMatch(value, RegexPrecept(regexMode, title, ref errorMsg));
            }
            errorMsg = isContinue ? string.Empty : errorMsg;
        }

        /// <summary>
        /// 正则验证(注意:第一句验证,isContinue需设置为true)
        /// </summary>
        /// <param name="value">要验证的值</param>
        /// <param name="title">错误消息标题</param>
        /// <param name="validateEmpty">数据为空字符时是否验证</param>
        /// <param name="regexMode">验证模式</param>
        /// <param name="isContinue">是否继续验证</param>
        /// <param name="errorMsg">验证失败显示的消息</param>
        /// <returns></returns>
        public static void RegexValidate(TextBox textBox, string title, bool validateEmpty, RegexMode regexMode, ref bool isContinue, ref string errorMsg)
        {
            if (isContinue && validateEmpty)
            {
                isContinue = Regex.IsMatch(textBox.Text.Trim(), RegexPrecept(regexMode, title, ref errorMsg));
                if (!isContinue) { textBox.Focus(); }
            }
            errorMsg = isContinue ? string.Empty : errorMsg;
        }

        /// <summary>
        /// 验证规则
        /// </summary>
        /// <param name="regexMode">验证模式</param>
        /// <param name="msg">错误消息</param>
        /// <returns></returns>
        private static string RegexPrecept(RegexMode regexMode, string title, ref string errorMsg)
        {
            string regValue = string.Empty;
            errorMsg = string.Empty;
            switch (regexMode)
            {
                case RegexMode.Age:
                    regValue = @"^([0-9]{1,2}|1[0-4][0-9]|150)$";
                    errorMsg = title + "格式错误,必须介于0-150之间!";
                    break;
                case RegexMode.Chinese:
                    //[\u4e00-\u9fa5]   //表示中文字符
                    //[^\u4e00-\u9fa5]  //表示不可存在中文
                    //区别在于存在字符^
                    regValue = @"[^\u4e00-\u9fa5]";
                    errorMsg = title + "不可使用中文字符!";
                    break;
                case RegexMode.Email:
                    regValue = @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
                    errorMsg = title + "格式错误,如:web@163.com !";
                    break;
                case RegexMode.FloatingPointNumbers:
                    regValue = @"^[1-9]\d*\.\d*|0\.\d*[1-9]\d*$";
                    errorMsg = title + "必须输入浮点数,如8.88!";
                    break;
                case RegexMode.Integer:
                    regValue = @"^-?\d+$";
                    errorMsg = title + "必须输入整数!";
                    break;
                case RegexMode.IP:
                    regValue = @"\d+\.\d+\.\d+\.\d+";
                    errorMsg = title + "格式错误!";
                    break;
                case RegexMode.PositiveInteger:
                    regValue = @"^\d+$";
                    errorMsg = title + "必须输入正整数!";
                    break;
                case RegexMode.PostCode:
                    regValue = @"\d{6}";
                    errorMsg = title + "格式错误,如:443400!";
                    break;
                case RegexMode.URL:
                    regValue = @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";
                    errorMsg = title + "格式错误,如:http://www.baidu.com !";
                    break;
            }
            return regValue;
        }
    }
}
