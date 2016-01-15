using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace  CommonToolKit
{
    public class LoggerHelper : IDisposable
    {
        private bool disposed;
        private DateTime timeSign;
        private string path = "Logs";
        private StreamWriter streamWriter;
        private LogTypes logType = LogTypes.Daily;
        private System.Timers.Timer timer = new System.Timers.Timer(2000);
        private Queue<LogMessage> messageQueue = new Queue<LogMessage>();

        /// <summary>
        /// 日志类型
        /// </summary>
        public LogTypes LogType
        {
            get { return logType; }
            set { logType = value; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public LoggerHelper(string path)
        {
            if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(Path.GetDirectoryName(path)))
            {
                this.path = path;
            }
            this.timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            this.timer.Start();
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~LoggerHelper()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 清除资源
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            ClearLogFile();
            if (!disposed)
            {
                if (timer != null)
                {
                    timer.Stop();
                }
                if (streamWriter != null)
                {
                    streamWriter.Close();
                }
                disposed = true;
            }
        }

        /// <summary>
        /// 计时器写日志
        /// </summary>
        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (messageQueue.Count > 0)
            {
                LogMessage message = null;
                lock (messageQueue)
                {
                    message = messageQueue.Dequeue();
                }
                if (message != null)
                {
                    FileWrite(message);
                }
            }
            CloseFile();
        }

        /// <summary>
        /// 清理日志文件
        /// 当日志目录中的文件数过多时，自动清理日志
        /// </summary>
        void ClearLogFile()
        {
            List<string> fileNames = new List<string>();
            try
            {
                fileNames.AddRange(Directory.GetFiles(path));
            }
            catch { return; }
            if (fileNames.Count > 30)
            {
                fileNames.Sort();
                for (int i = fileNames.Count - 1; i > 10; i--)
                {
                    try
                    {
                        File.Delete(fileNames[i]);
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// 获取日志文件名
        /// </summary>
        string GetFileName()
        {
            DateTime now = DateTime.Now;
            string format = "";
            switch (logType)
            {
                case LogTypes.Daily:
                    timeSign = new DateTime(now.Year, now.Month, now.Day);
                    timeSign = timeSign.AddDays(1);
                    format = "yyyyMMdd'.log'";
                    break;
                case LogTypes.Weekly:
                    timeSign = new DateTime(now.Year, now.Month, now.Day);
                    timeSign = timeSign.AddDays(7);
                    format = "yyyyMMdd'.log'";
                    break;
                case LogTypes.Monthly:
                    timeSign = new DateTime(now.Year, now.Month, 1);
                    timeSign = timeSign.AddMonths(1);
                    format = "yyyyMM'.log'";
                    break;
                case LogTypes.Annually:
                    timeSign = new DateTime(now.Year, 1, 1);
                    timeSign = timeSign.AddYears(1);
                    format = "yyyy'.log'";
                    break;
            }
            return now.ToString(format);
        }

        /// <summary>
        /// 写日志
        /// </summary>
        void FileWrite(LogMessage message)
        {
            try
            {
                if (streamWriter == null)
                {
                    OpenFile();
                }
                if (DateTime.Now >= timeSign)
                {
                    CloseFile();
                    OpenFile();
                }
                streamWriter.Write(message.DateTime);
                streamWriter.Write(' ');
                streamWriter.Write(message.Type);
                streamWriter.Write(' ');
                streamWriter.WriteLine(message.Text);
                streamWriter.Flush();
            }
            catch (Exception e) { Console.Out.WriteLine(e.Message); }
        }

        /// <summary>
        /// 打开文件准备写入
        /// </summary>
        void OpenFile()
        {
            if (!path.EndsWith(@"\")) { path += @"\"; }
            try
            {
                streamWriter = new StreamWriter(path + GetFileName(), true, Encoding.UTF8);
            }
            catch (DirectoryNotFoundException)
            {
                try
                {
                    Directory.CreateDirectory(path);
                    streamWriter = new StreamWriter(path + GetFileName(), true, Encoding.UTF8);
                }
                catch (Exception) { }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 关闭打开的日志文件
        /// </summary>
        void CloseFile()
        {
            if (streamWriter != null)
            {
                try
                {
                    streamWriter.Flush();
                    streamWriter.Close();
                    streamWriter = null;
                }
                catch (Exception) { }
            }
        }

        /// <summary>
        /// 写日志
        /// </summary>
        void Write(LogMessage message)
        {
            if (message != null)
            {
                lock (messageQueue)
                {
                    messageQueue.Enqueue(message);
                }
            }
        }

        /// <summary>
        /// 写日志
        /// </summary>
        public void Write(string message)
        {
            Write(new LogMessage(message, LogMessageTypes.Info));
        }

        /// <summary>
        /// 写日志
        /// </summary>
        public void Write(string message, LogMessageTypes type)
        {
            Write(new LogMessage(message, type));
        }

        /// <summary>
        /// 写日志
        /// </summary>
        public void Write(Exception e)
        {
            Write(e.Message, LogMessageTypes.Excepiton);
        }

        /// <summary>
        /// 销毁日志对象
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// 日志消息类
        /// </summary>
        private class LogMessage
        {
            private string text;            //日志消息文本
            private LogMessageTypes type;   //日志消息类型
            private DateTime dateTime;      //日志产生时间

            /// <summary>
            /// 日志消息文本
            /// </summary>
            public string Text
            {
                get { return text; }
                set { text = value; }
            }

            /// <summary>
            /// 日志消息类型
            /// </summary>
            public LogMessageTypes Type
            {
                get { return type; }
                set { type = value; }
            }

            /// <summary>
            /// 日志消息产生时间
            /// </summary>
            public DateTime DateTime
            {
                get { return dateTime; }
                set { dateTime = value; }
            }

            /// <summary>
            /// 构造函数
            /// </summary>
            public LogMessage()
                : this("", LogMessageTypes.Info, DateTime.Now)
            { }

            /// <summary>
            /// 构造函数
            /// </summary>
            public LogMessage(string text, LogMessageTypes type)
                : this(text, type, DateTime.Now)
            { }

            /// <summary>
            /// 构造函数
            /// </summary>
            public LogMessage(string text, LogMessageTypes type, DateTime dateTime)
            {
                this.text = text;
                this.type = type;
                this.dateTime = dateTime;
            }
        }
    }

    /// <summary>
    /// 日志类型
    /// </summary>
    public enum LogTypes
    {
        Daily = 0,      // 每天一个日志文件
        Weekly = 1,     // 每周一个
        Monthly = 2,    // 每月一个
        Annually = 3,   // 每年一个
    }

    /// <summary>
    /// 日志消息类型
    /// </summary>
    public enum LogMessageTypes
    {
        Info,       // 通知消息
        Warning,    // 警告消息
        Excepiton,  // 异常消息
        Error,      // 非致命错误消息
        Fatal,      // 致命错误消息
    }
}

