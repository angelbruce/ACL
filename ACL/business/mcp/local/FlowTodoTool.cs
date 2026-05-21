using System.ComponentModel;

namespace ACL.business.mcp.local
{
    public class FlowTodoStore : BasicCurd<TodoItem>
    {
        public event DgtTodoCompleted? Completed;
        public event DgtTodoInprogress? Inprogressed;

        private static FlowTodoStore instance = new FlowTodoStore();
        public static FlowTodoStore Instance { get { return instance; } }

        public TodoItem AddTodo(TodoItem item)
        {
            return base.Add(item, t =>
            {
                var now = DateTime.UtcNow;
                t.CreatedAt = now;
                t.UpdatedAt = now;

            });
        }

        public TodoItem[] GetTodoItemsByFlow(string flowId)
        {
            return datas.Values.Where(t => t.FlowId == flowId).ToArray();
        }


        public TodoItem[] GetTodoItemsByFlowNode(string flowId, string nodeId)
        {
            return datas.Values.Where(t => t.FlowId == flowId && t.NodeId == nodeId).ToArray();
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

        public void ClearFlowNodeTodos(string flowId, string nodeId)
        {
            var itemsToDelete = datas.Values.Where(t => t.FlowId == flowId && t.NodeId == nodeId).ToList();
            foreach (var item in itemsToDelete)
            {
                DeleteTodo(item.Id);
            }
        }
    }

    [McpServerTool]
    public class FlowTodoTool
    {
        [McpTool, Description("[流程相关TODO工具]:为流程节点创建一个新的TODO任务，必须传入 `flowId`,`nodeId`,`description`")]
        public static TodoItem FlowTodoCreate(
                [Required][Description("关联的流程ID")] string flowId
                , [Required][Description("关联的节点ID")] string nodeId
                , [Description("任务标题")] string title
                , [Required][Description("任务描述")] string? description
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
                FlowId = flowId,
                NodeId = nodeId
            });

            return todos;
        }


        [McpTool, Description("[流程相关TODO工具]:列出TODO任务，支持按状态、标签和优先级筛选")]
        public static TodoItem[] FlowTodoList(
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
            return qry.ToArray();
        }


        [McpTool, Description("[流程相关TODO工具]:获取单个TODO任务的详细信息,需要传入 `id`")]
        public static TodoItem? GetFlowTodo([Required][Description("任务ID")] string id)
        {
            return TodoStore.Instance.Get(id);
        }

        [McpTool, Description("[流程相关TODO工具]:根据任务ID更新任务信息")]
        public static TodoItem? UpdateFlowTodo(
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

        [McpTool, Description("[流程相关TODO工具]:删除TODO任务,需要传入 `id`")]
        public static bool DeleteFlowTodo([Required][Description("任务ID")] string id)
        {
            return TodoStore.Instance.DeleteTodo(id);
        }

        [McpTool, Description("[流程相关TODO工具]:标记TODO任务为已完成,任务id必须输入,需要传入 `id`")]
        public static TodoItem? MarkFlowTodoComplete([Required][Description("任务ID")] string id)
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

        [McpTool, Description("[流程相关TODO工具]:标记TODO任务为进行中,任务id必须输入,需要传入 `id`")]
        public static TodoItem? MarkFlowTodoInProgress([Required][Description("任务ID")] string id)
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

        [McpTool, Description("[流程相关TODO工具]:搜索TODO任务")]
        public static TodoItem[] FlowTodoSearch(
           [Required][Description("搜索关键词")] string keyword,
                [Description("任务状态")] TodoStatus? status
            )
        {
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

        [McpTool, Description("[流程相关TODO工具]:获取TODO统计信息")]
        public object FlowTodoStats()
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

        [McpTool, Description("[流程相关TODO工具]:获取指定流程的所有TODO任务必须传入`flowId`")]
        public static TodoItem[] GetFlowTodosByFlowId([Required][Description("关联的流程ID")] string flowId)
        {
            return FlowTodoStore.Instance.GetTodoItemsByFlow(flowId);
        }

        [McpTool, Description("[流程相关TODO工具]:获取指定流程节点的所有TODO任务列表，包含详细状态，必须传入`flowId`,`nodeId`")]
        public static TodoItem[] GetFlowTodosByFlowNode(
           [Required][Description("关联的流程ID")] string flowId
            , [Required][Description("关联的流程节点ID")] string nodeId)
        {
            return FlowTodoStore.Instance.GetTodoItemsByFlowNode(flowId, nodeId);
        }

        [McpTool, Description("[流程相关TODO工具]:将任务关联到流程节点，必须传入`id`,`flowId`,`nodeId`")]
        public static TodoItem? FlowTodoLinkToNode(
             [Required][Description("任务id")] string id
            , [Required][Description("关联的流程ID")] string flowId
            , [Required][Description("关联的流程节点ID")] string nodeId)
        {

            var todo = TodoStore.Instance.Get(id);
            if (todo == null) return null;

            todo.FlowId = flowId;
            todo.NodeId = nodeId;
            return TodoStore.Instance.UpdateTodo(todo);
        }


        [McpTool, Description("[流程相关TODO工具]:获取流程中未完成的任务数，必须传入`flowId`")]
        public static int GetPendingFlowTodoCountByFlowId([Required][Description("关联的流程ID")] string flowId)
        {
            var items = FlowTodoStore.Instance.GetTodoItemsByFlow(flowId);
            items = items.Where(x => x.Status == TodoStatus.pending).ToArray();
            return items.Length;
        }


        [McpTool, Description("[流程相关TODO工具]:根据流程节点id获取流程中未完成的任务数，必须传入`flowId`,`nodeId`")]
        public static int GetPendinggFlowTodoCountByFlowNode(
            [Required][Description("关联的流程ID")] string flowId,
             [Required][Description("关联的流程节点ID")] string nodeId)
        {
            var items = FlowTodoStore.Instance.GetTodoItemsByFlow(flowId);
            items = items.Where(x => x.NodeId != null && x.NodeId == nodeId && x.Status == TodoStatus.pending).ToArray();
            return items.Length;
        }



    }
}
