using System;
using System.Configuration;
using Memcached.ClientLibrary;

namespace CommonToolKit
{
    /// <summary>
    /// 封装使用Memchached信息，读取缓存存放在服务器
    /// </summary>
    public class MemcacheHelper
    {
        /// <summary>
        /// 字段_instance,存放注册的缓存信息
        /// </summary>
        private static MemcacheHelper _instance;

        /// <summary>
        /// 缓存客户端
        /// </summary>
        private readonly MemcachedClient _client;

        /// <summary>
        /// 受保护类型的缓存对象，初始化一个新的缓存对象
        /// </summary>
        protected MemcacheHelper()
        {
            //读取app.Config中需要缓存的服务器地址信息，可以传递多个地址，使用","分隔
            string[] serverList = ConfigurationManager.AppSettings["Memcached.ServerList"].Split(',');
            try
            {
                var sockIoPool = SockIOPool.GetInstance();
                sockIoPool.SetServers(serverList);
                sockIoPool.InitConnections = 3;
                sockIoPool.MinConnections = 3;
                sockIoPool.MaxConnections = 50;
                sockIoPool.SocketConnectTimeout = 1000;
                sockIoPool.SocketTimeout = 3000;
                sockIoPool.MaintenanceSleep = 30;
                sockIoPool.Failover = true;
                sockIoPool.Nagle = false;
                //实例化缓存对象
                _client = new MemcachedClient();
            }
            catch (Exception ex)
            {
                //错误信息写入事务日志
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 获取缓存的实例对象，方法调用的时候使用
        /// </summary>
        /// <returns></returns>
        public static MemcacheHelper GetInstance()
        {
            return _instance;
        }

        /// <summary>
        /// 添加缓存信息(如果存在缓存信息则直接重写设置，否则添加)
        /// 使用：MemcacheHelper.GetInstance().Add(key,value)
        /// </summary>
        /// <param name="key">需要缓存的键</param>
        /// <param name="value">需要缓存的值</param>
        public void Add(string key, object value)
        {
            if (_client.KeyExists(key))
            {
                _client.Set(key, value);
            }
            _client.Add(key, value);
        }

        /// <summary>
        /// 添加缓存信息
        /// 使用：MemcacheHelper.GetInstance().Add(key,value,Datetime.Now())
        /// </summary>
        /// <param name="key">需要缓存的键</param>
        /// <param name="value">需要缓存的值</param>
        /// <param name="expiredDateTime">设置的缓存的过时时间</param>
        public void Add(string key, object value, DateTime expiredDateTime)
        {
            _client.Add(key, value, expiredDateTime);
        }

        /// <summary>
        /// 修改缓存的值
        /// 使用：MemcacheHelper.GetInstance().Update(key,value)
        /// </summary>
        /// <param name="key">需要修改的键</param>
        /// <param name="value">需要修改的值</param>
        public void Update(string key, object value)
        {
            _client.Replace(key, value);
        }

        /// <summary>
        /// 修改缓存的值
        /// 使用：MemcacheHelper.GetInstance().Update(key,value,Datetime.Now())
        /// </summary>
        /// <param name="key">需要修改的键</param>
        /// <param name="value">需要修改的值</param>
        /// <param name="expiredDateTime">设置的缓存的过时时间</param>
        public void Update(string key, object value, DateTime expiredDateTime)
        {
            _client.Replace(key, value, expiredDateTime);
        }

        /// <summary>
        /// 设置缓存
        /// 使用：MemcacheHelper.GetInstance().Set(key,value)
        /// </summary>
        /// <param name="key">设置缓存的键</param>
        /// <param name="value">设置缓存的值</param>
        public void Set(string key, object value)
        {
            _client.Set(key, value);
        }

        /// <summary>
        /// 设置缓存，并修改过期时间
        /// 使用：MemcacheHelper.GetInstance().Set(key,value,Datetime.Now())
        /// </summary>
        /// <param name="key">设置缓存的键</param>
        /// <param name="value">设置缓存的值</param>
        /// <param name="expiredTime">设置缓存过期的时间</param>
        public void Set(string key, object value, DateTime expiredTime)
        {
            _client.Set(key, value, expiredTime);
        }

        /// <summary>
        /// 删除缓存
        /// 使用：MemcacheHelper.GetInstance().Delete(key)
        /// </summary>
        /// <param name="key">需要删除的缓存的键</param>
        public void Delete(string key)
        {
            _client.Delete(key);
        }

        /// <summary>
        /// 获取缓存的值
        /// 使用：MemcacheHelper.GetInstance().Get(key)
        /// </summary>
        /// <param name="key">传递缓存中的键</param>
        /// <returns>返回缓存在缓存中的信息</returns>
        public object Get(string key)
        {
            return _client.Get(key);
        }

        /// <summary>
        /// 缓存是否存在
        /// 使用：MemcacheHelper.GetInstance().KeyExists(key)
        /// </summary>
        /// <param name="key">传递缓存中的键</param>
        /// <returns>如果为true，则表示存在此缓存，否则比表示不存在</returns>
        public bool KeyExists(string key)
        {
            return _client.KeyExists(key);
        }

        /// <summary>
        /// 注册Memcache缓存(在Global.asax的Application_Start方法中注册)
        /// 使用：MemcacheHelper.RegisterMemcache();
        /// </summary>
        public static void RegisterMemcache()
        {
            if (_instance == null)
            {
                _instance = new MemcacheHelper();
            }
        }
    }
}