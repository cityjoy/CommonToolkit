
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace CommonToolKit
{
    /// <summary>
    /// 添加此特性的功能是：需要用户登录才能够浏览网页，如果不需要用户登录，则可以使用AllowAnonymousAttribute属性
    /// 使用：给ASP.NET MVC中的控制器类名或者Action方法上面打上[Login]标签
    /// 如果不需要验证用户是否登录就可以浏览，则给ASP.NET MVC中的控制器类名或者Action方法上面打上[AllowAnonymous]标签
    /// </summary>
    /// <auther>
    ///     <name>Kencery</name>
    ///     <date>2015-8-7</date>
    /// </auther>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class LoginAttribute : FilterAttribute, IAuthorizationFilter
    {
        /// <summary>
        /// 该值表示登录地址：默认路径为user/login,在Web.Config中设置
        /// </summary>
        private string _authUrl = string.Empty;

        public string AuthUrl
        {
            get
            {
                return _authUrl.Trim();
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("AuthUrl为空,验证用户登录信息的用户登录地址不能为空");
                }
                _authUrl = value.Trim();
            }
        }

        /// <summary>
        /// 该值表示yoghurt登录保存登录信息的键名，默认user,在web.Config中设置
        /// </summary>
        private string _authSaveKey = string.Empty;

        public string AuthSaveKey
        {
            get { return _authSaveKey.Trim(); }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("AuthSaveKey为空，验证用户登录的时候保存的登录键信息不能为空");
                }
                _authSaveKey = value.Trim();
            }
        }

        /// <summary>
        /// 该值表示用户登录保存登录信息的类型，默认Session,在web.Config中设置
        /// </summary>
        private string _authSaveType = string.Empty;

        public string AuthSaveType
        {
            get { return _authSaveType.Trim().ToUpper(); }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("AuthSaveType为空，验证用户登录的时候未保存键信息类型，类型不能为空");
                }
                _authSaveType = value.Trim().ToUpper();
            }
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public LoginAttribute()
        {
            const string authUrl = "/User/Index"; //登录页面路径，从Web.Config中读取
            const string saveKey = "user"; //验证(登录存放用户信息)，从Web.Config中读取
            const string saveType = "Session"; //存放类型判断(Session或者Cookie存放)，，从Web.Config中读取
            _authUrl = string.IsNullOrEmpty(authUrl) ? "/User/Index" : authUrl;

        }

        /// <summary>
        /// 构造函数重载
        /// </summary>
        /// <param name="authUrl"></param>
        public LoginAttribute(string authUrl) : this()
        {
            _authUrl = authUrl;
        }

        /// <summary>
        /// 构造函数重载
        /// </summary>
        /// <param name="authUrl"></param>
        /// <param name="saveKey"></param>
        public LoginAttribute(string authUrl, string saveKey) : this(authUrl)
        {
            _authSaveKey = saveKey;
            _authSaveType = "Session";
        }

        /// <summary>
        /// 构造函数重载
        /// </summary>
        /// <param name="authUrl"></param>
        /// <param name="saveKey"></param>
        /// <param name="saveType"></param>
        public LoginAttribute(string authUrl, string saveKey, string saveType) : this(authUrl, saveKey)
        {
            _authSaveType = saveType;
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext == null)
            {
                throw new Exception("此特性只试用于Web应用程序试用");
            }
            var flAd = filterContext.ActionDescriptor;
            var url = string.Format("{0}?Ref=/{1}/{2}", _authUrl, flAd.ControllerDescriptor.ControllerName,
                flAd.ActionName);
            switch (AuthSaveType)
            {
                case "SESSION":
                    if (filterContext.HttpContext.Session == null)
                    {
                        throw new Exception("服务器Session不可用");
                    }
                    if (!filterContext.ActionDescriptor.IsDefined(typeof (AllowAnonymousAttribute), true) &&
                        !filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(
                            typeof (AllowAnonymousAttribute), true))
                    {
                        if (filterContext.HttpContext.Session[_authSaveKey] == null)
                        {
                            filterContext.Result = new RedirectResult(url);
                        }
                    }
                    break;
                case "COOKIE":
                    if (!filterContext.ActionDescriptor.IsDefined(typeof (AllowAnonymousAttribute), true) &&
                        !filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(
                            typeof (AllowAnonymousAttribute), true))
                    {
                        if (filterContext.HttpContext.Request.Cookies[_authSaveKey] == null)
                        {
                            filterContext.Result = new RedirectResult(url);
                        }
                    }
                    break;
                default:
                    throw new ArgumentNullException("用户保存登录信息的方法不能为空，只能为Cookie和Session，请您检查");
            }
        }
    }
}