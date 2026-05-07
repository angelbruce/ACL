using ACL.business.agent;
using ACL.dao;
using ACL.flow;
using System.Threading.Channels;

namespace ACL.business.flow
{

    class AgentTask
    {

        public delegate void DgtAgentTaskFinished(Agent agent);

        public event DgtAgentTaskFinished AgentTaskFninshed;

        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// 详细信息
        /// </summary>
        public AgentBody? Body { get; set; } = null;
        /// <summary>
        /// prompt
        /// </summary>
        public List<string> Asks { get; private set; } = new List<string>();
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; } = string.Empty;
        /// <summary>
        /// 活动的agent
        /// </summary>
        public Agent? Agent { get; set; } = null;
        /// <summary>
        /// 任务列表
        /// </summary>
        public List<TaskInfo> Tasks { get; set; } = new List<TaskInfo>();

        public bool IsTaskOver { get { return false; } }

        public AgentTask(Action action)
        {
            this.Id = action.Id;
            this.Asks = action.Asks.ToList();
            this.Type = action.Type;
        }

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
                    AgentTaskFninshed(Agent);
                }
            }
        }

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


        public List<SessionItem> SessionItems { get; set; }

    }

}
