using ACL.business.mcp.local;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;

namespace ACL.business.agent
{
    /// <summary>
    /// simple state macheine
    /// </summary>
    public class StateMachine
    {
        private const string CONTINUE = "请继续";
        private Stack<StateTag> stack;

        public StateMachine()
        {
            stack = new Stack<StateTag>();
        }

        public StateTag Peek()
        {
            if (stack.Count == 0) return StateTag.Idle;

            return stack.Peek();
        }

        public void Push(StateTag? stateTag)
        {
            if (stateTag == null) return;

            stack.Push(stateTag.Value);
        }

        public (StateTag?, string?) Next()
        {
            var sbd = new StringBuilder();
            if (stack.Count == 0) return (null, null);
            var tag = stack.Pop();
            switch (tag)
            {
                case StateTag.Idle:
                    {
                        break;
                    }
                case StateTag.Output:
                    {
                        var datas = TodoTool.TodoList(TodoStatus.pending, null, null);
                        if (datas.Length == 0)
                        {
                            sbd.AppendLine("你必须立刻执行任务分解，并采用__LOCAL_TodoCreate创建所有任务项，如果所有任务已经完成，则不用分解任务，并执行`ALL_MISIION_FINISHED`结束。");
                        }
                        break;
                    }
                case StateTag.FnCall:
                case StateTag.FnError:
                    {
                        sbd.Append(CONTINUE);
                        break;
                    }
                case StateTag.TodoComplete:
                    {

                        var indatas = TodoTool.TodoList(TodoStatus.inProgress, null, null);
                        var datas = TodoTool.TodoList(TodoStatus.pending, null, null);
                        if (indatas.Length > 0)
                        {
                            sbd.AppendLine("你有以下任务处于进行中的状态：");
                            sbd.AppendLine(JsonConvert.SerializeObject(indatas));
                            sbd.AppendLine("请继续执行进行中`inProgress`的TODO任务，当该任务执行完成后，可以调用`__LOCAL__MarkTodoComplete，传入`id`，将已经完成的任务置为完成状态；否则，继续执行该任务。");
                        }

                        if (datas.Length > 0)
                        {
                            sbd.AppendLine("你有以下任务处于`pending`状态：");
                            sbd.AppendLine(JsonConvert.SerializeObject(datas));
                            sbd.AppendLine("请选一个状态为`pending`的TODO的高优先级任务，调用__LOCAL__MarkTodoInProgress，传入`id`，将其标记为进行中，以开始执行该任务。");
                        }
                        break;
                    }
                case StateTag.TodoUpdate:
                case StateTag.TodoCreate:
                case StateTag.TodoInprogress:
                    {
                        sbd.Append(CONTINUE);
                        break;
                    }
                case StateTag.MissionComplete:
                    {
                        return (null, null);
                    }

            }

            return (tag, sbd.ToString());
        }
    }

    public enum StateTag
    {
        Idle,
        Output,
        FnCall,
        FnError,
        TodoCreate,
        TodoUpdate,
        TodoComplete,
        TodoInprogress,
        MissionComplete
    }
}
