using ACL.dao;
using ACL.web;

namespace ACL.business.flow
{
    public class FlowExecutor
    {
        private DataStore dataStore;
        private long flowId;
        private Graph<AgentTask>? flowGraph;

        public FlowExecutor(long flowId)
        {
            dataStore = new DataStore();
            this.flowId = flowId;
        }

        public void Start()
        {
            var state = CanRun();
            if (!state) return;

            StartInternal();
        }

        private void StartInternal()
        {
            var flowBody = dataStore.GetFlowBody(flowId);
            var config = flowBody.Config;
            FetchGraph(flowBody);

            var runtimeBody = dataStore.GetFlowRuntimeBody(flowId);
            var runtime = runtimeBody.Runtime;
            var nodes = runtimeBody.Nodes;
            var allActions = runtimeBody.Actions;
            var runningNodes = nodes.Where(x => x.Status == NodeStates.Running).ToList();
            //TODO : run the graph with runtime and nodes, update the runtime and nodes status, save to db, and trigger the next nodes

        }


        private void FetchGraph(FlowBody flowBody)
        {
            var action = new Flow(flowBody);
            flowGraph = action.Configure();
            if (flowGraph == null) throw new InvalidFlowConfigException();
        }


        public void Stop(long flowId)
        {
            var runtime = GetFlowRuntime();
            if (runtime == null || runtime.IsOver) return;

            StopInternal(runtime);
        }

        private void StopInternal(FlowRuntime runtime)
        {
            runtime.IsOver = true;
            runtime.State = ABL.Object.EnumEntityState.Modified;
            dataStore.Save(runtime);
            //clear other resources : TODO
        }

        public bool CanRun()
        {
            var runtime = this.GetFlowRuntime();
            if (runtime == null) return true;

            if (!runtime.IsOver)
            {
                return false;
            }

            return true;
        }

        private FlowRuntime? GetFlowRuntime()
        {
            return dataStore.Fetch<FlowRuntime>(t=>t.Id == flowId && !t.IsOver);
        }
    }
}
