using System.ComponentModel;

namespace ACL.business.mcp.local
{

    public delegate void DgtTodoCompleted(TodoItem item);
    public delegate void DgtTodoInprogress(TodoItem item);

    public enum TodoStatus
    {
        pending,
        inProgress,
        completed,
        cancelled
    }

    public class TodoItem : IDable
    {
        [Description("标题")]
        public string Title { get; set; } = string.Empty;

        [Description("任务说明")]
        public string? Description { get; set; }

        [Description("任务状态")]
        public TodoStatus Status { get; set; } = TodoStatus.pending; // pending, in_progress, completed, cancelled

        [Description("优先级")]
        public TodoPriority Priority { get; set; } = TodoPriority.medium;

        [Description("标签")]
        public string? Tags { get; set; }

        [Description("流程编号")]
        public string? FlowId { get; set; }

        [Description("运行时流程节点编号")]
        public string? NodeId { get; set; }

        [Description("任务创建时间")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Description("任务更新时间")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Description("任务完成时间")]
        public DateTime? CompletedAt { get; set; }
    }

    public class TodoStore : BasicCurd<TodoItem>
    {
        public event DgtTodoCompleted? Completed;
        public event DgtTodoInprogress? Inprogressed;

        private static TodoStore instance = new TodoStore();
        public static TodoStore Instance { get { return instance; } }

        public TodoItem AddTodo(TodoItem item)
        {
            return base.Add(item, t =>
            {
                var now = DateTime.UtcNow;
                t.CreatedAt = now;
                t.UpdatedAt = now;

            });
        }


        public TodoItem UpdateTodo(TodoItem item)
        {
            return base.Update(item, (data) =>
            {
                var now = DateTime.UtcNow;
                data.Tags = item.Tags;

                if (data.Status == TodoStatus.completed && data.CompletedAt == null)
                {
                    data.CompletedAt = now;
                }
                else if (data.Status != TodoStatus.completed)
                {
                    data.CompletedAt = null;

                }

                if (item.Status == TodoStatus.completed)
                {
                    Completed?.Invoke(item);
                }
                else if (item.Status == TodoStatus.inProgress)
                {
                    Inprogressed?.Invoke(item);
                }
            });
        }

        public bool DeleteTodo(string id)
        {
            return base.Delete(id);
        }

        public void MarkTodoStatus(string id, TodoStatus status)
        {
            var existing = Get(id);
            if (existing == null) throw new Exception($"Todo item with id {id} does not exist.");
            existing.Status = status;
            if (status == TodoStatus.completed)
            {
                existing.CompletedAt = DateTime.UtcNow;
            }
            else
            {
                existing.CompletedAt = null;
            }
            UpdateTodo(existing);
        }
    }

    public enum TodoPriority
    {
        [Description("低")]
        low,
        [Description("中")]
        medium,
        [Description("高")]
        high,
        [Description("紧急")]
        urgent
    }

    [McpServerTool]
    public class TodoTool
    {
        [McpTool, Description("创建多个新的TODO任务")]
        public static bool CreateMultiTodoItems(TodoItem[] items)
        {
            if (items == null || items.Length == 0) return false;

            foreach (var item in items)
            {
                TodoStore.Instance.AddTodo(item);
            }

            return true;
        }

        [McpTool, Description("创建一个新的TODO任务")]
        public static TodoItem CreateOneTodoItem(
                  [Required][Description("任务标题")] string title
                , [Description("任务描述")] string? description
                , [Description("任务优先级，默认为 medium")] TodoPriority? priority
                , [Description("任务标签")] string? tags
               )
        {
            if (priority == null)
            {
                priority = TodoPriority.medium;
            }

            if (tags == null) tags = string.Empty;
            var todos = TodoStore.Instance.AddTodo(new TodoItem
            {
                Title = title,
                Description = description,
                Priority = priority ?? TodoPriority.medium,
                Tags = tags ?? string.Empty,
            });

            return todos;
        }


        [McpTool, Description("列出TODO任务，支持按状态、标签和优先级筛选")]
        public static TodoItem[] TodoList(
            [Description("任务状态")] TodoStatus? status,
            [Description("任务标签")] string? tag,
            [Description("任务优先级")] TodoPriority? priority)
        {
            var qry = from t in TodoStore.Instance.Gets() select t;
            if (status != null)
            {
                qry = qry.Where(t => t.Status == status);
            }

            if (tag != null)
            {
                qry = qry.Where(t => t.Tags != null && t.Tags.Contains(tag));
            }

            if (priority != null)
            {
                qry = qry.Where(t => t.Priority == priority);
            }

            qry.OrderBy(t =>
            {
                return t.Priority switch
                {
                    TodoPriority.urgent => 0,
                    TodoPriority.high => 1,
                    TodoPriority.medium => 2,
                    TodoPriority.low => 3,
                    _ => 4
                };
            }).ThenByDescending(t => t.CreatedAt);
            var datas = qry.ToList();
            if (datas.Count == 0)
            {
                datas.Add(new TodoItem
                {

                    Id = "-1",
                    Description = "这个是空任务，表示当前查询到的任务列表是空的。",
                    Title = "没有查询到任务",
                });
            }

            return datas.ToArray();
        }


