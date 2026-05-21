using ABL.Config.Ant;
using ABL.Data;
using ABL.Store;
using ACL.business.session;

namespace ACL
{
    internal static class Program
    {
        private const string LLM_MODEL_RESOURCE = "config.xml";


        [STAThread]
        static void Main()
        {
            AntContext.Instance.Register(LLM_MODEL_RESOURCE);
            Instance<PostOffice>.Data.Start();
            var driver = DbDriver.Create();
            var databaseSchema = new DataBaseSchema(driver);
            if (!databaseSchema.Initialize())
            {
                MessageBox.Show("初始化数据源数据失败。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}