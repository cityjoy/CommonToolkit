using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Xml;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Web.UI;
using System.Net;

namespace  CommonToolKit
{
    /// <summary>
    /// Description:文件上传方法类
    /// 注意:如果需要附加图片水印,需要在WebConfig中添加如下配置
    /// <appSettings><add key="WaterMarkPath" value="路径"/></appSettings>
    /// 如果需要添加小图,需要在WebConfig中添加如下配置,默认为small_
    /// <appSettings><add key="SmallPicPrefix" value="前缀"/></appSettings>
    /// 如果需要文字水印,需要在WebConfig中添加如下配置
    /// <appSettings><add key="WaterMarkText" value="文字内容"/></appSettings>
    /// </summary>
    public class UploadFileHelper
    {
        /// <summary>
        /// 大图子文件夹
        /// </summary>
        private string _bigChildPath = "b";
        /// <summary>
        /// 小图子文件夹
        /// </summary>
        private string _smallChildPath = "s";
        /// <summary>
        /// 上传文件控件
        /// </summary>
        private System.Web.UI.HtmlControls.HtmlInputFile _inputFile = null;
        /// <summary>
        /// 上传文件控件--Value
        /// </summary>
        private string _inputFileValue = string.Empty;
        /// <summary>
        /// 上传文件控件--FileName(文件名)
        /// </summary>
        private string _inputFileName = string.Empty;
        /// <summary>
        /// 上传文件控件--ContentLength(文件大小)
        /// </summary>
        private int _inputFileContentLength = 0;
        /// <summary>
        /// 水印图片文件
        /// </summary>
        private string _waterMarkImgPath = string.Empty;
        /// <summary>
        /// 网站根目录
        /// </summary>
        private string _webPath = System.Web.HttpContext.Current.Server.MapPath("~/");
        public UploadFileHelper()
        {
            //默认上传路径
            this.Path = System.Web.HttpContext.Current.Server.MapPath("~/");
            //水印图片路径
            this._waterMarkImgPath = Path + "/" + WebConfigManage.GetAppSetting("WaterMarkPath");

            //允许上传的文件类型
            this.AllowFileType = "jpg|gif|bmp|ico|png";
            //传文件的大小,默认2048KB(2MB)
            this.FileSize = 2048;
            //当前时间用作文件名(年,月,日,时,分,秒,3位毫秒)
            this.NewFileName = DateTime.Now.ToString("yMdhhmmssfff");
            //默认不生成小图
            this.SmallPic = false;
            //默认生成小图片最大的长度
            this.MaxWith = 140;
            //默认生成小图片最大的高度度
            this.MaxHeight = 140;
            //默认可以不上传文件
            this.PicNotNull = false;
            string prefix = WebConfigManage.GetAppSetting("SmallPicPrefix");//小图前缀
            //默认图片前缀
            this.SmallPicPrefix = prefix != string.Empty ? prefix : "small_";
            //默认不需要水印
            this.IsWaterMark = false;
            //默认水印类型
            this.WMType = WaterMarkType.WM_IMAGE;
            //水印位置,默认为右下
            this.WMLocation = WaterMarkLocation.WM_BOTTOM_RIGHT;
            //水印文字
            this.WaterMarkText = WebConfigManage.GetAppSetting("WaterMarkText");//水印文字
        }
        #region 公共属性
        /// <summary>
        /// 上传路径 默认:网站根目录
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 是否必须上传图片 已否决
        /// </summary>
        public bool PicNotNull { get; set; }
        ///<summary>
        /// 上传文件的类型,默认:jpg|gif|bmp|ico|png  注:*代表所有文件
        ///</summary>
        public string AllowFileType { get; set; }
        ///<summary>
        /// 新的文件名 默认:年 月 日 时 分 秒 毫秒
        ///</summary>
        public string NewFileName { get; set; }
        ///<summary>
        /// 上传小图片最大宽度 默认:140
        ///</summary>
        public int MaxWith { get; set; }
        ///<summary>
        /// 上传小图片最大高度度 默认:140
        ///</summary>
        public int MaxHeight { get; set; }
        ///<summary>
        /// 是否需要小图  已否决
        ///</summary>
        public bool SmallPic { get; set; }
        /// <summary>
        /// 小图前缀(默认为small_，需要小图时生效)
        /// </summary>
        private string SmallPicPrefix { get; set; }
        /// <summary>
        /// 上传的文件大小 默认:2048KB
        /// </summary>
        public long FileSize { get; set; }
        /// <summary>
        /// 是否添加水印
        /// </summary>
        public bool IsWaterMark { get; set; }
        /// <summary>
        /// 水印类型
        /// </summary>
        public WaterMarkType WMType { get; set; }
        /// <summary>
        /// 水印位置
        /// </summary>
        public WaterMarkLocation WMLocation { get; set; }
        /// <summary>
        /// 文字水印
        /// </summary>
        private string WaterMarkText { get; set; }

        #endregion
        #region 上传
        //根据 mime 类型，返回编码器
        private System.Drawing.Imaging.ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            System.Drawing.Imaging.ImageCodecInfo result = null;

