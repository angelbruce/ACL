using ABL;
using ABL.Store;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ACL.business.mcp.local
{
    public class UserInteractItem : IDable
    {
        public string Description { get; set; } = string.Empty;
        public string? FlowId { get; set; }
        public string? NodeId { get; set; }
    }

    public class UserInteractStore : BasicCurd<UserInteractItem>
    {
        private static UserInteractStore instance = new UserInteractStore();
        public static UserInteractStore Instance { get { return instance; } }

    }

    [McpServerTool]
    public class UserInteractTool
    {

        [McpTool, Description("告诉人类需要确认信息")]
        public static UserInteractItem HumanRequestAdd(
            [Required][Description("需要人类确认的信息")] string description,
           [Description("关联的流程ID")] string? flowId,
            [Description("关联的流程节点ID")] string? nodeId
            )
        {
            return UserInteractStore.Instance.Add(new UserInteractItem
            {
                Description = description,
                FlowId = flowId,
                NodeId = nodeId
            }, null);
        }

        [McpTool, Description("删除人类已经确认的信息")]
        public static bool HumanRequestRemove([Required][Description("信息ID")] string id)
        {
            return UserInteractStore.Instance.Delete(id);
        }


        [McpTool, Description("通过流程ID与流程节点ID删除人类已经确认的信息")]
        public static bool HumanRequestRemoveByFlowNodeId(
            [Required][Description("关联的流程ID")] string flowId,
            [Required][Description("关联的流程节点ID")] string nodeId
            )
        {
            var datas = UserInteractStore.Instance.Gets();
            datas = datas.Where(x => x.FlowId == flowId && x.NodeId == nodeId).ToArray();
            if (datas.Length == 0) return false;

            foreach (var item in datas)
            {
                UserInteractStore.Instance.Delete(item.Id);
            }

            return true;
        }
    }



}
