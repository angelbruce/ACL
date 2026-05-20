using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Data;

namespace ABL
{
    /// <summary>
    /// 日志
    /// </summary>
    public class Logger
    {
        static readonly object _lock = new object();
        const int LOG_SIZE = 1024 * 1024 * 250;

        DateTime begin = DateTime.Now;
        DateTime end = DateTime.Now;
        /// <summary>
        /// 从开始检测记录日志到结束检测记录日志总时消耗
        /// </summary>
        public TimeSpan Elasped
        {
            get { return end - begin; }
        }
        /// <summary>
        /// 开始检测记录
        /// </summary>
        public void Begin()
        {
            begin = DateTime.Now;
        }
        /// <summary>
        /// 结束检测记录
        /// </summary>
        /// <param name="msg"></param>
        public void End(string msg)
        {
            end = DateTime.Now;
            var total = (end - begin).TotalSeconds;
            Log(string.Format("thead:{2}elasped:{1}s\r\n{0}", msg, total, System.Threading.Thread.CurrentThread.ManagedThreadId));
        }
        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="type">发生错误日志的类类型</param>
        /// <param name="e">错误消息</param>
        public static void Error(Type type, string e)
        {
            if (LevelConfig.IsEnabled(LogLevel.Error)) Log(e);
        }
        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="type">发生错误日志的类类型</param>
        /// <param name="e">错误消息</param>
        public static void Error(Type type, Exception e)
        {
            if (LevelConfig.IsEnabled(LogLevel.Error)) Log(e.ToString());
        }
        /// <summary>
        /// 记录提示日志
        /// </summary>
        /// <param name="type">记录提示日志的类类型</param>
        /// <param name="e">提示信息</param>
        public static void Info(Type type, string e)
        {
            if (LevelConfig.IsEnabled(LogLevel.Info)) Log(e);
        }

        public static void Log(LogLevel level,string message,Exception exception)
        {
            if (LevelConfig.IsEnabled(level)) Log(string.Format("level:{0}\nmessage:{1}\nexception:{2}",level,message,exception));


        }

        public static void RecordCall(Type type, MethodInfo m, object ret, params object[] prms)
        {
            //var apiLog = ConfigHelper.Settings<bool>("apiLog", false);
            //if (!apiLog) return;

            var xml = new XmlDocument();
            xml.PreserveWhitespace = true;
            var node = xml.CreateElement(m.Name);
            xml.AppendChild(node);

            var inOp = xml.CreateElement("In");
            node.AppendChild(inOp);

            var outOp = xml.CreateElement("Out");
            node.AppendChild(outOp);

            var tps = m.GetParameters();
            if (tps != null && tps.Length > 0)
            {
                int i = 0;
                foreach (ParameterInfo p in tps)
                {
                    var on = xml.CreateElement(p.Name);
                    var att = xml.CreateAttribute("type");
                    att.Value = p.ParameterType.FullName;
                    on.Attributes.Append(att);
                    var obj = prms[i++];
                    string data = obj == null ? "NULL" : obj.ToString();
                    on.InnerText = data;
                    inOp.AppendChild(on);
                }
            }

            var rett = m.ReturnType;
            if (rett != null)
            {
                var oatt = xml.CreateAttribute("type");
                oatt.Value = rett.FullName;
                outOp.Attributes.Append(oatt);

                string sret = string.Empty;

                if (ret == null) sret = "NULL";
                else if (ret.GetType() == typeof(DataSet)) sret = ((DataSet)ret).GetXml();
                else if (ret.GetType() == typeof(XmlDocument)) sret = ((XmlDocument)ret).OuterXml;
                else sret = ret.ToString();

                var cdata = xml.CreateCDataSection(sret);
                outOp.AppendChild(cdata);
            }


            var log = xml.OuterXml + string.Format("<Time>{0:yyyy-MM-dd HH:mm:ss}</Time>\r\n\r\n", DateTime.Now);

            ABL.Logger.Info(type, log);
        }


        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="value"></param>
        static void Log(string value)
        {
            var info = new System.Threading.ParameterizedThreadStart(Invoke);
            var thread = new System.Threading.Thread(info);
            thread.Priority = System.Threading.ThreadPriority.Normal;
            thread.IsBackground = true;
            var context = thread.ExecutionContext;
            thread.Start(value);
        }

        /// <summary>
        /// 执行日志记录
        /// </summary>
        /// <param name="data">日志内容</param>
        static void Invoke(object data)
        {
            var s = data as string;
            lock (_lock)
            {
            A:
                try
                {
                    var file = new System.IO.FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt"));
                    Stream stream = null;
                    if (file.Exists && file.Length > LOG_SIZE)
                        stream = file.Open(FileMode.Truncate, FileAccess.Write);
                    else
                        stream = file.Open(FileMode.Append, FileAccess.Write);
                    var writer = new StreamWriter(stream);
                    writer.Write("--------------------------------------------------------------------\r\n");
                    writer.Write(string.Format("[{0:yyyy-MM-dd HH:mm:ss}]{1}\r\n", DateTime.Now, s));
                    writer.Write("--------------------------------------------------------------------\r\n");
                    writer.Flush();
                    writer.Close();
                    writer.Dispose();
                }
                catch
                {
                    goto A;
                }
            }
        }
    }
}
