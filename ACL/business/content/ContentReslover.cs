using ACL.dao;

namespace ACL.business.content
{
    class ContentReslover
    {
        private List<ContentStoreConfig> configs;
        private Dictionary<long, IBlockSpliter> blockSpliters;
        public ContentReslover(List<ContentStoreConfig> configs)
        {
            this.configs = configs;
            blockSpliters = new Dictionary<long, IBlockSpliter>();
        }

        public List<TaskInfo> Perform(string content)
        {
            var taskInfos = new List<TaskInfo>();
            if (string.IsNullOrEmpty(content)) return taskInfos;
            if (configs == null || configs.Count == 0) return taskInfos;

            EnsureBlocks();
            foreach (var config in configs)
            {
                var spliter = blockSpliters[config.Id];
                var blocks = spliter.Fetch(content);
                if (blocks == null || blocks.Count == 0) continue;

                var contentType = config.ContentType;
                var storeType = config.StoreType;
                if (contentType == ContentType.Doc)
                {
                    switch (storeType)
                    {
                        case StoreType.File:

                            var store = new FileStore();
                            foreach (var block in blocks)
                            {
                                store.Store(block);
                            }

                            break;
                        case StoreType.Database:
                            break;
                        case StoreType.WebApi:
                            break;
                        case StoreType.None:
                            break;
                    }
                }
                else
                {
                    FillPlans(contentType, blocks, taskInfos);
                }
            }

            return taskInfos;
        }

        private void FillPlans(ContentType contentType, List<Block> blocks, List<TaskInfo> taskInfos)
        {
            var fetcher = IFetchObj.Create(contentType);
            if (fetcher == null) return;
            foreach (var block in blocks)
            {
                foreach (var pair in block.Pairs)
                {
                    if (pair.Name.Equals("plan", StringComparison.OrdinalIgnoreCase))
                    {
                        var content = pair.Value;
                        var obj = fetcher.Fetch<TaskInfo>(content);
                        if (obj != null) taskInfos.Add(obj);
                    }
                }
            }
        }

        private void EnsureBlocks()
        {
            if (blockSpliters.Count == 0)
            {
                foreach (var config in configs)
                {
                    var regex = config.Regex;
                    var block = new RegexBlock(regex);
                    blockSpliters[config.Id] = block;
                }
            }
        }
    }
}