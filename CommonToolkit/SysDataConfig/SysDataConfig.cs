using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace  CommonToolKit
{
    /// <summary>
    /// 系统数据配置
    /// </summary>
    public class SysDataConfig
    {
        #region 获取系统数据  核心代码
        private readonly static string _sysDataConfigKey = "SysDataConfigKEY";
        /// <summary>
        /// 获取系统数据
        /// </summary>
        /// <param name="key"></param>
        /// <remarks>需在网站根目录的App_Data存放指定格式的SysDataConfig.xml文件</remarks>
        /// <returns></returns>
        private static T GetSysData<T>(string key)
        {
            try
            {
                object obj = HttpRuntimeCache.Get(_sysDataConfigKey);
                XElement xe = null;
                if (obj == null)
                {
                    xe = XElement.Load(UrlHelper.MapPath("~/App_Data/SysDataConfig.xml"));
                    //缓存一天
                    HttpRuntimeCache.SetCache(_sysDataConfigKey, xe, DateTime.Now.AddDays(1), System.Web.Caching.Cache.NoSlidingExpiration);
                }
                else
                {
                    try { xe = (XElement)obj; }
                    catch { }
                }
                XElement elNodeIndexName = (from nodeIndexName in xe.Elements("item")
                                            where nodeIndexName.Attribute("key").Value == key
                                            select nodeIndexName).ElementAt(0);

                string val = elNodeIndexName.Attribute("value").Value;
                return Common.ConvertValue<T>(val);
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("系统数据配置出现错误，可能原因：在网站根目录App_Data文件夹下找不到SysDataConfig.xml文件,或文件内容格式错误。详细信息如下：{0}", e.Message), e);
            }
        }
        #endregion

        /// <summary>
        /// Node表 广告主ID
        /// </summary>
        public static int Node_AdMainId { get { return GetSysData<int>("Node_AdMainId"); } }
        /// <summary>
        /// Node表 普通内容主ID
        /// </summary>
        public static int Node_ContentMainId { get { return GetSysData<int>("Node_ContentMainId"); } }
        /// <summary>
        /// Node表 产品主ID
        /// </summary>
        public static int Node_ProductMainId { get { return GetSysData<int>("Node_ProductMainId"); } }
        /// <summary>
        /// Node表 奖品主ID
        /// </summary>
        public static int Node_PrizeMainId { get { return GetSysData<int>("Node_PrizeMainId"); } }
        /// <summary>
        /// Node表 栏目资源主ID
        /// </summary>
        public static int Node_NodeResourcesMainId { get { return GetSysData<int>("Node_NodeResourcesMainId"); } }
        /// <summary>
        /// Node表 友情链接主ID
        /// </summary>
        public static int Node_LinkMainId { get { return GetSysData<int>("Node_LinkMainId"); } }
        /// <summary>
        /// Node表 普通内容->公司介绍主ID
        /// </summary>
        public static int Node_CompanyMainId { get { return GetSysData<int>("Node_CompanyMainId"); } }
        /// <summary>
        /// Node表 主导航主ID
        /// </summary>
        public static int Node_MainNavMainId { get { return GetSysData<int>("Node_MainNavMainId"); } }
    }
}
