using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Web;

namespace  CommonToolKit
{
    public class FolderManageHelper
    {
        #region 获取指定文件夹下所有子目录及文件(树形)
        /****************************************
         * 函数名称：GetFoldAll(string Path)
         * 功能说明：获取指定文件夹下所有子目录及文件(树形)
         * 参    数：Path:详细路径
         * 调用示列：
         *           string strDirlist = Server.MapPath("templates");       
         *           this.Literal1.Text = EC.GetFoldList.GetFoldAll(strDirlist);  
        *****************************************/
        /// <summary>
        /// 获取指定文件夹下所有子目录及文件
        /// </summary>
        /// <param name="Path">详细路径</param>
        public static string GetFoldAll(string Path)
        {

            string str = "";
            DirectoryInfo thisOne = new DirectoryInfo(Path);
            str = ListTreeShow(thisOne, 0, str);
            return str;

        }

        /// <summary>
        /// 获取指定文件夹下所有子目录及文件函数
        /// </summary>
        /// <param name="theDir">指定目录</param>
        /// <param name="nLevel">默认起始值,调用时,一般为0</param>
        /// <param name="Rn">用于迭加的传入值,一般为空</param>
        /// <returns></returns>
        public static string ListTreeShow(DirectoryInfo theDir, int nLevel, string Rn)//递归目录 文件
        {
            DirectoryInfo[] subDirectories = theDir.GetDirectories();//获得目录
            foreach (DirectoryInfo dirinfo in subDirectories)
            {

                if (nLevel == 0)
                {
                    Rn += "├";
                }
                else
                {
                    string _s = "";
                    for (int i = 1; i <= nLevel; i++)
                    {
                        _s += "│&nbsp;";
                    }
                    Rn += _s + "├";
                }
                Rn += "<b>" + dirinfo.Name.ToString() + "</b><br />";
                FileInfo[] fileInfo = dirinfo.GetFiles();   //目录下的文件
                foreach (FileInfo fInfo in fileInfo)
                {
                    if (nLevel == 0)
                    {
                        Rn += "│&nbsp;├";
                    }
                    else
                    {
                        string _f = "";
                        for (int i = 1; i <= nLevel; i++)
                        {
                            _f += "│&nbsp;";
                        }
                        Rn += _f + "│&nbsp;├";
                    }
                    Rn += fInfo.Name.ToString() + " <br />";
                }
                Rn = ListTreeShow(dirinfo, nLevel + 1, Rn);


            }
            return Rn;
        }



        /****************************************
         * 函数名称：GetFoldAll(string Path)
         * 功能说明：获取指定文件夹下所有子目录及文件(下拉框形)
         * 参    数：Path:详细路径
         * 调用示列：
         *            string strDirlist = Server.MapPath("templates");      
         *            this.Literal2.Text = EC.FileObj.GetFoldAll(strDirlist,"tpl","");
        *****************************************/
        /// <summary>
        /// 获取指定文件夹下所有子目录及文件(下拉框形)
        /// </summary>
        /// <param name="Path">详细路径</param>
        ///<param name="DropName">下拉列表名称</param>
        ///<param name="tplPath">默认选择模板名称</param>
        public static string GetFoldAll(string Path, string DropName, string tplPath)
        {
            string strDrop = "<select name=\"" + DropName + "\" id=\"" + DropName + "\"><option value=\"\">--请选择详细模板--</option>";
            string str = "";
            DirectoryInfo thisOne = new DirectoryInfo(Path);
            str = ListTreeShow(thisOne, 0, str, tplPath);
            return strDrop + str + "</select>";

        }