            //检索已安装的图像编码解码器的相关信息。
            System.Drawing.Imaging.ImageCodecInfo[] encoders =
                System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders();
            for (int i = 0; i < encoders.Length; i++)
            {
                if (encoders[i].MimeType == mimeType)
                {
                    result = encoders[i];
                    break;
                }
            }
            return result;
        }
        /// <summary>
        /// 开始上传
        /// </summary>
        /// <param name="inputFile">HtmlInputFile对象</param>
        /// <param name="state">上传状态.  0:上传成功.  1:没有选择要上传的文件.  2:上传文件类型不符.   3:上传文件过大  -1:应用程序错误.</param>
        /// <returns>文件名</returns>
        public string SaveFile(System.Web.UI.HtmlControls.HtmlInputFile inputFile, ref int state)
        {
            this.SetInputFileInfo(inputFile);
            return this.Save(string.Empty, string.Empty, ref state);
        }
        /// <summary>
        /// 开始上传
        /// </summary>
        /// <param name="inputFile">HtmlInputFile对象</param>
        /// <param name="savePath">要保存的路径</param>
        /// <returns>文件名</returns>
        public string SaveFile(System.Web.UI.HtmlControls.HtmlInputFile inputFile, FileFolderKey fileFolderKey)
        {
            int state = 0;
            this.SetInputFileInfo(inputFile);
            return this.Save(fileFolderKey.ToString(), string.Empty, ref state);
        }
        /// <summary>
        /// 开始上传
        /// </summary>
        /// <param name="inputFile">HtmlInputFile对象</param>
        /// <param name="savePath">要保存的路径</param>
        /// <param name="state">上传状态.  0:上传成功.  1:没有选择要上传的文件.  2:上传文件类型不符.   3:上传文件过大  -1:应用程序错误.</param>
        /// <returns>文件名</returns>
        public string SaveFile(System.Web.UI.HtmlControls.HtmlInputFile inputFile, FileFolderKey fileFolderKey, ref int state)
        {
            this.SetInputFileInfo(inputFile);
            return this.Save(fileFolderKey.ToString(), string.Empty, ref state);
        }
        /// <summary>
        /// 开始上传
        /// </summary>
        /// <param name="inputFile">HtmlInputFile对象</param>
        /// <param name="savePath">要保存的路径</param>
        /// <param name="state">上传状态.  0:上传成功.  1:没有选择要上传的文件.  2:上传文件类型不符.   3:上传文件过大  -1:应用程序错误.</param>
        /// <returns>文件名</returns>
        public string SaveFile(System.Web.UI.HtmlControls.HtmlInputFile inputFile, string savePath, ref int state)
        {
            this.SetInputFileInfo(inputFile);
            return this.Save(savePath, string.Empty, ref state);
        }
        /// <summary>
        /// 开始上传
        /// </summary>
        /// <param name="inputFile">HtmlInputFile对象</param>
        /// <param name="savePath">要保存的路径</param>
        /// <returns>文件名</returns>
        public string SaveFile(System.Web.UI.HtmlControls.HtmlInputFile inputFile, string savePath)
        {
            this.SetInputFileInfo(inputFile);
            int state = 0;
            return this.Save(savePath, string.Empty, ref state);
        }
        /// <summary>
        /// 开始上传
        /// </summary>
        /// <param name="inputFile">HtmlInputFile对象</param>
        /// <param name="savePath">要保存的路径</param>
        /// <param name="oldFileName">旧文件名称,便于删除(注:如果存在文件夹路径,程序将自动去除,只留下文件名)</param>
        /// <returns>文件名</returns>
        public string SaveFile(System.Web.UI.HtmlControls.HtmlInputFile inputFile, FileFolderKey fileFolderKey, string oldFileName)
        {
            int state = 0;
            oldFileName = this.ClearFileFolder(oldFileName);
            this.SetInputFileInfo(inputFile);
            return this.Save(fileFolderKey.ToString(), oldFileName, ref state);
        }
        /// <summary>
        /// 开始上传
        /// </summary>
        /// <param name="inputFile">HtmlInputFile对象</param>
        /// <param name="savePath">要保存的路径</param>
        /// <param name="oldFileName">旧文件名称,便于删除(注:如果存在文件夹路径,程序将自动去除,只留下文件名)</param>
        /// <param name="state">上传状态.  0:上传成功.  1:没有选择要上传的文件.  2:上传文件类型不符.   3:上传文件过大  -1:应用程序错误.</param>
        /// <returns>文件名</returns>
        public string SaveFile(System.Web.UI.HtmlControls.HtmlInputFile inputFile, string savePath, string oldFileName, ref int state)
        {
            oldFileName = this.ClearFileFolder(oldFileName);
            this.SetInputFileInfo(inputFile);
            return this.Save(savePath, oldFileName, ref state);
        }
        /// <summary>
        /// 开始上传
        /// </summary>
        /// <param name="inputFile">HtmlInputFile对象</param>
        /// <param name="savePath">要保存的路径</param>
        /// <param name="oldFileName">旧文件名称,便于删除(注:如果存在文件夹路径,程序将自动去除,只留下文件名)</param>
        /// <param name="state">上传状态.  0:上传成功.  1:没有选择要上传的文件.  2:上传文件类型不符.   3:上传文件过大  -1:应用程序错误.</param>
        /// <returns>文件名</returns>
        public string SaveFile(System.Web.UI.HtmlControls.HtmlInputFile inputFile, FileFolderKey fileFolderKey, string oldFileName, ref int state)
        {
            oldFileName = this.ClearFileFolder(oldFileName);
            this.SetInputFileInfo(inputFile);
            return this.Save(fileFolderKey.ToString(), oldFileName, ref state);
        }

