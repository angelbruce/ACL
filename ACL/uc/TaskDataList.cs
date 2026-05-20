using ACL.business;
using ACL.business.mcp.local;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ACL.uc
{
    public partial class TaskDataList : UserControl
    {
        public TaskDataList()
        {
            InitializeComponent();
            this.Load += TaskTree_Load;
        }

        private void TaskTree_Load(object? sender, EventArgs e)
        {
            //InitTaskListeners();
            TodoStore.Instance.OnDeleted += (s) => RefreshTodoItems();
            TodoStore.Instance.OnUpdated += (s) => RefreshTodoItems();
            TodoStore.Instance.OnCreated += (s) => RefreshTodoItems();
            TodoStore.Instance.Inprogressed += (s) => RefreshTodoItems();
            TodoStore.Instance.Completed += (s) => RefreshTodoItems();
        }

        private void RefreshTodoItems()
        {
            var list = TodoStore.Instance.Gets().ToList();
            this.dgvTasks.Invoke(() =>
            {
                this.dgvTasks.DataSource = list;
            });
        }

        private void InitTaskListeners()
        {
            Context.Instance.TaskChanged += OnTaskChanged;
        }


        private void OnTaskChanged(List<TaskInfo> tasks, TaskInfo task)
        {
            var list = new List<TaskInfo>();
            FlatTasks(tasks, list);
            this.dgvTasks.Invoke(() =>
            {
                this.dgvTasks.DataSource = list;
            });
        }



        private void FlatTasks(List<TaskInfo> tasks, List<TaskInfo> output)
        {
            foreach (var task in tasks.OrderBy(x => x.Id).ToList())
            {
                output.Add(task);
                if (task.SubTaskList != null && task.SubTaskList.Count > 0)
                {
                    FlatTasks(task.SubTaskList, output);
                }
            }
        }

    }
}
