using ABL.Config.Ant;
using ABL.Data;
using ABL.Object;
using ABL.Store;
using ACL.business;
using ACL.business.content;
using ACL.dao;
using YamlDotNet.Serialization;

namespace ACL
{
    internal static class Program
    {
        private const string LLM_MODEL_RESOURCE = "config.xml";


        [STAThread]
        static void Main()
        {
            AntContext.Instance.Register(LLM_MODEL_RESOURCE);
            //test();
            test1();
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

      



        static void test1()
        {
            var str = @"```yaml
Id:   Project_ARPG_MVP_001
CurrentStepDescription:   建立游戏的基础技术框架与移动/战斗原型
SubTaskList:
  - Id:   T1_ArchitectureSetup
    Status:  RUNNING
    NeedHumanJoinStrategy:  false
    Description:   初始化 Unity 项目，并构建基础的组件（Component）和数据（Data）层架构，以支持数据驱动的技能和怪物行为。
    InputData:   Unity Engine, Component-Driven Design Pattern Specification.
    VerificationStatus:   PENDING
    ReflectionNotes:   待执行。此任务旨在证明我们的技术架构能够承载复杂性和可扩展性。
NextActionPlan:   T2_BasicCombatSystem
```



";

            var storeConfig = new ContentStoreConfig
            {
                ContentType = ContentType.Yaml,
                StoreType = StoreType.None,
                Regex = @"```yaml(?<plan>[\s\S]*?)```"
            };


            var contentReslover = new ContentReslover(new List<ContentStoreConfig> { storeConfig });
            var data = contentReslover.Perform(str);
            Console.Write(data);
        }

        static void test()
        {
            var str = @"
{
    ""Sub_Tasks_List"": [
        {
            ""Task_ID"": ""T1"",
            ""NeedHumanJoinStrategy"": true,
        },
    ],
    ""Next_Action_Plan"": ""T1: 启动深度追问阶段，明确项目核心范围，并将业务需求转化为可行动的、结构化的设计文档。这是后续所有任务的基础。""
}
";
            var json = new RelexJSON();
            var data = json.Deserealize<TaskInfo>(str);
            Console.WriteLine(data);
        }
    }
}