        /// <summary>
        /// 为HtmlInputFile控件设置值
        /// </summary>
        /// <param name="inputFile">HtmlInputFile对象</param>
        private void SetInputFileInfo(System.Web.UI.HtmlControls.HtmlInputFile inputFile)
        {
            this._inputFile = inputFile;
            this._inputFileContentLength = inputFile.PostedFile.ContentLength;
            this._inputFileName = inputFile.PostedFile.FileName;
            this._inputFileValue = inputFile.Value;
        }
        /// <summary>
        /// 去除文件名中的文件夹名
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        private string ClearFileFolder(string fileName)
        {
            fileName = fileName.Trim().Replace("//", "/").Replace("\\", "/");
            if (fileName != string.Empty && fileName.IndexOf(".") != -1 && fileName.IndexOf("/") != -1)
            {
                fileName = fileName.Substring(fileName.LastIndexOf("/") + 1, fileName.Length - fileName.LastIndexOf("/") - 1);
            }
            return fileName;
        }
        /// <summary>
        /// 上传核心
        /// 返回文件名或上传状态
        /// </summary>
        /// <param name="inputFile">上传控件</param>
        /// <param name="savePath">存放的文件夹   从网站根目录起</param>
        /// <param name="oldFileName">在修改图片时指定旧图片名以便删除   默认为空字符串</param>
        /// <returns>返回上传状态.  0:上传成功.  1:没有选择要上传的文件.  2:上传文件类型不符.   3:上传文件过大  -1:应用程序错误.</returns>
        private string Save(string savePath, string oldFileName, ref int state)
        {
            try
            {
                #region 判断是否已选择文件  如果没选择返回旧图片名
                //_inputFileValue为空   表示没有选择文件   返回状态1
                if (this._inputFileValue == string.Empty)
                {
                    if (this.PicNotNull)
                    {
                        MessageBox.Show(this._inputFile, "请您先选择要上传的图片!");
                    }
                    //设置状态为没有选择图片
                    state = 1;
                    //返回旧图片名
                    return oldFileName;
                }
                #endregion
                #region 判断类型是否符合
                //文件扩展名的小些副本
                string extension = System.IO.Path.GetExtension(this._inputFileName).ToLower();
                //将允许上传的类型转为小写字符
                this.AllowFileType = this.AllowFileType.ToLower();
                //讲允许上传的文件扩展名放入数组
                string[] fileTypeList = this.AllowFileType.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                //是否找到可用类型
                bool isOk = false;
                //判断是否有可上传的类型  *表示任意类型
                foreach (string item in fileTypeList)
                {
                    if (item == "*" || item == extension.Replace(".", "")) { isOk = true; break; }
                }
                //没找到可用类型,设置状态为2,返回旧文件名
                if (!isOk)
                {
                    MessageBox.Show(string.Format("只能上传{0}类型的文件", this.AllowFileType.Replace("|", ",")));
                    state = 2;
                    return oldFileName;
                }
                #endregion
                #region 判断文件是否过大
                //文件过大,设置状态为3,返回旧文件名
                if (this._inputFileContentLength > this.FileSize * 1024)
                {
                    MessageBox.Show(string.Format("上传文件大小不可大于{0}KB", this.FileSize));
                    state = 3;
                    return oldFileName;
                }
                #endregion
                #region 检查目录是否存在并执行上传
                DirectoryInfo dir = new DirectoryInfo(this.Path + savePath + "/" + this._bigChildPath);
                //判断大图存放目录是否存在   不存在则创建
                if (!dir.Exists) { dir.Create(); }
                if (this.SmallPic)
                {
                    dir = new DirectoryInfo(this.Path + savePath + "/" + this._smallChildPath);
                    //判断小图存放目录是否存在   不存在则创建
                    if (!dir.Exists) { dir.Create(); }
                }
                #endregion
                #region 开始上传
                //存放文件路径+文件名
                string filePath = string.Format("{0}{1}/{2}/{3}{4}", this.Path, savePath, this._bigChildPath, this.NewFileName, extension);
                //判断是否需要水印
                if (IsWaterMark)
                {
                    string tempPath = string.Format("{0}/temp/{1}{2}", this.Path, this.NewFileName, extension);
                    if (this._inputFile != null) { this._inputFile.PostedFile.SaveAs(tempPath); }
                    //else { this.StreamSave(this._inputFileValue, tempPath); }
                    //开始上传文件
                    this.addWaterMark(tempPath, filePath);
                }
                else
                {
                    //开始上传文件
                    if (this._inputFile != null) { this._inputFile.PostedFile.SaveAs(filePath); }
                    //else { this.StreamSave(this._inputFileValue, filePath); }
                }
                #region 上传小图
                if (SmallPic)
                {
                    string newFilePath = string.Format("{0}{1}/{2}/{3}{4}", this.Path, savePath, this._smallChildPath, this.NewFileName, extension);
                    this.GreateMiniImage(filePath, newFilePath, this.MaxWith, this.MaxHeight);
                }
                #endregion
                #endregion
                #region 删除图片
                if (oldFileName.Trim() != string.Empty)
                {
                    this.Delete(savePath, oldFileName, true);
                }
                #endregion
                //设置状态为0,表示上传成功
                state = 0;
                //返回文件名
                return string.Format("{0}{1}", this.NewFileName, extension);
            }
            catch (Exception e)
            {
                //发生严重错误,设置状态为-1,返回旧文件名
                MessageBox.Show("应用程序错误,请与管理员联系,详细信息如下:" + e.Message);
                state = -1;
                return oldFileName;
            }
        }

