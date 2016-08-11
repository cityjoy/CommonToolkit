using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;

namespace WebAPITest
{
    /// <summary>
    /// MemberHelper 的摘要说明
    /// </summary>
    public abstract class MembercacheHelper
    {
        public MembercacheHelper()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        #region 添加缓存
        /// <summary>
        /// 添加缓存(键不存在则添加，存在则替换)
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool AddCache(string key, object value)
        {
            using (MemcachedClient mc = new MemcachedClient())
            {
                return mc.Store(StoreMode.Set, key, value);
            }
        }
        #endregion

        #region 添加缓存
        /// <summary>
        /// 添加缓存(键不存在则添加，存在则替换)
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="minutes">缓存时间(分钟)</param>
        /// <returns></returns>
        public static bool AddCache(string key, object value, int minutes)
        {
            using (MemcachedClient mc = new MemcachedClient())
            {
                return mc.Store(StoreMode.Set, key, value, DateTime.Now.AddMinutes(minutes));
            }
        }
        #endregion

        #region 获取缓存
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>返回缓存，没有找到则返回null</returns>
        public static object GetCache(string key)
        {
            using (MemcachedClient mc = new MemcachedClient())
            {
                return mc.Get(key);
            }
        }
        #endregion

        #region 是否存在该缓存
        /// <summary>
        /// 是否存在该缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static bool IsExists(string key)
        {
            using (MemcachedClient mc = new MemcachedClient())
            {
                return mc.Get(key) != null;
            }
        }
        #endregion

        #region 删除缓存(如果键不存在，则返回false)
        /// <summary>
        /// 删除缓存(如果键不存在，则返回false)
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>成功:true失败:false</returns>
        public static bool DelCache(string key)
        {
            using (MemcachedClient mc = new MemcachedClient())
            {
                return mc.Remove(key);
            }
        }
        #endregion

        #region 清空缓存
        /// <summary>
        /// 清空缓存
        /// </summary>
        public static void FlushCache()
        {
            using (MemcachedClient mc = new MemcachedClient())
            {
                mc.FlushAll();
            }
        }
        #endregion
    }
}