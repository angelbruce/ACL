using ABL.Object;
using System.Text;
using YamlDotNet.Serialization;

namespace ACL.business.content
{
    public class YamlFetchObj : IFetchObj
    {
        public T? Fetch<T>(string content) where T : class, new()
        {
            if (content == null || content.Length == 0) return default;

            var cont = content;
            if (typeof(T) == typeof(TaskInfo))
            {
                cont = CorrectYaml(cont);
            }

            var serializer = new DeserializerBuilder().IgnoreUnmatchedProperties().Build();
            var obj = serializer.Deserialize<T>(cont);

            return obj;
        }

        private string CorrectYaml(string yaml)
        {
            if (string.IsNullOrEmpty(yaml)) return yaml;

            var props = typeof(TaskInfo).GetProperties().Where(x => !x.IsSpecialName).ToDictionary(x => x.Name, y => y);
            var sb = new StringBuilder();
            yaml = yaml.Replace("：", ":");
            foreach (var line in yaml.Split(new char[] { '\r', '\n' }))
            {
                if (string.IsNullOrEmpty(line)) continue;

                var idx = line.IndexOf(':');
                var total = 0;
                foreach (var k in line)
                {
                    if (k == ' ') total++;
                    else break;
                }

                var nest = total > 1;
                var slashIdx = line.IndexOf('-');
                var slash = slashIdx != -1 && slashIdx < idx;

                var name = line.Substring(0, idx).TrimStart();
                if (slash) name = name.TrimStart().Substring(1).Trim();
                var value = line.Substring(idx + 1).TrimStart();
                var array = false;
                if (name.Equals("SubTaskList", StringComparison.OrdinalIgnoreCase))
                {
                    value = "";
                    array = true;
                }

                if (nest)
                {
                    sb.Append("  ");
                    if (slash) sb.Append("- ");
                    else sb.Append("  ");
                }

                sb.Append(name);
                sb.Append(":");
                if (!array)
                {
                    sb.Append(" ");
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (name != "NeedHumanJoinStrategy")
                        {
                            value = value.TrimStart('\"').TrimEnd('\"').Replace("\\", "")
                                .Replace("[", "\\\\[").Replace("]", "\\\\]")
                                .Replace("{", "\\\\{").Replace("}", "\\\\}")
                                .Replace("\"", "\\\"")
                                .Replace(":", "\\\\:")
                                ;
                            value = "\"" + value + "\"";
                        }
                    }
                    sb.Append(value);
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}