        /// <summary>
        /// 获取指定文件夹下所有子目录及文件函数
        /// </summary>
        /// <param name="theDir">指定目录</param>
        /// <param name="nLevel">默认起始值,调用时,一般为0</param>
        /// <param name="Rn">用于迭加的传入值,一般为空</param>
        /// <param name="tplPath">默认选择模板名称</param>
        /// <returns></returns>
        public static string ListTreeShow(DirectoryInfo theDir, int nLevel, string Rn, string tplPath)//递归目录 文件
        {
            DirectoryInfo[] subDirectories = theDir.GetDirectories();//获得目录

            foreach (DirectoryInfo dirinfo in subDirectories)
            {

                Rn += "<option value=\"" + dirinfo.Name.ToString() + "\"";
                if (tplPath.ToLower() == dirinfo.Name.ToString().ToLower())
                {
                    Rn += " selected ";
                }
                Rn += ">";

                if (nLevel == 0)
                {
                    Rn += "┣";
                }
                else
                {
                    string _s = "";
                    for (int i = 1; i <= nLevel; i++)
                    {
                        _s += "│&nbsp;";
                    }
                    Rn += _s + "┣";
                }
                Rn += "" + dirinfo.Name.ToString() + "</option>";


                FileInfo[] fileInfo = dirinfo.GetFiles();   //目录下的文件
                foreach (FileInfo fInfo in fileInfo)
                {


                    if (GetFileExtends(fInfo.Name.ToString()) == "htm" || GetFileExtends(fInfo.Name.ToString()) == "html" || GetFileExtends(fInfo.Name.ToString()) == "aspx")
                    {

                        Rn += "<option value=\"" + dirinfo.Name.ToString() + "/" + fInfo.Name.ToString() + "\"";
                        if (tplPath.ToLower() == dirinfo.Name.ToString().ToLower() + "/" + fInfo.Name.ToString().ToLower())
                        {
                            Rn += " selected ";
                        }
                        Rn += ">";

                        if (nLevel == 0)
                        {
                            Rn += "│&nbsp;├";
                        }
                        else
                        {
                            string _f = "";
                            for (int i = 1; i <= nLevel; i++)
                            {
                                _f += "│&nbsp;";
                            }
                            Rn += _f + "│&nbsp;├";
                        }
                        Rn += fInfo.Name.ToString() + "</option>";
                    }
                }
                Rn = ListTreeShow(dirinfo, nLevel + 1, Rn, tplPath);


            }
            return Rn;
        }
        #endregion


        #region 获取指定文件夹下目录
        /// <summary>
        /// 获取指定文件夹下目录
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <returns></returns>
        public static DataTable GetFold(string path)
        {
            DataTable dt = new DataTable();
            DataColumn dc;
            DataRow dr;

            dc = new DataColumn();
            dc.DataType = System.Type.GetType("System.String");
            dc.ColumnName = "name";
            dt.Columns.Add(dc);

            dc = new DataColumn();
            dc.DataType = System.Type.GetType("System.DateTime");
            dc.ColumnName = "createTime";
            dt.Columns.Add(dc);

            dc = new DataColumn();
            dc.DataType = System.Type.GetType("System.DateTime");
            dc.ColumnName = "updateTime";
            dt.Columns.Add(dc);

            dc = new DataColumn();
            dc.DataType = System.Type.GetType("System.String");
            dc.ColumnName = "path";
            dt.Columns.Add(dc);



            if (!System.IO.Directory.Exists(path))
            {
                throw new Exception("不存在相应目录");
            }
            else
            {
                DirectoryInfo thisOne = new DirectoryInfo(path);
                DirectoryInfo[] subDirectories = thisOne.GetDirectories();//获得目录

                foreach (DirectoryInfo dirinfo in subDirectories)
                {
                    dr = dt.NewRow();
                    dr["name"] = dirinfo.Name.ToString();
                    dr["createTime"] = dirinfo.CreationTime;
                    dr["updateTime"] = dirinfo.LastWriteTime;
                    dr["path"] = dirinfo.FullName;
                    
                    dt.Rows.Add(dr);

                }

            }
            return dt;


        }


        #endregion

        #region 判断是否存在子目录
        /// <summary>
        /// 获取指定目录是否存在子目录
        /// </summary>
        /// <param name="path">需判断的目录</param>
        /// <returns></returns>
        public static bool ExistsChildFolder(string path)
        {
            DirectoryInfo thisOne = new DirectoryInfo(path);
            DirectoryInfo[] subDirectories = thisOne.GetDirectories();
            return (subDirectories != null && subDirectories.Length > 0) ? true : false;
        }
        #endregion

        #region 获取指定目录下的文件

