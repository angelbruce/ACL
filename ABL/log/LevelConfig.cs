using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ABL
{
    /// <summary>
    /// 日志级别
    /// </summary>
    public enum LogLevel
    {
        Debug,
        Info,
        Warn,
        Error,
        Fatal
    }
    /// <summary>
    /// 日志配置缓存类
    /// </summary>
    public class LevelConfig
    {
        static readonly object _lock = new object();
        static Dictionary<LogLevel, bool> config = null;
        /// <summary>
        /// 缓存日志配置等级集合
        /// </summary>
        static Dictionary<LogLevel, bool> Config
        {
            get
            {
                if(config == null)
                {
                    lock (_lock)
                    {
                        if(config == null)
                        {
                            string appConfig = "Debug";
                            appConfig = appConfig ?? "";
                            var appConfigs = appConfig.Split(';');

                            var cfg = new Dictionary<LogLevel, bool>();
                            foreach (var fd in typeof(LogLevel).GetFields())
                            {
                                if (fd.IsSpecialName) continue;
                                bool enabled = false;
                                foreach (var appCfg in appConfigs)
                                {
                                    if (string.Compare(fd.Name, appCfg, true) == 0)
                                    {
                                        enabled = true;
                                        break;
                                    }
                                }
                                var level = (LogLevel)(Enum.Parse(typeof(LogLevel), fd.Name, true));
                                cfg.Add(level, enabled);
                            }
                            config = cfg;
                        }
                    }
                }
                return config;
            }
        }

        /// <summary>
        /// 等级为level的日志是否开启
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static bool IsEnabled(LogLevel level)
        {
            return Config[level];
        }

    }
}
