using ACL.dao;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;

namespace ACL.business.flow
{
    internal class FlowAgent
    {
        private long flowId;
        private Channel<string> output;
        private FlowRuntime? flowRuntime;

        public long FlowId { get { return flowId; } }

        public FlowAgent(long flowId, Channel<string> output)
        {
            this.flowId = flowId;
            this.output = output;
        }

        public void Configure()
        {
            if (flowRuntime != null)
            {
                flowRuntime.Stop();
                return;
            }

            var store = new DataStore();
            var flowBody = store.GetFlowBody(flowId);
            var flow = new Flow(flowBody);
            var flowActionGraph = flow.Configure();
            flowRuntime = new FlowRuntime(flowActionGraph);
            flowRuntime.Configure(output);
        }

        public void Start()
        {
            flowRuntime?.Start();
        }

        public void Stop()
        {
            flowRuntime?.Stop();
        }
    }
}