        ///// <summary>
        ///// 以二进制流保存图片
        ///// </summary>
        ///// <param name="clientFilePath">客户端路径</param>
        ///// <param name="serverFilePath">服务器端路径</param>
        //private void StreamSave(string clientFilePath,string serverFilePath)
        //{
        //    WebClient wc = new WebClient();
        //    wc.Credentials = CredentialCache.DefaultCredentials;
        //    System.IO.FileStream fs = new System.IO.FileStream(clientFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
        //    BinaryReader br = new BinaryReader(fs);
        //    byte[] fcount = br.ReadBytes((int)fs.Length);
        //    Stream stre = wc.OpenWrite(serverFilePath, "PUT");
        //    stre.Write(fcount, 0, fcount.Length);
        //    stre.Close();
        //    fs.Dispose();
        //    stre.Dispose();
        //}

        #endregion
        #region 删除文件
        /// <summary>
        /// 删除指定文件
        /// </summary>
        /// <param name="folder">文件夹路径</param>
        /// <param name="fileName">文件名</param>
        /// <param name="delSmallPic">是否删除小图</param>
        public void Delete(string folder, string fileName, bool delSmallPic)
        {
            string delPath = string.Format("{0}/{1}", folder, this._bigChildPath);
            this.Del(delPath, fileName);
            if (delSmallPic)
            {
                delPath = string.Format("{0}/{1}", folder, this._smallChildPath);
                this.Del(delPath, fileName);//删除小图
            }
        }
        /// <summary>
        /// 删除指定文件
        /// </summary>
        /// <param name="fileFolderKey">文件夹名</param>
        /// <param name="fileName">文件名</param>
        /// <param name="delSmallPic">是否删除小图</param>
        public void Delete(FileFolderKey fileFolderKey, string fileName, bool delSmallPic)
        {
            string folder = GetConfigString(fileFolderKey.ToString());
            folder = folder == null ? "other" : folder;//没有找到则默认为other文件夹
            string delPath = string.Format("{0}/{1}", folder, this._bigChildPath);
            this.Del(delPath, fileName);
            if (delSmallPic)
            {
                delPath = string.Format("{0}/{1}", folder, this._smallChildPath);
                this.Del(delPath, fileName);//删除小图
            }

        }
        /// <summary>
        /// 删除指定文件
        /// </summary>
        /// <param name="fileFolderKey">文件夹名</param>
        /// <param name="childFolderName">子文件夹路径</param>
        /// <param name="fileName">文件名</param>
        /// <param name="delSmallPic">是否删除小图</param>
        public void Delete(FileFolderKey fileFolderKey, string childFolderName, string fileName, bool delSmallPic)
        {
            string folder = GetConfigString(fileFolderKey.ToString());
            folder = folder == null ? "other" : folder;//没有找到则默认为other文件夹
            folder = string.Format("{0}/{1}", folder, childFolderName);//添加上子文件夹
            string delPath = string.Format("{0}/{1}", folder, this._bigChildPath);
            this.Del(delPath, fileName);
            if (delSmallPic)
            {
                delPath = string.Format("{0}/{1}", folder, this._smallChildPath);
                this.Del(delPath, fileName);//删除小图
            }

        }
        private int deleteCount = 0;
        /// <summary>
        /// 删除核心代码
        /// </summary>
        /// <param name="path"></param>
        private void Del(string folder, string fileName)
        {
            fileName = this.ClearFileFolder(fileName);
            string delPath = System.Web.HttpContext.Current.Server.MapPath("~/");
            string delPicPath = string.Empty;

            try
            {
                delPicPath = string.Format("{0}/{1}/{2}", delPath, folder, fileName);
                if (File.Exists(delPicPath))//如果该文件存在，则删除
                {
                    deleteCount++;
                    File.Delete(delPicPath);
                }
            }
            catch
            {
                if (deleteCount < 100)//为避免因资源被占用删除不了数据   所以在此循环100次
                    Del(folder, fileName);
            }
        }
        #endregion
        #region 获取AppSettings相应配置信息
        /// <summary>
        /// 获取上传文件文件夹名   如果在web.Config文件中没找到相应信息  则默认存放到网站根目录的other文件夹
        /// </summary>
        /// <param name="fileFolderKey">指定存放的文件夹</param>
        /// <returns></returns>
        public static string GetFolder(FileFolderKey fileFolderKey)
        {
            string path = GetConfigString(fileFolderKey.ToString());
            if (path == null) { return "other"; }
            return path;
        }

