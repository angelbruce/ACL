using ACL.business.project;

namespace ACL.business.content
{

    class FileStore : IStore
    {
        public const string PATH_TAG = "filename";
        public const string CONTENT_TAG = "content";
        public FileStore() { }

        public void Store(Block block)
        {
            string filename = "";
            string content = "";
            foreach (var pair in block.Pairs)
            {
                if (pair.Name.Equals(PATH_TAG))
                {
                    filename = pair.Value;
                    continue;
                }

                if (pair.Name.Equals(CONTENT_TAG))
                {
                    content = pair.Value;
                    continue;
                }
            }

            if (string.IsNullOrEmpty(filename)) return;

            if (string.IsNullOrEmpty(content)) return;

            var file = Path.Combine(ProjectConfig.Current.Directory, filename);
            if (!File.Exists(file))
            {
                var dir = Path.GetDirectoryName(file);
                if (!Directory.Exists(dir))
                {
                    var d = Directory.CreateDirectory(dir);
                    if (!d.Exists)
                    {
                        Thread.Sleep(0);
                    }
                }
            }

            File.WriteAllText(file, content);
        }
    }
}