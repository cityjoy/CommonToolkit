using System;
using System.Text;

namespace  CommonToolKit
{
    public class MessageBox
    {
        private MessageBox()
        {
        }

        /// <summary>
        /// 显示消息提示对话框
        /// </summary>
        /// <param name="page">当前页面指针，一般为this</param>
        /// <param name="msg">提示信息</param>
        public static void Show(System.Web.UI.Page page, string msg)
        {
            page.ClientScript.RegisterStartupScript(page.GetType(), "message", "<script language=\"javascript\" defer>alert(\"" + msg.ToString() + "\");</script>");
        }

        /// <summary>
        /// 显示消息提示对话框后关闭窗口
        /// </summary>
        /// <param name="msg">提示信息</param>
        public static void ShowAndClose(string msg)
        {
            System.Web.HttpContext.Current.Response.Write("<script type=\"text/javascript\" language=\"JavaScript\">alert('" + msg + "');parent.window.opener = null;parent.window.open('', '_self');parent.window.close();parent.window.location.href = '';window.close();</script>");
        }

        /// <summary>
        /// 显示消息提示对话框
        /// </summary>
        /// <param name="page">当前页面指针，一般为this</param>
        /// <param name="msg">提示信息</param>
        public static void Show(System.Web.UI.HtmlControls.HtmlInputFile Control, string msg)
        {
            System.Web.UI.Page page;
            page = System.Web.HttpContext.Current.Handler as System.Web.UI.Page;
            page.ClientScript.RegisterStartupScript(Control.GetType(), "message", "<script language=\"javascript\" defer>alert(\"" + msg.ToString() + "\");</script>");
        }
        /// <summary>
        /// 显示消息提示对话框
        /// </summary>
        /// <param name="msg">提示信息</param>
        public static void Show(string msg)
        {
            System.Web.HttpContext.Current.Response.Write("<script type=\"text/javascript\" language=\"JavaScript\">alert('"+msg+"');</script>");
        }

        /// <summary>
        /// 显示消息提示对话框，返回上一页
        /// </summary>
        /// <param name="msg">提示信息</param>
        public static void ShowAndGoToHistory(string msg)
        {
            System.Web.HttpContext.Current.Response.Write("<script language=\"javascript\" defer>alert(\"" + msg + "\");history.go(-1);</script>");
        }

        /// <summary>
        /// 显示消息提示对话框
        /// </summary>
        /// <param name="page">当前页面指针，一般为this</param>
        /// <param name="msg">提示信息</param>
        public static void Show(System.Web.UI.WebControls.WebControl Control, string msg)
        {
            System.Web.UI.Page page;
            page = System.Web.HttpContext.Current.Handler as System.Web.UI.Page;
            page.ClientScript.RegisterStartupScript(Control.GetType(), "message", "<script language=\"javascript\" defer>alert(\"" + msg.ToString() + "\");</script>");
        }

        /// <summary>
        /// 控件点击 消息确认提示框
        /// </summary>
        /// <param name="page">当前页面指针，一般为this</param>
        /// <param name="msg">提示信息</param>
        public static void ShowConfirm(System.Web.UI.WebControls.WebControl Control, string msg)
        {
            Control.Attributes.Add("onclick", "return confirm(\"" + msg + "\");");
        }

        /// <summary>
        /// 显示消息提示对话框，并进行页面跳转
        /// </summary>
        /// <param name="page">当前页面指针，一般为this</param>
        /// <param name="msg">提示信息</param>
        /// <param name="url">跳转的目标URL</param>
        public static void ShowAndRedirect(System.Web.UI.Page page, string msg, string url)
        {
            page.ClientScript.RegisterStartupScript(page.GetType(), "message", "<script language=\"javascript\">alert(\"" + msg + "\");window.location=\"" + url + "\"</script>");
        }
        /// <summary>
        /// 显示消息提示对话框，并进行页面跳转
        /// </summary>
        /// <param name="msg">提示信息</param>
        /// <param name="url">跳转的目标URL</param>
        public static void ShowAndRedirect(string msg, string url)
        {
            System.Web.HttpContext.Current.Response.Write("<script language=\"javascript\" defer>alert(\"" + msg + "\");window.location=\"" + url + "\"</script>");
        }
       
