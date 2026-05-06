using ABL.Config.Ant;

namespace ACL.business.prompt
{

    public class PromptConfig
    {
        private const string PROMPT_KEY = "prompt";

        public PromptConfig() { }

        private List<PromptFile> PromptFiles()
        {
            var list = new List<PromptFile>();
            var items = AntContext.Instance.GetItems(PROMPT_KEY);
            if (items != null)
            {
                foreach (var item in items)
                {
                    var data = item as PromptFile;
                    if (data != null)
                    {
                        list.Add(data);
                    }
                }
            }

            return list;
        }


        public List<Prompt> Prompts()
        {
            var data = new List<Prompt>();
            var list = PromptFiles();
            foreach (var item in list)
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, item.Path);
                if (!File.Exists(path)) continue;

                var content = File.ReadAllText(item.Path);
                if (content != null && content.Length > 0)
                {
                    data.Add(new Prompt { Content = content, Type = item.Type, Usage = item.Usage });
                }
            }

            return data;
        }
    }

    public class Prompt
    {
        public Prompt() { }

        public string Content { get; set; }
        public PromptType Type { get; set; }

        public string Usage { get; set; }
    }

    public enum PromptType
    {
        System,
        Assistant,
        User
    }

    public class PromptAnt : BaseAnt
    {
        public override IAntItem Create()
        {
            return new PromptFile();
        }
    }


    [AntRoot("prompts")]
    [AntPrefix("prompt")]
    public class PromptFile : IAntItem
    {
        [AntElement("type")]
        public PromptType Type { get; set; }

        [AntElement("path")]
        public string Path { get; set; }

        [AntElement("usage")]
        public string Usage { get; set; }
    }


}
