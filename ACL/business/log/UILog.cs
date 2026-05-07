using System;
using System.Collections.Generic;
using System.Text;

namespace ACL.business.log
{
    public class UILog : ILog
    {
        RichTextBox box;
        public UILog(RichTextBox richTextBox)
        {
            box = richTextBox;
        }

        private void Write(LogLevel level, string message)
        {
            if (message == null || message.Length == 0) { return; }

            box.Invoke(() =>
            {
                int len = box.TextLength;
                var msg = $"[{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}-{level}]-{message}";
                box.AppendText(msg + Environment.NewLine);
                box.Select(len, msg.Length);
                switch (level)
                {
                    case LogLevel.Debug:
                        box.SelectionColor = Color.Black;
                        break;
                    case LogLevel.Info:
                        box.SelectionColor = Color.Green;
                        break;
                    case LogLevel.Warn:
                        box.SelectionColor = Color.Orange;
                        break;
                    case LogLevel.Error:
                        box.SelectionColor = Color.Red;
                        break;
                    case LogLevel.Fatal:
                        box.SelectionColor = Color.DarkRed;
                        break;
                }
                box.SelectionLength = 0;
                box.ForeColor = Color.Black;
                box.ScrollToCaret();
            });
        }
        public void Debug(string message)
        {
            Write(LogLevel.Debug, message);
        }

        public void Info(string message)
        {
            Write(LogLevel.Info, message);
        }


        public void Warn(string message)
        {
            Write(LogLevel.Warn, message);
        }

        public void Error(string message)
        {
            Write(LogLevel.Error, message);
        }

        public void Fatal(string message)
        {
            Write(LogLevel.Fatal, message);
        }



    }
}