        /// <summary>
        /// 得到AppSettings中图片(pic)的配置字符串信息
        /// </summary>
        public static string AppPic
        {
            get { return GetFolder(FileFolderKey.pic); }
        }
        /// <summary>
        /// 得到AppSettings中图片(picture)的配置字符串信息
        /// </summary>
        public static string AppPicture
        {
            get { return GetFolder(FileFolderKey.picture); }
        }
        /// <summary>
        /// 得到AppSettings中图片(image)的配置字符串信息
        /// </summary>
        public static string AppImage
        {
            get { return GetFolder(FileFolderKey.image); }
        }
        /// <summary>
        /// 得到AppSettings中文件(file)的配置字符串信息
        /// </summary>
        public static string AppFile
        {
            get { return GetFolder(FileFolderKey.file); }
        }
        /// <summary>
        /// 得到AppSettings中文件(travelPic)的配置字符串信息
        /// </summary>
        public static string AppTravelPic
        {
            get { return GetFolder(FileFolderKey.travelPic); }
        }
        /// <summary>
        /// 得到AppSettings中的配置字符串信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string GetConfigString(string key)
        {
            string CacheKey = "AppSettings-" + key;
            object objModel = HttpRuntimeCache.Get(CacheKey);
            if (objModel == null)
            {
                try
                {
                    objModel = ConfigurationManager.AppSettings[key];
                    if (objModel != null)
                    {
                        HttpRuntimeCache.SetCache(CacheKey, objModel, DateTime.Now.AddMinutes(180), TimeSpan.Zero);
                    }
                }
                catch
                { }
            }
            return objModel.ToString();
        }
        #endregion
        #region 其它
        /// <summary>
        /// 删除指定文件夹中所有文件
        /// </summary>
        /// <param name="directoryPath"></param>
        public void DeleteByDirectory(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
            {
                DirectoryInfo dir = Directory.CreateDirectory(directoryPath);
                foreach (FileInfo fileInfo in dir.GetFiles())
                {
                    fileInfo.Delete();
                }
                dir.Delete(false);
            }
        }
        /// <summary>
        /// 删除指定文件夹中所有文件
        /// </summary>
        /// <param name="directoryPath"></param>
        public void DeleteByDirectory(FileFolderKey fileFolderKey, string directoryPath)
        {
            string folder = GetConfigString(fileFolderKey.ToString());
            directoryPath = string.Format("{0}/{1}/{2}", this.Path, folder, directoryPath);
            if (Directory.Exists(directoryPath))
            {
                DirectoryInfo dir = Directory.CreateDirectory(directoryPath);
                foreach (FileInfo fileInfo in dir.GetFiles())
                {
                    fileInfo.Delete();
                }
                dir.Delete(false);
            }
        }
        #endregion
        #region 添加水印
        /// <summary>
        /// 添加图片水印
        /// </summary>
        /// <param name="oldpath">原图片绝对地址</param>
        /// <param name="newpath">新图片放置的绝对地址</param>
        public void addWaterMark(string oldpath, string newpath)
        {
            try
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(oldpath);
                Bitmap b = new Bitmap(image.Width, image.Height, PixelFormat.Format24bppRgb);
                Graphics g = Graphics.FromImage(b);
                g.Clear(Color.White);
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.High;

                g.DrawImage(image, 0, 0, image.Width, image.Height);

                if (IsWaterMark)
                {
                    switch (this.WMType)
                    {

                        case WaterMarkType.WM_IMAGE: //水印图片
                            this.addWatermarkImage(g, _waterMarkImgPath, WMLocation, image.Width, image.Height);
                            break;

                        case WaterMarkType.WM_TEXT://水印文字
                            this.addWatermarkText(g, this.WaterMarkText, WMLocation, image.Width, image.Height);
                            break;
                    }

                    #region 降低图片质量
                    System.Drawing.Imaging.ImageCodecInfo encoder = GetEncoderInfo("image/jpeg");
                    System.Drawing.Imaging.EncoderParameters encoderParams = new System.Drawing.Imaging.EncoderParameters(1);
                    encoderParams.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)80);
                    #endregion
                    b.Save(newpath, encoder, encoderParams);
                    encoderParams.Dispose();
                    b.Dispose();
                    image.Dispose();
                }
            }
            catch
            {
                if (File.Exists(oldpath))
                {
                    File.Delete(oldpath);
                }
            }
            finally
            {
                if (File.Exists(oldpath))
                {
                    File.Delete(oldpath);
                }
            }
        }

        /// <summary>
        ///  加水印图片
        /// </summary>
        /// <param name="picture">imge 对象</param>
        /// <param name="WaterMarkPicPath">水印图片的地址</param>
        /// <param name="_watermarkPosition">水印位置</param>
        /// <param name="_width">被加水印图片的宽</param>
        /// <param name="_height">被加水印图片的高</param>
        private void addWatermarkImage(Graphics picture, string WaterMarkPicPath, WaterMarkLocation location, int _width, int _height)
        {
            Image watermark = new Bitmap(WaterMarkPicPath);

            ImageAttributes imageAttributes = new ImageAttributes();
            ColorMap colorMap = new ColorMap();

            colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
            colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
            ColorMap[] remapTable = { colorMap };

            imageAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

            float[][] colorMatrixElements = {
                                                 new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
                                                 new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
                                                 new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
                                                 new float[] {0.0f,  0.0f,  0.0f,  0.3f, 0.0f},
                                                 new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
                                             };

            ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);

            //设置透明色
            //imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            int xpos = 0;
            int ypos = 0;
            int WatermarkWidth = 0;
            int WatermarkHeight = 0;
            double bl = 1d;
            //计算水印图片的比率
            //取背景的1/4宽度来比较
            if ((_width > watermark.Width * 4) && (_height > watermark.Height * 4))
            {
                bl = 1;
            }
            else if ((_width > watermark.Width * 4) && (_height < watermark.Height * 4))
            {
                bl = Convert.ToDouble(_height / 4) / Convert.ToDouble(watermark.Height);

            }
            else

                if ((_width < watermark.Width * 4) && (_height > watermark.Height * 4))
                {
                    bl = Convert.ToDouble(_width / 4) / Convert.ToDouble(watermark.Width);
                }
                else
                {
                    if ((_width * watermark.Height) > (_height * watermark.Width))
                    {
                        bl = Convert.ToDouble(_height / 4) / Convert.ToDouble(watermark.Height);

                    }
                    else
                    {
                        bl = Convert.ToDouble(_width / 4) / Convert.ToDouble(watermark.Width);

                    }

                }
            WatermarkWidth = Convert.ToInt32(watermark.Width * bl);
            WatermarkHeight = Convert.ToInt32(watermark.Height * bl);
            switch (location)
            {
                case WaterMarkLocation.WM_TOP_LEFT:
                    xpos = 10;
                    ypos = 10;
                    break;
                case WaterMarkLocation.WM_TOP_RIGHT:
                    xpos = _width - WatermarkWidth - 10;
                    ypos = 10;
                    break;
                case WaterMarkLocation.WM_BOTTOM_RIGHT:
                    xpos = _width - WatermarkWidth - 10;
                    ypos = _height - WatermarkHeight - 10;
                    break;
                case WaterMarkLocation.WM_BOTTOM_LEFT:
                    xpos = 10;
                    ypos = _height - WatermarkHeight - 10;
                    break;
            }

            picture.DrawImage(watermark, new Rectangle(xpos, ypos, WatermarkWidth, WatermarkHeight), 0, 0, watermark.Width, watermark.Height, GraphicsUnit.Pixel, imageAttributes);


            watermark.Dispose();
            imageAttributes.Dispose();
        }
        /// <summary>
        ///  加水印文字
        /// </summary>
        /// <param name="picture">imge 对象</param>
        /// <param name="_watermarkText">水印文字内容</param>
        /// <param name="_watermarkPosition">水印位置</param>
        /// <param name="_width">被加水印图片的宽</param>
        /// <param name="_height">被加水印图片的高</param>
        private void addWatermarkText(Graphics picture, string _watermarkText, WaterMarkLocation location, int _width, int _height)
        {
            int[] sizes = new int[] { 16, 14, 12, 10, 8, 6, 4 };
            Font crFont = null;
            SizeF crSize = new SizeF();
            for (int i = 0; i < 7; i++)
            {
                crFont = new Font("arial", sizes[i], FontStyle.Bold);
                crSize = picture.MeasureString(_watermarkText, crFont);

                if ((ushort)crSize.Width < (ushort)_width)
                    break;
            }
            float xpos = 0;
            float ypos = 0;
            switch (location)
            {
                case WaterMarkLocation.WM_TOP_LEFT:
                    xpos = ((float)_width * (float).01) + (crSize.Width / 2);
                    ypos = (float)_height * (float).01;
                    break;
                case WaterMarkLocation.WM_TOP_RIGHT:
                    xpos = ((float)_width * (float).99) - (crSize.Width / 2);
                    ypos = (float)_height * (float).01;
                    break;
                case WaterMarkLocation.WM_BOTTOM_RIGHT:
                    xpos = ((float)_width * (float).99) - (crSize.Width / 2);
                    ypos = ((float)_height * (float).99) - crSize.Height;
                    break;
                case WaterMarkLocation.WM_BOTTOM_LEFT:
                    xpos = ((float)_width * (float).01) + (crSize.Width / 2);
                    ypos = ((float)_height * (float).99) - crSize.Height;
                    break;
            }

            StringFormat StrFormat = new StringFormat();
            StrFormat.Alignment = StringAlignment.Center;

            SolidBrush semiTransBrush2 = new SolidBrush(Color.FromArgb(153, 0, 0, 0));
            picture.DrawString(_watermarkText, crFont, semiTransBrush2, xpos + 1, ypos + 1, StrFormat);

            SolidBrush semiTransBrush = new SolidBrush(Color.FromArgb(153, 255, 255, 255));
            picture.DrawString(_watermarkText, crFont, semiTransBrush, xpos, ypos, StrFormat);

            semiTransBrush2.Dispose();
            semiTransBrush.Dispose();
        }

        /// <summary>
        /// 图片合成
        /// </summary>
        /// <param name="p1">图片1</param>
        /// <param name="p2">图片2</param>
        /// <param name="path_save">新图片路径</param>
        /// <param name="x_p2">p2位于p1的x坐标</param>
        /// <param name="y_p2">p2位于p1的y坐标</param>

        public static void Compound(string p1, string p2, string path_save,int x_p2,int y_p2)
        {
            int x = 0;
            int y = 0;
            x = x_p2;
            y = y_p2;
            //加载底图
            Image img1 = Image.FromFile(p1);
            Image img2 = Image.FromFile(p2);


            int w = img1.Width + img2.Width;
            int h = img1.Height > img2.Height ? img1.Height : img2.Height;

            //设置画布


            //Bitmap map = new Bitmap(w, h);
            Bitmap map = new Bitmap(229, 121);
            //绘图
            Graphics g = Graphics.FromImage(map);

            //TextureBrush Txbrus = new TextureBrush(img1);
            //Txbrus.WrapMode = System.Drawing.Drawing2D.WrapMode.Tile;
            //g.FillRectangle(Txbrus, new Rectangle(0, 0, img1.Width, img1.Height));
           
            //g.DrawImage(img1, 0, 0, new Rectangle(0, 0,229, 121), GraphicsUnit.Inch);
            ////g.DrawImage(img2, img1.Width, 0, new Rectangle(0, 0, img2.Width, img2.Height), GraphicsUnit.Pixel);
            //g.DrawImage(img2, x, y, new Rectangle(0, 0, 85, 95), GraphicsUnit.Inch);
            ////g.DrawImage(img, 0, 0, new Rectangle(0, 0, width, height), GraphicsUnit.Pixel);

            g.DrawImage(img1, 0, 0, img1.Width, img1.Height);
            g.DrawImage(img2, x, y, img2.Width, img2.Height);

      
            //保存
            map.Save(path_save);
        }

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="oldpath">原图片地址</param>
        /// <param name="newpath">新图片地址</param>
        /// <param name="tWidth">缩略图的宽</param>
        /// <param name="tHeight">缩略图的高</param>
        public static void GreateMiniImageS(string oldpath, string newpath, int tWidth, int tHeight)
        {
            try
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(oldpath);
                #region 原有代码(图片尺寸等比缩放)
                //double bl = 1d;
                //if ((image.Width <= image.Height) && (tWidth >= tHeight))
                //{
                //    bl = Convert.ToDouble(image.Height) / Convert.ToDouble(tHeight);
                //}
                //else if ((image.Width > image.Height) && (tWidth < tHeight))
                //{
                //    bl = Convert.ToDouble(image.Width) / Convert.ToDouble(tWidth);
                //}
                //else
                //    if ((image.Width <= image.Height) && (tWidth <= tHeight))
                //    {
                //        if (image.Height / tHeight >= image.Width / tWidth)
                //        {
                //            bl = Convert.ToDouble(image.Width) / Convert.ToDouble(tWidth);
                //        }
                //        else
                //        {
                //            bl = Convert.ToDouble(image.Height) / Convert.ToDouble(tHeight);
                //        }
                //    }
                //    else
                //    {
                //        if (image.Height / tHeight >= image.Width / tWidth)
                //        {
                //            bl = Convert.ToDouble(image.Height) / Convert.ToDouble(tHeight);
                //        }
                //        else
                //        {
                //            bl = Convert.ToDouble(image.Width) / Convert.ToDouble(tWidth);
                //        }
                //    }
                //Bitmap b = new Bitmap(image, Convert.ToInt32(image.Width / bl), Convert.ToInt32(image.Height / bl));
                #endregion
                #region 2010-03-29 改造
                int imgWidth = image.Width;
                int imgHeight = image.Height;
                int newWidth = 0;
                int newHeight = 0;
                if (imgWidth > tWidth)
                {
                    newWidth = tWidth;
                    newHeight = tWidth * imgHeight / imgWidth;
                    if (newHeight > tHeight)
                    {
                        newWidth = tHeight * newWidth / newHeight;
                        newHeight = tHeight;
                    }
                }
                else if (imgHeight > tHeight)
                {
                    newHeight = tHeight;
                    newWidth = tHeight * imgWidth / imgHeight;
                    if (newWidth > tWidth)
                    {
                        newHeight = tWidth * newHeight / newWidth;
                        newWidth = tWidth;
                    }
                }
                else
                {
                    newWidth = imgWidth;
                    newHeight = imgHeight;
                }
                Bitmap b = new Bitmap(image, newWidth, newHeight);
                #endregion
                b.Save(newpath);
                b.Dispose();
                image.Dispose();
            }
            catch
            {
            }
        }
        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="oldpath">原图片地址</param>
        /// <param name="newpath">新图片地址</param>
        /// <param name="tWidth">缩略图的宽</param>
        /// <param name="tHeight">缩略图的高</param>
        public void GreateMiniImage(string oldpath, string newpath, int tWidth, int tHeight)
        {
            try
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(oldpath);
                #region 原有代码(图片尺寸等比缩放)
                //double bl = 1d;
                //if ((image.Width <= image.Height) && (tWidth >= tHeight))
                //{
                //    bl = Convert.ToDouble(image.Height) / Convert.ToDouble(tHeight);
                //}
                //else if ((image.Width > image.Height) && (tWidth < tHeight))
                //{
                //    bl = Convert.ToDouble(image.Width) / Convert.ToDouble(tWidth);
                //}
                //else
                //    if ((image.Width <= image.Height) && (tWidth <= tHeight))
                //    {
                //        if (image.Height / tHeight >= image.Width / tWidth)
                //        {
                //            bl = Convert.ToDouble(image.Width) / Convert.ToDouble(tWidth);
                //        }
                //        else
                //        {
                //            bl = Convert.ToDouble(image.Height) / Convert.ToDouble(tHeight);
                //        }
                //    }
                //    else
                //    {
                //        if (image.Height / tHeight >= image.Width / tWidth)
                //        {
                //            bl = Convert.ToDouble(image.Height) / Convert.ToDouble(tHeight);
                //        }
                //        else
                //        {
                //            bl = Convert.ToDouble(image.Width) / Convert.ToDouble(tWidth);
                //        }
                //    }
                //Bitmap b = new Bitmap(image, Convert.ToInt32(image.Width / bl), Convert.ToInt32(image.Height / bl));
                #endregion
                #region 2010-03-29 改造
                int imgWidth = image.Width;
                int imgHeight = image.Height;
                int newWidth = 0;
                int newHeight = 0;
                if (imgWidth > tWidth)
                {
                    newWidth = tWidth;
                    newHeight = tWidth * imgHeight / imgWidth;
                    if (newHeight > tHeight)
                    {
                        newWidth = tHeight * newWidth / newHeight;
                        newHeight = tHeight;
                    }
                }
                else if (imgHeight > tHeight)
                {
                    newHeight = tHeight;
                    newWidth = tHeight * imgWidth / imgHeight;
                    if (newWidth > tWidth)
                    {
                        newHeight = tWidth * newHeight / newWidth;
                        newWidth = tWidth;
                    }
                }
                else
                {
                    newWidth = imgWidth;
                    newHeight = imgHeight;
                }
                Bitmap b = new Bitmap(image, newWidth, newHeight);
                #endregion
                b.Save(newpath);
                b.Dispose();
                image.Dispose();
            }
            catch
            {
            }
        }
        #endregion
        public static string GetSmallPicName(string picName)
        {
            return WebConfigManage.GetAppSetting("WaterMarkPath") + picName;
        }
    }

    /// <summary>
    /// 文件存放的文件夹
    /// </summary>
    public enum FileFolderKey
    {
        /// <summary>
        /// 在webConfig中appSettings节点下添加  add key="pic" value="文件夹名称"
        /// </summary>
        pic,
        /// <summary>
        /// 在webConfig中 appSettings 节点下添加 add key="image" value="文件夹名称"
        /// </summary>
        image,
        /// <summary>
        /// 在webConfig中 appSettings 节点下添加 add key="picture" value="文件夹名称"
        /// </summary>
        picture,
        /// <summary>
        /// 在webConfig中 appSettings 节点下添加 add key="file" value="文件夹名称"
        /// </summary>
        file,
        /// <summary>
        /// 旅游图片   在webConfig中 appSettings 节点下添加 add key="file" value="文件夹名称"
        /// </summary>
        travelPic
    }
    /// <summary>
    /// 水印类型
    /// </summary>
    public enum WaterMarkType
    {
        /// <summary>
        /// 图片水印
        /// </summary>
        WM_IMAGE,
        /// <summary>
        /// 文字水印
        /// </summary>
        WM_TEXT
    }
    /// <summary>
    /// 水印位置
    /// </summary>
    public enum WaterMarkLocation
    {
        /// <summary>
        /// 左上
        /// </summary>
        WM_TOP_LEFT,
        /// <summary>
        /// 右上
        /// </summary>
        WM_TOP_RIGHT,
        /// <summary>
        /// 左下
        /// </summary>
        WM_BOTTOM_LEFT,
        /// <summary>
        /// 右下
        /// </summary>
        WM_BOTTOM_RIGHT
    }
}