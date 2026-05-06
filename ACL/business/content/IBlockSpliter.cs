namespace ACL.business.content
{
    interface IBlockSpliter
    {
        public bool Test(string data);
        public List<Block> Fetch(string data);
    }
}