using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace  CommonToolKit
{
    public class HtmlHelper
    {
        private static int deleteCount = 0;
        /// <summary>
        /// 替换html
        /// </summary>
        /// <param name="templateContent">模板内容</param>
        /// <param name="tags">要替换的标签</param>
        /// <returns></returns>
        public static string ReplaceTags(string templateContent, Dictionary<string, string> tags)
        {
            foreach (KeyValuePair<string, string> item in tags)
            {
                templateContent = templateContent.Replace(item.Key, item.Value);
            }
            return templateContent;
        }
        /// <summary>
        /// 创建html文件
        /// </summary>
        /// <param name="key">生成的html文件名关键部分,可为新闻ID</param>
        /// <param name="tags">替换标签集合</param>
        /// <param name="templateContent">模板内容</param>
        /// <returns></returns>
        public static bool WriteHtml(int key, Dictionary<string, string> tags, string templateContent)
        {
            return Write(key, string.Empty, string.Empty, tags, templateContent);
        }
        /// <summary>
        /// 创建html文件
        /// </summary>
        /// <param name="key">生成的html文件名关键部分,可为新闻ID</param>
        /// <param name="tags">替换标签集合</param>
        /// <param name="templateContent">模板内容</param>
        /// <param name="createDt">创建时间,生成的html文件将此日期的年月作为文件夹名</param>
        /// <returns></returns>
        public static bool WriteHtml(int key, Dictionary<string, string> tags, string templateContent, DateTime createDt)
        {
            return Write(key, createDt.ToString("yyyyMM"), string.Empty, tags, templateContent);
        }
        /// <summary>
        /// 创建html文件
        /// </summary>
        /// <param name="key">生成的html文件名关键部分,可为新闻ID</param>
        /// <param name="savePath">保存的路径</param>
        /// <param name="tags">替换标签集合</param>
        /// <param name="templateContent">模板内容</param>
        /// <returns></returns>
        public static bool WriteHtml(int key, string savePath, Dictionary<string, string> tags, string templateContent)
        {
            return Write(key, savePath, string.Empty, tags, templateContent);
        }
        /// <summary>
        /// 创建html文件
        /// </summary>
        /// <param name="key">生成的html文件名关键部分,可为新闻ID</param>
        /// <param name="savePath">保存的路径</param>
        /// <param name="newFileName">文件名</param>
        /// <param name="tags">替换标签集合</param>
        /// <param name="templateContent">模板内容</param>
        /// <returns></returns>
        public static bool WriteHtml(int key, string savePath, string newFileName, Dictionary<string, string> tags, string templateContent)
        {
            return Write(key, savePath, newFileName, tags, templateContent);
        }

        /// <summary>
        /// 创建html文件
        /// </summary>
        /// <param name="key">生成的html文件名关键部分,可为新闻ID</param>
        /// <param name="savePath">保存的路径</param>
        /// <param name="newFileName">文件名</param>
        /// <param name="tags">替换标签集合</param>
        /// <param name="templateContent">模板内容</param>
        /// <returns></returns>
        private static bool Write(int key, string savePath, string newFileName, Dictionary<string, string> tags, string templateContent)
        {
            string webPath = System.Web.HttpContext.Current.Server.MapPath("~/");
            savePath = savePath.ToLower().Trim();
            if (savePath != string.Empty)
            {
                savePath = string.Format("{0}/news/{1}", webPath, savePath);
            }
            else
            {
                string date = DateTime.Now.ToString("yyyyMM");
                savePath = string.Format("{0}/news/{1}", webPath, date);
            }
            DirectoryInfo dir = new DirectoryInfo(savePath);
            //如果目录不存在则创建
            if (!dir.Exists) { dir.Create(); }
            if (newFileName != string.Empty)
            {
                savePath = string.Format("{0}/{1}.html", savePath, newFileName);
            }
            else
            {
                int keyLength = key.ToString().Length;
                string strKey = string.Empty;
                for (int i = 0; i < 6 - keyLength; i++) { strKey += "0"; }
                strKey += key.ToString();
                savePath = string.Format("{0}/{1}.html", savePath, strKey);
            }
            Encoding code = Encoding.GetEncoding("UTF-8");
            string content = ReplaceTags(templateContent, tags);//替换内容
            StreamWriter sw = new StreamWriter(savePath, false, code);
            try
            {
                sw.Write(content);
                sw.Flush();
            }
            catch (Exception e)
            {
                MessageBox.Show("在创建HTM文件时,出现错误,请与管理员联系。错误内容:" + e.Message);
                System.Web.HttpContext.Current.Response.End();
                return false;
            }
            finally
            {
                sw.Close();
            }
            return true;
        }
        /// <summary>
        /// 去除文件名中的文件夹名
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        private static string ClearFileFolder(string fileName)
        {
            fileName = fileName.Trim().Replace("//", "/").Replace("\\", "/");
            if (fileName != string.Empty && fileName.IndexOf(".") != -1 && fileName.IndexOf("/") != -1)
            {
                fileName = fileName.Substring(fileName.LastIndexOf("/") + 1, fileName.Length - fileName.LastIndexOf("/") - 1);
            }
            return fileName;
        }
        /// <summary>
        /// 删除html文件
        /// </summary>
        /// <param name="fileName">不带后缀的文件名</param>
        /// <param name="filePath">文件路径(一般为年月yyyyMM)</param>
        /// <returns></returns>
        public static bool Delete(string fileName, string filePath)
        {
            return Del(0, fileName, filePath);
        }
        /// <summary>
        /// 删除html文件
        /// </summary>
        /// <param name="fileName">不带后缀的文件名</param>
        /// <param name="filePath">文件路径(一般为年月yyyyMM)</param>
        /// <returns></returns>
        public static bool Delete(int key, string filePath)
        {
            return Del(key, string.Empty, filePath);
        }
        /// <summary>
        /// 删除html文件
        /// </summary>
        /// <param name="fileName">不带后缀的文件名</param>
        /// <param name="dt">删除文件的文件夹依据,一般为数据的添加时间</param>
        /// <returns></returns>
        public static bool Delete(int key, DateTime dt)
        {
            return Del(key, string.Empty, dt.ToString("yyyyMM"));
        }
        /// <summary>
        /// 删除html文件
        /// </summary>
        /// <param name="key">生成的HTM文件名关键部分,可为新闻ID</param>
        /// <param name="fileName">不带后缀的文件名</param>
        /// <param name="filePath">文件路径(一般为年月yyyyMM)</param>
        /// <returns></returns>
        private static bool Del(int key, string fileName, string filePath)
        {
            bool isOk = false;
            fileName = ClearFileFolder(fileName);
            if (fileName == string.Empty)
            {
                fileName = GetHtmFileName(key);
            }
            fileName = fileName.ToLower().Replace(".html", "");//去除后缀
            string webPath = System.Web.HttpContext.Current.Server.MapPath("~/");
            string delHtm = string.Empty;
            try
            {
                delHtm = string.Format("{0}/news/{1}/{2}.html", webPath, filePath, fileName);
                if (File.Exists(delHtm))//如果该文件存在，则删除
                {
                    deleteCount++;
                    File.Delete(delHtm);
                    isOk = true;
                }
            }
            catch
            {
                if (deleteCount < 100)//为避免因资源被占用删除不了数据   所以在此循环100次
                    Del(key, fileName, filePath);
            }
            finally
            {
                deleteCount = 0;
            }
            return isOk;
        }
        /// <summary>
        /// 获取html文件名
        /// </summary>
        /// <param name="key">生成的HTM文件名关键部分,可为新闻ID</param>
        /// <returns></returns>
        public static string GetHtmFileName(int key)
        {
            int keyLength = key.ToString().Length;
            string strKey = string.Empty;
            for (int i = 0; i < 6 - keyLength; i++) { strKey += "0"; }
            strKey += key.ToString() + ".html";
            return strKey;
        }
        /// <summary>
        /// 获取html文件路径
        /// </summary>
        /// <param name="key">生成的HTM文件名关键部分,可为新闻ID</param>
        /// <param name="folder">文件夹名</param>
        /// <returns></returns>
        public static string GetHtmFilePath(int key, string folder)
        {
            return string.Format("{0}/{1}", folder, GetHtmFileName(key));
        }
        /// <summary>
        /// 获取html文件路径
        /// </summary>
        /// <param name="key">生成的HTM文件名关键部分,可为新闻ID</param>
        /// <param name="dtFolder">文件夹名(以时间的年月:yyyyMM作为文件夹名)</param>
        /// <returns></returns>
        public static string GetHtmFilePath(int key, DateTime dtFolder)
        {
            return string.Format("{0}/{1}", dtFolder.ToString("yyyyMM"), GetHtmFileName(key));
        }

        /// <summary>
        /// 创建HTML文件(此方法将HTML文件存放在网站根目录的html文件夹中)
        /// </summary>
        /// <param name="newFileName">文件名</param>
        /// <param name="htmlContent">文件内容</param>
        /// <returns></returns>
        public static bool WriteHtml(string title,string newFileName, string htmlContent)
        {
            StringBuilder sbHtml = new StringBuilder();
            sbHtml.Append("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\r\n");
            sbHtml.Append("<html xmlns=\"http://www.w3.org/1999/xhtml\">\r\n");
            sbHtml.Append("<head>\r\n");
            sbHtml.Append(string.Format("<title>{0}</title>\r\n", title));
            sbHtml.Append("</head>\r\n");
            sbHtml.Append("<body>\r\n");
            sbHtml.Append(htmlContent);
            sbHtml.Append("\r\n</body>\r\n");
            sbHtml.Append("</html>\r\n");

            newFileName = newFileName.Trim();
            string savePath = System.Web.HttpContext.Current.Server.MapPath("~/") + "html";
            DirectoryInfo dir = new DirectoryInfo(savePath);
            //如果目录不存在则创建
            if (!dir.Exists) { dir.Create(); }

            if (newFileName != string.Empty)
                savePath = string.Format("{0}/{1}.html", savePath, newFileName);
            else
                savePath = string.Format("{0}/{1}.html", savePath, DateTime.Now.ToString("yMdhhmmssfff"));

            Encoding code = Encoding.GetEncoding("UTF-8");
            StreamWriter sw = new StreamWriter(savePath, false, code);
            try
            {
                sw.Write(sbHtml.ToString());
                sw.Flush();
            }
            catch (Exception e)
            {
                MessageBox.Show("在创建HTM文件时,出现错误,请与管理员联系。错误内容:" + e.Message);
                System.Web.HttpContext.Current.Response.End();
                return false;
            }
            finally
            {
                sw.Close();
            }
            return true;
        }
    }
}
