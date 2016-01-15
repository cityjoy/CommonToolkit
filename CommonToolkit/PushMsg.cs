using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Notification;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.Windows.Threading;

namespace ZP.Project.Toolkit
{
    public class PushMsg
    {
        #region 变量与常量
        private HttpNotificationChannel httpChannel;
        private const string channelName = "PushMsg";
        private const string saveDataName = "PushMsgSettings.dat";
        #endregion

        #region 事件委托
        public delegate void RawMsgCallbackEvent(object sender, HttpNotificationEventArgs args);
        /// <summary>
        /// 接收到Raw通知时触发
        /// </summary>
        public event RawMsgCallbackEvent RawMsgCallback;

        public delegate void TostOrTitleMsgCallbackEvent(object sender, NotificationEventArgs args);
        /// <summary>
        /// 接收到TostOrTitle通知时触发
        /// </summary>
        public event TostOrTitleMsgCallbackEvent TostOrTitleMsgCallback;

        public delegate void GetChannelUriSuccessCallbackEvent(string channelUri);
        /// <summary>
        /// 如果微软服务器成功给手机分配的Uri的话，则返回Uri
        /// </summary>
        public event GetChannelUriSuccessCallbackEvent GetChannelUriSuccessCallback;
        #endregion

        #region 相关逻辑代码

        public void Open()
        {
            if (!TryFindChannel())
            {
                DoConnect();
            }
        }

        private void DoConnect()
        {
            try
            {
                //首先查看现有的频道
                httpChannel = HttpNotificationChannel.Find(channelName);
                //如果频道存在
                if (httpChannel != null)
                {
                    //注册Microsoft推送通知事件
                    SubscribeToChannelEvents();
                    //检测Microsoft通知服务注册状态
                    SubscribeToService();
                    //订阅Toast和Title通知
                    SubscribeToNotifications();
                }
                else
                {
                    //试图创建一个新的频道
                    //创建频道
                    httpChannel = new HttpNotificationChannel(channelName, "PuzzleService");
                    //推送通知频道创建成功
                    SubscribeToChannelEvents();
                    //注册Microsoft推送通知事件
                    httpChannel.Open();
                }
            }
            catch (Exception ex)
            {
                //创建或恢复频道时发生异常
            }
        }

        private void ParseRAWPayload(Stream e, out string content)
        {
            XDocument document;
            using (var reader = new StreamReader(e))
            {
                string payload = reader.ReadToEnd().Replace('\0', ' ');
                document = XDocument.Parse(payload);
            }
            content = (from c in document.Descendants("PuzzleMsg")
                       select c.Element("content").Value).FirstOrDefault();
        }
        #endregion

        #region 订阅Microsoft推送通知
        /// <summary>
        /// 注册Microsoft推送通知事件
        /// </summary>
        private void SubscribeToChannelEvents()
        {
            //如果微软服务器成功给手机分配的Uri的话，则返回Uri给手机
            httpChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(httpChannel_ChannelUriUpdated);
            //接到微软服务器Raw通知消息
            httpChannel.HttpNotificationReceived += new EventHandler<HttpNotificationEventArgs>(httpChannel_HttpNotificationReceived);
            //推送通知发生错误
            httpChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(httpChannel_ExceptionOccurred);
            //接到微软服务器Toast通知消息
            httpChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(httpChannel_ShellToastNotificationReceived);
        }

        /// <summary>
        /// 检测Microsoft通知服务注册状态
        /// </summary>
        private void SubscribeToService()
        {
            //Microsoft通知服务Url
            Uri theUri = new Uri(string.Format("{0}?uri={1}", SystemConfig.Instance.NotificationsRegisterUrl, httpChannel.ChannelUri.ToString()));
            WebClient client = new WebClient();
            client.DownloadStringCompleted += (s, e) =>
            {
                if (e.Error == null)
                {
                    //注册成功
                }
                else
                {
                    //注册失败
                }
            };
            client.DownloadStringAsync(theUri);
        }

