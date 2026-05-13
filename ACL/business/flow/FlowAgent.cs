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
        private FlowExecutor executor;

        public long FlowId { get { return flowId; } }

        public FlowAgent(long flowId, Channel<string> output)
        {
            this.flowId = flowId;
            this.output = output;
        }

        public void Configure()
        {
            if (executor != null)
            {
                executor.Stop();
                return;
            }

            executor = new FlowExecutor(FlowId, output);
        }

        public void Start()
        {
            if (executor.CanRun())
            {
                executor?.Start();
            }
        }

        public void Stop()
        {
            executor?.Stop();
        }
    }
}
