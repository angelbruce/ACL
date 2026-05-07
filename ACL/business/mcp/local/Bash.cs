using ACL.business.project;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace ACL.business.mcp.local
{
    [McpServerTool]
    public class BashExecutor
    {
        /// <summary>
        /// Executes a system command and returns the output.
        /// </summary>
        /// <param name="command">The command to execute (e.g., "ls -l").</param>
        /// <param name="workDir">The directory to execute the command in (use "" for current directory).</param>
        /// <param name="timeoutMs">Optional timeout in milliseconds.</param>
        /// <returns>The standard output of the command as a string.</returns>
        [McpTool, Description("Executes a system command and returns the output.")]
        public static string ExecuteCommand(
         [Required][Description("the command to execute")] string command,
         [Required][Description("working directory")] string workDir
            )
        {

            if (string.IsNullOrEmpty(command)) return $"error found: command needed or error workDir, your inputs `command`: {command} , `workDir`: {workDir}";

            if (workDir == null)
            {
                workDir = ProjectConfig.Current.Directory;
            }

            Console.WriteLine($"Executing command: {command} in directory: {workDir}");

            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c {command}",
                WorkingDirectory = workDir,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (var process = new Process { StartInfo = startInfo })
            {
                var output = new StringBuilder();
                var error = new StringBuilder();
                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                        output.AppendLine(e.Data);
                };
                process.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                        error.AppendLine(e.Data);
                };
                try
                {
                    process.Start();

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();
                }
                catch (Exception ex)
                {
                    return $"Error executing command: {ex.Message}";
                }
                if (process.ExitCode != 0)
                {
                    return $"Command failed with exit code {process.ExitCode}.\nError Output:\n{error.ToString()}";
                }

                return output.ToString().Trim();
            }
        }
    }
}
