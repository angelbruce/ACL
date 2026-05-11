using ACL.business.agent;
using ACL.dao;
using ACL.flow;

namespace ACL.business.flow
{
    /// <summary>
    /// Agent任务
    /// </summary>

    class AgentTask
    {
        /// <summary>
        /// 任务完成代理
        /// </summary>
        /// <param name="agent"></param>
        public delegate void DgtAgentTaskFinished(AgentTask agent);

        /// <summary>
        /// 任务完成事件
        /// </summary>
        public event DgtAgentTaskFinished AgentTaskFninshed;
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
        public string Ask { get; private set; }
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
        /// 活动的agent
        /// </summary>
        public Agent? Agent { get; set; } = null;
        /// <summary>
        /// 任务列表
        /// </summary>
        public List<TaskInfo> Tasks { get; set; } = new List<TaskInfo>();
        /// <summary>
        /// 流程节点所有任务是否已经完成
        /// </summary>
        public bool IsTaskOver { get { return false; } }

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

        /// <summary>
        /// 刷新任务
        /// </summary>
        /// <param name="task"></param>
        public void RefreshTask(TaskInfo task)
        {
            if (task == null) { return; }

            if (!Refresh(Tasks, task))
            {
                Tasks.Add(task);
            }

            foreach (var t in Tasks)
            {
                if (t.Status != "FINISHED")
                {
                    return;
                }
            }

            if (string.IsNullOrEmpty(task.NextActionPlan) || task.NextActionPlan == "N/A")
            {
                if (AgentTaskFninshed != null)
                {
                    AgentTaskFninshed(this);
                }
            }
        }

        /// <summary>
        /// 刷新容器任务
        /// </summary>
        /// <param name="containers"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        private bool Refresh(List<TaskInfo> containers, TaskInfo task)
        {
            for (int i = 0; i < containers.Count; i++)
            {
                var tk = containers[i];
                if (tk.Id == task.Id)
                {
                    Tasks.RemoveAt(i);
                    Tasks.Insert(i, tk);
                    return true;
                }


                var subs = tk.SubTaskList;
                if (subs != null)
                {
                    if (Refresh(subs, task))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 产生的会话集合
        /// </summary>
        public List<SessionItem> SessionItems { get; set; }
    }

}
