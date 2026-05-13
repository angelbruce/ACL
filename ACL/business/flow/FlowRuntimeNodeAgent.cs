using ACL.dao;
using ACL.flow;
using System.Threading.Channels;

namespace ACL.business.flow
{
    class FlowRuntimeNodeAgent
    {
        /// <summary>
        /// 任务完成代理
        /// </summary>
        /// <param name="agent"></param>
        public delegate void DgtAgentTaskFinished(FlowRuntimeNode current);

        /// <summary>
        /// 任务完成事件
        /// </summary>
        public event DgtAgentTaskFinished? AgentTaskFninshed;
        /// <summary>
        /// 运行时节点
        /// </summary>
        public FlowRuntimeNode? Node { get; set; }
        /// <summary>
        /// 蓝图任务
        /// </summary>
        public AgentTask? AgentTask { get; set; }

        /// <summary>
        /// 任务列表
        /// </summary>
        public List<TaskInfo> Tasks { get; set; } = new List<TaskInfo>();

        /// <summary>
        /// 产生的会话集合
        /// </summary>
        public List<SessionItem> SessionItems { get; set; } = new List<SessionItem>();
        /// <summary>
        /// AGENT
        /// </summary>
        public Agent? Agent { get; set; }

        /// <summary>
        /// 启动AGENT
        /// </summary>
        /// <param name="channel"></param>
        public void Start(Channel<string> channel)
        {
            Agent?.Serve(channel);
        }

        /// <summary>
        /// 关停AGENT,并释放资源
        /// </summary>
        public void Stop()
        {
            Agent?.Stop();
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
                    AgentTaskFninshed(null);
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
    }
}
