using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace  CommonToolKit
{
    public class SQLFilter
    {
        /// <summary>
        /// 过滤标记
        /// </summary>
        /// <param name="NoHTML">包括HTML，脚本，数据库关键字，特殊字符的源码 </param>
        /// <returns>已经去除标记后的文字</returns>
        public static string FilterSQL(string filterStr)
        {
            if (filterStr == null)
            {
                return string.Empty;
            }
            else
            {
                filterStr = filterStr.Replace(",", "，");
                filterStr = filterStr.Replace("'", "＇");
                filterStr = filterStr.Replace("\"", "＂");
                filterStr = filterStr.Replace("<", "＜");
                filterStr = filterStr.Replace(">", "＞");
                filterStr = filterStr.Replace("|", "｜");
                filterStr = filterStr.Replace(";", "；");
                filterStr = filterStr.Replace("--", "——").Trim();
                filterStr = filterStr.Replace("<script", "").Trim();
                filterStr = filterStr.Replace("</script>", "").Trim();
                filterStr = Regex.Replace(filterStr, @"(^\s*)|(\s*$)", "", RegexOptions.IgnoreCase);
                return filterStr;
            }
        }

        /// <summary>
        /// 过滤SQL查询(逗号分隔参数)，非数字类型字符，默认给每项自动添加单引号
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str">待处理的字符</param>
        /// <param name="isAddQuotationMarks">是否给每项添加引号</param>
        /// <returns></returns>
        public static string FilterSearchCommaStr<T>(string str, bool isAddQuotationMarks = true)
        {
            string returnStr = string.Empty;
            string[] strs = str.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (typeof(T).Name.ToLower() == "int32")
            {
                foreach (string item in strs)
                {
                    if (Regex.IsMatch(item, "^[0-9]\\d*$"))
                    {
                        returnStr += returnStr != string.Empty ? "," + item : item;
                    }
                }
                return returnStr;
            }
            else if (typeof(T).Name.ToLower() == "string")
            {
                foreach (string item in strs)
                {
                    if (isAddQuotationMarks)
                    {
                        returnStr += returnStr != string.Empty ? "," + string.Format("'{0}'", FilterSQL(item)) : string.Format("'{0}'", FilterSQL(item));
                    }
                    else
                    {
                        returnStr += returnStr != string.Empty ? "," + FilterSQL(item) : FilterSQL(item);
                    }
                }
                return returnStr;
            }
            else if (typeof(T).Name.ToLower() == "guid")
            {
                foreach (string item in strs)
                {
                    try
                    {
                        Guid rGuid = new Guid(item.Trim());
                        if (isAddQuotationMarks)
                        {
                            returnStr += returnStr != string.Empty ? "," + string.Format("'{0}'", FilterSQL(item)) : string.Format("'{0}'", FilterSQL(item));
                        }
                        else
                        {
                            returnStr += returnStr != string.Empty ? "," + FilterSQL(item) : FilterSQL(item);
                        }
                    }
                    catch { }
                }
                return returnStr;
            }
            return returnStr;
        }
    }
}
