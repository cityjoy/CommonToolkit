using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace  CommonToolKit
{
    public class JsonNetHelper
    {
        #region 共用静态方法  Json.NET进行转换Json对象 

        /// <summary>
        /// 将实体对象转换为Json对象，生成字符串
        /// </summary>
        /// <param name="item">实体对象</param>
        /// <returns></returns>
        public static string JsonNetSerializeToStringInfo(object item)
        {
            try
            {
                return JsonConvert.SerializeObject(item);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 将Json串对象解析成固定的对象信息
        /// </summary>
        /// <typeparam name="T">需要得到解析后对象的实体类型</typeparam>
        /// <param name="jsonStr">json串</param>
        /// <returns></returns>
        public static T JsonNetDeserializeToEntityInfo<T>(string jsonStr)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(jsonStr);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region-----共用静态方法  JavaScript进行转换Json对象-----

        /// <summary>
        /// 将需要转换成json串的对象转换为Json字符串新消息
        /// </summary>
        /// <param name="item">需要实例化的对象</param>
        /// <returns></returns>
        public static string JavaScriptSerializeToStringInfo(object item)
        {
            try
            {
                var javaScriptSerializer = new JavaScriptSerializer();
                return javaScriptSerializer.Serialize(item);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 将指定的Json字符串转换为T类型的对象，JavaScript序列化
        /// </summary>
        /// <typeparam name="T">需要得到解析后的对象的实体类型</typeparam>
        /// <param name="jsonStr">进行反序列化的Json字符串</param>
        /// <returns></returns>
        public static T JavaScriptSerializeToEntityInfo<T>(string jsonStr)
        {
            try
            {
                var javaScriptSerializer = new JavaScriptSerializer();
                return javaScriptSerializer.Deserialize<T>(jsonStr);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion
      
        /// <summary>
        /// 将JSON时间类型转换为DateTime
        /// </summary>
        /// <param name="jsonDate"></param>
        /// <returns></returns>
        public static DateTime JsonToDateTime(string jsonDate)
        {
            string value = jsonDate.Substring(6, jsonDate.Length - 8);
            DateTimeKind kind = DateTimeKind.Utc;
            int index = value.IndexOf('+', 1);
            if (index == -1)
                index = value.IndexOf('-', 1);
            if (index != -1)
            {
                kind = DateTimeKind.Local;
                value = value.Substring(0, index);
            }
            long javaScriptTicks = long.Parse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
            long InitialJavaScriptDateTicks = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks;
            DateTime utcDateTime = new DateTime((javaScriptTicks * 10000) + InitialJavaScriptDateTicks, DateTimeKind.Utc);
            DateTime dateTime;
            switch (kind)
            {
                case DateTimeKind.Unspecified:
                    dateTime = DateTime.SpecifyKind(utcDateTime.ToLocalTime(), DateTimeKind.Unspecified);
                    break;
                case DateTimeKind.Local:
                    dateTime = utcDateTime.ToLocalTime();
                    break;
                default:
                    dateTime = utcDateTime;
                    break;
            }
            return dateTime;
        }
    }
}
