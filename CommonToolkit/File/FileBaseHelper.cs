// 源文件头信息：
// <copyright file="FileBaseHelper.cs">
// Copyright(c)2014-2034 Kencery.All rights reserved.
// 个人博客：http://www.cnblogs.com/hanyinglong
// 创建人：韩迎龙(kencery)
// 创建时间：2015/04/30
// </copyright>

using System;
using System.IO;

namespace CommonToolKit
{
    /// <summary>
    /// 文件操作基础类,（判断文件是否存在，检查目录是否存在...）
    /// </summary>
    public static class FileBaseHelper
    {
        /// <summary>
        /// 检查某个文件是否真的存在
        /// </summary>
        /// <param name="path">需要检查的文件的路径(包括路径的文件全名)</param>
        /// <returns>返回true则表示存在，false为不存在</returns>
        public static bool IsFileExists(string path)
        {
            try
            {
                return File.Exists(path);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 检查文件目录是否真的存在
        /// </summary>
        /// <param name="path">需要检查的文件目录</param>
        /// <returns>返回true则表示存在，false为不存在</returns>
        public static bool IsDirectoryExists(string path)
        {
            try
            {
                return Directory.Exists(Path.GetDirectoryName(path));
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 查找文件中是否存在匹配的内容
        /// </summary>
        /// <param name="fileInfo">查找的文件流信息</param>
        /// <param name="lineTxt">在文件中需要查找的行文本</param>
        /// <param name="lowerUpper">是否区分大小写，true为区分，false为不区分</param>
        /// <returns>返回true则表示存在，false为不存在</returns>
        public static bool FindLineTextFromFile(FileInfo fileInfo, string lineTxt, bool lowerUpper = false)
        {
            bool isTrue = false; //表示没有查询到信息
            try
            {
                //首先判断文件是否存在
                if (fileInfo.Exists)
                {
                    var streamReader = new StreamReader(fileInfo.FullName);
                    do
                    {
                        string readLine = streamReader.ReadLine(); //读取的信息
                        if (string.IsNullOrEmpty(readLine))
                        {
                            break;
                        }
                        if (lowerUpper)
                        {
                            if (readLine.Trim() != lineTxt.Trim())
                            {
                                continue;
                            }
                            isTrue = true;
                            break;
                        }
                        if (readLine.Trim().ToLower() != lineTxt.Trim().ToLower())
                        {
                            continue;
                        }
                        isTrue = true;
                        break;
                    } while (streamReader.Peek() != -1);
                    streamReader.Close(); //继承自IDisposable接口，需要手动释放资源
                }
            }
            catch (Exception)
            {
                isTrue = false;
            }
            return isTrue;
        }

    }
}