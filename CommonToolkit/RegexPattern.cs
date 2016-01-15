using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  CommonToolKit
{
    /// <summary>
    /// 正则表达式匹配模式
    /// </summary>
    public class RegexPattern
    {
        /// <summary>
        /// 整数（包括0）
        /// </summary>
        public static string integer = "^-?[0-9]\\d*$";
        /// <summary>
        /// 正整数（包括0）
        /// </summary>
        public static string integerPositive = "^[0-9]\\d*$";
        /// <summary>
        /// 负整数（不包括0）
        /// </summary>
        public static string integerNegative = "^-[0-9]\\d*$";
        /// <summary>
        /// 数字（包括0）
        /// </summary>
        public static string number = "^([+-]?)\\d*\\.?\\d+$";
        /// <summary>
        /// 正数（包括0）
        /// </summary>
        public static string numberPositive = "^[0-9]\\d*|0$";
        /// <summary>
        /// 负数（不包括0）
        /// </summary>
        public static string numberNegative = "^-[1-9]\\d*|0$";
        /// <summary>
        /// 浮点数或整数
        /// </summary>
        public static string decimalOrInteger = "^\\d+(\\.\\d+)?$";
        /// <summary>
        /// 浮点数
        /// </summary>
        public static string decmal = "^([+-]?)\\d*\\.\\d+$";
        /// <summary>
        /// 正浮点数
        /// </summary>
        public static string decmalPositive = "^[1-9]\\d*.\\d*|0.\\d*[1-9]\\d*|0?.0+|0$";
        /// <summary>
        /// 负浮点数
        /// </summary>
        public static string decmalNegative = "^(-([1-9]\\d*.\\d*|0.\\d*[1-9]\\d*))|0?.0+|0$";
        /// <summary>
        /// 非负浮点数（正浮点数 + 0，保留一位小数）
        /// </summary>
        public static string decmal6 = @"^(([0-9]+\.[0-9]{1})|([0-9]*[1-9][0-9]*\.[0-9]{1})|([0-9]*[0-9]{1}))$";
        /// <summary>
        /// 电子邮件
        /// </summary>
        public static string email = "^\\w+((-\\w+)|(\\.\\w+))*\\@[A-Za-z0-9]+((\\.|-)[A-Za-z0-9]+)*\\.[A-Za-z0-9]+$";
        /// <summary>
        /// 颜色
        /// </summary>
        public static string color = "^[a-fA-F0-9]{6}$";
        /// <summary>
        /// url
        /// </summary>
        public static string url = "^http[s]?:\\/\\/([\\w-]+\\.)+[\\w-]+([\\w-./?%&=]*)?$";
        /// <summary>
        /// 仅中文
        /// </summary>
        public static string chinese = "^[\\u4E00-\\u9FA5\\uF900-\\uFA2D]+$";
        /// <summary>
        /// 仅ACSII字符
        /// </summary>
        public static string ascii = "^[\\x00-\\xFF]+$";
        /// <summary>
        /// 邮编
        /// </summary>
        public static string postcode = "^\\d{6}$";
        /// <summary>
        /// 手机
        /// </summary>
        public static string mobile = "^(13|15|18|14)[0-9]{9}$";
        /// <summary>
        /// ip地址(IP4)
        /// </summary>
        public static string ip4 = "^(25[0-5]|2[0-4]\\d|[0-1]\\d{2}|[1-9]?\\d)\\.(25[0-5]|2[0-4]\\d|[0-1]\\d{2}|[1-9]?\\d)\\.(25[0-5]|2[0-4]\\d|[0-1]\\d{2}|[1-9]?\\d)\\.(25[0-5]|2[0-4]\\d|[0-1]\\d{2}|[1-9]?\\d)$";
        /// <summary>
        /// 非空
        /// </summary>
        public static string notempty = "^\\S+$";
        /// <summary>
        /// 图片
        /// </summary>
        public static string picture = "(.*)\\.(jpg|bmp|gif|ico|pcx|jpeg|tif|png|raw|tga)$";
        /// <summary>
        /// 压缩文件
        /// </summary>
        public static string rar = "(.*)\\.(rar|zip|7zip|tgz)$";
        /// <summary>
        /// 日期
        /// </summary>
        public static string date = @"^\d{4}(\\-|\\/|\.)\d{1,2}\\1\\d{1,2}$";
        /// <summary>
        /// QQ号码
        /// </summary>
        public static string qq = "^[1-9]*[1-9][0-9]*$";
        /// <summary>
        /// 电话号码的函数(包括验证国内区号,国际区号,分机号)
        /// </summary>
        public static string tel = "^(([0\\+]\\d{2,3}-)?(0\\d{2,3})-)?(\\d{7,8})(-(\\d{3,}))?$";
        /// <summary>
        /// 多个电话号码的函数(包括验证国内区号,国际区号,分机号),以/分隔
        /// </summary>
        public static string telMore = "^((([0\\+]\\d{2,3}-)?(0\\d{2,3})-)?(\\d{7,8})(-(\\d{3,}))?)(/(([0\\+]\\d{2,3}-)?(0\\d{2,3})-)?(\\d{7,8})(-(\\d{3,}))?)*$";
        /// <summary>
        /// 以字母开头，由数字、26个英文字母或下划线组成的6-20位字符
        /// </summary>
        public static string username = "^[a-zA-Z]{1}([a-zA-Z0-9]|[._]){5,19}$";
        /// <summary>
        /// 字母
        /// </summary>
        public static string letter = "^[A-Za-z]+$";
        /// <summary>
        /// 大写字母
        /// </summary>
        public static string letter_u = "^[A-Z]+$";
        /// <summary>
        /// 小写字母
        /// </summary>
        public static string letter_l = "^[a-z]+$";
        /// <summary>
        /// 密码(必须包含数字、字母、特殊字符且长度为8-12)
        /// </summary>
        public static string password = @"^(?!\D+$)(?![a-zA-Z0-9]+$)(?![^a-zA-Z0-9]+$)\S{8,12}$";
        /// <summary>
        /// 时间
        /// </summary>
        public static string time = "^([0-1]?[0-9]|2[0-3]):([0-5][0-9]):([0-5][0-9])$";
    }
}
