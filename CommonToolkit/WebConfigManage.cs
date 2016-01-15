using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace CommonToolKit
{
    /// <summary>
    /// 配置信息
    /// </summary>
    public class WebConfigManage
    {
        public static string GetAppSetting(string key)
        {
            var objModel = ConfigurationManager.AppSettings[key];
            return objModel.ToString();
        }

        public static int GetByAppSettingsKeyToInt(string key)
        {
            string v = GetAppSetting(key);
            int keyValue = 0;
            int.TryParse(v, out keyValue);
            return keyValue;
        }
    }
}
