using ACL.business.project;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ACL.business.mcp.local
{
    [McpServerTool]
    public class FileSystemTool
    {
        [McpTool, Description("检查文件是否存在，需要传入 `absolutePath`")]
        public static bool CheckFileExist([Required][Description("被检查文件路径")] string absolutePath)
        {
            var projectDir = ProjectConfig.Current.Directory;
            if (!absolutePath.ToLower().StartsWith(projectDir.ToLower())) absolutePath = Path.Combine(ProjectConfig.Current.Directory, absolutePath);

            return File.Exists(absolutePath);
        }

        [McpTool, Description("检查目录是否存在")]
        public static bool CheckDirExist([Required][Description("被检查的目录")] string path)
        {
            if(path == null)
            {
                path = ProjectConfig.Current.Directory;
            }

            if (path.Equals("/"))
            {
                path = ProjectConfig.Current.Directory;
            }
            return Directory.Exists(path);
        }

        [McpTool, Description("读取文件的内容，需要传入 `absolutePath`")]
        public static string ReadTextFile([Required][Description("被读取文件的绝对路径")] string absolutePath)
        {
            var projectDir = ProjectConfig.Current.Directory;
            if (!absolutePath.ToLower().StartsWith(projectDir.ToLower())) absolutePath = Path.Combine(ProjectConfig.Current.Directory, absolutePath);
            return File.ReadAllText(absolutePath);
        }

        [McpTool, Description("创建文件，需要传入文件绝对路径 `absolutePath` ")]
        public static bool CreateFile([Required][Description("被写入文件的绝对路径")] string absoluteDir)
        {
            if (absoluteDir == null) return false;
            if (absoluteDir.Equals("/"))
            {
                absoluteDir = ProjectConfig.Current.Directory;
            }

            if (!File.Exists(absoluteDir))
            {
                var dir = Path.GetDirectoryName(absoluteDir);
                if (!Directory.Exists(dir))
                {
                    var d = Directory.CreateDirectory(dir);
                    if (!d.Exists)
                    {
                        Thread.Sleep(0);
                    }
                }
            }
            File.WriteAllText(absoluteDir, null);
            return true;
        }


        [McpTool, Description("写入文本文件内容，需要传入文件绝对路径 `absolutePath` 与写入内容 `content` ,文件路径必须是全路径，不能只写入文件名，应该包含项目路径+相对路径+文件名。写入成功将返回 `OK` ")]
        public static string WriteTextFile([Required][Description("被写入文件的绝对路径")] string absolutePath, [Required][Description("写入内容")] string content)
        {
            try
            {
                var projectDir = ProjectConfig.Current.Directory;
                if (!absolutePath.ToLower().StartsWith(projectDir.ToLower())) absolutePath = Path.Combine(ProjectConfig.Current.Directory, absolutePath);
                if (!File.Exists(absolutePath))
                {
                    var dir = Path.GetDirectoryName(absolutePath);
                    if (!Directory.Exists(dir))
                    {
                        var d = Directory.CreateDirectory(dir);
                        if (!d.Exists)
                        {
                            Thread.Sleep(0);
                        }
                    }
                }

                File.WriteAllText(absolutePath, content);
                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }




        [McpTool, Description("获取本目录下不包含子目录的文件路径，会输出少量数据，必须传入被查询的文件路经参数 `absoluteDir`")]
        public static async Task<List<string>> ListMyTopDirectory([Required][Description("被查询目录的绝对路径，必须传入")] string absoluteDir)
        {
            var list = new List<string>();
            if (absoluteDir.Equals("/"))
            {
                absoluteDir = ProjectConfig.Current.Directory;
            }
            if (!Directory.Exists(absoluteDir))
            {
                Console.WriteLine($"错误：指定的目录不存在：{absoluteDir}");
                return [];
            }

            // 1. 获取所有文件
            // SearchOption.AllDirectories 确保会递归地搜索子目录
            string[] allFiles = Directory.GetFiles(absoluteDir, "*", SearchOption.TopDirectoryOnly);
            list.AddRange(allFiles);


            // 2. 获取所有目录
            string[] allDirectories = Directory.GetDirectories(absoluteDir, "*", SearchOption.TopDirectoryOnly);
            list.AddRange(allDirectories);

            return list;
        }



        [McpTool, Description("获取目录下包含子目录所有文件路径，会输出大量数据，必须传入被查询的文件路经参数 `absoluteDir`")]
        public static async Task<List<string>> ListAllSubDirectories([Required][Description("被查询目录的绝对路径，必须传入")] string absoluteDir)
        {
            var list = new List<string>();
            if (!Directory.Exists(absoluteDir))
            {
                Console.WriteLine($"错误：指定的目录不存在：{absoluteDir}");
                return [];
            }

            if (absoluteDir.Equals("/"))
            {
                absoluteDir = ProjectConfig.Current.Directory;
            }


            var stack = new Stack<string>();
            stack.Push(absoluteDir);
            var ignored = Ignore.Patterns();
            while (stack.Count > 0)
            {
                var dir = stack.Pop();
                var dirs = Directory.GetDirectories(dir, "*", SearchOption.TopDirectoryOnly);
                var files = Directory.GetFiles(dir, "*", SearchOption.TopDirectoryOnly);
                foreach (var item in files)
                {
                    var flag = true;
                    foreach (var key in ignored)
                    {
                        if (item.Contains(key))
                        {
                            flag = false;
                            break;
                        }
                    }

                    if (flag)
                    {
                        list.Add(item);
                    }
                }

                foreach (var item in dirs)
                {
                    var flag = true;
                    foreach (var key in ignored)
                    {
                        if (item.Contains(key))
                        {
                            flag = false;
                            break;
                        }
                    }

                    if (flag)
                    {
                        stack.Push(item);
                        list.Add(item);
                    }
                }
            }


            return list;
        }


    }
}
