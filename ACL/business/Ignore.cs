using ACL.business.project;
using System;
using System.Collections.Generic;
using System.Text;

namespace ACL.business
{
    internal class Ignore
    {
        private static string FILE_NAME = ".ignore";
        public static HashSet<string> Patterns()
        {
            var dir = ProjectConfig.Current.Directory;
            var file = Path.Combine(dir, FILE_NAME);
            if (!File.Exists(file))
            {
                return new HashSet<string>();
            }

            var content = File.ReadAllText(file);
            return content.Split(new char[] { '\r', '\n' }).Where(x => !string.IsNullOrEmpty(x)).ToHashSet();
        }
    }
}