        [McpTool, Description("获取单个TODO任务的详细信息,需要传入 `id`")]
        public static TodoItem? GetTodo([Required][Description("任务ID")] string id)
        {
            return TodoStore.Instance.Get(id);
        }

        [McpTool, Description("根据任务ID更新任务信息")]
        public static TodoItem? UpdateTodo(
            [Required][Description("任务ID")] string id,
            [Description("任务状态")] TodoStatus? status,
            [Description("任务标题")] string? title,
            [Description("任务描述")] string? description,
            [Description("任务优先级")] TodoPriority? priority,
            [Description("任务标签")] string? tags)
        {
            var existing = TodoStore.Instance.Get(id);
            if (existing == null) return null;
            if (status != null) existing.Status = status.Value;
            if (title != null) existing.Title = title;
            if (description != null) existing.Description = description;
            if (priority != null) existing.Priority = priority.Value;
            if (tags != null) existing.Tags = tags;
            return TodoStore.Instance.UpdateTodo(existing);
        }

        [McpTool, Description("删除TODO任务,需要传入 `id`")]
        public static bool DeleteTodo([Required][Description("任务ID")] string id)
        {
            return TodoStore.Instance.DeleteTodo(id);
        }

        [McpTool, Description("标记TODO任务为已完成,任务id必须输入,需要传入 `id`")]
        public static TodoItem? MarkTodoComplete([Required][Description("任务ID")] string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                var datas = TodoStore.Instance.Gets();
                datas = datas.Where(x => x.Status == TodoStatus.inProgress).ToArray();
                if (datas.Length == 0) datas = datas.Where(x => x.Status == TodoStatus.pending).ToArray();
                if (datas.Length == 0)
                {
                    return null;
                }

                id = datas[0].Id;
            }

            var existing = TodoStore.Instance.Get(id);
            if (existing == null) return null;
            existing.Status = TodoStatus.completed;
            existing.CompletedAt = DateTime.UtcNow;
            return TodoStore.Instance.UpdateTodo(existing);
        }

        [McpTool, Description("标记TODO任务为进行中,任务id必须输入,需要传入 `id`")]
        public static TodoItem? MarkTodoInProgress([Required][Description("任务ID")] string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                var datas = TodoStore.Instance.Gets();
                datas = datas.Where(x => x.Status == TodoStatus.inProgress).ToArray();
                if (datas.Length == 0) datas = datas.Where(x => x.Status == TodoStatus.pending).ToArray();
                if (datas.Length == 0)
                {
                    return null;
                }

                id = datas[0].Id;
            }

            var existing = TodoStore.Instance.Get(id);
            if (existing == null) return null;
            existing.Status = TodoStatus.inProgress;
            existing.CompletedAt = DateTime.UtcNow;
            return TodoStore.Instance.UpdateTodo(existing);
        }

        [McpTool, Description("搜索TODO任务")]
        public static TodoItem[] TodoSearch(
           [Required][Description("搜索关键词")] string keyword,
                [Description("任务状态")] TodoStatus? status
            )
        {
            if (string.IsNullOrEmpty(keyword)) return TodoStore.Instance.Gets();
            var key = keyword.ToLower();
            var qry = from t in TodoStore.Instance.Gets()
                      where t.Title.ToLower().Contains(key) || (t.Description != null && t.Description.ToLower().Contains(key))
                      select t;

            if (status != null)
            {
                qry = qry.Where(t => t.Status == status);
            }

            qry = qry.OrderBy(t =>
            {
                return t.Priority switch
                {
                    TodoPriority.urgent => 0,
                    TodoPriority.high => 1,
                    TodoPriority.medium => 2,
                    TodoPriority.low => 3,
                    _ => 4
                };
            }).ThenByDescending(t => t.CreatedAt);

            return qry.ToArray();
        }

        [McpTool, Description("获取TODO统计信息")]
        public object TodoStats()
        {
            var todos = TodoStore.Instance.Gets();
            var stats = new
            {
                total = todos.Length,
                pending = todos.Count(t => t.Status == TodoStatus.pending),
                in_progress = todos.Count(t => t.Status == TodoStatus.inProgress),
                completed = todos.Count(t => t.Status == TodoStatus.completed),
                cancelled = todos.Count(t => t.Status == TodoStatus.cancelled),
                by_priority = new
                {
                    urgent = todos.Count(t => t.Priority == TodoPriority.urgent),
                    high = todos.Count(t => t.Priority == TodoPriority.high),
                    medium = todos.Count(t => t.Priority == TodoPriority.medium),
                    low = todos.Count(t => t.Priority == TodoPriority.low)
                },
                completion_rate = todos.Length > 0 ? ((double)todos.Count(t => t.Status == TodoStatus.completed) / todos.Length * 100).ToString("F2") + "%" : "0%"
            };

            return stats;
        }

    }
}
