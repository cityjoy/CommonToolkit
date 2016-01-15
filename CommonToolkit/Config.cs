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
    /// 读取应用程序配置信息
    /// 注意：只能在表现层，即存在config.xml文件的类库中使用此类
    /// 警告：在不存在config.xml文件的类库中使用此类时，会出现异常
    /// </summary>
    public class Config
    {
        #region 静态变量
        private static Config _instance = null;
        private static XDocument document = null;
        private static IEnumerable<XElement> appSettings = null;
        #endregion

        #region SystemConfig实例对象
        /// <summary>
        /// SystemConfig实例对象
        /// </summary>
        public static Config Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Config();
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
        /// 数据库名称
        /// </summary>
        public string DatabaseName { get { return this.GetValueByKey("DatabaseName"); } }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionString { get { return this.GetValueByKey("ConnectionString"); } }
    }
}
