using ACL.business.agent;

namespace ACL.business.flow
{
    /// <summary>
    /// Agent任务
    /// </summary>

    class AgentTask
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// 详细信息
        /// </summary>
        public AgentBody? Body { get; set; } = null;
        /// <summary>
        /// 提问
        /// </summary>
        public string Ask { get;  set; }
        /// <summary>
        /// 提示词
        /// </summary>
        public List<string> Prompts { get; set; }
        /// <summary>
        /// 下一步选项
        /// </summary>
        public List<string> Choices { get; set; }
        /// <summary>
        /// 下一步完成度
        /// </summary>
        public string Degree { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public EnumActionType Type { get; set; }
      

        /// <summary>
        /// 开始构建
        /// </summary>
        /// <param name="action"></param>
        public AgentTask(Action action)
        {
            Id = action.Id;
            Ask = action.Ask;
            Choices = action.NextChoices;
            Prompts = action.Prompts;
            Type = action.Type;
        }
    }

}
