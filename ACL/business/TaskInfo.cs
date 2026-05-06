using ABL.Object;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.Json.Serialization;

namespace ACL.business
{

    public class TaskInfo
    {
        /// <summary>
        /// Task_ID
        /// </summary>
        [JsonProperty("Task_ID")]
        [RJson("Task_ID")]
        [Description("编号")]
        public string Id { get; set; }

        /// <summary>
        /// "Status": "RUNNING" | "FINISHED",
        /// </summary>
        [JsonProperty("Status")]
        [RJson("Status")]
        [Description("状态")]
        public string Status { get; set; }

        /// <summary>
        /// 是否需要人类参与 true/false
        /// </summary>
        [JsonProperty("RJson")]
        [RJson("NeedHumanJoinStrategy")]
        [Description("是否需要人类参与")]
        public bool NeedHumanJoinStrategy { get; set; }


        /// <summary>
        ///    "Description": "具体执行的动作描述 (e.g., Call API X with parameter Y)",
        /// </summary>
        [JsonProperty("Description")]
        [RJson("Description", "Current_Step_Description")]
        [Description("描述")]
        public string Description { get; set; }


        /// <summary>
        ///    "Description": "具体执行的动作描述 (e.g., Call API X with parameter Y)",
        /// </summary>
        [JsonProperty("Current_Step_Description")]
        [RJson("Description", "Current_Step_Description")]
        [Description("描述")]
        public string CurrentStepDescription { get; set; }


        /// <summary>
        /// Input_Data": 用于本次动作的输入数据",
        /// </summary>
        [JsonProperty("Input_Data")]
        [RJson("Input_Data")]
        [Description("输入")]
        public string InputData { get; set; }


        /// <summary>
        /// "Output_Result": 本次动作产生的结果/数据",
        /// </summary>
        [JsonProperty("Output_Result")]
        [RJson("Output_Result")]
        [Description("输出")]
        public string OutputResult { get; set; }


        [JsonProperty("Out_File")]
        [RJson("Out_File")]
        [Description("输出文件")]
        public string OutFile { get; set; }

        /// <summary>
        /// "Verification_Status": "PASSED" | "FAILED",
        /// </summary>
        [JsonProperty("Verification_Status")]
        [RJson("Verification_Status")]
        [Description("校验状态")]
        public string VerificationStatus { get; set; }

        /// <summary>
        /// Reflection_Notes": 如果失败，在这里说明失败的原因和修正方向
        /// </summary>
        [JsonProperty("Reflection_Notes")]
        [RJson("Reflection_Notes")]
        [Description("失败原因")]
        public string ReflectionNotes { get; set; }

        /// <summary>
        /// 子任务
        /// </summary>
        [JsonProperty("Sub_Tasks_List")]
        [RJson("Sub_Tasks_List")]
        public List<TaskInfo> SubTaskList { get; set; }

        /// <summary>
        /// 下一步计划
        /// </summary>
        [JsonProperty("Next_Action_Plan")]
        [RJson("Next_Action_Plan")]
        [Description("下一步计划")]
        public string NextActionPlan { get; set; }
    }
}
