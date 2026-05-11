using ABL.Config.Ant;
using ABL.Data;
using ABL.Object;
using ABL.Store;
using ACL.business;
using ACL.business.content;
using ACL.dao;
using Microsoft.Testing.Platform.Extensions.Messages;
using System.CodeDom;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ACL
{
    internal static class Program
    {
        private const string LLM_MODEL_RESOURCE = "config.xml";


        [STAThread]
        static void Main()
        {
            AntContext.Instance.Register(LLM_MODEL_RESOURCE);
            //test2();
            //test1();
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


        static void test2()
        {
            long id = 10;
            Expression<Func<FlowRuntime, bool>> where = a => (a.Id == id && a.IsOver == true);
            var sql = new StringBuilder();
            VisitWhereExpr(where.Body, sql, new Dictionary<string, string>(), id);
            var a = sql.ToString();

            Console.Write(a);

        }

        private static void VisitWhereExpr(Expression expression, StringBuilder where, Dictionary<string, string> reverseMap, long id)
        {
            if (expression is BinaryExpression binaryExpr)
            {
                where.Append("(");
                VisitWhereExpr(binaryExpr.Left, where, reverseMap, id);

                switch (binaryExpr.NodeType)
                {
                    case ExpressionType.Add:
                        where.Append('+');
                        break;
                    case ExpressionType.Multiply:
                        where.Append('*');
                        break;
                    case ExpressionType.Divide:
                        where.Append('/');
                        break;
                    case ExpressionType.Subtract:
                        where.Append('-');
                        break;

                    case ExpressionType.Equal:
                        where.Append('=');
                        break;
                    case ExpressionType.NotEqual:
                        where.Append("<>");
                        break;
                    case ExpressionType.GreaterThan:
                        where.Append(">");
                        break;

                    case ExpressionType.GreaterThanOrEqual:
                        where.Append(">=");
                        break;

                    case ExpressionType.LessThan:
                        where.Append("<");
                        break;
                    case ExpressionType.LessThanOrEqual:
                        where.Append("<=");
                        break;

                    case ExpressionType.And:
                    case ExpressionType.AndAlso:
                        where.Append(") and (");
                        break;
                    case ExpressionType.Or:
                    case ExpressionType.OrElse:
                        where.Append(") or (");
                        break;

                }

                VisitWhereExpr(binaryExpr.Right, where, reverseMap, id);
                where.Append(")");
            }
            else if (expression is MemberExpression memberExpr)
            {
                switch (memberExpr.Member.MemberType)
                {
                    case System.Reflection.MemberTypes.Property:
                        var memberName = memberExpr.Member.Name;
                        var column = reverseMap.ContainsKey(memberName) ? reverseMap[memberName] : memberName;
                        where.Append(column);
                        break;
                    case System.Reflection.MemberTypes.Field:
                        //how to invoke the expression here?
                        object instance = null;
                        var fieldInfo = (FieldInfo)memberExpr.Member;
                        if (memberExpr.Expression != null)
                        {
                            // 递归求值以获取实例对象
                            // 这里需要一个辅助方法来计算表达式的值
                            instance = EvaluateExpression(memberExpr.Expression);
                        }

                        // 2. 获取字段值
                        object fieldValue = fieldInfo.GetValue(instance);

                        break;
                }

            }

            else if (expression is ConstantExpression constExpr)
            {
                where.Append(constExpr.Value?.ToString() ?? "null");
            }
            else
            {
                throw new NotSupportedException($"Expression type {expression.GetType().Name} is not supported.");
            }
        }

        static object EvaluateExpression(Expression expr)
        {
            if (expr == null) return null;

            switch (expr.NodeType)
            {
                case ExpressionType.Constant:
                    return ((ConstantExpression)expr).Value;

                case ExpressionType.MemberAccess:
                    var memberExpr = (MemberExpression)expr;
                    var instance = EvaluateExpression(memberExpr.Expression);

                    if (memberExpr.Member is PropertyInfo prop)
                        return prop.GetValue(instance);
                    else if (memberExpr.Member is FieldInfo field)
                        return field.GetValue(instance);
                    else
                        throw new NotSupportedException();

                default:
                    // 对于更复杂的表达式，可以编译并执行
                    var lambda = Expression.Lambda(expr);
                    var compiled = lambda.Compile();
                    return compiled.DynamicInvoke();
            }
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