        /// <summary>
        /// 订阅Toast和Title通知
        /// </summary>
        private void SubscribeToNotifications()
        {
            //////////////////////////////////////////
            // 绑定到Toast通知 
            //////////////////////////////////////////
            try
            {
                if (httpChannel.IsShellToastBound == true)
                {
                    //已注册Toast通知
                }
                else
                {
                    //开始注册Toast通知
                    httpChannel.BindToShellToast();
                }
            }
            catch (Exception ex)
            {
                //注册Toast通知发生异常
            }

            //////////////////////////////////////////
            // 注册Tile通知
            //////////////////////////////////////////
            try
            {
                if (httpChannel.IsShellTileBound == true)
                {
                    //已注册Title通知
                }
                else
                {
                    //开始注册Title通知
                    //可以注册手机应用程序从远程服务器接收的图像[这是可选的]
                    Collection<Uri> uris = new Collection<Uri>();
                    uris.Add(new Uri("http://zp366.com"));
                    httpChannel.BindToShellTile(uris);
                }
            }
            catch (Exception ex)
            {
                //注册Title通知发生异常
            }
        }
        #endregion

        #region 频道事件处理
        void httpChannel_ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {
            //频道已打开. Got Uri: httpChannel.ChannelUri.ToString()
            this.SaveChannelInfo();
            //订阅频道事件
            SubscribeToService();
            //订阅Toast和Title通知
            SubscribeToNotifications();

            if (this.GetChannelUriSuccessCallback != null)
            {
                this.GetChannelUriSuccessCallback(e.ChannelUri.ToString());
            }
        }

        void httpChannel_ExceptionOccurred(object sender, NotificationChannelErrorEventArgs e)
        {
            //出现异常
        }



        void httpChannel_HttpNotificationReceived(object sender, HttpNotificationEventArgs e)
        {
            //接收到RAW通知
            if (this.RawMsgCallback != null)
            {
                this.RawMsgCallback(sender, e);
            }
        }


        void httpChannel_ShellToastNotificationReceived(object sender, NotificationEventArgs e)
        {
            //接收到Toast或Tile消息
            if (this.TostOrTitleMsgCallback != null)
            {
                this.TostOrTitleMsgCallback(sender, e);
            }
        }
        #endregion

        #region 加载或保存频道信息
        private bool TryFindChannel()
        {
            bool bRes = false;

            //当前应用程序的独立存储空间
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                //检查保存的文件是否存在
                if (isf.FileExists(saveDataName))
                {
                    //频道数据存在，正在载入...
                    using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(saveDataName, FileMode.Open, isf))
                    {
                        using (StreamReader sr = new StreamReader(isfs))
                        {
                            string uri = sr.ReadLine();
                            //查询以前常见的通知频道
                            httpChannel = HttpNotificationChannel.Find(channelName);
                            if (httpChannel != null)
                            {
                                //如果当前活动的通知频道URL与以前的相同
                                if (httpChannel.ChannelUri.ToString() == uri)
                                {
                                    SubscribeToChannelEvents();
                                    SubscribeToService();
                                    bRes = true;

                                    if (this.GetChannelUriSuccessCallback != null)
                                    {
                                        this.GetChannelUriSuccessCallback(httpChannel.ChannelUri.ToString());
                                    }

                                }
                                sr.Close();
                            }
                        }
                    }
                }
                else
                {
                    //未找到频道数据
                }
            }
            return bRes;
        }

        /// <summary>
        /// 保存频道数据信息（频道消息uri）
        /// </summary>
        private void SaveChannelInfo()
        {
            //当前应用程序的独立存储空间
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                //创建数据文件
                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(saveDataName, FileMode.Create, isf))
                {
                    using (StreamWriter sw = new StreamWriter(isfs))
                    {
                        //正在保存数据频道...
                        sw.WriteLine(httpChannel.ChannelUri.ToString());
                        sw.Close();
                        //保存完成
                    }
                }
            }
        }
        #endregion
    }
}
