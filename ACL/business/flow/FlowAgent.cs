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
        private Channel<string> input;
        private FlowExecutor executor;

        public long FlowId { get { return flowId; } }

        public FlowAgent(long flowId, Channel<string> output, Channel<string> input)
        {
            this.flowId = flowId;
            this.output = output;
            this.input = input;
        }

        public void Configure()
        {
            if (executor != null)
            {
                executor.Stop();
                return;
            }

            executor = new FlowExecutor(FlowId, output, input);
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
