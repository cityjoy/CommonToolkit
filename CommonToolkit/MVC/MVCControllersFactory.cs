
using System;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;

namespace BookSystem_Controllers
{
    /// <summary>
    /// 重写注册控制器的方法，使之能够将控制器管分离到其它类库中实现
    /// </summary>
    /// 说明：IControllerFactory接口含有三个需要实现的方法：CreateController，GetControllerSessionBehavior，ReleaseController
    /// 使用：在MVC App_Start文件夹中的RouteConfig中的方法RegisterRoutes中的第一行中写入下面注册语句,Global.asax中也可注册，放到注册路由之前即可
    /// ControllerBuilder.Current.SetControllerFactory(new ControllersFactory("BookSystem_Controllers"));  //BookSystem_Controllers为控制器的类库
    public class ControllersFactory : IControllerFactory
    {
        private readonly string _assemblyName;
        private readonly string _controlerDefaultNameSpage;
        private Assembly _controllerAssembly;

        /// <summary>
        /// 获取控制器所在的程序集名称
        /// </summary>
        public string AssemblyName
        {
            get { return _assemblyName; }
        }

        /// <summary>
        /// 获取控制器的默认命名空间
        /// </summary>
        public string ControlerDefaultNameSpage
        {
            get { return _controlerDefaultNameSpage; }
        }

        /// <summary>
        /// 获取控制器所在的程序集的Assembly实例
        /// </summary>
        public Assembly ControllerAssembly
        {
            get
            {
                return _controllerAssembly ?? (_controllerAssembly = Assembly.Load(AssemblyName)); //加载控制器信息
            }
        }

        /// <summary>
        /// 构造函数实例化
        /// </summary>
        /// <param name="assemblyName"></param>
        public ControllersFactory(string assemblyName)
        {
            _assemblyName = assemblyName;
        }

        /// <summary>
        /// 构造函数实例化
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="controlerDefaultNameSpage"></param>
        public ControllersFactory(string assemblyName, string controlerDefaultNameSpage)
        {
            _assemblyName = assemblyName;
            _controlerDefaultNameSpage = controlerDefaultNameSpage;
        }

        /// <summary>
        /// 获取控制器类的全名
        /// </summary>
        /// <param name="controllerName">控制器名称</param>
        public string GetControllerFullName(string controllerName)
        {
            return string.Format("{0}.{1}Controller",
                string.IsNullOrEmpty(ControlerDefaultNameSpage) ? AssemblyName : ControlerDefaultNameSpage,
                controllerName);
        }

        /// <summary>
        /// 获取控制器实例对象，根据controllerName生成一个没有请求上下文对象的空的控制器，在为此控制器制定ControllerContext对象，然后返回控制器实例
        /// </summary>
        /// <param name="requestContext"></param>
        /// <param name="controllerName"></param>
        public IController CreateController(RequestContext requestContext, string controllerName)
        {
            var controller = ControllerAssembly.CreateInstance(GetControllerFullName(controllerName)) as Controller;
            if (controller == null)
                return null;
            if (controller.ControllerContext == null)
            {
                controller.ControllerContext = new ControllerContext(requestContext, controller);
            }
            else
            {
                controller.ControllerContext.RequestContext = requestContext;
            }
            return controller;
        }

        /// <summary>
        /// 返回请求的会话状态的支持类型
        /// </summary>
        /// <param name="requestContext"></param>
        /// <param name="controllerName"></param>
        public SessionStateBehavior GetControllerSessionBehavior(RequestContext requestContext, string controllerName)
        {
            var controllerType = ControllerAssembly.GetType(GetControllerFullName(controllerName), true, true);
            var sessionStateAttr =
                Attribute.GetCustomAttribute(controllerType, typeof (SessionStateAttribute), false) as
                    SessionStateAttribute;
            return sessionStateAttr == null ? SessionStateBehavior.Default : sessionStateAttr.Behavior;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="controller"></param>
        public void ReleaseController(IController controller)
        {
            var idDisposable = controller as IDisposable;
            if (idDisposable != null)
            {
                idDisposable.Dispose();
            }
        }
    }
}