        /// <summary>
        /// 显示消息提示对话框，并进行页面跳转
        /// </summary>
        /// <param name="page">当前页面指针，一般为this</param>
        /// <param name="msg">提示信息</param>
        /// <param name="url">跳转的目标URL</param>
        public static void ShowAndRedirects(System.Web.UI.Page page, string msg, string url)
        {
            StringBuilder Builder = new StringBuilder();
            Builder.Append("<script language=\"javascript\" defer>");
            Builder.AppendFormat("alert('{0}');", msg);
            Builder.AppendFormat("top.location.href='{0}'", url);
            Builder.Append("</script>");
            page.ClientScript.RegisterStartupScript(page.GetType(), "message", Builder.ToString());

        }

        /// <summary>
        /// 输出自定义脚本信息
        /// </summary>
        /// <param name="page">当前页面指针，一般为this</param>
        /// <param name="script">输出脚本</param>
        public static void ResponseScript(System.Web.UI.Page page, string script)
        {
            page.ClientScript.RegisterStartupScript(page.GetType(), "message", "<script language='javascript' defer>" + script + "</script>");
        }

        /// <summary>
        /// 关闭弹出窗口(注：针对zDialog.js文件设计，如果是使用zDialog.js弹出的窗口，则可使用此方法。否者禁止调用，会报脚本错误)
        /// </summary>
        /// <param name="page">System.Web.UI.Page</param>
        /// <param name="isReload">是否刷新父页</param>
        public static void ZDialogClose(System.Web.UI.Page page, bool isReload)
        {
            string script = script = "<script language='javascript' defer>Dialog.Close();</script>";
            if (isReload)
            {
                script = "<script language='javascript' defer>top.main_page.location.reload();Dialog.Close();</script>";
            }
            page.ClientScript.RegisterStartupScript(page.GetType(), "message", script);
        }

        /// <summary>
        /// 关闭弹出窗口(注：针对zDialog.js文件设计，如果是使用zDialog.js弹出的窗口，则可使用此方法。否者禁止调用，会报脚本错误)
        /// </summary>
        /// <param name="page">System.Web.UI.Page</param>
        /// <param name="msg">弹出提示信息</param>
        /// <param name="isReload">是否刷新父页</param>
        public static void ZDialogClose(System.Web.UI.Page page, string msg, bool isReload)
        {
            string script = script = "<script language='javascript' defer>Dialog.alert('" + msg + "',function(){ownerDialog.close();});</script>";
            if (isReload)
            {
                script = "<script language='javascript' defer>Dialog.alert('" + msg + "',function(){top.main_page.location.reload();ownerDialog.close();});</script>";
            }
            page.ClientScript.RegisterStartupScript(page.GetType(), "message", script);
        }

        public static void ZDialogAlert(System.Web.UI.Page page, string msg)
        {
            string script = script = "<script language='javascript' defer>Dialog.alert('" + msg + "');</script>";
            page.ClientScript.RegisterStartupScript(page.GetType(), "message", script);
        }

        public static void ZDialogAlert(System.Web.UI.Page page, string msg, string url)
        {
            string script = script = "<script language='javascript' defer>Dialog.alert('" + msg + "',function(){window.location.href='" + url + "';});</script>";
            page.ClientScript.RegisterStartupScript(page.GetType(), "message", script);
        }
        public static void ZDialogAlertGoOutIframe(System.Web.UI.Page page, string msg, string url)
        {
            string script = script = "<script language='javascript' defer>Dialog.alert('" + msg + "',function(){top.location.href='" + url + "';});</script>";
            page.ClientScript.RegisterStartupScript(page.GetType(), "message", script);
        }
    }
}
