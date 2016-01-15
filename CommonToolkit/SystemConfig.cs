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
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

namespace ZP.Project.Toolkit
{
    /// <summary>
    /// 应用程序配置文件（使用此类需在WP7项目根目录新建config.xml）
    /// 
    /// 属性：
    /// 复制到输出目录：不复制
    /// 生成操作：Resources
    /// 
    /// 格式：
    /// <?xml version="1.0" encoding="utf-8" ?>
    /// <configuration>
    ///   <appSettings>
    ///     <add key="key" value="value"/>
    ///   </appSettings>
    /// </configuration>
    /// </summary>
    public class SystemConfig
    {
        #region 静态变量
        private static SystemConfig _instance = null;
        private static XDocument document = null;
        private static IEnumerable<XElement> appSettings = null;
        #endregion

        #region AppSettings  Key常量
        /// <summary>
        /// 通知服务基础Url
        /// </summary>
        private const string KEY_NOTIFICATIONSBASEURL = "NotificationsBaseUrl";
        /// <summary>
        /// 通知服务注册Url
        /// </summary>
        private const string KEY_NOTIFICATIONSREGISTERURL = "NotificationsRegisterUrl";
        /// <summary>
        /// Bing地图Key
        /// </summary>
        private const string KEY_BINGMAPSKEY = "BingMapsKey";
        #endregion

        #region SystemConfig实例对象
        /// <summary>
        /// SystemConfig实例对象
        /// </summary>
        public static SystemConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SystemConfig();
                    document = XDocument.Load(new Uri("/View;component/config.xml", UriKind.Relative).ToString());
                    appSettings = document.Descendants("configuration").Elements("appSettings");
                }
                return _instance;
            }
        }
        #endregion

        #region 私有辅助方法
        /// <summary>
        /// 根据Key获取Value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetValueByKey(string key)
        {
            XElement setting = appSettings.Elements().SingleOrDefault(s => s.Attribute("key").Value == key);
            if (setting != null && setting.Attribute("value") != null)
            {
                return setting.Attribute("value").Value;
            }
            return null;
        }
        #endregion

        /// <summary>
        /// 通知服务基础Url
        /// </summary>
        public string NotificationsBaseUrl { get { return this.GetValueByKey(KEY_NOTIFICATIONSBASEURL); } }

        /// <summary>
        /// 通知服务注册Url
        /// </summary>
        public string NotificationsRegisterUrl { get { return this.GetValueByKey(KEY_NOTIFICATIONSREGISTERURL); } }

        /// <summary>
        /// Bing地图Key
        /// </summary>
        public string BingMapsKey { get { return this.GetValueByKey(KEY_BINGMAPSKEY); } }
        
    }
}