        /// <summary>
        /// 获取指定目录下所有文件信息
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="extensions">文件类型集合，多个以英文逗号分隔。如：.jpg;.gif;.rar;</param>
        /// <returns></returns>
        public static IList<FolderFileInfo> GetFils(string Path, string extensions, int pageIndex, int pageSize, out int rowCount)
        {
            pageIndex -= 1;
            IList<FolderFileInfo> listFolderFileInfo = GetFils(Path, extensions);
            rowCount = listFolderFileInfo.Count;
            IList<FolderFileInfo> listFolderFileInfo2 = new List<FolderFileInfo>();
            int index = 0;
            foreach (FolderFileInfo item in listFolderFileInfo)
            {
                if (item.Id > pageIndex * pageSize)
                {
                    if (index < pageSize)
                    {
                        listFolderFileInfo2.Add(item);
                        index++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return listFolderFileInfo2;
        }

        /// <summary>
        /// 获取指定目录下所有文件信息
        /// </summary>
        /// <param name="path"></param>
        /// <param name="extensions">文件类型集合，多个以英文逗号分隔。如：.jpg;.gif;.rar;</param>
        /// <returns></returns>
        public static IList<FolderFileInfo> GetFils(string path, string extensions)
        {
            IList<FolderFileInfo> listFolderFileInfo = new List<FolderFileInfo>();
            try
            {
                DirectoryInfo thisOne = new DirectoryInfo(path);
                int id = 1;
                FileInfo[] fileInfo = thisOne.GetFiles();
                string[] exten = extensions.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (FileInfo fInfo in fileInfo)
                {
                    //获取指定文件类型的文件
                    if (exten.Length > 0)
                    {
                        foreach (string item in exten)
                        {
                            if (fInfo.Extension.ToLower() == item.ToLower())
                            {
                                FolderFileInfo folderFileInfo = new FolderFileInfo();
                                folderFileInfo.Id = id;
                                folderFileInfo.CreateTime = fInfo.CreationTime;
                                folderFileInfo.Extension = fInfo.Extension;
                                folderFileInfo.FileSize = (int)fInfo.Length;
                                folderFileInfo.FileUrl = path + fInfo.Name + fInfo.Extension;
                                folderFileInfo.Folder = fInfo.DirectoryName;
                                folderFileInfo.Name = fInfo.Name;
                                folderFileInfo.FullName = fInfo.FullName;
                                listFolderFileInfo.Add(folderFileInfo);
                                id++;
                                break;
                            }
                        }
                    }
                    else//获取所有文件
                    {
                        FolderFileInfo folderFileInfo = new FolderFileInfo();
                        folderFileInfo.Id = id;
                        folderFileInfo.CreateTime = fInfo.CreationTime;
                        folderFileInfo.Extension = fInfo.Extension;
                        folderFileInfo.FileSize = (int)fInfo.Length;
                        folderFileInfo.FileUrl = path + fInfo.Name + fInfo.Extension;
                        folderFileInfo.Folder = fInfo.DirectoryName;
                        folderFileInfo.Name = fInfo.Name;
                        folderFileInfo.FullName = fInfo.FullName;
                        listFolderFileInfo.Add(folderFileInfo);
                        id++;
                    }
                }
            }
            catch { }
            return listFolderFileInfo;
        }

        #endregion

        #region 取得文件后缀
        /**********************************
         * 函数名称:GetFileExtends
         * 功能说明:取得文件后缀
         * 参    数:filename:文件名称
         * 调用示例:
         *          GetRemoteObj o = new GetRemoteObj();
         *          string url = @"http://www.baidu.com/img/logo.gif";
         *          string s = o.GetFileExtends(url);
         *          Response.Write(s);
         *          o.Dispose();
         * ********************************/
        /// <summary>
        /// 取得文件后缀
        /// </summary>
        /// <param name="filename">文件名称</param>
        /// <returns></returns>
        public static string GetFileExtends(string filename)
        {
            string ext = null;
            if (filename.IndexOf(".") > 0)
            {

                string[] fs = filename.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                ext = fs[fs.Length - 1];
            }
            return ext;
        }
        #endregion
    }

    public class FolderFileInfo
    {
        private int _id = 0;
        private string _name = string.Empty;
        private string _fileUrl = string.Empty;
        private string _extension = string.Empty;
        private DateTime _createTime = DateTime.Now;
        private DateTime _updateTime = DateTime.Now;
        private string _folder = string.Empty;
        private int _fileSize = 0;
        private string _fullName = string.Empty;

        /// <summary>
        /// 编号
        /// </summary>
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// 图片名称
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// 文件Url
        /// </summary>
        public string FileUrl
        {
            get { return _fileUrl; }
            set { _fileUrl = value; }
        }

        /// <summary>
        /// 文件类型
        /// </summary>
        public string Extension
        {
            get { return _extension; }
            set { _extension = value; }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime
        {
            get { return _createTime; }
            set { _createTime = value; }
        }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdateTime
        {
            get { return _updateTime; }
            set { _updateTime = value; }
        }

        /// <summary>
        /// 所属目录
        /// </summary>
        public string Folder
        {
            get { return _folder; }
            set { _folder = value; }
        }

        /// <summary>
        /// 文件大小
        /// </summary>
        public int FileSize
        {
            get { return _fileSize; }
            set { _fileSize = value; }
        }

        /// <summary>
        /// 文件的完整目录
        /// </summary>
        public string FullName
        {
            get { return _fullName; }
            set { _fullName = value; }
        }
    }
}

