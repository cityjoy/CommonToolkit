using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Web;
using Ionic.Zip;

namespace  CommonToolKit
{
    public class ZipHelper
    {
        /// <summary>  
        /// 压缩文件  
        /// </summary>  
        /// <param name="oldFileUrlList"></param>  
        /// <param name="newFileUrlList"></param>  
        public static void CreateZip(string saveUrl, List<string> oldFileUrlList, List<string> newFileUrlList)
        {
            //传递的数据如果有误  则不执行压缩
            if (oldFileUrlList.Count != newFileUrlList.Count) return;

            if (!Directory.Exists(Path.GetDirectoryName(saveUrl)))
                Directory.CreateDirectory(Path.GetDirectoryName(saveUrl));

            if (File.Exists(saveUrl)) { File.Delete(saveUrl); }
            using (ZipFile zip = new ZipFile(saveUrl, System.Text.Encoding.UTF8))
            {
                zip.UseZip64WhenSaving = Zip64Option.Never;

                int fileCount = oldFileUrlList.Count;
                for (int i = 0; i < fileCount; i++)
                {
                    string fileUrl = oldFileUrlList[i];
                    if (File.Exists(fileUrl))
                    {
                        string tempFile = newFileUrlList[i];
                        tempFile = Path.GetDirectoryName(tempFile);
                        //if (tempFile.IndexOf("/") == -1 && tempFile.IndexOf(".xml") != -1) tempFile = "";
                        zip.AddFile(fileUrl, tempFile);
                    }
                }
                zip.Save();
            }
        }

        /// <summary>
        /// 从zip文件中解压全部文件  
        /// </summary>
        /// <param name="zipFullName">待解压的ZIP文件完整名称（包括路径）</param>
        /// <param name="saveRootFullPath">文件保存完整路径</param>
        /// <returns></returns>
        public static bool UntieAllFile(string zipFullName, string saveRootFullPath, out string errorMsg)
        {
            errorMsg = string.Empty;
            if (!File.Exists(zipFullName)) { errorMsg = "待解压文件不存在！"; return false; }
            if (Directory.Exists(saveRootFullPath)) { errorMsg = "解压目标文件夹已存在！"; return false; }
            Directory.CreateDirectory(saveRootFullPath);
            try
            {
                using (ZipFile zip = ZipFile.Read(zipFullName))
                {
                    //if (!string.IsNullOrEmpty(password)) { zip.Password = password; }
                    foreach (ZipEntry entry in zip)
                    {
                        entry.Extract(saveRootFullPath);
                    }
                }
            }
            catch (Exception e)
            {
                errorMsg = string.Format("解压文件发生异常，详细信息如下：" + e.Message);
            }
            return true;
        }
    }
}