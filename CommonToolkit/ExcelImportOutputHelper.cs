using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;

namespace  CommonToolKit
{
    public class ExcelImportOutputHelper
    {
        protected HttpContextBase _context;
        /// <summary>
        /// 文件Url
        /// </summary>
        protected string _fileUrl = string.Empty;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="context"></param>
        public void Init(HttpContextBase context, string fileUrl)
        {
            this._context = context;
            this._fileUrl = this._context.Server.MapPath("~/" + fileUrl);
        }

        private bool DownFile(System.Web.HttpResponse Response, string fileName, string fullPath)
        {
            try
            {
                Response.ContentType = "application/octet-stream";

                Response.AppendHeader("Content-Disposition", "attachment;filename=" +
                HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8) + ";charset=GB2312");
                System.IO.FileStream fs = System.IO.File.OpenRead(fullPath);
                long fLen = fs.Length;
                int size = 102400;//每100K同时下载数据 
                byte[] readData = new byte[size];//指定缓冲区的大小 
                if (size > fLen) size = Convert.ToInt32(fLen);
                long fPos = 0;
                bool isEnd = false;
                while (!isEnd)
                {
                    if ((fPos + size) > fLen)
                    {
                        size = Convert.ToInt32(fLen - fPos);
                        readData = new byte[size];
                        isEnd = true;
                    }
                    fs.Read(readData, 0, size);//读入一个压缩块 
                    Response.BinaryWrite(readData);
                    fPos += size;
                }
                fs.Close();
                System.IO.File.Delete(fullPath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 检测上传的文件是否存在指定的列
        /// </summary>
        /// <param name="dt">上传的Excel转化为DataTable后的数据</param>
        /// <param name="importColumnNames">需要检查的列</param>
        /// <returns></returns>
        protected bool CheckColumnIsAccess(DataTable dt, string[] importColumnNames)
        {
            int length = importColumnNames.Length;
            int currLength = 0;
            foreach (string colN in importColumnNames)
            {
                foreach (DataColumn column in dt.Columns)
                {
                    if (column.ColumnName.Trim() == colN)
                    {
                        currLength++;
                        break;
                    }
                }
            }
            return length == currLength;
        }

        /// <summary>
        /// 文件导入初期检测
        /// </summary>
        /// <param name="ds">需导入的集合</param>
        /// <param name="importMaxRowCount">允许导入的最大行（如果为0则表示不限制）</param>
        /// <param name="importColumnNames">需要导入的列</param>
        /// <param name="notRepeatColumns">不可重复的列</param>
        /// <returns></returns>
        public string ImportEarlyChecked(DataSet ds, int importMaxRowCount, string[] importColumnNames, params string[] notRepeatColumns)
        {
            string errorMsg = string.Empty;
            if (ds == null)
            {
                errorMsg = "系统检查发现您更改过excel模板文件或选择了不符合规则的文件\r\n" +
                        "故您此次上传的文件无效，请还原后重新添加上传。";
            }
            else if (ds.Tables.Count <= 0)
            {
                errorMsg = "您选择的Excel文件没有任何表可供导入!";
            }
            else if (ds.Tables[0].Rows.Count <= 0)
            {
                errorMsg = "您选择的Excel文件没有任何数据可供导入!";
            }
            else if (!this.CheckColumnIsAccess(ds.Tables[0], importColumnNames))
            {
                errorMsg = "系统检查发现您更改过excel模板文件或选择了不符合规则的文件\r\n" +
                        "故您此次上传的文件无效，请还原后重新添加上传。";
            }
            else if (importMaxRowCount != 0 && ds.Tables[0].Rows.Count > importMaxRowCount)
            {
                errorMsg = string.Format("每次操作的数据不可超过{0}行！请自行拆分！", importMaxRowCount);
            }
            else
            {
                DataTable dt = ds.Tables[0];
                int rowCount = dt.Rows.Count;
                List<string> listColumn = new List<string>();
                //检测上传的excel中是否有重复的数据
                foreach (string column in notRepeatColumns)
                {
                    listColumn.Clear();
                    int rowIndex = 2;
                    string errorIndex = string.Empty;
                    foreach (DataRow item in dt.Rows)
                    {
                        string v = item[column].ToString();
                        if (listColumn.Contains(v))
                        {
                            errorIndex += errorIndex != string.Empty ? "," + rowIndex.ToString() : rowIndex.ToString();
                        }
                        listColumn.Add(v);
                        rowIndex++;
                    }
                    if (errorIndex != string.Empty)
                    {
                        errorMsg += "列“" + column + "”，第" + errorIndex + "行存在重复数据；\r\n";
                    }
                }
            }
            return errorMsg;
        }


        //恶心的多次删除文件，避免被占用无法删除
        int delCount = 0;
        /// <summary>
        /// 删除上传的Excel文件
        /// </summary>
        public void DelUploadExcelFile()
        {
            if (File.Exists(this._fileUrl))
            {
                try
                {
                    File.Delete(this._fileUrl);
                }
                catch
                {
                    delCount++;
                    if (delCount < 200)
                    {
                        this.DelUploadExcelFile();
                    }
                }
            }
        }

        /// <summary>
        /// 输出信息(如果输入1,则表示操作成功)
        /// </summary>
        /// <param name="msg">需要输出的内容（如果为1，则表示操作成功）</param>
        protected void ResponseMsg(string msg)
        {
            this._context.Response.Write(msg);
        }
    }
}
