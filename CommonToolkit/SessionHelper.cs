using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace  CommonToolKit
{
    public class SessionHelper
    {
        #region 后台用户信息
        /// <summary>
        /// 写后台用户信息 Session值
        /// </summary>
        /// <param name="userId">后台用户Id</param>
        /// <param name="loginId">后台用户名</param>
        /// <param name="name">姓名</param>
        /// <param name="usertype">后台用户类别</param>
        public static void WriteManagerUsersSession(int userId, string loginId, string name, int usertype)
        {
            Dictionary<string, object> dicUserInfo = new Dictionary<string, object>();
            dicUserInfo.Add("UserId", userId);
            dicUserInfo.Add("loginId", loginId);
            dicUserInfo.Add("Name", name);
            dicUserInfo.Add("UserType", usertype);
            HttpContext.Current.Session["ManagerUsers"] = dicUserInfo;
        }
        /// <summary>
        /// 删除后台用户信息 Session值
        /// </summary>
        public static void DelManagerUsersSession()
        {
            HttpContext.Current.Session.Remove("ManagerUsers");
        }

        /// <summary>
        /// 注册后台用户Session容器
        /// </summary>
        public static object ManagerUserSession
        {
            get
            {
                return HttpContext.Current.Session["ManagerUsers"];
            }
        }

        /// <summary>
        /// 后台用户是否已登录
        /// </summary>
        /// <returns></returns>
        public static bool IsManagerUserLogin
        {
            get
            {
                if (ManagerUserSession == null || ManagerUserId == 0)
                    return false;
                return true;
            }
        }

        /// <summary>
        /// 获取后台用户Id
        /// </summary>
        public static int ManagerUserId
        {
            get
            {
                if (ManagerUserSession != null)
                {
                    Dictionary<string, object> dicUserInfo = HttpContext.Current.Session["ManagerUsers"] as Dictionary<string, object>;
                    return (int)dicUserInfo["UserId"];
                }
                else { return 0; }
            }
        }

        /// <summary>
        /// 获取后台用户登录名
        /// </summary>
        public static string ManagerUserLoginId
        {
            get
            {
                if (ManagerUserSession != null)
                {
                    Dictionary<string, object> dicUserInfo = HttpContext.Current.Session["ManagerUsers"] as Dictionary<string, object>;
                    return dicUserInfo["loginId"].ToString();
                }
                else { return string.Empty; }
            }
        }
        /// <summary>
        /// 获取后台用户姓名
        /// </summary>
        public static string ManagerUserName
        {
            get
            {
                if (ManagerUserSession != null)
                {
                    Dictionary<string, object> dicUserInfo = HttpContext.Current.Session["ManagerUsers"] as Dictionary<string, object>;
                    return dicUserInfo["loginId"].ToString();
                }
                else { return string.Empty; }
            }
        }

        /// <summary>
        /// 后台用户类型(LoginType)
        /// </summary>
        public static int ManagerUserType
        {
            get
            {
                if (ManagerUserSession != null)
                {
                    Dictionary<string, object> dicUserInfo = HttpContext.Current.Session["ManagerUsers"] as Dictionary<string, object>;
                    return (int)dicUserInfo["UserType"];
                }
                else { return 0; }
            }
        }
        #endregion

        #region 前台用户信息
        /// <summary>
        /// 写前台用户信息 Session值
        /// </summary>
        /// <param name="userId">前台用户Id</param>
        /// <param name="loginId">前台用户名</param>
        /// <param name="name">姓名</param>
        /// <param name="usertype">前台用户类别</param>
        public static void WriteUsersSession(int userId, string loginId, string name, int usertype)
        {
            Dictionary<string, object> dicUserInfo = new Dictionary<string, object>();
            dicUserInfo.Add("UserId", userId);
            dicUserInfo.Add("loginId", loginId);
            dicUserInfo.Add("Name", name);
            dicUserInfo.Add("UserType", usertype);
            HttpContext.Current.Session["Users"] = dicUserInfo;
        }
        /// <summary>
        /// 删除前台用户信息 Session值
        /// </summary>
        public static void DelUsersSession()
        {
            HttpContext.Current.Session.Remove("Users");
        }

        /// <summary>
        /// 注册前台用户Session容器
        /// </summary>
        public static object UserSession
        {
            get
            {
                return HttpContext.Current.Session["Users"];
            }
        }

        /// <summary>
        /// 前台用户是否已登录
        /// </summary>
        /// <returns></returns>
        public static bool IsUserLogin
        {
            get
            {
                if (UserSession == null || UserId == 0)
                    return false;
                return true;
            }
        }

        /// <summary>
        /// 获取前台用户Id
        /// </summary>
        public static int UserId
        {
            get
            {
                if (UserSession != null)
                {
                    Dictionary<string, object> dicUserInfo = HttpContext.Current.Session["Users"] as Dictionary<string, object>;
                    return (int)dicUserInfo["UserId"];
                }
                else { return 0; }
            }
        }

        /// <summary>
        /// 获取前台用户登录名
        /// </summary>
        public static string UserLoginId
        {
            get
            {
                if (UserSession != null)
                {
                    Dictionary<string, object> dicUserInfo = HttpContext.Current.Session["Users"] as Dictionary<string, object>;
                    return dicUserInfo["loginId"].ToString();
                }
                else { return string.Empty; }
            }
        }
        /// <summary>
        /// 获取前台用户姓名
        /// </summary>
        public static string UserName
        {
            get
            {
                if (UserSession != null)
                {
                    Dictionary<string, object> dicUserInfo = HttpContext.Current.Session["Users"] as Dictionary<string, object>;
                    return dicUserInfo["loginId"].ToString();
                }
                else { return string.Empty; }
            }
        }

        /// <summary>
        /// 前台用户类型(LoginType)
        /// </summary>
        public static int UserType
        {
            get
            {
                if (UserSession != null)
                {
                    Dictionary<string, object> dicUserInfo = HttpContext.Current.Session["Users"] as Dictionary<string, object>;
                    return (int)dicUserInfo["UserType"];
                }
                else { return 0; }
            }
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 获取Session值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">值</param>
        /// <returns></returns>
        public static T GetSession<T>(string key)
        {
            string val = HttpContext.Current.Session[key] != null ? HttpContext.Current.Session[key].ToString() : string.Empty;
            return Common.ConvertValue<T>(val);
        }

        /// <summary>
        /// 设置Session值
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">值</param>
        public static void SetSession(string key, object value)
        {
            HttpContext.Current.Session[key] = value;
        }
        #endregion
    }
}
