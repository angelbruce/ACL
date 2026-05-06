using System.Text.RegularExpressions;

namespace ACL.business.content
{
    class RegexBlock : IBlockSpliter
    {
        private string regex = string.Empty;
        public RegexBlock(string regex)
        {
            this.regex = regex;
        }

        public bool Test(string data)
        {
            if (data == null || data.Length == 0) return false;
            if (string.IsNullOrEmpty(regex)) return false;

            return Regex.IsMatch(data, regex);

        }
        public List<Block> Fetch(string data)
        {
            var blocks = new List<Block>();
            var pattern = new Regex(regex, RegexOptions.Multiline | RegexOptions.IgnoreCase);
            for (var match = pattern.Match(data); match.Success; match = match.NextMatch())
            {
                var block = new Block();
                blocks.Add(block);
                var groups = match.Groups;
                if (groups.Count > 1)
                {
                    foreach (Group group in groups)
                    {
                        var a = 0;
                        if (int.TryParse(group.Name, out a)) continue;
                        if (string.IsNullOrEmpty(group.Name)) continue;
                        var val = group.Value;
                        if (val != null) val = val.Trim();
                        block.Pairs.Add(new NameValue { Name = group.Name, Value = val });
                    }
                }

            }

            return blocks;

        }
    }
}